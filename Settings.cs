using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public InputField droneMassInput,properPitchInput, maxThrottleInput, minThrottleInput, yawSensiInput, rollSensiInput, pitchSensiInput, maxRollInput, maxPitchInput;
    public GameObject settingScreen;
    public GameObject drone;

    static public float droneMass,properPitch, maxThrottle, minThrottle,yawSensi,rollSensi,pitchSensi,maxRoll,maxPitch;


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
    public void Restart()
    {
        SendMessage("RestartMsg");
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
    void readSettings()
    {
        var comp = drone.GetComponent<FlyControl>();
        droneMass = comp.weight;
        properPitch = comp.properPitch;
        maxThrottle = comp.maxThrottle;
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
        maxThrottleInput.text = maxThrottle.ToString();
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
        maxThrottle = float.Parse(maxThrottleInput.text);
        minThrottle = float.Parse(minThrottleInput.text);
        yawSensi = float.Parse(yawSensiInput.text);
        rollSensi = float.Parse(rollSensiInput.text);
        pitchSensi = float.Parse(pitchSensiInput.text);
        maxRoll = float.Parse(maxRollInput.text);
        maxPitch = float.Parse(maxPitchInput.text);
        comp.RecvSettings();
    }
}
