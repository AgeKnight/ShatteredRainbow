using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum BarrageType
{
    Shotgun,
    TrackShotgun,
    CircleBarrage,
    FirRoundGroup,
    MachineGun
}
public enum AttackType
{
    useBarrage,
    suicideAttack,
    nonAttack
}
public enum MoveType
{
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
    Death death;
    Coroutine[] otherCorotine = new Coroutine[1];
    Coroutine coroutine;
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
    [HideInInspector]
    //觸碰到會不會死
    public bool canTouch = true;
    [HideInInspector]
    //可不可以記數
    public bool canCount = false;
    //[HideInInspector]
    public Vector3[] Dot;
    [HideInInspector]
    public List<GameObject> Allbullet = new List<GameObject>();
    #endregion
    #region "調難度"
    [Header("調難度")]
    public EnemyBarrageCount[] enemyBarrageCounts;
    public float countTime;
    #endregion
    void Start()
    {
        death = gameObject.GetComponent<Death>();
        if (moveType != MoveType.ToPlayerMove)
            targetPosition = Dot[0];
        else
        {
            Vector3 eulerAngle = new Vector3();
            if (GameManager.Instance.playerScript)
                eulerAngle = GetAngle(transform.position, GameManager.Instance.playerScript.transform.position);
            else
                eulerAngle = GetAngle(transform.position, GameManager.Instance.PlayerResurrectionPosition.transform.position);
            transform.Rotate(eulerAngle);
            if (useBarrage == AttackType.useBarrage)
                Attack();
        }
        if (death.indexMax > enemyBarrageCounts.Length)
            death.indexMax = enemyBarrageCounts.Length;
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
        Debug.Log(1);
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
                    if (transform.position != targetPosition)
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                    else
                        canChooseBarrage = false;
                    break;
                case MoveType.StayAttackMove:
                     if (transform.position != targetPosition)
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                    else
                        changeBarrage();
                    break;
                case MoveType.ToPlayerMove:
                    transform.Translate(Vector3.up * Time.deltaTime * Speed, Space.Self);
                    break;
            }
        }
    }
    #endregion
    #region "攻擊"
    public void Attack()
    {
        if (death.enemyType == EnemyType.Trash)
            StartCoroutine(UseBarrage());
        else
        {
            death.hpBar.gameObject.SetActive(true);
            GameManager.Instance.Triangles[0].SetActive(true);
            GameManager.Instance.Triangles[1].SetActive(true);
            GameManager.Instance.ShowBossStaire(GameManager.Instance.enemyManager.AllBossStaire, GameManager.Instance.enemyManager.nowBossStage);
            //播放血條動畫(開)
            GameManager.Instance.BarUse.Play("Open");
            canTouch = true;
            if (GameManager.Instance.enemyManager.nowBossStage > 1)
            {
                canCount = true;
                StartCoroutine(UltimateAttack());
            }
            else
                canBeginAttack();
        }
    }
    void canBeginAttack()
    {
        GameManager.Instance.BeginReciprocal();
        death.isInvincible = false;
        canCount = false;
        ClearBarrage();
        StartCoroutine(UseBarrage());
    }
    IEnumerator UltimateAttack()
    {
        while (true)
        {
            string nowBarrage = System.Enum.GetName(typeof(BarrageType), death.ultimateAttack.barrageType);
            StartCoroutine(nowBarrage, death.ultimateAttack.count);
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
            if (!canChooseBarrage && !isAttack)
            {
                string nowBarrage = System.Enum.GetName(typeof(BarrageType), enemyBarrageCounts[nowIndex].barrageType);
                StartCoroutine(nowBarrage, enemyBarrageCounts[nowIndex].count);
                yield return new WaitForSeconds(countTime);
            }
            else
                yield return null;
        }
    }
    void changeBarrage()
    {
        if (nowIndex >= death.indexMax - 1)
            nowIndex = 0;
        else
            nowIndex++;
        ReturnMove();
    }
    void ChooseTypeBarrage()
    {
        for (int i = 0; i < otherCorotine.Length; i++)
        {
            if (otherCorotine[i] != null)
            {
                StopCoroutine(otherCorotine[i]);
                otherCorotine[i] = null;
            }
        }
        nowCountBarrage += 1;
        if (nowCountBarrage == enemyBarrageCounts[nowIndex].count[1])
        {
            nowCountBarrage = 0;
            if (moveType == MoveType.SomeTimesMove)
            {
                canChooseBarrage = true;
                changeBarrage();
            }
        }
        isAttack = false;
    }
    #region "所有彈幕方法"
    void Shotgun(int[] count)
    {
        float angle = Random.Range(90, 220);
        for (int j = 0; j < count[0]; j++)
        {
            Quaternion quaternion = Quaternion.Euler(0, 0, angle);
            Instantiate(enemyBarrageCounts[nowIndex].barrage, bulletTransform.position, quaternion);
            angle += 12;
        }
        ChooseTypeBarrage();
    }
    void TrackShotgun(int[] count)
    {
        Vector3 eulerAngle = new Vector3();
        if (GameManager.Instance.playerScript)
            eulerAngle = GetAngle(transform.position, GameManager.Instance.playerScript.transform.position);
        else
            eulerAngle = GetAngle(transform.position, GameManager.Instance.PlayerResurrectionPosition.transform.position);
        for (int j = 0; j < count[0]; j++)
        {
            Instantiate(enemyBarrageCounts[nowIndex].barrage, bulletTransform.position, Quaternion.Euler(0, 0, eulerAngle.z));
            eulerAngle.z += 12;
        }
        ChooseTypeBarrage();
    }
    void CircleBarrage(int[] count)
    {
        int indexz = count[2];
        for (int j = 0; j <= count[0]; j++)
        {
            indexz += 360 / count[0];
            Instantiate(enemyBarrageCounts[nowIndex].barrage, bulletTransform.position, Quaternion.Euler(0, 0, indexz));
        }
        if (nowCountBarrage % 3 == 2)
            enemyBarrageCounts[nowIndex].count[2] += 20;
        ChooseTypeBarrage();
    }
    IEnumerator MachineGun(int[] count)
    {
        Vector3 eulerAngle = new Vector3();
        isAttack = true;
        if (GameManager.Instance.playerScript)
            eulerAngle = GetAngle(transform.position, GameManager.Instance.playerScript.transform.position);
        else
            eulerAngle = GetAngle(transform.position, GameManager.Instance.PlayerResurrectionPosition.transform.position);
        for (int i = 0; i < count[0]; i++)
        {
            Instantiate(enemyBarrageCounts[nowIndex].barrage, bulletTransform.position, Quaternion.Euler(0, 0, eulerAngle.z));
            yield return new WaitForSeconds(0.1f);
        }
        ChooseTypeBarrage();
    }
    IEnumerator CircleBarrage(int[] count, Vector3 Barrage)
    {
        int nowCount = 0;
        int indexz = 0;
        for (int i = 0; i < count[3]; i++)
        {
            if (FindObjectOfType<Player>())
            {
                if (nowCount % 3 == 2)
                    indexz += 20;
                for (int j = 0; j <= count[2]; j++)
                {
                    indexz += 360 / count[2];
                    Instantiate(enemyBarrageCounts[nowIndex].barrage, Barrage, Quaternion.Euler(0, 0, indexz));
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
        ChooseTypeBarrage();
    }
    IEnumerator FirRoundGroup(int[] count)
    {
        bool exist = true;
        isAttack = true;
        int indexz = 0;
        List<Bullet> bullets = new List<Bullet>();
        for (int i = 0; i < count[0]; i++)
        {
            var temp = Instantiate(enemyBarrageCounts[nowIndex].barrage, bulletTransform.position, Quaternion.Euler(0, 0, indexz));
            indexz += 360 / count[0];
            bullets.Add(temp.GetComponent<Bullet>());
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i])
            {
                bullets[i].speed = 0;
                List<Coroutine> list = new List<Coroutine>(otherCorotine.ToList());
                list.Add(StartCoroutine(CircleBarrage(enemyBarrageCounts[nowIndex].count, bullets[i].transform.position)));
                otherCorotine = list.ToArray();
            }
            else
            {
                exist = false;
                break;
            }
        }
        if (!exist)
            ChooseTypeBarrage();
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