using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : CellObject
{
    public override void PlayerEntered()
    {
        GameManager.Instance.CompletarRonda();

        
    }
}
