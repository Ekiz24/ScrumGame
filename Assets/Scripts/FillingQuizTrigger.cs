using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FillingQuizTrigger : MonoBehaviour
{
    [Header("题目设置")]
    [TextArea(3, 5)]
    [SerializeField] private string questionText = "请输入答案:";
    [SerializeField] private string correctAnswer = ""; // 正确答案

    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI questionDisplay; // 题目文本
    [SerializeField] private TMP_InputField answerInputField; // 答案输入框
    [SerializeField] private Button checkButton; // 检查按钮
    [SerializeField] private Image checkButtonImage; // 检查按钮图片

    [Header("错误反馈设置")]
    [SerializeField] private Color errorColor = Color.red; // 错误提示颜色
    [SerializeField] private int flashCount = 3; // 闪烁次数
    [SerializeField] private float flashDuration = 0.3f; // 闪烁持续时间

    [Header("完成后隐藏")]
    [SerializeField] private GameObject[] hideOnComplete; // 完成后隐藏的对象

    private Color originalButtonColor;
    private bool isChecking = false;

    void Start()
    {
        // 初始化UI
        if (questionDisplay != null)
        {
            questionDisplay.text = questionText;
        }

        if (checkButtonImage != null)
        {
            originalButtonColor = checkButtonImage.color;
        }

        // 设置按钮点击事件
        if (checkButton != null)
        {
            checkButton.onClick.AddListener(CheckAnswer);
        }

        // 初始化输入框
        if (answerInputField != null)
        {
            answerInputField.text = "";
        }
    }

    public void CheckAnswer()
    {
        if (isChecking || answerInputField == null)
            return;

        string userAnswer = answerInputField.text.Trim();

        // 检查答案是否正确
        bool isCorrect = string.Equals(userAnswer, correctAnswer, System.StringComparison.OrdinalIgnoreCase);

        if (isCorrect)
        {
            // 答案正确，隐藏父对象
            Debug.Log("答案正确！");
            gameObject.SetActive(false);
            if (hideOnComplete != null)
            {
                foreach (GameObject obj in hideOnComplete)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }
            }
        }
        else
        {
            // 答案错误，闪烁按钮
            Debug.Log("答案错误，请重试");
            StartCoroutine(FlashButtonCoroutine());
        }
    }

    private IEnumerator FlashButtonCoroutine()
    {
        isChecking = true;

        // 禁用按钮交互
        if (checkButton != null)
        {
            checkButton.interactable = false;
        }

        // 闪烁按钮
        for (int i = 0; i < flashCount; i++)
        {
            // 变为错误颜色
            if (checkButtonImage != null)
            {
                checkButtonImage.color = errorColor;
            }
            yield return new WaitForSeconds(flashDuration);

            // 恢复原色
            if (checkButtonImage != null)
            {
                checkButtonImage.color = originalButtonColor;
            }
            yield return new WaitForSeconds(flashDuration);
        }

        // 重新启用按钮交互
        if (checkButton != null)
        {
            checkButton.interactable = true;
        }

        isChecking = false;
    }
}
