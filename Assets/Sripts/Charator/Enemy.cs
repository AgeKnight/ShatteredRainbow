using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BarrageType
{
    Shotgun,
    TrackShotgun,
    CircleBarrage,
    FirRoundGroup
}
public enum AttackType
{
    useBarrage,
    suicideAttack,
    nonAttack
}
public enum MoveType
{
    MoveToTarget,
    SomeTimesMove,
    StayAttackMove,
    ToPlayerMove,
}
[System.Serializable]
public struct EnemyBarrageCount
{
    //0 生成的彈幕個數,1 生成的彈幕波數,生成的子彈幕數量,3 生成的子彈幕波數
    public int[] count;
    public BarrageType barrageType;
    public GameObject barrage;
}
public class Enemy : MonoBehaviour
{
    #region "private"
    Coroutine coroutine;
    Coroutine nowCorotine;
    Coroutine[] otherCorotine = new Coroutine[10];
    Vector3 targetPosition;
    float ultimateAttackTime = 0;
    int nowCountBarrage = 0;
    bool canChooseBarrage = false;
    bool canAttack = false;
    bool isAttack = false;
    int nowIndex = 0;
    #endregion
    #region  "public"
    public AttackType useBarrage;
    public MoveType moveType;
    public Transform bulletTransform;
    public float Speed;
    public bool deadUseBarrage;
    #endregion
    #region "Hide"
    //[HideInInspector]
    public bool canTouch = true;
    [HideInInspector]
    public bool canCount = false;
    [HideInInspector]
    public Vector3[] Dot = new Vector3[10];
    [HideInInspector]
    public List<GameObject> Allbullet = new List<GameObject>();
    #endregion
    #region "調難度"
    [Header("調難度")]
    public EnemyBarrageCount[] enemyBarrageCounts;
    public EnemyBarrageCount ultimateAttack;
    public float countTime;
    public int indexMax = 1;
    #endregion
    void Start()
    {
        if (moveType != MoveType.ToPlayerMove)
            targetPosition = Dot[0];
        else
        {
            if (FindObjectOfType<Player>())
            {
                var temp = FindObjectOfType<Player>().gameObject;
                Vector3 dir = temp.transform.position - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 1);
            }
            else
            {
                Vector3 dir = GameManager.Instance.PlayerResurrectionPosition.position - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 1);
            }
            if (useBarrage == AttackType.useBarrage)
                Attack();
        }
        if (indexMax < 1)
            indexMax = 1;
        else if (indexMax > enemyBarrageCounts.Length)
            indexMax = enemyBarrageCounts.Length;
    }
    void Update()
    {
        if (canCount)
            ultimateAttackTime += Time.deltaTime;
        Move();
    }
    #region "移動"
    void ReturnMove()
    {
        for (int i = 0; i < Dot.Length; i++)
        {
            if (targetPosition == Dot[i])
            {
                if (i == Dot.Length - 1)
                    targetPosition = Dot[0];
                else
                    targetPosition = Dot[i + 1];
                break;
            }
        }
    }
    void Move()
    {
        if (!canAttack && moveType != MoveType.ToPlayerMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
            if (transform.position == targetPosition)
            {
                ReturnMove();
                canAttack = true;
                canChooseBarrage = false;
                if (useBarrage == AttackType.useBarrage)
                    Attack();
            }
        }
        else
        {
            switch (moveType)
            {
                case MoveType.SomeTimesMove:
                    if (transform.position == targetPosition && canChooseBarrage)
                    {
                        canChooseBarrage = false;
                        ReturnMove();
                    }
                    if (canChooseBarrage)
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                    break;
                case MoveType.StayAttackMove:
                    if (transform.position != targetPosition)
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                    else
                        ReturnMove();
                    break;
                case MoveType.ToPlayerMove:
                    transform.Translate(transform.forward * Time.deltaTime * Speed);
                    break;
            }
        }
    }
    #endregion
    #region "攻擊"
    public void Attack()
    {
        if (gameObject.GetComponent<Death>().enemyType == EnemyType.Trash)
            coroutine = StartCoroutine(UseBarrage());
        else
        {
            gameObject.GetComponent<Death>().hpBar.gameObject.SetActive(true);
            GameManager.Instance.ShowBossImage(gameObject.GetComponent<SpriteRenderer>().sprite);
            //加入變身動畫
            canTouch = true;
            if (FindObjectOfType<EnemyManager>().nowBossStage > 1)
            {
                canCount = true;
                coroutine = StartCoroutine(UltimateAttack());
            }
            else
               canBeginAttack();
        }
    }
    void canBeginAttack()
    {
        GameManager.Instance.BeginReciprocal();
        gameObject.GetComponent<Death>().isInvincible = false;
        canCount = false;
        ClearBarrage();
        coroutine = StartCoroutine(UseBarrage());
    }
    IEnumerator UltimateAttack()
    {
        while (true)
        {
            string nowBarrage = System.Enum.GetName(typeof(BarrageType), ultimateAttack.barrageType);
            nowCorotine = StartCoroutine(nowBarrage, ultimateAttack.count);
            yield return new WaitForSeconds(countTime);
            if (ultimateAttackTime > 3f)
            {
                canBeginAttack();
                break;
            }
        }
    }
    IEnumerator UseBarrage()
    {
        while (true)
        {
            if (FindObjectOfType<Player>() && !canChooseBarrage && !isAttack)
            {
                string nowBarrage = System.Enum.GetName(typeof(BarrageType), enemyBarrageCounts[nowIndex].barrageType);
                nowCorotine = StartCoroutine(nowBarrage, enemyBarrageCounts[nowIndex].count);
                yield return new WaitForSeconds(countTime);
            }
            else
                yield return null;
        }
    }
    void changeBarrage()
    {
        if (nowIndex >= indexMax - 1)
            nowIndex = 0;
        else
            nowIndex++;
    }
    void ChooseTypeBarrage()
    {
        if (nowCorotine != null)
        {
            StopCoroutine(nowCorotine);
            nowCorotine = null;
        }
        for (int i = 0; i < otherCorotine.Length; i++)
        {
            if (otherCorotine[i] != null)
            {
                StopCoroutine(otherCorotine[i]);
                otherCorotine[i] = null;
            }
        }
        nowCountBarrage += 1;
        if (nowCountBarrage >= enemyBarrageCounts[nowIndex].count[1])
        {
            nowCountBarrage = 0;
            changeBarrage();
            if (moveType == MoveType.SomeTimesMove)
                canChooseBarrage = true;
        }
        isAttack = false;
    }
    #region "所有彈幕方法"
    void Shotgun(int[] count)
    {
        float angle = Random.Range(90, 220);
        for (int j = 0; j < count[0]; j++)
        {
            GameObject temp = Instantiate(enemyBarrageCounts[nowIndex].barrage, bulletTransform.position, Quaternion.Euler(0, 0, angle));
            Allbullet.Add(temp);
            angle += 12;
        }
        ChooseTypeBarrage();
    }
    void TrackShotgun(int[] count)
    {
        var player = FindObjectOfType<Player>();
        Vector3 eulerAngle = GetAngle(transform.position, player.transform.position);
        eulerAngle.z -= 24;
        for (int j = 0; j < count[0]; j++)
        {
            GameObject temp = Instantiate(enemyBarrageCounts[nowIndex].barrage, bulletTransform.position, Quaternion.Euler(0, 0, eulerAngle.z));
            Allbullet.Add(temp);
            eulerAngle.z += 12;
        }
        ChooseTypeBarrage();
    }
    void CircleBarrage(int[] count)
    {
        int indexz = 0;
        for (int j = 0; j <= count[0]; j++)
        {
            indexz += 360 / count[0];
            GameObject temp = Instantiate(enemyBarrageCounts[nowIndex].barrage, bulletTransform.position, Quaternion.Euler(0, 0, indexz));
            Allbullet.Add(temp);
        }
        ChooseTypeBarrage();
    }
    IEnumerator CircleBarrage(int[] count, Vector3 Barrage, List<Bullet> Barrages)
    {

        int nowCount = 0;
        for (int i = 0; i < count[3]; i++)
        {
            int indexz = 0;
            if (FindObjectOfType<Player>())
            {
                for (int j = 0; j <= count[2]; j++)
                {
                    indexz += 360 / count[2];
                    GameObject temp = Instantiate(enemyBarrageCounts[nowIndex].barrage, Barrage, Quaternion.Euler(0, 0, indexz));
                    Allbullet.Add(temp);
                }
                nowCount = i;
                yield return new WaitForSeconds(countTime);
            }
            else
            {
                i = nowCount;
                yield return null;
            }
        }
        for (int i = 0; i < Barrages.Count; i++)
        {
            if (Barrages[i])
                Destroy(Barrages[i].gameObject);
        }

        ChooseTypeBarrage();
    }
    IEnumerator FirRoundGroup(int[] count)
    {
        bool exist = true;
        isAttack = true;
        int indexz = 0;
        List<Bullet> bullets = new List<Bullet>();
        List<Transform> transforms = new List<Transform>();
        for (int i = 0; i < count[0]; i++)
        {
            var temp = Instantiate(enemyBarrageCounts[nowIndex].barrage, bulletTransform.position, Quaternion.Euler(0, 0, indexz));
            indexz += 360 / count[0];
            bullets.Add(temp.GetComponent<Bullet>());
            Allbullet.Add(temp);
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i])
            {
                bullets[i].speed = 0;
                otherCorotine[i] = StartCoroutine(CircleBarrage(enemyBarrageCounts[nowIndex].count, bullets[i].transform.position, bullets));
            }
            else
            {
                exist = false;
                break;
            }
        }
        if (!exist)
        {
            ChooseTypeBarrage();
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
    #endregion
    public void ClearBarrage()
    {
        for (int i = 0; i < Allbullet.Count; i++)
        {
            if (Allbullet[i] != null)
                Allbullet[i].GetComponent<Bullet>().Die();
        }
        Allbullet.Clear();
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Barrier")
            Destroy(this.gameObject);
    }
}