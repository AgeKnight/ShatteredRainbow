using System.Collections;
using System.Collections.Generic;
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
    int totalExp = 100;
    #endregion
    #region "Public"
    [Header("復活秒數")]
    public float AllResurrectionTime;
    public EnemyManager enemyManager;
    #endregion
    #region "Hide"  
    [HideInInspector]
    public List<GameObject> playerBullet = new List<GameObject>();
    [HideInInspector]
    public int playerLevel = 0;
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
    public Text LifeText;
    [HideInInspector]
    public Slider expBar;
    [HideInInspector]
    public int playerScore;
    [HideInInspector]
    public int playerExp;
    [HideInInspector]
    public bool PlayerReallyDeath = false;
    [HideInInspector]
    public bool PlayerIsDied = false;
    [HideInInspector]
    public Transform PlayerResurrectionPosition;
    #endregion
    #region "難度"
    [Header("調難度")]
    public Difficulty difficulty;
    public int playerBottom;
    public int playerDrone;
    public int playerLife;
    [Header("無敵時間")]
    public float AllInvincibleTime; //無敵秒數
    #endregion
    void Awake()
    {
        instance = this;
        StartCoroutine(enemyManager.CreateEnemy());
    }
    void Update()
    {
        scoreText.text = playerScore.ToString();
        bottomText.text = "×" + playerBottom.ToString();
        if (playerLife < 0)
            LifeText.text = "×0";
        else
            LifeText.text = "×" + playerLife.ToString();
    }
    public void Resurrection()
    {
        Debug.Log("ss");
        if (playerLife < 0)
            PlayerReallyDeath = true;
        else
            Invoke("PlayerResurrection",AllResurrectionTime);
    }
    void PlayerResurrection()
    {
        PlayerIsDied=false;
        var tempPlayer = Instantiate(player, PlayerResurrectionPosition.position, Quaternion.identity);
        tempPlayer.gameObject.GetComponent<Death>().isInvincible = true;
        Invoke("PlayerNotInvincible",AllInvincibleTime);
    }
    void PlayerNotInvincible()
    {
        var tempPlayer = FindObjectOfType<Player>();
        tempPlayer.gameObject.GetComponent<Death>().isInvincible = false;
    }
    public void EatItem(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Life:
                AddScore(item.score);
                AddLife(1);
                break;
            case ItemType.Bomb:
                AddScore(item.score);
                AddBottom();
                break;
            case ItemType.EXP:
                if (playerLevel < 3)
                {
                    AddExp();
                    AddScore(item.score);
                }
                else
                {
                    AddScore(item.overflowScore);
                }
                break;
            case ItemType.Drone:
                if (playerDrone <= 3)
                {
                    AddDrone();
                    AddScore(item.score);
                }
                else
                {
                    AddScore(item.overflowScore);
                }
                break;
            case ItemType.HalfLife:
                AddHp(item);
                break;
        }
    }
    public void AddScore(int value)
    {
        playerScore += value;
    }
    public void AddLife(int value)
    {
        playerLife += value;
        var tempPlayer = FindObjectOfType<Player>();
        tempPlayer.gameObject.GetComponent<Death>().hp = tempPlayer.gameObject.GetComponent<Death>().totalHp;
    }
    void AddBottom()
    {
        playerBottom += 1;
    }
    void AddHp(Item item)
    {
        var tempPlayer = FindObjectOfType<Player>().gameObject.GetComponent<Death>();
        if (tempPlayer.hp < tempPlayer.totalHp)
        {
            tempPlayer.hp += 1;
            AddScore(item.score);
        }
        else
        {
            AddScore(item.overflowScore);
            AddLife(1);
        }
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
                playerExp = totalExp;
                playerLevel = 3;
                Level.text = "Levil Max".ToString();
                break;
            }
        }
        expBar.value = (float)playerExp / totalExp;
    }
    void AddDrone()
    {
        playerDrone += 1;
        if (playerDrone > 3)
            playerDrone = 3;
    }
    public void ClearBarrage()
    {
        if (PlayerIsDied)
        {
            for (int i = 0; i < playerBullet.Count; i++)
            {
                if (playerBullet[i] != null)
                    playerBullet[i].GetComponent<Bullet>().Die();
            }
            playerBullet.Clear();
        }
        var enemy = FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].ClearBarrage();
        }
    }
    public void ChangeDifficulty(GameObject gameObject = null)
    {
        //調難度
        float tempCountTime = 1;
        bool tempCanTrack = false;
        float tempProbability = 1; //0 生命 1 炸彈 2 小弟 4生命碎片
        bool[] tempCanAttract = { false, false, false, false, false }; //0 exp 1 生命 2 炸彈 3 小弟 4 生命碎片
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
                    case ItemType.HalfLife:
                        item.CanAttract = tempCanAttract[4];
                        break;
                }
            }
        }
    }
}
