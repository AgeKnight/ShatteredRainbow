using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    float nowspeed;
    //   float useDroneTime = 0;
    float[] annularColor = { 0.8f, 0.8f };
    bool isUseBomb = false;
    public bool isUseTimeBarrage = false;
    bool BombAttack = true;
    bool isUseDrone = false;
    float invokeTime;
    Coroutine coroutine;
    Bomb myBomb;
    AnnularSlider annular;
    Image Annular;
    GameObject shootEffect;
    int trailnums = 0;
    Color[] shade = { new Vector4(1, 1, 0.69f), new Vector4(0.69f, 0.97f, 1), new Vector4(1, 0.69f, 0.71f) };
    #region "Public"
    public float maxUseDroneTime = 20;
    public int Dronecount;
    public float SlowSpeed;
    public float speed;
    public GameObject TimeBarrageTrail;
    public float timeRegain;
    public bool canControlAttack;
    public float useBombTime;
    public float MaxBarrageTime;
    public GameObject Bomb;
    public GameObject SliderTime;
    public Transform BombPosition;
    public AudioSource[] musicEffect; //0射擊音 1炸彈音

    #endregion
    #region "Hide"
    [HideInInspector]
    public GameObject DroneGroup;
    [HideInInspector]
    public GameObject AnnularCircle;
    [HideInInspector]
    public bool canMove = false;
    [HideInInspector]
    public bool isAttack = false;
    public GameObject[] bulletPrefab;
    
    [HideInInspector]
    public Transform[] bulletTransform;
    [HideInInspector]
    public GameObject[] Drone;
    #endregion
    void Awake()
    {
        nowspeed = speed;
        canControlAttack = GameManager.Instance.canControlAttack;
        SetBro(GameManager.Instance.droneCount);
        Annular = AnnularCircle.GetComponent<Image>();
        invokeTime = MaxBarrageTime;
        annular = SliderTime.GetComponent<AnnularSlider>();
        Annular.color = new Color(1, 1, 1, 0);
        StartCoroutine(RegainTimeBarrage());
    }

    void Update()
    {
        /*  
         if (isUseDrone)
          {

              useDroneTime += Time.deltaTime;
              if (useDroneTime >= maxUseDroneTime)
              {
                  for (int i = 0; i < Drone.Length; i++)
                  {
                      Drone[i].SetActive(false);
                  }
                  useDroneTime = 0;
                  isUseDrone = false;
              }



          }
          */


        if (canMove && BombAttack)
        {
            if (canControlAttack)
            {
                UseAttack();
            }
            else if (!canControlAttack && !isAttack)
            {
                isAttack = true;
                coroutine = StartCoroutine(Attack());
            }
            UseButton();
        }
        if (!BombAttack)
        {
            isAttack = false;
        }
        if (isUseTimeBarrage)
        {
            invokeTime -= Time.unscaledDeltaTime;
            annular.Value = invokeTime / MaxBarrageTime;
            if (invokeTime <= 0)
            {
                isUseTimeBarrage = false;
                Time.timeScale = 1;
            }
        }
        if (canMove)
        {
            Move();
        }
        ChangeColorAnnular();
        UseTimeBarrage();
    }


    void ChangeColorAnnular()
    {
        if (invokeTime >= MaxBarrageTime)
        {
            Annular.color = new Color(1, 1, 1, 0);
        }
        else
        {
            annularColor[0] = invokeTime / MaxBarrageTime * 0.8f;
            annularColor[1] = invokeTime / MaxBarrageTime * 0.8f;
            Annular.color = new Color(1, annularColor[0], annularColor[1], 1);
        }
    }
    public void SetBro(int value)
    {
        GameManager.Instance.droneCount = value;
        if(GameManager.Instance.droneCount>0)
        {
            isUseDrone = true;
        }
        else 
        {
            GameManager.Instance.droneCount = 0;
            isUseDrone = false;
        }
        for (int i = 0; i < 6; i++)
        {
            Drone[i].SetActive(false);
        }
        for (int i = 0; i <= GameManager.Instance.droneCount-1; i++)
        {
            Drone[i].SetActive(true);
        }
    }
    public void AddBro(int value)
    {
        
        GameManager.Instance.droneCount += value;
        if(GameManager.Instance.droneCount>0)
        {
            isUseDrone = true;
        }
        else 
        {
            GameManager.Instance.droneCount = 0;
            isUseDrone = false;
        }
        for (int i = 0; i < 6; i++)
        {
            Drone[i].SetActive(false);
        }
        for (int i = 0; i <= GameManager.Instance.droneCount-1; i++)
        {
            Drone[i].SetActive(true);
        }
    }
    void Move()
    {
        
        int vertical = 0;
        int horizontal = 0;
        if (Input.GetKey(GameManager.Instance.curinput[0]))
            horizontal = 1;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            horizontal = -1;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            vertical = -1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
           vertical = 1;
           


        // if (isUseBomb)
        //     transform.Translate(vertical * speed * myBomb.SlowSpeed * Time.deltaTime, horizontal * speed * myBomb.SlowSpeed * Time.deltaTime, 0);
        // else
        //     transform.Translate(vertical * speed * Time.deltaTime, horizontal * speed * Time.deltaTime, 0);
            
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed * vertical , speed *horizontal ); //�H���z���D�����ʡA������|���b����ݰʪ����D

        if (isUseBomb)
            speed = nowspeed * myBomb.SlowSpeed;
        else
            speed = nowspeed;


    }

    void UseAttack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {

            isAttack = true;
            //BroAnime();
            coroutine = StartCoroutine(Attack());
        }
        if (Input.GetKeyUp(KeyCode.Z) && coroutine != null)
        {

            isAttack = false;
            DroneGroup.GetComponent<Animator>().SetBool("Drone_attacking", false);
            //  BroAnime();
        }
    }
    /*void BroAnime()  //�ϥ�animator
    {
        if (isUseDrone)
        {
            for (int i = 2; i < Drone.Length; i++)
            {
                if (isAttack)
                    Drone[i].GetComponent<Animator>().Play("Attack");
                else
                    Drone[i].GetComponent<Animator>().Play("NotAttack");
            }
        }
    }*/
    IEnumerator Attack()
    {
        while (isAttack)
        {
            shootEffect = GameManager.Instance.AudioPlay(musicEffect[1], false);
            shootEffect.transform.parent = this.transform;
            for (int i = 0; i < bulletTransform.Length; i++)
            {
                if (i <= GameManager.Instance.playerLevel * 2)
                {

                    Instantiate(bulletPrefab[0], bulletTransform[i].transform.position, Quaternion.identity);
                }
            }
            if (isUseDrone)
            {

                DroneGroup.GetComponent<Animator>().SetBool("Drone_attacking", true);
                for (int i = 0; i < GameManager.Instance.droneCount; i++)
                {
                    GameObject tempObject = Instantiate(bulletPrefab[1], Drone[i].transform.GetChild(0).transform.position, Quaternion.identity);
                    tempObject.GetComponent<Bullet>().canTrackEnemy = true;
                }
            }

            yield return new WaitForSeconds(0.1f);
            Destroy(shootEffect.gameObject);
        }
    }
    void UseButton()
    {
        if (Input.GetKeyDown(KeyCode.X) && GameManager.Instance.bombCount > 0 && !isUseBomb && !isUseTimeBarrage)
        {
            GameManager.Instance.thisMapBomb = true;
            GameManager.Instance.thisMapBombCount +=1;
            GameManager.Instance.AudioPlay(musicEffect[2],true);
            myBomb = Instantiate(Bomb, BombPosition.position, Quaternion.identity).GetComponent<Bomb>();
            myBomb.gameObject.transform.parent = this.gameObject.transform;
            isUseBomb = true;
            BombAttack = myBomb.canUseAttack;
            if (GameManager.Instance.enemyManager.isSpanBoss)
                GameManager.Instance.awardType = AwardType.Common;
            GameManager.Instance.AddBomb(-1);
            gameObject.GetComponent<Death>().isInvincible = true;
            Invoke("againUseBomb", useBombTime);
        }
        if (myBomb)
        {
            myBomb.transform.position = transform.position;
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
        if (Input.GetKeyDown(KeyCode.C) && !isUseBomb && invokeTime > 0)
        {

            isUseTimeBarrage = true;
            StartCoroutine(Trails());
            this.GetComponent<Animator>().SetBool("AnimBulletTime", isUseTimeBarrage);
            Time.timeScale = SlowSpeed;
            Time.fixedDeltaTime = Time.timeScale * 0.05f;
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            isUseTimeBarrage = false;
            this.GetComponent<Animator>().SetBool("AnimBulletTime", isUseTimeBarrage);
            Time.timeScale = 1;
        }
    }
    IEnumerator RegainTimeBarrage()
    {
        while (true)
        {
            if (invokeTime < MaxBarrageTime && !isUseTimeBarrage)
            {
                AddTimeBarrage(timeRegain);
                yield return new WaitForSeconds(1f);
            }
            else
                yield return null;
        }
    }

    //子彈時間的殘影
    IEnumerator Trails()
    {
        while (isUseTimeBarrage)
        {

            GameObject shadow = Instantiate(TimeBarrageTrail, this.transform.position, Quaternion.identity);
            shadow.GetComponent<SpriteRenderer>().color = shade[trailnums];
            yield return new WaitForSeconds(0.05f);
            if (trailnums < 2)
                trailnums += 1;
            else
                trailnums = 0;
            Destroy(shadow, 0.2f);
        }
    }


    public void AddTimeBarrage(float value)
    {
        invokeTime += value;
        annular.Value = invokeTime / MaxBarrageTime;
        if (invokeTime >= MaxBarrageTime)
        {
            invokeTime = MaxBarrageTime;
        }
    }
}
