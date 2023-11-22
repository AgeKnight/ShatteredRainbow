using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    EXP,
    Life,
    Bomb,
    Drone
}
public enum ExpType
{
    Small,
    Middle,
    Big,
}
public class Item : MonoBehaviour
{
    #region Public
    public ItemType itemType;
    public ExpType expType;
    public float speed=1;
    public float allAdriftTime=1f;
    #endregion
    #region Hide
    //難度
    [HideInInspector]
    public bool CanAttract = false;
    [HideInInspector]
    public int Exp=0;
    #endregion
    #region private
    float adriftTime=0;
    #endregion   
    void Start() 
    {
        switch (expType)
        {
            case ExpType.Small:
                Exp=1;
                break;
            case ExpType.Middle:
                Exp=3;
                break;
            case ExpType.Big:
                Exp=7;
                break;
        }
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
        if(adriftTime>=allAdriftTime)
            transform.Translate(Vector3.down*Time.deltaTime*speed,Space.World);
        else
            adriftTime+=Time.deltaTime;
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
