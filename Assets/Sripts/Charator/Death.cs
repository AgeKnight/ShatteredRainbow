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
    public GameObject deadEffect;
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
            Time.timeScale = 1;
            
            if (GameManager.Instance.enemyManager.isSpanBoss)
                GameManager.Instance.awardType = AwardType.Common;
            GameManager.Instance.thisMapHurt = true;
            GameManager.Instance.MinusEXP();
            GetComponent<Player>().AddBro(-6);
            GameManager.Instance.AddLife(-1);
            GameManager.Instance.ClearBarrage();
            GameManager.Instance.Resurrection();
        
            //玩家死亡的畫面表現
            DeadEffect();


         
            Destroy(this.gameObject, 1);

        }
        else 
        {
            if (enemyType == EnemyType.Boss)
            {
                GameManager.Instance.BossNext();
                GameManager.Instance.ClearBarrage();//Boss死亡後的畫面清理
            }
            Enemydeath();
        
            DeadEffect();
           

            if (GameManager.Instance.awardType==AwardType.Bonus)
                GameManager.Instance.AddScore(bonusScore);

            
           
        }
    }

    void DeadEffect() //物件死亡的畫面表現
    {
        
        GameObject effect = Instantiate(deadEffect);
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().SetTrigger(gameObject.tag + "Dies");
        this.GetComponent<Animator>().SetTrigger("Dead");
        if(gameObject.tag == "Player")
        {
            this.gameObject.GetComponent<Player>().canMove = false;
        }
        this.gameObject.SetActive(false);
        Destroy(effect, 0.8f);
        Destroy(this.gameObject, 1);
    }


    void Enemydeath()
    {
        
    

        GameManager.Instance.AddScore(score);
        int probabilityExp = Random.Range(minExp, maxExp);
        float enemyX = gameObject.transform.position.x;
        float enemyY = gameObject.transform.position.y;
        for (int i = 0; i <= probabilityExp; i++)
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
                // if(i==1&&GameManager.Instance.awardType==AwardType.Common)
                // {
                //     GameManager.Instance.awardType=AwardType.Bonus;
                //     break;
                // }

                if (i == 0 || GameManager.Instance.awardType == AwardType.Bonus) //Boss戰表現判定 獎勵依照遊戲設定
                {
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
        }
        //   GameManager.Instance.awardType=AwardType.Bonus;
      
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
