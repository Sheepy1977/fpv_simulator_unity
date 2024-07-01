using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public Vector2 inputDelta;
    public float radius; //公开已让其他代码获取最大值
    public float returnDuration = 1f;
    

    public GameObject replicateObj; //复制显示
    private GameObject replicatePoint;
    private GameObject point;
    private Vector2 center,touchPoint,repCenter;
    private float replicateControlRadius;
    //private Rect rect;
    void Start()
    {
        center = gameObject.GetComponent<RectTransform>().position;
        radius = gameObject.GetComponent<RectTransform>().sizeDelta.x / 2;
        //rect = gameObject.GetComponent<RectTransform>().rect;
        point = gameObject.transform.GetChild(0).gameObject;
        touchPoint = center;
        replicatePoint = replicateObj.transform.GetChild(0).gameObject; //控制指示器
        replicateControlRadius = replicateObj.GetComponent<RectTransform>().sizeDelta.x / 2;
        repCenter = replicateObj.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            if (!CheckPoint(touch.position))
            //if (Vector2.Distance(touch.position, center) > radius)
            {
                if (Input.touchCount > 1)
                {
                    touch = Input.touches[1];
                }

            }
            if (touch.phase != TouchPhase.Ended)
            {
                touchPoint = touch.position;
                if (CheckPoint(touchPoint))
                //if (Vector2.Distance(touchPoint, center) <= radius)
                {
                    point.transform.position = touchPoint;
                }
            }
            else
            {
                StartCoroutine(ReturnCenter(returnDuration));
            }
        }
        
        touchPoint = point.transform.position;
        inputDelta = touchPoint - center;
        ReplicateControlShow();
    }

    IEnumerator ReturnCenter(float duration)
    {
        float timeStep = Time.deltaTime;
        for (float i = 0;i < duration; i+= timeStep) {
            yield return new WaitForSeconds(timeStep);
            float percent = Mathf.Clamp(i / duration,0,1);
            point.transform.position = Vector2.Lerp(point.transform.position, center, percent);
        }
    }

    void ReplicateControlShow()
    {
        float x = inputDelta.x / radius * replicateControlRadius + repCenter.x;
        float y = inputDelta.y / radius * replicateControlRadius + repCenter.y;
        replicatePoint.transform.position = new Vector2(x, y);
    }

    bool CheckPoint(Vector2 touchPoint)
    {
        float minX = center.x - radius;
        float maxX = center.x + radius;
        float minY = center.y - radius;
        float maxY = center.y + radius;
        float x = touchPoint.x;
        float y = touchPoint.y; 
        if (x >= minX && y >= minY && x <= maxX && y <= maxY) { return true; } else { return false; }
    }
}
