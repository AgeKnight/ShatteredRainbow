using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public bool isAttack = true;
    [HideInInspector]
    public GameObject bulletPrefab;
    [HideInInspector]
    public Transform[] bulletTransform;
    [HideInInspector]
    public GameObject[] Drone;
    #endregion
    void Start()
    {
        StartCoroutine(Attack());
    }
    void Update()
    {
        if (gameObject.GetComponent<Death>().isInvincible)
            Invincible();
        AddBro();
    }
    void FixedUpdate()
    {
        Move();
    }
    void AddBro()
    {
        for (int i = 0; i < Drone.Length; i++)
        {
            if (GameManager.Instance.playerDrone > i / 2)
            {
                Drone[i].SetActive(true);
                if(i>=2)
                {
                    if(isAttack)
                        Drone[i].GetComponent<Animator>().Play("Attack");
                    else
                        Drone[i].GetComponent<Animator>().Play("NotAttack");
                }
            }
            else
                Drone[i].SetActive(false);
        }
    }
    void Move()
    {
        int vertical = 0;
        int horizontal = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            horizontal = 1;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            horizontal = -1;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            vertical = -1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            vertical = 1;
        transform.Translate(vertical * speed * Time.deltaTime, horizontal * speed * Time.deltaTime, 0);
    }
    IEnumerator Attack()
    {
        while (isAttack)
        {
            for (int i = 0; i < bulletTransform.Length; i++)
            {
                if (i <= GameManager.Instance.playerLevel * 2)
                {
                    GameObject tempObject = Instantiate(bulletPrefab, bulletTransform[i].transform.position, Quaternion.identity);
                    GameManager.Instance.playerBullet.Add(tempObject);
                }
            }
            for (int i = 0; i < Drone.Length; i++)
            {
                if (GameManager.Instance.playerDrone > i / 2)
                {
                    GameObject tempObject = Instantiate(bulletPrefab, Drone[i].transform.GetChild(0).transform.position, Quaternion.identity);
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
            gameObject.GetComponent<Death>().isInvincible = false;
    }
}
