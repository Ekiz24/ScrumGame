using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;

using System.IO;

public class InkDialogueManager : MonoBehaviour
{
    [SerializeField]
    [Header("Objects that need to be hidden at Start")]
    private GameObject[] hideObjects = null;
    [SerializeField]
    [Header("Objects that need to be shown at Start")]

    private GameObject[] showObjectsAtStart = null;

    [SerializeField]
    [Header("Objects that need to be hidden at End")]
    private GameObject[] hideObjectsAtEnd = null;
    [SerializeField]
    [Header("Objects that need to be shown at End")]
    private GameObject[] showObjects = null;



    //Ink file assest
    [SerializeField]
    [Header("Put the Json File here")]
    private TextAsset inkJSONAsset = null;
    // The story class generated from the json file
    public Story story;
    //public GameObject player;
    [SerializeField]
    public List<AllExpressions> allExpressions;
    //The canvas that display all the story text
    private Transform canvas = null;
    //Children of the ui component
    private Transform dialogueBox;
    private Transform dialogueBoxText;
    private Transform characterNameBox;
    private Transform characterNameText;
    private Transform choiceBox;
    private Transform choiceButton;
    private Transform characterBox;
    private List<Transform> characterImages = new List<Transform>();
    private List<string> charactersPos = new List<string>();

    private string savePath;

    void Awake()
    {

        //player.SetActive(false);
        //Find the components
        canvas = transform.Find("Dialog Canvas");
        dialogueBox = canvas.Find("Dialogue Box");
        dialogueBoxText = dialogueBox.Find("Text");
        characterNameBox = dialogueBox.Find("Character Name Box");
        characterNameText = characterNameBox.Find("Text");
        choiceBox = canvas.Find("Choice Box");
        choiceButton = choiceBox.Find("Choice Button");
        characterBox = canvas.Find("Character Box");

        for (int i = 0; i < characterBox.childCount; i++)
        {
            characterImages.Add(characterBox.GetChild(i));
            charactersPos.Add("");
        }
        StartStory();
    }

    // Creates a new Story object with the compiled story which we can then play!
    void StartStory()
    {
        story = new Story(inkJSONAsset.text);

        // 隐藏需要隐藏的物体
        if (hideObjects != null)
        {
            for (int i = 0; i < hideObjects.Length; i++)
            {
                if (hideObjects[i] != null)
                    hideObjects[i].SetActive(false);
            }
        }

        // 显示需要在开始时显示的物体
        if (showObjectsAtStart != null)
        {
            for (int i = 0; i < showObjectsAtStart.Length; i++)
            {
                if (showObjectsAtStart[i] != null)
                    showObjectsAtStart[i].SetActive(true);
            }
        }

        ContinueStory();
    }

