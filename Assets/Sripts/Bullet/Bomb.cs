using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int Hurt;
    public float Time;
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
