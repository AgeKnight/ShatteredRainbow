using UnityEngine;


public enum ItemType
{
   
    Life,
    Bomb,
    Drone,
    EXP_Small,
    EXP_Mid,
    EXP_Big
}
public class Item : MonoBehaviour
{
    #region Public
    public GameObject usk;
    public ItemType itemType;
    public float speed = 1;
    public int score;
    public int overflowScore;
    public float distance = 2f;
    bool Stopped;
    #endregion
    #region Hide
    //難度
    //[HideInInspector]
    public bool CanAttract = false;
    #endregion
    void Awake() 
    {
        if(GameManager.Instance.ChoicePlayer==4)
        {
           CanAttract = true;
        }
    }
    private void Start()
    {   
        GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)));
    }
    void Update()
    {
        if (CanAttract)
            Attract();
        else
            Move();

        if (GetComponent<Rigidbody2D>().velocity.y > -0.2f && GetComponent<Rigidbody2D>().velocity.y < 0.2f && GetComponent<Rigidbody2D>().velocity.x > -0.2f && GetComponent<Rigidbody2D>().velocity.x < 0.2f)
            Stopped = true;
    }
    void Move()
    {

        if (FindObjectOfType<Player>())
        {
            var player = FindObjectOfType<Player>().gameObject;
            if (Vector2.Distance(player.transform.position, gameObject.transform.position) <= distance)
                gameObject.transform.position = Vector2.MoveTowards(this.gameObject.transform.position, player.transform.position, 10 * speed * Time.deltaTime);
        }
        if (Stopped) //噴出後的速率是否減緩到開始下降的限度
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, -speed); 

    }
   

    void Attract()
    {
        if (FindObjectOfType<Player>())
        {
            var player = FindObjectOfType<Player>().gameObject;
            gameObject.transform.position = Vector2.MoveTowards(this.gameObject.transform.position, player.transform.position, 10 * speed * Time.deltaTime);
        }
        else
            transform.Translate(Vector3.down * Time.deltaTime * speed, Space.World);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Barrier")
            Die();
    }
    public void Die()
    {
        Destroy(this.gameObject);
    }
}
