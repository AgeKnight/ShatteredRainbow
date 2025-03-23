using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public enum CharatorType
{
    Player,
    Enemy,
    None
}
public enum EnemyType
{
    Trash,
    Boss
}
public class Death : MonoBehaviour
{
    bool isDead = false;
    #region Public
    public AudioSource deathAudio;
    public AudioSource Hurtaudio;
    public GameObject deadEffect;
    public float totalHp;
    public CharatorType charatorType;
    public EnemyType enemyType;
    public bool Bonus;
    [Range(0f, 100f)] public float[] probability;//0 生命 1 炸彈 2 小弟 
    #endregion
    #region "Hide"
    [HideInInspector]
    public float tempHurt = 0;
    [HideInInspector]
    public float hp;
    //[HideInInspector]
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
    public int minExp;
    public int maxExp;
    public int score;
    public int bonusScore;
    #endregion
    void Awake()
    {
        if (!GameManager.Instance.isRush && GameManager.Instance.GameStage != 1 && charatorType == CharatorType.None)
        {
            hp = GameManager.Instance.hp;
            hpBar.value = hp / totalHp;
        }
        else
        {
            hp = totalHp;
        }
        isInBomb = true;
        canInBomb = true;
    }
    public void Hurt(float value = 1)
    {
        switch (charatorType)
        {
            case CharatorType.Player:
                if (!GameManager.Instance.ReallyInvincible && charatorType == CharatorType.Player && !isInvincible && !isDead && !GameManager.Instance.enemyManager.isWin)
                {
                    isDead = true;
                    GameManager.Instance.AllHurt = true;
                    Die();
                }
                break;
            case CharatorType.Enemy:
                if (!isInvincible)
                {
                    GameManager.Instance.playerScript.AddTimeBarrage(0.05f);
                    GameManager.Instance.AudioPlay(Hurtaudio, true);
                    hp -= value;
                    if (enemyType == EnemyType.Boss)
                    {
                        hpBar.value = hp / totalHp;
                    }
                    if (hp <= 0 && !isDead)
                    {
                        this.GetComponent<Animator>().SetTrigger("Dead");
                        this.GetComponent<Enemy>().StopAllCoroutines();
                        isDead = true;
                    }
                }
                break;
            case CharatorType.None:
                if (!isDead && !GameManager.Instance.enemyManager.isWin)
                {
                    if (GameManager.Instance.playerScript.isUseBomb)
                    {
                        float tempValue = value * (GameManager.Instance.playerScript.eatDrone + 1);
                        hp += tempValue;
                        if (hp >= totalHp)
                        {
                            hp = totalHp;
                            GameManager.Instance.AddExp((int)tempValue);
                        }
                    }
                    if (!GameManager.Instance.playerScript.isUseBomb && !GameManager.Instance.ReallyInvincible && !isInvincible)
                    {
                        GameManager.Instance.AllHurt = true;
                        hp -= value;
                    }
                    hpBar.value = hp / totalHp;
                    if (GameManager.Instance.playerScript.isAttack)
                    {
                        tempHurt += value;
                    }
                    if (hp <= 0)
                    {
                        isDead = true;
                        Die();
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
            GameManager.Instance.AllDeath += 1;
            GameManager.Instance.FinishAchievement(23);
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
            if (!Bonus)
            {
                GameManager.Instance.killEnemy += 1;
            }
            if (isInBomb && GameManager.Instance.playerScript)
            {
                GameManager.Instance.playerScript.GetComponent<Death>().ExitBomb();
            }
            GameManager.Instance.AllKill += 1;
            GameManager.Instance.Save();
            if (enemyType == EnemyType.Boss)
                GameManager.Instance.BossNext();
            Enemydeath();
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
            for (int i = 0; i < GameManager.Instance.items.Length; i++)
            {
                if (i == 0 || GameManager.Instance.awardType == AwardType.Bonus) //Boss戰表現判定 獎勵依照遊戲設定
                {
                    float tempProbability = Random.Range(1, 100);
                    if (tempProbability <= probability[i])
                    {
                        Instantiate(GameManager.Instance.items[i], transform.position, Quaternion.identity);
                    }
                }

            }
        }

    }
    public IEnumerator BeBombDamage(float hurt, float time)
    {
        canInBomb = false;
        isInBomb = true;
        while (isInBomb)
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
