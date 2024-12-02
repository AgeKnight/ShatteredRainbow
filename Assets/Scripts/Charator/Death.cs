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
    public float hp;
    #region Public
    public AudioSource deathAudio;
    public AudioSource Hurtaudio;
    public GameObject deadEffect;
    public float totalHp;
    public CharatorType charatorType;
    public EnemyType enemyType;
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
        isInBomb = true;
        canInBomb = true;
    }
    public void Hurt(float value = 1)
    {
        switch (charatorType)
        {
            case CharatorType.Player:
                if (!GameManager.Instance.ReallyInvincible&&charatorType == CharatorType.Player && !isInvincible && !isDead && !GameManager.Instance.enemyManager.isWin)
                {
                    isDead = true;
                    Die();
                }
                break;
            case CharatorType.Enemy:

                if (!isInvincible)
                {
                    GameManager.Instance.playerScript.AddTimeBarrage(0.05f);
                    GameManager.Instance.AudioPlay(Hurtaudio, true);
                    hp -= value;
                    if (hpBar != null)
                        hpBar.value = hp / totalHp;
                    if (hp <= 0 && !isDead)
                    {
                        this.GetComponent<Animator>().SetTrigger("Dead");
                        this.GetComponent<Enemy>().StopAllCoroutines();
                        isDead = true;

                        //GetComponent<Enemy>().StopAllCoroutines();
                        //Die();

                    }
                }
                break;
        }
    }
    public void Die()
    {


        this.gameObject.SetActive(false);

        if (gameObject.tag == "Player")
        {
            Time.timeScale = 1;
            if (GameManager.Instance.enemyManager.isSpanBoss)
                GameManager.Instance.awardType = AwardType.Common;
            GameManager.Instance.thisMapHurt = true;
            GameManager.Instance.thisMapHurtCount += 1;
            GameManager.Instance.MinusEXP();
            GetComponent<Player>().SetBro(0);
            GameManager.Instance.AddLife(-1);
            GameManager.Instance.ClearBarrage();
            GameManager.Instance.Resurrection();

            //玩家死亡的畫面表現
            if (gameObject.tag == "Player")
            {
                this.gameObject.GetComponent<Player>().canMove = false;
            }
            DeadEffect("Player");

        }
        else
        {
            if (enemyType == EnemyType.Boss)
            {
                GameManager.Instance.BossNext();
               
            }
            Enemydeath();
            //   DeadEffect("Enemy",transform.position);


            if (GameManager.Instance.awardType == AwardType.Bonus)
                GameManager.Instance.AddScore(bonusScore);

        }
        Destroy(this.gameObject, 1f);
    }

    public void DeadEffect(string ExplosionType) //物件死亡的畫面表現
    {
        Transform Spot = this.transform;

        if (gameObject.tag != "Player")
        {
            Spot = this.GetComponent<Enemy>().bulletTransform;
        }

        GameManager.Instance.AudioPlay(deathAudio, true);

        GameObject effect = Instantiate(deadEffect, Spot.position, Quaternion.identity);
        effect.GetComponent<Animator>().SetTrigger(ExplosionType);
   //     this.GetComponent<Animator>().SetTrigger("Dead");
       
        Destroy(effect, 0.5f);
        
    }
    void Enemydeath()
    {
        GameManager.Instance.AddScore(score);
        int probabilityExp = Random.Range(minExp, maxExp);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < (probabilityExp / Mathf.Pow(10, i)) % 10; j++)
            {
                Instantiate(GameManager.Instance.expObject[i], transform.position, Quaternion.identity);
            }

        }
        if (GameManager.Instance.awardType != AwardType.Failed)
        {
            //0 生命 1 炸彈 2 滿等
            for (int i = 0; i < itemStruct.Length; i++)
            {
                if (i == 0 || GameManager.Instance.awardType == AwardType.Bonus) //Boss戰表現判定 獎勵依照遊戲設定
                {
                    float tempProbability = Random.Range(1, 100);
                    if (tempProbability <= itemStruct[i].probability)
                    {
                        Instantiate(itemStruct[i].items,transform.position , Quaternion.identity);
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
