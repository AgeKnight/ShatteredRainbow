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
[System.Serializable]
public struct VivyBarrageTrans
{
    public Transform[] bulletTransform;
}
public class Player : MonoBehaviour
{
    #region "Private"
    GameObject[] tempVyleBarrage = new GameObject[6];
    int VylesIndex = 0;
    float nowspeed;
    float[] annularColor = { 0.8f, 0.8f };
    bool isUseBomb = false;
    bool isUseLazer = false;
    bool isUseDrone = false;
    float invokeTime;
    Coroutine coroutine;
    Bomb myBomb;
    AnnularSlider annular;
    AnnularSlider LilyGather;
    Image Annular;
    GameObject shootEffect;
    GameObject lazerObject;
    GameObject[] LazerPrefab = new GameObject[6];
    public GameObject TimeBarrageTrail;
    int trailnums = 0;
    float GatherTime = 0;
    float LazerTime = 0; //從前的LazerTime
    int SlashUsed = 0;
    Color[] shade = { new Vector4(1, 1, 0.69f), new Vector4(0.69f, 0.97f, 1), new Vector4(1, 0.69f, 0.71f) };
    #endregion
    #region "Public"
    public bool BombAttack = true;
    public int BumbNums;
    public PlayerType playerType;
    public float SlowSpeed;
    public float MaxLazerTime;
    public float speed;
    public float MaxGatherTime;
    public float timeRegain;
    public bool canControlAttack;
    public float useBombTime;
    public float MaxBarrageTime;
    public float AttackTime;
    public GameObject Bomb;
    public GameObject SliderTime;
    public GameObject LilyGatherTime;
    public Transform BombPosition;
    public VivyBarrageTrans[] vivyBarrageTrans;
    public AudioSource[] musicEffect; //0射擊音 1炸彈音

    #endregion
    #region "Hide"
    [HideInInspector]
    public int AllVylesIndex = 0;
    [HideInInspector]
    public GameObject[] VyleBarrage = new GameObject[6];
    [HideInInspector]
    public bool isUseTimeBarrage = false;
    //[HideInInspector]
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
        if (playerType == PlayerType.Lily|| playerType == PlayerType.Frostto)
            LilyGather = LilyGatherTime.GetComponent<AnnularSlider>();
        Annular.color = new Color(1, 1, 1, 0);
        if (playerType == PlayerType.vyles)
        {
            AllVylesIndex = GameManager.Instance.playerLevel + 3;
            VyleCreate();
        }
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

        if (playerType == PlayerType.Lily || playerType == PlayerType.Frostto)
        {
            if (playerType != PlayerType.Frostto || !isUseLazer)
                for (int i = 0; i < Drone.Length; i++)
                {
                    LilyRotateDrone(Drone[i]);
                }
            if (isAttack && !isUseLazer && playerType == PlayerType.Lily || GameManager.Instance.droneCount !=0)
            {
             
                GatherTime += Time.deltaTime;
                LilyGather.Value = GatherTime / MaxGatherTime;
                if (GatherTime >= MaxGatherTime && !canControlAttack)
                {
                    switch(playerType)
                    {case PlayerType.Lily:
                          LilyGathering();
                          break;
                      case PlayerType.Frostto:
                          FrosttoCharged();
                          break;
                    }
                }
            }
            if (isUseLazer)
            {
                LazerTime += Time.deltaTime;
                if (LazerTime >= MaxLazerTime)
                {
                   
                    if(playerType == PlayerType.Frostto)
                        {
                        for (int i = 0; i < GameManager.Instance.droneCount; i++)
                        {
                            if (isUseDrone)
                            {

                                Drone[i].SetActive(true);
                           
                            }
                            i++;

                        }
                    }
                    LilyGather.Value = GatherTime / MaxGatherTime;
                    isUseLazer = false;
                    LazerTime = 0;
                }
            }
        }
      
