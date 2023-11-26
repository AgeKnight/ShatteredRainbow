using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    EXP,
    Life,
    Bomb,
    Drone,
    HalfLife
}
public class Item : MonoBehaviour
{
    #region Public
    public ItemType itemType;
    public float speed=1;
    public int score;
    public int overflowScore;
    #endregion
    #region Hide
    //難度
    [HideInInspector]
    public bool CanAttract = false;
    #endregion
    void Start() 
    {
        GameManager.Instance.ChangeDifficulty(this.gameObject);
    }
    void Update() 
    {
        if(CanAttract)
            Attract();
        else
            Move();     
    }
    void Move()
    {
        transform.Translate(Vector3.down*Time.deltaTime*speed,Space.World);
    }
    void Attract()
    {
        var player = FindObjectOfType<Player>().gameObject;
        gameObject.transform.position = Vector2.MoveTowards(this.gameObject.transform.position,player.transform.position,10*speed*Time.deltaTime);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag=="Barrier")
        {
            Die();
        }
    }
    public void Die()
    {
        Destroy(this.gameObject);
    }
}
