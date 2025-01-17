using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


public enum Wave
{
    OneColumn,
    TwoColumn,
    WholeLeftRow,
    WholeRightRow
}
[System.Serializable]
public struct BonusMonster
{
    public GameObject BonusPrefab;
    [Range(0f, 100f)] public float probability;
}
[System.Serializable]
public struct WaveMonster
{
    public GameObject monsterPrefab;
    public int count;
    public float spanTime;
    public Wave wave;
    public Transform spanPosition;
    public Transform[] movePosition;
}
[System.Serializable]
public struct WaveBoss
{
    public GameObject[] bossPrefab;
    public Transform spanPosition;
    public Transform[] movePosition;
}
public class EnemyManager : MonoBehaviour
{
    #region "Private"
    int nowIndex = 0;
    int nowCount = 0;
    int allIndex = 0;
    bool isInBossAttack = false;
    List<GameObject> waveEnemy = new List<GameObject>();
    GameObject tempEnemy;
    #endregion
    #region "Hide"
    [HideInInspector]
    public bool OtherStage = false;
    [HideInInspector]
    public float nowEveryStairTime = 0;
    [HideInInspector]
    public bool canGoNext = true;
    [HideInInspector]
    public bool isSpanBoss = false;

    [HideInInspector]
    public bool isWin = false;
    [HideInInspector]
    public int nowBossStage = 1;
    [HideInInspector]
    public int bossIndex = 0;
    [HideInInspector]
    public int AllBossStaire;
    #endregion
    public WaveMonster[] waveMonster;
    public WaveBoss[] waveBosses;
    public BonusMonster[] BonusMonster;
    public float everyStairTime;
    public int allEnemySpan;
    void Update()
    {
        if (!canGoNext && !isInBossAttack && !isSpanBoss)
            nowEveryStairTime += Time.deltaTime;
    }
    public void EnemySpanOper()
    {
        allEnemySpan +=GameManager.Instance.DifficulAllIndex;
        if(allEnemySpan%2==1)
        {
            allEnemySpan -= 1;   
        }
        else if(allEnemySpan>waveMonster.Length)
        {
            allEnemySpan = waveMonster.Length;
        }
        else if(allEnemySpan<=0)
        {
            allEnemySpan = 2;
        }

    }
    void SpanBonus()
    {
        int bonusE = Random.Range(0,BonusMonster.Length-1);
        if(BonusMonster[bonusE].probability>=Random.Range(0,100))
        {
            //隨機生成出生點
            int type = Random.Range(0,1);
            float spanX = 0,spanY = 0,moveX=0,moveY=0,upDown=0,leftRight=0;
            if(type==0)
            {
                spanX = Random.Range(GameManager.Instance.mapPosition[0].transform.position.x,GameManager.Instance.mapPosition[1].transform.position.x);
                upDown = Random.Range(0,1);
                if(upDown==0)
                    spanY = GameManager.Instance.mapPosition[0].transform.position.y;
                else
                    spanY = GameManager.Instance.mapPosition[1].transform.position.y;
            }
            else
            {
                spanY = Random.Range(GameManager.Instance.mapPosition[0].transform.position.y,GameManager.Instance.mapPosition[1].transform.position.y);
                leftRight = Random.Range(0,1);
                if(leftRight==0)
                    spanX = GameManager.Instance.mapPosition[0].transform.position.x;
                else
                    spanX = GameManager.Instance.mapPosition[1].transform.position.x;
            }
            GameObject BonusEnemy = Instantiate(BonusMonster[bonusE].BonusPrefab, new Vector2(spanX,spanY), Quaternion.identity);
            if(type==0&&upDown==0)
            {
                moveX = Random.Range(GameManager.Instance.mapPosition[0].transform.position.x,GameManager.Instance.mapPosition[1].transform.position.x);
                moveY = GameManager.Instance.mapPosition[1].transform.position.y-3;
            }
            else if(type==0&&upDown==1)
            {
                moveX = Random.Range(GameManager.Instance.mapPosition[0].transform.position.x,GameManager.Instance.mapPosition[1].transform.position.x);
                moveY = GameManager.Instance.mapPosition[0].transform.position.y+3;
            }
            else if(type==1&&leftRight==0)
            {
                moveY = Random.Range(GameManager.Instance.mapPosition[0].transform.position.y,GameManager.Instance.mapPosition[1].transform.position.y);
                moveX = GameManager.Instance.mapPosition[1].transform.position.x+3;
            }
            else if(type==1&&leftRight==1)
            {
                moveY = Random.Range(GameManager.Instance.mapPosition[0].transform.position.y,GameManager.Instance.mapPosition[1].transform.position.y);
                moveX = GameManager.Instance.mapPosition[0].transform.position.x-3;
            }
            List<Vector3> list = new List<Vector3>(BonusEnemy.GetComponent<Enemy>().Dot.ToList());
            list.Add(new Vector3(moveX,moveY,0));
            BonusEnemy.GetComponent<Enemy>().Dot = list.ToArray();
        }
    }
    public IEnumerator CreateEnemy()
    {
        GameManager.Instance.coroutine = null;
        while (true)
        {
            if (!isInBossAttack && !isSpanBoss && nowCount < waveMonster[nowIndex].count)
            {
                for (int i = 0; i < waveMonster[nowIndex].count; i++)
                {
                    string nowWave = System.Enum.GetName(typeof(Wave), waveMonster[nowIndex].wave);
                    CreateNowEnemy(waveMonster[nowIndex].monsterPrefab, waveMonster[nowIndex].spanPosition, waveMonster[nowIndex].movePosition);
                    StartCoroutine(nowWave);
                    nowCount++;
                    SpanBonus();
                    yield return new WaitForSeconds(waveMonster[nowIndex].spanTime);
                }
            }
            else if (isInBossAttack)
            {
                isSpanBoss = true;
                isInBossAttack = false;
                GameManager.Instance.ClearBarrage();
                CreateNowEnemy(waveBosses[bossIndex].bossPrefab[nowBossStage - 1], waveBosses[bossIndex].spanPosition, waveBosses[bossIndex].movePosition);
                if (nowBossStage >= waveBosses[bossIndex].bossPrefab.Length)
                    OtherStage = false;
                else
                    waveBosses[bossIndex].spanPosition = waveBosses[bossIndex].movePosition[0];
                nowCount = waveMonster[nowIndex].count + 1;
                yield return null;
            }
            else
            {
                if (nowEveryStairTime >= everyStairTime)
                {
                    nowEveryStairTime = 0;
                    //是否可以前往下一階段
                    if ((isSpanBoss && !OtherStage) || !isSpanBoss)
                    {
                        allIndex++;
                        nowBossStage = 1;
                        if (isSpanBoss)
                        {
                            bossIndex++;
                        }
                    }
                    //boss進入二階段
                    if (isSpanBoss && OtherStage)
                        nowBossStage++;
                    //完全勝利
                    if (bossIndex >= waveBosses.Length)
                    {
                        StartCoroutine(GameManager.Instance.StageResults()); //關卡完成畫面顯示&獎勵分數計算搬至gamemanager並帶入動畫
                        break;
                    }
                    nowCount = 0;
                    //進入下一階段
                    if (!isSpanBoss)
                        nowIndex++;
                    else
                    {
                        GameManager.Instance.awardType = AwardType.Bonus;
                        isSpanBoss = false;
                    }


                    //防止溢出
                    if (nowIndex >= waveMonster.Length)
                        nowIndex = 0;
                    waveEnemy.Clear();
                    //開始進入boss戰
                    //all index目前小怪+boss波數
                    if (allIndex % (allEnemySpan / 2 + 1) == allEnemySpan / 2)
                    {       
                        StartCoroutine(BossAppear());
                        break;
                    }
                }
                yield return null;
            }
        }
    }
    void CreateNowEnemy(GameObject prefab, Transform transform, Transform[] movePosition)
    {
        tempEnemy = Instantiate(prefab, transform.position, Quaternion.identity);
        if (isSpanBoss)
        {
            tempEnemy.GetComponent<Enemy>().canTouch = false;
            tempEnemy.GetComponent<Death>().isInvincible = true;
            GameManager.Instance.BossBar.value = 1;
            tempEnemy.GetComponent<Death>().hpBar = GameManager.Instance.BossBar;
        }
        for (int i = 0; i < movePosition.Length; i++)
        {
            List<Vector3> list = new List<Vector3>(tempEnemy.GetComponent<Enemy>().Dot.ToList());
            list.Add(movePosition[i].position);
            tempEnemy.GetComponent<Enemy>().Dot = list.ToArray();
        }
        waveEnemy.Add(tempEnemy);
    }
    IEnumerator BossAppear()
    {
        if (GameManager.Instance.playerScript)
            GameManager.Instance.playerScript.againUseBomb();
        AllBossStaire = waveBosses[bossIndex].bossPrefab.Length;
        OtherStage = true;
        isInBossAttack = true;

        if (nowBossStage == 1)
        {
            var items = FindObjectsOfType<Item>();
            for (int i = 0; i < items.Length; i++)
                items[i].CanAttract = true;
            yield return new WaitForSeconds(3f);
            if (bossIndex == waveBosses.Length - 1)
            {
                StartCoroutine(GameManager.Instance.BGMchange(GameManager.Instance.BackMusic[1]));//boss音樂切換
            }
        }
        StartCoroutine(CreateEnemy());
    }
    void OneColumn() { }
    void TwoColumn()
    {
        for (int i = 0; i < waveMonster[nowIndex].movePosition.Length; i++)
        {
            float tempPosition = (GameManager.Instance.mapPosition[0].transform.position.x + GameManager.Instance.mapPosition[1].transform.position.x) / 2 - waveMonster[nowIndex].movePosition[i].position.x;
            waveMonster[nowIndex].movePosition[i].position = new Vector3(tempPosition, waveMonster[nowIndex].movePosition[i].position.y, waveMonster[nowIndex].movePosition[i].position.z);
        }
        float tempX = (GameManager.Instance.mapPosition[0].transform.position.x + GameManager.Instance.mapPosition[1].transform.position.x) / 2 - waveMonster[nowIndex].spanPosition.position.x;
        waveMonster[nowIndex].spanPosition.position = new Vector3(tempX, waveMonster[nowIndex].spanPosition.position.y, waveMonster[nowIndex].spanPosition.position.z);
    }
    void WholeLeftRow()
    {
        float distance = -0.6f;
        float tempX = waveMonster[nowIndex].spanPosition.position.x + distance;
        waveMonster[nowIndex].spanPosition.position = new Vector3(tempX, waveMonster[nowIndex].spanPosition.position.y, waveMonster[nowIndex].spanPosition.position.z);
        for (int i = 0; i < waveMonster[nowIndex].movePosition.Length; i++)
        {
            float tempPosition = waveMonster[nowIndex].movePosition[i].position.x + distance;
            waveMonster[nowIndex].movePosition[i].position = new Vector3(tempPosition, waveMonster[nowIndex].movePosition[i].position.y, waveMonster[nowIndex].movePosition[i].position.z);
        }
    }
    void WholeRightRow()
    {
        float distance = 0.6f;
        float tempX = waveMonster[nowIndex].spanPosition.position.x + distance;
        waveMonster[nowIndex].spanPosition.position = new Vector3(tempX, waveMonster[nowIndex].spanPosition.position.y, waveMonster[nowIndex].spanPosition.position.z);
        for (int i = 0; i < waveMonster[nowIndex].movePosition.Length; i++)
        {
            float tempPosition = waveMonster[nowIndex].movePosition[i].position.x + distance;
            waveMonster[nowIndex].movePosition[i].position = new Vector3(tempPosition, waveMonster[nowIndex].movePosition[i].position.y, waveMonster[nowIndex].movePosition[i].position.z);
        }
    }

}
