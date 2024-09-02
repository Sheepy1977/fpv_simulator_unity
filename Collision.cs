using System.Collections;
using System.Collections.Generic;

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

    private void OnTriggerEnter(Collider other)
    {
        var cl = other.gameObject;
        if (cl.name.Contains("Ballon"))
        {
            cl.GetComponent<AudioSource>().Play();
            cl.transform.position = Vector3.zero;
        }
    }
}
