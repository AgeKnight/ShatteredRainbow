using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class Boss1Attack : Enemy
{
    protected override void Start()
    {
        nowUse = "Barrage";
        base.Start();
    }
    protected override void BarrageMethod()
    {
        Invoke(nowUse,0.01f);
        barrageCount+=1;
        if(barrageCount>=allBarragecount)
        {
            barrageCount=0;
            canChooseBarrage=true;
        }
    }
    protected override string changeBarrage()
    {
        if(nowUse=="Barrage"||nowUse=="Barrage2")
        {
            return "BaseBarrage";
        }
        else
        {
            if(gameObject.GetComponent<Death>().hpBar.value>0.5)
            {
                return "Barrage";
            }
            else
            {
                return "Barrage2";
            }
        }
    }
    protected override void ReturnMove()
    {
        if (targetPosition == Dot[0].position)
            targetPosition = Dot[1].position;
        else
            targetPosition = Dot[0].position;
    }
    void Barrage()
    {
        float angle = Random.Range(90, 220);
        for (int i = 0; i < spanCount[0]; i++)
        {
            GameObject temp = Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, angle));
            Allbullet.Add(temp);
            angle += 12;
        }
    }
    void Barrage2()
    {
        if (FindObjectOfType<Player>())
        {
            var player = FindObjectOfType<Player>();
            Vector3 eulerAngle = GetAngle(transform.position, player.transform.position);
            eulerAngle.z -= 24;
            for (int i = 0; i < spanCount[0]; i++)
            {
                GameObject temp = Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, eulerAngle.z));
                Allbullet.Add(temp);
                eulerAngle.z += 12;
            }
        }
    }
    void BaseBarrage()
    {
        int indexz = 0;
        for (int i = 0; i <= spanCount[1]; i++)
        {
            indexz += 360 / spanCount[1];
            GameObject temp = Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, indexz));
            Allbullet.Add(temp);
        }
    }
    Vector3 GetAngle(Vector3 aPoint, Vector3 bPoint)
    {
        Vector3 direct = bPoint - aPoint;
        Vector3 normal = Vector3.Cross(Vector2.up, direct.normalized);
        float zAngle = Vector2.Angle(Vector2.up, direct.normalized);
        zAngle = normal.z > 0 ? zAngle : -zAngle;
        return new Vector3(0, 0, zAngle);
    }
}
