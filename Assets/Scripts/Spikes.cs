using UnityEngine;
using System.Collections;

public class Spikes : MyMonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();

            if (rb.velocity.magnitude > 1.0)
            {
                RestartLevel();
            }
        }
        else if (other.gameObject.tag == "Box")
        {
            Destroy(other.gameObject);
        }
    }
}
