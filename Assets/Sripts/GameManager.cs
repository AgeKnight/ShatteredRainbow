using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum Difficulty
{
    easy,
    middle,
    Hard,
    VerryHard,
    Hell
}
public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance { get => instance; set => instance = value; }
    #region "Private"
    float resurrectionTime = 0;
    int totalExp = 10;
    #endregion
    #region "Public"
    public float AllResurrectionTime;
    [Header("調難度")]
    public Difficulty difficulty;
    public float AllInvincibleTime;
    #endregion
    #region "Hide"  
    [HideInInspector]
    public List<GameObject> playerBullet = new List<GameObject>();
    [HideInInspector]
    public int playerLevel = 1;
    [HideInInspector]
    public bool canTrack = false;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Text scoreText;
    [HideInInspector]
    public Text bottomText;
    [HideInInspector]
    public Text Level;
    [HideInInspector]
    public Slider expBar;
    [HideInInspector]
    public int playerScore;
    [HideInInspector]
    public int playerExp;
    [HideInInspector]
    public int playerBottom;
    [HideInInspector]
    public int playerLife;
    [HideInInspector]
    public Text LifeText;
    [HideInInspector]
    public bool PlayerReallyDeath = false;
    [HideInInspector]
    public bool PlayerIsDied = false;
    [HideInInspector]
    public Transform PlayerResurrectionPosition;
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (PlayerIsDied)
        {
            Resurrection();
        }
    }
    void Resurrection()
    {
        if (playerLife <= 0)
        {
            PlayerReallyDeath = true;
        }
        else
        {
            resurrectionTime += Time.deltaTime;
            if (resurrectionTime >= AllResurrectionTime)
            {
                PlayerResurrection();
            }
        }
    }
    void PlayerResurrection()
    {
        var tempEnemy = FindObjectOfType<Enemy>();
        resurrectionTime = 0;
        PlayerIsDied = false;
        var tempPlayer = Instantiate(player, PlayerResurrectionPosition.position, Quaternion.identity);
        tempPlayer.gameObject.GetComponent<Player>().isInvincible = true;
        if (tempEnemy)
            tempEnemy.Attack();
    }
    public void EatItem(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Life:
                AddLife(1);
                break;
            case ItemType.Bomb:
                AddBottom(1);
                break;
            case ItemType.EXP:
                if(playerLevel<=3)
                    AddExp();
                break;
            case ItemType.Drone:
                AddDrone();
                break;
        }
    }
    public void AddLife(int life)
    {
        playerLife += life;
        var tempPlayer = FindObjectOfType<Player>();
        tempPlayer.gameObject.GetComponent<Death>().hp = 3;
        if (playerLife < 0)
            playerLife = 0;
        LifeText.text = ":" + playerLife.ToString();
    }
    void AddBottom(int count)
    {
        playerBottom += count;
        bottomText.text = ":" + playerBottom.ToString();
    }
    public void AddScore(int value)
    {
        playerScore += value;
        scoreText.text = playerScore.ToString();
    }
    void AddExp()
    {
        playerExp += 1;
        while (playerExp >= totalExp)
        {
            playerLevel += 1;
            if (playerLevel < 3)
            {
                playerExp -= totalExp;
                Level.text = "Levil " + playerLevel.ToString();
            }
            else
            {
                playerLevel = 3;
                Level.text = "Levil Max".ToString();
                break;
            }
        }
        expBar.value = (float)playerExp / totalExp;
    }
    void AddDrone()
    {

    }
    public void ClearBarrage()
    {
        if (PlayerIsDied)
        {
            for (int i = 0; i < playerBullet.Count; i++)
            {
                if (playerBullet[i] != null)
                {
                    Destroy(playerBullet[i]);
                }
            }
            playerBullet.Clear();
        }
        var enemy = FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemy.Length; i++)
        {
            for (int j = 0; j < enemy[i].Allbullet.Count; j++)
            {
                if (enemy[i].Allbullet[j] != null)
                {
                    Destroy(enemy[i].Allbullet[j]);
                }
            }
        }
    }
    public void ChangeDifficulty(GameObject gameObject = null)
    {
        //調難度
        float tempCountTime = 1;
        bool tempCanTrack = false;
        float tempProbability = 1; //0 生命 1 炸彈 2 小弟
        bool[] tempCanAttract = { false, false, false, false }; //0 exp 1 生命 2 炸彈 3 小弟
        switch (difficulty)
        {
            case Difficulty.easy:
                tempCountTime *= 2;
                tempCanTrack = true;
                tempProbability = 2;
                for (int i = 0; i < tempCanAttract.Length; i++)
                {
                    tempCanAttract[i] = true;
                }
                break;
            case Difficulty.middle:
                tempCanAttract[0] = true;
                tempCanAttract[1] = true;
                break;
            case Difficulty.Hard:
                tempCountTime *= 0.5f;
                tempProbability = 0.5f;
                break;
        }
        canTrack = tempCanTrack;
        if (gameObject != null)
        {
            if (gameObject.GetComponent<Death>())
            {
                var death = gameObject.GetComponent<Death>();
                for (int i = 0; i < death.itemStruct.Length; i++)
                {
                    death.itemStruct[i].probability *= tempProbability;
                }
            }
            if (gameObject.GetComponent<Enemy>())
            {
                var enemy = gameObject.GetComponent<Enemy>();
                enemy.countTime *= tempCountTime;
            }
            if (gameObject.GetComponent<Item>())
            {
                var item = gameObject.GetComponent<Item>();
                switch (item.itemType)
                {
                    case ItemType.EXP:
                        item.CanAttract = tempCanAttract[0];
                        break;
                    case ItemType.Life:
                        item.CanAttract = tempCanAttract[1];
                        break;
                    case ItemType.Bomb:
                        item.CanAttract = tempCanAttract[2];
                        break;
                    case ItemType.Drone:
                        item.CanAttract = tempCanAttract[3];
                        break;
                }
            }
        }
    }
}
