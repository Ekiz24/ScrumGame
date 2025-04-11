using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SnakeTigger : MonoBehaviour
{
    [Header("游戏元素")]
    [SerializeField] private RectTransform gameArea; // 游戏区域/背景
    [SerializeField] private RectTransform snakeHead; // 蛇头
    [SerializeField] private RectTransform snakeBodyPrefab; // 蛇身预制体
    [SerializeField] private RectTransform[] foods; // 食物数组
    [SerializeField] private GameObject winPanel; // 胜利面板
    [SerializeField] private TextMeshProUGUI failText; // 失败文本

    [Header("游戏设置")]
    [SerializeField] private float moveSpeed = 10f; // 移动速度
    [SerializeField] private float gridSize = 20f; // 网格大小
    [SerializeField] private int restartDelay = 3; // 重启延迟（秒）

    // 游戏状态
    private enum Direction { Up, Down, Left, Right }
    private Direction currentDirection = Direction.Right;
    private Direction nextDirection = Direction.Right;

    private List<RectTransform> snakeBody = new List<RectTransform>();
    private List<Vector2> previousPositions = new List<Vector2>();
    private float moveTimer = 0f;
    private float moveInterval => 1f / moveSpeed;
    private int foodsEaten = 0;
    private bool isGameOver = false;
    private bool isWin = false;

    private void Start()
    {
        InitializeGame();
    }

    // 修改InitializeGame方法中蛇头的初始位置设置
    private void InitializeGame()
    {
        // 重置游戏状态
        currentDirection = Direction.Right;
        nextDirection = Direction.Right;
        foodsEaten = 0;
        isGameOver = false;
        isWin = false;
        moveTimer = 0f;

        // 隐藏UI面板
        if (winPanel != null)
            winPanel.SetActive(false);
        if (failText != null)
            failText.gameObject.SetActive(false);

        // 重置蛇的位置
        if (snakeHead != null)
        {
            // 将蛇头放置在游戏区域中央偏左的位置（考虑背景偏移）
            Vector2 areaCenter = gameArea.anchoredPosition;
            Vector2 startPos = new Vector2(areaCenter.x - gameArea.rect.width / 4, areaCenter.y);
            snakeHead.anchoredPosition = startPos;
        }

        // 清除之前的蛇身
        ClearSnakeBody();
        previousPositions.Clear();
        if (snakeHead != null)
            previousPositions.Add(snakeHead.anchoredPosition);

        // 重置食物位置
        foreach (RectTransform food in foods)
        {
            if (food != null)
                food.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (isGameOver || isWin)
            return;

        // 处理输入
        HandleInput();

        // 移动蛇
        moveTimer += Time.deltaTime;
        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;
            MoveSnake();
        }
    }

    private void HandleInput()
    {
        // 获取键盘输入
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && currentDirection != Direction.Down)
        {
            nextDirection = Direction.Up;
        }
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && currentDirection != Direction.Up)
        {
            nextDirection = Direction.Down;
        }
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && currentDirection != Direction.Right)
        {
            nextDirection = Direction.Left;
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && currentDirection != Direction.Left)
        {
            nextDirection = Direction.Right;
        }
    }

    private void MoveSnake()
    {
        // 存储蛇头上一个位置
        Vector2 previousHeadPosition = snakeHead.anchoredPosition;

        // 更新方向
        currentDirection = nextDirection;

        // 根据方向移动蛇头
        Vector2 moveDirection = Vector2.zero;
        switch (currentDirection)
        {
            case Direction.Up:
                moveDirection = new Vector2(0, gridSize);
                break;
            case Direction.Down:
                moveDirection = new Vector2(0, -gridSize);
                break;
            case Direction.Left:
                moveDirection = new Vector2(-gridSize, 0);
                break;
            case Direction.Right:
                moveDirection = new Vector2(gridSize, 0);
                break;
        }

        // 更新蛇头位置
        snakeHead.anchoredPosition += moveDirection;

        // 检查边界碰撞
        if (IsOutOfBounds(snakeHead.anchoredPosition))
        {
            GameOver(false);
            return;
        }

        // 更新位置历史
        previousPositions.Insert(0, previousHeadPosition);
        if (previousPositions.Count > snakeBody.Count + 1)
        {
            previousPositions.RemoveAt(previousPositions.Count - 1);
        }

        // 更新蛇身位置
        for (int i = 0; i < snakeBody.Count; i++)
        {
            snakeBody[i].anchoredPosition = previousPositions[i];
        }

        // 检查食物碰撞
        CheckFoodCollision();
    }

    // 修改IsOutOfBounds方法，考虑背景偏移量
    private bool IsOutOfBounds(Vector2 position)
    {
        // 获取游戏区域的一半尺寸
        float halfWidth = gameArea.rect.width / 2;
        float halfHeight = gameArea.rect.height / 2;

        // 获取背景的中心位置（考虑偏移量）
        Vector2 areaCenter = gameArea.anchoredPosition;

        // 相对于背景中心的边界
        float leftBound = areaCenter.x - halfWidth;
        float rightBound = areaCenter.x + halfWidth;
        float bottomBound = areaCenter.y - halfHeight;
        float topBound = areaCenter.y + halfHeight;

        // 检查是否超出边界
        return position.x < leftBound || position.x > rightBound ||
               position.y < bottomBound || position.y > topBound;
    }

    private void CheckFoodCollision()
    {
        // 检查和每个食物的碰撞
        for (int i = 0; i < foods.Length; i++)
        {
            if (foods[i] != null && foods[i].gameObject.activeSelf)
            {
                // 使用距离检测碰撞
                float distance = Vector2.Distance(snakeHead.anchoredPosition, foods[i].anchoredPosition);
                if (distance < gridSize)
                {
                    // 吃到食物
                    EatFood(foods[i]);
                    break;
                }
            }
        }
    }

    private void EatFood(RectTransform food)
    {
        // 禁用食物
        food.gameObject.SetActive(false);
        foodsEaten++;

        // 添加蛇身
        AddSnakeBodyPart();

        // 检查是否吃完所有食物
        if (foodsEaten >= foods.Length)
        {
            GameOver(true);
        }
    }

    // 修改AddSnakeBodyPart方法
    private void AddSnakeBodyPart()
    {
        // 创建新的蛇身部分，确保正确设置父对象和位置
        RectTransform newBodyPart = Instantiate(snakeBodyPrefab);
        newBodyPart.SetParent(transform, false); // 使用这种方式设置父对象

        // 设置初始位置
        if (snakeBody.Count == 0 && previousPositions.Count > 1)
        {
            newBodyPart.anchoredPosition = previousPositions[previousPositions.Count - 1];
        }
        else if (snakeBody.Count > 0)
        {
            // 放在最后一个蛇身后面
            newBodyPart.anchoredPosition = previousPositions[Mathf.Min(snakeBody.Count, previousPositions.Count - 1)];
        }
        else
        {
            // 防止索引越界
            newBodyPart.anchoredPosition = snakeHead.anchoredPosition;
        }

        // 添加到蛇身列表
        snakeBody.Add(newBodyPart);
    }

    private void GameOver(bool win)
    {
        isGameOver = true;
        isWin = win;

        if (win)
        {
            // 胜利
            if (winPanel != null)
                winPanel.SetActive(true);
        }
        else
        {
            // 失败
            StartCoroutine(RestartGame());
        }
    }

    private IEnumerator RestartGame()
    {
        // 显示失败文本
        if (failText != null)
        {
            failText.gameObject.SetActive(true);

            // 倒计时
            for (int i = restartDelay; i > 0; i--)
            {
                failText.text = $"You failed. The game will restart in {i}. Try harder next time!";
                yield return new WaitForSeconds(1);
            }

            failText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(restartDelay);
        }

        // 重新开始游戏
        InitializeGame();
    }

    // 修改ClearAllCards方法，确保安全销毁所有蛇身部分
    private void ClearSnakeBody()
    {
        // 安全删除所有蛇身部分
        for (int i = snakeBody.Count - 1; i >= 0; i--)
        {
            RectTransform body = snakeBody[i];
            snakeBody.RemoveAt(i);

            if (body != null)
            {
                if (Application.isPlaying)
                    Destroy(body.gameObject);
                else
                    DestroyImmediate(body.gameObject);
            }
        }

        snakeBody.Clear();
    }

    // 在OnDisable中清理资源
    private void OnDisable()
    {
        ClearSnakeBody();
    }
}
