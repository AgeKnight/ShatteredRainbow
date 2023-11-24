using System.Collections;
using System.Collections.Generic;
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
public class Death : MonoBehaviour
{
    #region Public
    //難度
    [System.Serializable]
    public struct ItemStruct //0 生命 1 炸彈 2 小弟 3經驗值
    {
        //難度
        public float probability;
        public GameObject items;
    }
    public int totalHp = 3;
    public CharatorType charatorType;
    public EnemyType enemyType;
    public Sprite[] status;
    public Slider hpBar;
    public int score;
    public ItemStruct[] itemStruct;
    public int minExp;
    public int maxExp;
    #endregion
    [HideInInspector]
    public int hp;
    void Awake()
    {
        hp = totalHp;
    }
    void Start()
    {
        GameManager.Instance.ChangeDifficulty(this.gameObject);
    }
    public void Hurt()
    {
        if (charatorType==CharatorType.Player&&gameObject.GetComponent<Player>().isInvincible){}
        else
        {
            hp -= 1;
            if (charatorType == CharatorType.Player && hp > 0)
            {
                var sprite = GameObject.Find("Image").GetComponent<SpriteRenderer>();
                sprite.sprite = status[3 - hp];
            }
            if (enemyType == EnemyType.Boss)
                hpBar.value = (float)hp / totalHp;
            if (hp == 0)
                Die();
        }
    }
    public void Die()
    {
        if (gameObject.tag == "Player")
        {
            GameManager.Instance.PlayerIsDied = true;
            GameManager.Instance.AddLife(-1);
            GameManager.Instance.ClearBullet();
            Destroy(this.gameObject);
        }
        if (gameObject.tag == "Enemy")
        {
            var temp = gameObject.GetComponent<Enemy>();
            temp.ClearBarrage();
            Enemydeath();
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
            Instantiate(itemStruct[3].items, tempPosition, Quaternion.identity);
        }
        for (int i = 0; i < itemStruct.Length; i++)
        {
            float tempProbability = Random.Range(1, 100);
            if (tempProbability <= itemStruct[i].probability)
            {
                float tempx = Random.Range(-1.5f, 1.5f);
                float tempY = Random.Range(-1.5f, 1.5f);
                var tempPosition = new Vector2(enemyX + tempx, enemyY + tempY);
                Instantiate(itemStruct[i].items, tempPosition, Quaternion.identity);
            }
        }
    }
}
