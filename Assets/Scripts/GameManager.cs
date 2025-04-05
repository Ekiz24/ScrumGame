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
}

public class GameManager : MonoBehaviour
{
    public Transform highPriorityContainer;
    public Transform mediumPriorityContainer;
    public Transform lowPriorityContainer;
    public Transform noPriorityContainer;
    public Button completeButton;

    public GameObject itemPrefab;

    private GameData gameData = new GameData();
    private string saveFilePath;

    void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "itemsData.json");
        LoadData();

        // 注册完成按钮点击事件
        completeButton.onClick.AddListener(OnCompleteButtonClick);
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
        string[] ids = { "Weather Changes", "Menu Development System", "Marketing", "Music and Sound Effects", "Holiday Rewards", "Restaurant Operation System", "Customized Employee Appearance", "Dance Floor Construction", "Renovation and Upgrades", "Room Cleaning", "Hunting System", "UI/UX" };
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

        SaveData();
    }

    void UpdateUI()
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

        // 按优先级和索引排序
        List<ItemData> highPriorityItems = new List<ItemData>();
        List<ItemData> mediumPriorityItems = new List<ItemData>();
        List<ItemData> lowPriorityItems = new List<ItemData>();
        List<ItemData> noPriorityItems = new List<ItemData>();

        // 根据优先级分组
        foreach (ItemData item in gameData.items)
        {
            switch (item.priority)
            {
                case "高优先级": highPriorityItems.Add(item); break;
                case "中优先级": mediumPriorityItems.Add(item); break;
                case "低优先级": lowPriorityItems.Add(item); break;
                case "无优先级": noPriorityItems.Add(item); break;
            }
        }

        // 按索引排序
        highPriorityItems.Sort((a, b) => a.positionIndex.CompareTo(b.positionIndex));
        mediumPriorityItems.Sort((a, b) => a.positionIndex.CompareTo(b.positionIndex));
        lowPriorityItems.Sort((a, b) => a.positionIndex.CompareTo(b.positionIndex));
        noPriorityItems.Sort((a, b) => a.positionIndex.CompareTo(b.positionIndex));

        // 创建高优先级物体
        CreateItemsForContainer(highPriorityItems, highPriorityContainer);
        // 创建中优先级物体
        CreateItemsForContainer(mediumPriorityItems, mediumPriorityContainer);
        // 创建低优先级物体
        CreateItemsForContainer(lowPriorityItems, lowPriorityContainer);
        // 创建无优先级物体
        CreateItemsForContainer(noPriorityItems, noPriorityContainer);
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

    public void ToggleItemSelection(string itemId)
    {
        // 切换物体的选中状态
        ItemData item = gameData.items.Find(i => i.itemId == itemId);
        if (item != null)
        {
            item.isSelected = !item.isSelected;
            SaveData();

            // 打印调试信息
            Debug.Log($"物体 {itemId} 选中状态: {item.isSelected}");
        }
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

    public void SwapItems(string itemId1, string itemId2)
    {
        // 交换两个物体的优先级和位置
        ItemData item1 = gameData.items.Find(i => i.itemId == itemId1);
        ItemData item2 = gameData.items.Find(i => i.itemId == itemId2);

        if (item1 != null && item2 != null)
        {
            Debug.Log($"交换物体：{item1.itemId}(优先级:{item1.priority}, 位置:{item1.positionIndex}) 和 " +
                      $"{item2.itemId}(优先级:{item2.priority}, 位置:{item2.positionIndex})");

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

    void SaveData()
    {
        // 保存数据到JSON文件
        string jsonData = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, jsonData);
        Debug.Log("数据已保存: " + saveFilePath);
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
}