using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSpriteMove : MonoBehaviour
{
    bool isChanged = false;
    Transform rt;
    Transform rtNext;
    public float speed;
    public GameObject BackImages;
    public GameObject nextImage;
    public float[] ImageTransform;
    public Sprite[] Images;
    void Awake()
    {
        rt = BackImages.GetComponent<Transform>();
        rtNext = nextImage.GetComponent<Transform>();
    }
    void Update()
    {
        if(!GameManager.Instance.enemyManager.canGoNext)
            Move();
    }
    void Move()
    {
        if (rtNext.position.y <= ImageTransform[0] && !isChanged)
        {
            isChanged = true;
            InfinityMove();
        }
        if(!isChanged)
        {
            BackImages.transform.Translate(new Vector3(0,-1*speed*Time.deltaTime),Space.World);
            nextImage.transform.Translate(new Vector3(0,-1*speed*Time.deltaTime),Space.World);
        }   
    }
    void InfinityMove()
    {
        if (GameManager.Instance.enemyManager.isSpanBoss)
        {
            BackImages.GetComponent<SpriteRenderer>().sprite = Images[2];
        }
        else
        {
            BackImages.GetComponent<SpriteRenderer>().sprite = Images[1];
        }
        rt.position = new Vector3(0, ImageTransform[1], 0);
        GameObject temp = BackImages;
        BackImages = nextImage;
        nextImage = temp;
        rt = BackImages.GetComponent<Transform>();
        rtNext = nextImage.GetComponent<Transform>();
        isChanged = false;
    }
}
