using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance {  get; private set; }

    public int m_comida = 100;
    private int m_Round = 1;

    public GenerateMap mapGenerator { get;  set; }
    public  TurnManager turnManager { get; private set; }

    public UIDocument UIDoc;
    private Label m_FoodLabel;
    private Label m_RoundLabel;

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
        turnManager = new TurnManager();
        turnManager.OnTick += RestarComida;

        if (mapGenerator == null)
            return;

        //Decirle al boardmanager que genere el terreno
        mapGenerator.MapGenerator();

        //Spawnea el jugador
        mapGenerator.SpawnPlayer();

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_FoodLabel.text = "Comida: " + m_comida;

        m_RoundLabel = UIDoc.rootVisualElement.Q<Label>("RoundLabel");

        m_RoundLabel.text = "Ronda: " + m_Round;
    }

    public void RestarComida() { 
        
        m_comida--;
        m_FoodLabel.text = "Comida: " + m_comida;

        if(m_comida <= 0){
            m_comida = 0;
            Death();
            return;
        }
    }

    public void RestarComida(int dmg)
    {
        m_comida-=dmg;
        m_FoodLabel.text = "Comida: " + m_comida;

        if (m_comida <= 0)
        {
            m_comida = 0;
            Death();
            return;
        }

    }

    public void AddComida(int puntosComida)
    {
        m_comida+= puntosComida;

        if (m_comida >= 100)
            m_comida = 101;
        
    }

    void Death()
    {
        Debug.Log("Ha muerto");
    }

    void SubirRonda()
    {
        m_Round++;
        m_RoundLabel.text = "Ronda: " + m_Round;
    }

    public void DestroyWorld()
    {
       mapGenerator.DestroyWorld();
       InicializarPartida();
       SubirRonda();
    }
}
