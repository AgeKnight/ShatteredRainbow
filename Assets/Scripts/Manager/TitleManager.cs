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
    public bool autoShoot;
    public bool Invincible;
    public int ChoicePlayer;
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
    public AudioSource SelectSound;
    public AudioSource ClickSound;
    public AudioSource BackSound;
    public ControkKey[] controkKeys;
    public GameObject ChooseCharactor;
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
    public Toggle FullscreenTog;
    public Toggle VsyncTog;
    public List<ResItem> resolutions = new List<ResItem>();
    [HideInInspector]
    public int SelectedRes;
    public Text CurrentRes;
    public GameObject[] OptinionMessage;
    public Text[] Records;
    public Toggle[] autoShoot;
  
   
    
    void Awake()
    {
        FullscreenTog.isOn = Screen.fullScreen;
        if (QualitySettings.vSyncCount == 1)
            VsyncTog.isOn = true;
        else
            VsyncTog.isOn = false;

            Instance = this;
        Load();

        bool foundres=false;
        for(int i=0; i<resolutions.Count;i++)
        {
            if(Screen.width==resolutions[i].Width&&Screen.height==resolutions[i].Height)
            {
                foundres = true;
                SelectedRes = i;
                CurrentRes.text = resolutions[SelectedRes].Width.ToString() + "x" + resolutions[SelectedRes].Height.ToString();
            }
        }
        if(!foundres)
        {
            ResItem newRes = new ResItem();
            newRes.Width = Screen.width;
            newRes.Height = Screen.height;
            resolutions.Add(newRes);
            SelectedRes = resolutions.Count - 1;
            CurrentRes.text = resolutions[SelectedRes].Width.ToString() + "x" + resolutions[SelectedRes].Height.ToString();
        }
    }

    public void StartGame()
    {
        Save();
        ChooseCharactor.SetActive(true);
    }

    public void Highscorereset()
    {
        Records[0].text = "0";
        SaveSystem.LoadGame<SaveData>().HiPlayerScore = 0;

    }

    public void ShowRecords()
    {
        Records[0].text = SaveSystem.LoadGame<SaveData>().HiPlayerScore.ToString();
    }

    public void ExitGame()
    {
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
    /* ������ܪ����Ϋ��s���إ\��h���}��
      public void OpnionUse(int value)
    {
        for(int i =0;i<OptinionMessage.Length;i++)
        {
            OptinionMessage[i].SetActive(false);
        }
        if(value>=0)
            OptinionMessage[value].SetActive(true);
    }*/
    public void VoiceControllBGM()
    {
        BGM_Text.text = ((int)(BGM.value * 100)).ToString();
        BackSound.volume = BGM.value * All.value;
    }
    public void VoiceControllEffect()
    {
        Effect_Text.text = ((int)(Effect.value * 100)).ToString();
        SelectSound.volume = Effect.value * All.value;
        ClickSound.volume = Effect.value * All.value;
    }
    public void VoiceControllAll()
    {
        All_Text.text = ((int)(All.value * 100)).ToString();



        BackSound.volume = BGM.value * All.value;



        SelectSound.volume = Effect.value * All.value;
        ClickSound.volume = Effect.value * All.value;
    }
    public void Save()
    {
        SaveSystem.SaveGameVoice(SavingData());
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
        bool noSave = false;
        if (!File.Exists(@"Assets\game_SaveData\Voice.game"))
        {
            noSave = true;
        }
        if (!noSave)
        {
            var saveData = SaveSystem.LoadGameVoice<SaveVoiceData>();
            LoadData(saveData);
        }
        else
        {
            RefreshGame();
            var saveData = SaveSystem.LoadGameVoice<SaveVoiceData>();
            LoadData(saveData);
        }
    }
    SaveVoiceData SavingData()
    {
        var saveData = new SaveVoiceData();
        saveData.BGM_num = BGM.value;
        saveData.Effect_num = Effect.value;
        saveData.All_num = All.value;
        saveData.autoShoot = autoShoot[0].isOn;
        saveData.Invincible = autoShoot[1].isOn;
        saveData.ChoicePlayer = ChoicePlayer;
        for (int i = 0; i < controkKeys.Length; i++)
        {
            saveData.curinput[i] = controkKeys[i].curinput;
        }
        return saveData;
    }
    void LoadData(SaveVoiceData saveData)
    {
        All.value = saveData.All_num;
        BGM.value = saveData.BGM_num;
        Effect.value = saveData.Effect_num;


        BGM_Text.text = ((int)(saveData.BGM_num * 100)).ToString();
        //     BGM.value = saveData.BGM_num;
        BackSound.volume = saveData.BGM_num * saveData.All_num;

        Effect_Text.text = ((int)(saveData.Effect_num * 100)).ToString();
        //   Effect.value = saveData.Effect_num;
        SelectSound.volume = saveData.Effect_num * saveData.All_num;
        ClickSound.volume = saveData.Effect_num * saveData.All_num;

        All_Text.text = ((int)(saveData.All_num * 100)).ToString();
        autoShoot[0].isOn = saveData.autoShoot;
        autoShoot[1].isOn =saveData.Invincible ; 
        ChoicePlayer = saveData.ChoicePlayer;
        for (int i = 0; i < controkKeys.Length; i++)
        {
            controkKeys[i].curinput = saveData.curinput[i];
        }
    }
    public void AutoShoot()
    {
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
            if(SelectedRes> resolutions.Count - 1)
            {
                SelectedRes = 0;
            }
        }
        CurrentRes.text = resolutions[SelectedRes].Width.ToString()+"x"+resolutions[SelectedRes].Height.ToString();
        Screen.SetResolution(resolutions[SelectedRes].Width, resolutions[SelectedRes].Height, FullscreenTog.isOn);

    }
}
