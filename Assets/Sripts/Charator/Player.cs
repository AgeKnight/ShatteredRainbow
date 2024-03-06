using System;
using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool isUseBomb = false;
    bool isUseTimeBarrage = false;
    bool BombAttack = true;
    float invokeTime;
    Coroutine coroutine;
    Bomb myBomb;
    #region "Public"
    public float SlowSpeed;
    public float speed;
    public float timeRegain;
    public bool canControlAttack;
    public float useBombTime;
    public float MaxBarrageTime;
    public GameObject Bomb;
    public Transform BombPosition;
    #endregion
    #region "Hide"
    [HideInInspector]
    public bool canMove = false;
    [HideInInspector]
    public bool isAttack = false;
    [HideInInspector]
    public GameObject bulletPrefab;
    [HideInInspector]
    public Transform[] bulletTransform;
    [HideInInspector]
    public GameObject[] Drone;
    #endregion
    void Awake() 
    {
        invokeTime = MaxBarrageTime;
    }
    void Update()
    {
        if (canMove&&BombAttack)
        {
            if(canControlAttack)
            {
                UseAttack();
            }
            else if(!canControlAttack&&!isAttack)
            {
                isAttack = true;
                coroutine = StartCoroutine(Attack());
            }
            UseButton();
        }
        if(!BombAttack)
        {
            isAttack=false;
        }
        if(isUseTimeBarrage)
        {
            invokeTime -= Time.unscaledDeltaTime;
            if(invokeTime<=0)
            {
                isUseTimeBarrage=false;
                Time.timeScale=1;
            }
        }
        if (canMove)
        {
            Move();
        }
        UseTimeBarrage();
        RegainTimeBarrage();
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
        if(isUseBomb)
            transform.Translate(vertical*speed *myBomb.SlowSpeed* Time.deltaTime, horizontal*speed*myBomb.SlowSpeed*Time.deltaTime, 0);
        else
            transform.Translate(vertical*speed* Time.deltaTime, horizontal*speed*Time.deltaTime, 0);   
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
        while (isAttack)
        {
            for (int i = 0; i < bulletTransform.Length; i++)
            {
                if (i <= GameManager.Instance.playerLevel * 2)
                {
                    Instantiate(bulletPrefab, bulletTransform[i].transform.position, Quaternion.identity);
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
    void UseButton()
    {
        if(Input.GetKeyDown(KeyCode.X)&&GameManager.Instance.boumbCount>0&&!isUseBomb&&!isUseTimeBarrage)
        {
            myBomb = Instantiate(Bomb,BombPosition.position,Quaternion.identity).GetComponent<Bomb>();
            myBomb.gameObject.transform.parent = this.gameObject.transform;  
            isUseBomb = true;
            BombAttack = myBomb.canUseAttack;

            if(GameManager.Instance.enemyManager.isSpanBoss)
                GameManager.Instance.awardType = AwardType.Common;
            GameManager.Instance.AddBottom(-1);
            gameObject.GetComponent<Death>().isInvincible = true;
            Invoke("againUseBomb",useBombTime);           
        }
    }
    void againUseBomb()
    {
        Destroy(myBomb.gameObject);
        isUseBomb = false;
        BombAttack = true;
        gameObject.GetComponent<Death>().isInvincible = false;
    }
    void UseTimeBarrage()
    {
        if(Input.GetKeyDown(KeyCode.C)&&!isUseBomb&&invokeTime>0)
        {
            isUseTimeBarrage=true;
            Time.timeScale = SlowSpeed;            
        }
        if(Input.GetKeyUp(KeyCode.C))
        {
            isUseTimeBarrage=false;
            Time.timeScale=1;
        }
    }
    void RegainTimeBarrage()
    {
        if(invokeTime<MaxBarrageTime&&!isUseTimeBarrage)
        {
            invokeTime+=(Time.deltaTime)/timeRegain;
        }
    }
    public void AddTimeBarrage(int value)
    {
        invokeTime+=value;
        if(invokeTime>=MaxBarrageTime)
        {
            invokeTime=MaxBarrageTime;
        }
    }
}
