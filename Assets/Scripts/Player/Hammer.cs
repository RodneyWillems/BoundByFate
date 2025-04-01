using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void UseHammer()
    {
        m_animator.Play("Hammer");
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        float time = 0;
        float targetTime = m_animator.runtimeAnimatorController.animationClips[0].length;
        yield return new WaitUntil(() =>
        {
            time += Time.deltaTime;
            return time >= targetTime;
        });

        gameObject.SetActive(false);
    }
}
