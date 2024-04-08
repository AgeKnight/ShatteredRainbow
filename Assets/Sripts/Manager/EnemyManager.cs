using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum Wave
{
    OneColumn,
    TwoColumn,
    WholeLeftRow,
    WholeRightRow
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
    bool OtherStage = true;
    List<GameObject> waveEnemy = new List<GameObject>();
    GameObject tempEnemy;
    #endregion
    #region "Hide"
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
   // [HideInInspector]
    public int AllBossStaire;
    #endregion
    public WaveMonster[] waveMonster;
    public WaveBoss[] waveBosses;
    public float everyStairTime;
    void Update()
    {
        if(!canGoNext&&!isInBossAttack&&!isSpanBoss)
            nowEveryStairTime += Time.deltaTime;
    }
    public IEnumerator CreateEnemy()
    {
        GameManager.Instance.coroutine = null;
        while (true)
        {
            if (!isInBossAttack&&!isSpanBoss && nowCount < waveMonster[nowIndex].count)
            {
                for (int i = 0; i < waveMonster[nowIndex].count; i++)
                {
                    string nowWave = System.Enum.GetName(typeof(Wave), waveMonster[nowIndex].wave);
                    CreateNowEnemy(waveMonster[nowIndex].monsterPrefab, waveMonster[nowIndex].spanPosition, waveMonster[nowIndex].movePosition);
                    StartCoroutine(nowWave);
                    nowCount++;
                    yield return new WaitForSeconds(waveMonster[nowIndex].spanTime);
                }
            }
            else if (isInBossAttack)
            {
                isSpanBoss = true;
                isInBossAttack = false;
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
                if(nowEveryStairTime>=everyStairTime)
                {       
                    nowEveryStairTime = 0;
                    //是否可以前往下一階段
                    if ((isSpanBoss && !OtherStage) || !isSpanBoss)
                    {
                        allIndex++;
                        nowBossStage = 1;
                        if (isSpanBoss)
                            bossIndex++;
                    }
                    //boss進入二階段
                    if (isSpanBoss && OtherStage)
                        nowBossStage++;
                    //完全勝利
                    if (bossIndex >= waveBosses.Length)
                    {
                        isWin = true;
                        yield return new WaitForSeconds(1f);
                        GameManager.Instance.StageClear.SetActive(true);
                        StartCoroutine(ScoreBonus());
                        break;
                    }
                    nowCount = 0;
                    //進入下一階段
                    if (!isSpanBoss)
                        nowIndex++;
                    else
                        isSpanBoss = false;
                     
                    //防止溢出
                    if (nowIndex >= waveMonster.Length)
                        nowIndex = 0;
                    waveEnemy.Clear();
                    //開始進入boss戰
                    if (allIndex % (waveMonster.Length / 2 + 1) == waveMonster.Length / 2)
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
        GameManager.Instance.ChangeDifficulty(tempEnemy);
    }
    IEnumerator BossAppear()
    {
        AllBossStaire = waveBosses[bossIndex].bossPrefab.Length;
        OtherStage = true;
        isInBossAttack = true;
        if(bossIndex==waveBosses.Length)
            GameManager.Instance.AudioPlay(GameManager.Instance.BackMusic[1],false);
        if (nowBossStage == 1)
        {
            var items = FindObjectsOfType<Item>();
            for (int i = 0; i < items.Length; i++)
                items[i].CanAttract = true;
            yield return new WaitForSeconds(3f);
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
    IEnumerator ScoreBonus()
    {
        GameManager.Instance.MapBonusScores[1].text = GameManager.Instance.playerScore.ToString();
        GameManager.Instance.MapBonusScores[0].text = GameManager.Instance.thisMapScore.ToString();
        if (!GameManager.Instance.thisMapBomb)
        {
            yield return new WaitForSeconds(1.5f);
            GameManager.Instance.thisMapScore *= 1.5f;
            GameManager.Instance.BonusScores[0].SetActive(true);
        }
        if (!GameManager.Instance.thisMapHurt)
        {
            yield return new WaitForSeconds(1.5f);
            GameManager.Instance.thisMapScore *= 1.5f;
            GameManager.Instance.BonusScores[1].SetActive(true);
        }
        GameManager.Instance.thisMapBomb = false;
        GameManager.Instance.thisMapHurt = false;
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.AddScore(GameManager.Instance.thisMapScore);
        GameManager.Instance.MapBonusScores[1].text = GameManager.Instance.playerScore.ToString();
        GameManager.Instance.thisMapScore = 0;
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.StageClear.SetActive(false);
        GameManager.Instance.statusType = StatusType.Win;
    }
}
