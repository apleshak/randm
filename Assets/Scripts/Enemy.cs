using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MyMonoBehaviour
{
    public GameObject hitSpot;
    public float moveSpeed = 0.3f;
    public float detectDist = 4.0f;
    public float stunDuration = 2.0f;
    bool stunned;
    public bool enabled = false;
    Rigidbody2D r;
    UnityStandardAssets._2D.PlatformerCharacter2D m_character;
    // Use this for initialization
    void Start ()
    {
        r = GetComponent<Rigidbody2D>();
        stunned = false;
        m_character = GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!enabled)
            return;

        if (!stunned)
        {
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) < detectDist)
            {
                if (transform.position.x < player.transform.position.x)
                {
                    m_character.Move(1.0f*moveSpeed, false, false);
                }
                else
                {
                    m_character.Move(-1.0f*moveSpeed, false, false);
                }
                //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position, moveSpeed);
            }
            else
            {
                m_character.Move(0.0f, false, false);
            }
        }
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!enabled)
            return;

        Transform t = other.transform;
        float speed = r.velocity.magnitude;

        if (t.gameObject.tag == "Box" && speed > 0.5)
        {
            Stunme();
            GameObject.Destroy(t.gameObject);
        }
    }

    public void Stunme()
    {
        StartCoroutine(_Stun());
    }

    // Update is called once per frame
    IEnumerator _Stun()
    {
        hitSpot.active = true;
        stunned = true;

        float count = 0;

        while (count < stunDuration)
        {
            count += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        stunned = false;
        hitSpot.active = false;
    }
}
