using System;
using System.Collections;
using UnityEngine;

public class VehicleAvoidance : MonoBehaviour
{

#region Variables
    public float speed = 10.0f;
    public float mass = 5.0f;
    public float force = 40.0f;
    public float minimumDistToAvoid = 20.0f;

    float velocity;
    //Actual speed of the vehicle 
    public float curSpeed;
    private Vector3 targetPoint;
    private float initialSpeed;
    Vector3 desiredDestination;
#endregion

#region Unity Methods
    // Use this for initialization
    void Start()
    {
        mass = 5.0f;
        targetPoint = this.transform.localPosition;
        initialSpeed = speed;
    }

    void OnGUI()
    {
        GUILayout.Label("Click anywhere to move the vehicle to the clicked point");
    }

    // Update is called once per frame
    void Update()
    {
        //Vehicle move by mouse click
        RaycastHit hit;
        //Retrieve the mouse click position by shooting a ray from the camera
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)&& Physics.Raycast(ray, out hit, 100.0f))
        {
            //Take the point where the ray hits the ground plane as the target rotation
            targetPoint = hit.point;
        }

        //1- Compute the directional vector to the target position
        desiredDestination = targetPoint - transform.localPosition;

        // //2- When the target point is 1 meter away, exit the update function, so that the vehicle stops 
        if (desiredDestination.sqrMagnitude < 1)
            return;

        //3- Adjust the speed to delta time
        velocity = speed * Time.deltaTime;

        //4- Apply obstacle avoidance
        var newVector = AvoidObstacles();

        //5- Rotate the vehicle to its target directional vector
        LookAtTarget(newVector);

        //6- Move the vehicle towards the target point
        SetDestination();
    }
#endregion

#region Callbacks

    //Calculate the new directional vector to avoid the obstacle
    public Vector3 AvoidObstacles()
    {
        RaycastHit hit;

        float shoulderMultiplier = 1f;
        Vector3 leftR = transform.localPosition - (transform.right * shoulderMultiplier);
        Vector3 rightR = transform.localPosition + (transform.right * shoulderMultiplier);

        if (Physics.Raycast(leftR, transform.forward, out hit, minimumDistToAvoid))
        {
            if (hit.transform != transform)
            {
                Debug.DrawLine(leftR, hit.point, Color.blue);
                desiredDestination += hit.normal * force;
            }
        }

        else if (Physics.Raycast(rightR, transform.forward, out hit, minimumDistToAvoid))
        {
            if (hit.transform != transform)
            {
                Debug.DrawLine(rightR, hit.point, Color.green);
                desiredDestination += hit.normal * force;
            }
        }

        return desiredDestination;
    }

    void LookAtTarget(Vector3 target)
    {
        Quaternion rot = Quaternion.LookRotation(target);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
    }

    void SetDestination()
    {
        Debug.DrawLine(targetPoint, transform.position, Color.red);

        transform.localPosition += transform.forward * velocity;
    }
#endregion
}