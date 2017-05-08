using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ButtonHold : MyMonoBehaviour
{
	public bool shrink = false;
	public bool move = false;
	public string moveAxis = "x";
	Vector3 initialPos;
	Vector3 initialScale;
	Coroutine currentCoroutine;
	public GameObject actionTarget;
	public GameObject pushableElement;
    Vector3 pushableElementInitPos;
	float buttonPushDist = 0.5f;
	float buttonPushSpeed = 0.1f;
	public float pushSpeed = 0.1f;
	public float pushDist = 1.0f;
	public float shrinkSpeed = 0.1f;
	public float shrinkAmount = 0.25f;
	bool active;
	bool pushed;
	public GameObject actingObj;

	// Use this for initialization
	void Start()
	{
		initialPos = actionTarget.transform.position;
		initialScale = actionTarget.transform.localScale;
        pushableElementInitPos = pushableElement.transform.position;
		pushed = false;
	}

	public void OnTriggerStay2D (Collider2D other)
	{
		if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy") && !pushed)
		{
			if (currentCoroutine != null)
				StopCoroutine(currentCoroutine);
			actingObj = other.gameObject;
            active = true;
			currentCoroutine = StartCoroutine(PerformAction());
		}

        active = true;
    }
	
	public void OnTriggerExit2D (Collider2D other)
	{
		if (other.gameObject == actingObj)
		{
			StopCoroutine(currentCoroutine);
            pushed = false;
			actingObj = null;
            active = false;
            Debug.Log("Stopping button");
            currentCoroutine = StartCoroutine(ReverseAction());
        }
    }
	
	IEnumerator PerformAction()
	{
		//Button pushing logic
		Transform t = pushableElement.transform;
        Vector3 target = pushableElementInitPos + new Vector3(0, -buttonPushDist, 0);
        float accum = 0.0f;

        while (active && t != null && t.position != target)
        {
            yield return new WaitForEndOfFrame();
            accum += buttonPushSpeed;
            t.position = Vector3.Lerp(t.position, target, accum);
        }

        //Button pushed all the way down
        pushed = true;

        //Start moving and/or shrinking target object
		t = actionTarget.transform;
		Vector3 targetPos = t.position;
		Vector3 targetScale = t.localScale;
		
        if (move)
        {
            if (moveAxis == "x")
            {
                targetPos = new Vector3(initialPos.x - pushDist, initialPos.y, initialPos.z);
            }
            else
            {
                targetPos = new Vector3(initialPos.x, initialPos.y - pushDist, initialPos.z);
            }
        }

        if (shrink)
		    targetScale = new Vector3(initialScale.x, initialScale.y-shrinkAmount, initialScale.z);

        while (Vector3.Angle(t.position, targetPos) > 0 || (Vector3.Angle(t.localScale, targetScale)) > 0)
        {
            if (move)
            {
                if (moveAxis == "x")
                {
                    t.position = new Vector3(t.position.x - pushSpeed, t.position.y, t.position.z);
                }
                else
                {
                    t.position = new Vector3(t.position.x, t.position.y - pushSpeed, t.position.z);
                }
            }
			if (shrink)
				t.localScale = new Vector3(t.localScale.x, t.localScale.y-shrinkSpeed, t.localScale.z);
			yield return new WaitForEndOfFrame();
		}

        //Sit and wait for someone to get off the button
        while (pushed)
        {
            yield return new WaitForEndOfFrame();
        }
	}
	
	IEnumerator ReverseAction()
	{
		//Button logic
		Transform t = pushableElement.transform;
        Vector3 target = pushableElementInitPos;
        float accum = 0.0f;

        while (t != null && t.position != target)
        {
            yield return new WaitForEndOfFrame();
            accum += buttonPushSpeed;
            t.position = Vector3.Lerp(t.position, target, accum);
        }

        //Buttom released all the way up
        pushed = false;
        active = false;

        t = actionTarget.transform;

		while (Vector3.Angle(t.position, initialPos) > 0 || (Vector3.Angle(t.localScale, initialScale) > 0)) //add shrink
		{
            if (move)
            {
                if (moveAxis == "x")
                {
                    t.position = new Vector3(t.position.x + pushSpeed, t.position.y, t.position.z);
                }
                else
                {
                    t.position = new Vector3(t.position.x, t.position.y + pushSpeed, t.position.z);
                }
            }
            if (shrink)
				t.localScale = new Vector3(t.localScale.x, t.localScale.y+shrinkSpeed, t.localScale.z);
			yield return new WaitForEndOfFrame();
		}
	}
}
