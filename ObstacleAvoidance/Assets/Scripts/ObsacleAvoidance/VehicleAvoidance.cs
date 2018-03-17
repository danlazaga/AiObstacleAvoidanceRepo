using UnityEngine;
using System.Collections;
using System;

public class VehicleAvoidance : MonoBehaviour
{
    public float speed = 10.0f;
    public float mass = 5.0f;
    public float force = 40.0f;
    public float minimumDistToAvoid = 20.0f;
    public float heightMultiplier;

    float velocity;
    //Actual speed of the vehicle 
    public float curSpeed;
    private Vector3 targetPoint;
    private float initialSpeed;
    float distance;

    // Use this for initialization
    void Start()
    {
        mass = 5.0f;
        targetPoint = Vector3.zero;
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
        ComputeDistance();

        // //2- When the target point is 1 meter away, exit the update function, so that the vehicle stops 
        if (ComputeDistance() > 20)
            return;

        // //3- Adjust the speed to delta time
        speed = Time.deltaTime;

        // //4- Apply obstacle avoidance
        AvoidObstacles();

        // //5- Rotate the vehicle to its target directional vector
        LookAtTarget(targetPoint);

        //6- Move the vehicle towards the target point
        SetDestination(targetPoint, speed);
    }

    float ComputeDistance()
    {
        distance = Vector3.Distance(transform.position, targetPoint);

        return distance;
    }

    //Calculate the new directional vector to avoid the obstacle
    public Vector3 AvoidObstacles()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, transform.forward * minimumDistToAvoid, Color.green);
        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized * minimumDistToAvoid, Color.green);
        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized * minimumDistToAvoid, Color.green);

        if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, minimumDistToAvoid))
        {
            if (hit.transform != transform)
            {
                targetPoint += hit.normal * force;
            }
        }

        if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right), out hit, minimumDistToAvoid))
        {
            if (hit.transform != transform)
            {
                targetPoint += hit.normal * force;
            }
        }

        if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right), out hit, minimumDistToAvoid))
        {
            if (hit.transform != transform)
            {
                targetPoint += hit.normal * force;
            }
        }

        return hit.normal;
    }

    void LookAtTarget(Vector3 target)
    {
         var rot = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
    }

    void SetDestination(Vector3 target, float deltaTime)
    {
        Vector3 direction = target - transform.position;
        direction.Normalize();
        transform.position += direction * curSpeed * deltaTime;
    }
}