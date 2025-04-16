using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum SelectedButton
{
    Solo,
    Duo,
    Items
}

public class CombatPlayer : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject m_player1;
    [SerializeField] private GameObject m_player2;
    [SerializeField] private GameObject m_options1;
    [SerializeField] private GameObject m_options2;
    [SerializeField] private GameObject m_attackOptions1;
    [SerializeField] private GameObject m_attackOptions2;

    private CombatCharacter m_currentPlayer;
    private GameObject m_currentOptions;
    private GameObject m_currentAttackOptions;

    private Func<CombatCharacter> m_selectedAttack;

    private PlayerControls m_controls;

    #endregion

    #region Setup

    private void OnEnable()
    {
        m_controls = new();
        m_controls.Combat.Enable();
        m_controls.Combat.Jump1.performed += Jump1;
        m_controls.Combat.Jump2.performed += Jump2;
    }

    private void OnDisable()
    {
        m_controls.Combat.Disable();
    }

    #endregion

    #region Inputs

    private void Jump1(InputAction.CallbackContext context)
    {
        m_player1.GetComponent<Rigidbody>().AddForce(Vector3.up * 4, ForceMode.Impulse);
    }

    private void Jump2(InputAction.CallbackContext context)
    {
        m_player2.GetComponent<Rigidbody>().AddForce(Vector3.up * 4, ForceMode.Impulse);
    }

    #endregion

    #region Turn

    public void UsePlayer(int player)
    {
        if (player == 1)
        {
            m_currentPlayer = m_player1.GetComponent<CombatCharacter>();
            m_currentOptions = m_options1;
            m_controls.Combat.Jump1.Disable();
            m_controls.Combat.Jump2.Enable();
        }
        else
        {
            m_currentPlayer = m_player2.GetComponent<CombatCharacter>();
            m_currentOptions = m_options2;
            m_controls.Combat.Jump2.Disable();
            m_controls.Combat.Jump1.Enable();
        }
        EnableOptions();
    }

    private void EnableOptions()
    {
        m_currentOptions.SetActive(true);
        m_currentOptions.transform.GetChild(0).GetComponent<Button>().Select();
    }

    public void EndTurn()
    {
        m_currentPlayer = null;
        if (m_currentOptions != null)
        m_currentOptions.SetActive(false);
        m_currentOptions = null;
        CombatManager.Instance.NextOrder();
    }

    #endregion

    #region Attacks

    public void AttackOptions(int player)
    {
        // Open up the Attack options 
        if (player == 1)
        {
            m_currentAttackOptions = m_attackOptions1;
        }
        else
        {
            m_currentAttackOptions = m_attackOptions2;
        }
        m_currentOptions.SetActive(false);
        m_currentAttackOptions.SetActive(true);
        m_currentAttackOptions.transform.GetChild(0).GetComponent<Button>().Select();
    }

    public void JumpAttack()
    {
        // Jump on top of the enemy before landing back in place
        m_currentAttackOptions.SetActive(false);
        print("Jumped!");
        EndTurn();
    }

    public void HammerAttack()
    {
        // Walk to the enemy before playing hammer animation doing damage
        m_currentAttackOptions.SetActive(false);
        print("Hit hammer!");
        EndTurn();
    }

    public void ElementAttack()
    {
        // Play animation and instantiate element at the correct time
        m_currentAttackOptions.SetActive(false);
        print("Threw element");
        EndTurn();
    }

    #endregion
}
