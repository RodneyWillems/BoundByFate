using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemyState
{
    idle,
    deciding,
    walk
}

public class OverworldEnemies : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject[] m_combatPrefabs;

    [Header("Movement")]
    [SerializeField] private int m_movingSpeed;

    private Animator m_animator;
    private enemyState m_state;
    private Vector3 m_startPosition;
    private Vector3 m_endPosition;
    private Coroutine m_coroutine;

    #endregion

    #region Setup

    void Start()
    {
        m_startPosition = transform.position;
        m_state = enemyState.idle;
        m_animator = GetComponent<Animator>();
    }

    #endregion

    #region States

    private void HandleIdle()
    {
        m_state = enemyState.deciding;
        if (m_coroutine == null)
        { 
            m_coroutine = StartCoroutine(Idle()); 
        }
    }

    private void HandleWalking()
    {
        // When the enemy is moving it stops once it gets close enough to it's destination
        if (Vector3.Distance(transform.position, m_endPosition) > 0.2f)
        {
            m_animator.SetBool("Walking", true);
            transform.LookAt(m_endPosition);
            transform.position = Vector3.MoveTowards(transform.position, m_endPosition, m_movingSpeed * Time.deltaTime);
        }
        else
        {
            m_animator.SetBool("Walking", false);
            m_state = enemyState.idle;
        }
    }

    private IEnumerator Idle()
    {
        // Everytime the enemy reaches it's destination it waits a little bit before it starts moving again
        m_animator.SetBool("Walking", false);
        while (Vector3.Distance(transform.position, m_endPosition) < 0.5f)
        {
            m_endPosition = Random.insideUnitSphere * 3 + m_startPosition;
            m_endPosition.y = m_startPosition.y;
        }
        yield return new WaitForSeconds(2);
        m_state = enemyState.walk;
        m_coroutine = null;
    }

    void Update()
    {
        switch(m_state)
        {
            case enemyState.idle:
                HandleIdle();
                break;
            case enemyState.walk:
                HandleWalking();
                break;
        }
    }

    #endregion
}
