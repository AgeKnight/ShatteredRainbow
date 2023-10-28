using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Attack : Enemy
{
    public override void Attack()
    {
        coroutine = StartCoroutine(UseBarrage());
    }
    IEnumerator UseBarrage()
    {
        while (FindObjectOfType<Player>())
        {
            if (gameObject.GetComponent<Death>().hpBar.value < 0.5)
            {
                Barrage2();
            }
            else
            {
                Barrage();
            }
            yield return new WaitForSeconds(0.3f);
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
            float angle = Vector3.Angle(gameObject.transform.position, player.transform.position);
            Vector3 normal = Vector3.Cross(gameObject.transform.position, player.transform.position);
            angle = angle * Mathf.Sign(Vector3.Dot(normal, gameObject.transform.forward)) - 24;
            int count = 5;
            for (int i = 0; i < count; i++)
            {
                GameObject temp = Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, angle));
                Allbullet.Add(temp);
                angle += 12;
            }
        }
    }
}
