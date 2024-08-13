using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerType
{
    Prismie,
    Lily,
    Frostto,
    vyles,
    Lil_Void,
}
public class Player : MonoBehaviour
{
    #region "Private"
    float nowspeed;
    //   float useDroneTime = 0;
    float[] annularColor = { 0.8f, 0.8f };
    bool isUseBomb = false;
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
    #endregion
    #region "Public"
    public PlayerType playerType;
    public float SlowSpeed;
    public float speed;
    public GameObject TimeBarrageTrail;
    public float timeRegain;
    public bool canControlAttack;
    public float useBombTime;
    public float MaxBarrageTime;
    public float AttackTime;
    public GameObject Bomb;
    public GameObject SliderTime;
    public Transform BombPosition;
    public AudioSource[] musicEffect; //0射擊音 1炸彈音

    #endregion
    #region "Hide"
    [HideInInspector]
    public bool isUseTimeBarrage = false;
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
        if (GameManager.Instance.droneCount > 0)
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
        for (int i = 0; i <= GameManager.Instance.droneCount - 1; i++)
        {
            Drone[i].SetActive(true);
        }
    }
    public void AddBro(int value)
    {

        GameManager.Instance.droneCount += value;
        if (GameManager.Instance.droneCount > 0)
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
        for (int i = 0; i <= GameManager.Instance.droneCount - 1; i++)
        {
            Drone[i].SetActive(true);
        }
    }
    void Move()
    {

        int vertical = 0;
        int horizontal = 0;
        if (Input.GetKey(GameManager.Instance.curinput[0]) || Input.GetKey(GameManager.Instance.curinput[1]))
            horizontal = 1;
        if (Input.GetKey(GameManager.Instance.curinput[2]) || Input.GetKey(GameManager.Instance.curinput[3]))
            horizontal = -1;
        if (Input.GetKey(GameManager.Instance.curinput[4]) || Input.GetKey(GameManager.Instance.curinput[5]))
            vertical = -1;
        if (Input.GetKey(GameManager.Instance.curinput[6]) || Input.GetKey(GameManager.Instance.curinput[7]))
            vertical = 1;
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed * vertical, speed * horizontal); //�H���z���D�����ʡA������|���b����ݰʪ����D

        if (isUseBomb)
            speed = nowspeed * myBomb.SlowSpeed;
        else
            speed = nowspeed;


    }

    void UseAttack()
    {
        if (Input.GetKeyDown(GameManager.Instance.curinput[8]))
        {

            isAttack = true;
            //BroAnime();
            coroutine = StartCoroutine(Attack());
        }
        if (Input.GetKeyUp(GameManager.Instance.curinput[8]) && coroutine != null)
        {

            isAttack = false;
            DroneGroup.GetComponent<Animator>().SetBool("Drone_attacking", false);
            //  BroAnime();
        }
    }
    IEnumerator Attack()
    {
        while (isAttack)
        {
            shootEffect = GameManager.Instance.AudioPlay(musicEffect[1], false);
            shootEffect.transform.parent = this.transform;
            if (playerType == PlayerType.Prismie)
            {
                PrismieAttack();
            }
            yield return new WaitForSeconds(AttackTime);
            Destroy(shootEffect.gameObject);
        }
    }
    void PrismieAttack()
    {
        for (int i = 0; i < bulletTransform.Length; i++)
        {
            if (i <= GameManager.Instance.playerLevel * 2)
            {

               GameObject bullet = Instantiate(bulletPrefab[0], bulletTransform[i].transform.position, Quaternion.identity);
                if (i != 0) //起始彈道外額外子彈傷害減少兩倍
                {
                    bullet.GetComponent<Bullet>().hurt = bullet.GetComponent<Bullet>().hurt / 3;
                }
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

    }
    void UseButton()
    {
        if (Input.GetKeyDown(GameManager.Instance.curinput[9]) && GameManager.Instance.bombCount > 0 && !isUseBomb && !isUseTimeBarrage)
        {
            GameManager.Instance.thisMapBomb = true;
            GameManager.Instance.thisMapBombCount += 1;
            GameManager.Instance.AudioPlay(musicEffect[2], true);
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
        if (Input.GetKeyDown(GameManager.Instance.curinput[10]) && !isUseBomb && invokeTime > 0)
        {

            isUseTimeBarrage = true;
            StartCoroutine(Trails());
            this.GetComponent<Animator>().SetBool("AnimBulletTime", isUseTimeBarrage);
            Time.timeScale = SlowSpeed;
            Time.fixedDeltaTime = Time.timeScale * 0.05f;
        }
        if (Input.GetKeyUp(GameManager.Instance.curinput[10]))
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
