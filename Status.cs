using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        //rollIcon.transform.rotation = Quaternion.Euler(0, 0, roll);
        planeFront.transform.rotation = Quaternion.Euler(0, 0, roll);
        //flatSpdText.text = flatSpd.ToString("F2") + "km/h";
        //verSpdText.text = verSpd.ToString("F0") + "m/s";

       
        
    }

    
}
