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
    Failed
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
    
}
public class GameManager : MonoBehaviour
{

    static GameManager instance;
    public static GameManager Instance { get => instance; set => instance = value; }
    #region "Private"
    int totalExp = 100;
    bool isOnButton = false;
    float sideA;
    float sideB;
    #endregion
    #region "Public"
    public Coroutine coroutine;
    [Header("復活秒數")]
    public float AllResurrectionTime;
    #endregion
    #region "Hide"
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

    public AudioSource[] ItemgainsSound;

    public AudioSource LevelupSound;
    [HideInInspector]
    public float playerScore;
    [HideInInspector]
    public GameObject[] LightSide;
   // [HideInInspector]
    public Text[] MapBonusScores;//0 本關分數 1 加成分數 2 炸彈bonus名 3 炸彈bonus 4 生命bonus名 5 生命bonus
    public GameObject[] BonusShine;
    [HideInInspector]
    public float thisMapScore = 0;
    [HideInInspector]
    public bool thisMapBomb = false;
    [HideInInspector]
    public bool thisMapHurt = false;

    [HideInInspector]
    public int thisMapBombCount=0; //本關炸彈使用數
    [HideInInspector]
    public int thisMapHurtCount=0; //本關死亡數

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
  //  [HideInInspector]
    public Sprite[] bossImages;//0 空心 1 實心 
    [HideInInspector]
    public GameObject[] Triangles;
    //public Animator BarUse;
    [HideInInspector]
    public Animator UIanimator;
    [HideInInspector]
    public Player playerScript;
    [HideInInspector]
    public GameObject[] bossStaire;
    [HideInInspector]
    //0 左上 1 右下
    public GameObject[] mapPosition;
    [HideInInspector]
    public int allBomb;
    [HideInInspector]
    public int allLife;
 //   [HideInInspector]
    public GameObject StageClear;
    [HideInInspector]
    public Slider BossBar;
    [HideInInspector]
    public StatusType statusType = StatusType.Pause;
    [HideInInspector]
    public Sprite[] playerFace;
    [HideInInspector]
    public GameObject playerStatus;
    [HideInInspector]
    public GameObject[] Lifes;
    [HideInInspector]
    public GameObject[] Bombs;
   //[HideInInspector]
    public GameObject[] Menus;//0 暫停 1 輸 2贏
    [HideInInspector]
    public Transform playerSpan;
    //[HideInInspector]
    public EnemyManager enemyManager;
    [HideInInspector]
    public GameObject Reciprocal;
    [HideInInspector]
    public int playerLevel;
    [HideInInspector]
    public bool canTrack = false;
    [HideInInspector]
    public GameObject player;
   // [HideInInspector]
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

        playerScript = Instantiate(player, playerSpan.transform.position, Quaternion.identity).gameObject.GetComponent<Player>();
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

        //  AudioPlay(BackMusic[0], false);
        //    BackgroundMusicChange(BackMusic[0]); //背景音樂的播放直接掛在gamemanager身上

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
                LoseGame();
                break;
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
        playerScript = Instantiate(player, PlayerResurrectionPosition.position, Quaternion.identity).GetComponent<Player>();
        playerScript.gameObject.GetComponent<Death>().isInvincible = true;
        playerScript.canMove = true;
        Invoke("PlayerNotInvincible", AllInvincibleTime);
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
               
