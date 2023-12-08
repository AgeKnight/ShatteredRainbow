using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Wave
{
    OneColumn,
    TwoColumn,
    WholeRow
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
public class EnemyManager : MonoBehaviour
{
    int nowIndex = 0;
    int nowCount = 0;
    List<GameObject> waveEnemy = new List<GameObject>();
    public WaveMonster[] waveMonster;
    public IEnumerator CreateEnemy()
    {
        while (true)
        {
            if (FindObjectOfType<Player>() && nowCount < waveMonster[nowIndex].count)
            {
                for (int i = 0; i < waveMonster[nowIndex].count; i++)
                {
                    string nowWave = System.Enum.GetName(typeof(Wave), waveMonster[nowIndex].wave);
                    StartCoroutine(nowWave);
                    yield return new WaitForSeconds(waveMonster[nowIndex].spanTime);
                }
            }
            else
            {
                bool allEnemyDie = true;
                if (nowCount >= waveMonster[nowIndex].count)
                {
                    for (int i = 0; i < waveEnemy.Count; i++)
                    {
                        if (waveEnemy[i] != null)
                        {
                            allEnemyDie = false;
                        }
                    }
                }
                if (allEnemyDie)
                {
                    nowCount = 0;
                    nowIndex++;
                    waveEnemy.Clear();
                    if (nowIndex >= waveMonster.Length)
                    {
                        nowIndex = 0;
                    }
                }
                yield return 0.1;
            }
        }
    }
    void OneColumn()
    {
        GameObject enemy = Instantiate(waveMonster[nowIndex].monsterPrefab, waveMonster[nowIndex].spanPosition.position, Quaternion.identity);
        for (int i = 0; i < waveMonster[nowIndex].movePosition.Length; i++)
        {
            enemy.GetComponent<Enemy>().Dot[i] = waveMonster[nowIndex].movePosition[i];
        }
        waveEnemy.Add(enemy);
        GameManager.Instance.ChangeDifficulty(enemy);
        nowCount++;
    }
    void TwoColumn()
    {

    }
    void WholeRow()
    {

    }
}
