using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject m_selected;

    public void OnSelect(BaseEventData eventData)
    {
        m_selected.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        m_selected.SetActive(false);
    }
}
