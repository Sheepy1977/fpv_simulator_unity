using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.Arm;

public class SpawnBallons : MonoBehaviour
{
    //public GameObject ballonPrefab;
    //public Transform spaceCenter;
    public GameObject ballonParent;
    public GameObject Drone;

    void Start()
    {
        ChangeBallonsColor(ballonParent);
    }

    void Update()
    {
        if (FlyControl.onFly == true)
        {
            if (ballonParent.transform.childCount == 0 && Replay.isOnReplay == false)
            {
                Drone.GetComponent<Replay>().Play();
            }
        }
    }

    void ChangeBallonsColor(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            float a = 0.8f;
            Color rc = new(r, g, b, a);
            child.GetComponent<MeshRenderer>().material.color = rc;
        }
    }

}

