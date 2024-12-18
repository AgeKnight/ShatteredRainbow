using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum ChoiceType
{
    Charactor,
    Stage,
    Boss,
    Bumb,
    Level,
    Life,
    Drone,
}
public class CharactorChoose : MonoBehaviour
{
    int CharCount = 0;
    int chooseStage = 0;
    public bool normalChoice;
    public int maxCount;
    public ChoiceType choiceType;
    public Animator Menuanimator;
    public string[] chooseText;
    public Text nowChooseText;
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
                    DisplayText(chooseStage);
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
                    chooseStage = AddCount(chooseStage);
                    break;
                case ChoiceType.Charactor:
                    CharCount = AddCount(CharCount);
                    TitleManager.Instance.ChoicePlayer = CharCount;
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
            }
        }
        else if (!next)
        {
            switch (choiceType)
            {
                case ChoiceType.Stage:
                    chooseStage = MinusCount(chooseStage);
                    break;
                case ChoiceType.Charactor:
                    CharCount = MinusCount(CharCount);
                    TitleManager.Instance.ChoicePlayer = CharCount;
                    break;
                case ChoiceType.Boss:
                    TitleManager.Instance.chooseBoss--;
                    if (TitleManager.Instance.chooseBoss <1)
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
            }
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
        Debug.Log(index);
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
    public void PlayGame()
    {
        if (normalChoice)
        {
            TitleManager.Instance.isRush = false;
            TitleManager.Instance.Save();
            StartCoroutine(GameStart(1));
        }
        else
        {
            //4 5 6 7 8 9
            TitleManager.Instance.isRush = true;
            TitleManager.Instance.Save();
            if(TitleManager.Instance.chooseBoss==0)
            {
                StartCoroutine(GameStart(chooseStage + 1));
            }
            else
            {
                StartCoroutine(GameStart(chooseStage*2 + 3+TitleManager.Instance.chooseBoss));
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
