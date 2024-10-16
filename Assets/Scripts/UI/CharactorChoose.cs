using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharactorChoose : MonoBehaviour
{
    public GameObject[] Charators;
    public GameObject[] Buttons;
    public Animator Menuanimator;
    int CharCount = 0;
    void Update()
    {
        Menuanimator.SetInteger("CharCount", CharCount);
    }
    public void Charactor(bool next)
    {
      /*  for(int i = 0;i<Charators.Length;i++)
        {
           Charators[i].SetActive(false);
        }*/
        if(next&&CharCount!=Charators.Length-1)

            CharCount++;
        else if(!next&&CharCount!=0)
            CharCount--;
        if(CharCount==Charators.Length-1)
        {
            Buttons[0].SetActive(false);
            Buttons[1].SetActive(true);
        }
        else if(CharCount==0)
        {
            Buttons[0].SetActive(true);
            Buttons[1].SetActive(false);
        }
        else if(CharCount!=0&&CharCount!=Charators.Length-1)
        {
            Buttons[0].SetActive(true);
            Buttons[1].SetActive(true);
        }
      //  Charators[CharCount].SetActive(true);
    }
    public void PlayGame()
    {
     
          StartCoroutine(GameStart());
    }
    IEnumerator GameStart()
    {

        TitleManager.Instance.ChoicePlayer = CharCount;
        TitleManager.Instance.Save();
        DontDestroyOnLoad(TitleManager.Instance.AudioPlay(TitleManager.Instance.ClickSound, true));
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene("Stage1");
    }
    public void ExitMenu()
    {
        CharCount=0;
        Buttons[1].SetActive(false);
        Buttons[0].SetActive(true);
        for(int i = 1;i<Charators.Length;i++)
        {
            Charators[i].SetActive(false);
        }
        Charators[0].SetActive(true);
    }
}
