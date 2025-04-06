using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private GameManager gameManager;

    [HideInInspector] public ItemData itemData;

    public TextMeshProUGUI itemText; // 物体上显示的文本
    public Image background;         // 物体的背景

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color completedColor = Color.green;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Initialize(ItemData data, GameManager manager)
    {
        itemData = data;
        gameManager = manager;

        // 设置显示文本
        itemText.text = data.itemId;
        UpdateVisualState();
    }

    void UpdateVisualState()
    {
        Debug.Log($"更新物体 {itemData.itemId} 状态: 已完成={itemData.isCompleted}, 已选中={itemData.isSelected}");

        if (itemData.isCompleted)
        {
            background.color = completedColor;
            Debug.Log($"设置物体 {itemData.itemId} 为完成颜色");
        }
        else if (itemData.isSelected)
        {
            background.color = selectedColor;
            Debug.Log($"设置物体 {itemData.itemId} 为选中颜色");
        }
        else
        {
            background.color = normalColor;
            Debug.Log($"设置物体 {itemData.itemId} 为正常颜色");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 如果物体已完成，不允许拖动
        if (itemData.isCompleted)
            return;

        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // 降低透明度，表示正在拖动
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // 将物体提到最前面
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemData.isCompleted)
            return;

        // 跟随鼠标移动
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemData.isCompleted)
            return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        bool exchangeSuccessful = false;

        // 处理拖放目标
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;

            // 向上查找直到找到DraggableItem或PriorityContainer
            Transform currentTransform = hitObject.transform;
            DraggableItem targetItem = null;
            PriorityContainer targetContainer = null;

            // 向上递归查找父物体，最多尝试5层
            int searchDepth = 0;
            while (currentTransform != null && searchDepth < 5)
            {
                targetItem = currentTransform.GetComponent<DraggableItem>();
                if (targetItem != null && targetItem != this) break;

                targetContainer = currentTransform.GetComponent<PriorityContainer>();
                if (targetContainer != null) break;

                currentTransform = currentTransform.parent;
                searchDepth++;
            }

            if (targetItem != null && targetItem != this && !targetItem.itemData.isCompleted)
            {
                // 与另一个物体交换位置
                gameManager.SwapItems(itemData.itemId, targetItem.itemData.itemId);
                // 交换后销毁当前拖动的物体，因为UI已经被重新创建
                Destroy(gameObject);
                return;
            }
            else if (targetContainer != null)
            {
                // 找出容器中与鼠标位置最接近的物体
                DraggableItem closestItem = FindClosestItem(targetContainer.transform, eventData.position);

                if (closestItem != null && !closestItem.itemData.isCompleted)
                {
                    // 与最近的物体交换位置
                    gameManager.SwapItems(itemData.itemId, closestItem.itemData.itemId);
                    Destroy(gameObject);
                    return;
                }
                // 如果没找到可交换的物体，并且容器未满，则直接添加到容器
                else if (targetContainer.transform.childCount < 3)
                {
                    gameManager.UpdateItemPriority(itemData.itemId, targetContainer.priorityLevel);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        // 如果没有拖到有效位置，返回原位置
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }

    // 添加这个新方法来找出容器中与给定位置最接近的物体
    private DraggableItem FindClosestItem(Transform container, Vector2 position)
    {
        DraggableItem closestItem = null;
        float closestDistance = float.MaxValue;

        // 记录鼠标位置，用于调试
        Debug.Log($"鼠标位置: {position}");

        // 遍历容器中的所有子物体
        foreach (Transform child in container)
        {
            DraggableItem item = child.GetComponent<DraggableItem>();
            if (item != null && item != this && !item.itemData.isCompleted)
            {
                // 获取物体的中心位置
                RectTransform itemRect = item.GetComponent<RectTransform>();
                Vector3 itemPos = itemRect.position;

                // 计算物体位置与鼠标位置的距离
                float distance = Vector2.Distance(position, itemPos);
                Debug.Log($"物体: {item.itemData.itemId}, 位置: {itemPos}, 距离: {distance}");

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = item;
                }
            }
        }

        // 调试输出最近的物体
        if (closestItem != null)
        {
            Debug.Log($"最近的物体是: {closestItem.itemData.itemId}, 距离: {closestDistance}");
        }

        return closestItem;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 检查是否处于复原模式
        if (gameManager.IsInResetMode())
        {
            // 在复原模式下，只有已完成的物体可以被恢复
            if (itemData.isCompleted)
            {
                gameManager.ResetItemState(itemData.itemId);
                return;
            }
        }

        // 正常模式下的行为
        // 如果物体已完成，不允许再选中
        if (itemData.isCompleted)
            return;

        // 只有在规划模式下才能选中/取消选中任务
        if (!gameManager.IsInPlanningMode())
        {
            Debug.Log("当前不在规划模式，无法选择任务。请先点击规划按钮。");
            // 可以添加提示效果
            //StartCoroutine(FlashInfo());
            return;
        }

        // 切换物体的选中状态
        bool selectionChanged = gameManager.ToggleItemSelection(itemData.itemId);

        if (selectionChanged)
        {
            // 从GameManager获取最新状态并更新视觉效果
            ItemData updatedData = gameManager.GetItemData(itemData.itemId);
            if (updatedData != null)
            {
                itemData = updatedData;
                UpdateVisualState();
            }
        }
        else
        {
            // 选择失败，可以添加提示效果
            Debug.Log("已达到最大选择数量(3个)，请先取消选择其他物体");
            // 可以添加UI反馈，比如闪烁效果
            StartCoroutine(FlashWarning());
        }
    }

    // 添加闪烁效果作为警告
    private IEnumerator FlashWarning()
    {
        Color originalColor = background.color;

        // 闪烁3次
        for (int i = 0; i < 3; i++)
        {
            background.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            background.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // // 添加信息提示闪烁效果:不能选中，未在规划模式
    // private IEnumerator FlashInfo()
    // {
    //     Color originalColor = background.color;

    //     // 闪烁2次蓝色表示信息提示
    //     for (int i = 0; i < 2; i++)
    //     {
    //         background.color = Color.blue;
    //         yield return new WaitForSeconds(0.1f);
    //         background.color = originalColor;
    //         yield return new WaitForSeconds(0.1f);
    //     }
    // }
}