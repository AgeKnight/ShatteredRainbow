using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance { get => instance; set => instance = value; }
    #region "Private"
    float resurrectionTime = 0;
    int playerLevel=1;
    int totalExp = 10;
    #endregion
    #region "Public"
    public float AllResurrectionTime;
    #endregion
    #region "Hide"  
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
    [HideInInspector]
    public Item item;
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        item = GetComponent<Item>();
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
    public void EatItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Life:
                AddLife(1);
                break;
            case ItemType.Bomb:
                AddBottom(1);
                break;
            case ItemType.EXP:
                AddExp(1);
                break;    
        }
    } 
    public void AddLife(int life)
    {
        playerLife+=life;
        var tempPlayer = FindObjectOfType<Player>();
        tempPlayer.gameObject.transform.Find("Image").gameObject.GetComponent<SpriteRenderer>().sprite=player.gameObject.GetComponent<Death>().status[0];
        tempPlayer.gameObject.GetComponent<Death>().hp=3;
        if(playerLife<0)
            playerLife=0;
        LifeText.text = ":"+playerLife.ToString();
    }
    void AddBottom(int count)
    {
        playerBottom+=count;
        bottomText.text = ":"+playerBottom.ToString();
    }
    public void AddScore(int value)
    {
        playerScore+=value;
        scoreText.text = playerScore.ToString();
    }
    void AddExp(int value)
    {
        playerExp+=value;
        while(playerExp>=totalExp)
        {
            playerExp-=totalExp;
            playerLevel+=1;
            Level.text = "Levil "+playerLevel.ToString();
        }
        expBar.value = (float)playerExp/totalExp;
    }
}
