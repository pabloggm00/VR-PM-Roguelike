using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : CellObject
{

    public int puntosComida;

    public override void PlayerEntered()
    {

        GameManager.Instance.AddComida(puntosComida);
        Destroy(gameObject);
    }
}
