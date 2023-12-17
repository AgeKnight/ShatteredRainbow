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
//難度
[System.Serializable]
public struct ItemStruct //0 生命 1 炸彈 2 小弟 3生命碎片
{
    [Range(0f,100f)]public float probability;
    public GameObject items;
}
[System.Serializable]
public struct SpriteStruct
{
    public Sprite[] nowStatus;
}
public class Death : MonoBehaviour
{
    #region Public
    public int totalHp = 2;
    public CharatorType charatorType;
    public EnemyType enemyType;
    public SpriteStruct[] Status;
    public Slider hpBar;
    public GameObject expObject;
    #endregion
    [HideInInspector]
    public bool isInvincible = false;
    [HideInInspector]
    public int hp;
    #region  "調難度"
    [Header("調難度")]
    public ItemStruct[] itemStruct;
    public int minExp;
    public int maxExp;
    public int score;
    #endregion
    void Awake()
    {
        hp = totalHp;
    }
    void Update()
    {
        if (charatorType == CharatorType.Player && hp > 0)
        {
            var sprite = GameObject.Find("Image").GetComponent<SpriteRenderer>();
            sprite.sprite = Status[GameManager.Instance.playerLevel].nowStatus[totalHp - hp];
        }
    }
    public void Hurt(int value = 1)
    {
        if (isInvincible)
            value = 0;           
        hp -= value;
        if (enemyType == EnemyType.Boss)
            hpBar.value = (float)hp / totalHp;
        if (hp <= 0)
            Die();
    }
    public void Die()
    {
        if (gameObject.tag == "Player")
        {
            GameManager.Instance.PlayerIsDied = true;
            GameManager.Instance.AddLife(-1);
            GameManager.Instance.ClearBarrage();
            Destroy(this.gameObject);
        }
        if (gameObject.tag == "Enemy")
        {
            Enemydeath();
            var enemy = gameObject.GetComponent<Enemy>();
            enemy.ClearBarrage();
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
        for (int i = 0; i < itemStruct.Length; i++)
        {
            float tempProbability = Random.Range(1, 100);
            if (tempProbability <= itemStruct[i].probability)
            {
                float tempx = Random.Range(-1.5f, 1.5f);
                float tempY = Random.Range(-1.5f, 1.5f);
                var tempPosition = new Vector2(enemyX + tempx, enemyY + tempY);
                var tempObject=Instantiate(itemStruct[i].items, tempPosition, Quaternion.identity);
                GameManager.Instance.ChangeDifficulty(tempObject);
            }
        }
    }
}
