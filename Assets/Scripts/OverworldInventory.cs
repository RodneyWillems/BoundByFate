using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OverworldInventory : MonoBehaviour
{
    [SerializeField] private OverworldPlayer m_player;
    [SerializeField] private GameObject m_lastUsedMenu;

    private PlayerControls m_controls;

    private void OnEnable()
    {
        m_controls = new();
        m_controls.Inventory.Enable();
    }

    private void OnDisable()
    {
        m_controls.Inventory.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_controls.Inventory.Back.performed += Back;
        m_controls.Inventory.Close.performed += Close;

        m_player = FindObjectOfType<OverworldPlayer>();
    }

    private void Back(InputAction.CallbackContext context)
    {
        if (m_lastUsedMenu != null)
        {
            m_lastUsedMenu.SetActive(false);
            m_lastUsedMenu = null;
        }
        else
        {
            CloseInventory();
        }
    }

    private void Close(InputAction.CallbackContext context)
    {
        CloseInventory();
    }

    public void SetLastUsedButton(GameObject last)
    {
        m_lastUsedMenu = last;
    }

    private void CloseInventory()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
