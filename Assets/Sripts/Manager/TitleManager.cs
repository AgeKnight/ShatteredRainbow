using System;
using System.Collections;
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
}
public class TitleManager : MonoBehaviour
{

    static TitleManager instance;
    public static TitleManager Instance { get => instance; set => instance = value; }
    public AudioSource SelectSound;
    public AudioSource ClickSound;
    public AudioSource BackSound;
    public Slider BGM;
    public Text BGM_Text;
    public Slider Effect;
    public Text Effect_Text;
    public Slider All;
    public Text All_Text;
    void Awake()
    {
        Instance = this;
        Load();
    }
    public void StartGame()
    {
        StartCoroutine(startgame_alternative());
    }

    public void Highscorereset()
    {
        PlayerPrefs.SetFloat("Hi_Score", 0);
    }

    public IEnumerator startgame_alternative()
    {
        DontDestroyOnLoad(AudioPlay(ClickSound, true));
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Game1");
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
    public void VoiceControllBGM()
    {
        BGM_Text.text = ((int)(BGM.value * 100)).ToString();
        BackSound.volume = BGM.value;
        Save();
    }
    public void VoiceControllEffect()
    {
        Effect_Text.text = ((int)(Effect.value * 100)).ToString();
        SelectSound.volume = Effect.value;
        ClickSound.volume = Effect.value;
        Save();
    }
    public void VoiceControllAll()
    {
        All_Text.text = ((int)(All.value * 100)).ToString();
        
        BGM_Text.text = ((int)(All.value * 100)).ToString();
        BGM.value= All.value;
        BackSound.volume = All.value;

        Effect_Text.text = ((int)(All.value  * 100)).ToString();
        Effect.value= All.value;
        SelectSound.volume = All.value;
        ClickSound.volume = All.value;
        Save();
    }
    public void Save()
    {
        SaveSystem.SaveGameVoice(SavingData());
    }
    void RefreshGame()
    {
        BGM.value=100;
        Effect.value = 100;
        All.value = 100;
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
        return saveData;
    }
    void LoadData(SaveVoiceData saveData)
    {
        BGM.value = saveData.BGM_num;
        Effect.value = saveData.Effect_num;
        All.value = saveData.All_num;

        BGM_Text.text = ((int)(saveData.BGM_num  * 100)).ToString();
        BGM.value = saveData.BGM_num;
        BackSound.volume = saveData.BGM_num;

        Effect_Text.text = ((int)(saveData.Effect_num * 100)).ToString();
        Effect.value = saveData.Effect_num;
        SelectSound.volume = saveData.Effect_num;
        ClickSound.volume = saveData.Effect_num;

        All_Text.text = ((int)(saveData.All_num * 100)).ToString();
    }

}
