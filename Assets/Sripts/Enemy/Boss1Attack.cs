using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Attack : Enemy
{
    protected override void BarrageMethod()
    {
        if (gameObject.GetComponent<Death>().hpBar.value < 0.5)
        {
            Barrage2();
        }
        else
        {
            Barrage();
        }
    }
    protected override void ReturnMove()
    {
        if (targetPosition == Dot[0].position)
        {
            targetPosition = Dot[1].position;
        }
        else
        {
            targetPosition = Dot[0].position;
        }
    }
    void Barrage()
    {
        float angle = Random.Range(90, 220);
        int count = 5;
        for (int i = 0; i < count; i++)
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
            int count = 5;
            for (int i = 0; i < count; i++)
            {
                GameObject temp = Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, eulerAngle.z));
                Allbullet.Add(temp);
                eulerAngle.z += 12;
            }
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
