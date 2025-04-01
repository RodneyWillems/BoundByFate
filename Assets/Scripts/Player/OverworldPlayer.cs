using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    [Header("Characters")]
    [SerializeField] private GameObject m_characterInFront;
    [SerializeField] private GameObject m_characterInBack;

    private Rigidbody m_firstRB;
    private Rigidbody m_secondRB;

    [Header("Movement")]
    [SerializeField] private int m_frameLag;
    [SerializeField] private int m_speed;

    private float m_speedControl;
    private List<Vector3> m_followPositions;

    [Header("Commands")]
    [SerializeField] private LayerMask m_floor;
    [SerializeField] private Commands m_firstCommand;
    [SerializeField] private TextMeshProUGUI m_commandText;
    [SerializeField] private GameObject m_firePrefab;
    [SerializeField] private GameObject m_icePrefab;
    [SerializeField] private Transform m_elementSpawnPos;
    [SerializeField] private Hammer m_hammer;

    private int m_firstCommandCount = 0;
    private bool m_elementIsFire = true;
    private Coroutine m_commandRoutine;

    [Header("Inventory")]
    [SerializeField] private GameObject m_inventoryScreen;
    
    private OverworldInventory m_overworldInventory;

    // Misc
    private PlayerControls m_controls;

    #endregion

    #region Setup

    private void OnEnable()
    {
        // Setting inputs to listen to functions
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
        // Getting the Rigidbodies for movement
        m_firstRB = m_characterInFront.GetComponent<Rigidbody>();
        m_secondRB = m_characterInBack.GetComponent<Rigidbody>();

        // Making sure the elementSpawnPos is at the right position
        m_elementSpawnPos.parent = m_characterInFront.transform;
        m_elementSpawnPos.transform.localPosition = new(0, 0.6f, 0.6f);

        // Setting the speedControl and making a new list
        m_speedControl = m_speed;
        m_followPositions = new();

        m_overworldInventory = m_inventoryScreen.GetComponent<OverworldInventory>();
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
        // Saving the positions and the oldFirst to swap them around
        GameObject oldFirst = m_characterInFront;
        Vector3 oldFirstPosition = m_characterInFront.transform.position;
        Vector3 oldSecondPosition = m_characterInBack.transform.position;

        // Swapping around the characters in variables
        m_characterInFront = m_characterInBack;
        m_characterInBack = oldFirst;

        // Swapping the characters' positions to eachother's
        m_characterInFront.transform.position = oldFirstPosition;
        m_characterInBack.transform.position = oldSecondPosition;

        // The command always goes back to jump
        m_firstCommand = Commands.Jump; 
        m_firstCommandCount = 0;

        // Saving the Rigidbodies
        Rigidbody oldFirstRB = m_firstRB;
        Rigidbody oldSecondRB = m_secondRB;

        // Swapping the Rigidbodies
        m_firstRB = oldSecondRB;
        m_secondRB = oldFirstRB;

        // Swapping the element and the spawnPos to the correct element/parent
        m_elementIsFire = !m_elementIsFire;
        m_elementSpawnPos.parent = m_characterInFront.transform;
        m_elementSpawnPos.transform.localPosition = new(0, 0.6f, 0.6f);

        m_hammer.transform.parent = m_characterInFront.transform;

        UpdateCommand();
    }

    private void OpenInventory(InputAction.CallbackContext context)
    {
        // Open Inventory
        m_controls.Overworld.Disable();
        m_inventoryScreen.SetActive(true);
        m_overworldInventory.Open();
        print("Opening Inventory");
    }

    public void CloseInventory()
    {
        // When the inventory closes the player gets control back again and the game continues
        m_controls.Overworld.Enable();
    }

    private void ChangeCommand(InputAction.CallbackContext context)
    {
        // The command only cycles forwards not backwards but when it reaches the end it does cycle back to the start
        m_firstCommandCount++;
        if (m_firstCommandCount > 2)
        {
            m_firstCommandCount = 0;
        }
        m_firstCommand = (Commands)m_firstCommandCount;
        UpdateCommand();
    }

    private void UpdateCommand()
    {
        m_commandText.text = "Current command: " + m_firstCommand.ToString();
    }

    private void UseFirstCommand(InputAction.CallbackContext context)
    {
        // By using a Coroutine I emulate coyote time
        if (m_commandRoutine == null)
        m_commandRoutine = StartCoroutine(CommandRoutine());
    }

    private IEnumerator CommandRoutine()
    {
        float timer = 0;
        timer += Time.deltaTime;
        while (timer < 0.2f) 
        {
            // Depending on which command is currently selected it does that action
            if (Physics.Raycast(m_characterInFront.transform.position + Vector3.up * 0.5f, Vector3.down, 0.6f, m_floor))
            {
                switch (m_firstCommand)
                {
                    case Commands.Jump:
                        // Play animation
                        m_firstRB.AddForce(Vector3.up * 4, ForceMode.Impulse);
                        break;
                    case Commands.Hammer:
                        // Play animation
                        m_hammer.gameObject.SetActive(true);
                        m_hammer.UseHammer();
                        break;
                    case Commands.Element:
                        // Play animation
                        if (m_elementIsFire)
                        {
                            Instantiate(m_firePrefab, m_elementSpawnPos.position, Quaternion.identity);
                        }
                        else
                        {
                            Instantiate(m_icePrefab, m_elementSpawnPos.position, Quaternion.identity);
                        }
                        break;
                    default:
                        break;
                }
                timer = 0.2f;
            }
            yield return null;
        }
        m_commandRoutine = null;
    }

    private void UseSecondCommand(InputAction.CallbackContext context)
    {
        // The character in the back can only jump as to eliminate confusing situations
        if (Physics.Raycast(m_characterInBack.transform.position + Vector3.up * 0.5f, Vector3.down, 0.6f, m_floor))
            m_secondRB.AddForce(Vector3.up * 4, ForceMode.Impulse); 
    }

    private void Move()
    {
        // Depending on the input is where the character in front moves to
        Vector2 moveInput = m_controls.Overworld.Move.ReadValue<Vector2>();
        Vector3 moveDirection = transform.forward * moveInput.y * m_speed + transform.right * moveInput.x * m_speed;

        // Only when the character in the front moves does it save the position of that character once every frame so the character in the back can follow the positions
        // But only saving the x and z position so when the character in front jumps the character in the back doesn't jump with them
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
        // Depending on how far the second character has to be behind the front character it'll start removing positions
        if (m_followPositions.Count >= m_frameLag)
        {
            m_followPositions.RemoveAt(0);
        }

        // Once there are enough positions to follow the front character
        // The back character always looks at the next position it has to follow
        if (m_followPositions.Count > 1)
        {
            m_characterInBack.transform.LookAt(m_followPositions[1] + Vector3.up * m_characterInBack.transform.position.y);
            m_characterInBack.transform.position = m_followPositions[0] + Vector3.up * m_characterInBack.transform.position.y;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Start combat
            print("Combat starting");
        }
    }

    void Update()
    {
        // When the player jumps they slow down slightly 
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
