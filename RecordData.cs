using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
public class RecordData 
{
    public Vector3 position;
    public Quaternion rotation;
    public float engineSoundPitch;
    public Vector2 leftControlPointPos;
    public Vector2 rightControlPointPos;
    public float speed;
    public float relativeAlt;

    public bool SaveTransform(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        return true;
    }
    
}
