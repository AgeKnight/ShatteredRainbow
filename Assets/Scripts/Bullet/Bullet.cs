 using UnityEngine;
public enum BulletType
{
    Player,
    Enemy
}
public enum BulletMoveType
{
    Common,
    Track,
    Bounce
}
public class Bullet : MonoBehaviour
{

    float RotaZ = 0;
    float rainTime = 0;
    float speedtemp;
    bool isTracked = false;
    bool isExit = false;
    float TrackTime;
    [HideInInspector]
    public float AllRainTime;
    [HideInInspector]
    public bool rain = false;
    [HideInInspector]
    public bool canWallDestroy = true;
    [HideInInspector]
    public int VyleIndex;
    public bool canBounceWall = false;
    public float speed;
    public float hurt;
    public float BurnHurt;
    public GameObject hitspark;
    public GameObject BurnSpark;
    public AudioSource Hitsound;
    public bool canAttack = true;
    public int allBounceNum;
    public float MaxTrackTime;
    public BulletType bulletType;
    public BulletMoveType bulletMoveType;
    public float rotatespeed; //跟蹤時的旋轉速度
    private void Start()
    {
        speedtemp = speed;
        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().volume = SaveSystem.LoadGameVoice<SaveVoiceData>().Effect_num * SaveSystem.LoadGameVoice<SaveVoiceData>().All_num;
    }
    void Update()
    {
        switch (bulletMoveType)
        {
            case BulletMoveType.Track:
                Track();
                break;
            case BulletMoveType.Common:
                Move();
                break;
            case BulletMoveType.Bounce:
                Bounce();
                break;
        }
    }
    void Bounce()
    {
        var enemy = GameObject.FindWithTag("Enemy");
        if (!canBounceWall)
        {
            if (enemy)
            {
                if (!isTracked)
                {
                    Vector3 vectorToTarget = enemy.transform.position - transform.position; //抓目標方向
                    float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90; //方位計算 後面的-90拯救了這個部分 沒有他子彈是往反方向飛離 >:(
                    Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);  //面對目標的rotation
                    transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotatespeed); //開始轉向 每次update轉一點
                    transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
                }
                else
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);
                    if (isExit)
                    {
                        TrackTime += Time.deltaTime;
                        if (TrackTime >= MaxTrackTime)
                        {
                            TrackTime = 0;
                            isTracked = false;
                            isExit = false;
                        }
                    }

                }
            }
            else
            {
                transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
            }
        }
        else
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
        }
    }
    protected void Move()
    {
        if (rain)
        {
            speed = Random.Range(0.5f, 2f);
            rainTime += Time.deltaTime;
            if (rainTime >= AllRainTime)
            {
                this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                speed = speedtemp;
                rain = false;
                canWallDestroy = true;
            }
        }
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }
    void Track()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);  //模仿導彈/使子彈轉向敵人的追蹤
        if (bulletType.ToString() == "Player")
        {
            var enemy = GameObject.FindWithTag("Enemy");
            if (enemy)
            {
                Vector3 vectorToTarget = enemy.transform.position - transform.position; //抓目標方向
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90; //方位計算 後面的-90拯救了這個部分 沒有他子彈是往反方向飛離 >:(
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);  //面對目標的rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotatespeed); //開始轉向 每次update轉一點
            }
        }
        else
        {
            var enemy = FindObjectOfType<Player>().gameObject;
            if (enemy)
            {
                Vector3 vectorToTarget = enemy.transform.position - transform.position; //抓目標方向
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90; //方位計算 後面的-90拯救了這個部分 沒有他子彈是往反方向飛離 >:(
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);  //面對目標的rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotatespeed); //開始轉向 每次update轉一點
            }
            if (TrackTime >= MaxTrackTime)
            {
                Die();
            }
            TrackTime += Time.deltaTime;
        }

    }
    public void Die()
    {
        if (GetComponent<Collider2D>())
            GetComponent<Collider2D>().enabled = false;
        if (bulletMoveType != BulletMoveType.Bounce&&gameObject.GetComponent<Animator>())
            gameObject.GetComponent<Animator>().SetTrigger("Vanish");
        if (bulletMoveType == BulletMoveType.Bounce)
        {
            if (GameManager.Instance.playerScript)
            {
                GameManager.Instance.playerScript.VyleBarrage[VyleIndex].gameObject.SetActive(true);
            }
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject, 0.4f);

        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Death>() && other.gameObject.tag != bulletType.GetType().GetEnumName(bulletType) && canAttack)
        {
            other.gameObject.GetComponent<Death>().Hurt(hurt);
            if (hitspark)
                Instantiate(hitspark, this.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 180)));
            if (bulletMoveType != BulletMoveType.Bounce)
                Die();
            else
            {
                if (!canBounceWall)
                {
                    if (!isTracked)
                    {
                        isTracked = true;
                    }
                    if (allBounceNum > 0)
                    {
                        allBounceNum -= 1;
                        transform.eulerAngles = new Vector3(0, 0, RotaZ);
                        RotaZ += 30;
                    }
                    if (allBounceNum <= 0)
                    {
                        other.gameObject.GetComponent<Death>().Hurt(BurnHurt);
                        Instantiate(BurnSpark, this.transform.position, Quaternion.identity);
                        Die();
                    }
                }
            }
        }
        if (other.gameObject.tag == "Barrage" && canBounceWall && other.gameObject.GetComponent<Bullet>().bulletType == BulletType.Enemy)
        {
            other.gameObject.GetComponent<Bullet>().Die();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Barrier" && canWallDestroy)
        {
            if (canBounceWall)
            {
                if (allBounceNum > 0)
                {
                    RotaZ -= Random.Range(100, 170);
                    transform.eulerAngles = new Vector3(0, 0, RotaZ);
                    allBounceNum -= 1;
                }
                if (allBounceNum <= 0)
                {
                    Die();
                }
            }
            else
            {
                Die();
            }
        }
        if (other.gameObject.tag == "Enemy" && bulletMoveType == BulletMoveType.Bounce)
        {
            isExit = true;
        }
    }
}