    public void ContinueStory()
    {

        if (story.canContinue)
        {
            choiceBox.gameObject.SetActive(false);
            string text = story.Continue();
            // This removes any blank space from the text.
            text = text.Trim();
            //Find the tags of this line
            List<string> tags = story.currentTags;
            // Display the text on screen!
            PrintContent(text, tags);
        }
        else if (story.currentChoices.Count > 0)
        {
            choiceBox.gameObject.SetActive(true);
            if (story.currentChoices.Count > choiceBox.childCount)
            {
                Transform newButton = Instantiate(choiceButton, choiceBox);
            }
            for (int i = 0; i < choiceBox.childCount; i++)
            {
                if (i < (story.currentChoices.Count))
                {
                    Choice choice = story.currentChoices[i];
                    choiceBox.GetChild(i).gameObject.SetActive(true);
                    choiceBox.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    choiceBox.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { OnClickChoiceButton(choice); });
                    choiceBox.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = choice.text.Trim();

                }
                else
                {
                    choiceBox.GetChild(i).gameObject.SetActive(false);
                }

            }
        }
        else
        {
            Debug.Log("story ends");

            // 隐藏所有需要在结束时隐藏的物体
            if (hideObjectsAtEnd != null)
            {
                for (int i = 0; i < hideObjectsAtEnd.Length; i++)
                {
                    if (hideObjectsAtEnd[i] != null)
                        hideObjectsAtEnd[i].SetActive(false);
                }
            }

            // 显示所有需要在结束时显示的物体
            if (showObjects != null)
            {
                for (int i = 0; i < showObjects.Length; i++)
                {
                    if (showObjects[i] != null)
                        showObjects[i].SetActive(true);
                }
            }
            // 销毁对话管理器
            //Destroy(gameObject);
        }
    }

    public void PrintContent(string content, List<string> tags)
    {
        /*Important rules
         * 1. #C:A:Happy 
         * Show Character A happy image
         * 2.#POS:0
         * 0-left,1-centre,2-right
         */
        string[] tagSplit;
        Sprite characterImage = null;
        string characterName = "";
        int characterPos = 0;
        for (int i = 0; i < tags.Count; i++)
        {
            if (!tags[i].Contains(":"))
            {
                continue;
            }

            tagSplit = tags[i].Trim().Split(':');
            //Character tag
            if (tagSplit[0] == "C")
            {
                characterName = tagSplit[1];
                Debug.Log(tagSplit[2]);
                for (int j = 0; j < allExpressions.Count; j++)
                {
                    if (allExpressions[j].characterName == characterName)
                    {
                        for (int k = 0; k < allExpressions[j].expressions.Length; k++)
                        {
                            if (allExpressions[j].expressions[k].expression == tagSplit[2])
                            {
                                characterImage = allExpressions[j].expressions[k].img;
                            }

                        }
                    }
                }
            }
            else if (tagSplit[0] == "POS")
            {
                characterPos = Int32.Parse(tagSplit[1]);
            }
        }
        //
        if (characterImage != null)
        {
            if (charactersPos.Contains(characterName))
            {
                characterImages[charactersPos.IndexOf(characterName)].gameObject.SetActive(false);
                charactersPos[charactersPos.IndexOf(characterName)] = "";
            }
            characterImages[characterPos].gameObject.SetActive(true);
            characterImages[characterPos].GetComponent<Image>().sprite = characterImage;
            charactersPos[characterPos] = characterName;
        }
        if (characterName != "")
        {
            characterNameBox.gameObject.SetActive(true);
            characterNameText.GetComponent<TMP_Text>().text = characterName;
        }
        else
        {
            characterNameBox.gameObject.SetActive(true);
        }

        //Change text
        dialogueBoxText.GetComponent<TMP_Text>().text = content;

    }

    //choice
    //after press choice
    void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        choiceBox.gameObject.SetActive(false);
        ContinueStory();
    }

    /// <summary>
    /// 设置新的对话并开始
    /// </summary>
    /// <param name="newInkJSON">新的Ink JSON文件</param>
    /// <param name="newHideObjects">需要在对话开始时隐藏的物体</param>
    /// <param name="newShowObjects">需要在对话结束时显示的物体</param>
    /// <param name="newShowObjectsAtStart">需要在对话开始时显示的物体</param>
    /// <param name="newHideObjectsAtEnd">需要在对话结束时隐藏的物体</param>
    public void SetupNewDialogue(TextAsset newInkJSON,
                                GameObject[] newShowObjects,
                                GameObject[] newHideObjectsAtEnd = null)
    {
        // 设置新的JSON文件
        inkJSONAsset = newInkJSON;

        // 设置新的隐藏/显示物体
        showObjects = newShowObjects;
        hideObjectsAtEnd = newHideObjectsAtEnd;

        // 重置角色图像和位置
        foreach (Transform charImage in characterImages)
        {
            if (charImage != null)
                charImage.gameObject.SetActive(false);
        }
        charactersPos.Clear();
        for (int i = 0; i < characterImages.Count; i++)
        {
            charactersPos.Add("");
        }

        // 开始新故事
        StartStory();
    }

    [Serializable]
    public class AllExpressions
    {
        //character Name
        public string characterName;
        public Expression[] expressions;

    }
    [Serializable]
    public class Expression
    {
        //expression
        public string expression;
        //ͼƬ sprite
        public Sprite img;

    }

}


