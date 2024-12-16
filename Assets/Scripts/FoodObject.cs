using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : CellObject
{

    public int puntosComida;
    public List<Comida> comidas;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        GenerateComida();
    }

    void GenerateComida()
    {
        int rnd = Random.Range(0,comidas.Count);

        puntosComida = Random.Range(comidas[rnd].minPuntosComida, comidas[rnd].maxPuntosComida + 1);

        spriteRenderer.sprite = comidas[rnd].sprite;
    }

    public override void PlayerEntered()
    {

        GameManager.Instance.AddComida(puntosComida);
        Destroy(gameObject);
    }
}
