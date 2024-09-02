using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlyControl : MonoBehaviour
{
    public float properPitch = 4.3f;//桨叶倾角
    public float minThrottle = 5.5f;//最小油门
    public float maxThrottle = 30f;
    public float weight = 0.715f;

    public float yawScale = 10.0f;
    public float pitchScale = 30.0f;
    public float rollScale = 30.0f;
    public bool autoLevel = true;
    public float maxPitch = 60f;
    public float maxRoll = 15f;
    public float maxControlDis = 400;
    public float warningControlDis = 300;
    public AudioSource audioSource;
    public GameObject leftButton,rightButton,crossHair,headingMeter,vsMeter,hsMeter,distMeter;
    public Text autoLevelText,throttleText,distText,warningText,countDownText;
    public Image warningMask;
    public AnimationCurve curve;
    public Image throttleLine;
    public Button replayButton;

    private Vector2 leftInputDelta, rightInputDelta;
    private Vector3 liftForce,oriPos;
    private Quaternion rotation;
    private float maxInput, distFromTakeoffPad;
    private string[] settingsArray;
    private float autoLevelFloat = 0;
    private float startTimeStamp;//3秒倒计时

    public static bool isTakeoff = false;
    public static float engineSoundPitch;
    public static bool onReplay, onFly,onPause;

    void Start()
    {
        oriPos = transform.position;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        settingsArray = new string[10];
        settingsArray[0] = "droneMass";
        settingsArray[1] = "properPitch";
        settingsArray[2] = "minThrottle";
        settingsArray[3] = "yawSensi";
        settingsArray[4] = "rollSensi";
        settingsArray[5] = "pitchSensi";
        settingsArray[6] = "maxRoll";
        settingsArray[7] = "maxPitch";
        settingsArray[8] = "maxThrottle";
        settingsArray[9] = "autoLevel";
        GetPrefSettings();
        gameObject.GetComponent<Rigidbody>().mass = weight;
        Reinit();
    }

 
    void GetPrefSettings()
    {
        foreach (string key in settingsArray)
        {
            if (PlayerPrefs.HasKey(key))
            {
                var value = PlayerPrefs.GetFloat(key);
                //Debug.Log("key:" + key + ":" + value.ToString());
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
                    case "maxThrottle":
                        maxThrottle = value;break;
                    case "autoLevel":
                        autoLevelFloat = value;break;
                }
            }
        }
        if (autoLevelFloat == 1)
        {
            autoLevel = true;
            autoLevelText.text = "Angle";
        }
        else
        {
            autoLevel = false;
            autoLevelText.text = "ACRO";
        }
    }

    void Update()
    {
        if (!onFly) return; //在回放模式，不进行任何操作
        
        rotation = Quaternion.Euler(properPitch, 0, 0);
        float throttlePercent = 0f;
        leftInputDelta = leftButton.GetComponent<TouchControl>().inputDelta;
        rightInputDelta = rightButton.GetComponent<TouchControl>().inputDelta;
        maxInput = rightButton.GetComponent<TouchControl>().radius;

        Rigidbody rigbody = gameObject.GetComponent<Rigidbody>();
        float throttle = leftInputDelta.y;

        
        //if (throttle != 0) isTakeoff = true;
        //if (isTakeoff) throttlePercent = Mathf.Clamp(((leftInputDelta.y / maxInput) / 2 + 0.5f), 0, 1); else throttlePercent = 0.25f;
        throttlePercent = Mathf.Clamp(((leftInputDelta.y / maxInput) / 2 + 0.5f), 0, 1);
        throttleLine.fillAmount = throttlePercent;
        throttleText.text = (throttlePercent * 100).ToString("F1") + "%";
        audioSource.pitch = engineSoundPitch = throttlePercent * 2;

        var countdown = 5 - (Time.time - startTimeStamp); //起飞倒计时
        var ttext = countdown.ToString("F0");
        countDownText.text = ttext;

        if (ttext == "4") StartCoroutine(FlashIn(headingMeter));
        if (ttext == "3") 
        {
            StartCoroutine(FlashIn(vsMeter)); 
            StartCoroutine(FlashIn(hsMeter));
        }
        if (ttext == "2") StartCoroutine(FlashIn(distMeter));

        if (countdown > 0) return; //倒计时未结束前不会起飞

        replayButton.interactable = true;
        countDownText.enabled = false;
        isTakeoff = true;
        crossHair.SetActive(true);
        
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
            float roll = CurveOutput(rightInputDelta.x) * -rollScale;
            transform.Rotate(Vector3.left, pitch, Space.Self);
            //transform.Rotate(Vector3.forward, -transform.rotation.eulerAngles.z, Space.Self);//消除左右偏航引起的意外横滚。
            transform.Rotate(Vector3.forward, roll, Space.Self);
            transform.Rotate(Vector3.up, yaw, Space.Self);
        }
        
        Vector3 rotatedDirection = rotation * transform.up;
        var liftForce = ((maxThrottle-minThrottle) * throttlePercent) * rotatedDirection; 
        //Debug.Log("liftForce:" + liftForce.magnitude);
        if (isTakeoff) rigbody.AddForce(liftForce,ForceMode.Force);
        //gameObject.GetComponent<Rigidbody>().rotation = Quaternion.identity;

        distFromTakeoffPad = Vector3.Distance(oriPos, transform.position);
        distText.text = "D:" + distFromTakeoffPad.ToString("F0") + "m";

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

    public void AutoLevel()
    {
        if (autoLevel)
        {
            autoLevel = false;
            autoLevelText.text = "ACRO";
            PlayerPrefs.SetFloat("autoLevel", 0f);
        }
        else
        {
            autoLevel = true;
            autoLevelText.text = "Angle";
            PlayerPrefs.SetFloat("autoLevel", 1f);
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

        maxThrottle = Settings.maxThrottle;
        PlayerPrefs.SetFloat("maxThrottle", maxThrottle);

        PlayerPrefs.Save();
    }
    IEnumerator FlashIn(GameObject obj)
    {
        bool isVisible = false;
        for (int i = 0;i< 3; i++)
        {
            if (isVisible)
            {
                obj.SetActive(false);
                isVisible = false;
            }
            else
            {
                obj.SetActive(true);
                isVisible = true;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void OnReplayMsg()
    {
        onReplay = true;
        onFly = false;
        isTakeoff = false;
    }

    public void RestartMsg()
    {
        Reinit();
    }
    void Reinit()
    {
        isTakeoff = false;
        onFly = true;
        startTimeStamp = Time.time;
        countDownText.enabled = true;
        crossHair.SetActive(false);
        headingMeter.SetActive(false);
        vsMeter.SetActive(false);
        hsMeter.SetActive(false);
        distMeter.SetActive(false);
        replayButton.interactable = false;
    }
}
