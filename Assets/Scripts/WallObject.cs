using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObject : CellObject
{

    public int hP=2;
    public Sprite spriteDaniado;
    public static event System.Action<WallObject> OnDestroyWall;

    public static event System.Action OnPlayerPicar;

    public void PlayerWantsToEnter()
    {
        hP--;
        GameManager.Instance.RestarComida();
        OnPlayerPicar?.Invoke();

        if (hP % 2 == 0 || hP <= 1)
        {
            GetComponent<SpriteRenderer>().sprite = spriteDaniado;

        }

        if (hP <= 0)
        {
            OnDestroyWall?.Invoke(this);
            Destroy(gameObject);
        }

    }
}
