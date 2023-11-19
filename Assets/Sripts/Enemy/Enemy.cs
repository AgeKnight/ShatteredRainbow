using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UseBarrageType
{
    useBarrage,
    nonUse
}
public enum NonUseType //同時選擇notmove與nonuse
{
    Barrier,
    DeathAfter
}
public enum MoveType
{
    NotMove,
    SomeTimesMove,
    ToPlayerMove
}
public class Enemy : MonoBehaviour
{
    #region "private"
    protected int barrageCount = 0;
    protected Vector3 targetPosition;
    protected Coroutine coroutine;
    protected Transform bulletTransform;
    protected bool canChooseBarrage = false;
    protected string nowUse;
    #endregion
    #region  "public"
    public UseBarrageType useBarrage;
    public MoveType moveType;
    public NonUseType nonUseType;
    public float allMoveTime = 0.6f;
    public GameObject bullet;
    public Transform[] Dot; //開始 結束
    public int[] spanCount;
    public int allBarragecount;
    public float Speed;
    public float countTime;
    #endregion
    #region "Hide"
    [HideInInspector]
    public List<GameObject> Allbullet = new List<GameObject>();
    #endregion
    void Start()
    {
        if (useBarrage == UseBarrageType.useBarrage)
        {
            nowUse = "Barrage";
            bulletTransform = gameObject.transform.GetChild(0).transform;
            Attack();
        }
        if (Dot.Length != 0)
        {
            targetPosition = Dot[0].position;
        }
    }
    void Update()
    {
        Move();
    }
    protected virtual void ReturnMove() { }
    protected virtual string changeBarrage() { return null; }
    void Move()
    {
        switch (moveType)
        {
            case MoveType.SomeTimesMove:
                if (transform.position == targetPosition && canChooseBarrage)
                {
                    canChooseBarrage = false;
                    nowUse = changeBarrage();
                    ReturnMove();
                    Attack();
                }
                if (canChooseBarrage)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                }
                break;
            case MoveType.ToPlayerMove:
                if (FindObjectOfType<Player>())
                {
                    var temp = FindObjectOfType<Player>().gameObject;
                    transform.position = Vector3.MoveTowards(transform.position, temp.transform.position, Speed * Time.deltaTime);
                }
                break;

        }
    }
    public void Attack()
    {
        coroutine = StartCoroutine(UseBarrage());
    }
    IEnumerator UseBarrage()
    {
        while (FindObjectOfType<Player>() && !canChooseBarrage)
        {
            Invoke(nowUse,0);
            barrageCount += 1;
            if (barrageCount >= allBarragecount)
            {
                barrageCount = 0;
                canChooseBarrage = true;
            }
            yield return new WaitForSeconds(countTime);
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
