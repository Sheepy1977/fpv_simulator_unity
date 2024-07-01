using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollPitchShow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject crossIcon;
    public Text pitchT1,pitchT2;
    public GameObject textParent;
    public GameObject textPrefab;
    public GameObject drone;
    public float lineSpace = 40f;
    public int meterRange = 180;
    public int spaceCount = 30;

    private string spaceString,lineString;
    void Start()
    {
        for (int i = 0;i < spaceCount; i++)
        {
            spaceString += "\u00A0";
            lineString += "\u2500";
        }

        GenText(MyFuntions.GenMeter(0, meterRange, textParent.transform,inter:5));
    }

    // Update is called once per frame
    void Update()
    {
        float pitch = drone.transform.eulerAngles.x;
        if (pitch > 180) pitch -= 360;
        pitchT1.text = (0-pitch).ToString("F0") + "\u25B7";
        pitchT2.text = "\u25C1" + (0-pitch).ToString("F0");
        foreach (Transform tr in textParent.transform) //移动对象到正确的位置
        {
            var number = int.Parse(tr.name[3..]);
            float deltaY = (pitch - number) * lineSpace;
            //var p = crossIcon.transform.position;
            //tr.position = new Vector3(p.x, p.y + deltaY, p.z);
            //tr.position = GetTargetPosition(deltaY, pitch, p);
            tr.localPosition = new Vector3(0, deltaY, 0);
        }
        var roll = drone.transform.eulerAngles.z;
        crossIcon.transform.rotation = Quaternion.Euler(0, 0, roll);
    }

    void GenText(List<int> list)
    {
        string str;
        string showStr;

        foreach (int i in list)
        {
            var newText = Instantiate(textPrefab, new Vector3(0, 0, 0), Quaternion.identity, textParent.transform);
            showStr = (0 - i).ToString();
            if (i < 0) str = "\u250C" + spaceString + "\u2510"; else str = "\u2514" + spaceString + "\u2518";
            if (i % 10 == 0) str = showStr + str + showStr;
            if (i == 0) str = lineString;
            newText.GetComponent<Text>().text = str;
            newText.name = "pit" + i.ToString();
        }
    }

}
