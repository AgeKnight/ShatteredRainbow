using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region "Private"
    float InvincibleTime = 0;
    #endregion
    #region "Public"
    public float speed;
    #endregion
    #region "Hide"
    [HideInInspector]
    public GameObject bulletPrefab;
    [HideInInspector]
    public bool isInvincible = false;
    [HideInInspector]
    public Transform[] bulletTransform;
    //[HideInInspector]
    public Transform[] DroneBro;
    public Sprite[] DroneRightSprite;
    public Sprite[] DroneLeftSprite;
    public GameObject[] Drone;
    #endregion
    void Start()
    {
        StartCoroutine(Attack());
    }
    void Update()
    {
        if (isInvincible)
        {
            Invincible();
        }
        AddBro();
    }
    void FixedUpdate()
    {
        Move();
    }
    void AddBro()
    {
        if(GameManager.Instance.playerDrone<=0)
        {
            for (int i = 0; i < Drone.Length; i++)
            {
                Drone[i].SetActive(false);
            }
        }
        else
        {
            Drone[0].GetComponent<SpriteRenderer>().sprite =  DroneRightSprite[GameManager.Instance.playerDrone-1];
            Drone[1].GetComponent<SpriteRenderer>().sprite =  DroneLeftSprite[GameManager.Instance.playerDrone-1];
            for (int i = 0; i < Drone.Length; i++)
            {
                Drone[i].SetActive(true);
            }
        }
    }
    void Move()
    {
        int vertical = 0;
        int horizontal = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            horizontal = 1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            horizontal = -1;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            vertical = -1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            vertical = 1;
        }
        transform.Translate(vertical * speed * Time.deltaTime, horizontal * speed * Time.deltaTime, 0);
    }
    IEnumerator Attack()
    {
        while (true)
        {
            for (int i = 0; i < bulletTransform.Length; i++)
            {
                if (i <= GameManager.Instance.playerLevel * 2)
                {
                    GameObject tempObject = Instantiate(bulletPrefab, bulletTransform[i].transform.position, Quaternion.identity);
                    GameManager.Instance.playerBullet.Add(tempObject);
                }
            }
            for (int i = 0; i < DroneBro.Length; i++)
            {
                if (GameManager.Instance.playerDrone>i/2)
                {
                    GameObject tempObject = Instantiate(bulletPrefab, DroneBro[i].transform.position, Quaternion.identity);
                    tempObject.GetComponent<Bullet>().canTrackEnemy = true;
                    GameManager.Instance.playerBullet.Add(tempObject);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    void Invincible()
    {
        InvincibleTime += Time.deltaTime;
        if (InvincibleTime >= GameManager.Instance.AllInvincibleTime)
        {
            isInvincible = false;
        }
    }
}
