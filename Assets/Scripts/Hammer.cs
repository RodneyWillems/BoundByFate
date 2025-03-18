using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField] private bool m_try;

    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_try)
        {
            UseHammer();
            m_try = false;
        }
    }

    public void UseHammer()
    {
        m_animator.Play("Hammer");
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        float time = 0;
        float targetTime = 0;
        AnimationClip[] clips = m_animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "Hammer")
            {
                targetTime = clip.length;
            }
        }
        yield return new WaitUntil(() =>
        {
            time += Time.deltaTime;
            return time >= targetTime;
        });

        gameObject.SetActive(false);
    }
}
