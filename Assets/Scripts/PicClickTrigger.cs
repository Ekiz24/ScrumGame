using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PicClickTrigger : MonoBehaviour
{
    [Header("游戏元素")]
    [SerializeField] private Image gameImage; // 主游戏图片
    [SerializeField] private Button[] hiddenButtons; // 透明按钮数组
    [SerializeField] private Image redCircleTemplate; // 红圈图片模板
    [SerializeField] private GameObject winScreen; // 获胜按钮

    [Header("游戏设置")]
    [SerializeField] private float winDelay = 0.5f; // 胜利延迟时间

    private int totalButtons; // 按钮总数
    private int clickedButtons = 0; // 已点击的按钮数
    private Image[] redCircles; // 红圈实例数组

    private void OnEnable()
    {
        // 初始化游戏
        InitializeGame();
    }

    private void InitializeGame()
    {
        // 确保游戏图片可见
        if (gameImage != null)
            gameImage.gameObject.SetActive(true);

        // 隐藏胜利按钮
        if (winScreen != null)
            winScreen.gameObject.SetActive(false);

        // 初始化按钮计数
        totalButtons = hiddenButtons.Length;
        clickedButtons = 0;

        // 创建红圈数组
        redCircles = new Image[totalButtons];

        // 如果红圈模板存在，则隐藏它
        if (redCircleTemplate != null)
            redCircleTemplate.gameObject.SetActive(false);

        // 设置所有按钮的点击事件
        for (int i = 0; i < hiddenButtons.Length; i++)
        {
            if (hiddenButtons[i] != null)
            {
                // 移除所有现有的点击事件监听器，避免重复
                hiddenButtons[i].onClick.RemoveAllListeners();

                // 添加新的点击事件监听器
                int buttonIndex = i; // 需要为闭包创建局部变量
                hiddenButtons[i].onClick.AddListener(() => OnButtonClicked(buttonIndex));

                // 确保按钮可交互并可见
                hiddenButtons[i].interactable = true;
                hiddenButtons[i].gameObject.SetActive(true);
            }
        }

        Debug.Log("游戏初始化完成，共有 " + totalButtons + " 个需要找出的不合理之处");
    }

    private void OnButtonClicked(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= hiddenButtons.Length)
            return;

        Debug.Log("点击了按钮 #" + buttonIndex);

        // 禁用按钮，防止重复点击
        hiddenButtons[buttonIndex].interactable = false;

        // 在按钮位置创建红圈
        CreateRedCircle(buttonIndex);

        // 增加点击计数
        clickedButtons++;

        // 检查是否所有按钮都被点击
        if (clickedButtons >= totalButtons)
        {
            // 启动胜利流程
            StartCoroutine(WinSequence());
        }
    }

    private void CreateRedCircle(int buttonIndex)
    {
        // 如果已经有红圈在这个位置，则返回
        if (redCircles[buttonIndex] != null)
            return;

        // 在按钮位置创建红圈
        if (redCircleTemplate != null)
        {
            // 实例化红圈
            Image newRedCircle = Instantiate(redCircleTemplate, hiddenButtons[buttonIndex].transform.position, Quaternion.identity, transform);

            // 设置红圈的位置和尺寸
            RectTransform redCircleRT = newRedCircle.GetComponent<RectTransform>();
            RectTransform buttonRT = hiddenButtons[buttonIndex].GetComponent<RectTransform>();

            // 匹配按钮的尺寸
            redCircleRT.sizeDelta = buttonRT.sizeDelta;
            redCircleRT.anchoredPosition = buttonRT.anchoredPosition;

            // 激活红圈
            newRedCircle.gameObject.SetActive(true);

            // 存储红圈引用
            redCircles[buttonIndex] = newRedCircle;
        }
    }

    private IEnumerator WinSequence()
    {
        Debug.Log("所有不合理之处都被找到了！");

        // 等待指定的延迟时间
        yield return new WaitForSeconds(winDelay);

        // 隐藏游戏图片
        if (gameImage != null)
            gameImage.gameObject.SetActive(false);

        // 隐藏所有按钮
        foreach (Button button in hiddenButtons)
        {
            if (button != null)
                button.gameObject.SetActive(false);
        }

        // 隐藏所有红圈
        foreach (Image redCircle in redCircles)
        {
            if (redCircle != null)
                redCircle.gameObject.SetActive(false);
        }

        // 显示获胜按钮
        if (winScreen != null)
            winScreen.gameObject.SetActive(true);

        Debug.Log("游戏完成！");
    }
}
