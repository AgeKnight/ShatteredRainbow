using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region "Private"
    float InvincibleTime = 0;
    #endregion
    #region "Public"
    public float AllInvincibleTime;
    public float speed;
    #endregion
    #region "Hide"
    [HideInInspector]
    public GameObject Bullet;
    [HideInInspector]
    public Transform bulletTransform;
    [HideInInspector]
    public bool isInvincible = true;
    #endregion
    void Start() 
    {   
        StartCoroutine(Attack());   
    }
    void Update() 
    {
        if(isInvincible)
        {
            Invincible();
        }
    }
    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        int vertical = 0;
        int horizontal = 0;
        if (Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow))
        {
            horizontal = 1;
        }
        if(Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow))
        {
            horizontal = -1;
        }
        if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow))
        {
            vertical = -1;
        }
        if(Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow))
        {
            vertical = 1;
        }
        transform.Translate(vertical*speed * Time.deltaTime, horizontal*speed * Time.deltaTime,0);
    }
    IEnumerator Attack()
    {
        while(true)
        {
            GameObject tempObject = Instantiate(Bullet,bulletTransform.position,Quaternion.identity);
            GameManager.Instance.Playerbullet.Add(tempObject);
            yield return new WaitForSeconds(0.1f);
        }
    }
    void Invincible()
    {
        InvincibleTime+=Time.deltaTime;
        if(InvincibleTime>=AllInvincibleTime)
        {
            isInvincible=false;
        }
    } 
}
