using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Directions
{
    Up, Down, Left, Right
}

public class GenerateMap : MonoBehaviour
{
    
    //PUBLIC
    public int ancho;
    public int altura;
    public Tile[] suelos;
    public Tile[] paredes;
    public GameObject player;
    public Vector2Int posInicialPlayer;
    public int velocidadPlayerTiles = 1;
    public Exit exitPrefab;

    [Header("Comida")]
    public FoodObject foodPrefab;
    public int numMaxComidaASpawnear;
    public int numMinComidaASpawnear;

    [Header("Muros")]
    public WallObject wallPrefab;
    public int numMaxWallASpawnear;
    public int numMinWallASpawnear;

    [Header("Enemigos")]
    public List<GameObject> enemiesPrefab;
    public int minEnemies;
    public int maxEnemies;

    

    //PRIVATE
    private Tilemap m_Mapa;
    private CellData[,] m_BoardData;
    private Vector2Int m_playerCurrentPositionInCells;
    private Grid m_Grid;
    private List<Vector2Int> m_EmptyCells;
    //private List<EnemyObject> m_EnemiesInGame;


    private void OnEnable()
    {
        //EnemyObject.OnMorir += DestroyEnemyInGame; 
    }

    private void OnDisable()
    {
        //EnemyObject.OnMorir -= DestroyEnemyInGame;
    }

    public class CellData
    {
        public bool canPass;
        public CellObject containedObject;
    }

    private void Awake()
    {
        GameManager.Instance.mapGenerator = this;
    }

    private void Start()
    {
    }

    public void MapGenerator()
    {

        m_EmptyCells = new List<Vector2Int>();
        ancho = Mathf.RoundToInt(ancho);
        altura = Mathf.RoundToInt(altura);
        m_Grid = GetComponent<Grid>();

        m_Mapa = GetComponentInChildren<Tilemap>();
        m_BoardData = new CellData[ancho, altura];

        //Lógica para generar un mapa
        for (int i = 0; i < ancho; i++)
        {
            for (int j = 0; j < altura; j++)
            {

                Tile tile;
                m_BoardData[i,j] = new CellData();

                if (j == 0 || j == altura-1 || i == 0 || i == ancho-1)
                {
                    tile = paredes[UnityEngine.Random.Range(0, paredes.Length)];
                    m_BoardData[i,j].canPass = false;
                }
                else
                {
                    tile = suelos[UnityEngine.Random.Range(0, suelos.Length)];
                    m_BoardData[i, j].canPass = true;
                    m_EmptyCells.Add(new Vector2Int(i,j));
                }

                m_Mapa.SetTile(new Vector3Int(i, j, 0), tile);

            }
        }

        //encuentro un punto en el mapa para el inicio del player
        
        m_playerCurrentPositionInCells = new Vector2Int(posInicialPlayer.x, posInicialPlayer.y);
        m_EmptyCells.Remove(m_playerCurrentPositionInCells);

        //comida
        SpawnExit();
        SpawnWall();
        SpawnComida();
        SpawnEnemies();
    }

    public void SpawnPlayer()
    {
        player.GetComponent<PlayerController>().Spawn(this, new Vector2Int(posInicialPlayer.x, posInicialPlayer.y));
    }

    public Vector3 CellToWorld (Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }


