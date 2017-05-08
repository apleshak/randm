using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MyMonoBehaviour
{
    public float moveSpeed = 0.1f;
    public float detectDist = 4.0f;
    public float stunDuration = 2.0f;
    bool stunned;
    public bool enabled = false;
    Rigidbody2D r;
	// Use this for initialization
	void Start ()
    {
        r = GetComponent<Rigidbody2D>();
        stunned = false;	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!enabled)
            return;

        if (!stunned)
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

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!enabled)
            return;

        Transform t = other.transform;
        float speed = r.velocity.magnitude;
        
        if (speed > 0.5)
        {
            StartCoroutine(Stunned());
        }

        if (t.gameObject.tag == "Box")
        {
            GameObject.Destroy(t.gameObject);
        }
    }

    IEnumerator Stunned()
    {
        float accum = 0.0f;

        while (accum <= stunDuration)
        {
            stunned = true;
            accum += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        stunned = false;
    }
}
