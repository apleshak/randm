using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBox : MyMonoBehaviour
{
    Grab g;

	// Use this for initialization
	void Start ()
    {
        g = GetComponent<Grab>();	
	}
	
	// Update is called once per frame
	void Update ()
    {
        GameObject grabbed = g.grabbedObject;

        if (grabbed != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                g.Release();
                Rigidbody2D r = grabbed.GetComponent<Rigidbody2D>();
                r.AddForce(new Vector2(10, 10));
                Debug.Log("doing");
            }
        }
	}
}
