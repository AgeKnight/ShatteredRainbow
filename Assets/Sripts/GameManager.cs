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
    #endregion
    #region "Public"
    public int playerLife;
    public float AllResurrectionTime;
    #endregion
    #region "Hide"
    [HideInInspector]
    public GameObject player;
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
        for (int i = 0; i < Playerbullet.Count; i++)
        {
            if (Playerbullet[i] != null)
            {
                Destroy(Playerbullet[i]);
            }
        }
        for (int i = 0; i < tempEnemy.Allbullet.Count; i++)
        {
            if (tempEnemy.Allbullet[i] != null)
            {
                Destroy(tempEnemy.Allbullet[i]);
            }
        }
        tempEnemy.Allbullet.Clear();
        Playerbullet.Clear();
    }
}
