using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    float moveTime = 0;
    public float allMoveTime=0.6f;
    protected bool canMove = true;
    protected Coroutine coroutine;
    protected Transform bulletTransform;
    protected Vector3 TargetPosition;
    public GameObject bullet;
    public float Speed;
    #region "Hide"
    [HideInInspector]
    public List<GameObject> Allbullet = new List<GameObject>();
    #endregion
    protected virtual void Start()
    {
        bulletTransform = gameObject.transform.GetChild(0).transform;
        Attack();
    }
    void Update() 
    {
        Move();
        if(!canMove)
            TimeReturn();
    }
    void TimeReturn()
    {
        moveTime+=Time.deltaTime;
        if(moveTime>=allMoveTime)
        {
            moveTime=0;
            ReturnMove();
            canMove=true;
        }
    }
    protected virtual void ReturnMove(){}
    protected virtual void Move() { }
    public void Attack()
    {
        coroutine = StartCoroutine(UseBarrage());
    }
    IEnumerator UseBarrage()
    {
        while (FindObjectOfType<Player>())
        {
            BarrageMethod();
            yield return new WaitForSeconds(gameObject.GetComponent<Death>().countTime);
        }
    }
    protected virtual void BarrageMethod() { }
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
    public void ClearBarrage()
    {
        for (int i = 0; i < Allbullet.Count; i++)
        {
            if (Allbullet[i] != null)
            {
                Destroy(Allbullet[i]);
            }
        }
        Allbullet.Clear();
    }
}
