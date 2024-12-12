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
    Other
}
public class CharactorChoose : MonoBehaviour
{
    int CharCount = 0;
    int chooseStage = 1;
    int index = 0;
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
            nowChooseText.text = chooseText[CharCount];
        }
    }
    public void Charactor(bool next)
    {
        if (next)
        {
            switch (choiceType)
            {
                case ChoiceType.Stage:
                    chooseStage++;
                    if (chooseStage > 3)
                    {
                        chooseStage = 1;
                    }
                    break;
                case ChoiceType.Charactor:
                    CharCount++;
                    TitleManager.Instance.ChoicePlayer = CharCount;
                    if (CharCount > maxCount-1)
                    {
                        CharCount = 0;
                    }
                    break;
                case ChoiceType.Other:
                    index++;
                    if (index > maxCount-1)
                    {
                        index = 0;
                    }
                    break;
            }
        }
        else if (!next)
        {
            switch (choiceType)
            {
                case ChoiceType.Stage:
                    chooseStage--;
                    if (chooseStage < 1)
                    {
                        chooseStage = 3;
                    }
                    break;
                case ChoiceType.Charactor:
                    CharCount--;
                    TitleManager.Instance.ChoicePlayer = CharCount;
                    if (CharCount > 0)
                    {
                        CharCount = maxCount-1;
                    }
                    break;
                case ChoiceType.Other:
                    index--;
                    if (index > 0)
                    {
                        index = maxCount-1;
                    }
                    break;
            }
        }
    }
    public void PlayGame()
    {
        if (normalChoice)
        {
            chooseStage = 1;
        }
        StartCoroutine(GameStart(chooseStage));
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
        //  Buttons[1].SetActive(false);
        //  Buttons[0].SetActive(true);
        /*  for(int i = 1;i<Charators.Length;i++)
          {
              Charators[i].SetActive(false);
          }
          Charators[0].SetActive(true);*/

    }
}
