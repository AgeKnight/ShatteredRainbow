using UnityEngine;

public enum ItemType
{
    EXP,
    Life,
    Bomb,
    Drone,
}
public class Item : MonoBehaviour
{
    #region Public
    public ItemType itemType;
    public float speed = 1;
    public int score;
    public int overflowScore;
    float distance = 2f;
    #endregion
    #region Hide
    //難度
    [HideInInspector]
    public bool CanAttract = false;
    #endregion
    void Update()
    {
        if (CanAttract)
            Attract();
        else
            Move();
    }
    void Move()
    {
        if (FindObjectOfType<Player>())
        {
            var player = FindObjectOfType<Player>().gameObject;
            if (Vector2.Distance(player.transform.position, gameObject.transform.position) <= distance)
                gameObject.transform.position = Vector2.MoveTowards(this.gameObject.transform.position, player.transform.position, 10 * speed * Time.deltaTime);
            else
                transform.Translate(Vector3.down * Time.deltaTime * speed, Space.World);
        }
        else
            transform.Translate(Vector3.down * Time.deltaTime * speed, Space.World);
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
