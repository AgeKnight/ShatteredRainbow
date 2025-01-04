using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum Difficulty
{
    easy,
    middle,
    Hard,
    VerryHard,
    Hell
}
public enum StatusType
{
    Pause,
    Lose,
    Win
}
public enum AwardType
{
    Bonus,
    Common,
    Failed,
}
[System.Serializable]
public class SaveData
{
    public int playerBomb;
    public int playerLife;
    public int playerLevel;
    public int playerExp;
    public int droneCount;
    public float playerScore;
    public float HiPlayerScore;
    public int GameStage = 1;
    public bool Invincible;
    public bool autoShoot;
    public bool[] Achievements = new bool[30];
    public bool AllHurt = false;
    public bool AllBomb = false;
    public bool AllTimeBarrage = false;
    public bool AllAttack = false;
    public int enemyCount;
    public int killEnemy;
}
public class GameManager : MonoBehaviour
{

    static GameManager instance;
    public static GameManager Instance { get => instance; set => instance = value; }
    #region "Private"
    bool isCheat = false;
    int totalExp = 1000;
    bool isOnButton = false;
    bool isRefreshed = false;
    float sideA;
    float sideB;
    bool isOperate = false;
    #endregion
    #region "Public"
    public Coroutine coroutine;
    [Header("復活秒數")]
    public float AllResurrectionTime;
    #endregion
    #region "Hide"
    public int killEnemy;
    [HideInInspector]
    public int enemyCount;
    [HideInInspector]
    public bool AllHurt = false;
    [HideInInspector]
    public bool AllBomb = false;
    [HideInInspector]
    public bool AllTimeBarrage = false;
    [HideInInspector]
    public bool AllAttack = false;
    [HideInInspector]
    public bool[] Achievements = new bool[30];
    [HideInInspector]
    public bool ReallyInvincible;
  //  [HideInInspector]
    public GameObject[] expObject;
    //上:0,1 下:2,3 左:4,5 右6,7 攻擊:8 大招:9 子彈時間:10 菜單:11
    [HideInInspector]
    public KeyCode[] curinput = new KeyCode[30];
    [HideInInspector]
    public bool canControlAttack = true;
    [HideInInspector]
    public float HiScore;
    [HideInInspector]
    public int GameStage = 1;
    [HideInInspector]
    public int playerExp;
   // [HideInInspector]
    public AudioSource[] BackMusic;
    [HideInInspector]
    public AudioSource[] MenuSound;
    [HideInInspector]
    public AudioSource[] ItemgainsSound;
    [HideInInspector]
    public AudioSource LevelupSound;
    [HideInInspector]
    public float playerScore;
    [HideInInspector]
    public GameObject[] LightSide;
    [HideInInspector]
    public Text[] MapBonusScores;//0 本關分數 1 加成分數 2 炸彈bonus名 3 炸彈bonus 4 生命bonus名 5 生命bonus
    public GameObject[] BonusShine;
    [HideInInspector]
    public float thisMapScore = 0;
    [HideInInspector]
    public bool thisMapBomb = false;
    [HideInInspector]
    public bool thisMapHurt = false;
    [HideInInspector]
    public int thisMapBombCount = 0; //本關炸彈使用數
    [HideInInspector]
    public int thisMapHurtCount = 0; //本關死亡數
    [HideInInspector]
    public AwardType awardType = AwardType.Bonus;
    [HideInInspector]
    public int lifeCount = 0;
    [HideInInspector]
    public int bombCount = 0;
    [HideInInspector]
    public int droneCount = 0;
    [HideInInspector]
    public Sprite[] LifeImages;//0 空心 1 實心
    [HideInInspector]
    public Sprite[] bombImages;//0 空心 1 實心
    [HideInInspector]
    public Sprite[] bossImages;//0 空心 1 實心 
    [HideInInspector]
    public GameObject[] Triangles;
    [HideInInspector]
    public Animator UIanimator;
    [HideInInspector]
    public Player playerScript;
    [HideInInspector]
    public GameObject[] bossStaire;
    //0 左上 1 右下
    [HideInInspector]
    public GameObject[] mapPosition;
    [HideInInspector]
    public int allBomb;
    [HideInInspector]
    public int allLife;
    [HideInInspector]
    public GameObject StageClear;
    [HideInInspector]
    public Slider BossBar;
    [HideInInspector]
    public StatusType statusType = StatusType.Pause;
    [HideInInspector]
    public Sprite[] playerFace;
    [HideInInspector]
    public GameObject[] Lifes;
    [HideInInspector]
    public GameObject[] Bombs;
    [HideInInspector]
    public GameObject[] Menus;//0 暫停 1 輸 2贏
    [HideInInspector]
    public Transform playerSpan;
    [HideInInspector]
    public EnemyManager enemyManager;
    [HideInInspector]
    public GameObject Reciprocal;
    [HideInInspector]
    public int playerLevel;
    [HideInInspector]
    public int ChoicePlayer;
    [HideInInspector]
    public GameObject[] player;
    [HideInInspector]
    public GameObject EXP;
    [HideInInspector]
    public Text Hi_scoreText;
    [HideInInspector]
    public Text scoreText;
    [HideInInspector]
    public Text Level;
    [HideInInspector]
    public Slider expBar;
    [HideInInspector]
    public Transform PlayerResurrectionPosition;
    [HideInInspector]
    public bool isRush;
    #endregion
    #region "難度"
    [Header("調難度")]
    public Difficulty difficulty;
    public int default_playerBomb = 2;
    public int default_playerLife = 2;
    public int default_droneCount = 0;
    public int default_playerLevel = 0;
    [Header("無敵時間")]
    public float AllInvincibleTime; //無敵秒數
    #endregion
    void Awake()
    {
        instance = this;
        Load();
        coroutine = StartCoroutine(Begin());
    }
    public IEnumerator Begin()
    {

        playerScript = Instantiate(player[ChoicePlayer], playerSpan.transform.position, Quaternion.identity).gameObject.GetComponent<Player>();
        playerScript.gameObject.GetComponent<CircleCollider2D>().isTrigger = true;

        enemyManager.canGoNext = false;
        yield return new WaitForSeconds(2);

        while (playerScript.gameObject.transform.position != PlayerResurrectionPosition.position)
        {
            playerScript.gameObject.transform.position = Vector2.MoveTowards(playerScript.gameObject.transform.position, PlayerResurrectionPosition.position, playerScript.speed * Time.deltaTime);
            yield return 0f;
        }

        playerScript.gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
        playerScript.canMove = true;

        GetComponent<AudioSource>().PlayOneShot(BackMusic[0].clip);

        yield return new WaitForSeconds(4);

        StartCoroutine(enemyManager.CreateEnemy());
    }
    void Update()
    {
        SideAdjustment();
        switch (statusType)
        {
            case StatusType.Pause:
                MenuUse();
                break;
            case StatusType.Win:
                WinGame();
                break;
            case StatusType.Lose:
                {
                    statusType = StatusType.Pause;
                    Invoke("LoseGame", 1);
                    break;
                }
        }
    }
    #region "復活"
    public void Resurrection()
    {
        if (lifeCount < 0)
        {
            statusType = StatusType.Lose;
        }
        else
        {
            Invoke("PlayerResurrection", AllResurrectionTime);
        }
    }

