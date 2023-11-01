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
    public int totalHp = 3;
    public CharatorType charatorType;
    public EnemyType enemyType;
    public Sprite[] status;
    public Slider hpBar;
    [HideInInspector]
    public int hp;
    public int score;
    [System.Serializable]
    public struct ItemStruct //0 生命 1 炸彈 2 小弟
    {
        public int probability;
        public GameObject items;
    }
    public ItemStruct[] itemStruct;
    public GameObject Exps;
    void Awake()
    {
        hp = totalHp;
    }
    public void Hurt()
    {
        hp -= 1;
        if (charatorType == CharatorType.Player && !gameObject.GetComponent<Player>().isInvincible && hp > 0)
        {
            var sprite = GameObject.Find("Image").GetComponent<SpriteRenderer>(); ;
            sprite.sprite = status[3 - hp];
        }
        if (enemyType == EnemyType.Boss)
        {
            hpBar.value = (float)hp / totalHp;
        }
        if (hp <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        if (gameObject.tag == "Player")
        {
            GameManager.Instance.PlayerIsDied = true;
            GameManager.Instance.PlayerDiePosition = gameObject.transform.position;
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
        int probabilityExp = Random.Range(1, 5);
        for (int i = 0; i < probabilityExp; i++)
        {
            Instantiate(Exps, gameObject.transform.position, Quaternion.identity);
        }
        for (int i = 0; i < itemStruct.Length; i++)
        {
            int tempProbability = Random.Range(1, 100);
            if(tempProbability<=itemStruct[i].probability)
            {
                Instantiate(itemStruct[i].items, gameObject.transform.position, Quaternion.identity);
            }
        }
    }
}
