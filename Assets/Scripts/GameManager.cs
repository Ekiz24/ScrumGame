using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // 用于LINQ查询
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemData
{
    public string itemId;     // 例如 "A", "B", "C" 等
    public string priority;   // "高优先级", "中优先级", "低优先级", "无优先级"
    public int positionIndex; // 在所属容器中的位置索引
    public bool isSelected;   // 是否被选中
    public bool isCompleted;  // 是否已完成
}

[Serializable]
public class GameData
{
    public List<ItemData> items = new List<ItemData>();
    public int performanceScore = 100; // 初始值为100
    public int clientSatisfaction = 5; // 初始值为5
}

public class GameManager : MonoBehaviour
{
    public Transform highPriorityContainer;
    public Transform mediumPriorityContainer;
    public Transform lowPriorityContainer;
    public Transform noPriorityContainer;
    public Transform selectionContainer; // 新增选择容器引用
    public Button completeButton;
    public Button resetButton;  // 新增复原按钮引用
    //public Button planningButton; // 新增规划按钮引用
    public Image scrumFrameworkStageImage;
    public Image developmentStageImage;
    public Sprite[] stageImages; // Scrum框架阶段的图片数组

    public GameObject itemPrefab;

    private GameData gameData = new GameData();
    private string saveFilePath;
    private bool isInResetMode = false;  // 是否处于复原模式
    private bool isInPlanningMode = false; // 是否处于规划模式

    public int PerformanceScore
    {
        get { return gameData.performanceScore; }
        set
        {
            gameData.performanceScore = value;
            SaveData();
        }
    }

    public int ClientSatisfaction
    {
        get { return gameData.clientSatisfaction; }
        set
        {
            gameData.clientSatisfaction = value;
            SaveData();
        }
    }

