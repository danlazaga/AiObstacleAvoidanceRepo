using UnityEngine;
using System.Collections;
using System;

public class VehicleAvoidance : MonoBehaviour
{
    public float speed = 10.0f;
    public float mass = 5.0f;
    public float force = 40.0f;
    public float minimumDistToAvoid = 20.0f;

    float velocity;
    //Actual speed of the vehicle 
    public float curSpeed;
    private Vector3 targetPoint;
    private float initialSpeed;
    Vector3 dir;

    // Use this for initialization
    void Start()
    {
        mass = 5.0f;
        targetPoint = this.transform.position;
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

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, 100.0f))
        {
            //Take the point where the ray hits the ground plane as the target rotation
            targetPoint = hit.point;
        }

        //1- Compute the directional vector to the target position
        //ComputeDistance();
        dir = (targetPoint - transform.position).normalized;

        // //2- When the target point is 1 meter away, exit the update function, so that the vehicle stops 
        if (ComputeDistance() < 1)
            return;

        //3- Adjust the speed to delta time
        speed = Time.deltaTime;

        //4- Apply obstacle avoidance
        AvoidObstacles();

        //5- Rotate the vehicle to its target directional vector
        LookAtTarget(AvoidObstacles());

        //6- Move the vehicle towards the target point
        SetDestination();
    }

    #region Callbacks
    float ComputeDistance()
    {
        var distance = Vector3.Distance(transform.position, targetPoint);

        return distance;
    }

    //Calculate the new directional vector to avoid the obstacle
    public Vector3 AvoidObstacles()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, minimumDistToAvoid))
        {
            if (hit.transform != transform)
            {
                Debug.DrawLine(transform.position, hit.point, Color.blue);
                dir += hit.normal * 50;
            }
        }

        Vector3 leftR = transform.position;
        Vector3 rightR = transform.position;

        leftR.x -= 2;
        rightR.x += 2;

        if (Physics.Raycast(leftR, transform.forward, out hit, minimumDistToAvoid))
        {
            if (hit.transform != transform)
            {
                Debug.DrawLine(leftR, hit.point, Color.blue);
                dir += hit.normal * 50;
            }
        }

        if (Physics.Raycast(rightR, transform.forward, out hit, minimumDistToAvoid))
        {
            if (hit.transform != transform)
            {
                Debug.DrawLine(rightR, hit.point, Color.blue);
                dir += hit.normal * 50;
            }
        }

        return dir;
    }

    void LookAtTarget(Vector3 target)
    {
        Quaternion rot = Quaternion.LookRotation(target);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
    }

    void SetDestination()
    {
        Debug.DrawLine(targetPoint, transform.position, Color.red);
        transform.position += transform.forward * curSpeed * Time.deltaTime;
    }
    #endregion
}