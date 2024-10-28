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
    // bool canAttack = true;
    float nowspeed;
    float[] annularColor = { 0.8f, 0.8f };
    bool isUseBomb = false;
    bool isUseLazer = false;
    public bool BombAttack = true;
    bool isUseDrone = false;
    bool[] droneUseLazer = new bool[6];
    float invokeTime;
    Coroutine coroutine;
    Bomb myBomb;
    AnnularSlider annular;
    AnnularSlider LilyGather;
    Image Annular;
    GameObject shootEffect;
    GameObject lazerObject;
    GameObject[] LazerPrefab = new GameObject[6];
    int trailnums = 0;
    float GatherTime = 0;
    float LazerTime = 0;
    Color[] shade = { new Vector4(1, 1, 0.69f), new Vector4(0.69f, 0.97f, 1), new Vector4(1, 0.69f, 0.71f) };
    #endregion
    #region "Public"
    public PlayerType playerType;
    public float maxLazerTime;
    public float SlowSpeed;
    public float speed;
    public float MaxGatherTime;
    public GameObject TimeBarrageTrail;
    public float timeRegain;
    public bool canControlAttack;
    public float useBombTime;
    public float MaxBarrageTime;
    public float AttackTime;
    public GameObject Bomb;
    public GameObject SliderTime;
    public GameObject LilyGatherTime;
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
    //[HideInInspector]
    public Transform[] bulletTransform;
    //[HideInInspector]
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
        if (playerType == PlayerType.Lily)
            LilyGather = LilyGatherTime.GetComponent<AnnularSlider>();
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

        if (playerType == PlayerType.Lily)
        {
            for (int i = 0; i < Drone.Length; i++)
            {
                LilyRotateDrone(Drone[i]);
            }
            if (isAttack && !isUseLazer)
            {
                GatherTime += Time.deltaTime;
                LilyGather.Value = GatherTime / MaxGatherTime;
                if (GatherTime >= MaxGatherTime && !canControlAttack )
                {
                    LilyGathering();
                }
            }
            if (isUseLazer)
            {
               // if (lazerObject)
                 //   lazerObject.transform.position = this.gameObject.transform.position;
                LazerTime += Time.deltaTime;
                if (LazerTime >= maxLazerTime)
                {
                    LilyGather.Value = GatherTime / MaxGatherTime;
                  //  Destroy(lazerObject);
                    isUseLazer = false;
                    LazerTime = 0;
                }
            }
        }
        if (!BombAttack)
        {
            isAttack = false;
        }
        /*if (isUseBomb)
        {
            myBomb.transform.position = transform.position;
        }*/
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
    #region "Attack"
    void UseAttack()
    {
        if (Input.GetKeyDown(GameManager.Instance.curinput[8])|| Input.GetKeyDown(GameManager.Instance.curinput[9]))
        {
            isAttack = true;
            coroutine = StartCoroutine(Attack());
        }
        if (Input.GetKeyUp(GameManager.Instance.curinput[8]) || Input.GetKeyUp(GameManager.Instance.curinput[9]) && coroutine != null)
        {
            isAttack = false;
            if (DroneGroup.GetComponent<Animator>())
                DroneGroup.GetComponent<Animator>().SetBool("Drone_attacking", false);
            if (playerType == PlayerType.Lily)
            {
                /*以子彈取代雷射
                for (int i = 0; i < Drone.Length; i++)
                {
                    droneUseLazer[i] = false;
                    if (LazerPrefab[i])
                    {
                        Destroy(LazerPrefab[i]);
                    }
                }
                */
                if (GatherTime >= MaxGatherTime)
                {
                    LilyGathering();
                }
            }
        }
    }
    IEnumerator Attack()
    {
        while (isAttack)
        {
            GameManager.Instance.AudioPlay(musicEffect[1], true);
            //shootEffect = GameManager.Instance.AudioPlay(musicEffect[1], false);
           // shootEffect.transform.parent = this.transform;
            switch (playerType)
            {
                case PlayerType.Prismie:
                    PrismieAttack();
                    break;
                case PlayerType.Lily:
                    LilyAttack();
                    break;
              /*  case PlayerType.Frostto:
                    break;
                case PlayerType.vyles:
                    break;
                case PlayerType.Lil_Void:
                    break;*/
                default:
                    PrismieAttack();
                    break;
            }
            yield return new WaitForSeconds(AttackTime);
          //  Destroy(shootEffect.gameObject);
        }
    }
    void PrismieAttack()
    {
        for (int i = 0; i < bulletTransform.Length; i++)
        {
            if (i <= GameManager.Instance.playerLevel * 2)
            {
                GameObject bullet = Instantiate(bulletPrefab[0], bulletTransform[i].transform.position, Quaternion.identity);
                if (i != 0) //起始彈道外額外子彈傷害變1/3倍
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
    void LilyGathering()
    {
       
        Quaternion quaternionMiddle = Quaternion.Euler(0, 0, 0);

        float Left = 15;
        float Right = -15;
        float Expansion = 1.5f;
        GatherTime = 0;

        // float lazerScaleY = lazerObject.transform.localScale.y * Expansion;
        // float lazerWidth = lazerObject.GetComponent<LineRenderer>().endWidth * Expansion;
        lazerObject = Instantiate(bulletPrefab[2], BombPosition.position, Quaternion.identity);
        lazerObject.transform.parent = gameObject.transform;
        float lazerScaleX = lazerObject.transform.localScale.x;
        Instantiate(bulletPrefab[3], bulletTransform[0].transform.position, quaternionMiddle);
        GameManager.Instance.AudioPlay(musicEffect[3], true);
        for (int i = 1; i <= GameManager.Instance.playerLevel; i++)
        {
            //雷射_ver1
            /* AnimationCurve curve = new AnimationCurve();
             curve.AddKey(0.0f, lazerWidth);
             lazerObject.GetComponent<LineRenderer>().widthCurve = curve;
             lazerObject.transform.localScale = new Vector3(3, lazerScaleY, 0);
             lazerWidth *= Expansion;
             lazerScaleY *= Expansion;*/
            //雷射_ver2_特效完全綁animator
            lazerScaleX *= Expansion;
            lazerObject.transform.localScale = new Vector3( lazerScaleX,0.8f,1);

            lazerObject.GetComponent<Bullet>().hurt *= 1.25f;

            //左邊
            Quaternion quaternionLeft = Quaternion.Euler(0, 0, quaternionMiddle.z + Left);
            Instantiate(bulletPrefab[3], bulletTransform[0].transform.position, quaternionLeft);
            Left += 30;
            //右邊
            Quaternion quaternionRight = Quaternion.Euler(0, 0, quaternionMiddle.z + Right);
            Instantiate(bulletPrefab[3], bulletTransform[0].transform.position, quaternionRight);
            Right -= 30;
        }
        
        isUseLazer = true;
    }
    void LilyAttack()//普通攻擊
    {
        Instantiate(bulletPrefab[0], bulletTransform[0].transform.position, Quaternion.identity);
        Instantiate(bulletPrefab[0], bulletTransform[1].transform.position, Quaternion.identity);
        /* 以射子彈代替 有那麼點沒趣:(
         if (isUseDrone)
           {
               for (int i = 0; i < GameManager.Instance.droneCount; i++)
               {
                   if (!droneUseLazer[i])
                   {
                       droneUseLazer[i] = true;
                       LazerPrefab[i] = Instantiate(bulletPrefab[1], Drone[i].transform.GetChild(0).transform.position, Quaternion.identity);
                       LazerPrefab[i].transform.parent = Drone[i].transform.GetChild(0).transform;
                       LazerPrefab[i].transform.rotation =Drone[i].transform.GetChild(0).transform.rotation;
                   }
                   //LazerPrefab[i].transform.position = Drone[i].transform.position;
                   //LazerPrefab[i].transform.rotation = Drone[i].transform.rotation;
               }
           }
        */
        for (int i = 0; i < GameManager.Instance.droneCount; i++)
        {
            if (isUseDrone)
            {

                LazerPrefab[i] = Instantiate(bulletPrefab[1], Drone[i].transform.GetChild(0).transform.position, Quaternion.identity);
                LazerPrefab[i].transform.rotation = Drone[i].transform.GetChild(0).transform.rotation;
            }
        }
    }
    public float around_speed = 60f; //公转环绕速度
    void LilyRotateDrone(GameObject drone)
    {
        drone.transform.RotateAround(this.transform.position, Vector3.forward, around_speed * Time.deltaTime);
    }
    #endregion
    #region "Bomb"
    void UseButton()
    {
        if (Input.GetKeyDown(GameManager.Instance.curinput[10])|| Input.GetKeyDown(GameManager.Instance.curinput[11]) && GameManager.Instance.bombCount > 0 && !isUseBomb && !isUseTimeBarrage)
        {
            GameManager.Instance.thisMapBomb = true;
            GameManager.Instance.thisMapBombCount += 1;
            GameManager.Instance.AudioPlay(musicEffect[2], true);
            myBomb = Instantiate(Bomb, BombPosition.position, Quaternion.identity).GetComponent<Bomb>();
            myBomb.gameObject.transform.parent = gameObject.transform;
            isUseBomb = true;
            BombAttack = myBomb.canUseAttack;
            if (GameManager.Instance.enemyManager.isSpanBoss)
                GameManager.Instance.awardType = AwardType.Common;
            GameManager.Instance.AddBomb(-1);
            gameObject.GetComponent<Death>().isInvincible = true;
            switch (playerType)
            {
                case PlayerType.Prismie:
                    Invoke("againUseBomb", useBombTime);
                    break;
               /* case PlayerType.Lily:
                    Invoke("BombLily", 4.5f);
                    break;*/
                default:
                    Invoke("againUseBomb", useBombTime);
                    break;
            }
        }
    }
   /* 目前改成和白色機依樣跑動畫流程，時間到關掉
     void BombLily()
    {
        Destroy(myBomb.gameObject);
        myBomb = Instantiate(Bomb, BombPosition.position, Quaternion.identity).GetComponent<Bomb>();
        myBomb.gameObject.transform.parent = gameObject.transform;
        Invoke("againUseBomb", 4.5f);
    }*/
    void againUseBomb()
    {
        isUseBomb = false;
        BombAttack = true;
        gameObject.GetComponent<Death>().isInvincible = false;
        //s if (playerType == PlayerType.Prismie)
        // {
        //    myBomb.gameObject.GetComponent<Animator>().SetTrigger("Bombover");
        //}
        myBomb.gameObject.GetComponent<Animator>().SetTrigger("Bombover");
        Destroy(myBomb.gameObject, 1);
    }
    #endregion
    #region "子彈時間"
    void UseTimeBarrage()
    {
        if (Input.GetKeyDown(GameManager.Instance.curinput[12]) || Input.GetKeyDown(GameManager.Instance.curinput[13]) && !isUseBomb && invokeTime > 0)
        {
            isUseTimeBarrage = true;
            StartCoroutine(Trails());
            this.GetComponent<Animator>().SetBool("AnimBulletTime", isUseTimeBarrage);
            Time.timeScale = SlowSpeed;
            Time.fixedDeltaTime = Time.timeScale * 0.05f;
        }
        if (Input.GetKeyUp(GameManager.Instance.curinput[12])|| Input.GetKeyUp(GameManager.Instance.curinput[13]))
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
    #endregion
}
