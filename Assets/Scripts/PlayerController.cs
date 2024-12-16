using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public int speed;

    private GenerateMap m_GenerateMap;
    private Vector3 m_MoveTarget;
    private Vector2Int m_CellPosition;
    private Vector2Int newCellTarget;
    private Animator m_Anim;

    bool m_isMoving;
    bool hasMoved;


    private void OnEnable()
    {
        InputManager.playerControls.Player.MoveUp.performed += GetInputMoveUp;
        InputManager.playerControls.Player.MoveUp.canceled += GetInputMoveUp;

        InputManager.playerControls.Player.MoveDown.performed += GetInputMoveDown;
        InputManager.playerControls.Player.MoveDown.canceled += GetInputMoveDown;

        InputManager.playerControls.Player.MoveLeft.performed += GetInputMoveLeft;
        InputManager.playerControls.Player.MoveLeft.canceled += GetInputMoveLeft;

        InputManager.playerControls.Player.MoveRight.performed += GetInputMoveRight;
        InputManager.playerControls.Player.MoveRight.canceled += GetInputMoveRight;

        WallObject.OnPlayerPicar += PicarAnimation;
        EnemyObject.OnHerir += HerirAnimation;
    }

    private void OnDisable()
    {
        InputManager.playerControls.Player.MoveUp.performed -= GetInputMoveUp;
        InputManager.playerControls.Player.MoveUp.canceled -= GetInputMoveUp;

        InputManager.playerControls.Player.MoveDown.performed -= GetInputMoveDown;
        InputManager.playerControls.Player.MoveDown.canceled -= GetInputMoveDown;

        InputManager.playerControls.Player.MoveLeft.performed -= GetInputMoveLeft;
        InputManager.playerControls.Player.MoveLeft.canceled -= GetInputMoveLeft;

        InputManager.playerControls.Player.MoveRight.performed -= GetInputMoveRight;
        InputManager.playerControls.Player.MoveRight.canceled -= GetInputMoveRight;

        WallObject.OnPlayerPicar -= PicarAnimation;
        EnemyObject.OnHerir -= HerirAnimation;
    }

    private void Start()
    {
        m_Anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        
        if (m_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, speed * Time.deltaTime);
            if (transform.position == m_MoveTarget)
            {
                m_isMoving = false;
                var cellData = m_GenerateMap.GetCellData(m_CellPosition); 
                if (cellData.containedObject != null) cellData.containedObject.PlayerEntered();
                ActivarInput();
            }
            return;
        }

        if (hasMoved)
        {
            //comprueba si la nueva posición es pasable, y muevela si lo es.
            GenerateMap.CellData cellData = m_GenerateMap.GetCellData(newCellTarget);
            
            if (cellData != null && cellData.canPass)
            {
                GameManager.Instance.turnManager.NextTurn();

                if (cellData.containedObject == null)
                {
                    MoveTo(newCellTarget, false);
                }
                else if (cellData.containedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget, false);
                    ActivarInput();
                }
                else
                {
                    ActivarInput();
                }
                
            }else if(!cellData.canPass){

                ActivarInput();

            }

            hasMoved = false;

        }

    }


    #region Inputs
    void GetInputMoveUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Move(Directions.Up);

            SetNewCellTarget();
            newCellTarget.y += 1;
            hasMoved = true;
            DesactivarInput();
            //Move();
        }
    }

    void GetInputMoveDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Move(Directions.Down);


            SetNewCellTarget();
            newCellTarget.y -= 1;
            hasMoved = true;
            DesactivarInput();
            //Move();
        }
    }

    void GetInputMoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Move(Directions.Left);
            GetComponentInChildren<SpriteRenderer>().flipX = true;

            SetNewCellTarget();
            newCellTarget.x -= 1;
            hasMoved = true;
            DesactivarInput();
            //Move();
        }
    }

    void GetInputMoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Move(Directions.Right);
            GetComponentInChildren<SpriteRenderer>().flipX = false;

            SetNewCellTarget();
            newCellTarget.x += 1;
            hasMoved = true;
            DesactivarInput();
            //Move();
        }
    }

    #endregion

    public void Spawn(GenerateMap generateMap, Vector2Int cell)
    {
        m_GenerateMap = generateMap;
        m_CellPosition = cell;

        newCellTarget = cell;

        transform.position = generateMap.CellToWorld(cell);
        GetComponentInChildren<SpriteRenderer>().flipX = false;
    }

    public void SetNewCellTarget()
    {
        newCellTarget = m_CellPosition;
    }

    void MoveTo(Vector2Int cell, bool inmediate)//refactoriación del método que sirve para moverse
    {
        m_CellPosition = cell;
        if (inmediate)
        {
            m_isMoving = false;
            transform.position = m_GenerateMap.CellToWorld(m_CellPosition);
        }
        else
        {
            m_isMoving = true;
            m_MoveTarget = m_GenerateMap.CellToWorld(m_CellPosition);
        }
    }


    void ActivarInput()
    {
        InputManager.playerControls.Player.Enable();
    }

    void DesactivarInput()
    {
        InputManager.playerControls.Player.Disable();
    }

    

    #region Animaciones

    void PicarAnimation()
    {
        m_Anim.SetTrigger("Picar");
    }

    void HerirAnimation()
    {
        m_Anim.SetTrigger("Hurt");
    }

    #endregion

}
