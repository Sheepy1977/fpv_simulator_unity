using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class HeadingShow : MonoBehaviour
{
    public GameObject textParent;
    public GameObject textPrefab;
    public GameObject drone;
    public Text hdgMainText;
    public float spaceScale = 25f;
    public int hdgMeterRange = 40;
    

    // Update is called once per frame
    void Update()
    {
        float hdg = drone.transform.rotation.eulerAngles.y;
        hdgMainText.text = hdg.ToString("F0") + "\n\u25BD";
        GenText(MyFuntions.GenMeter(hdg, hdgMeterRange, textParent.transform));
        foreach (Transform tr in textParent.transform) //移动对象到正确的位置
        {
            var number = int.Parse(tr.name[3..]);
            float delta = (number - hdg) * spaceScale;
            var mainTextPos = hdgMainText.transform.position;
            tr.position = new Vector3(mainTextPos.x + delta, mainTextPos.y - 120, mainTextPos.z);
        }
    }
    void GenText(List<int> list)
    {
        string str;
        foreach (int i in list)
        {
            int showHdg = i;
            if (i >= 360) showHdg -= 360;
            if (i < 0) showHdg += 360;
            if (showHdg % 5 == 0) str = "|\n" + showHdg.ToString() ; else str = "|";
            if (showHdg == 90) str = "|\nE";
            if (showHdg == 180) str = "|\nS";
            if (showHdg == 270) str = "|\nW";
            if (showHdg == 0) str = "|\nN";

            var newText = Instantiate(textPrefab, new Vector3(0, 0, 0), Quaternion.identity, textParent.transform);
            newText.GetComponent<Text>().text = str;
            newText.name = "hdg" + i.ToString();
        }
    }
}
