using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region "Private"
    float InvincibleTime = 0;
    #endregion
    #region "Public"
    public float speed;
    #endregion
    #region "Hide"
    [HideInInspector]
    public GameObject bulletPrefab;
    [HideInInspector]
    public bool isInvincible = false;
    [HideInInspector]
    public Transform[] bulletTransform;
    #endregion
    void Start()
    {
        StartCoroutine(Attack());
    }
    void Update()
    {
        if (isInvincible)
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
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            horizontal = 1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            horizontal = -1;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            vertical = -1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            vertical = 1;
        }
        transform.Translate(vertical * speed * Time.deltaTime, horizontal * speed * Time.deltaTime, 0);
    }
    IEnumerator Attack()
    {
        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i <= (GameManager.Instance.playerLevel - 1) * 2)
                {
                    GameObject tempObject = Instantiate(bulletPrefab, bulletTransform[i].transform.position, Quaternion.identity);
                    tempObject.transform.parent = gameObject.transform;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    void Invincible()
    {
        InvincibleTime += Time.deltaTime;
        if (InvincibleTime >= GameManager.Instance.AllInvincibleTime)
        {
            isInvincible = false;
        }
    }
}
