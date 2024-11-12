using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class GenerateMap : MonoBehaviour
{
    //PUBLIC
    public int ancho;
    public int altura;
    public Tile[] suelos;
    public Tile[] paredes;


    //PRIVATE
    private Tilemap m_Mapa;
    private CellData[,] m_BoardData;

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
        
    }
}
