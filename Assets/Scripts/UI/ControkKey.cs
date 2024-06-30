using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ControkKey : MonoBehaviour, IPointerExitHandler, IPointerClickHandler
{
    public bool isstart = false;//当前是否是修改状态
    public KeyCode curinput;
    public Text nowChooseText;
    public void OnPointerClick(PointerEventData eventData)//按下按鈕
    {
        isstart = true;
    }

    public void OnPointerExit(PointerEventData eventData)//滑鼠離開按鈕
    {
        isstart = false;
    }
    void Update()
    {
        Debug.Log(curinput);
        if (isstart)
        {
            if (Input.anyKeyDown)//检测到按键或者鼠标
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetMouseButton(0)|| Input.GetMouseButton(1) || Input.GetMouseButton(2))
                    {
                        continue;//去除鼠标按键的影响
                    }
                    if (Input.GetKeyDown(keyCode))
                    {
                        curinput = keyCode;//按下的按钮
                    }
                }
            }
        }
        nowChooseText.text = curinput.ToString();
    }
}
