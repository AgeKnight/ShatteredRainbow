using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ControkKey : MonoBehaviour, IPointerExitHandler, IPointerClickHandler ,IPointerEnterHandler
{
    public bool isstart = false;//当前是否是修改状态
    public KeyCode curinput;
    public Text nowChooseText;
    public void OnPointerEnter(PointerEventData eventData)//選擇按鈕
    {
        gameObject.GetComponent<Image>().color = new Color(1, 0, 0,0.1f);
    }
    public void OnPointerClick(PointerEventData eventData)//按下按鈕
    {
        isstart = true;
        nowChooseText.text = "Rebinding...";
    }

    public void OnPointerExit(PointerEventData eventData)//滑鼠離開按鈕
    {
        isstart = false;
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1,0.1f);
        nowChooseText.text = curinput.ToString();
    }
    void Update()
    {
        if (isstart)
        {
            if (Input.anyKeyDown)//检测到按键或者鼠标
            {
                
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetMouseButton(0)|| Input.GetMouseButton(1) || Input.GetMouseButton(2))
                    {
                        continue;//去除鼠标按键的影响
                   ;
                    }
                    if (Input.GetKeyDown(keyCode))
                    {
                        for (int i = 0; i < TitleManager.Instance.controkKeys.Length; i++)
                        {
                            if(keyCode.ToString()==TitleManager.Instance.controkKeys[i].curinput.ToString())
                            {
                                TitleManager.Instance.controkKeys[i].curinput = curinput;
                                TitleManager.Instance.controkKeys[i].GetComponent<ControkKey>().nowChooseText.text = curinput.ToString();
                            }
                        }                 
                        curinput = keyCode;//按下的按钮
                        nowChooseText.text = curinput.ToString();
                    }
                   
                }
                
            }
        }
       
    }
}
