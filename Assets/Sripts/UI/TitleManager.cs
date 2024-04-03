using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    static TitleManager instance;
    public static TitleManager Instance { get => instance; set => instance = value; }
    public AudioSource SelectSound;
    public AudioSource ClickSound;
    void Awake() 
    {
        Instance = this;
    }
    public void StartGame()
    {
        DontDestroyOnLoad(AudioPlay(ClickSound,true));
        SceneManager.LoadScene("Game");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public GameObject AudioPlay(AudioSource audio,bool canDestroy)
    {
        GameObject temp = Instantiate(audio.gameObject);
        audio.Play();
        if(canDestroy)
            Destroy(temp,1.5f);
        return temp;
    }
}
