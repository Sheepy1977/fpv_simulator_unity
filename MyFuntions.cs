using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyFuntions : MonoBehaviour
{
    // Start is called before the first frame update
    public static int FindClosestMultiple(float num,int multi)
    {
        int lowerMultiple = ((int)num / multi) * multi; // 小于等于num的最大5的倍数
        int upperMultiple = lowerMultiple + multi; // 大于num的最小5的倍数

        if (Mathf.Abs(num - lowerMultiple) <= Mathf.Abs(num - upperMultiple))
        {
            return lowerMultiple;
        }
        else
        {
            return upperMultiple;
        }
    }

    public static List<int> GenMeter(float input, int range, Transform parent, int MinMulti = 5, int inter = 1, string lowLimitStr = "",string highLimitStr = "" )
    {
        var a = FindClosestMultiple(input, MinMulti);//找到最接近能被5整除的数字
        int lowLimit;
        int highLimit;

        lowLimit = a - range / 2;
        highLimit = a + range / 2;

        if (lowLimitStr != "" && lowLimit < int.Parse(lowLimitStr))
        {
            lowLimit = int.Parse(lowLimitStr);
        }

        if (highLimitStr != "" && highLimit > int.Parse(highLimitStr))
        {
            highLimit = int.Parse(highLimitStr);
        }

        int smallest = 0;
        int bigest = 0;
        List<int> list = new();
        if (parent.childCount > 0)
        {
            foreach (Transform tr in parent) //删掉不需要的对象
            {
                var number = int.Parse(tr.name[3..]);
                if (number > highLimit || number < lowLimit)
                {
                    Destroy(tr.gameObject);
                }
            }
            int count = 0;
            foreach (Transform tr in parent) //求出当前剩下的对象最大和最小的
            {
                var number = int.Parse(tr.name[3..]);
                if (count == 0)
                {
                    smallest = number;
                    bigest = number;
                    count++;
                }
                else
                {
                    if (number < smallest) smallest = number;
                    if (number > bigest) bigest = number;
                }
            }
            if (smallest > lowLimit) //如果最小的都大于应显示的下限
            {
                for (int i = lowLimit; i <= smallest; i+= inter)
                {
                    list.Add(i);
                }
            }
            if (bigest < highLimit)//如果最大的都小于应显示的上限
            {
                for (int i = bigest; i <= highLimit; i+=inter)
                {
                    list.Add(i);
                }
            }
        }
        else //初始生成，从下限生成到上限
        {
            for (int i = lowLimit; i <= highLimit; i+=inter)
            {
                list.Add(i);
            }
        }
        return list;
    }
}
