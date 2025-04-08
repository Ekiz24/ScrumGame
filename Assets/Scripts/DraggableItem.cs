using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

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
        if (itemData.isCompleted)
        {
            background.color = completedColor;
        }
        else if (itemData.isSelected)
        {
            background.color = selectedColor;
        }
        else
        {
            background.color = normalColor;
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

        // 首先检查是否拖到了另一个物体上（优先级最高）
        DraggableItem targetItem = FindDraggableItemUnderPosition(eventData.position);
        if (targetItem != null && targetItem != this && !targetItem.itemData.isCompleted)
        {
            // 如果当前物体是选中状态，取消选中
            if (itemData.isSelected)
            {
                gameManager.SetItemSelected(itemData.itemId, false);
            }

            gameManager.SwapItems(itemData.itemId, targetItem.itemData.itemId);
            Destroy(gameObject);
            return;
        }

        // 如果没有拖到物体上，再检查是否拖到了容器上
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
            Transform hitTransform = hitObject.transform;

            // 寻找容器
            Transform targetContainer = FindContainerInHierarchy(hitTransform);

            // 处理拖放到容器的情况
            if (targetContainer != null)
            {
                if (targetContainer == gameManager.selectionContainer)
                {
                    HandleDropToSelectionContainer();
                    return;
                }
                else if (targetContainer == gameManager.highPriorityContainer)
                {
                    HandleDirectDropToPriorityContainer("高优先级");
                    return;
                }
                else if (targetContainer == gameManager.mediumPriorityContainer)
                {
                    HandleDirectDropToPriorityContainer("中优先级");
                    return;
                }
                else if (targetContainer == gameManager.lowPriorityContainer)
                {
                    HandleDirectDropToPriorityContainer("低优先级");
                    return;
                }
                else if (targetContainer == gameManager.noPriorityContainer)
                {
                    HandleDirectDropToPriorityContainer("无优先级");
                    return;
                }
            }
        }

        // 如果没有拖到有效位置，返回原位置
        ReturnToOriginalPosition();
    }

    // 辅助方法：在层级中查找容器
    private Transform FindContainerInHierarchy(Transform start)
    {
        Transform current = start;
        int searchDepth = 0;

        while (current != null && searchDepth < 5)
        {
            // 检查是否是选择容器或优先级容器
            if (current == gameManager.selectionContainer ||
                current == gameManager.highPriorityContainer ||
                current == gameManager.mediumPriorityContainer ||
                current == gameManager.lowPriorityContainer ||
                current == gameManager.noPriorityContainer)
            {
                return current;
            }

            current = current.parent;
            searchDepth++;
        }

        return null;
    }

    // 辅助方法：在层级中查找可拖动物体
    private DraggableItem FindDraggableItemInHierarchy(Transform start)
    {
        Transform current = start;
        int searchDepth = 0;

        while (current != null && searchDepth < 5)
        {
            DraggableItem item = current.GetComponent<DraggableItem>();
            if (item != null && item != this)
            {
                return item;
            }

            current = current.parent;
            searchDepth++;
        }

        return null;
    }

    // 通过射线查找指定位置下的DraggableItem
    private DraggableItem FindDraggableItemUnderPosition(Vector2 screenPosition)
    {
        // 创建射线
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 搜索所有结果，查找第一个不是自己的DraggableItem
        foreach (RaycastResult result in results)
        {
            if (result.gameObject != null)
            {
                // 先尝试直接获取DraggableItem组件
                DraggableItem item = result.gameObject.GetComponent<DraggableItem>();
                if (item != null && item != this)
                    return item;

                // 如果没找到，尝试递归查找父物体
                item = FindDraggableItemInHierarchy(result.gameObject.transform);
                if (item != null && item != this)
                    return item;
            }
        }

        return null;
    }

    // 处理拖放到选择容器
    private void HandleDropToSelectionContainer()
    {
        // 检查已选中数量限制
        int selectedCount = gameManager.GetSelectedItemsCount();
        if (selectedCount >= 3 && !itemData.isSelected)
        {
            Debug.Log($"已达到最大选中数量(3个)，不能再将物体 {itemData.itemId} 拖入选择区");
            ReturnToOriginalPosition();
            return;
        }

        // 设置为选中状态
        bool success = gameManager.SetItemSelected(itemData.itemId, true);
        if (success)
        {
            Destroy(gameObject);
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    // 直接拖到容器中（不是拖到物体上）
    private void HandleDirectDropToPriorityContainer(string priority)
    {
        // 只有直接添加到不同容器才检查容器满的限制
        bool isChangingPriority = priority != itemData.priority;

        if (isChangingPriority && !itemData.isSelected)
        {
            // 检查目标容器物体数量
            int count = gameManager.GetItemCountByPriority(priority);
            if (count >= 3)
            {
                Debug.Log($"{priority}容器已满(3个)，请拖到具体物体上进行交换");
                ReturnToOriginalPosition();
                return;
            }
        }

        // 处理从选中状态拖回的情况
        if (itemData.isSelected)
        {
            gameManager.SetItemSelected(itemData.itemId, false);
        }

        // 更新优先级
        if (isChangingPriority)
        {
            gameManager.UpdateItemPriority(itemData.itemId, priority);
        }
        else
        {
            // 如果只是在同一容器内调整位置，不改变优先级
            gameManager.SaveData();
            gameManager.UpdateUI();
        }

        // 显示当前任务状态和优先级
        Debug.Log($"任务 {itemData.itemId}: 优先级={priority}, 状态={(itemData.isSelected ? "已选中" : itemData.isCompleted ? "已完成" : "未选中")}");

        Destroy(gameObject);
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
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
    }

}