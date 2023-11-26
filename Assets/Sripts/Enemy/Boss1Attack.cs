using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Attack : Enemy
{
    protected override void changeBarrage()
    {
        if(nowIndex==0||nowIndex==1)
        {
            nowIndex=2;
        }
        else
        {
            if(gameObject.GetComponent<Death>().hpBar.value<=0.5)
            {
                nowIndex=1;
            }
            else
            {
                nowIndex=0;
            }
        }
    }
}
