using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimTrigger : MonoBehaviour
{
    [Header("激活时要隐藏的对象")]
    public GameObject[] hideOnActivate;

    [Header("激活时要显示的对象")]
    public GameObject[] showOnActivate;

    [Header("动画设置")]
    public float animationDuration = 5.0f;
    private Animator animator;

    [Header("结束时要隐藏的对象")]
    public GameObject[] hideOnComplete;

    [Header("结束时要显示的对象")]
    public GameObject[] showOnComplete;

    [Header("进度条设置")]
    public Slider progressSlider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("AnimTrigger: 没有找到Animator组件！");
        }
    }

    private void OnEnable()
    {
        // 处理激活时要隐藏/显示的对象
        SetGameObjectsActive(hideOnActivate, false);
        SetGameObjectsActive(showOnActivate, true);

        // 初始化进度条
        if (progressSlider != null)
        {
            progressSlider.value = 0f;
        }

        // 开始播放动画
        if (animator != null)
        {
            animator.enabled = true;
        }

        // 启动计时器
        StartCoroutine(AnimationTimer());
    }

    private IEnumerator AnimationTimer()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        // 持续更新直到动画时间结束
        while (elapsedTime < animationDuration)
        {
            elapsedTime = Time.time - startTime;

            // 更新进度条
            if (progressSlider != null)
            {
                progressSlider.value = elapsedTime / animationDuration;
            }

            yield return null; // 等待下一帧
        }

        // 确保进度条显示100%
        if (progressSlider != null)
        {
            progressSlider.value = 1f;
        }

        // 处理结束时要隐藏/显示的对象
        SetGameObjectsActive(hideOnComplete, false);
        SetGameObjectsActive(showOnComplete, true);

        // 停止动画
        if (animator != null)
        {
            animator.enabled = false;
        }

        // 禁用自身GameObject
        gameObject.SetActive(false);
    }

    // 设置GameObject数组的激活状态
    private void SetGameObjectsActive(GameObject[] objects, bool active)
    {
        if (objects != null)
        {
            foreach (GameObject obj in objects)
            {
                if (obj != null)
                {
                    obj.SetActive(active);
                }
            }
        }
    }
}
