using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 0.1f;
    public float detectDist = 4.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (Vector3.Distance(gameObject.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < detectDist)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position, moveSpeed);
        }
        else
        {

        }
	}
}
