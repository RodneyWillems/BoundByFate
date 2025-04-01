using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    
    private void Start()
    {
        // Play animation
        // Destroy when animation is done
        Destroy(gameObject, 1);
    }

}
