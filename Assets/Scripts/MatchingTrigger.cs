using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingTrigger : MonoBehaviour
{
    [System.Serializable]
    public class MatchingCardData
    {
        public Sprite cardImage;
    }

    [Header("游戏设置")]
    [SerializeField] private MatchingCardData[] cardTypes = new MatchingCardData[8]; // 改为8种不同的卡片图片
    [SerializeField] private Button completionButton; // 完成按钮
    [SerializeField] private RectTransform backgroundTransform; // 背景对象

    [Header("卡片设置")]
    [SerializeField] private float cardSize = 100f; // 卡片边长
    [SerializeField] private float cardSpacing = 20f; // 卡片间隔
    [SerializeField] private GameObject cardPrefab; // 卡片预制体
    [SerializeField] private float yOffset = 0f; // Y轴偏移量，避免被界面元素遮挡

    // 游戏状态
    private List<MatchingCard> allCards = new List<MatchingCard>();
    private MatchingCard firstSelected = null;
    private bool canSelect = true;

    private void OnEnable()
    {
        // 隐藏完成按钮
        if (completionButton != null)
            completionButton.gameObject.SetActive(false);

        // 清除之前的卡片
        ClearAllCards();

        // 创建新游戏
        StartCoroutine(CreateGame());
    }

    private void ClearAllCards()
    {
        foreach (MatchingCard card in allCards)
        {
            if (card != null && card.gameObject != null)
                Destroy(card.gameObject);
        }
        allCards.Clear();
        firstSelected = null;
    }

    private IEnumerator CreateGame()
    {
        yield return new WaitForEndOfFrame(); // 等待一帧确保背景尺寸已更新

        // 创建卡片数据 - 8种卡片，每种出现2次，组成8对
        List<int> cardIndices = new List<int>();
        for (int i = 0; i < 8; i++) // 8种卡片类型
        {
            for (int j = 0; j < 2; j++) // 每种卡片出现2次，形成8对
            {
                cardIndices.Add(i);
            }
        }

        // 洗牌算法，确保卡片随机分布
        ShuffleList(cardIndices);

        // 计算起始位置（以背景为中心）
        Vector2 backgroundSize = backgroundTransform.rect.size;
        float totalWidth = 4 * cardSize + 3 * cardSpacing; // 4列卡片宽度
        float totalHeight = 4 * cardSize + 3 * cardSpacing; // 4行卡片高度

        float startX = -totalWidth / 2 + cardSize / 2;
        float startY = totalHeight / 2 - cardSize / 2 + yOffset; // 添加Y轴偏移

        // 创建卡片
        int index = 0;
        for (int row = 0; row < 4; row++) // 4行
        {
            for (int col = 0; col < 4; col++) // 4列
            {
                if (index < cardIndices.Count)
                {
                    CreateCard(cardIndices[index], row, col, startX, startY);
                    index++;
                }
            }
        }
    }

    private void CreateCard(int cardTypeIndex, int row, int col, float startX, float startY)
    {
        // 检查图片是否存在
        if (cardTypeIndex >= 0 && cardTypeIndex < cardTypes.Length && cardTypes[cardTypeIndex].cardImage == null)
        {
            Debug.LogWarning($"卡片类型 {cardTypeIndex} 没有设置图片!");
        }

        // 实例化卡片预制体
        GameObject cardObj = Instantiate(cardPrefab, transform);
        RectTransform rectTransform = cardObj.GetComponent<RectTransform>();

        // 设置卡片位置和大小
        rectTransform.sizeDelta = new Vector2(cardSize, cardSize);
        float posX = startX + col * (cardSize + cardSpacing);
        float posY = startY - row * (cardSize + cardSpacing);
        rectTransform.anchoredPosition = new Vector2(posX, posY);

        // 设置卡片数据
        MatchingCard card = cardObj.AddComponent<MatchingCard>();
        card.Initialize(cardTypes[cardTypeIndex].cardImage, cardTypeIndex, this);

        // 添加到卡片列表
        allCards.Add(card);
    }

    // 处理卡片选择
    public void OnCardSelected(MatchingCard card)
    {
        if (!canSelect || card.IsMatched)
            return;

        // 如果是第一张卡，记录并等待第二张
        if (firstSelected == null)
        {
            firstSelected = card;
            card.Flip(true);
            return;
        }

        // 如果选择了同一张卡，忽略
        if (firstSelected == card)
            return;

        // 第二张卡
        card.Flip(true);

        // 检查是否匹配
        if (firstSelected.CardTypeIndex == card.CardTypeIndex)
        {
            // 匹配成功
            StartCoroutine(HandleMatchSuccess(firstSelected, card));
        }
        else
        {
            // 匹配失败
            StartCoroutine(HandleMatchFailure(firstSelected, card));
        }

        firstSelected = null;
    }

    private IEnumerator HandleMatchSuccess(MatchingCard card1, MatchingCard card2)
    {
        canSelect = false;
        yield return new WaitForSeconds(0.5f);

        // 标记为已匹配并隐藏
        card1.SetMatched(true);
        card2.SetMatched(true);

        // 检查游戏是否完成
        CheckGameCompletion();

        canSelect = true;
    }

    private IEnumerator HandleMatchFailure(MatchingCard card1, MatchingCard card2)
    {
        canSelect = false;
        yield return new WaitForSeconds(1f);

        // 翻回
        card1.Flip(false);
        card2.Flip(false);

        canSelect = true;
    }

    // 修改CheckGameCompletion方法
    private void CheckGameCompletion()
    {
        bool allMatched = true;
        foreach (MatchingCard card in allCards)
        {
            if (!card.IsMatched)
            {
                allMatched = false;
                break;
            }
        }

        if (allMatched)
        {
            // 游戏完成
            Debug.Log("连连看游戏完成！");

            // 销毁所有卡片预制体并在延迟后显示完成按钮
            StartCoroutine(DestroyAllCardsWithDelay(0.5f));

            // 移除这行代码，改为在协程中执行
            // if (completionButton != null)
            //     completionButton.gameObject.SetActive(true);
        }
    }

    // 修改延迟销毁所有卡片的协程
    private IEnumerator DestroyAllCardsWithDelay(float delay)
    {
        // 等待指定的延迟时间
        yield return new WaitForSeconds(delay);

        // 销毁所有卡片
        foreach (MatchingCard card in allCards)
        {
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }

        // 清空卡片列表
        allCards.Clear();
        firstSelected = null;

        // 在延迟后显示完成按钮
        if (completionButton != null)
            completionButton.gameObject.SetActive(true);
    }

    // 辅助方法：随机洗牌列表
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            int r = i + Random.Range(0, n - i);
            T temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }
}

