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
    public GameObject foodPrefab;
    public int numComidaASpawnear;
    public List<GameObject> enemiesPrefab;
    public int maxEnemies;
    

    //PRIVATE
    private Tilemap m_Mapa;
    private CellData[,] m_BoardData;
    private Vector2Int m_playerCurrentPositionInCells;
    private Grid m_Grid;
    private List<Vector2Int> m_EmptyCells;
    private List<GameObject> m_EnemiesInGame;

  

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
                    tile = paredes[Random.Range(0, paredes.Length)];
                    m_BoardData[i,j].canPass = false;
                }
                else
                {
                    tile = suelos[Random.Range(0, suelos.Length)];
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
    public void MovePlayer(Directions direction)
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
        if (m_BoardData[m_playerCurrentPositionInCells.x, m_playerCurrentPositionInCells.y].canPass 
            && m_BoardData[m_playerCurrentPositionInCells.x, m_playerCurrentPositionInCells.y] != null) //si la velocidad es mayor a dos salta un indexoutofrange, hay que controlarlo.
        {
            player.transform.position = CellToWorld(m_playerCurrentPositionInCells);

            if (GetCellData(m_playerCurrentPositionInCells).containedObject != null)
            {
                CheckObject();
            }

            GameManager.Instance.turnManager.NextTurn();
        }
        else
        {
            m_playerCurrentPositionInCells = startedPosition;
        }

        MoveAllEnemies();
    }

    void CheckObject()
    {
        if (GetCellData(m_playerCurrentPositionInCells).containedObject.TryGetComponent(out CellObject cellObject))
        {
            cellObject.PlayerEntered();
        }
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (!m_BoardData[cellIndex.x, cellIndex.y].canPass) { return null; }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    //cuantos objetos a spawnear, que por cada uno vaya buscando el escenario
    //y que lo spawnee en una casilla aleatoria, si esa casilla está vacía
    void SpawnComida()
    {

        CellData cell = null;

        for (int i = 0; i < numComidaASpawnear; i++)
        {
            int rndEmptyCasilla = Random.Range(0, m_EmptyCells.Count);
            Vector2Int casilla = m_EmptyCells[rndEmptyCasilla];
            cell = GetCellData(casilla);

            GameObject cellObject = Instantiate(foodPrefab, CellToWorld(casilla), Quaternion.identity);
            cell.containedObject = cellObject.GetComponent<CellObject>();
            m_EmptyCells.RemoveAt(rndEmptyCasilla);
            

        } 
    }

    void SpawnEnemies()
    {
        m_EnemiesInGame = new List<GameObject>();
        CellData cell = null;

        int rndEnemyCount = Random.Range(0, maxEnemies);

        for (int i = 0; i < rndEnemyCount; i++)
        {
            int rndEmptyCasilla = Random.Range(0, m_EmptyCells.Count);
            Vector2Int casilla = m_EmptyCells[rndEmptyCasilla];
            cell = GetCellData(casilla);

            GameObject cellObject = Instantiate(enemiesPrefab[Random.Range(0, enemiesPrefab.Count)], CellToWorld(casilla), Quaternion.identity);
            cell.containedObject = cellObject.GetComponent<CellObject>();

            cell.containedObject.posicion = casilla;

            m_EnemiesInGame.Add(cellObject); //para saber cuántos enemigos tengo in game y poder controlarlos
            m_EmptyCells.RemoveAt(rndEmptyCasilla);


        }
    }

    void MoveAllEnemies()
    {
        foreach (GameObject enemy in m_EnemiesInGame)
        {
            EnemyObject enemyObject = enemy.GetComponent<EnemyObject>();
            enemy.transform.position = CellToWorld(CheckDirectionEnemy(enemyObject.posicion));
        }
    }

    Vector2Int CheckDirectionEnemy(Vector2Int currentEnemyPos)
    {
        Vector2Int direction = new Vector2Int(1,0);


        while (!m_BoardData[direction.x, direction.y].canPass)
        {
            int rndDirection = Random.Range(1, 5);

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
        }

        /*if (m_BoardData[direction.x, direction.y].canPass)
        {
            return direction;
        }*/

        return currentEnemyPos;
    }


}
