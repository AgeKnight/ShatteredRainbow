using UnityEngine;

public class Bomb : MonoBehaviour
{
    public bool canUseAttack;
    [Range(0f,1f)]public float SlowSpeed; 
    public float Hurt;
    public float Time;

    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.gameObject.tag=="Barrage")
        {
            Destroy(other.gameObject);
        }
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
