using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [HideInInspector]
    public static InventoryManager Instance;

    public Item[] m_items;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        m_items = new Item[21];
        m_items = Resources.LoadAll<Item>("Items");

    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < m_items.Length; i++)
        {
            if (m_items[i] == item)
            {
                m_items[i].itemAmount++;
            }
        }
    }
}
