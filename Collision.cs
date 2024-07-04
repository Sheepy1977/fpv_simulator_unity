using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Collision : MonoBehaviour
{
    public AudioSource collisionSound;
    void Start()
    {
        
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        collisionSound.volume = Random.Range(0.1f, 1f);
        collisionSound.Play();
    }
}
