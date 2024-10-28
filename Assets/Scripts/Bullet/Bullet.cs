using UnityEngine;
public enum BulletType
{
    Player,
    Enemy
}
public class Bullet : MonoBehaviour
{
    public bool cantMove;
    GameObject hit; //擊中效果
    float rainTime = 0;
    public bool rain = false;
    public float speed;
    float speedtemp;
    public float hurt;
    public GameObject hitspark;
    public AudioSource Hitsound;
    //調難度
    public bool canTrackEnemy = false;
    public float MaxTrackTime;
    float TrackTime;
    public BulletType bulletType;
    public float focusDistance; //跟蹤範圍
    public float rotatespeed; //跟蹤時的旋轉速度
    bool isLookingAtObject;

    public float AllRainTime;
    public bool canDestroy = true;
    public bool canAttack = true;

    private void Start()
    {
        speedtemp = speed;
        if(GetComponent<AudioSource>())
       GetComponent<AudioSource>().volume= SaveSystem.LoadGameVoice<SaveVoiceData>().Effect_num * SaveSystem.LoadGameVoice<SaveVoiceData>().All_num;
    }
    void Update()
    {

        /*if (speed == 0 && !cantMove)
        {
            cantMove = true;
            Invoke("SpeedRefre", 3f);
        }*/
        if (!cantMove)
        {
            //   if ((GameManager.Instance.canTrack || canTrackEnemy) && bulletType == BulletType.Player)
            //     Track();
            if ((GameManager.Instance.canTrack || canTrackEnemy))
                Track();
            else
                        Move();
        }
    }
    /*
    void SpeedRefre()
    {
        speed = 10f;
    }
    */
    protected void Move()
    {
        
        if (rain)
        {
            speed = Random.Range(0.5f,2f);
            rainTime += Time.deltaTime;
            if (rainTime >= AllRainTime)
            {
                this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                speed =speedtemp;
                rain= false;
                canDestroy = true;
            }
        }
      
         transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
   
    }
    void Track()
    {
        /*
          var enemy = GameObject.FindWithTag("Enemy");
         if (enemy)
             gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, enemy.transform.position, speed  *Time.deltaTime);
         else
           transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
        */


        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);  //模仿導彈/使子彈轉向敵人的追蹤
        if (bulletType.ToString() =="Player")
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
            if(TrackTime>=MaxTrackTime)
            {
                Die();
            }
            TrackTime += Time.deltaTime;
        }

    }
    
    public void Die()
    {
        // if(GetComponent<Animator>().GetBool("Die"))
        //{
       
        gameObject.GetComponent<Animator>().SetTrigger("Vanish");
        if(GetComponent<Collider2D>())  
        GetComponent<Collider2D>().enabled = false;
        //}
           
            Destroy(this.gameObject, 0.4f);
            
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Death>() && other.gameObject.tag != bulletType.GetType().GetEnumName(bulletType)&&canAttack)
        {
            other.gameObject.GetComponent<Death>().Hurt(hurt);

            hit = Instantiate(hitspark, this.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 180)));
            if (canDestroy)
                Destroy(this.gameObject);

        }
        /*if(other.gameObject.tag == "Barrier" && rain == false)
        {
            canDestroy = true;
        }*/
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Barrier"&&canDestroy)
            Destroy(this.gameObject);
    }
}
