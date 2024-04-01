using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    static TitleManager instance;
    public static TitleManager Instance { get => instance; set => instance = value; }
    public AudioSource SelectSound;
    void Awake() 
    {
        Instance = this;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
