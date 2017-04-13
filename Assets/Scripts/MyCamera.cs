using UnityEngine;
using System.Collections;

/* 	Tracks and follows the player. Implements organic wiggle independent of changes to camera angle and
	movement. The wiggle is accomlished by rotating the camera to look at random points on a virtual
	circle of radius 1. The strenght of the rotation transfers to the camera through magnitude. 
	
	"How to lerp like a pro" will improve this script. */
[RequireComponent (typeof(Camera))]
public class MyCamera : MyMonoBehaviour
{
	Camera camera;
	Vector3 heading;
	Vector3 oldHeading;
	float percentTurn;
	Vector3 targetCirclePos;
	Vector3 currentCirclePos;
	Vector3 targetCameraPos;
	Vector3 cameraTarget;
	
	public bool wiggle;
    public bool followHorizontal;
    public bool followVertical;
	public Vector3 lookAt;
    public GameObject track;

	/* Speed at which the heading changes. */
	public float turnSpeed;
	/* Speed at which the camera moves towards a target point on the circle. */
	public float moveSpeed;
	/* Min distance between waypoints before camera finds a new waypoint. */
	public float touchDist;
	/* The strength of the effect on the camera movement. */
	public float magnitude;
	/* Camera starts to follow after the target moves away from where the camera was looking at by this much. */
	public float followLeashLength;
	public float followSpeed;

	void Awake ()
	{
		camera = GetComponent<Camera>();
		currentCirclePos = new Vector3(0, 0, 0);
		targetCirclePos = RandPointOnCircle();
		heading = targetCirclePos.normalized;
		oldHeading = heading;
	}
	
	void Start ()
	{
        if (lookAt == Vector3.zero)
        {
            lookAt = transform.position;
        }

		targetCameraPos = lookAt;
		cameraTarget = targetCameraPos;
	}
	
	/* All wiggle relate code here, except actual lookAt call - it's in LateUpdate. */
	void Update () 
	{		
		if (wiggle)
		{
			percentTurn += turnSpeed * Time.deltaTime;
			
			/* Update our position with heading. */
			currentCirclePos = Vector3.Slerp(currentCirclePos + heading * moveSpeed, 
											 targetCirclePos, moveSpeed * Time.deltaTime);
			/* Update our heading. */
			heading = Vector3.Slerp(oldHeading, (targetCirclePos - currentCirclePos).normalized, percentTurn);
			
			/* If we have arrived at our destination, then choose a new destination. */
			if (Vector3.Distance(currentCirclePos, targetCirclePos) <= touchDist)
			{
				oldHeading = heading;
				percentTurn = 0;
				targetCirclePos = RandPointOnCircle();
			}
			
			/* Instead of udating camera here we do it in LateUpdate. */
		}
	}
	
	/* 	Have to manipulate camera here to keep execution consistency and avoid jitter. Candidate for
		better/proper lerp logic. Find "How to lerp like a pro" for that. */
	void LateUpdate ()
	{
        /*
		if (followVertical || followHorizontal)
		{
			//Find where the camera would be if it followed all the time.
			Vector3 targetMovement = track.transform.position - cameraTarget;
			targetCameraPos = transform.position + targetMovement;
			
            if (followHorizontal && followVertical)
            {
                targetCameraPos = transform.position + new Vector3(targetMovement.x, targetMovement.y, targetMovement.z);
            }
            else if (followVertical)
            {
                targetCameraPos = transform.position + new Vector3(0, targetMovement.y, targetMovement.z);
            }
            else
            {
                targetCameraPos = transform.position + new Vector3(targetMovement.x, 0, targetMovement.z);
            }

			// If the distance between where it should have been and where it is is greater than the leash slide it in.
			if (Vector3.Distance(targetCameraPos, transform.position) >= followLeashLength)
			{
				transform.position = Vector3.Slerp(transform.position, targetCameraPos, followSpeed * Time.deltaTime);	
				cameraTarget = Vector3.Lerp(cameraTarget, cameraTarget + targetMovement, followSpeed * Time.deltaTime);
			}
		}
        */

		//Update our camera rotation.
		camera.transform.LookAt(new Vector3(lookAt.x, lookAt.y, lookAt.z));
	}
	
	Vector3 RandPointOnCircle ()
	{
		/* Fix random x within bounds of the circle. */
		float x = Random.Range(-1.0f, 1.0f);
		/* Solve for its absolute y value.*/
		float y = Mathf.Sqrt(1 - x*x);
		/* Decide is it should be negative or positive. */
		if (Random.Range(0.0f,1.0f) > 0.5f)
		{
			y *= -1;
		}
		
		return new Vector3(x, y, 0.0f);
	}
}