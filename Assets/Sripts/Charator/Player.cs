using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Coroutine coroutine;
    #region "Public"
    public float speed;
    public bool canMove = false;
    public bool canControlAttack;
    #endregion
    #region "Hide"
    [HideInInspector]
    public bool isAttack = false;
    [HideInInspector]
    public GameObject bulletPrefab;
    [HideInInspector]
    public Transform[] bulletTransform;
    [HideInInspector]
    public GameObject[] Drone;
    #endregion
    void FixedUpdate()
    {
        if (canMove)
        {
            Move();
        }
    }
    void Update()
    {
        if (canMove)
        {
            UseAttack();
        }
    }
    public void AddBro()
    {
        for (int i = 0; i < Drone.Length; i++)
        {
            if (GameManager.Instance.playerLevel > i / 2)
                Drone[i].SetActive(true);
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
    void UseAttack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isAttack = true;
            BroAnime();
            coroutine = StartCoroutine(Attack());
        }
        if (Input.GetKeyUp(KeyCode.Z)&&coroutine!=null)
        {
            isAttack = false;
            BroAnime();
            StopCoroutine(coroutine);
        }
    }
    void BroAnime()
    {
        for (int i = 2; i < Drone.Length; i++)
        {
            if (GameManager.Instance.playerLevel > i / 2)
            {
                if (isAttack)
                    Drone[i].GetComponent<Animator>().Play("Attack");
                else
                    Drone[i].GetComponent<Animator>().Play("NotAttack");
            }
        }
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
                }
            }
            for (int i = 0; i < Drone.Length; i++)
            {
                if (GameManager.Instance.playerLevel > i / 2)
                {
                    GameObject tempObject = Instantiate(bulletPrefab, Drone[i].transform.GetChild(0).transform.position, Quaternion.identity);
                    tempObject.GetComponent<Bullet>().canTrackEnemy = true;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
