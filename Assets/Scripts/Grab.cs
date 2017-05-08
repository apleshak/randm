using UnityEngine;
using System.Collections;

public class Grab : MyMonoBehaviour
{
	public GameObject grabbedObject;
	public float radius = 1.0f;
    public float holdHeight;
    public bool holding = false;

	// Use this for initialization
	void Start () 
	{

	}
	
    public void Release()
    {
        grabbedObject.transform.parent = null;
        //grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
        grabbedObject = null;
        holding = false;
    }

	void Update () 
	{
        if (grabbedObject == null)
        {
            holding = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (holding)
            {
                Release();
            }
            else
            {
                GameObject tograb = GetClosestBoxInRange();

                if (tograb != null)
                {
                    grabbedObject = tograb;
                    grabbedObject.transform.position = gameObject.transform.position + new Vector3(0, holdHeight, 0);
                    //grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                    grabbedObject.transform.parent = gameObject.transform;
                    holding = true;
                }
            }
        }
	}

    private GameObject GetClosestBoxInRange()
    {
        GameObject closestObject = null;
        float closestDistance = radius;

        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), radius);

        foreach (Collider2D collider in objectsInRange)
        {
            if (collider.gameObject.tag == "Box")
            {
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
