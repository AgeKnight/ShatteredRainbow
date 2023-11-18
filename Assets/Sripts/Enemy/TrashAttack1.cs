using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashAttack1 : Enemy
{
    protected override void Move()
    {
        if(FindObjectOfType<Player>())
        {
            var temp = FindObjectOfType<Player>().gameObject;
            transform.position = Vector3.MoveTowards(transform.position, temp.transform.position, Speed * Time.deltaTime);
        }
    }
}
