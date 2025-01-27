using UnityEngine;

public class ItemUse : MonoBehaviour
{
    public Sprite[] status;
    public float Hurt;
    public float Time;
    Death death;
    Enemy enemy;
    void Awake()
    {
        death = this.transform.parent.gameObject.GetComponent<Death>();
    }
    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = status[GameManager.Instance.playerLevel];
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Item":
                var temp = other.gameObject.GetComponent<Item>();
                GameManager.Instance.EatItem(temp);
                temp.Die();
                break;
            case "Enemy":
                var tempEnemy = other.gameObject.GetComponent<Enemy>();
                var tempEnemy2 = other.gameObject.GetComponent<Death>();
                if (tempEnemy.useBarrage == AttackType.suicideAttack)
                    tempEnemy2.Die();
                if (!death.isInvincible && tempEnemy.canTouch && !GameManager.Instance.ReallyInvincible && death.charatorType != CharatorType.None)
                    death.Die();
                break;
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" && death.charatorType == CharatorType.None && other.gameObject.GetComponent<Death>().canInBomb)
        {
            StartCoroutine(other.gameObject.GetComponent<Death>().BeBombDamage(Hurt, Time));
            if (!GameManager.Instance.ReallyInvincible && !death.isInvincible && !GameManager.Instance.playerScript.isUseBomb)
                StartCoroutine(death.BeBombDamage(Hurt, Time));
            if (GameManager.Instance.playerScript.isUseBomb)
            {
                if (death.hp <= death.totalHp)
                    StartCoroutine(death.BeBombDamage(-Hurt * (GameManager.Instance.droneCount / 2 + 1), Time));
                else
                    StartCoroutine(GameManager.Instance.UninterruptedExp((int)(-Hurt * (GameManager.Instance.droneCount / 2 + 1)),Time));
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.GetComponent<Death>().ExitBomb();
            death.ExitBomb();
        }
    }
}
