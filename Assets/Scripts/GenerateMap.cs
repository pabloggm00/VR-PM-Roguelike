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

    //PRIVATE
    private Tilemap m_Mapa;
    private CellData[,] m_BoardData;
    private Vector3 m_playerPositionInCells;
    private Vector2Int m_playerCurrentPositionInCells;
    private Grid m_Grid;

    public class CellData
    {
        public bool canPass;
    }

    // Start is called before the first frame update
    void Start()
    {

        ancho = Mathf.RoundToInt(ancho);
        altura = Mathf.RoundToInt(altura);
        m_Mapa = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponent<Grid>();    

        m_BoardData = new CellData[ancho, altura];

        MapGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MapGenerator()
    {
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
                }

                m_Mapa.SetTile(new Vector3Int(i, j, 0), tile);

            }
        }

        //encuentro un punto en el mapa para el inicio del player
        player.GetComponent<PlayerController>().Spawn(this, new Vector2Int(posInicialPlayer.x, posInicialPlayer.y));
        m_playerCurrentPositionInCells = new Vector2Int(posInicialPlayer.x, posInicialPlayer.y);
    }

    public Vector3 CellToWorld (Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public void MovePlayer(Directions direction)
    {

        Vector2Int startedPosition = m_playerCurrentPositionInCells;

        switch (direction)
        {
            case Directions.Up:
                m_playerCurrentPositionInCells = new Vector2Int(m_playerCurrentPositionInCells.x, m_playerCurrentPositionInCells.y + 1);
                break;
            case Directions.Down:
                m_playerCurrentPositionInCells = new Vector2Int(m_playerCurrentPositionInCells.x, m_playerCurrentPositionInCells.y - 1);
                break;
            case Directions.Left:
                m_playerCurrentPositionInCells = new Vector2Int(m_playerCurrentPositionInCells.x - 1, m_playerCurrentPositionInCells.y);
                break;
            case Directions.Right:
                m_playerCurrentPositionInCells = new Vector2Int(m_playerCurrentPositionInCells.x + 1, m_playerCurrentPositionInCells.y);
                break;
            default:
                break;
        }

        //Compruebo si es un muro antes de moverme
        if (m_BoardData[m_playerCurrentPositionInCells.x, m_playerCurrentPositionInCells.y].canPass)
        {
            player.transform.position = CellToWorld(m_playerCurrentPositionInCells);
        }
        else
        {
            m_playerCurrentPositionInCells = startedPosition;
        }
    }


    /*public CellData GetCellData(Vector2Int cellIndex)
    {
        if (!m_BoardData[cellIndex.x, cellIndex.y].canPass) { return null; }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }*/




}
