using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum CharatorType
{
    Player,
    Boss,
    Enemy
}
public class Death : MonoBehaviour
{
    int hp;
    SpriteRenderer sprite;
    public int totalHp;
    public CharatorType type;
    public Sprite[] status;
    public Slider hpBar;
    void Awake()
    {
        if (GameObject.Find("Image"))
        {
            var chObject = GameObject.Find("Image");
            sprite = chObject.GetComponent<SpriteRenderer>();
        }
        hp = totalHp;
    }
    public bool Hurt(string tag, string type)
    {
        if (tag != type)
        {
            hp -= 1;
            if (tag == "Player" && !gameObject.GetComponent<Player>().isInvincible)
            {
                if (hp > 0)
                {
                    sprite.sprite = status[totalHp - hp];
                }
            }
            else if (tag == "Enemy" && hpBar != null)
            {
                hpBar.value = (float)hp / totalHp;
            }
            if (hp <= 0)
            {
                Die(tag);
            }
            return true;
        }
        return false;
    }
    public void Die(string tag)
    {
        if (tag == "Player")
        {
            GameManager.Instance.PlayerIsDied = true;
            GameManager.Instance.PlayerDiePosition = gameObject.transform.position;
            GameManager.Instance.playerLife -= 1;
            GameManager.Instance.LifeText.text = ":" + GameManager.Instance.playerLife.ToString();
            GameManager.Instance.ClearBullet();
            Destroy(this.gameObject);
        }
        else if (tag == "Enemy")
        {
            var temp = gameObject.GetComponent<Enemy>();
            for (int i = 0; i < temp.Allbullet.Count; i++)
            {
                if (temp.Allbullet[i] != null)
                {
                    Destroy(temp.Allbullet[i]);
                }
            }
            Destroy(this.gameObject);
        }
    }
}
