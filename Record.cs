using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Record : MonoBehaviour
{
    public float maxRecordTime = 30f;
    public float recordFrameRate = 25f;//Ã¿Ãëfps
    public Text recordText;

    public Transform leftControlPoint;
    public Transform rightControlPoint;
    
    public static string jsonText;
    public static int maxFrame;
    public static int fixStep;
    public static object[] jsonArray;
    public static float recordTime;

    private RecordData data = new();
    private float recordStartTime;
    private int frameCounter = 0;
    private Queue jsonData = new Queue();
    private int stepCounter;
    void Start()
    {
        maxFrame = (int)(maxRecordTime * recordFrameRate);
        jsonArray = new object[maxFrame];
        fixStep = (int)((1 / recordFrameRate) / 0.02f);
    }

    void FixedUpdate() 
    {
        var isPlay = FlyControl.isTakeoff;
        stepCounter++;
        if (stepCounter == 1 && isPlay && !Replay.isOnReplay)
        {
           
            if (recordStartTime == 0) recordStartTime = Time.time;
            recordText.text = "Fly time:"+(Time.time - recordStartTime).ToString("F1") + "s";
            recordTime = Time.time - recordStartTime;

            data.SaveTransform(gameObject.transform);
            data.engineSoundPitch = FlyControl.engineSoundPitch ;
            data.leftControlPointPos = leftControlPoint.position;
            data.rightControlPointPos = rightControlPoint.position;
            data.speed = HSpeedShow.flatSpd;
            data.relativeAlt = VSpeedShow.relativeAlt;

            string json = JsonUtility.ToJson(data);
            jsonData.Enqueue(json);
            jsonText += json;
            frameCounter ++;
            //Debug.Log("F" + frameCounter.ToString());
            if (frameCounter > maxFrame)
            {
                jsonData.Dequeue();
            }
            jsonArray = jsonData.ToArray();
        }
        
        if (stepCounter > fixStep) stepCounter = 0;
    }

    public void RestartMsg()
    {
        Reinit();
    }

    void Reinit()
    {
        jsonData = new Queue();
    }
}
