using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance {  get; private set; }

    public GenerateMap m_mapGenerator { get;  set; }
    public  TurnManager m_turnManager { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InicializarPartida();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InicializarPartida()
    {
        //Iniciar Turn manager
        m_turnManager = new TurnManager();

        if (m_mapGenerator == null)
            return;

        //Decirle al boardmanager que genere el terreno
        m_mapGenerator.MapGenerator();

        //Spawnea el jugador
        m_mapGenerator.SpawnPlayer();
    }
}
