using UnityEngine;
using UnityEngine.UI;

public class Reciprocal : MonoBehaviour
{
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public float allTime;
    void Update()
    {
        gameObject.GetComponent<Text>().text = ((int)allTime).ToString();
        if (allTime <= 0)
        {
            GameManager.Instance.awardType = AwardType.Failed;
            Die();
        }
        else
            allTime -= Time.deltaTime;
    }
    public void Die()
    {
        if (FindObjectOfType<Enemy>() != null&&!isDead)
        {
            isDead = true;
            var tempEnemy = FindObjectOfType<Enemy>();
            tempEnemy.gameObject.GetComponent<Death>().Die();
        }
        gameObject.SetActive(false);
    }
}