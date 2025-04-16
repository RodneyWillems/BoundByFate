using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private OverworldPlayer m_player;

    private void Awake()
    {
        m_player = FindObjectOfType<OverworldPlayer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_player.DetectCollision(collision);
    }

}
