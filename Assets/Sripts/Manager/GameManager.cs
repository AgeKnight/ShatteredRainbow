using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    bool isOnButton = false;
    Player playerScript;
    #endregion
    #region "Public"
    [Header("復活秒數")]
    public float AllResurrectionTime;
    #endregion
    #region "Hide" 
    public GameObject Win;
    [HideInInspector]
    public GameObject Menu;
    [HideInInspector]
    public Text Title;
    [HideInInspector]
    public Transform playerSpan;
    [HideInInspector]
    public EnemyManager enemyManager;
    [HideInInspector]
    public GameObject[] stars;
    [HideInInspector]
    public Image BossImage;
    [HideInInspector]
    public GameObject Reciprocal; 
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
    public Transform PlayerResurrectionPosition;
    #endregion
    #region "難度"
    [Header("調難度")]
    public Difficulty difficulty;
    public int playerBottom;
    public int playerLife;
    [Header("無敵時間")]
    public float AllInvincibleTime; //無敵秒數
    #endregion
    void Awake()
    {
        instance = this;
        StartCoroutine(Begin());
    }
    IEnumerator Begin()
    {
        Title.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        Title.gameObject.SetActive(false);
        playerScript = Instantiate(player,playerSpan.transform.position,Quaternion.identity).gameObject.GetComponent<Player>();
        playerScript.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
        while(playerScript.gameObject.transform.position!=PlayerResurrectionPosition.position)
        {
            playerScript.gameObject.transform.position = Vector2.MoveTowards(playerScript.gameObject.transform.position,PlayerResurrectionPosition.position,playerScript.speed*Time.deltaTime);
            yield return 0f;
        }
        playerScript.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
        playerScript.canMove=true;
        yield return new WaitForSeconds(1);
        StartCoroutine(enemyManager.CreateEnemy());
    }
    void Update()
    {
        if(enemyManager.isWin)
        {
            playerScript.enabled = false;
            Win.SetActive(true);
            Time.timeScale=0;
        }
        if (playerLife < 0)
            LifeText.text = "×0";
        else
        {
            MenuUse();
            LifeText.text = "×" + playerLife.ToString();
        }           
    }
    public void Resurrection()
    {
        if (playerLife < 0)
        {
            PlayerReallyDeath = true;
            Menu.SetActive(true);
            Time.timeScale = 0;
        }        
        else
        {
            Invoke("PlayerResurrection",AllResurrectionTime);
        }
    }
    void PlayerResurrection()
    {
        if(!FindObjectOfType<Player>())
        {
            playerScript = Instantiate(player, PlayerResurrectionPosition.position, Quaternion.identity).GetComponent<Player>();
        }
        playerScript.AddBro();
        playerScript.gameObject.GetComponent<Death>().isInvincible = true;
        playerScript.canMove = true;
        Invoke("PlayerNotInvincible",AllInvincibleTime);
    }
    void PlayerNotInvincible()
    {
        playerScript.gameObject.GetComponent<Death>().isInvincible = false;
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
                    playerScript.AddBro();
                    AddScore(item.score);
                }
                else
                {
                    AddScore(item.overflowScore);
                }
                break;
        }
    }
    public void AddScore(int value)
    {
        playerScore += value;
        scoreText.text = playerScore.ToString();
    }
    public void AddLife(int value)
    {
        playerLife += value;
        playerScript.gameObject.GetComponent<Death>().hp = playerScript.gameObject.GetComponent<Death>().totalHp;
    }
    void AddBottom()
    {
        playerBottom += 1;
        bottomText.text = "×" + playerBottom.ToString();
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
    public void ClearBarrage()
    {
        var barrages = FindObjectsOfType<Bullet>();
        for (int i = 0; i < barrages.Length; i++)
            if(barrages[i]!=null)
                barrages[i].Die();
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
                }
            }
        }
    }
    public void BeginReciprocal()
    {
        Reciprocal.SetActive(true);
        Reciprocal.GetComponent<Reciprocal>().allTime = 60;
        Reciprocal.GetComponent<Reciprocal>().isDead = false;
    }
    public void ShowBossImage(Sprite sprite)
    {
        BossImage.gameObject.SetActive(true);
        BossImage.sprite = sprite;
        
        int tempLength = enemyManager.waveBosses[enemyManager.bossIndex].bossPrefab.Length;
        for (int i = 0; i < tempLength-enemyManager.nowBossStage; i++)
        {
            stars[i].SetActive(true);
        }
    }
    public void HideBossImage()
    {
        BossImage.gameObject.SetActive(false);
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
        }
    }
    public void MenuUse()
    {
        if(Input.GetKeyDown(KeyCode.Escape)&&!PlayerReallyDeath&&enemyManager.isWin)
        {
            isOnButton = !isOnButton;
            Menu.SetActive(isOnButton);
            if(isOnButton)
            {
                if(playerScript)
                    playerScript.enabled = false;
                Time.timeScale = 0;
            }
            else
            {
                if(playerScript)
                    playerScript.enabled = true;
                Time.timeScale = 1;
            }
        }
    }
    public void Replay()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }
    public void BackToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main");
    }
    public void Return()
    {
        isOnButton = false;
        Menu.SetActive(false);
        if(playerScript)
        {
            playerScript.enabled = true;
        }           
        if(PlayerReallyDeath)
        {
            playerLife=3;
            PlayerReallyDeath = false;
            PlayerResurrection();
        }
        Time.timeScale = 1;
    }
}
