using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VSpeedShow : MonoBehaviour
{
    public GameObject textParent; //竖向的数字父物体
    public GameObject textPrefab;
    public GameObject drone;
    public Text mainText; //主显示数字
    public Text altText;
    public Text relativeAltText; //相对高度
    public float lineScale = 20f;
    public int meterRange = 40;
    public Image postiveLine, negativeLine;

    public static float relativeAlt;
    private float height,lastHeight,verSpd;
    private int lowLimit, highLimit;
    private void Start()
    {
        lastHeight = drone.transform.position.y;
        lowLimit = -meterRange / 2;
        highLimit = meterRange / 2;
    }

    // Update is called once per frame
    void Update()
    {
        float alt = drone.transform.position.y;
        altText.text = alt.ToString("F0") + "m";
        mainText.text = verSpd.ToString("F1") + "m/s" + "\u25B7";
        var calVerSpd = Mathf.Clamp(verSpd,lowLimit, highLimit);
        GenText(MyFuntions.GenMeter(calVerSpd, meterRange, textParent.transform,lowLimitStr:lowLimit.ToString(),highLimitStr:highLimit.ToString()));
        foreach (Transform tr in textParent.transform) //移动对象到正确的位置
        {
            var number = int.Parse(tr.name[3..]);
            float deltaY = (number - calVerSpd) * lineScale;
            var mainTextPos = mainText.transform.position;
            tr.position = new Vector3(mainTextPos.x + 300, (mainTextPos.y + deltaY), mainTextPos.z); //80是主文字的x宽度的一半
        }

        float vsPercent;
        if (calVerSpd < 0)
        {
            vsPercent = Mathf.Clamp(-calVerSpd / -lowLimit, 0, 1);
            negativeLine.fillAmount = vsPercent;
            postiveLine.fillAmount = 0;
        }
        else
        {
            vsPercent = Mathf.Clamp(calVerSpd / highLimit, 0, 1);
            negativeLine.fillAmount = 0;
            postiveLine.fillAmount = vsPercent;
        }

        var th = Terrain.activeTerrain.SampleHeight(drone.transform.position);
        relativeAlt = alt - th;
        relativeAltText.text = "H:" + relativeAlt.ToString("F1") + "m";
    }

    private void FixedUpdate()
    {
        height = drone.transform.position.y;
        var vetDis = height - lastHeight;
        verSpd = vetDis / (Time.deltaTime);
        lastHeight = height;
    }

    void GenText(List<int> list)
    {
        string str;
        foreach (int i in list)
        {
            var newText = Instantiate(textPrefab, new Vector3(0, 0, 0), Quaternion.identity, textParent.transform);
            if (i % 5 == 0) str = "-" + i.ToString(); else str = "-";
            //if (i == 0) str = "-0";
            newText.GetComponent<Text>().text = str;
            newText.name = "vsp" + i.ToString();
        }
    }
}
