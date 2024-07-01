using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlyControl : MonoBehaviour
{
    public float properPitch = 4.3f;//Ω∞“∂«„Ω«
    public float minThrottle = 5.5f;//◊Ó–°”Õ√≈
    public float weight = 0.715f;

    public float liftScale = 1.0f;
    public float yawScale = 1.0f;
    public float pitchScale = 1.0f;
    public float rollScale = 1.0f;
    public bool autoLevel = true;
    public float maxPitch = 60f;
    public float maxRoll = 15f;
    public float maxControlDis = 400;
    public float warningControlDis = 300;

    public GameObject leftButton,rightButton;
    public Text autoLevelText,throttleText,distText,warningText;
    public Image warningMask;
    public AnimationCurve curve;

    public Image postiveLine,negativeLine;

    private Vector2 leftInputDelta, rightInputDelta;
    private Vector3 liftForce,oriPos;
    private Quaternion rotation;
    private float maxInput, distFromTakeoffPad;
    private string[] settingsArray;
    private bool isTakeoff = false;
    void Start()
    {
        rotation = Quaternion.Euler(properPitch,0 , 0);
        oriPos = transform.position;
        if (autoLevel)
        {
            autoLevelText.text = "AutoLevel:On";
        }
        else
        {
            autoLevelText.text = "AutoLevel:Off";
        }
        settingsArray = new string[9];
        settingsArray[0] = "droneMass";
        settingsArray[1] = "properPitch";
        settingsArray[2] = "minThrottle";
        settingsArray[3] = "yawSensi";
        settingsArray[4] = "rollSensi";
        settingsArray[5] = "pitchSensi";
        settingsArray[6] = "maxRoll";
        settingsArray[7] = "maxPitch";
        settingsArray[8] = "liftForce";
        GetPrefSettings();
        gameObject.GetComponent<Rigidbody>().mass = weight;
    }

    void GetPrefSettings()
    {
        foreach (string key in settingsArray)
        {
            if (PlayerPrefs.HasKey(key))
            {
                var value = PlayerPrefs.GetFloat(key);
                Debug.Log("key:" + key + ":" + value.ToString());
                switch (key)
                {
                    case "droneMass":
                        weight = value; break;
                    case "properPitch":
                        properPitch = value; break;
                    case "minThrottle":
                        minThrottle = value;break;
                    case "yawSensi":
                        yawScale = value; break;
                    case "pitchSensi":
                        pitchScale = value; break;
                    case "rollSensi":
                        rollScale = value;break;
                    case "maxRoll":
                        maxRoll = value;break;
                    case "maxPitch":
                        maxPitch = value; break;
                    case "liftForce":
                        liftScale = value;break;
                }
            }
        }
    }

    void Update()
    {
        leftInputDelta = leftButton.GetComponent<TouchControl>().inputDelta;
        rightInputDelta = rightButton.GetComponent<TouchControl>().inputDelta;
        maxInput = rightButton.GetComponent<TouchControl>().radius;

        Rigidbody rigbody = gameObject.GetComponent<Rigidbody>();
        float throttle = leftInputDelta.y;
        if (throttle > 0) isTakeoff = true;
        float throttlePercent = leftInputDelta.y / maxInput;
        if (throttlePercent > 0)
        {
            postiveLine.fillAmount = throttlePercent;
            negativeLine.fillAmount = 0;
        }
        else
        {
            postiveLine.fillAmount = 0;
            negativeLine.fillAmount = -throttlePercent;
        }
        throttleText.text = (throttlePercent * 100).ToString("F1") + "%";

        float yaw = CurveOutput(leftInputDelta.x) * yawScale;
        if (autoLevel)
        {
            float pitch = (rightInputDelta.y / maxInput) * maxPitch;
            float roll = -(rightInputDelta.x / maxInput) * maxRoll;

            transform.Rotate(Vector3.up, yaw, Space.Self);
            transform.rotation = Quaternion.Euler(pitch, transform.rotation.eulerAngles.y, roll);
        }
        else
        {
            float pitch = CurveOutput(rightInputDelta.y ) * pitchScale;
            float roll = CurveOutput(rightInputDelta.x) * rollScale;
            transform.Rotate(Vector3.left, pitch, Space.Self);
            transform.Rotate(Vector3.forward, roll, Space.Self);
            transform.Rotate(Vector3.up, yaw, Space.Self);
        }
        
        Vector3 rotatedDirection = rotation * transform.up;
        liftForce = (liftScale * throttle + minThrottle) * rotatedDirection; 
        //Debug.Log("liftForce:" + liftForce.magnitude);
        if (isTakeoff) rigbody.AddForce(liftForce,ForceMode.Force);
        //gameObject.GetComponent<Rigidbody>().rotation = Quaternion.identity;

        distFromTakeoffPad = Vector3.Distance(oriPos, transform.position);
        distText.text = "Dist:" + distFromTakeoffPad.ToString("F0") + "m";

        dimMask(distFromTakeoffPad);
        if (distFromTakeoffPad > maxControlDis)
        {
            SceneManager.LoadScene(0);
        }
    }

    float  CurveOutput(float input)
    {
        float inputPercent = input / maxInput;
        float output = curve.Evaluate(Mathf.Abs(inputPercent));
        if (inputPercent >= 0)
        {
            return output;
        }
        else
        {
            return -output;
        }

    }


    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void AutoLevel()
    {
        if (autoLevel)
        {
            autoLevel = false;
            autoLevelText.text = "AutoLevel:Off";
        }
        else
        {
            autoLevel = true;
            autoLevelText.text = "AutoLevel:On";
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + liftForce * 5);
    }

    void dimMask(float dis)
    {
        float alpha = Mathf.Clamp((dis - warningControlDis) / (maxControlDis - warningControlDis), 0, 1);
        warningMask.color = new Color(warningMask.color.r, warningMask.color.g, warningMask.color.b, alpha);
        warningText.color = new Color(warningText.color.r, warningText.color.g, warningText.color.b, alpha);
    }
    public void RecvSettings()
    {
        //var comp = GameObject.Find("Settings").GetComponent<Settings>();
        weight = Settings.droneMass;
        PlayerPrefs.SetFloat("droneMass", weight);

        properPitch = Settings.properPitch;
        PlayerPrefs.SetFloat("properPitch", properPitch);

        minThrottle = Settings.minThrottle;
        PlayerPrefs.SetFloat("minThrottle", minThrottle);

        yawScale = Settings.yawSensi;
        PlayerPrefs.SetFloat("yawSensi",yawScale);

        rollScale = Settings.rollSensi;
        PlayerPrefs.SetFloat("rollSensi", rollScale);

        pitchScale = Settings.pitchSensi;
        PlayerPrefs.SetFloat("pitchSensi", pitchScale);

        maxRoll = Settings.maxRoll;
        PlayerPrefs.SetFloat("maxRoll", maxRoll);

        maxPitch = Settings.maxPitch;
        PlayerPrefs.SetFloat("maxPitch", maxPitch);

        liftScale = Settings.liftForce;
        PlayerPrefs.SetFloat("liftForce", liftScale);

        PlayerPrefs.Save();
    }
}
