using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class HSpeedShow : MonoBehaviour
{
    public GameObject textParent; //竖向的数字父物体
    public GameObject textPrefab;
    public GameObject drone;
    public GameObject mainText; //主显示数字
    public float lineScale = 27f;
    public int meterRange = 40;

    private Vector3 lastPosition;
    private float flatSpd,realSpeed;
    private void Start()
    {
        lastPosition = drone.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        mainText.GetComponent<Text>().text = "\u25C1" + flatSpd.ToString("F0") + "km/h";
        GenText(MyFuntions.GenMeter(flatSpd, meterRange, textParent.transform,lowLimitStr:"0"));
        foreach (Transform tr in textParent.transform) //移动对象到正确的位置
        {
            var number = int.Parse(tr.name[3..]);
            float deltaY = (number - flatSpd) * lineScale;
            var mainTextPos = mainText.transform.position;
            tr.position = new Vector3(mainTextPos.x-300, (mainTextPos.y + deltaY), mainTextPos.z); 
        }
    }
    private void FixedUpdate()
    {
        var lastFlatPositon = new Vector2(lastPosition.x, lastPosition.z);
        var position = drone.transform.position;
        var droneFlatPosition = new Vector2(position.x, position.z);
        var flatDis = Vector2.Distance(lastFlatPositon, droneFlatPosition);

        //realSpeed = drone.GetComponent<Rigidbody>().velocity.magnitude;
        //realSpeed = (Vector3.Distance(lastPosition, position) / Time.deltaTime) * 3.6f;
        flatSpd = (flatDis / Time.deltaTime) * 3.6f;
        lastPosition = position;
    }

    void GenText(List<int> list)
    {
        string str;
        string showStr;
        foreach (int i in list)
        {
            var newText = Instantiate(textPrefab, new Vector3(0, 0, 0), Quaternion.identity, textParent.transform);
            showStr = i.ToString();
            //if (i < 10) showStr = "00" + showStr;
            //if (i > 10 && i < 100) showStr = "0" + showStr;
            if (i % 5 == 0) str = showStr + "-"; else str = "-";
            newText.GetComponent<Text>().text = str;
            newText.name = "spd" + i.ToString();
        }
    }
}
