using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UseBarrageType
{
    useBarrage,
    deadUseBarrage,
    nonUse
}
public enum MoveType
{
    NotMove,
    MoveToTarget,
    SomeTimesMove,
    StayAttackMove,
    ToPlayerMove,
}
[System.Serializable]
public struct EnemyBarrageCount
{
    //生成的彈幕個數
    public int spanCount;
    //生成的彈幕波數
    public int countBarrage;
}
public class Enemy : MonoBehaviour
{
    #region "private"
    protected Vector3 targetPosition;
    protected Coroutine coroutine;
    protected Transform bulletTransform;
    protected bool canChooseBarrage = false;
    protected string nowUse;
    protected int nowIndex = 0;
    protected int nowCountBarrage = 0;
    #endregion
    #region  "public"
    public UseBarrageType useBarrage;
    public MoveType moveType;
    public GameObject bullet;
    //移動的位置
    public Transform[] Dot;
    public float Speed;
    #endregion
    #region "Hide"
    [HideInInspector]
    public List<GameObject> Allbullet = new List<GameObject>();
    #endregion
    #region "調難度"
    [Header("調難度")]
    public EnemyBarrageCount[] enemyBarrageCounts;
    public float countTime;
    #endregion
    protected virtual void Start()
    {
        if (moveType == MoveType.MoveToTarget)
            canChooseBarrage = true;
        if(useBarrage != UseBarrageType.nonUse)
            bulletTransform = gameObject.transform.GetChild(0).transform;
        if (useBarrage == UseBarrageType.useBarrage)
            Attack();
        if (Dot.Length != 0)
            targetPosition = Dot[0].position;
    }
    void Update()
    {
        Move();
    }
    void ReturnMove()
    {
        for (int i = 0; i < Dot.Length; i++)
        {
            if (targetPosition == Dot[i].position)
            {
                if (i == Dot.Length - 1)
                    targetPosition = Dot[0].position;
                else
                    targetPosition = Dot[i + 1].position;
                break;
            }
        }
    }
    protected virtual string changeBarrage() { return null; }
    void Move()
    {
        switch (moveType)
        {
            case MoveType.SomeTimesMove:
                if (transform.position == targetPosition && canChooseBarrage)
                {
                    canChooseBarrage = false;
                    ReturnMove();
                    Attack();
                }
                if (canChooseBarrage)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                }
                break;
            case MoveType.MoveToTarget:
                if (transform.position != targetPosition)
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                else
                {
                    if (canChooseBarrage)
                    {
                        canChooseBarrage = false;
                        Attack();
                    }
                }
                break;
            case MoveType.StayAttackMove:
                if (transform.position != targetPosition)
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                else
                    ReturnMove();
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
            StartCoroutine(nowUse, enemyBarrageCounts[nowIndex].spanCount);
            nowCountBarrage += 1;
            if (nowCountBarrage >= enemyBarrageCounts[nowIndex].countBarrage)
            {
                nowCountBarrage = 0;
                nowUse = changeBarrage();
                if (moveType == MoveType.SomeTimesMove)
                    canChooseBarrage = true;
            }
            yield return new WaitForSeconds(countTime);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Barrier")
        {
            Destroy(this.gameObject);
        }
    }
    #region "所有彈幕方法"
    void Shotgun(int count)
    {
        float angle = Random.Range(90, 220);
        for (int i = 0; i < count; i++)
        {
            GameObject temp = Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, angle));
            temp.transform.parent = gameObject.transform;
            angle += 12;
        }
    }
    void TrackShotgun(int count)
    {
        if (FindObjectOfType<Player>())
        {
            var player = FindObjectOfType<Player>();
            Vector3 eulerAngle = GetAngle(transform.position, player.transform.position);
            eulerAngle.z -= 24;
            for (int i = 0; i < count; i++)
            {
                GameObject temp = Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, eulerAngle.z));
                temp.transform.parent = gameObject.transform;
                eulerAngle.z += 12;
            }
        }
    }
    void CircleBarrage(int count)
    {
        int indexz = 0;
        for (int i = 0; i <= count; i++)
        {
            indexz += 360 / count;
            GameObject temp = Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, indexz));
            temp.transform.parent = gameObject.transform;
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
    #endregion

}
