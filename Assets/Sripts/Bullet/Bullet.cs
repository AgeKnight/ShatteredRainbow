using UnityEngine;
public enum BulletType
{
    Player,
    Enemy
}
public class Bullet : MonoBehaviour
{
    bool cantMove = false;

    GameObject hit; //擊中效果

    public float speed;
    public float hurt;
    public GameObject hitspark;
    //調難度
    public bool canTrackEnemy = false;
    public BulletType bulletType;
    public float focusDistance; //跟蹤範圍
    public float rotatespeed; //跟蹤時的旋轉速度
    bool isLookingAtObject;


    void Update()
    {
        if(speed==0&&!cantMove)
        {
            cantMove=true;
            Invoke("SpeedRefre",3f);
        }
        if ((GameManager.Instance.canTrack||canTrackEnemy) && bulletType == BulletType.Player)
            Track();
        else
            Move();
    }

    void SpeedRefre()
    {
        speed = 10f;
    }
    protected void Move()
    {
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
        var enemy = GameObject.FindWithTag("Enemy");
        if (enemy)
        {
            Vector3 vectorToTarget = enemy.transform.position - transform.position; //抓目標方向
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg -90; //方位計算 後面的-90拯救了這個部分 沒有他子彈是往反方向飛離 >:(
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);  //面對目標的rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotatespeed); //開始轉向 每次update轉一點
        }
       

    }

    public void Die()
    {
       
        Destroy(hit,0.02f);
        Destroy(this.gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Death>() && other.gameObject.tag != bulletType.GetType().GetEnumName(bulletType))
        {
            other.gameObject.GetComponent<Death>().Hurt(hurt);
        
            hit = Instantiate(hitspark,this.transform.position,Quaternion.Euler(0,0,Random.Range(0,90))) ;

            Die();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Barrier")
            Die();
    }
}
