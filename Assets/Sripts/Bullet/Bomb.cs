using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public bool canUseAttack;
    [Range(0f,1f)]public float SlowSpeed; 
    public float Hurt;
    public float Time;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag=="EnemyBarrage")
        {
            Destroy(other.gameObject);
        }
    }
    void OnTriggerStay2D(Collider2D other) 
    {    
        if(other.gameObject.tag=="Enemy"&&other.gameObject.GetComponent<Death>().canInBomb)
        {
            StartCoroutine(other.gameObject.GetComponent<Death>().BeBombDamage(Hurt,Time));
        }
    }
    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag=="Enemy")
            other.GetComponent<Death>().ExitBomb();
    }
}
