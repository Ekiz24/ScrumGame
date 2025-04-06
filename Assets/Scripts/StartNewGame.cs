using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class StartNewGame : MonoBehaviour
{
    // 你可以在Inspector中指定场景名称
    public string mainGameSceneName = "MainGame";

    // 当按钮点击时调用此方法
    public void StartGame()
    {
        Debug.Log("开始新游戏...");

        // 删除现有的游戏存档数据
        DeleteExistingGameData();

        // 加载主游戏场景
        SceneManager.LoadScene(mainGameSceneName);
    }

    // 删除现有的游戏存档数据
    private void DeleteExistingGameData()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "itemsData.json");

        if (File.Exists(saveFilePath))
        {
            Debug.Log("删除现有游戏数据: " + saveFilePath);
            File.Delete(saveFilePath);
        }
        else
        {
            Debug.Log("未找到现有游戏数据，将创建新数据");
        }

        // 清除PlayerPrefs中可能存在的任何相关数据
        PlayerPrefs.DeleteKey("GameInitialized");
        PlayerPrefs.Save();
    }
}
