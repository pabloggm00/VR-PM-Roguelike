using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private GenerateMap m_Generate;
    private Vector2Int m_CellPosition;
    private Vector2Int newCellTarget;
    private Animator m_Anim;

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


    #region Inputs
    void GetInputMoveUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move(Directions.Up);


            /*SetNewCellTarget();
            newCellTarget.y += 1;
            Move();*/
        }
    }

    void GetInputMoveDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move(Directions.Down);


            /*SetNewCellTarget();
            newCellTarget.y -= 1;
            Move();*/
        }
    }

    void GetInputMoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move(Directions.Left);


            /*SetNewCellTarget();
            newCellTarget.x -= 1;
            Move();*/
        }
    }

    void GetInputMoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move(Directions.Right);


            /*SetNewCellTarget();
            newCellTarget.x += 1;
            Move();*/
        }
    }

    #endregion

    public void Spawn(GenerateMap generateMap, Vector2Int cell)
    {
        m_Generate = generateMap;
        m_CellPosition = cell;

        newCellTarget = cell;

        transform.position = generateMap.CellToWorld(cell);
    }

    public void SetNewCellTarget()
    {
        newCellTarget = m_CellPosition;
    }

    //Es la forma que yo había pensado
    void Move(Directions direction)
    {
        m_Generate.MovePlayer(direction);
        
    }

    //Es la forma del profesor
    void Move()
    {
        GenerateMap.CellData cellData = m_Generate.GetCellData(newCellTarget);

        if (cellData != null && cellData.canPass)
        {
            m_CellPosition = newCellTarget;
            transform.position = m_Generate.CellToWorld(m_CellPosition);
        }
    }


    void PicarAnimation()
    {
        //Aqui se aplica el animar
        m_Anim.SetTrigger("Picar");
    }

    void HerirAnimation()
    {
        m_Anim.SetTrigger("Hurt");
    }
}
