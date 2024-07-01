using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControl : MonoBehaviour
{
    public float forceScale = 1.0f;
    public GameObject Drone;
    private Rigidbody rig;
    void Start()
    {
        rig = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float spd = rig.velocity.magnitude;
        if (spd <= 30f)
        {
            rig.AddForce(transform.forward * forceScale, ForceMode.Force);
        }

        float dis = Vector3.Distance(transform.position, Drone.transform.position);
        if (dis > 500f)
        {
            Vector3 currentDirection = transform.forward;
            var targetPosition = new Vector3(Drone.transform.position.x,transform.position.y,Drone.transform.position.z);
            Vector3 targetDirection = (targetPosition - transform.position).normalized;
            Vector3 newDirection = Vector3.Lerp(currentDirection, targetDirection, Time.deltaTime * 2);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.forward * forceScale);
    }
}
