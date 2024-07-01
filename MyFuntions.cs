using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyFuntions : MonoBehaviour
{
    // Start is called before the first frame update
    public static int FindClosestMultiple(float num,int multi)
    {
        int lowerMultiple = ((int)num / multi) * multi; // С�ڵ���num�����5�ı���
        int upperMultiple = lowerMultiple + multi; // ����num����С5�ı���

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
        var a = FindClosestMultiple(input, MinMulti);//�ҵ���ӽ��ܱ�5����������
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
            foreach (Transform tr in parent) //ɾ������Ҫ�Ķ���
            {
                var number = int.Parse(tr.name[3..]);
                if (number > highLimit || number < lowLimit)
                {
                    Destroy(tr.gameObject);
                }
            }
            int count = 0;
            foreach (Transform tr in parent) //�����ǰʣ�µĶ���������С��
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
            if (smallest > lowLimit) //�����С�Ķ�����Ӧ��ʾ������
            {
                for (int i = lowLimit; i <= smallest; i+= inter)
                {
                    list.Add(i);
                }
            }
            if (bigest < highLimit)//������Ķ�С��Ӧ��ʾ������
            {
                for (int i = bigest; i <= highLimit; i+=inter)
                {
                    list.Add(i);
                }
            }
        }
        else //��ʼ���ɣ����������ɵ�����
        {
            for (int i = lowLimit; i <= highLimit; i+=inter)
            {
                list.Add(i);
            }
        }
        return list;
    }
}
