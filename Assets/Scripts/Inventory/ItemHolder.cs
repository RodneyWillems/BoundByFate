using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour, ISelectHandler
{
    public Item m_item;
    public int m_index;

    private OverworldInventory m_inventory;

    private void Awake()
    {
        m_inventory = FindObjectOfType<OverworldInventory>();
    }

    private void Start()
    {
        // When the button gets instantiated it adds it's own OnClick function
        // It's OnClick is to use the item it's holding
        GetComponent<Button>().onClick.AddListener(delegate { m_inventory.UseItem(m_item); });
    }

    public void OnSelect(BaseEventData eventData)
    {
        m_inventory.UpdateDescription(m_item.description);
    }

}
