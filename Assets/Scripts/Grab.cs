using UnityEngine;
using System.Collections;

public class Grab : MyMonoBehaviour
{
	public GameObject grabbedObject;
    Rigidbody2D rbGrabbedObject;
	public float grabRadius = 3.0f;
    public float holdHeight = 1.5f;
    public bool holding = false;

	// Use this for initialization
	void Start () 
	{

	}
	
    public void Release()
    {
        if (grabbedObject != null)
        {
            rbGrabbedObject.isKinematic = false;
            grabbedObject.GetComponent<BoxCollider2D>().enabled = true;
            grabbedObject = null;
        }

        holding = false;
    }

	void Update ()
	{
        if (grabbedObject == null)
        {
            holding = false;
        }
        else
        {
            grabbedObject.transform.position = gameObject.transform.position + new Vector3(0, holdHeight, 0);
            grabbedObject.GetComponent<BoxCollider2D>().enabled = false;
            rbGrabbedObject.velocity = new Vector2(0, 0);
            rbGrabbedObject.angularVelocity = 0;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (holding)
            {
                Release();
            }
            else
            {
                Debug.Log("grabbing");
                GameObject tograb = GetClosestBoxInRange();

                if (tograb != null)
                {
                    Debug.Log("got something to grab");
                    grabbedObject = tograb;
                    rbGrabbedObject = grabbedObject.GetComponent<Rigidbody2D>();
                    rbGrabbedObject.isKinematic = true;
                    rbGrabbedObject.velocity = new Vector2(0, 0);
                    grabbedObject.GetComponent<BoxCollider2D>().enabled = false;
                    holding = true;
                }
            }
        }
	}

    private GameObject GetClosestBoxInRange()
    {
        GameObject closestObject = null;
        float closestDistance = grabRadius + 1;

        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), grabRadius);

        foreach (Collider2D collider in objectsInRange)
        {
            if (collider.gameObject.tag == "Box")
            {
                Debug.Log("found box i nrange");
                float newDistance = Vector3.Distance(transform.position, collider.transform.position);

                if (newDistance < closestDistance)
                {
                    closestDistance = newDistance;
                    closestObject = collider.gameObject;
                }
            }
        }

        return closestObject;
    }
}
