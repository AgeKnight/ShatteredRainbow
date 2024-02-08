using UnityEngine;
public enum BulletType
{
    Player,
    Enemy
}
public class Bullet : MonoBehaviour
{
    bool cantMove = false;
    public float speed;
    public float hurt;
    //調難度
    public bool canTrackEnemy = false;
    public BulletType bulletType;
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
        var enemy = GameObject.FindWithTag("Enemy");
        if (enemy)
            gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, enemy.transform.position, speed * Time.deltaTime);
        else
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }
    public void Die()
    {
        Destroy(this.gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Death>() && other.gameObject.tag != bulletType.GetType().GetEnumName(bulletType))
        {
            other.gameObject.GetComponent<Death>().Hurt(hurt);
            Die();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Barrier")
            Die();
    }
}
