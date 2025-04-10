using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JigsawPuzzleManager : MonoBehaviour
{
    [Header("拼图碎片")]
    public RectTransform pieceA;
    public RectTransform pieceB;
    public RectTransform pieceC;
    public RectTransform pieceD;
    public RectTransform pieceE;
    public RectTransform pieceF;
    public RectTransform pieceG;
    public RectTransform pieceH;
    public RectTransform pieceI;

    [Header("放置位置")]
    public RectTransform slotA;
    public RectTransform slotB;
    public RectTransform slotC;
    public RectTransform slotD;
    public RectTransform slotE;
    public RectTransform slotF;
    public RectTransform slotG;
    public RectTransform slotH;
    public RectTransform slotI;

    [Header("UI元素")]
    public GameObject victoryPanel;
    public Image backgroundImage;

    [Header("设置")]
    public float snapDistance = 50f; // 吸附距离

    // 存储碎片的初始位置
    private Dictionary<RectTransform, Vector2> originalPositions = new Dictionary<RectTransform, Vector2>();
    // 存储碎片的正确插槽
    private Dictionary<RectTransform, RectTransform> correctSlots = new Dictionary<RectTransform, RectTransform>();
    // 存储当前碎片所在的插槽
    private Dictionary<RectTransform, RectTransform> currentSlots = new Dictionary<RectTransform, RectTransform>();

    // 当前被拖动的碎片
    private RectTransform draggedPiece = null;

    void Start()
    {
        // 隐藏胜利面板
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        // 初始化碎片
        InitializePuzzlePieces();

        // 初始化正确的碎片-插槽配对
        SetupCorrectSlots();

        // 为所有碎片添加拖拽功能
        SetupDragHandlers();
    }

    private void InitializePuzzlePieces()
    {
        // 记录所有碎片的初始位置
        StoreOriginalPosition(pieceA);
        StoreOriginalPosition(pieceB);
        StoreOriginalPosition(pieceC);
        StoreOriginalPosition(pieceD);
        StoreOriginalPosition(pieceE);
        StoreOriginalPosition(pieceF);
        StoreOriginalPosition(pieceG);
        StoreOriginalPosition(pieceH);
        StoreOriginalPosition(pieceI);
    }

    private void StoreOriginalPosition(RectTransform piece)
    {
        if (piece != null)
        {
            originalPositions[piece] = piece.anchoredPosition;
            currentSlots[piece] = null; // 初始时没有插槽
        }
    }

    private void SetupCorrectSlots()
    {
        // 设置每个碎片对应的正确插槽
        correctSlots[pieceA] = slotA;
        correctSlots[pieceB] = slotB;
        correctSlots[pieceC] = slotC;
        correctSlots[pieceD] = slotD;
        correctSlots[pieceE] = slotE;
        correctSlots[pieceF] = slotF;
        correctSlots[pieceG] = slotG;
        correctSlots[pieceH] = slotH;
        correctSlots[pieceI] = slotI;
    }

    private void SetupDragHandlers()
    {
        // 为所有碎片添加拖拽组件
        AddDragHandler(pieceA);
        AddDragHandler(pieceB);
        AddDragHandler(pieceC);
        AddDragHandler(pieceD);
        AddDragHandler(pieceE);
        AddDragHandler(pieceF);
        AddDragHandler(pieceG);
        AddDragHandler(pieceH);
        AddDragHandler(pieceI);
    }

    private void AddDragHandler(RectTransform piece)
    {
        if (piece != null)
        {
            // 确保有Image组件用于接收事件
            if (piece.GetComponent<Image>() == null)
            {
                piece.gameObject.AddComponent<Image>();
                piece.GetComponent<Image>().color = new Color(0, 0, 0, 0); // 透明色
            }

            // 添加拖拽处理组件
            PieceDragHandler dragHandler = piece.gameObject.AddComponent<PieceDragHandler>();
            dragHandler.Initialize(this, piece);
        }
    }

    // 碎片开始拖动时调用
    public void OnPieceBeginDrag(RectTransform piece)
    {
        draggedPiece = piece;

        // 如果碎片已经在插槽中，将其移除
        if (currentSlots[piece] != null)
        {
            currentSlots[piece] = null;
        }

        // 将正在拖动的碎片移到前面(设置为最后一个兄弟节点)
        piece.SetAsLastSibling();
    }

    // 碎片拖动中调用
    public void OnPieceDrag(RectTransform piece, Vector2 position)
    {
        if (piece != null)
        {
            piece.anchoredPosition = position;
        }
    }

    // 碎片拖动结束时调用
    public void OnPieceEndDrag(RectTransform piece)
    {
        if (piece != null)
        {
            // 寻找最近的插槽
            RectTransform nearestSlot = FindNearestSlot(piece);

            if (nearestSlot != null && Vector2.Distance(piece.anchoredPosition, nearestSlot.anchoredPosition) <= snapDistance)
            {
                // 检查插槽是否已被占用
                bool slotOccupied = IsSlotOccupied(nearestSlot, piece);

                if (!slotOccupied)
                {
                    // 将碎片吸附到插槽位置
                    piece.anchoredPosition = nearestSlot.anchoredPosition;
                    currentSlots[piece] = nearestSlot;

                    // 检查是否完成拼图
                    CheckVictory();
                }
                else
                {
                    // 插槽已被占用，返回原位
                    ReturnToOriginalPosition(piece);
                }
            }
            else
            {
                // 不在任何插槽附近，返回原位
                ReturnToOriginalPosition(piece);
            }

            draggedPiece = null;
        }
    }

    // 查找最近的插槽
    private RectTransform FindNearestSlot(RectTransform piece)
    {
        RectTransform[] allSlots = { slotA, slotB, slotC, slotD, slotE, slotF, slotG, slotH, slotI };

        RectTransform nearestSlot = null;
        float minDistance = float.MaxValue;

        foreach (RectTransform slot in allSlots)
        {
            if (slot != null)
            {
                float distance = Vector2.Distance(piece.anchoredPosition, slot.anchoredPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestSlot = slot;
                }
            }
        }

        return nearestSlot;
    }

    // 检查插槽是否已被占用
    private bool IsSlotOccupied(RectTransform slot, RectTransform currentPiece)
    {
        foreach (var entry in currentSlots)
        {
            if (entry.Key != currentPiece && entry.Value == slot)
            {
                return true;
            }
        }

        return false;
    }

    // 让碎片返回原位
    private void ReturnToOriginalPosition(RectTransform piece)
    {
        if (originalPositions.ContainsKey(piece))
        {
            piece.anchoredPosition = originalPositions[piece];
            currentSlots[piece] = null;
        }
    }

    // 检查胜利条件
    private void CheckVictory()
    {
        bool allCorrect = true;

        // 检查所有碎片是否都在正确位置
        foreach (var entry in correctSlots)
        {
            RectTransform piece = entry.Key;
            RectTransform correctSlot = entry.Value;

            if (currentSlots[piece] != correctSlot)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            // 拼图完成，显示胜利面板
            StartCoroutine(ShowVictoryPanelWithDelay(0.5f));
        }
    }

    // 延迟显示胜利面板
    private IEnumerator ShowVictoryPanelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }

    // 重置拼图
    public void ResetPuzzle()
    {
        // 将所有碎片返回原位
        foreach (var entry in originalPositions)
        {
            RectTransform piece = entry.Key;
            Vector2 originalPos = entry.Value;

            piece.anchoredPosition = originalPos;
            currentSlots[piece] = null;
        }

        // 隐藏胜利面板
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }
}

