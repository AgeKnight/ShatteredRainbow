using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BulletType
{
    Player,
    Enemy
}
public class Bullet : MonoBehaviour
{
    public float speed;
    public BulletType bulletType;
    void Update()
    {
        Move();
    }
    void Move()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }
    void Die()
    {
        Destroy(this.gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Death>() != null)
        {
            if(other.gameObject.tag!=bulletType.GetType().GetEnumName(bulletType))
            {
                other.gameObject.GetComponent<Death>().Hurt();
                Die();
            }
        }
        else if(other.gameObject.tag=="Barrier")
        {
            Die();
        }
    }
}
