using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private GenerateMap m_mapGenerator;
    private TurnManager m_turnManager;

    public GenerateMap SetMapGenerator { set { m_mapGenerator = value; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