// 处理单个碎片的拖拽
public class PieceDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private JigsawPuzzleManager puzzleManager;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public void Initialize(JigsawPuzzleManager manager, RectTransform piece)
    {
        puzzleManager = manager;
        rectTransform = piece;

        // 获取Canvas - 修改为查找根Canvas
        canvas = piece.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = GameObject.FindFirstObjectByType<Canvas>();
            Debug.LogWarning("没有找到父Canvas，使用场景中的第一个Canvas");
        }

        // 添加CanvasGroup
        canvasGroup = rectTransform.gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 减小透明度表示正在拖动
        canvasGroup.alpha = 0.6f;
        // 阻止射线检测，这样鼠标下的其他UI元素也能接收事件
        canvasGroup.blocksRaycasts = false;

        puzzleManager.OnPieceBeginDrag(rectTransform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null)
            return;

        // 直接使用鼠标的delta移动
        Vector2 delta = eventData.delta;

        // 考虑Canvas缩放
        if (canvas != null && canvas.scaleFactor != 0)
            delta /= canvas.scaleFactor;

        // 移动拼图碎片
        rectTransform.anchoredPosition += delta;

        // 告知manager（可选，因为我们已经直接修改了位置）
        puzzleManager.OnPieceDrag(rectTransform, rectTransform.anchoredPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 恢复正常透明度
        canvasGroup.alpha = 1f;
        // 恢复射线检测
        canvasGroup.blocksRaycasts = true;

        puzzleManager.OnPieceEndDrag(rectTransform);
    }
}
