using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public enum CharatorType
{
    Player,
    Enemy
}
public enum EnemyType
{
    Trash,
    Boss
}
//難度
[System.Serializable]
public struct ItemStruct //0 生命 1 炸彈 2 小弟 3生命碎片
{
    [Range(0f, 100f)] public float probability;
    public GameObject items;
}
public class Death : MonoBehaviour
{
    bool isDead = false;
    float hp;
    #region Public
    public AudioSource deathAudio;
    public float totalHp;
    public CharatorType charatorType;
    public EnemyType enemyType;
    public GameObject expObject;
    #endregion
    #region "Hide"
    [HideInInspector]
    public Slider hpBar;
    [HideInInspector]
    public bool isInvincible = false;
    [HideInInspector]
    public bool isInBomb;
    [HideInInspector]
    public bool canInBomb;
    #endregion
    #region  "調難度"
    [Header("調難度")]
    public EnemyBarrageCount ultimateAttack;
    public int indexMax = 1;
    public ItemStruct[] itemStruct;
    public int minExp;
    public int maxExp;
    public int score;
    public int bonusScore;
    #endregion
    void Awake()
    {
        hp = totalHp;
        isInBomb=true;
        canInBomb=true;
    }
    public void Hurt(float value = 1)
    {
        switch (charatorType)
        {
            case CharatorType.Player:
                if (charatorType == CharatorType.Player && !isInvincible && !isDead && !GameManager.Instance.enemyManager.isWin)
                    Die();
                break;
            case CharatorType.Enemy:
                GameManager.Instance.playerScript.AddTimeBarrage(0.05f);
                if (isInvincible)
                    value = 0;
                hp -= value;
                if (hpBar != null)
                    hpBar.value = hp / totalHp;
                if (hp <= 0 && !isDead)
                {
                    Die();
                }                  
                break;
        }
    }
    public void Die()
    {
        isDead = true;
        GameManager.Instance.AudioPlay(deathAudio,true);
        if (gameObject.tag == "Player")
        {
            GameManager.Instance.thisMapHurt = true;
            if(GameManager.Instance.enemyManager.isSpanBoss)
                GameManager.Instance.awardType = AwardType.Common;
            GameManager.Instance.AddLife(-1);
            GameManager.Instance.ClearBarrage();
            GameManager.Instance.Resurrection();
            Destroy(this.gameObject);
        }
        else 
        {
            if (enemyType == EnemyType.Boss)
                GameManager.Instance.BossNext();
            Enemydeath();
            if(GameManager.Instance.awardType==AwardType.Bonus)
                GameManager.Instance.AddScore(bonusScore);
            Destroy(this.gameObject);
        }
    }
    void Enemydeath()
    {
        GameManager.Instance.AddScore(score);
        int probabilityExp = Random.Range(minExp, maxExp);
        float enemyX = gameObject.transform.position.x;
        float enemyY = gameObject.transform.position.y;
        for (int i = 0; i < probabilityExp; i++)
        {
            float tempx = Random.Range(-1.5f, 1.5f);
            float tempY = Random.Range(-1.5f, 1.5f);
            var tempPosition = new Vector2(enemyX + tempx, enemyY + tempY);
            var tempObject = Instantiate(expObject, tempPosition, Quaternion.identity);
            GameManager.Instance.ChangeDifficulty(tempObject);
        }
        if (GameManager.Instance.awardType != AwardType.Failed)
        {
            //0 生命 1 炸彈 2 滿等
            for (int i = 0; i < itemStruct.Length; i++)
            {
                if(i==1&&GameManager.Instance.awardType==AwardType.Common)
                {
                    GameManager.Instance.awardType=AwardType.Bonus;
                    break;
                }
                float tempProbability = Random.Range(1, 100);
                if (tempProbability <= itemStruct[i].probability)
                {
                    float tempx = Random.Range(-1.5f, 1.5f);
                    float tempY = Random.Range(-1.5f, 1.5f);
                    var tempPosition = new Vector2(enemyX + tempx, enemyY + tempY);
                    var tempObject = Instantiate(itemStruct[i].items, tempPosition, Quaternion.identity);
                    GameManager.Instance.ChangeDifficulty(tempObject);
                }
            }
        }
        GameManager.Instance.awardType=AwardType.Bonus;
    }
    public IEnumerator BeBombDamage(float hurt,float time)
    {
        canInBomb = false;
        isInBomb = true;
        while(isInBomb)
        {
            Hurt(hurt);
            yield return new WaitForSeconds(time);
        }       
    }
    public void ExitBomb()
    {
        canInBomb = true;
        isInBomb = false;
    }
}
