using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager 
{

    public event System.Action OnTick;

    //algo que me cuente los turnos 
    int turn = 1;


    //algo que me modifique/cambie los turnos
    public void NextTurn() {  

        turn++; 
        OnTick?.Invoke();

    }



}
