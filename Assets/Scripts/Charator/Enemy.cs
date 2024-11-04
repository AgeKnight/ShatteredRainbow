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
    MachineGun,
    FireTurbine,
    FiveStar,
    Rain,
    LazerCatch,
    CircleRandomBarrage,
    MoveAttack,
    CrossAttack,
    TwoStar,
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
    public float[] count;
    public BarrageType barrageType;
    public GameObject[] barrage;
    public AudioSource Shootsound;
}
public class Enemy : MonoBehaviour
{
    #region "private"
    bool isMove = false;
    bool isUlimated = false;
    GameObject temp;
    AudioSource tempsound;
    float nowDownTime = 0;
    bool canMove = true;
    Death death;
    Coroutine[] otherCorotine = new Coroutine[1];
    Vector3 targetPosition;
    bool canChooseBarrage = false;
    bool canAttack = false;
    bool isAttack = false;
    int nowIndex = 0;
    #endregion
    #region  "public"
    public float DownTime;
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
        temp = enemyBarrageCounts[0].barrage[0];
        tempsound = enemyBarrageCounts[0].Shootsound;
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
        // if(!isAttack&&nowMusic)
        // {
        //     Destroy(nowMusic);
        //     isListenShoot = false;
        // }
        if (!canMove)
        {
            nowDownTime += Time.deltaTime;
            if (nowDownTime >= DownTime)
            {
                canMove = true;
                isMove = false;
                nowDownTime = 0;
            }
        }
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
                    if (transform.position != targetPosition && canMove)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                    }
                    else if (transform.position == targetPosition)
                    {
                        canMove = false;
                        canChooseBarrage = false;
                    }
                    break;
                case MoveType.StayAttackMove:
                    if (transform.position != targetPosition)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                    }
                    else if (transform.position == targetPosition)
                    {
                        changeBarrage();
                    }
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

            //GameManager.Instance.BarUse.Play("Open");  //播放血條動畫(開)
            GameManager.Instance.UIanimator.SetInteger("Boss", 0);
            canTouch = true;
            if (GameManager.Instance.enemyManager.nowBossStage > 1)
            {
                UltimateAttack();
            }
            else
                canBeginAttack();
        }
    }
    void canBeginAttack()
    {
        GameManager.Instance.BeginReciprocal();
        death.isInvincible = false;
        // ClearBarrage();
        StartCoroutine(UseBarrage());
    }
    void UltimateAttack()
    {
        if (!isAttack)
        {
            isAttack = true;
            isUlimated = true;
            string nowBarrage = System.Enum.GetName(typeof(BarrageType), death.ultimateAttack.barrageType);
            enemyBarrageCounts[0].barrage = death.ultimateAttack.barrage;
            enemyBarrageCounts[0].Shootsound = death.ultimateAttack.Shootsound;
            StartCoroutine(nowBarrage, death.ultimateAttack.count);
        }
    }
    IEnumerator UseBarrage()
    {
        while (true)
        {
            // if(!isListenShoot)
            // {
            //     isListenShoot = true;
            //     nowMusic = GameManager.Instance.AudioPlay(shoot,false);
            //     nowMusic.transform.parent = this.gameObject.transform;
            // }
            if (!canChooseBarrage && !isAttack)
            {
                isAttack = true;
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
        if (moveType == MoveType.SomeTimesMove)
        {
            canChooseBarrage = true;
            changeBarrage();
        }
        if (isUlimated)
        {
            enemyBarrageCounts[0].barrage[0] = temp;
            enemyBarrageCounts[0].Shootsound = tempsound;
            isUlimated = false;
            canBeginAttack();
        }
        isAttack = false;
    }
    #region "所有彈幕方法"
    /// <summary>
    /// 隨機攻擊玩家
    /// </summary>
    /// <param name="count">0總共幾發,1生成時間,變大時間</param>
    /// <returns></returns>
    IEnumerator LazerCatch(float[] count)
    {
        GameObject[] tempLazer = new GameObject[(int)count[0]];
        for (int i = 0; i < count[0]; i++)
        {
            float type = Random.Range(0, 3);
            float spanX = 0;
            float spanY = 0;
            float angle = 0;
            if (type == 0)
            {
                spanX = Random.Range(GameManager.Instance.mapPosition[0].transform.position.x - 0.3f, GameManager.Instance.mapPosition[1].transform.position.x + 0.3f);
                spanY = GameManager.Instance.mapPosition[0].transform.position.y + 0.3f;
                angle = Random.Range(-170, 10);
            }
            else if (type == 1)
            {
                spanX = GameManager.Instance.mapPosition[0].transform.position.x - 0.3f;
                spanY = Random.Range(GameManager.Instance.mapPosition[0].transform.position.y + 0.3f, GameManager.Instance.mapPosition[1].transform.position.y - 0.3f);
                angle = Random.Range(-80, 80);
            }
            else if (type == 2)
            {
                spanX = Random.Range(GameManager.Instance.mapPosition[0].transform.position.x - 0.3f, GameManager.Instance.mapPosition[1].transform.position.x + 0.3f);
                spanY = GameManager.Instance.mapPosition[1].transform.position.y - 0.3f;
                angle = Random.Range(10, 170);
            }
            else if (type == 3)
            {
                spanX = GameManager.Instance.mapPosition[1].transform.position.x + 0.3f;
                spanY = Random.Range(GameManager.Instance.mapPosition[0].transform.position.y + 0.3f, GameManager.Instance.mapPosition[1].transform.position.y - 0.3f);
                angle = Random.Range(100, 260);
            }
            Quaternion quaternion = Quaternion.Euler(0, 0, angle);
            tempLazer[i] = Instantiate(enemyBarrageCounts[nowIndex].barrage[0], new Vector2(spanX, spanY), quaternion);
            yield return new WaitForSeconds(count[1]);
        }
        yield return new WaitForSeconds(0.5f);
        //變大 --此處改成變大、消失全部綁在animator上
        for (int i = 0; i < count[0]; i++)
        {
            tempLazer[i].GetComponent<Animator>().SetBool("CanBig", true);
        }
        GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
        yield return new WaitForSeconds(count[2]);
        /*
        //消失
        for (int i = 0; i < count[0]; i++)
        {
            Destroy(tempLazer[i]);
        }
        */
        ChooseTypeBarrage();
    }
    /// <summary>
    /// 隨機攻擊玩家
    /// </summary>
    /// <param name="count">0每波彈幕數量,1總共幾波,2生成時間</param>
    /// <returns></returns>
    IEnumerator Shotgun(float[] count)
    {
        float angle = Random.Range(90, 220);
        for (int i = 0; i < count[1]; i++)
        {
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            for (int j = 0; j < count[0]; j++)
            {
                Quaternion quaternion = Quaternion.Euler(0, 0, angle);
                Instantiate(enemyBarrageCounts[nowIndex].barrage[0], bulletTransform.position, quaternion);
                angle += 12;
            }
            yield return new WaitForSeconds(count[2]);
        }

        ChooseTypeBarrage();
    }
    /// <summary>
    /// 
    /// </summary>
    /// barrage 0十字1一般機關槍
    /// <param name="count">0每波彈幕數量,1生成時間,2總共幾波</param>
    /// <returns></returns>
    IEnumerator CrossAttack(float[] count)
    {
        for (int i = 0; i < count[2]; i++)
        {
            float indexz = 0;
            float spanX = Random.Range(GameManager.Instance.mapPosition[0].transform.position.x + 0.5f, GameManager.Instance.mapPosition[1].transform.position.x - 0.5f);
            float spanY = Random.Range(GameManager.Instance.mapPosition[0].transform.position.y - 0.5f, GameManager.Instance.mapPosition[0].transform.position.y - 3f);
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            for (int j = 0; j < 4; j++)
            {
                Instantiate(enemyBarrageCounts[nowIndex].barrage[0], new Vector2(spanX, spanY), Quaternion.Euler(0, 0, indexz));
                indexz += 90;
            }
            indexz = 0;
            for (int j = 0; j <= count[0]; j++)
            {
                indexz += 360 / count[0];
                Instantiate(enemyBarrageCounts[nowIndex].barrage[1], new Vector2(spanX, spanY), Quaternion.Euler(0, 0, indexz));
            }
            yield return new WaitForSeconds(count[1]);
        }
        ChooseTypeBarrage();
    }
    /// <summary>
    /// 追蹤玩家
    /// </summary>
    /// <param name="count">0每波彈幕數量,1總共幾波,2生成時間</param>
    /// <returns></returns>
    IEnumerator TrackShotgun(float[] count)
    {
        Vector3 eulerAngle = new Vector3();


        for (int i = 0; i < count[1]; i++)
        {
           /*
            if(!FindObjectOfType<Player>())
            {
                break;
            }
           */
           
            if (GameManager.Instance.playerScript)
                eulerAngle = GetAngle(transform.position, GameManager.Instance.playerScript.transform.position);
            else
                eulerAngle = GetAngle(transform.position, GameManager.Instance.PlayerResurrectionPosition.transform.position);

            eulerAngle.z -= 6 * count[0];
            for (int j = 0; j < count[0]; j++)
            {

                Instantiate(enemyBarrageCounts[nowIndex].barrage[0], bulletTransform.position, Quaternion.Euler(0, 0, eulerAngle.z));
                eulerAngle.z += 12;

            }
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            yield return new WaitForSeconds(count[2]);
        }
        ChooseTypeBarrage();
    }
    /// <summary>
    /// 生成圓形彈幕
    /// </summary>
    /// <param name="count">0每波彈幕數量,1總共幾波,2偏移角度,3生成時間
    /// </param>
    /// <returns></returns>
    IEnumerator CircleBarrage(float[] count)
    {
        float indexz = 0;
        for (int i = 0; i < count[1]; i++)
        {
           /* if(!FindObjectOfType<Player>())
            {
                break;
            }*/
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            for (int j = 0; j <= count[0]; j++)
            {
                indexz += 360 / count[0];
                Instantiate(enemyBarrageCounts[nowIndex].barrage[0], bulletTransform.position, Quaternion.Euler(0, 0, indexz));
            }
            if (i % 3 == 2)
            {
                indexz += count[2];
            }
            yield return new WaitForSeconds(count[3]);
        }
        ChooseTypeBarrage();
    }
    /// <summary>
    /// 生成圓形彈幕，位置隨機
    /// </summary>
    /// <param name="count">0每波彈幕數量,1總共幾波,2生成時間
    /// </param>
    /// <returns></returns>
    IEnumerator CircleRandomBarrage(float[] count)
    {
        float indexz = 0;
        for (int i = 0; i < count[1]; i++)
        {
            float spanX = 0;
            float spanY = 0;
            float type = Random.Range(0, 3);
            if (type == 0)
            {
                spanX = Random.Range(GameManager.Instance.mapPosition[0].transform.position.x + 0.5f, GameManager.Instance.mapPosition[1].transform.position.x - 0.5f);
                spanY = GameManager.Instance.mapPosition[0].transform.position.y - 0.5f;
            }
            else if (type == 1)
            {
                spanX = GameManager.Instance.mapPosition[0].transform.position.x + 0.5f;
                spanY = Random.Range(GameManager.Instance.mapPosition[0].transform.position.y - 0.5f, GameManager.Instance.mapPosition[1].transform.position.y + 0.5f);
            }
            else if (type == 2)
            {
                spanX = Random.Range(GameManager.Instance.mapPosition[0].transform.position.x + 0.5f, GameManager.Instance.mapPosition[1].transform.position.x - 0.5f);
                spanY = GameManager.Instance.mapPosition[1].transform.position.y + 0.5f;
            }
            else if (type == 3)
            {
                spanX = GameManager.Instance.mapPosition[1].transform.position.x - 0.5f;
                spanY = Random.Range(GameManager.Instance.mapPosition[0].transform.position.y - 0.5f, GameManager.Instance.mapPosition[1].transform.position.y + 0.5f);
            }
            Allbullet.Add(Instantiate(enemyBarrageCounts[nowIndex].barrage[0], new Vector2(spanX, spanY), Quaternion.identity));
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            for (int j = 0; j <= count[0]; j++)
            {
                indexz += 360 / count[0];
                Instantiate(enemyBarrageCounts[nowIndex].barrage[1], new Vector2(spanX, spanY), Quaternion.Euler(0, 0, indexz));
            }
            yield return new WaitForSeconds(count[2]);
        }
        ClearBarrage();
        ChooseTypeBarrage();
    }
    /// <summary>
    /// 隨機生成彈幕，敵人俯衝玩家
    /// </summary>
    /// <param name="count">0總共幾波,1敵人速度,2阻礙彈幕數量,3生成時間
    /// </param>
    /// <returns></returns>
    IEnumerator MoveAttack(float[] count)
    {
        float spanX = 0;
        float spanY = 0;
        float tempX = 0;
        float tempY = 0;
        float DisX = 0;
        float DisY = 0;
        for (int i = 0; i < count[2]; i++)
        {
            if (i % 20 == 0)
            {
                tempX = Random.Range(GameManager.Instance.mapPosition[0].transform.position.x + 0.5f, GameManager.Instance.mapPosition[1].transform.position.x - 0.5f);
                tempY = Random.Range(GameManager.Instance.mapPosition[0].transform.position.y + 0.5f, GameManager.Instance.mapPosition[1].transform.position.y - 0.5f);
                if (tempX == spanX)
                {
                    tempX += 3f;
                }
                if (tempY == spanY)
                {
                    tempY += 3f;
                }
                DisX = Random.Range(-0.3f, 0.3f);
                if (DisX < 0.2f && DisX > 0)
                {
                    DisX = 0.2f;
                }
                else if (DisX > -0.2f && DisX < 0)
                {
                    DisX = -0.2f;
                }
                DisY = Random.Range(-0.3f, 0.3f);
                DisX = Random.Range(-0.3f, 0.3f);
                if (DisY < 0.2f && DisY > 0)
                {
                    DisY = 0.2f;
                }
                else if (DisY > -0.2f && DisY < 0)
                {
                    DisY = -0.2f;
                }
            }
            spanX = tempX + i % 20 * DisX;
            spanY = tempY + i % 20 * DisY;
            Allbullet.Add(Instantiate(enemyBarrageCounts[nowIndex].barrage[0], new Vector2(spanX, spanY), Quaternion.identity));
            Allbullet[i].GetComponent<Bullet>().speed = 0;
            yield return new WaitForSeconds(count[3]);
        }
        int countX = 0;
        Speed = count[1];
        if (!FindObjectOfType<Player>())
        {
            ClearBarrage();
        }
        while (true)
        {
            if (!isMove)
            {
                isMove = true;
                countX++;
                Vector3 eulerAngle = GetAngle(transform.position, GameManager.Instance.playerScript.transform.position);
                DownTime = 0.8f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, eulerAngle.z), 0.05f);
                targetPosition = GameManager.Instance.playerScript.transform.position;
            }
            else
            {
                yield return null;
            }
            if (countX > count[0] * 2 || gameObject.GetComponent<Death>().hp <= 0 || !FindObjectOfType<Player>())
            {
                ClearBarrage();
                break;
            }
        }
        targetPosition = Dot[0];
        ChooseTypeBarrage();
    }
    /// <summary>
    /// 機關槍式攻擊
    /// </summary>
    /// <param name="count">0每波彈幕數量,1生成時間</param>
    /// <returns></returns>
    IEnumerator MachineGun(float[] count)
    {
        //  Vector3 eulerAngle = new Vector3();

        for (int i = 0; i < count[0]; i++)
        {
            /*
            if (GameManager.Instance.playerScript)
                eulerAngle = GetAngle(transform.position, GameManager.Instance.playerScript.transform.position);
            else
                eulerAngle = GetAngle(transform.position, GameManager.Instance.PlayerResurrectionPosition.transform.position);
            Instantiate(enemyBarrageCounts[nowIndex].barrage, bulletTransform.position, Quaternion.Euler(0, 0, eulerAngle.z));
            yield return new WaitForSeconds(count[1]);
            */
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            //以玩家子彈跟蹤的基礎做的瞄準
            Vector3 vectorToTarget;
            vectorToTarget = GameManager.Instance.PlayerResurrectionPosition.transform.position - transform.position;
            if (GameManager.Instance.playerScript)
                vectorToTarget = GameManager.Instance.playerScript.transform.position - bulletTransform.position;

            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90; //方位計算 後面的-90拯救了這個部分 沒有他子彈是往反方向飛離 >:(
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);  //面對目標的rotation

            Instantiate(enemyBarrageCounts[nowIndex].barrage[0], bulletTransform.position, q);
            yield return new WaitForSeconds(count[1]);
        }

        ChooseTypeBarrage();
    }
    /// <summary>
    /// 出現兩顆子彈，不斷旋轉攻擊，途中瞄準敵人攻擊，最後兩顆子彈往下跑
    /// barrage 0雙子彈 1 旋轉子彈 2 瞄準敵人
    /// </summary>
    /// <param name="count">0幾波子彈,1生成時間</param>
    /// <returns></returns>
    IEnumerator TwoStar(float[] count)
    {
        GameObject Center1 = Instantiate(enemyBarrageCounts[nowIndex].barrage[0], new Vector2(-3, 2), Quaternion.Euler(0, 0, -180));
        GameObject Center2 = Instantiate(enemyBarrageCounts[nowIndex].barrage[0], new Vector2(3, 2), Quaternion.Euler(0, 0, -180));
        Center1.GetComponent<Bullet>().speed = 0;
        Center2.GetComponent<Bullet>().speed = 0;
        Vector3 Center1P = Center1.transform.position;
        Vector3 Center2P = Center2.transform.position;
        float indexz = 0;
        for (int i = 0; i < count[0]; i++)
        {
           /* if(!FindObjectOfType<Player>())
            {
                break;
            }
           */
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            Instantiate(enemyBarrageCounts[nowIndex].barrage[1], Center1P, Quaternion.Euler(0, 0, indexz));
            Instantiate(enemyBarrageCounts[nowIndex].barrage[1], Center2P, Quaternion.Euler(0, 0, indexz));
            indexz += 10;
            if (i > count[0] / 2 && i%10==0)
            {
                for (int j = 0; j < count[2]; j++)
                {
                    Vector3 eulerAngle1;
                    Vector3 eulerAngle2;
                    if (GameManager.Instance.playerScript)
                    {
                        eulerAngle1 = GetAngle(Center1.transform.position, GameManager.Instance.playerScript.transform.position);
                        eulerAngle2 = GetAngle(Center2.transform.position, GameManager.Instance.playerScript.transform.position);
                    }
                    else
                    {
                        eulerAngle1 = GetAngle(Center1.transform.position, GameManager.Instance.PlayerResurrectionPosition.transform.position);
                        eulerAngle2 = GetAngle(Center2.transform.position, GameManager.Instance.PlayerResurrectionPosition.transform.position);
                    }
                    Instantiate(enemyBarrageCounts[nowIndex].barrage[2], Center1P, Quaternion.Euler(0, 0, eulerAngle1.z));
                    Instantiate(enemyBarrageCounts[nowIndex].barrage[2], Center2P, Quaternion.Euler(0, 0, eulerAngle2.z));
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return new WaitForSeconds(count[1]);
        }
        Center1.GetComponent<Bullet>().speed = 5;
        Center2.GetComponent<Bullet>().speed = 5;
        ChooseTypeBarrage();
    }
    /// <summary>
    /// 旋轉式攻擊
    /// </summary>
    /// <param name="count">0總共幾波,1每波彈幕數量,2生成時間</param>
    /// <returns></returns>
    IEnumerator FiveStar(float[] count)
    {
        float indexz = 0;
        for (int i = 0; i < count[1]; i++)
        {
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            for (int j = 0; j < count[0]; j++)
            {
                Instantiate(enemyBarrageCounts[nowIndex].barrage[0], bulletTransform.position, Quaternion.Euler(0, 0, indexz));
                indexz += 360 / count[0];
            }
            indexz += 10;
            yield return new WaitForSeconds(count[2]);
        }
        ChooseTypeBarrage();
    }
    IEnumerator CircleBarrage(float[] count, Vector3 Barrage)
    {
        int nowCount = 0;
        float indexz = 0;
        for (int i = 0; i < count[1]; i++)
        {
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            if (FindObjectOfType<Player>())
            {
                if (nowCount % 3 == 2)
                    indexz += 20;
                for (int j = 0; j <= count[2]; j++)
                {
                    indexz += 360 / count[2];
                    Instantiate(enemyBarrageCounts[nowIndex].barrage[0], Barrage, Quaternion.Euler(0, 0, indexz));
                }
                nowCount = i;
                yield return new WaitForSeconds(countTime);
            }
            else
            {

                yield return null;
            }
        }
        ChooseTypeBarrage();
    }
    IEnumerator CircleBarrage2(float[] count, Vector3 Barrage)
    {
        float indexz = 0;
        for (int j = 0; j <= count[1]; j++)
        {
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            indexz += 360 / count[1];
            Instantiate(enemyBarrageCounts[nowIndex].barrage[0], Barrage, Quaternion.Euler(0, 0, indexz));
        }
        yield return new WaitForSeconds(countTime);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="count">0每波數量,1,2生成角度,3生成時間,4,5飛行時間</param>
    /// <returns></returns>
    IEnumerator Rain(float[] count)
    {
        for (int i = 0; i < count[0]; i++)
        {
            float indexz = Random.Range(count[1], count[2]);
            Bullet bullet = Instantiate(enemyBarrageCounts[nowIndex].barrage[0], bulletTransform.position, Quaternion.Euler(0, 0, indexz)).GetComponent<Bullet>();
            bullet.AllRainTime = Random.Range(count[4], count[5]);
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            bullet.rain = true;
            bullet.canWallDestroy = false;
            yield return new WaitForSeconds(count[3]);
        }
        ChooseTypeBarrage();
    }
    /// <summary>
    /// 螺旋式攻擊
    /// </summary>
    /// <param name="count">0 生成幾波,1 每波數量,2生成時間,3生成半径,4每生成一次增加的距离</param>
    /// <returns></returns>
    IEnumerator FireTurbine(float[] count)
    {
        Vector3 bulletDir = bulletTransform.transform.up;      //发射方向
        Quaternion rotateQuate = Quaternion.AngleAxis(20, Vector3.forward);//使用四元数制造绕Z轴旋转20度的旋转
        float radius = count[3];        //生成半径
        float distance = count[4];      //每生成一次增加的距离
        for (int i = 0; i < count[0]; i++)
        {
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            Vector3 firePoint = bulletTransform.position + bulletDir * radius;   //使用向量计算生成位置
            List<Coroutine> list = new List<Coroutine>(otherCorotine.ToList());
            list.Add(StartCoroutine(CircleBarrage2(enemyBarrageCounts[nowIndex].count, firePoint)));
            otherCorotine = list.ToArray();
            yield return new WaitForSeconds(count[2]);     //延时较小的时间（为了表现效果），计算下一步
            bulletDir = rotateQuate * bulletDir;        //发射方向改变
            radius += distance;     //生成半径增加
        }
        ChooseTypeBarrage();
    }
    /// <summary>
    /// 多圓盤攻擊
    /// </summary>
    /// <param name="count">0生成幾波,1每顆生出幾波,2每波幾顆彈幕,3等待時間</param>
    /// <returns></returns>
    //0 生成的彈幕個數,1 生成的彈幕波數,生成的子彈幕數量,3 生成的子彈幕波數
    IEnumerator FirRoundGroup(float[] count)
    {
        bool exist = true;
        float indexz = 0;
        List<Bullet> bullets = new List<Bullet>();
        for (int i = 0; i < count[0]; i++)
        {
            GameManager.Instance.AudioPlay(enemyBarrageCounts[nowIndex].Shootsound, true);
            var temp = Instantiate(enemyBarrageCounts[nowIndex].barrage[0], bulletTransform.position, Quaternion.Euler(0, 0, indexz));
            indexz += 360 / count[0];
            bullets.Add(temp.GetComponent<Bullet>());
        }
        yield return new WaitForSeconds(count[3]);
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
    //求方位
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