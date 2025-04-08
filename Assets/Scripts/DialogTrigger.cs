using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField]
    [Header("要加载的对话文件")]
    private TextAsset inkJSONFile;

    [SerializeField]
    [Header("需要在对话开始时隐藏的物体")]
    private GameObject[] objectsToHide;

    [SerializeField]
    [Header("需要在对话开始时显示的物体")]
    private GameObject[] objectsToShowAtStart;

    [SerializeField]
    [Header("需要在对话结束时隐藏的物体")]
    private GameObject[] objectsToHideAtEnd;

    [SerializeField]
    [Header("需要在对话结束时显示的物体")]
    private GameObject[] objectsToShow;


    [SerializeField]
    [Header("是否在Awake时自动触发对话")]
    private bool triggerOnAwake = true;

    private void Awake()
    {
        // 隐藏需要在对话开始时隐藏的物体
        if (objectsToHide != null)
        {
            foreach (GameObject obj in objectsToHide)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
        // 显示需要在对话开始时显示的物体
        if (objectsToShowAtStart != null)
        {
            foreach (GameObject obj in objectsToShowAtStart)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
        if (triggerOnAwake)
        {
            TriggerDialogue();
        }
    }

    /// <summary>
    /// 触发对话
    /// </summary>
    public void TriggerDialogue()
    {
        if (inkJSONFile == null)
        {
            Debug.LogError("没有指定Ink JSON文件！");
            return;
        }

        // 查找场景中现有的InkDialogueManager
        InkDialogueManager dialogueManager = FindFirstObjectByType<InkDialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogError("场景中没有找到InkDialogueManager！");
            return;
        }

        // 重置已有的对话管理器并设置新的对话文件
        ResetAndSetupDialogueManager(dialogueManager);
    }

    /// <summary>
    /// 重置并设置对话管理器
    /// </summary>
    private void ResetAndSetupDialogueManager(InkDialogueManager dialogueManager)
    {
        // 添加一个在InkDialogueManager中的公共方法来重设对话
        dialogueManager.SetupNewDialogue(inkJSONFile, objectsToShow, objectsToHideAtEnd);
    }
}
