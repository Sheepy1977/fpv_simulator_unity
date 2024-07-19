using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Status : MonoBehaviour
{
    public GameObject drone, planeSide, planeFront, rollIcon;
    private float  pitch, roll;

    void Update()
    {
        var a = drone.transform.rotation.eulerAngles;
        pitch = a.x;
        roll = a.z;

        planeSide.transform.rotation = Quaternion.Euler(0, 0, pitch);
        planeFront.transform.rotation = Quaternion.Euler(0, 0, roll);
    }
}
