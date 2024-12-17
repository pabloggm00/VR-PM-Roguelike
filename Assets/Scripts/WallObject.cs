using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObject : CellObject
{

    public int hP=200;
    public int costeComida = 3;
    public List<Wall> muros;
    public SpriteRenderer spriteRenderer;
    Sprite spriteNormal;
    Sprite spriteDaniado;

    private void Start()
    {
        GenerateSprite();
    }

    void GenerateSprite()
    {
        int rnd = UnityEngine.Random.Range(0, muros.Count);

        spriteNormal = muros[rnd].spriteNormal;
        spriteDaniado = muros[rnd].spriteDaniado;

        spriteRenderer.sprite = spriteNormal;
    }

    public override bool PlayerWantsToEnter()
    {
        hP--;
        GameManager.Instance.RestarComida(costeComida);


        if (hP % 2 == 0 || hP <= 1)
        {
            spriteRenderer.sprite = spriteDaniado;

        }

        if (hP <= 0)
        {
            //OnDestroyWall?.Invoke(this);
            Destroy(gameObject);
            return true;
        }

        return false;

    }
}
