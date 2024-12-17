using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance {  get; private set; }

    public int comidaInicial = 100;
    public int roundInicial = 1;
    public PlayerController playerController;
    public GenerateMap mapGenerator { get;  set; }
    public  TurnManager turnManager { get; private set; }

    private int m_comida;
    private int m_Round = 1;
    public UIDocument UIDoc;
    private Label m_FoodLabel;
    private Label m_RoundLabel;
    private Label m_GameOverText;
    private VisualElement m_GameOverPanel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        InputManager.playerControls.UI.Reiniciar.performed += ResetGame;
        InputManager.playerControls.UI.Reiniciar.canceled += ResetGame;
    }

    private void OnDisable()
    {
        InputManager.playerControls.UI.Reiniciar.performed -= ResetGame;
        InputManager.playerControls.UI.Reiniciar.canceled -= ResetGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_comida = comidaInicial;
        InicializarPartida();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ResetWorld();
            InicializarPartida();
        }
    }

    void InicializarPartida()
    {
  
        //Iniciar Turn manager
        turnManager = new TurnManager();
        turnManager.OnTick += RestarComidaPorMovimiento;

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

        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverPanel.style.display = DisplayStyle.None;

        m_GameOverText = UIDoc.rootVisualElement.Q<Label>("GameOverText");

        InputManager.playerControls.Player.Enable();
        InputManager.playerControls.UI.Disable();

    }

    public void RestarComidaPorMovimiento() { 
        
        m_comida--;
        m_FoodLabel.text = "Comida: " + m_comida;

        //mapGenerator.MoveAllEnemies();

        IsDeath();
    }

    public void RestarComida(int dmg)
    {
        m_comida-=dmg;
        m_FoodLabel.text = "Comida: " + m_comida;

        IsDeath();

    }

    public void ActivarInput()
    {
        InputManager.playerControls.Player.Enable();
    }

    public void DesactivarInput()
    {
        InputManager.playerControls.Player.Disable();
    }

    bool IsDeath()
    {
        if (m_comida <= 0)
        {
            Death();
            m_comida = 0;
            m_FoodLabel.text = "Comida: " + m_comida;
            return true;
        }

        return false;

    }

    public void AddComida(int puntosComida)
    {
        m_comida+= puntosComida;
        if (m_comida >= 100)
            m_comida = 100;

        m_FoodLabel.text = "Comida: " + m_comida;
        
    }

    void Death()
    {
        InputManager.playerControls.Player.Disable();
        InputManager.playerControls.UI.Enable();    
        m_GameOverText.text = "Has muerto \n\n Has sobrevivido " + m_Round + " rondas. \n\n Pulsa R para reinicar la escena.";
        m_GameOverPanel.style.display = DisplayStyle.Flex;
    }

    void SubirRonda()
    {
        m_Round++;
        m_RoundLabel.text = "Ronda: " + m_Round;
    }

    public void CompletarRonda()
    {
       mapGenerator.Clean();
       InicializarPartida();
       SubirRonda();
    }

    public void ResetWorld()
    {
        //reseteamos comida y rondas
        m_comida = comidaInicial;
        m_Round = roundInicial;

        mapGenerator.Clean();
    }

}
