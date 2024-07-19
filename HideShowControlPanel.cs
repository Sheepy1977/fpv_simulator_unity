using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HideShowControlPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public float hideTime = 3f;
    private float timeStamp;
    private Vector3 oriPos,hidePos;
    private bool isVisible;
    void Start()
    {
        oriPos = transform.position;
        isVisible = true;
        hidePos = oriPos + new Vector3(450, 0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(oriPos.ToString());
        if (Replay.isOnReplay && timeStamp == 0)
        {
            timeStamp = Time.time;
        }

        if (Input.touchCount > 0)
        {
            timeStamp = Time.time;
            transform.position = oriPos;
            isVisible = true;
        }
        if (Time.time - timeStamp > hideTime && isVisible)
        {
            if (transform.position != hidePos){
                // 每帧沿着方向移动物体
                transform.Translate(25,0,0);
            }
            else
            {
                isVisible = false;
            }
        }
    }

}
