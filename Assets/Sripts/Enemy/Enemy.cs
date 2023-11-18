using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UseBarrageType
{
    useBarrage,
    nonUse
}
public enum MoveType
{
    NotMove,
    SomeTimesMove,
    AllwaysMove
}
public class Enemy : MonoBehaviour
{
    #region "private"
    float moveTime = 0;
    protected bool canMove = true;
    protected Coroutine coroutine;
    protected Transform bulletTransform;
    #endregion
    #region  "public"
    public UseBarrageType useBarrage;
    public MoveType moveType;
    public float allMoveTime=0.6f;
    public GameObject bullet;
    public float Speed;
    #endregion
    #region "Hide"
    [HideInInspector]
    public List<GameObject> Allbullet = new List<GameObject>();
    #endregion
    protected virtual void Start()
    {
        if(useBarrage==UseBarrageType.useBarrage)
        {
            bulletTransform = gameObject.transform.GetChild(0).transform;
            Attack();
        }     
    }
    void Update() 
    {
        Move();
        if(!canMove&&moveType==MoveType.SomeTimesMove)
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
