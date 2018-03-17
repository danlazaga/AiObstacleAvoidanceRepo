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
    float distance;

	// Use this for initialization
	void Start () 
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
	void Update () 
    {
        //Vehicle move by mouse click
        RaycastHit hit;
		//Retrieve the mouse click position by shooting a ray from the camera
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, 100.0f))
        {
			//Take the point where the ray hits the ground plane as the target rotation
			targetPoint = hit.point;
        }
			
		//1- Compute the directional vector to the target position
		ComputeDistance();
       
		//2- When the target point is 1 meter away, exit the update function, so that the vehicle stops 
		CheckDistance();

		//3- Adjust the speed to delta time
		speed = Time.deltaTime;

		//4- Apply obstacle avoidance

		//5- Rotate the vehicle to its target directional vector

		//6- Move the vehicle towards the target point
		SetDestination(targetPoint, speed, CheckDistance());
	}

    float ComputeDistance()
    {
        distance = (transform.position - targetPoint).magnitude;

        return distance;
	}

    bool CheckDistance()
    {
        if (distance > 25)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

	//Calculate the new directional vector to avoid the obstacle
	public Vector3 AvoidObstacles()
	{
		throw new NotImplementedException ();
	}

	void SetDestination(Vector3 destination, float deltaTime, bool isMoving)
	{
		if(isMoving)
		{
            Vector3 direction = destination - transform.position;
            direction.Normalize();
            transform.position += direction * curSpeed * deltaTime;

            Quaternion newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 2f);
		}
	}
}