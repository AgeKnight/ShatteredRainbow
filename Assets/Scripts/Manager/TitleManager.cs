using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class SaveVoiceData
{
    public float BGM_num;
    public float Effect_num;
    public float All_num;
    public int ChoicePlayer;
    public bool canCheat = false;
    public bool isRush = false;
    public int ChoiceDifficulty;
    public KeyCode[] curinput = new KeyCode[30];
}

[System.Serializable]
public class ResItem
{
    public int Width, Height;
}
public class TitleManager : MonoBehaviour
{
    static TitleManager instance;
    public static TitleManager Instance { get => instance; set => instance = value; }
    #region "Hide"
    //[HideInInspector]
    public int chooseStage;

    [HideInInspector]
    public int GameStage;
    [HideInInspector]
    public bool canSaveGameData = false;
    [HideInInspector]
    public float HiScore = 0;
    [HideInInspector]
    public int HiKill;
    [HideInInspector]
    public int HiBomb;
    [HideInInspector]
    public int HiDeath;
    [HideInInspector]
    public int HiBT;
    [HideInInspector]
    public bool[] Achievements = new bool[30];
    [HideInInspector]
    public bool isRush = false;
    [HideInInspector]
    public AudioSource SelectSound;
    [HideInInspector]
    public AudioSource ClickSound;
    [HideInInspector]
    public AudioSource BackSound;
    [HideInInspector]
    public ControkKey[] controkKeys;
    [HideInInspector]
    public int ChoicePlayer = 0;
    [HideInInspector]
    public Slider BGM;
    [HideInInspector]
    public Text BGM_Text;
    [HideInInspector]
    public Slider Effect;
    [HideInInspector]
    public Text Effect_Text;
    [HideInInspector]
    public Slider All;
    [HideInInspector]
    public Text All_Text;
    [HideInInspector]
    public Toggle FullscreenTog;
    [HideInInspector]
    public Toggle VsyncTog;
    [HideInInspector]
    public List<ResItem> resolutions = new List<ResItem>();
    [HideInInspector]
    public int SelectedRes;
    [HideInInspector]
    public Text CurrentRes;
    [HideInInspector]
    public Text[] Records;
    [HideInInspector]
    public Toggle[] autoShoot;
    [HideInInspector]
    public int Level;
    [HideInInspector]
    public int Bomb;
    [HideInInspector]
    public int Life;
    [HideInInspector]
    public int Drone;
    [HideInInspector]
    public GameObject CheatObject;
    [HideInInspector]
    public GameObject BossChoice;
    [HideInInspector]
    public int chooseBoss = 0;
    [HideInInspector]
    public int ChoiceDifficulty;
    [HideInInspector]
    public Image[] CharImage;
    [HideInInspector]
    public GameObject[] CharText;
    [HideInInspector]
    public GameObject[] ExtraObject;
    [HideInInspector]
    public GameObject[] OptinionMessage;
    [HideInInspector]
    public Image[] AchievementObject;
    [HideInInspector]
    public GameObject[] AchievementDifficulty;
    #endregion   
    public Sprite[] AchievementsImage;
    void Awake()
    {
        FullscreenTog.isOn = Screen.fullScreen;
        if (QualitySettings.vSyncCount == 1)
            VsyncTog.isOn = true;
        else
            VsyncTog.isOn = false;

        Instance = this;
        Load();
        AchievementBtn();
        bool foundres = false;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (Screen.width == resolutions[i].Width && Screen.height == resolutions[i].Height)
            {
                foundres = true;
                SelectedRes = i;
                CurrentRes.text = resolutions[SelectedRes].Width.ToString() + "x" + resolutions[SelectedRes].Height.ToString();
            }
        }
        if (!foundres)
        {
            ResItem newRes = new ResItem();
            newRes.Width = Screen.width;
            newRes.Height = Screen.height;
            resolutions.Add(newRes);
            SelectedRes = resolutions.Count - 1;
            CurrentRes.text = resolutions[SelectedRes].Width.ToString() + "x" + resolutions[SelectedRes].Height.ToString();
        }
        if(Achievements[22])
        {
            AchievementDifficulty[0].SetActive(true);
            AchievementDifficulty[1].SetActive(false);
        }
        else
        {
            AchievementDifficulty[0].SetActive(false);
            AchievementDifficulty[1].SetActive(true);
        }
    }
    public void AchievementBtn()
    {
       for (int i = 0; i < AchievementObject.Length; i++)
       {
            if(Achievements[i])
            {
                AchievementObject[i].sprite = AchievementsImage[1];
            }
            else
            {
                AchievementObject[i].sprite = AchievementsImage[0];
            }
       }
    }
    public void Highscorereset()
    {
        HiScore = 0;
        HiBomb = 0;
        HiDeath = 0;
        HiKill = 0;
        HiBT = 0;
        for (int i = 0; i < Records.Length; i++)
        {
            Records[i].text = "0";
        }
        for (int i = 0; i < Achievements.Length; i++)
        {
            Achievements[i]= false;
        }
        AchievementBtn();
        canSaveGameData = true;
        Save();
    }

    public void ShowRecords()
    {
        Load();
        Records[0].text = HiScore.ToString();
        Records[1].text = HiDeath.ToString();
        Records[2].text = HiBomb.ToString();
        Records[3].text = HiKill.ToString();
        Records[4].text = HiBT.ToString();
    }
    public void AchievementCheat(GameObject hint)
    {
        if (Achievements[3] == true)
        {
            ExtraObject[0].SetActive(true);
            ExtraObject[2].SetActive(autoShoot[2].isOn);
            hint.SetActive(false);
        }
        else
        {
            hint.SetActive(true);
            ExtraObject[0].SetActive(false);
            ExtraObject[2].SetActive(false);
            CheatObject.SetActive(autoShoot[2].isOn);
        }
    }
    public void AchievementRush(GameObject hint)
    {
        if(Achievements[6] == true)
        {
            ExtraObject[1].SetActive(true);
            hint.SetActive(false);
        }
        else
        {
            hint.SetActive(true);
            ExtraObject[1].SetActive(false);
        }
    }

    public void ExitGame()
    {
        Save();
        Application.Quit();
    }

    public GameObject AudioPlay(AudioSource audio, bool canDestroy)
    {
        GameObject temp = Instantiate(audio.gameObject);
        audio.Play();
        if (canDestroy)
            Destroy(temp, 1.5f);
        return temp;
    }
    public void VoiceControllBGM()
    {
        BGM_Text.text = ((int)(BGM.value * 100)).ToString();
        BackSound.volume = BGM.value * All.value;
        Save();
    }
    public void VoiceControllEffect()
    {
        Effect_Text.text = ((int)(Effect.value * 100)).ToString();
        SelectSound.volume = Effect.value * All.value;
        ClickSound.volume = Effect.value * All.value;
        Save();
    }
    public void VoiceControllAll()
    {
        All_Text.text = ((int)(All.value * 100)).ToString();
        BackSound.volume = BGM.value * All.value;
        SelectSound.volume = Effect.value * All.value;
        ClickSound.volume = Effect.value * All.value;
        Save();
    }
    public void Save()
    {
        SaveSystem.SaveGameVoice(SavingData());
        if(canSaveGameData)
        {
            canSaveGameData = false;
            SaveSystem.SaveGame(SavingGameData());
        }
    }
    void RefreshGame()
    {
        BGM.value = 100;
        Effect.value = 100;
        All.value = 100;
        for (int i = 0; i < autoShoot.Length; i++)
        {
            autoShoot[i].isOn = false;
        }
        Save();
    }
    public void Load()
    {
        if (!File.Exists(@"Assets\game_SaveData\Voice.game") || !File.Exists(@"Assets\game_SaveData\Game.game"))
        {
            RefreshGame();
        }
        else
        {
            var saveData = SaveSystem.LoadGameVoice<SaveVoiceData>();
            LoadData(saveData);
            var saveData2 = SaveSystem.LoadGame<SaveData>();
            LoadData(saveData2);
        }
    }
    SaveData SavingGameData()
    {
        var saveData = new SaveData();
        saveData.Invincible = autoShoot[1].isOn;
        saveData.autoShoot = autoShoot[0].isOn;
        saveData.playerBomb = Bomb;
        saveData.playerLevel = Level;
        saveData.playerLife = Life;
        saveData.droneCount = Drone;
        saveData.HiPlayerScore = HiScore;
        saveData.AllKill = HiKill;
        saveData.AllDeath = HiDeath;
        saveData.AllUseBomb = HiBomb;
        saveData.AllUseBT = HiBT;
        saveData.GameStage = GameStage;
        for (int i = 0; i < Achievements.Length; i++)
        {
            saveData.Achievements[i] = Achievements[i];
        }
        return saveData;
    }
    SaveVoiceData SavingData()
    {
        var saveData = new SaveVoiceData();
        saveData.BGM_num = BGM.value;
        saveData.Effect_num = Effect.value;
        saveData.All_num = All.value;
        saveData.ChoicePlayer = ChoicePlayer;
        saveData.canCheat = autoShoot[2].isOn;
        saveData.isRush = isRush;
        saveData.ChoiceDifficulty = ChoiceDifficulty;
        for (int i = 0; i < controkKeys.Length; i++)
        {
            saveData.curinput[i] = controkKeys[i].curinput;
        }
        return saveData;
    }
    void LoadData(SaveData saveData)
    {
        autoShoot[1].isOn = saveData.Invincible;
        autoShoot[0].isOn = saveData.autoShoot;
        Bomb = saveData.playerBomb;
        Level = saveData.playerLevel;
        Life = saveData.playerLife;
        HiScore = saveData.HiPlayerScore;
        HiBomb = saveData.AllUseBomb;
        HiDeath = saveData.AllDeath;
        HiKill = saveData.AllKill;
        HiBT = saveData.AllUseBT;
        for (int i = 0; i < Achievements.Length; i++)
        {
            Achievements[i] = saveData.Achievements[i];
        }
    }
    void LoadData(SaveVoiceData saveData)
    {
        All.value = saveData.All_num;
        BGM.value = saveData.BGM_num;
        Effect.value = saveData.Effect_num;


        BGM_Text.text = ((int)(saveData.BGM_num * 100)).ToString();
        BackSound.volume = saveData.BGM_num * saveData.All_num;

        Effect_Text.text = ((int)(saveData.Effect_num * 100)).ToString();
        SelectSound.volume = saveData.Effect_num * saveData.All_num;
        ClickSound.volume = saveData.Effect_num * saveData.All_num;

        All_Text.text = ((int)(saveData.All_num * 100)).ToString();
        autoShoot[2].isOn = saveData.canCheat;
        ChoicePlayer = 0;
        ChoiceDifficulty = saveData.ChoiceDifficulty;
        for (int i = 0; i < controkKeys.Length; i++)
        {
            controkKeys[i].curinput = saveData.curinput[i];
        }
        if (saveData.canCheat)
        {
            var saveData2 = SaveSystem.LoadGame<SaveData>();
            LoadData(saveData2);
        }
    }
    public void AutoShoot()
    {
        Save();
    }
    public void justBoss()
    {
        if(!autoShoot[3].isOn)
        {
            chooseBoss = 0;
        }
        else
        {
            chooseBoss = 1;
        }
        BossChoice.SetActive(autoShoot[3].isOn);
    }
    public void Cheat()
    {
        if(!autoShoot[2].isOn)
        {
            Level=0;
            Bomb=3;
            Life=3;
            Drone=0;
            autoShoot[0].isOn = false;
            autoShoot[1].isOn = false;
        }
        CheatObject.SetActive(autoShoot[2].isOn);
        Save();
    }
    public void VideoSetting()
    {
        Screen.fullScreen = FullscreenTog.isOn;
        if (VsyncTog.isOn)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }
    public void ResolutionChange(bool plus)
    {
        if (!plus)
        {
            SelectedRes--;
            if (SelectedRes < 0)
            {
                SelectedRes = resolutions.Count - 1;
            }
        }
        else
        {
            SelectedRes++;
            if (SelectedRes > resolutions.Count - 1)
            {
                SelectedRes = 0;
            }
        }
        CurrentRes.text = resolutions[SelectedRes].Width.ToString() + "x" + resolutions[SelectedRes].Height.ToString();
        Screen.SetResolution(resolutions[SelectedRes].Width, resolutions[SelectedRes].Height, FullscreenTog.isOn);

    }
}
