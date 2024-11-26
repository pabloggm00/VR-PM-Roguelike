using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : CellObject
{

    public int damage;

    public override void PlayerEntered()
    {
        Destroy(gameObject);
        GameManager.Instance.RestarComida(damage);
    }

    public void MoveEnemy()
    {

    }
}
