using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
public enum ChoiceType
{
    Charactor,
    Stage,
    Boss,
    Bumb,
    Level,
    Life,
    Drone,
    Difficulty,
}
public class CharactorChoose : MonoBehaviour
{
    int CharCount = 0;
        public bool normalChoice;
    public int maxCount;
    public ChoiceType choiceType;
    public Animator Menuanimator;
    public string[] chooseText;
    public Text nowChooseText;
    public GameObject[] AchievementChar;
    void Awake()
    {
        if (choiceType == ChoiceType.Difficulty)
        {
            TitleManager.Instance.ChoiceDifficulty = 0;
            DisplayText(TitleManager.Instance.ChoiceDifficulty);
            if (TitleManager.Instance.Achievements[22])
                maxCount = 5;
            else
                maxCount = 4;
        }
    }
    void Update()
    {
        if (normalChoice)
        {
            Menuanimator.SetInteger("CharCount", CharCount);
        }
        else
        {
            switch (choiceType)
            {
                case ChoiceType.Stage:
                    DisplayText(TitleManager.Instance.chooseStage);
                    break;
                case ChoiceType.Boss:
                    DisplayText(TitleManager.Instance.chooseBoss);
                    break;
                case ChoiceType.Charactor:
                    DisplayText(CharCount);
                    break;
                case ChoiceType.Bumb:
                    DisplayText(TitleManager.Instance.Bomb);
                    break;
                case ChoiceType.Life:
                    DisplayText(TitleManager.Instance.Life);
                    break;
                case ChoiceType.Level:
                    DisplayText(TitleManager.Instance.Level);
                    break;
                case ChoiceType.Drone:
                    DisplayText(TitleManager.Instance.Drone);
                    break;
                case ChoiceType.Difficulty:
                    DisplayText(TitleManager.Instance.ChoiceDifficulty);
                    break;
            }
        }
    }
    public void Charactor(bool next)
    {
        if (next)
        {
            switch (choiceType)
            {
                case ChoiceType.Stage:
                    TitleManager.Instance.chooseStage = AddCount(TitleManager.Instance.chooseStage);
                    break;
                case ChoiceType.Charactor:
                    CharCount = AddCount(CharCount);
                    TitleManager.Instance.ChoicePlayer = CharCount;
                    DisplayAchieveChar();
                    break;
                case ChoiceType.Boss:
                    TitleManager.Instance.chooseBoss++;
                    if (TitleManager.Instance.chooseBoss > 2)
                    {
                        TitleManager.Instance.chooseBoss = 1;
                    }
                    break;
                case ChoiceType.Bumb:
                    TitleManager.Instance.Bomb = AddCount(TitleManager.Instance.Bomb);
                    break;
                case ChoiceType.Life:
                    TitleManager.Instance.Life = AddCount(TitleManager.Instance.Life);
                    break;
                case ChoiceType.Level:
                    TitleManager.Instance.Level = AddCount(TitleManager.Instance.Level);
                    break;
                case ChoiceType.Drone:
                    TitleManager.Instance.Drone = AddCount(TitleManager.Instance.Drone);
                    break;
                case ChoiceType.Difficulty:
                    TitleManager.Instance.ChoiceDifficulty = AddCount(TitleManager.Instance.ChoiceDifficulty);
                    break;
            }
        }
        else if (!next)
        {
            switch (choiceType)
            {
                case ChoiceType.Stage:
                    TitleManager.Instance.chooseStage = MinusCount(TitleManager.Instance.chooseStage);
                    break;
                case ChoiceType.Charactor:
                    CharCount = MinusCount(CharCount);
                    TitleManager.Instance.ChoicePlayer = CharCount;
                    DisplayAchieveChar();
                    break;
                case ChoiceType.Boss:
                    TitleManager.Instance.chooseBoss--;
                    if (TitleManager.Instance.chooseBoss < 1)
                    {
                        TitleManager.Instance.chooseBoss = 2;
                    }
                    break;
                case ChoiceType.Bumb:
                    TitleManager.Instance.Bomb = MinusCount(TitleManager.Instance.Bomb);
                    break;
                case ChoiceType.Life:
                    TitleManager.Instance.Life = MinusCount(TitleManager.Instance.Life);
                    break;
                case ChoiceType.Level:
                    TitleManager.Instance.Level = MinusCount(TitleManager.Instance.Level);
                    break;
                case ChoiceType.Drone:
                    TitleManager.Instance.Drone = MinusCount(TitleManager.Instance.Drone);
                    break;
                case ChoiceType.Difficulty:
                    TitleManager.Instance.ChoiceDifficulty = MinusCount(TitleManager.Instance.ChoiceDifficulty);
                    break;
            }
        }
    }
    void DisplayAchieveChar()
    {
        if ((CharCount == 2 && TitleManager.Instance.Achievements[20]) || (CharCount == 3 && TitleManager.Instance.Achievements[19]) || (CharCount == 4 && TitleManager.Instance.Achievements[16]))
        {
            TitleManager.Instance.CharImage[CharCount].color = new Color(1, 1, 1, 0.1f);
            TitleManager.Instance.CharText[CharCount].SetActive(true);
            AchievementChar[0].SetActive(false);
            AchievementChar[1].SetActive(true);
        }
        else if ((CharCount == 2 && !TitleManager.Instance.Achievements[20]) || (CharCount == 3 && !TitleManager.Instance.Achievements[19]) || (CharCount == 4 && !TitleManager.Instance.Achievements[16]))
        {
            TitleManager.Instance.CharImage[CharCount].color = new Color(0, 0, 0, 0.5f);
            TitleManager.Instance.CharText[CharCount].SetActive(false);
            AchievementChar[0].SetActive(true);
            AchievementChar[1].SetActive(false);
        }
        else if (CharCount == 0 || CharCount == 1)
        {
            AchievementChar[0].SetActive(false);
            AchievementChar[1].SetActive(true);
        }
    }
    void DisplayText(int num)
    {
        if (choiceType == ChoiceType.Bumb || choiceType == ChoiceType.Life || choiceType == ChoiceType.Drone)
        {
            nowChooseText.text = num.ToString();
        }
        else if (choiceType == ChoiceType.Boss)
        {
            nowChooseText.text = chooseText[num - 1];
        }
        else
        {
            nowChooseText.text = chooseText[num];
        }
    }
    int AddCount(int index)
    {
        if (choiceType == ChoiceType.Drone)
        {
            index += 2;
        }
        else
        {
            index++;
        }
        if (index > maxCount - 1)
        {
            index = 0;
        }
        return index;
    }
    int MinusCount(int index)
    {
        if (choiceType == ChoiceType.Drone)
        {
            index -= 2;
        }
        else
        {
            index--;
        }
        if (index < 0)
        {
            index = maxCount - 1;
        }
        return index;
    }
    public void ToDifficulty()
    {
        AchievementChar[2].SetActive(true);
        AchievementChar[3].SetActive(false);
    }
    public void ToChar()
    {
        AchievementChar[2].SetActive(false);
        AchievementChar[3].SetActive(true);
    }
    public void PlayGame(int index)
    {
        if (index < 5)
        {
            TitleManager.Instance.isRush = false;
            TitleManager.Instance.ChoiceDifficulty = index;
            TitleManager.Instance.canSaveGameData = true;
            TitleManager.Instance.GameStage = 1;
            TitleManager.Instance.Save();
            StartCoroutine(GameStart(1));
        }
        else
        {
            //4 5 6 7 8 9
            TitleManager.Instance.isRush = true;
            TitleManager.Instance.canSaveGameData = true;
            TitleManager.Instance.GameStage = TitleManager.Instance.chooseStage + 1;
            TitleManager.Instance.Save();
            if (TitleManager.Instance.chooseBoss == 0)
            {
                StartCoroutine(GameStart(TitleManager.Instance.chooseStage + 1));
            }
            else
            {
                StartCoroutine(GameStart(TitleManager.Instance.chooseStage * 2 + 3 + TitleManager.Instance.chooseBoss));
            }
        }
    }
    IEnumerator GameStart(int stageselect)
    {
        TitleManager.Instance.Save();
        DontDestroyOnLoad(TitleManager.Instance.AudioPlay(TitleManager.Instance.ClickSound, true));
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene(stageselect);
    }
    public void ExitMenu()
    {
        CharCount = 0;
    }
}
