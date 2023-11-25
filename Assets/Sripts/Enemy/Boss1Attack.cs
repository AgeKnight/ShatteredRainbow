using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Attack : Enemy
{
    protected override void Start()
    {
        nowUse = "Shotgun";
        base.Start();
    }
    protected override string changeBarrage()
    {

        if(nowUse=="Shotgun"||nowUse=="TrackShotgun")
        {
            nowIndex = 1;
            return "CircleBarrage";
        }
        else
        {
            nowIndex = 0;
            if(gameObject.GetComponent<Death>().hpBar.value>0.5)
            {
                return "Shotgun";
            }
            else
            {
                return "TrackShotgun";
            }
        }
    }
}
