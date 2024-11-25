using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance {  get; private set; }

    public int m_comida = 100;

    public GenerateMap mapGenerator { get;  set; }
    public  TurnManager turnManager { get; private set; }

    public UIDocument UIDoc;
    private Label m_FoodLabel;

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
    }

    public void RestarComida() { 
        
        if(m_comida <= 0){
            m_comida = 0;
            Death();
            return;
        }
        
        m_comida--;
        m_FoodLabel.text = "Comida: " + m_comida;
    }

    void Death()
    {
        Debug.Log("Ha muerto");
    }
}
