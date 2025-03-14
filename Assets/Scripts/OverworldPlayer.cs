using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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

    [SerializeField] private int m_frameLag;

    [SerializeField] private GameObject m_characterInFront;
    [SerializeField] private GameObject m_characterInBack;
    private Rigidbody m_firstRB;
    private Rigidbody m_secondRB;

    [SerializeField] private LayerMask m_floor;
    [SerializeField] private Commands m_firstCommand;
    [SerializeField] private int m_speed;
    private int m_firstCommandCount = 0;
    private float m_speedControl;
    private List<Vector3> m_followPositions;

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
        m_controls.Overworld.SecondAction.performed += UseSecondCommand;
    }

    private void OnDisable()
    {
        m_controls.Overworld.Disable();
    }

    void Start()
    {
        m_firstRB = m_characterInFront.GetComponent<Rigidbody>();
        m_secondRB = m_characterInBack.GetComponent<Rigidbody>();

        m_speedControl = m_speed;
        m_followPositions = new();
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
                if (Physics.Raycast(m_characterInFront.transform.position + Vector3.up * 0.5f, Vector3.down, 0.6f, m_floor))
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
        if (Physics.Raycast(m_characterInBack.transform.position + Vector3.up * 0.5f, Vector3.down, 0.6f, m_floor))
            m_secondRB.AddForce(Vector3.up * 4, ForceMode.Impulse); 
    }

    private void Move()
    {
        Vector2 moveInput = m_controls.Overworld.Move.ReadValue<Vector2>();
        Vector3 moveDirection = transform.forward * moveInput.y * m_speed + transform.right * moveInput.x * m_speed;

        if (moveInput.x != 0 || moveInput.y != 0)
        {
            m_firstRB.velocity += moveDirection;
            m_followPositions.Add(new(m_characterInFront.transform.position.x, 0, m_characterInFront.transform.position.z));
        }
        else
        {
            m_firstRB.velocity = new(0, m_firstRB.velocity.y, 0);
        }
        m_characterInFront.transform.LookAt(moveDirection + m_characterInFront.transform.position);

        SpeedControl();
        Follow();
    }

    private void Follow()
    {
        if (m_followPositions.Count >= m_frameLag)
        {
            m_followPositions.RemoveAt(0);
        }

        if (m_followPositions.Count > 1)
        {
            m_characterInBack.transform.LookAt(m_followPositions[m_followPositions.Count - 1] + Vector3.up * m_characterInBack.transform.position.y);
            m_characterInBack.transform.position = m_followPositions[0] + new Vector3(0, m_characterInBack.transform.position.y, 0);
        }
    }

    private void SpeedControl()
    {
        // Limit Velocity when needed
        Vector3 flatVel = new Vector3(m_firstRB.velocity.x, 0, m_firstRB.velocity.z);

        if (flatVel.magnitude > m_speedControl)
        {
            Vector3 limitedVel = flatVel.normalized * m_speedControl;
            m_firstRB.velocity = new Vector3(limitedVel.x, m_firstRB.velocity.y, limitedVel.z);
        }
    }

    #endregion

    void Update()
    {
        if (!Physics.Raycast(m_characterInFront.transform.position + (Vector3.up * 0.5f), Vector3.down, 0.6f, m_floor) || !Physics.Raycast(m_characterInBack.transform.position + (Vector3.up * 0.5f), Vector3.down, 0.6f, m_floor))
        {
            m_speedControl = m_speed * 0.9f;
        }
        else
        {
            m_speedControl = m_speed;
        }

    }

    private void FixedUpdate()
    {
        Move();
    }
}
