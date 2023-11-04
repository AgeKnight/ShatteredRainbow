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
}
public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance { get => instance; set => instance = value; }
    #region "Private"
    float resurrectionTime = 0;
    int playerLevel = 1;
    int totalExp = 10;
    #endregion
    #region "Public"
    public float AllResurrectionTime;
    [Header("調難度")]
    public Difficulty difficulty;
    #endregion
    #region "Hide"  
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
    public Vector3 PlayerDiePosition = new Vector3(0, 0, 0);
    [HideInInspector]
    public List<GameObject> Playerbullet = new List<GameObject>();
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
        Instantiate(player, PlayerDiePosition, Quaternion.identity);
        tempEnemy.Attack();
    }
    public void ClearBullet()
    {
        var tempEnemy = FindObjectOfType<Enemy>();
        tempEnemy.ClearBarrage();
        for (int i = 0; i < Playerbullet.Count; i++)
        {
            if (Playerbullet[i] != null)
            {
                Destroy(Playerbullet[i]);
            }
        }
        Playerbullet.Clear();
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
                AddExp(item.Exp);
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
        tempPlayer.gameObject.transform.Find("Image").gameObject.GetComponent<SpriteRenderer>().sprite = player.gameObject.GetComponent<Death>().status[0];
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
    void AddExp(int value)
    {
        playerExp += value;
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
                Level.text = "Levil Max".ToString();
                break;
            }
        }
        expBar.value = (float)playerExp / totalExp;
    }
    void AddDrone()
    {

    }
    public void ChangeDifficulty(GameObject gameObject=null)
    {
        //調難度
        float tempCountTime = 1;
        bool tempCanTrack = false;
        float tempProbability = 1; //0 生命 1 炸彈 2 小弟
        bool[] tempCanAttract = { true, true, true, true }; //0 exp 1 生命 2 炸彈 3 小弟
        switch (difficulty)
        {
            case Difficulty.easy:
                tempCountTime *= 2;
                tempCanTrack = true;
                tempProbability = 2;
                break;
            case Difficulty.middle:
                tempCanAttract[2] = false;
                tempCanAttract[3] = false;
                break;
            case Difficulty.Hard:
                tempCountTime *= 0.5f;
                tempProbability = 0.5f;
                tempCanAttract[0] = false;
                tempCanAttract[1] = false;
                tempCanAttract[2] = false;
                tempCanAttract[3] = false;
                break;
        }
        canTrack = tempCanTrack;
        if (gameObject != null)
        {
            if (gameObject.GetComponent<Death>())
            {
                var death = gameObject.GetComponent<Death>();
                death.countTime *= tempCountTime;
                for (int i = 0; i < death.itemStruct.Length; i++)
                {
                    death.itemStruct[i].probability *= tempProbability;
                }
            }
            else if (gameObject.GetComponent<Item>())
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
                        item.CanAttract = tempCanAttract[1];
                        break;
                    case ItemType.Drone:
                        item.CanAttract = tempCanAttract[1];
                        break;
                }
            }
        }
    }
}
