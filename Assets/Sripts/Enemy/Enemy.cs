using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Coroutine coroutine;
    public GameObject bullet;
    public Transform bulletTransform;
    [HideInInspector]
    public List<GameObject> Allbullet = new List<GameObject>();
    void Start()
    {
        Attack();
    }
    public virtual void Attack(){}
    protected void BaseBarrage()
    {
        int indexz = 0;
        int count = 30;
        for (int i = 0; i <= count; i++)
        {
            indexz += 360 / count;
            GameObject temp = Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, indexz));
            Allbullet.Add(temp);
        }
    }
}
