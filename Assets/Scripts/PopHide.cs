using UnityEngine;
using UnityEngine.UI;

public class PopHide : MonoBehaviour
{
    [SerializeField] private Button closeButton; // 关闭按钮引用
    [SerializeField] private bool pauseTimeOnEnable = true; // 是否在激活时暂停时间

    private float previousTimeScale; // 保存之前的时间缩放值

    private void OnEnable()
    {
        // 保存当前时间缩放值
        previousTimeScale = Time.timeScale;

        // 暂停游戏时间
        if (pauseTimeOnEnable)
        {
            Time.timeScale = 0f;
            Debug.Log("游戏时间已暂停");
        }

        // 检查并设置关闭按钮
        if (closeButton != null)
        {
            // 移除所有现有的监听器，避免重复
            closeButton.onClick.RemoveAllListeners();
            // 添加点击事件
            closeButton.onClick.AddListener(ClosePopup);
        }
        else
        {
            Debug.LogWarning("关闭按钮未设置，请在Inspector中指定按钮引用！");
        }
    }

    private void OnDisable()
    {
        // 确保在对象被禁用时恢复时间，防止意外情况导致游戏永久暂停
        if (pauseTimeOnEnable && Time.timeScale == 0f)
        {
            RestoreTime();
        }
    }

    /// <summary>
    /// 关闭弹窗的公共方法，可以被按钮调用
    /// </summary>
    public void ClosePopup()
    {
        // 恢复游戏时间
        RestoreTime();

        // 隐藏对象
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 恢复游戏时间
    /// </summary>
    private void RestoreTime()
    {
        // 恢复到之前的时间缩放值，如果之前就是0，则设为1
        Time.timeScale = previousTimeScale > 0 ? previousTimeScale : 1f;
        Debug.Log($"游戏时间已恢复至 {Time.timeScale}");
    }
}