        if (playerType == PlayerType.vyles)
        {
            for (int i = 0; i < tempVyleBarrage.Length; i++)
            {
                if (tempVyleBarrage[i] != null)
                {
                    VyleBarrage[i].SetActive(false);
                }
            }
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
        if(GameManager.Instance.droneCount>=6)
        {
            GameManager.Instance.FinishAchievement(13);
            GameManager.Instance.CheckUpperLimit();
        }
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

        if (isUseBomb && playerType != PlayerType.vyles)
            speed = nowspeed * myBomb.SlowSpeed;
        else
            speed = nowspeed;
    }
    #region "Attack"
    void UseAttack()
    {
        if ((Input.GetKeyDown(GameManager.Instance.curinput[8]) || Input.GetKeyDown(GameManager.Instance.curinput[9])) && !isAttack)
        {
            isAttack = true;
            if (!GameManager.Instance.AllAttack)
            {
                GameManager.Instance.AllAttack = true;
                GameManager.Instance.Save();
            }
            coroutine = StartCoroutine(Attack());
        }
        if (Input.GetKeyUp(GameManager.Instance.curinput[8]) || Input.GetKeyUp(GameManager.Instance.curinput[9]) && coroutine != null)
        {
            isAttack = false;
            if (DroneGroup.GetComponent<Animator>())
                DroneGroup.GetComponent<Animator>().SetBool("Drone_attacking", false);
                if (GatherTime >= MaxGatherTime)
                {
                    switch(playerType)
                    {
                    case PlayerType.Lily:
                        LilyGathering();
                        break;
                    case PlayerType.Frostto:
                        FrosttoCharged();
                        break;
                    }
                }
                else
                {
                    GatherTime=0;
                }
            
           
                
        }
    }
    IEnumerator Attack()
    {
        while (isAttack)
        {
            GameManager.Instance.AudioPlay(musicEffect[1], true);
            switch (playerType)
            {
                case PlayerType.Prismie:
                    PrismieAttack();
                    break;
                case PlayerType.Lily:
                    LilyAttack();
                    break;
                case PlayerType.Frostto:
                    FrosttoAttack();
                    break;
                case PlayerType.vyles:
                    VylesAttack();
                    break;
                case PlayerType.Lil_Void:
                    PrismieAttack();
                    break;
            }
            yield return new WaitForSeconds(AttackTime);
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
                Instantiate(bulletPrefab[1], Drone[i].transform.GetChild(0).transform.position, Quaternion.identity);
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
        LilyGather.Value =0;
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
            lazerObject.transform.localScale = new Vector3(lazerScaleX, 0.8f, 1);

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

    void FrosttoAttack()
    {
      

        SlashUsed++;
        if(SlashUsed==3)
            bulletPrefab[GameManager.Instance.playerLevel].GetComponent<Bullet>().hurt *= 1.5f;
        if (SlashUsed >= 4)
        {
            SlashUsed = 1;
                bulletPrefab[GameManager.Instance.playerLevel].GetComponent<Bullet>().hurt /= 1.5f;
        }
        /* lazerObject = Instantiate(bulletPrefab[0], BombPosition.position, Quaternion.identity);
         lazerObject.transform.parent = gameObject.transform;
         lazerObject.GetComponent<Animator>().SetInteger("SlashNum",SlashUsed);*/
        bulletPrefab[GameManager.Instance.playerLevel].GetComponent<Animator>().SetInteger("SlashNum", SlashUsed);
       
       




    }
    void FrosttoCharged()
    {
        
        GatherTime = 0;
        LilyGather.Value =0;
        isUseLazer = true;
          for (int i = 0; i < GameManager.Instance.droneCount; i++)
        {
            if (isUseDrone)
            {

                /*  LazerPrefab[i] = Instantiate(bulletPrefab[1], Drone[i].transform.GetChild(0).transform.position, Quaternion.identity);
                  LazerPrefab[i].transform.rotation = Drone[i].transform.GetChild(0).transform.rotation;*/
                var enemy = GameObject.FindWithTag("Enemy");
                GameObject Star = Instantiate(bulletPrefab[4], Drone[i].transform.position, Quaternion.identity);
                if (enemy)
                {
                    Vector3 vectorToTarget = enemy.transform.position - transform.position;
                    float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
                    Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                    Star.transform.rotation = q;
                   
                }
                Drone[i].SetActive(false);
       
                
            }
            i++;

        }
    }
    void VylesAttack()
    {
        isAttack = false;
        AllVylesIndex = GameManager.Instance.playerLevel + 3;
       
        if (VylesIndex <= AllVylesIndex - 1 )//&& VyleBarrage[VylesIndex] == null)
        {
            Debug.Log(VylesIndex);
            tempVyleBarrage[VylesIndex] = Instantiate(bulletPrefab[0], vivyBarrageTrans[GameManager.Instance.playerLevel].bulletTransform[VylesIndex].transform.position, Quaternion.identity);
            tempVyleBarrage[VylesIndex].GetComponent<Bullet>().VyleIndex = VylesIndex;
            VyleBarrage[VylesIndex].SetActive(false);
            VylesIndex += 1;
            if (VylesIndex >= AllVylesIndex)
            {
                VylesIndex = 0;
            }
        }
        if (isUseDrone)
        {
            DroneGroup.GetComponent<Animator>().SetBool("Drone_attacking", true);
            for (int i = 0; i < GameManager.Instance.droneCount; i++)
            {
                Instantiate(bulletPrefab[1], Drone[i].transform.GetChild(0).transform.position, Quaternion.identity);
            }
        }
        if (!canControlAttack)
        {
            isAttack = true;
        }
    }
    public void VyleCreate()
    {
        for (int i = 0; i < VyleBarrage.Length; i++)
        {
            if (VyleBarrage[i] != null)
                Destroy(VyleBarrage[i]);
        }
        for (int i = 0; i < AllVylesIndex; i++)
        {
            VyleBarrage[i] = Instantiate(bulletPrefab[2], vivyBarrageTrans[GameManager.Instance.playerLevel].bulletTransform[i].transform.position, Quaternion.identity);
            VyleBarrage[i].transform.parent = this.gameObject.transform;
            VylesIndex = 0;
           
        }
    }
    #endregion
    #region "Bomb"
    void UseButton()
    {
        if ((Input.GetKeyDown(GameManager.Instance.curinput[10]) || Input.GetKeyDown(GameManager.Instance.curinput[11])) && GameManager.Instance.bombCount > 0 && !isUseBomb && !isUseTimeBarrage)
        {
            GameManager.Instance.thisMapBomb = true;
            GameManager.Instance.thisMapBombCount += 1;
            GameManager.Instance.AudioPlay(musicEffect[2], true);
            Instantiate(GetComponent<Death>().deadEffect, this.transform.position, Quaternion.identity);
            GameManager.Instance.AllBomb = true;
            GameManager.Instance.AllUseBomb+=1;
            GameManager.Instance.FinishAchievement(17);
            if (playerType != PlayerType.vyles)
            {
                myBomb = Instantiate(Bomb, BombPosition.position, Quaternion.identity).GetComponent<Bomb>();
                myBomb.gameObject.transform.parent = gameObject.transform;
                BombAttack = myBomb.canUseAttack;
            }
            else
            {
                for (int i = 0; i < BumbNums; i++)
                {
                    Instantiate(Bomb, BombPosition.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                }
            }
            isUseBomb = true;
            if (GameManager.Instance.enemyManager.isSpanBoss)
                GameManager.Instance.awardType = AwardType.Common;
            GameManager.Instance.AddBomb(-1);
            gameObject.GetComponent<Death>().isInvincible = true;
            Invoke("againUseBomb", useBombTime);
        }
    }
    public void againUseBomb()
    {
        isUseBomb = false;
        BombAttack = true;
        gameObject.GetComponent<Death>().isInvincible = false;
        if (myBomb)
        {
            myBomb.gameObject.GetComponent<Animator>().SetTrigger("Bombover");
            Destroy(myBomb.gameObject, 1);
        }
    }
    #endregion
    #region "子彈時間"
    void UseTimeBarrage()
    {
        if (Input.GetKeyDown(GameManager.Instance.curinput[12]) || Input.GetKeyDown(GameManager.Instance.curinput[13]) && !isUseBomb && invokeTime > 0)
        {
            isUseTimeBarrage = true;
            GameManager.Instance.AllTimeBarrage = true;
            GameManager.Instance.AllUseBT+=1;
            GameManager.Instance.FinishAchievement(18);
            StartCoroutine(Trails());
            this.GetComponent<Animator>().SetBool("AnimBulletTime", isUseTimeBarrage);
            Time.timeScale = SlowSpeed;
            Time.fixedDeltaTime = Time.timeScale * 0.05f;
        }
        if (Input.GetKeyUp(GameManager.Instance.curinput[12]) || Input.GetKeyUp(GameManager.Instance.curinput[13]))
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
