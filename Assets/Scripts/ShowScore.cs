using UnityEngine;
using TMPro;

public class ShowScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameManager gameManager; // 可选引用

    private void OnEnable()
    {
        // 确保文本组件已赋值
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
            if (scoreText == null)
            {
                Debug.LogError("ShowScore需要TextMeshProUGUI组件！");
                return;
            }
        }

        // 获取GameManager实例
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("场景中没有找到GameManager！");
                return;
            }
        }

        // 获取并显示分数
        int score = gameManager.PerformanceScore;
        scoreText.text = $"Your Final Score is {score}";

        Debug.Log($"显示最终得分：{score}");
    }
}
