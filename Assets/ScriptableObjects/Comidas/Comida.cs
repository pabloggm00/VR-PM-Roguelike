using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Comida", menuName = "Crear Comida")]
public class Comida : ScriptableObject
{
    public Sprite sprite;
    public int minPuntosComida;
    public int maxPuntosComida;
}
