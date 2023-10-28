using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Coroutine coroutine;
    public GameObject bullet;
    public Transform bulletTransform;
    #region "Hide"
    [HideInInspector]
    public List<GameObject> Allbullet = new List<GameObject>();
    public float countTime;
    #endregion
    void Start()
    {
        Attack();
    }
    public void Attack()
    {
        coroutine = StartCoroutine(UseBarrage());
    } 
    IEnumerator UseBarrage()
    {
        while (FindObjectOfType<Player>())
        {
            BarrageMethod();
            yield return new WaitForSeconds(countTime);
        }
    }
    protected virtual void BarrageMethod(){}
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
