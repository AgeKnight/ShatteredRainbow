using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackImageMove : MonoBehaviour
{
    bool isChanged = false ;
    RectTransform rt;
    public float speed;
    public GameObject BackImages;
    public GameObject nextImage;
    public float[] ImageTransform;
    public Sprite[] Images;
    void Awake() {
        rt = BackImages.GetComponent<RectTransform>();
    }
    void Update()
    {
        Move();
    }
    void Move()
    {
        BackImages.transform.Translate(0,-1*speed*Time.deltaTime,0);
        nextImage.transform.Translate(0,-1*speed*Time.deltaTime,0);
        if(rt.anchoredPosition3D.y<=ImageTransform[0]&&!isChanged)
        {
            isChanged=true;
            InfinityMove();
        }
    }
    void InfinityMove()
    {
        if(GameManager.Instance.enemyManager.isSpanBoss)
        {
            BackImages.GetComponent<Image>().sprite = Images[2];
        }
        else
        {
            BackImages.GetComponent<Image>().sprite = Images[1];
        }
        rt.anchoredPosition3D = new Vector3 (0, ImageTransform[1], 0);
        GameObject temp = BackImages;
        BackImages = nextImage;
        nextImage = temp;
        rt = BackImages.GetComponent<RectTransform>();
        isChanged=false;
    }
}
