using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTask : MonoBehaviour
{
    [System.Serializable]
    public class TaskReference
    {
        public string taskId;
        public GameObject miniGameObject;
    }

    [Header("任务迷你游戏对象")]
    [SerializeField] private TaskReference[] taskReferences = new TaskReference[12];

    [Header("当前对象激活时")]
    [SerializeField] private GameObject[] hideOnActivate;
    [SerializeField] private GameObject[] showOnActivate;

    [Header("当前对象失活时")]
    [SerializeField] private GameObject[] hideOnDeactivate;
    [SerializeField] private GameObject[] showOnDeactivate;

    private GameManager gameManager;
    private GameObject currentMiniGame;
    private string currentTaskId;

    private void Awake()
    {
        // 获取GameManager引用
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("未找到GameManager！");
        }
    }

    private void OnEnable()
    {
        // 处理激活时要显示/隐藏的对象
        SetGameObjectsActive(hideOnActivate, false);
        SetGameObjectsActive(showOnActivate, true);

        // 寻找选中的任务并激活对应的迷你游戏
        FindAndActivateSelectedTask();
    }

    private void FindAndActivateSelectedTask()
    {
        List<string> selectedTaskIds = new List<string>();

        // 检查所有Task引用
        foreach (TaskReference taskRef in taskReferences)
        {
            if (string.IsNullOrEmpty(taskRef.taskId) || taskRef.miniGameObject == null)
                continue;

            // 获取任务状态
            ItemData itemData = gameManager.GetItemData(taskRef.taskId);
            if (itemData != null && itemData.isSelected)
            {
                selectedTaskIds.Add(taskRef.taskId);
            }
        }

        // 如果没有选中的任务，直接失活
        if (selectedTaskIds.Count == 0)
        {
            Debug.Log("没有选中的任务，退出任务模式");
            StartCoroutine(DelayedDeactivate());
            return;
        }

        // 使用第一个选中的任务
        string firstSelectedTaskId = selectedTaskIds[0];
        Debug.Log($"找到选中的任务: {firstSelectedTaskId}，将激活其迷你游戏");

        // 激活对应的迷你游戏
        foreach (TaskReference taskRef in taskReferences)
        {
            if (taskRef.taskId == firstSelectedTaskId)
            {
                currentTaskId = firstSelectedTaskId;
                currentMiniGame = taskRef.miniGameObject;

                // 先禁用其他所有迷你游戏
                DisableAllMiniGames();

                // 然后激活当前选中的迷你游戏
                currentMiniGame.SetActive(true);

                // 开始监听迷你游戏的状态
                StartCoroutine(MonitorMiniGameState());
                return;
            }
        }
    }

    private void DisableAllMiniGames()
    {
        foreach (TaskReference taskRef in taskReferences)
        {
            if (taskRef.miniGameObject != null)
            {
                taskRef.miniGameObject.SetActive(false);
            }
        }
    }

    private IEnumerator MonitorMiniGameState()
    {
        // 等待迷你游戏被禁用
        while (currentMiniGame != null && currentMiniGame.activeInHierarchy)
        {
            yield return null;
        }

        // 迷你游戏结束，将任务标记为完成
        if (!string.IsNullOrEmpty(currentTaskId))
        {
            Debug.Log($"迷你游戏已完成，将任务 {currentTaskId} 标记为已完成");

            // 先取消选中状态
            gameManager.SetItemSelected(currentTaskId, false);

            // 然后通过GameManager把当前任务设为完成状态
            ItemData itemData = gameManager.GetItemData(currentTaskId);
            if (itemData != null)
            {
                itemData.isCompleted = true;
                gameManager.SaveData();
                gameManager.UpdateUI();
            }
        }

        // 任务完成，失活当前对象
        StartCoroutine(DelayedDeactivate());
    }

    private IEnumerator DelayedDeactivate()
    {
        // 短暂延迟，确保所有状态都已更新
        yield return new WaitForSeconds(0.1f);

        // 处理失活时要显示/隐藏的对象
        SetGameObjectsActive(hideOnDeactivate, false);
        SetGameObjectsActive(showOnDeactivate, true);

        // 失活当前对象
        gameObject.SetActive(false);
    }

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
