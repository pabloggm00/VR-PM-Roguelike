using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : CellObject
{

    public int damage;

    public int health;

    private int m_currentHealth;

    private Animator m_Anim;

    public static event System.Action OnHerir;

    private void Awake()
    {
        GameManager.Instance.turnManager.OnTick += EnemyTurnHappen;
    }

    private void OnDestroy()
    {
        GameManager.Instance.turnManager.OnTick -= EnemyTurnHappen;
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_currentHealth = health;
    }

    private void Start()
    {
        m_Anim = GetComponent<Animator>();
    }

    public override bool PlayerWantsToEnter()
    {

        m_currentHealth -= 1;

        if (m_currentHealth <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }

        return false;
       
    }

    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.mapGenerator;

        var targetCell = board.GetCellData(coord);

        if (targetCell == null
            || !targetCell.canPass
            || targetCell.containedObject != null)
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);
        currentCell.containedObject = null;

        targetCell.containedObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);


        return true;
    }

    void EnemyTurnHappen()
    {
        //Buscamos la posicion actual del player
        var playerCell = GameManager.Instance.playerController.cellPosition;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if ((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
        {
            GameManager.Instance.RestarComida(damage);
            //Animacion de atacar
            m_Anim.SetTrigger("Attack");
            OnHerir?.Invoke();
        }
        else
        {
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    Debug.Log("Me muevo en y");
                    TryMoveInY(yDist); //si no me puedo mover en x (ni atacar) , me muevo en la y
                }
            }
            else 
            {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    bool TryMoveInX(int xDist)
    {

        if (xDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.right);
        }

        return MoveTo(m_Cell + Vector2Int.left);

    }
    bool TryMoveInY(int yDist)
    {

        if (yDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.up);
        }

        return MoveTo(m_Cell + Vector2Int.down);

    }

}
