using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OverworldInventory : MonoBehaviour
{
    #region Variables
    [Header("Menu navigation")]
    [SerializeField] private GameObject m_inventoryScreen;
    [SerializeField] private GameObject m_itemsScreen;
    [SerializeField] private GameObject m_statsScreen;
    [SerializeField] private GameObject m_lastUsedMenu;
    [SerializeField] private Button m_firstSelected;
    [SerializeField] private Button m_statsButton;

    [Header("Items")]
    [SerializeField] private GameObject m_itemSlotPrefab;
    [SerializeField] private Transform m_itemSlotParent;
    [SerializeField] private TextMeshProUGUI m_descriptionText;

    private Item[] m_usableItems;
    private List<GameObject> m_itemSlotList;

    [Header("Stats")]
    [SerializeField] private GameObject m_statsSol;
    [SerializeField] private GameObject m_statsShawn;

    // Misc
    private PlayerControls m_controls;
    private OverworldPlayer m_player;

    #endregion

    #region Setup

    private void OnEnable()
    {
        m_controls = new();
        m_controls.Inventory.Back.performed += Back;
        m_controls.Inventory.Close.performed += Close;
    }

    private void OnDisable()
    {
        m_controls.Inventory.Disable();
    }

    void Start()
    {
        m_itemSlotList = new();

        m_player = FindObjectOfType<OverworldPlayer>();

        m_inventoryScreen.SetActive(false);
    }

    #endregion

    #region Inputs

    public void Open()
    {
        m_inventoryScreen.SetActive(true);
        ReactivateButtons();
        m_firstSelected.Select();
        m_controls.Inventory.Enable();
    }

    private void Back(InputAction.CallbackContext context)
    {
        if (m_lastUsedMenu != null)
        {
            ReactivateButtons();
            CloseItems();
            m_lastUsedMenu.SetActive(false);
            m_lastUsedMenu = null;
            m_firstSelected.Select();
        }
        else
        {
            CloseInventory();
        }
    }

    private void CloseItems()
    {
        if (m_itemSlotList.Count > 0)
        {
            for (int i = m_itemSlotList.Count - 1; i >= 0; i--)
            {
                Destroy(m_itemSlotList[i]);
                m_itemSlotList.Remove(m_itemSlotList[i]);
            }
        }
    }


    private void Close(InputAction.CallbackContext context)
    {
        CloseInventory();
    }


    private void CloseInventory()
    {
        m_controls.Inventory.Disable();
        CloseItems();
        m_player.CloseInventory();
        m_itemsScreen.SetActive(false);
        m_statsScreen.SetActive(false);
        m_inventoryScreen.SetActive(false);
    }

    #endregion

    #region Buttons

    public void SetLastUsedButton(GameObject last)
    {
        m_lastUsedMenu = last;
    }

    public void DeactivateButtons()
    {
        m_firstSelected.interactable = false;
        m_statsButton.interactable = false;
    }

    private void ReactivateButtons()
    {
        m_firstSelected.interactable = true;
        m_statsButton.interactable = true;
    }

    public void ShowItems()
    {
        m_itemsScreen.SetActive(true);

        int slotAmount = 0;
        m_usableItems = new Item[21];

        foreach (Item item in InventoryManager.Instance.m_items)
        {
            if (item.itemAmount > 0)
            {
                m_usableItems[slotAmount] = item;
                slotAmount++;
            }
        }

        for (int i = 0; i < slotAmount; i++)
        {
            GameObject newSlot = Instantiate(m_itemSlotPrefab, m_itemSlotParent);
            m_itemSlotList.Add(newSlot);

            m_itemSlotList[i].GetComponentInChildren<TextMeshProUGUI>().text = m_usableItems[i].itemAmount.ToString();
            m_itemSlotList[i].transform.GetChild(0).GetComponent<Image>().sprite = m_usableItems[i].icon;
            m_itemSlotList[i].GetComponent<ItemHolder>().Item = m_usableItems[i];
        }

        m_itemSlotList[0].GetComponent<Button>().Select();
    }

    public void UpdateDescription(string description)
    {
        m_descriptionText.text = description;
    }

    public void UseItem(Item item)
    {
        item.UseItem();

        for (int i = 0; i < m_itemSlotList.Count; i++)
        {
            m_itemSlotList[i].GetComponentInChildren<TextMeshProUGUI>().text = m_usableItems[i].itemAmount.ToString();
        }
    }

    public void ShowStats()
    {
        m_statsScreen.SetActive(true);
        m_statsSol.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "HP " + PlayerInfo.Instance.GetStat("Sol", "HP", true) + "/" + PlayerInfo.Instance.GetStat("Sol", "HP");
        m_statsSol.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "DP " + PlayerInfo.Instance.GetStat("Sol", "DP", true) + "/" + PlayerInfo.Instance.GetStat("Sol", "DP");
        m_statsSol.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Attack " + PlayerInfo.Instance.GetStat("Sol", "Attack");
        m_statsShawn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "HP " + PlayerInfo.Instance.GetStat("Shawn", "HP", true) + "/" + PlayerInfo.Instance.GetStat("Shawn", "HP");
        m_statsShawn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "DP " + PlayerInfo.Instance.GetStat("Shawn", "DP", true) + "/" + PlayerInfo.Instance.GetStat("Shawn", "DP");
        m_statsShawn.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Attack " + PlayerInfo.Instance.GetStat("Shawn", "Attack");
    }

    #endregion

}