                AudioPlay(ItemgainsSound[0],true);
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
                    AddExp(1);
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
                    AddExp(1);
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
        scoreText.text = playerScore.ToString();
        if (playerScore >= HiScore)
        {
            HiScore = playerScore;
            Hi_scoreText.text =  HiScore.ToString();
            SaveSystem.LoadGame<SaveData>().HiPlayerScore = HiScore;
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
      /*  if (lifeCount <= 0)
        {
            lifeCount = 0;
        }
        不會gameover 死到底生命仍鎖在0不到-1
         */
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
          //  playerStatus.gameObject.GetComponent<Image>().sprite = playerFace[playerLevel];
            if (playerLevel < 3)
            {
                playerExp -= totalExp;
                Level.text = "Level " + playerLevel.ToString();
            }
            else
            {
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
      //  playerStatus.gameObject.GetComponent<Image>().sprite = playerFace[playerLevel];
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
    /* void MinusLevel()
    {
        playerExp = 0;
        if (playerLevel > 0)
        {
            playerLevel -= 1;
            playerStatus.gameObject.GetComponent<Image>().sprite = playerFace[playerLevel];
            expBar.value = (float)playerExp / totalExp;
            Level.text = "Level " + playerLevel.ToString();
        }
    }
    */
    public void MinusEXP() //每次死亡扣除經驗值總數的1/4
    {
        int exploss; //經驗值所失
        if (playerLevel == 3)
        {
            exploss = (playerLevel * 100) / 4;  //經驗值所失
            playerExp = (playerLevel * 100) * 3 / 4;//懲罰後經驗值
        }
        else
        {
            exploss = (playerLevel * 100 + playerExp) / 4;  //經驗值所失
            playerExp = (playerLevel * 100 + playerExp) * 3 / 4; //懲罰後經驗值
        }
        for (int i = 0; i < exploss ; i++)
        {
            GameObject drop = Instantiate(EXP, playerScript.gameObject.GetComponent<Transform>().position, Quaternion.identity);
        }


        playerLevel = playerExp / 100;    //處罰後等級
        playerExp = playerExp - playerLevel * 100;    //處罰後的顯示經驗值    

       // playerStatus.gameObject.GetComponent<Image>().sprite = playerFace[playerLevel];
        expBar.value = (float)playerExp / totalExp;
        Level.text = "Level " + playerLevel.ToString();

    }



    public void ClearBarrage()
    {
        var barrages = FindObjectsOfType<Bullet>();
        for (int i = 0; i < barrages.Length; i++)
            if (barrages[i] != null)
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
            /*   bossStaire[i].SetActive(false);
               BossBar.gameObject.SetActive(false);
               Triangles[0].SetActive(false);
               Triangles[1].SetActive(false);*/
        }

        enemyManager.nowEveryStairTime = enemyManager.everyStairTime;
    }
    #endregion



    public void MenuUse()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
    public void Replay()
    {
        Time.timeScale = 1;
        DontDestroyOnLoad(AudioPlay(MenuSound[3], true));
        statusType = StatusType.Pause;
        // SceneManager.LoadScene("Game" + GameStage.ToString());
        //  SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartCoroutine(Loadscene(SceneManager.GetActiveScene().buildIndex));
    }
    public void BackToMenu()
    {
        Time.timeScale = 1;
        DontDestroyOnLoad(AudioPlay(MenuSound[3], true));
        statusType = StatusType.Pause;
        RefreshGame();
        StartCoroutine(Loadscene(0));
    }
    public void Continue()
    {
        Menus[1].SetActive(false);
        statusType = StatusType.Pause;
        //MinusLevel();
        AddLife(2);
        PlayerResurrection();
      
        Time.timeScale = 1;
    }
    void WinGame()
    {
        playerScript.enabled = false;
       // Menus[2].SetActive(true);
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
            /*     if(LightSide[0].gameObject.GetComponent<Image>().color.a<=0.5&&!enemyManager.isSpanBoss&&enemyManager.bossIndex==0||enemyManager.isSpanBoss)
               {

                   sideA+=Time.deltaTime/25;
               }
              else if(LightSide[0].gameObject.GetComponent<Image>().color.a>=0.5&&!enemyManager.isSpanBoss)
              {
                  sideA-=Time.deltaTime/25;
                }
               if (enemyManager.bossIndex != 0)
                {
                    sideB += Time.deltaTime / 25;
                }
            */


            if (enemyManager.isSpanBoss)//邊框效果修飾
            {

                if (sideB != 1 && enemyManager.bossIndex == (enemyManager.waveBosses.Length-1)) //關底Boss戰鬥特效
                    sideB += Time.deltaTime;

                if (sideA <= 0.5) //中Boss-關底Boss以外
                {
                    sideA += Time.deltaTime * 0.1f;
                }
            }
            else
            {
                if (sideA >= 0.25)
                {
                    sideA -= Time.deltaTime * 0.1f;
                }
                else if (sideA <= 0.25)
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
     
           audio.volume = SaveSystem.LoadGameVoice<SaveVoiceData>().Effect_num* SaveSystem.LoadGameVoice<SaveVoiceData>().All_num;
        GameObject temp;
        if (audio.gameObject.name == "EXPGain"|| audio.gameObject.name == "hurtaudio")//音效平衡 避免一次出現太多導致發生耳膜攻擊
        {
            var items = FindObjectsOfType<AudioSource>();
            audio.volume /= items.Length;
        }
        /*  if (audio.clip.name == "射擊3")
          {
            //  audio.pitch = Random.Range(0.8f, 1.2f);
              audio.volume /= 1.25f+playerLevel+droneCount/2;
          }*/
        if (audio.gameObject.name == "Levelup")

        {
            temp = Instantiate(audio.gameObject, playerScript.transform.position, Quaternion.identity);
            temp.gameObject.transform.parent = playerScript.transform;
            temp.transform.localScale =new Vector3(1, 1, 1);
        }
        else
             temp = Instantiate(audio.gameObject);
        audio.Play();
        if (canDestroy)
            Destroy(temp, temp.GetComponent<AudioSource>().clip.length);
        return temp;
   
      

    }
    #region "存檔讀檔"
    public void Save()
    {
        SaveSystem.SaveGame(SavingData());
    }
    public void Load()
    {
        bool noSave = false;
        var saveData2 = SaveSystem.LoadGameVoice<SaveVoiceData>();
        LoadData(saveData2);
        if (!File.Exists(@"Assets\game_SaveData\Game.game"))
        {
            noSave = true;
        }
        if (!noSave)
        {
            var saveData = SaveSystem.LoadGame<SaveData>();
            LoadData(saveData);
        }
        else
        {
            RefreshGame();
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
    }
    void LoadData(SaveVoiceData saveData)
    {
        for(int i = 0; i < BackMusic.Length; i++) 
        {
            BackMusic[i].volume = saveData.BGM_num * saveData.All_num;
            GetComponent<AudioSource>().volume = saveData.BGM_num* saveData.All_num;
        }
        for(int i = 0; i < MenuSound.Length; i++) 
        {
            MenuSound[i].volume = saveData.Effect_num * saveData.All_num;   
        }       
        canControlAttack = !saveData.autoShoot;
        Debug.Log(canControlAttack);
        Debug.Log(saveData.autoShoot);
    }


   public IEnumerator BGMchange(AudioSource audio) //背景音樂切換 針對關底Boss
    {
        GetComponent<Rigidbody2D>().velocity = Vector3.up;
    /*    while (this.GetComponent<AudioSource>().volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }*/
        yield return new WaitForSeconds(3f);
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(audio.clip);
        GetComponent<Rigidbody2D>().velocity = new Vector3 (0,0,0);
        transform.position = new Vector3(0, 0, 0);
    }


  public IEnumerator StageResults()
    {

        GameStage += 1;
        Save();
       StartCoroutine(BGMchange(BackMusic[2]));
        MapBonusScores[0].text = thisMapScore.ToString();
        MapBonusScores[1].text = playerScore.ToString(); //尚未使用 
        MapBonusScores[2].text = thisMapBombCount.ToString();
        MapBonusScores[3].text = "x1";
        MapBonusScores[4].text =  thisMapHurtCount.ToString();
        MapBonusScores[5].text = "x1";
        float deathbonus =1;
        float bombbonus =1;

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
        MapBonusScores[0].text = thisMapScore.ToString();
            if(thisMapBombCount<=1)
                AudioPlay(LevelupSound, true);
        yield return new WaitForSeconds(1.3f);
        thisMapScore *= deathbonus;
        MapBonusScores[0].text = thisMapScore.ToString();
            if(thisMapHurtCount<=1)
                AudioPlay(LevelupSound,true);
        yield return new WaitForSeconds(1.3f);

        AddScore(thisMapScore);
        MapBonusScores[1].text = playerScore.ToString();

        thisMapBomb = false;
        thisMapHurt = false;
     //   MapBonusScores[1].text = playerScore.ToString();
        thisMapScore = 0;
        yield return new WaitForSeconds(5f);
        StartCoroutine(Loadscene(SceneManager.GetActiveScene().buildIndex + 1));



    }



  public IEnumerator Loadscene(int SceneIndex) //帶入淡出動畫的轉場
    {
     
        StartCoroutine(BGMchange(null));
        GameManager.Instance.UIanimator.SetBool("IsEnd", true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneIndex);
    }


    #endregion
}
