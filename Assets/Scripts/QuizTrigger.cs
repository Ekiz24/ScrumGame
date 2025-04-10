using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizTrigger : MonoBehaviour
{
    [System.Serializable]
    public class QuizQuestion
    {
        [TextArea(3, 5)]
        public string questionText;
        public bool correctAnswer; // true = 对，false = 错
        public Sprite questionImage; // 添加题目图片引用
    }

    [Header("题目设置")]
    [SerializeField] private List<QuizQuestion> questions = new List<QuizQuestion>();

    [Header("结语设置")]
    [TextArea(3, 5)]
    [SerializeField] private string conclusionText = "恭喜你完成了所有题目！";
    [SerializeField] private Sprite conclusionImage; // 添加结语图片引用

    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button trueButton;
    [SerializeField] private Button falseButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image trueButtonImage;
    [SerializeField] private Image falseButtonImage;
    [SerializeField] private Image questionImageDisplay; // 添加图片显示组件

    [Header("闪烁设置")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private int flashCount = 3;

    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private Color trueButtonOriginalColor;
    private Color falseButtonOriginalColor;
    private bool isAnswering = false;

    void OnEnable()
    {
        // 保存按钮原始颜色
        trueButtonOriginalColor = trueButtonImage.color;
        falseButtonOriginalColor = falseButtonImage.color;

        // 初始化界面
        InitializeQuiz();
    }

    private void InitializeQuiz()
    {
        // 重置答题状态
        currentQuestionIndex = 0;
        correctAnswers = 0;
        isAnswering = false;

        // 设置按钮监听
        trueButton.onClick.RemoveAllListeners();
        falseButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

        trueButton.onClick.AddListener(() => OnAnswerSelected(true));
        falseButton.onClick.AddListener(() => OnAnswerSelected(false));
        closeButton.onClick.AddListener(OnCloseButtonClicked);

        // 隐藏关闭按钮
        closeButton.gameObject.SetActive(false);

        // 如果有题目，显示第一题
        if (questions.Count > 0)
        {
            DisplayCurrentQuestion();
        }
        else
        {
            questionText.text = "None";
            progressText.text = "0/0";

            // 清空图片
            if (questionImageDisplay != null)
            {
                questionImageDisplay.sprite = null;
                questionImageDisplay.gameObject.SetActive(false);
            }

            DisableAnswerButtons();
        }
    }

    private void DisplayCurrentQuestion()
    {
        if (currentQuestionIndex < questions.Count)
        {
            // 显示当前题目
            questionText.text = questions[currentQuestionIndex].questionText;

            // 显示题目图片
            if (questionImageDisplay != null)
            {
                questionImageDisplay.sprite = questions[currentQuestionIndex].questionImage;
                questionImageDisplay.gameObject.SetActive(questions[currentQuestionIndex].questionImage != null);
            }

            // 更新进度
            progressText.text = $"{currentQuestionIndex + 1}/{questions.Count}";

            // 确保按钮可点击
            EnableAnswerButtons();
        }
        else
        {
            // 所有题目已回答完，显示结论
            ShowConclusion();
        }
    }

    private void OnAnswerSelected(bool selectedAnswer)
    {
        if (isAnswering)
            return;

        isAnswering = true;

        // 获取当前题目的正确答案
        bool correctAnswer = questions[currentQuestionIndex].correctAnswer;

        // 判断答案是否正确
        bool isCorrect = (selectedAnswer == correctAnswer);

        if (isCorrect)
        {
            correctAnswers++;
        }

        // 显示按钮闪烁效果
        Button selectedButton = selectedAnswer ? trueButton : falseButton;
        Image selectedButtonImage = selectedAnswer ? trueButtonImage : falseButtonImage;

        StartCoroutine(FlashButtonCoroutine(selectedButtonImage, isCorrect));
    }

    private IEnumerator FlashButtonCoroutine(Image buttonImage, bool isCorrect)
    {
        Color flashColor = isCorrect ? correctColor : incorrectColor;
        Color originalColor = buttonImage.color;

        // 禁用按钮交互
        DisableAnswerButtons();

        // 闪烁效果
        for (int i = 0; i < flashCount; i++)
        {
            buttonImage.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            buttonImage.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        // 进入下一题
        currentQuestionIndex++;
        isAnswering = false;
        DisplayCurrentQuestion();
    }

    private void ShowConclusion()
    {
        // 显示结论文本
        questionText.text = conclusionText + $"\n[Correctness: {correctAnswers}/{questions.Count}]";
        progressText.text = " ";

        // 显示结语图片
        if (questionImageDisplay != null)
        {
            questionImageDisplay.sprite = conclusionImage;
            questionImageDisplay.gameObject.SetActive(conclusionImage != null);
        }

        // 禁用回答按钮
        DisableAnswerButtons();

        // 显示关闭按钮
        closeButton.gameObject.SetActive(true);
    }

    private void OnCloseButtonClicked()
    {
        // 关闭答题面板
        gameObject.SetActive(false);
    }

    private void EnableAnswerButtons()
    {
        trueButton.interactable = true;
        falseButton.interactable = true;
    }

    private void DisableAnswerButtons()
    {
        trueButton.interactable = false;
        falseButton.interactable = false;
    }
}