    //Mover player a mi forma
    /*public void MovePlayer(Directions direction)
    {

        Vector2Int startedPosition = m_playerCurrentPositionInCells;

        switch (direction)
        {
            case Directions.Up:
                m_playerCurrentPositionInCells = new Vector2Int(m_playerCurrentPositionInCells.x, m_playerCurrentPositionInCells.y + velocidadPlayerTiles);
                break;
            case Directions.Down:
                m_playerCurrentPositionInCells = new Vector2Int(m_playerCurrentPositionInCells.x, m_playerCurrentPositionInCells.y - velocidadPlayerTiles);
                break;
            case Directions.Left:
                m_playerCurrentPositionInCells = new Vector2Int(m_playerCurrentPositionInCells.x - velocidadPlayerTiles, m_playerCurrentPositionInCells.y);
                break;
            case Directions.Right:
                m_playerCurrentPositionInCells = new Vector2Int(m_playerCurrentPositionInCells.x + velocidadPlayerTiles, m_playerCurrentPositionInCells.y);
                break;
            default:
                break;
        }

        

        //Compruebo si es un muro antes de moverme
        if (m_BoardData[m_playerCurrentPositionInCells.x, m_playerCurrentPositionInCells.y].canPass) //si la velocidad es mayor a dos salta un indexoutofrange, hay que controlarlo.
        {
            //player.transform.position = CellToWorld(m_playerCurrentPositionInCells);
            OnPlayerMove?.Invoke(CellToWorld(m_playerCurrentPositionInCells));
 
            MoveAllEnemies();
            //CheckObject();

            GameManager.Instance.turnManager.NextTurn();
        }
        else
        {
            
            //comprobamos si es un obstáculo
            if (m_BoardData[m_playerCurrentPositionInCells.x, m_playerCurrentPositionInCells.y].containedObject != null)
            {
                if (m_BoardData[m_playerCurrentPositionInCells.x, m_playerCurrentPositionInCells.y].containedObject.TryGetComponent(out WallObject wallObject))
                {
                    wallObject.PlayerWantsToEnter();
                   
                }

                GameManager.Instance.turnManager.NextTurn();
                MoveAllEnemies();
                //CheckObject();
            }

            m_playerCurrentPositionInCells = startedPosition;
        }

        CheckObject();

    }*/

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= ancho || cellIndex.y < 0 || cellIndex.y >= altura)
        {
            return null;
        }
        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.containedObject = obj;
        obj.Init(coord);
    }

    //cuantos objetos a spawnear, que por cada uno vaya buscando el escenario
    //y que lo spawnee en una casilla aleatoria, si esa casilla está vacía
    void SpawnComida()
    {

        CellData cell = null;

        int rndNumComida = UnityEngine.Random.Range(numMinComidaASpawnear, numMaxComidaASpawnear + 1); //obtengo un numero aleatorio de comida

        for (int i = 0; i < rndNumComida; i++)
        {
            int rndEmptyCasilla = UnityEngine.Random.Range(0, m_EmptyCells.Count); //obtengo un indice aleatorio
            Vector2Int casilla = m_EmptyCells[rndEmptyCasilla]; //obtengo la posición de la casilla con el index generado aleatoriamente
            cell = GetCellData(casilla); //recojo esa casilla en una variable

            FoodObject cellObject = Instantiate(foodPrefab, CellToWorld(casilla), Quaternion.identity); //instancio la comida
            cell.containedObject = cellObject; //relleno ese hueco
            m_EmptyCells.RemoveAt(rndEmptyCasilla); //elimino el hueco libre que ahora está ocupado
            

        } 
    }

    void SpawnExit()
    {

        Vector2Int exit = new Vector2Int(ancho - 2, altura - 2);
        m_EmptyCells.Remove(exit); //elimino la casilla libre

        Exit cellObject = Instantiate(exitPrefab, CellToWorld(exit), Quaternion.identity); //instancio la salida

        GetCellData(exit).containedObject = cellObject; //relleno la información de la casilla

    } 

    void SpawnWall()
    {

        CellData cell = null;

        int rndNumWall = UnityEngine.Random.Range(numMinWallASpawnear, numMaxWallASpawnear+1); //obtengo un numero aleatorio de muros

        for (int i = 0; i < rndNumWall; i++)
        {
            int rndEmptyCasilla = UnityEngine.Random.Range(0, m_EmptyCells.Count); //obtengo un indice aleatorio
            Vector2Int casilla = m_EmptyCells[rndEmptyCasilla]; //obtengo la posición de la casilla con el index generado aleatoriamente
            cell = GetCellData(casilla); //recojo esa casilla en una variable

            WallObject cellObject = Instantiate(wallPrefab, CellToWorld(casilla), Quaternion.identity); //instancio el muro
            cell.containedObject = cellObject; //relleno ese hueco

            AddObject(cellObject, casilla);//le digo en qué posición está

            m_EmptyCells.RemoveAt(rndEmptyCasilla); //elimino el hueco libre que ahora está ocupado


        }
    }

    void SpawnEnemies()
    {
        //m_EnemiesInGame = new List<EnemyObject>();
        CellData cell = null;

        int rndEnemyCount = UnityEngine.Random.Range(minEnemies, maxEnemies+1);

        for (int i = 0; i < rndEnemyCount; i++)
        {
            int rndEmptyCasilla = UnityEngine.Random.Range(0, m_EmptyCells.Count); //cojo un numero random de las casillas libres
            Vector2Int casilla = m_EmptyCells[rndEmptyCasilla]; //consigo una casilla libre
            cell = GetCellData(casilla); //cojo esos catos 

            GameObject cellObject = Instantiate(enemiesPrefab[UnityEngine.Random.Range(0, enemiesPrefab.Count)], CellToWorld(casilla), Quaternion.identity); //instancio un enemigo en dicha celda
            
            EnemyObject enemy = cellObject.GetComponent<EnemyObject>();

            cell.containedObject = enemy; //actualizo esa celda y que contiene algo

            AddObject(enemy, casilla);

            //m_EnemiesInGame.Add(enemy); //para saber cuántos enemigos tengo in game y poder controlarlos
            m_EmptyCells.RemoveAt(rndEmptyCasilla); //elimino esa casilla libre


        }
    }

    /*public void MoveAllEnemies()
    {

        CellData cell = null;

        foreach (EnemyObject enemy in m_EnemiesInGame)
        {
            
            cell = GetCellData(enemy.posicion); //recojo la posición en la que está

            Vector2Int toMove = CheckDirectionEnemy(enemy.posicion); //calculo hacia donde me muevo

            cell.containedObject = null; //elimino toda información de la posición antigua

            enemy.transform.position = CellToWorld(toMove); //muevo al enemigo a la nueva posición
            enemy.posicion = toMove; //guardo esa posición nueva

            cell = GetCellData(enemy.posicion); //recojo la celda en la que está ahora
            cell.containedObject = enemy; //relleno la información de la nueva celda

            //aqui tengo que chequear si tengo al player a un bloque de distancia
            IsCloseToPlayer(enemy.posicion);
        }
    }*/

    bool IsCloseToPlayer(Vector2Int enemyPos)
    {
        CellData cell = GetCellData(enemyPos);

        


        return false;
    }


    Vector2Int CheckDirectionEnemy(Vector2Int currentEnemyPos)
    {
        Vector2Int direction = new Vector2Int(1,1);

        int rndDirection = UnityEngine.Random.Range(1, 5);

        switch (rndDirection)
        {
            case 1:
                //arriba
                direction = new Vector2Int(currentEnemyPos.x, currentEnemyPos.y + 1);
                break;
            case 2:
                //abajo
                direction = new Vector2Int(currentEnemyPos.x, currentEnemyPos.y - 1);
                break;
            case 3:
                //derecha
                direction = new Vector2Int(currentEnemyPos.x + 1, currentEnemyPos.y);
                break;
            case 4:
                //izquierda
                direction = new Vector2Int(currentEnemyPos.x - 1, currentEnemyPos.y);
                break;
            default:
                break;
        }

        if (!m_BoardData[direction.x, direction.y].canPass || GetCellData(direction).containedObject != null)
        {
            return currentEnemyPos;
        }


        return direction;
    }

    //para cuando mueran los enemigos
    public void DestroyEnemyInGame(EnemyObject enemy)
    {
        //m_EnemiesInGame.Remove(enemy);
    }




    public void Clean()
    {
        if (m_BoardData == null) return;

        for (int i = 0; i < ancho; ++i)
        {
            for (int j = 0; j < altura; j++) 
            {
                var cellData = m_BoardData[i, j];

                if (cellData.containedObject != null)
                {
                    Destroy(cellData.containedObject.gameObject);
                }
                m_Mapa.SetTile(new Vector3Int(i, j, 0), null);
            }
        }
    }
}
