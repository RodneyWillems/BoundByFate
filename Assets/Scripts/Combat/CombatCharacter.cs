using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CombatCharacter : MonoBehaviour
{
    [SerializeField] private bool m_isSol;
    [SerializeField] private GameObject m_AttackOptions;

    private GameObject m_openedMenu;

    private PlayerControls m_controls;

    private void OnEnable()
    {
        m_controls = new();
        m_controls.Combat.Enable();
        m_controls.Combat.Back.performed += Back;
    }

    public void Enable()
    {
        m_controls.Combat.Enable();
    }

    public void OpenedMenu(GameObject openedMenu)
    {
        m_openedMenu = openedMenu;
    }

    private void Back(InputAction.CallbackContext context)
    {
        print("Pressed back wtf?");
    }

    public void Attack()
    {
        m_AttackOptions.SetActive(false);
    }

    public void DoDamage()
    {

    }

}