    void PlayerResurrection()
    {
        playerScript = Instantiate(player[ChoicePlayer], PlayerResurrectionPosition.position, Quaternion.identity).GetComponent<Player>();
        playerScript.gameObject.GetComponent<Death>().isInvincible = true;
        playerScript.canMove = true;
        Invoke("PlayerNotInvincible", AllInvincibleTime);
    }
    public void FinishAchievement(int index)
    {
        if (!Achievements[index]  && !isCheat && !isRush)
        {
            Achievements[index] = true;
            Save();
        }
    }

    void PlayerNotInvincible()
    {
        playerScript.gameObject.GetComponent<Death>().isInvincible = false;
    }
    #endregion
    #region "吃東西"
    public void EatItem(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Life:
                if (lifeCount < allLife)
                {
                    AddScore(item.score);
                    AddLife(1);
                }
                else
                {
                    AddScore(item.overflowScore);
                }
                break;
            case ItemType.Bomb:
                if (bombCount < allBomb)
                {
                    AddScore(item.score);
                    AddBomb(1);
                }
                else
                {
                    AddScore(item.overflowScore);
                }
                break;
            case ItemType.Drone:
                if (droneCount < 6)
                {
                    playerScript.AddBro(2);
                    AddScore(item.score);
                }
                else
                {
                    AddScore(item.overflowScore);
                }
                break;
            case ItemType.EXP_Small:

                AudioPlay(ItemgainsSound[0], true);
                if (playerLevel < 3)
                {
                    AddExp(1);
                    AddScore(item.score);
                }
                else
                {
                    AddScore(item.overflowScore);
                }
                break;
            case ItemType.EXP_Mid:


                AudioPlay(ItemgainsSound[0], true);
                if (playerLevel < 3)
                {
                    AddExp(10);
                    AddScore(item.score);
                }
                else
                {
                    AddScore(item.overflowScore);
                }
                break;
            case ItemType.EXP_Big:

                AudioPlay(ItemgainsSound[0], true);
                if (playerLevel < 3)
                {
                    AddExp(100);
                    AddScore(item.score);
                }
                else
                {
                    AddScore(item.overflowScore);
                }
                break;
        }
    }
    public void AddScore(float value)
    {
        thisMapScore += value;

        playerScore += value;
        playerScore = Mathf.RoundToInt(playerScore);
        scoreText.text = playerScore.ToString();
        if (playerScore >= HiScore)
        {
            HiScore = playerScore;
            Hi_scoreText.text = HiScore.ToString();
        }
    }
    public void SetScore(float value)
    {
        playerScore = value;
        scoreText.text = playerScore.ToString();
    }
    public void SetHiScore(float value)
    {
        HiScore = value;
        Hi_scoreText.text = HiScore.ToString();
    }
    public void SetLife(int value)
    {
        lifeCount = value;
        for (int i = 0; i < allLife; i++)
        {
            Lifes[i].gameObject.GetComponent<Image>().sprite = LifeImages[0];
        }
        for (int i = 0; i < lifeCount; i++)
        {
            Lifes[i].gameObject.GetComponent<Image>().sprite = LifeImages[1];
        }
    }
    public void AddLife(int value)
    {
        lifeCount += value;
        if (lifeCount >= 10)
            FinishAchievement(14);
        CheckUpperLimit();
        for (int i = 0; i < allLife; i++)
        {
            Lifes[i].gameObject.GetComponent<Image>().sprite = LifeImages[0];
        }
        for (int i = 0; i < lifeCount; i++)
        {
            Lifes[i].gameObject.GetComponent<Image>().sprite = LifeImages[1];
        }
    }
    public void SetBomb(int value)
    {
        bombCount = value;
        for (int i = 0; i < allBomb; i++)
        {
            Bombs[i].gameObject.GetComponent<Image>().sprite = bombImages[0];
        }
        for (int i = 0; i < bombCount; i++)
        {
            Bombs[i].gameObject.GetComponent<Image>().sprite = bombImages[1];
        }
    }
    public void AddBomb(int value)
    {
        bombCount += value;
        if (bombCount >= 10)
            FinishAchievement(15);
        CheckUpperLimit();
        if (bombCount <= 0)
        {
            bombCount = 0;
        }
        for (int i = 0; i < allBomb; i++)
        {
            Bombs[i].gameObject.GetComponent<Image>().sprite = bombImages[0];
        }
        for (int i = 0; i < bombCount; i++)
        {
            Bombs[i].gameObject.GetComponent<Image>().sprite = bombImages[1];
        }
    }
    void AddExp(int value)
    {
        playerExp += value;
        UIanimator.SetTrigger("EXPGain");
        while (playerExp >= totalExp)
        {
            playerScript.gameObject.GetComponent<Animator>().SetTrigger("Levelup");
            UIanimator.SetTrigger("Levelup");
            AudioPlay(LevelupSound, true);
            playerLevel += 1;
            if (playerScript)
            {
                if (playerScript.playerType == PlayerType.vyles)
                {
                    playerScript.AllVylesIndex = playerLevel + 3;
                    playerScript.VyleCreate();
                }
            }
            if (playerLevel < 3)
            {
                playerExp -= totalExp;
                Level.text = "Level " + playerLevel.ToString();
            }
            else
            {
                FinishAchievement(12);
                CheckUpperLimit();
                playerExp = totalExp;
                playerLevel = 3;
                Level.text = "Level Max".ToString();
                break;
            }
        }

        expBar.value = (float)playerExp / totalExp;
    }
    void SetExp(int value)
    {
        playerExp = value;
        if (playerLevel < 3)
        {
            Level.text = "Level " + playerLevel.ToString();
        }
        else
        {
            playerExp = totalExp;
            playerLevel = 3;
            Level.text = "Level Max".ToString();
        }
        expBar.value = (float)playerExp / totalExp;
    }
    #endregion
    public void MinusEXP() //每次死亡扣除經驗值總數的1/4
    {
        int exploss; //經驗值所失
        GameObject drop;
        if (playerLevel == 3)
        {
            exploss = (playerLevel * 1000) / 4;  //經驗值所失
            playerExp = (playerLevel * 1000) * 3 / 4;//懲罰後經驗值
        }
        else
        {
            exploss = (playerLevel * 1000 + playerExp) / 4;  //經驗值所失
            playerExp = (playerLevel * 1000 + playerExp) * 3 / 4; //懲罰後經驗值
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < (exploss / Mathf.Pow(10, i)) % 10; j++)
            {
                drop = Instantiate(expObject[i], playerScript.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                drop.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f)));
                drop.GetComponent<Item>().CanAttract = false;
            }

        }


        playerLevel = playerExp / 1000;    //處罰後等級
        playerExp = playerExp - playerLevel * 1000;    //處罰後的顯示經驗值    

        // playerStatus.gameObject.GetComponent<Image>().sprite = playerFace[playerLevel];
        expBar.value = (float)playerExp / totalExp;
        Level.text = "Level " + playerLevel.ToString();

    }
    public void ClearBarrage()
    {
        var barrages = FindObjectsOfType<Bullet>();
        for (int i = 0; i < barrages.Length; i++)
            if (barrages[i] != null&&barrages[i].GetComponent<Bullet>().Unerasable==false)
                barrages[i].Die();
    }
    public void ChangeDifficulty(GameObject gameObject = null)
    {
        //調難度
        float tempCountTime = 1;
        float tempProbability = 1; //0 生命 1 炸彈 2 小弟 4生命碎片
        bool[] tempCanAttract = { false, false, false, false, false }; //0 exp 1 生命 2 炸彈 3 小弟 4 生命碎片
        switch (difficulty)
        {
            case Difficulty.easy:
                tempCountTime *= 2;
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
                    case ItemType.EXP_Small:
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
    #region "boss戰"
    public void BeginReciprocal()
    {
        Reciprocal.SetActive(true);

        UIanimator.SetInteger("BossState", 0);//大招結束，進入可傷害狀態，血條再開

        Reciprocal.GetComponent<Reciprocal>().allTime = 60;
        Reciprocal.GetComponent<Reciprocal>().isDead = false;
    }
    public void ShowBossStaire(int count, int nowStage)
    {
        for (int i = 0; i < count - 1; i++)
        {
            bossStaire[i].SetActive(true);
            if (i < count - nowStage)
                bossStaire[i].GetComponent<Image>().sprite = bossImages[1];
        }
    }
    //階段顯示
    public void BossNext()
    {
        BossBar.value = 1;
        Reciprocal.GetComponent<Reciprocal>().gameObject.SetActive(false);
        GameManager.Instance.ClearBarrage();//Boss死亡後的畫面清理
        //播放血條動畫<關>
        //  BarUse.Play("Close");
        UIanimator.SetInteger("BossState", 1);
        if (!enemyManager.OtherStage)
        {
            UIanimator.SetInteger("BossState", 2);
        }
        for (int i = 0; i < bossStaire.Length; i++)
        {
            bossStaire[i].GetComponent<Image>().sprite = bossImages[0];
        }

        enemyManager.nowEveryStairTime = enemyManager.everyStairTime;
    }
    #endregion



    public void MenuUse()
    {
        if (playerScript.canMove == true && Input.GetKeyDown(curinput[14]) || Input.GetKeyDown(curinput[15]))
        {
            isOnButton = !isOnButton;
            Menus[0].SetActive(isOnButton);
            if (playerScript)
                playerScript.enabled = !isOnButton;
            if (isOnButton)
            {
                AudioPlay(MenuSound[0], true);
                Time.timeScale = 0;
            }
            else
            {
                AudioPlay(MenuSound[1], true);
                Time.timeScale = 1;
            }
        }
    }

    public void Resume()
    {
        isOnButton = !isOnButton;
        Menus[0].SetActive(isOnButton);
        if (playerScript)
            playerScript.enabled = !isOnButton;
        if (isOnButton)
        {
            AudioPlay(MenuSound[0], true);
            Time.timeScale = 0;
        }
        else
        {
            AudioPlay(MenuSound[1], true);
            Time.timeScale = 1;
        }
    }


    public void Replay()
    {
        Time.timeScale = 1;
        DontDestroyOnLoad(AudioPlay(MenuSound[3], true));
        statusType = StatusType.Pause;
        StartCoroutine(Loadscene(SceneManager.GetActiveScene().buildIndex));
    }
    public void BackToMenu()
    {
        DontDestroyOnLoad(AudioPlay(MenuSound[3], true));
        statusType = StatusType.Pause;
        isRush = false;
        RefreshGame();
        StartCoroutine(Loadscene(0));
    }
    public void Continue()
    {
        Menus[1].SetActive(false);
        statusType = StatusType.Pause;
        SetLife(2);
        PlayerResurrection();
        AddScore(1);
        Time.timeScale = 1;
    }
    void WinGame()
    {
        playerScript.enabled = false;
        Time.timeScale = 0;
    }
    void LoseGame()
    {
        Menus[1].SetActive(true);
        Time.timeScale = 0;
    }
    void RefreshGame()
    {
        SetBomb(default_playerBomb);
        SetLife(default_playerLife);
        droneCount = default_droneCount;
        GameStage = 1;
        playerLevel = 0;
        SetScore(0);
        SetExp(0);
        Save();
    }
    void SideAdjustment()
    {
        if (!enemyManager.canGoNext)
        {
            if (enemyManager.isSpanBoss)//邊框效果修飾
            {

                if (sideB != 1 && enemyManager.bossIndex == (enemyManager.waveBosses.Length - 1)) //關底Boss戰鬥特效
                    sideB += Time.deltaTime;

                if (sideA <= 0.5f) //中Boss-關底Boss以外
                {
                    sideA += Time.deltaTime * 0.1f;
                }
            }
            else
            {
                if (sideA >= 0.15f)
                {
                    sideA -= Time.deltaTime * 0.1f;
                }
                else if (sideA <= 0.15f)
                {
                    sideA += Time.deltaTime * 0.1f;
                }

                if (sideB == 1)
                {
                    sideB -= Time.deltaTime * 0.1f;
                }
            }

            LightSide[0].gameObject.GetComponent<Image>().color = new Color(1, 1, 1, sideA);
            LightSide[1].gameObject.GetComponent<Image>().color = new Color(1, 1, 1, sideB);
        }
    }
    public GameObject AudioPlay(AudioSource audio, bool canDestroy)
    {

        audio.volume = SaveSystem.LoadGameVoice<SaveVoiceData>().Effect_num * SaveSystem.LoadGameVoice<SaveVoiceData>().All_num;
        GameObject temp;
        if (audio.gameObject.name == "EXPGain" || audio.gameObject.name == "hurtaudio")//音效平衡 避免一次出現太多導致發生耳膜攻擊
        {
            var items = FindObjectsOfType<AudioSource>();
            audio.volume /= items.Length;
        }
        if (audio.gameObject.name == "Levelup")

        {
            temp = Instantiate(audio.gameObject, playerScript.transform.position, Quaternion.identity);
            temp.gameObject.transform.parent = playerScript.transform;
            temp.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            temp = Instantiate(audio.gameObject);
            audio.Play();
        }
        if (canDestroy)
            Destroy(temp, temp.GetComponent<AudioSource>().clip.length);
        return temp;
    }
    public void CheckUpperLimit()
    {
        if (bombCount >= 10 && lifeCount >= 0 && playerLevel >= 3 && droneCount >= 6)
            FinishAchievement(16);
    }

    #region "存檔讀檔"
    public void Save()
    {
        SaveSystem.SaveGame(SavingData());
    }
    public void Load()
    {
        var saveData = SaveSystem.LoadGame<SaveData>();
        LoadData(saveData);
        if (!isRefreshed)
        {
            var saveData2 = SaveSystem.LoadGameVoice<SaveVoiceData>();
            LoadData(saveData2);
            if (!saveData2.canCheat&&GameStage==1)
            {
                RefreshGame();
            }
            isRefreshed = true;
        }
    }
    #endregion
    #region "存檔幫助"
    SaveData SavingData()
    {
        var saveData = new SaveData();
        saveData.playerBomb = bombCount;
        saveData.playerExp = playerExp;
        saveData.playerLevel = playerLevel;
        saveData.playerLife = lifeCount;
        saveData.droneCount = droneCount;
        saveData.GameStage = GameStage;
        saveData.playerScore = playerScore;
        saveData.HiPlayerScore = HiScore;
        saveData.AllAttack = AllAttack;
        saveData.AllBomb = AllBomb;
        saveData.AllHurt = AllHurt;
        saveData.AllTimeBarrage = AllTimeBarrage;
        saveData.enemyCount = enemyCount;
        saveData.killEnemy = killEnemy;
        for (int i = 0; i < Achievements.Length; i++)
        {
            saveData.Achievements[i] = Achievements[i];
        }
        return saveData;
    }

    void LoadData(SaveData saveData)
    {
        SetBomb(saveData.playerBomb);
        SetLife(saveData.playerLife);
        playerLevel = saveData.playerLevel;
        SetExp(saveData.playerExp);
        SetScore(saveData.playerScore);
        SetHiScore(saveData.HiPlayerScore);
        droneCount = saveData.droneCount;
        GameStage = saveData.GameStage;
        ReallyInvincible = saveData.Invincible;
        canControlAttack = !saveData.autoShoot;
        AllAttack = saveData.AllAttack;
        AllBomb = saveData.AllBomb;
        AllHurt = saveData.AllHurt;
        AllTimeBarrage = saveData.AllTimeBarrage;
        killEnemy = saveData.killEnemy;
        if(!isOperate)
        {
            enemyCount+=OperateEnemyCount();
            enemyCount+=saveData.enemyCount;
            Save();
            isOperate = true;
        }
        for (int i = 0; i < Achievements.Length; i++)
        {
            Achievements[i] = saveData.Achievements[i];
        }
    }
    void LoadData(SaveVoiceData saveData)
    {
        for (int i = 0; i < BackMusic.Length; i++)
        {
            BackMusic[i].volume = saveData.BGM_num * saveData.All_num;
            GetComponent<AudioSource>().volume = saveData.BGM_num * saveData.All_num;
        }
        for (int i = 0; i < MenuSound.Length; i++)
        {
            MenuSound[i].volume = saveData.Effect_num * saveData.All_num;
        }
        ChoicePlayer = saveData.ChoicePlayer;
        for (int i = 0; i < curinput.Length; i++)
        {
            curinput[i] = saveData.curinput[i];
        }
        isRush = saveData.isRush;
        isCheat = saveData.canCheat;
        switch (saveData.ChoiceDifficulty)
        {
            case 0:
                difficulty = Difficulty.easy;
                break;
            case 1:
                difficulty = Difficulty.middle;
                break;
            case 2:
                difficulty = Difficulty.Hard;
                break;
            case 3:
                difficulty = Difficulty.VerryHard;
                break;
            case 4:
                difficulty = Difficulty.Hell;
                break;            
        }
    }


    public IEnumerator BGMchange(AudioSource audio) //背景音樂切換 針對關底Boss
    {
        GetComponent<Rigidbody2D>().velocity = Vector3.up;
        yield return new WaitForSeconds(3f);
      //  if (GetComponent<AudioSource>().clip)
       // {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().PlayOneShot(audio.clip);
        //}
        GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        transform.position = new Vector3(0, 0, 0);
    }

    int OperateEnemyCount()
    {
        int index = 0;
        for (int i = 0; i < enemyManager.waveMonster.Length; i++)
        {
            index+=enemyManager.waveMonster[i].count;
        }
        for (int i = 0; i < enemyManager.waveBosses.Length; i++)
        {
            index+=enemyManager.waveBosses[i].bossPrefab.Length;
        }
        return index;
    }
    public IEnumerator StageResults()
    {

        if (GameStage == 1)
            FinishAchievement(0);
        else if (GameStage == 2)
            FinishAchievement(1);
        else if (GameStage == 3)
        {
            FinishAchievement(2);
            FinishAchievement(3);
        }

        if (difficulty == Difficulty.Hard)
            FinishAchievement(4);
        else if (difficulty == Difficulty.VerryHard)
            FinishAchievement(5);
        else if (difficulty == Difficulty.Hell)
            FinishAchievement(6);

        FinishAchievement(7 + ChoicePlayer);

        if (!AllHurt)
            FinishAchievement(19);
        else if (!AllBomb)
            FinishAchievement(20);
        else if (!AllTimeBarrage)
            FinishAchievement(21);
        else if (!AllAttack)
            FinishAchievement(22);

        if(killEnemy>=enemyCount)
            FinishAchievement(24);

        Achievements[25] = true;
        for (int i = 0; i < 25; i++)
        {
            if (!Achievements[i])
            {
                Achievements[25] = false;
            }
        }
        if (Achievements[25])
            FinishAchievement(25);

        GameStage += 1;
        StartCoroutine(BGMchange(BackMusic[2]));
        MapBonusScores[0].text = thisMapScore.ToString();
        MapBonusScores[1].text = playerScore.ToString(); //尚未使用 
        MapBonusScores[2].text = thisMapBombCount.ToString();
        MapBonusScores[3].text = "x1";
        MapBonusScores[4].text = thisMapHurtCount.ToString();
        MapBonusScores[5].text = "x1";
        float deathbonus = 1;
        float bombbonus = 1;

        switch (thisMapBombCount)
        {
            case 0:
                {
                    BonusShine[0].SetActive(true);
                    MapBonusScores[2].text += "  Great!!";
                    MapBonusScores[3].text = "x1.5 Bonus!";
                    bombbonus = 1.5f;
                    break;
                }
            case 1:
                {
                    BonusShine[0].SetActive(true);
                    MapBonusScores[2].text += "  Nice!";
                    MapBonusScores[3].text = "x1.2 Bonus!";
                    bombbonus = 1.2f;
                    break;
                }
            case 2:
            case 3:
            case 4:
                {
                    MapBonusScores[2].text += "";
                    MapBonusScores[3].text = "x1";
                    break;
                }

            case 5:
            case 6:
            case 7:
                {
                    MapBonusScores[2].text += "  Try dodging.";
                    MapBonusScores[3].text = "x1";
                    break;
                }
            case 8:
            case 9:
                {
                    MapBonusScores[2].text += "  YOU CHEESED IT!";
                    MapBonusScores[3].text = "x1";
                    break;
                }

        }


        switch (thisMapHurtCount)
        {
            case 0:
                {
                    BonusShine[1].SetActive(true);
                    MapBonusScores[4].text += "  Great!!";
                    MapBonusScores[5].text = "x1.5 Bonus!";
                    deathbonus = 1.5f;
                    break;
                }
            case 1:
                {
                    BonusShine[1].SetActive(true);
                    MapBonusScores[4].text += "  Nice!";
                    MapBonusScores[5].text = "x1.2 Bonus!";
                    deathbonus = 1.2f;
                    break;
                }
            case 6:
            case 7:
            case 8:
                {
                    MapBonusScores[2].text += "  Try harder.";
                    MapBonusScores[3].text = "x1";
                    break;
                }
            case 9:
                {
                    MapBonusScores[2].text += "  Well... You made it!";
                    MapBonusScores[3].text = "x1";
                    break;
                }


        }

        UIanimator.SetBool("IsClear", true);
        yield return new WaitForSeconds(8.3f);
        AudioPlay(MenuSound[2], true);
        yield return new WaitForSeconds(1);
        AudioPlay(MenuSound[2], true);
        yield return new WaitForSeconds(1.2f);
        thisMapScore *= bombbonus;
        thisMapScore = Mathf.RoundToInt(thisMapScore);
        MapBonusScores[0].text = thisMapScore.ToString();
        if (thisMapBombCount <= 1)
            AudioPlay(LevelupSound, true);
        yield return new WaitForSeconds(1.3f);
        thisMapScore *= deathbonus;
        thisMapScore = Mathf.RoundToInt(thisMapScore);
        MapBonusScores[0].text = thisMapScore.ToString();
        if (thisMapHurtCount <= 1)
            AudioPlay(LevelupSound, true);
        yield return new WaitForSeconds(1.3f);

        AddScore(thisMapScore);
        MapBonusScores[1].text = playerScore.ToString();
        Save();
        thisMapBomb = false;
        thisMapHurt = false;
        thisMapScore = 0;
        yield return new WaitForSeconds(5f);
        if (SceneManager.GetActiveScene().name == "Stage3" || isRush)
            StartCoroutine(Loadscene(0));
        else
            StartCoroutine(Loadscene(SceneManager.GetActiveScene().buildIndex + 1));
    }



    public IEnumerator Loadscene(int SceneIndex) //帶入淡出動畫的轉場
    {
        Time.timeScale = 1;
        StartCoroutine(BGMchange(null));
        UIanimator.SetBool("IsEnd", true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneIndex);
    }


    #endregion
}
