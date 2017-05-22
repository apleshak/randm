using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBox : MyMonoBehaviour
{
    Grab g;
    public float throwForce = 1000.0f;

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
                UnityStandardAssets._2D.PlatformerCharacter2D cc = gameObject.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>();
                if (cc.m_FacingRight)
                {
                    r.AddForce((Vector2)(gameObject.transform.forward + new Vector3(throwForce, 0, 0)));
                }
                else
                {
                    r.AddForce((Vector2)(gameObject.transform.forward + new Vector3(-throwForce, 0, 0)));
                }
            }
        }
	}
}
