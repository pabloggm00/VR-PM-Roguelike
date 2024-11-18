using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private GenerateMap m_Generate;
    private Vector2Int m_CellPosition;
    private Vector2Int newCellTarget;

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
    }

    void GetInputMoveUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move(Directions.Up);
            /*newCellTarget.y += 1;
            Move();*/
        }
    }

    void GetInputMoveDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move(Directions.Down);
            /*newCellTarget.y -= 1;
            Move();*/
        }
    }

    void GetInputMoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move(Directions.Left);
            /*newCellTarget.x -= 1;
            Move();*/
        }
    }

    void GetInputMoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move(Directions.Right);
            /*newCellTarget.x += 1;
            Move();*/
        }
    }

    public void Spawn(GenerateMap generateMap, Vector2Int cell)
    {
        m_Generate = generateMap;
        m_CellPosition = cell;

        newCellTarget = cell;

        transform.position = generateMap.CellToWorld(cell);
    }

    /*void Move()
    {
        if (m_Generate.GetCellData(newCellTarget) == null) { return; }

        transform.position = m_Generate.CellToWorld(newCellTarget);
    }*/

    void Move(Directions direction)
    {
        m_Generate.MovePlayer(direction);
    }


}
