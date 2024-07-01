using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public InputField droneMassInput,properPitchInput, liftForceInput, minThrottleInput, yawSensiInput, rollSensiInput, pitchSensiInput, maxRollInput, maxPitchInput;
    public GameObject settingScreen;
    public GameObject drone;

    static public float droneMass,properPitch,liftForce,minThrottle,yawSensi,rollSensi,pitchSensi,maxRoll,maxPitch;


    private void Start()
    {
        readSettings();
        SetInputField();
    }
    public void  GotoSetting()
    {
        settingScreen.SetActive(true);
    }
    
    public void ReturnToMain()
    {
        SetSettings();
        settingScreen.SetActive(false);
    }

    void readSettings()
    {
        var comp = drone.GetComponent<FlyControl>();
        droneMass = comp.weight;
        properPitch = comp.properPitch;
        liftForce = comp.liftScale;
        minThrottle = comp.minThrottle;
        yawSensi = comp.yawScale;
        rollSensi = comp.rollScale;
        pitchSensi = comp.pitchScale;
        maxRoll = comp.maxRoll;
        maxPitch = comp.maxPitch;
    }

    void SetInputField()
    {
        droneMassInput.text = droneMass.ToString();
        properPitchInput.text = properPitch.ToString();
        liftForceInput.text = liftForce.ToString();
        minThrottleInput.text = minThrottle.ToString();
        yawSensiInput.text = yawSensi.ToString();
        rollSensiInput.text = rollSensi.ToString();
        pitchSensiInput.text = pitchSensi.ToString();
        maxRollInput.text = maxRoll.ToString();
        maxPitchInput.text = maxPitch.ToString();
    }

    void SetSettings()
    {
        var comp = drone.GetComponent<FlyControl>();
        droneMass = float.Parse(droneMassInput.text);
        properPitch = float.Parse(properPitchInput.text);
        liftForce = float.Parse(liftForceInput.text);
        minThrottle = float.Parse(minThrottleInput.text);
        yawSensi = float.Parse(yawSensiInput.text);
        rollSensi = float.Parse(rollSensiInput.text);
        pitchSensi = float.Parse(pitchSensiInput.text);
        maxRoll = float.Parse(maxRollInput.text);
        maxPitch = float.Parse(maxPitchInput.text);
        comp.RecvSettings();
    }
}