    void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "itemsData.json");
        LoadData();

        // 注册按钮点击事件
        completeButton.onClick.AddListener(OnCompleteButtonClick);
        resetButton.onClick.AddListener(OnResetButtonClick);  // 注册复原按钮点击事件
        //planningButton.onClick.AddListener(OnPlanningButtonClick); // 新增规划按钮监听
    }

    void Start()
    {
        // 如果是首次运行，初始化数据
        if (gameData.items.Count == 0)
        {
            InitializeData();
        }

        // 根据数据创建UI
        UpdateUI();
    }

    void InitializeData()
    {
        // 初始化12个物体的数据
        string[] ids = { "Weather Changes", "Recipe Creation System", "Marketing", "Music and Sound Effects", "Holiday Rewards", "Restaurant Operation System", "Customized Employee Appearance", "Dance Floor Construction", "Renovation and Upgrades", "Room Cleaning", "Hunting System", "UI/UX" };
        string[] priorities = {
            "高优先级", "高优先级", "高优先级",
            "中优先级", "中优先级", "中优先级",
            "低优先级", "低优先级", "低优先级",
            "无优先级", "无优先级", "无优先级"
        };

        // 为每个优先级类别分配索引
        int highIndex = 0, medIndex = 0, lowIndex = 0, noIndex = 0;

        for (int i = 0; i < ids.Length; i++)
        {
            int posIdx = 0;
            string prio = priorities[i];

            // 根据优先级分配索引
            if (prio == "高优先级") posIdx = highIndex++;
            else if (prio == "中优先级") posIdx = medIndex++;
            else if (prio == "低优先级") posIdx = lowIndex++;
            else posIdx = noIndex++;

            ItemData item = new ItemData
            {
                itemId = ids[i],
                priority = prio,
                positionIndex = posIdx,
                isSelected = false,
                isCompleted = false
            };
            gameData.items.Add(item);
        }

        // 确保性能分数和客户满意度设置为初始值
        gameData.performanceScore = 100;
        gameData.clientSatisfaction = 5;

        SaveData();
    }

    public void UpdateUI()
    {
        // 清除所有容器中的子物体
        foreach (Transform child in highPriorityContainer)
            Destroy(child.gameObject);
        foreach (Transform child in mediumPriorityContainer)
            Destroy(child.gameObject);
        foreach (Transform child in lowPriorityContainer)
            Destroy(child.gameObject);
        foreach (Transform child in noPriorityContainer)
            Destroy(child.gameObject);
        foreach (Transform child in selectionContainer)
            Destroy(child.gameObject);

        // 按优先级和索引排序
        List<ItemData> highPriorityItems = new List<ItemData>();
        List<ItemData> mediumPriorityItems = new List<ItemData>();
        List<ItemData> lowPriorityItems = new List<ItemData>();
        List<ItemData> noPriorityItems = new List<ItemData>();
        List<ItemData> selectedItems = new List<ItemData>();

        // 根据优先级和选中状态分组
        foreach (ItemData item in gameData.items)
        {
            if (item.isSelected)
            {
                selectedItems.Add(item);
            }
            else
            {
                switch (item.priority)
                {
                    case "高优先级": highPriorityItems.Add(item); break;
                    case "中优先级": mediumPriorityItems.Add(item); break;
                    case "低优先级": lowPriorityItems.Add(item); break;
                    case "无优先级": noPriorityItems.Add(item); break;
                }
            }
        }

        // 按索引排序
        highPriorityItems.Sort((a, b) => a.positionIndex.CompareTo(b.positionIndex));
        mediumPriorityItems.Sort((a, b) => a.positionIndex.CompareTo(b.positionIndex));
        lowPriorityItems.Sort((a, b) => a.positionIndex.CompareTo(b.positionIndex));
        noPriorityItems.Sort((a, b) => a.positionIndex.CompareTo(b.positionIndex));

        // 创建高优先级物体 (只创建未选中的)
        CreateItemsForContainer(highPriorityItems, highPriorityContainer);
        // 创建中优先级物体
        CreateItemsForContainer(mediumPriorityItems, mediumPriorityContainer);
        // 创建低优先级物体
        CreateItemsForContainer(lowPriorityItems, lowPriorityContainer);
        // 创建无优先级物体
        CreateItemsForContainer(noPriorityItems, noPriorityContainer);
        // 创建选中的物体
        CreateItemsForContainer(selectedItems, selectionContainer);
        AdjustSelectionContainerItems(); // 调整选择容器中的物体位置
    }

    void CreateItemsForContainer(List<ItemData> items, Transform container)
    {
        foreach (ItemData itemData in items)
        {
            GameObject itemObj = Instantiate(itemPrefab);
            DraggableItem item = itemObj.GetComponent<DraggableItem>();
            if (item != null)
            {
                item.Initialize(itemData, this);
                itemObj.transform.SetParent(container, false);
            }
        }
    }

    public void UpdateItemPriority(string itemId, string newPriority)
    {
        // 更新物体的优先级
        ItemData item = gameData.items.Find(i => i.itemId == itemId);
        if (item != null)
        {
            // 记录旧优先级
            string oldPriority = item.priority;

            // 获取目标容器中的物体数量，作为新的位置索引
            int newIndex = 0;
            foreach (ItemData otherItem in gameData.items)
            {
                if (otherItem.priority == newPriority && otherItem != item)
                {
                    newIndex = Math.Max(newIndex, otherItem.positionIndex + 1);
                }
            }

            // 更新优先级和索引
            item.priority = newPriority;
            item.positionIndex = newIndex;

            // 重新整理所有索引，防止冲突
            ReorganizePositionIndices();

            SaveData();
            UpdateUI();
        }
    }

    // 新增方法：重新整理所有物体的位置索引，确保每个容器内的索引连续且不重复
    private void ReorganizePositionIndices()
    {
        // 按优先级分组
        var highItems = gameData.items.FindAll(i => i.priority == "高优先级").OrderBy(i => i.positionIndex).ToList();
        var medItems = gameData.items.FindAll(i => i.priority == "中优先级").OrderBy(i => i.positionIndex).ToList();
        var lowItems = gameData.items.FindAll(i => i.priority == "低优先级").OrderBy(i => i.positionIndex).ToList();
        var noItems = gameData.items.FindAll(i => i.priority == "无优先级").OrderBy(i => i.positionIndex).ToList();

        // 重新分配索引
        for (int i = 0; i < highItems.Count; i++) highItems[i].positionIndex = i;
        for (int i = 0; i < medItems.Count; i++) medItems[i].positionIndex = i;
        for (int i = 0; i < lowItems.Count; i++) lowItems[i].positionIndex = i;
        for (int i = 0; i < noItems.Count; i++) noItems[i].positionIndex = i;
    }

    public ItemData GetItemData(string itemId)
    {
        return gameData.items.Find(i => i.itemId == itemId);
    }

    // 获取当前已选中物体的数量
    public int GetSelectedItemsCount()
    {
        return gameData.items.Count(item => item.isSelected);
    }

    // 获取指定优先级的物体数量
    public int GetItemCountByPriority(string priority)
    {
        return gameData.items.Count(item =>
            item.priority == priority &&
            !item.isSelected &&
            !item.isCompleted);
    }

    public bool ToggleItemSelection(string itemId)
    {
        // 切换物体的选中状态
        ItemData item = gameData.items.Find(i => i.itemId == itemId);
        if (item != null)
        {
            // 如果当前是未选中状态，要变为选中
            if (!item.isSelected)
            {
                // 检查已选中的物体数量
                int selectedCount = GetSelectedItemsCount();
                if (selectedCount >= 3)
                {
                    // 已达到最大选中数量，不允许再选
                    Debug.Log($"已达到最大选择数量(3个)，请先取消选择其他物体");
                    return false;
                }
            }

            // 执行切换操作
            item.isSelected = !item.isSelected;
            SaveData();

            return true;
        }
        return false;
    }

    // 直接设置物体的选中状态
    public bool SetItemSelected(string itemId, bool selected)
    {
        // 更新物体的选中状态
        ItemData item = gameData.items.Find(i => i.itemId == itemId);
        if (item != null && !item.isCompleted)
        {
            // 如果要设为选中，先检查数量限制
            if (selected && !item.isSelected)
            {
                int selectedCount = GetSelectedItemsCount();
                if (selectedCount >= 3)
                {
                    Debug.Log($"已达到最大选中数量(3个)，不能再选中物体 {itemId}");
                    return false;
                }
            }

            item.isSelected = selected;
            SaveData();
            UpdateUI(); // 立即更新UI显示

            return true;
        }
        return false;
    }

    public void OnCompleteButtonClick()
    {
        // 将所有选中的物体标记为已完成
        foreach (ItemData item in gameData.items)
        {
            if (item.isSelected)
            {
                item.isSelected = false;
                item.isCompleted = true;
            }
        }

        SaveData();
        UpdateUI();
    }

    // 添加复原按钮点击处理函数
    public void OnResetButtonClick()
    {
        // 进入复原模式
        isInResetMode = true;
        Debug.Log("进入复原模式：点击已完成的物体可以恢复它们");
    }

    // 添加检查是否处于复原模式的方法
    public bool IsInResetMode()
    {
        return isInResetMode;
    }

    // 添加恢复物体状态的方法
    public void ResetItemState(string itemId)
    {
        // 恢复物体到未选中未完成状态
        ItemData item = gameData.items.Find(i => i.itemId == itemId);
        if (item != null && item.isCompleted)
        {
            item.isCompleted = false;
            item.isSelected = false;

            SaveData();
            UpdateUI();

            // 复原操作完成后，退出复原模式并禁用复原按钮
            isInResetMode = false;
            resetButton.gameObject.SetActive(false);

            Debug.Log($"物体 {itemId} 已被恢复到初始状态，复原按钮已禁用");
        }
    }

    public void SwapItems(string itemId1, string itemId2)
    {
        // 交换两个物体的优先级和位置
        ItemData item1 = gameData.items.Find(i => i.itemId == itemId1);
        ItemData item2 = gameData.items.Find(i => i.itemId == itemId2);

        if (item1 != null && item2 != null)
        {
            // 交换优先级
            string tempPriority = item1.priority;
            item1.priority = item2.priority;
            item2.priority = tempPriority;

            // 交换索引位置
            int tempIndex = item1.positionIndex;
            item1.positionIndex = item2.positionIndex;
            item2.positionIndex = tempIndex;

            // 确保索引一致性
            ReorganizePositionIndices();

            SaveData();
            UpdateUI();
        }
    }

    // 更新性能分数
    public void UpdatePerformanceScore(int amount)
    {
        gameData.performanceScore += amount;
        Debug.Log($"性能分数更新: {gameData.performanceScore}");
        SaveData();
    }

    // 更新客户满意度
    public void UpdateClientSatisfaction(int amount)
    {
        gameData.clientSatisfaction += amount;
        // 确保客户满意度在合理范围内（例如1-10）
        gameData.clientSatisfaction = Mathf.Clamp(gameData.clientSatisfaction, 1, 10);
        Debug.Log($"客户满意度更新: {gameData.clientSatisfaction}");
        SaveData();
    }

    public void SaveData()
    {
        // 保存数据到JSON文件
        string jsonData = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, jsonData);
    }

    void LoadData()
    {
        // 从JSON文件加载数据
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            gameData = JsonUtility.FromJson<GameData>(jsonData);
        }
    }

    // 添加规划按钮点击处理方法
    // public void OnPlanningButtonClick()
    // {
    //     // 切换规划模式状态
    //     isInPlanningMode = !isInPlanningMode;

    //     // 更新按钮文本或外观以反映当前状态
    //     // 这里假设规划按钮有Text组件
    //     Text buttonText = planningButton.GetComponentInChildren<Text>();
    //     if (buttonText != null)
    //     {
    //         buttonText.text = isInPlanningMode ? "完成规划" : "开始规划";
    //     }

    //     Debug.Log(isInPlanningMode ? "进入规划模式：可以选择任务" : "退出规划模式：不能选择任务");
    // }

    // 添加检查是否处于规划模式的方法
    public bool IsInPlanningMode()
    {
        return isInPlanningMode;
    }

    /// <summary>
    /// 筛选ProductBacklog，检查特定任务是否为无优先级
    /// 如果特定任务不在无优先级，则会扣除性能分数
    /// </summary>
    /// <returns>扣除的分数</returns>
    public void SelectProductBacklog()
    {
        // 需要检查的任务列表
        string[] tasksToCheck = { "Hunting System", "Dance Floor Construction", "Room Cleaning" };
        int deductionPoints = 0;

        foreach (string taskId in tasksToCheck)
        {
            ItemData item = gameData.items.Find(i => i.itemId == taskId);
            if (item != null)
            {
                if (item.priority != "无优先级")
                {
                    // 如果任务不是无优先级，扣1分
                    deductionPoints++;
                    Debug.Log($"任务 '{taskId}' 不在无优先级中，扣1分表现分");
                }
                else
                {
                    Debug.Log($"任务 '{taskId}' 正确地在无优先级中");
                }
            }
            else
            {
                Debug.LogWarning($"未找到任务 '{taskId}'");
            }
        }

        // 如果有扣分，更新性能分数
        if (deductionPoints > 0)
        {
            UpdatePerformanceScore(-deductionPoints);
            Debug.Log($"总共扣除 {deductionPoints} 分表现分，当前表现分：{gameData.performanceScore}");
        }
    }

    /// <summary>
    /// 检查Product Backlog中的任务优先级是否正确
    /// 每个任务分类错误将扣除1分表现分
    /// </summary>
    public void PrioritizeProductBacklog()
    {
        int deductionPoints = 0;

        // 定义每个优先级应有的任务
        Dictionary<string, string[]> priorityTasks = new Dictionary<string, string[]>
        {
            { "高优先级", new[] { "Restaurant Operation System", "Recipe Creation System", "UI/UX" } },
            { "中优先级", new[] { "Marketing", "Renovation and Upgrades", "Music and Sound Effects" } },
            { "低优先级", new[] { "Weather Changes", "Holiday Rewards", "Customized Employee Appearance" } },
            { "无优先级", new[] { "Hunting System", "Dance Floor Construction", "Room Cleaning" } }
        };

        // 检查每个优先级的任务
        foreach (var priorityGroup in priorityTasks)
        {
            string priority = priorityGroup.Key;
            string[] tasks = priorityGroup.Value;

            foreach (string taskId in tasks)
            {
                ItemData item = gameData.items.Find(i => i.itemId == taskId);
                if (item != null)
                {
                    if (item.priority != priority)
                    {
                        // 任务优先级错误，扣1分
                        deductionPoints++;
                        Debug.Log($"任务 '{taskId}' 应该在'{priority}'中，但当前在'{item.priority}'中，扣1分表现分");
                    }
                    else
                    {
                        Debug.Log($"任务 '{taskId}' 正确地在'{priority}'中");
                    }
                }
                else
                {
                    Debug.LogWarning($"未找到任务 '{taskId}'");
                }
            }
        }

        // 如果有扣分，更新性能分数
        if (deductionPoints > 0)
        {
            UpdatePerformanceScore(-deductionPoints);
            Debug.Log($"总共扣除 {deductionPoints} 分表现分，当前表现分：{gameData.performanceScore}");
        }
        else
        {
            Debug.Log("所有任务优先级正确！");
        }
    }

    /// <summary>
    /// 检查三个特定任务是否处于选中或完成状态
    /// 如果任务既不是选中状态也不是完成状态，则扣1分表现分
    /// </summary>
    public void Check3SelectionStates()
    {
        // 需要检查的任务列表
        string[] tasksToCheck = { "Restaurant Operation System", "Recipe Creation System", "UI/UX" };
        int deductionPoints = 0;

        foreach (string taskId in tasksToCheck)
        {
            ItemData item = gameData.items.Find(i => i.itemId == taskId);
            if (item != null)
            {
                if (!item.isSelected && !item.isCompleted)
                {
                    // 如果任务既不是选中状态也不是完成状态，扣1分
                    deductionPoints++;
                    Debug.Log($"任务 '{taskId}' 既不是选中状态也不是完成状态，扣1分表现分");
                }
                else
                {
                    string status = item.isCompleted ? "完成" : "选中";
                    Debug.Log($"任务 '{taskId}' 当前处于{status}状态");
                }
            }
            else
            {
                Debug.LogWarning($"未找到任务 '{taskId}'");
            }
        }

        // 如果有扣分，更新性能分数
        if (deductionPoints > 0)
        {
            UpdatePerformanceScore(-deductionPoints);
            Debug.Log($"总共扣除 {deductionPoints} 分表现分，当前表现分：{gameData.performanceScore}");
        }
        else
        {
            Debug.Log("所有关键任务都已经被选中或完成！");
        }
    }

    /// <summary>
    /// 将当前所有处于选中状态的Task变成完成状态
    /// </summary>
    public void CompleteSelectedTasks()
    {
        int completedCount = 0;

        // 将所有选中的物体标记为已完成
        foreach (ItemData item in gameData.items)
        {
            if (item.isSelected)
            {
                item.isSelected = false;
                item.isCompleted = true;
                completedCount++;
                Debug.Log($"任务 '{item.itemId}' 已从选中状态变为完成状态");
            }
        }

        // 如果有任务被完成，激活复原按钮
        if (completedCount > 0)
        {
            resetButton.gameObject.SetActive(true);
            Debug.Log($"总共完成 {completedCount} 个任务，复原按钮已激活");
        }

        SaveData();
        UpdateUI();
    }

    /// <summary>
    /// 切换Scrum框架阶段图片
    /// </summary>
    /// <param name="stageIndex">阶段序号(从0开始)</param>
    public void ChangeScrumFrameworkStage(int stageIndex)
    {
        // 检查索引是否有效
        if (stageIndex >= 0 && stageIndex < stageImages.Length)
        {
            // 设置图片
            scrumFrameworkStageImage.sprite = stageImages[stageIndex];
            Debug.Log($"已切换Scrum框架阶段图片到第{stageIndex}张");
        }
        else
        {
            Debug.LogWarning($"无效的阶段序号: {stageIndex}, 有效范围: 0-{stageImages.Length - 1}");
        }
    }

    /// <summary>
    /// 切换开发阶段图片
    /// </summary>
    /// <param name="stageIndex">阶段序号(从0开始)</param>
    public void ChangeDevelopmentStage(int stageIndex)
    {
        // 检查索引是否有效
        if (stageIndex >= 0 && stageIndex < stageImages.Length)
        {
            // 设置图片
            developmentStageImage.sprite = stageImages[stageIndex];
            Debug.Log($"已切换开发阶段图片到第{stageIndex}张");
        }
        else
        {
            Debug.LogWarning($"无效的阶段序号: {stageIndex}, 有效范围: 0-{stageImages.Length - 1}");
        }
    }

    public void AdjustSelectionContainerItems()
    {
        int index = 0;
        foreach (Transform child in selectionContainer)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt != null)
            {
                // 垂直排列物体，每个间隔50个单位
                rt.anchoredPosition = new Vector2(0, -50 * index);
                index++;
            }
        }
    }
}