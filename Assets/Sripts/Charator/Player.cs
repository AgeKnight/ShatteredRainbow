using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    float useDroneTime = 0;
    float[] annularColor = { 0.8f, 0.8f};
    bool isUseBomb = false;
    bool isUseTimeBarrage = false;
    bool BombAttack = true;
    bool isUseDrone = false;
    bool isPlayTimeMusic = true;
    float invokeTime;
    Coroutine coroutine;
    Bomb myBomb;
    AnnularSlider annular;
    Image Annular;
    GameObject shootEffect;
    #region "Public"
    public float maxUseDroneTime = 20;
    public float SlowSpeed;
    public float speed;
    public float timeRegain;
    public bool canControlAttack;
    public float useBombTime;
    public float MaxBarrageTime;
    public GameObject Bomb;
    public GameObject SliderTime;
    public Transform BombPosition;
    public AudioSource[] musicEffect;
    #endregion
    #region "Hide"
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
        Annular = AnnularCircle.GetComponent<Image>();
        invokeTime = MaxBarrageTime;
        annular = SliderTime.GetComponent<AnnularSlider>();
        Annular.color = new Color(1, 1, 1, 0);
        StartCoroutine(RegainTimeBarrage());
    }
    void Update()
    {
        if (isUseDrone)
        {
            useDroneTime += Time.deltaTime;
            if (useDroneTime >=maxUseDroneTime)
            {
                for (int i = 0; i < Drone.Length; i++)
                {
                    Drone[i].SetActive(false);
                }
                isUseDrone = false;
            }
        }
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
        UseTimeBarrage();
        ChangeColorAnnular();
    }
    void ChangeColorAnnular()
    {
        if(invokeTime >= MaxBarrageTime)
        {
            Annular.color = new Color(1, 1, 1, 0);
        }
        else
        {
            annularColor[0] =  invokeTime / MaxBarrageTime*0.8f;
            annularColor[1] = invokeTime / MaxBarrageTime*0.8f;
            Annular.color = new Color(1, annularColor[0], annularColor[1], 1);
        }      
    }
    public void AddBro()
    {
        isUseDrone = true;
        useDroneTime = 0;
        for (int i = 0; i < Drone.Length; i++)
        {
            Drone[i].SetActive(true);
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
        if (isUseBomb)
            transform.Translate(vertical * speed * myBomb.SlowSpeed * Time.deltaTime, horizontal * speed * myBomb.SlowSpeed * Time.deltaTime, 0);
        else
            transform.Translate(vertical * speed * Time.deltaTime, horizontal * speed * Time.deltaTime, 0);
    }
    void UseAttack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            shootEffect = GameManager.Instance.AudioPlay(musicEffect[1]);
            shootEffect.transform.parent = this.transform;
            isAttack = true;
            BroAnime();
            coroutine = StartCoroutine(Attack());
        }
        if (Input.GetKeyUp(KeyCode.Z) && coroutine != null)
        {
            Destroy(shootEffect.gameObject);
            isAttack = false;
            BroAnime();
        }
    }
    void BroAnime()
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
    }
    IEnumerator Attack()
    {
        while (isAttack)
        {
            for (int i = 0; i < bulletTransform.Length; i++)
            {
                if (i <= GameManager.Instance.playerLevel * 2)
                {
                    Instantiate(bulletPrefab[0], bulletTransform[i].transform.position, Quaternion.identity);
                }
            }
            if (isUseDrone)
            {
                for (int i = 0; i < Drone.Length; i++)
                {
                    GameObject tempObject = Instantiate(bulletPrefab[1], Drone[i].transform.GetChild(0).transform.position, Quaternion.identity);
                    tempObject.GetComponent<Bullet>().canTrackEnemy = true;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    void UseButton()
    {
        if (Input.GetKeyDown(KeyCode.X) && GameManager.Instance.boumbCount > 0 && !isUseBomb && !isUseTimeBarrage)
        {
            GameManager.Instance.thisMapBomb = true;
            myBomb = Instantiate(Bomb, BombPosition.position, Quaternion.identity).GetComponent<Bomb>();
            myBomb.gameObject.transform.parent = this.gameObject.transform;
            isUseBomb = true;
            BombAttack = myBomb.canUseAttack;
            if (GameManager.Instance.enemyManager.isSpanBoss)
                GameManager.Instance.awardType = AwardType.Common;
            GameManager.Instance.AddBottom(-1);
            gameObject.GetComponent<Death>().isInvincible = true;
            Invoke("againUseBomb", useBombTime);
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
            isPlayTimeMusic = false;
            Time.timeScale = SlowSpeed;
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            isUseTimeBarrage = false;
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
    public void AddTimeBarrage(float value)
    {
        invokeTime += value;
        annular.Value = invokeTime / MaxBarrageTime;
        if (invokeTime >= MaxBarrageTime)
        {
            if (!isPlayTimeMusic)
            {
                GameObject temp = GameManager.Instance.AudioPlay(musicEffect[0]);
                Destroy(temp, 2f);
                isPlayTimeMusic = true;
            }
            invokeTime = MaxBarrageTime;
        }
    }
}