// 卡片类
public class MatchingCard : MonoBehaviour
{
    private Sprite cardImage;
    private int cardTypeIndex;
    private Image imageComponent;
    private MatchingTrigger gameManager;
    private Sprite backImage; // 卡片背面图片

    public bool IsMatched { get; private set; }
    public bool IsFaceUp { get; private set; }
    public int CardTypeIndex => cardTypeIndex;

    public void Initialize(Sprite image, int typeIndex, MatchingTrigger manager)
    {
        cardImage = image;
        cardTypeIndex = typeIndex;
        gameManager = manager;
        IsMatched = false;
        IsFaceUp = false;

        // 获取图像组件
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
            imageComponent = gameObject.AddComponent<Image>();

        // 保存背面图片
        backImage = imageComponent.sprite;

        // 如果cardImage为空，设置一个默认的颜色
        if (cardImage == null)
        {
            Debug.LogWarning($"卡片 {typeIndex} 的图片为空!");
            // 创建一个随机颜色作为替代
            Color randomColor = new Color(
                Random.Range(0.5f, 1.0f),
                Random.Range(0.5f, 1.0f),
                Random.Range(0.5f, 1.0f)
            );
            cardImage = CreateColorSprite(randomColor);
        }

        // 添加点击事件
        Button button = GetComponent<Button>();
        if (button == null)
            button = gameObject.AddComponent<Button>();

        button.onClick.AddListener(OnClick);
    }

    public void Flip(bool faceUp)
    {
        IsFaceUp = faceUp;
        imageComponent.sprite = faceUp ? cardImage : backImage;
    }

    public void SetMatched(bool matched)
    {
        IsMatched = matched;
        if (matched)
        {
            // 淡出效果
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0.5f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnClick()
    {
        if (!IsMatched && !IsFaceUp)
        {
            gameManager.OnCardSelected(this);
        }
    }

    // 添加此辅助方法来创建纯色Sprite
    private Sprite CreateColorSprite(Color color)
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = color;

        texture.SetPixels(colors);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
}
