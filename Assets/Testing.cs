using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Testing : MonoBehaviour
{
    [SerializeField] private int m_waitTime;

    private void Awake()
    {
        StartCoroutine(Wait());
        Time.timeScale = 1;
    }

    private IEnumerator Wait()
    {
        float timer = 0;
        yield return new WaitUntil(() =>
        {
            timer += Time.deltaTime;
            return timer > m_waitTime;
        });
        CombatManager.Instance.EndCombat();
    }
}
