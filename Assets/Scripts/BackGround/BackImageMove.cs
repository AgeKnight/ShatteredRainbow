using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackImageMove : MonoBehaviour
{
    bool isChanged = false;
    RectTransform rt;
    RectTransform rtNext;
    public float speed;
    public GameObject BackImages;
    public GameObject nextImage;
    public float[] ImageTransform;
    public Sprite[] Images;
    void Awake()
    {
        rt = BackImages.GetComponent<RectTransform>();
        rtNext = nextImage.GetComponent<RectTransform>();
    }
    void Update()
    {
        if (!GameManager.Instance.enemyManager.canGoNext)
            Move();
    }
    void Move()
    {
        if (rtNext.anchoredPosition3D.y <= ImageTransform[0] && !isChanged)
        {
            isChanged = true;
            InfinityMove();
        }
        if (!isChanged)
        {
            BackImages.transform.Translate(new Vector3(0, -1 * speed * Time.deltaTime), Space.World);
            nextImage.transform.Translate(new Vector3(0, -1 * speed * Time.deltaTime), Space.World);
        }
    }
    void InfinityMove()
    {
        if (GameManager.Instance.GameStage != 3)
        {
            if (GameManager.Instance.enemyManager.isSpanBoss)
            {
                BackImages.GetComponent<Image>().sprite = Images[2];
            }
            else
            {
                BackImages.GetComponent<Image>().sprite = Images[1];
            }
        }
        rt.anchoredPosition3D = new Vector3(0, ImageTransform[1], 0);
        GameObject temp = BackImages;
        BackImages = nextImage;
        nextImage = temp;
        rt = BackImages.GetComponent<RectTransform>();
        rtNext = nextImage.GetComponent<RectTransform>();
        isChanged = false;
    }
}
