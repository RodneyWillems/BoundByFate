using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Commands
{
    Jump,
    Hammer,
    Element
}

public class OverworldPlayer : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject m_characterInFront;
    [SerializeField] private GameObject m_characterInBack;
    private Rigidbody m_firstRB;
    private Rigidbody m_secondRB;

    [SerializeField] private LayerMask m_floor;
    [SerializeField] private Commands m_firstCommand;
    private int m_firstCommandCount = 0;
    [SerializeField] private int m_jumpDrag;

    private PlayerControls m_controls;
    private PlayerInfo m_playerInfo;

    #endregion

    #region Setup
    private void OnEnable()
    {
        m_controls = new();
        m_controls.Overworld.Enable();

        m_controls.Overworld.Move.started += StartMoving;
        m_controls.Overworld.Move.canceled += StopMoving;
        m_controls.Overworld.Swap.performed += Swap;
        m_controls.Overworld.Inventory.performed += OpenInventory;
        m_controls.Overworld.ChangeCommand.performed += ChangeCommand;
        m_controls.Overworld.FirstAction.performed += UseFirstCommand;
    }

    private void OnDisable()
    {
        m_controls.Overworld.Disable();
    }

    void Start()
    {
        m_firstRB = m_characterInFront.GetComponent<Rigidbody>();
        m_secondRB = m_characterInBack.GetComponent<Rigidbody>();
    }

    #endregion

    #region Inputs

    private void StartMoving(InputAction.CallbackContext context)
    {
        // Start walking animation
    }

    private void StopMoving(InputAction.CallbackContext context)
    {
        // Stop walking animation
    }

    private void Swap(InputAction.CallbackContext context)
    {
        GameObject oldFirst = m_characterInFront;
        Vector3 oldFirstPosition = m_characterInFront.transform.position;
        Vector3 oldSecondPosition = m_characterInBack.transform.position;

        m_characterInFront = m_characterInBack;
        m_characterInBack = oldFirst;

        m_characterInFront.transform.position = oldFirstPosition;
        m_characterInBack.transform.position = oldSecondPosition;

        m_firstCommand = Commands.Jump;

        m_firstCommandCount = 0;

        Rigidbody oldFirstRB = m_firstRB;
        Rigidbody oldSecondRB = m_secondRB;

        m_firstRB = oldSecondRB;
        m_secondRB = oldFirstRB;
    }

    private void OpenInventory(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void ChangeCommand(InputAction.CallbackContext context)
    {
        m_firstCommandCount++;
        if (m_firstCommandCount > 2)
        {
            m_firstCommandCount = 0;
        }
        m_firstCommand = (Commands)m_firstCommandCount;
    }

    private void UseFirstCommand(InputAction.CallbackContext context)
    {
        switch (m_firstCommand)
        {
            case Commands.Jump:
                if (Physics.Raycast(m_characterInFront.transform.position + (Vector3.up * 0.5f), Vector3.down, 0.5f, m_floor))
                    m_firstRB.AddForce(Vector3.up * 4, ForceMode.Impulse);
                break;
            case Commands.Hammer:
                print("Hitting your first hammer!");
                break;
            case Commands.Element:
                print("Throwing your first element!");
                break;
            default:
                break;
        }
    }

    private void UseSecondCommand(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(m_characterInBack.transform.position + (Vector3.up * 0.5f), Vector3.down, 0.5f, m_floor))
            m_secondRB.AddForce(Vector3.up * 4, ForceMode.Force);
    }

    private void Move()
    {
        Vector2 moveInput = m_controls.Overworld.Move.ReadValue<Vector2>();
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        m_firstRB.AddForce(moveDirection.normalized * 15, ForceMode.Force);
        m_secondRB.AddForce(moveDirection.normalized * 15, ForceMode.Force);
    }

    #endregion

    void Update()
    {
        if (!Physics.Raycast(m_characterInFront.transform.position + (Vector3.up * 0.5f), Vector3.down, 0.5f, m_floor) || !Physics.Raycast(m_characterInBack.transform.position + (Vector3.up * 0.5f), Vector3.down, 0.5f, m_floor))
        {
            m_firstRB.drag = m_jumpDrag;
            m_secondRB.drag = m_jumpDrag;
        }
        else
        {
            m_firstRB.drag = 1;
            m_secondRB.drag = 1;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }
}
