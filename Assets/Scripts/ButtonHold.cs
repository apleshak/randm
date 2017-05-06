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
	public Coroutine currentCoroutine;
	public GameObject actionTarget;
	public GameObject pushableElement;
	public float buttonPushDist = 0.5f;
	public float buttonPushSpeed = 0.1f;
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
		pushed = false;
	}

	public void OnTriggerStay2D (Collider2D other)
	{
		if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy") && !active && !pushed)
		{
			if (currentCoroutine != null)
				StopCoroutine(currentCoroutine);
			active = true;
			actingObj = other.gameObject;
			currentCoroutine = StartCoroutine(PerformAction());
		}
	}
	
	public void OnTriggerExit2D (Collider2D other)
	{
		if (active && other.gameObject == actingObj)
		{
			StopCoroutine(currentCoroutine);
			active = false;
			actingObj = null;
			currentCoroutine = StartCoroutine(ReverseAction());
		}
	}
	
	IEnumerator PerformAction()
	{
		//Button pushing logic
		Transform t = pushableElement.transform;
        Vector3 target = t.position + new Vector3(0, -buttonPushDist, 0);
        float accum = 0.0f;

        while (t != null && t.position != target)
        {
            yield return new WaitForEndOfFrame();
            accum += buttonPushSpeed;
            t.position = Vector3.Lerp(t.position, target, accum);
        }

        //Button pushed all the way down
        pushed = true;

        //Start moving and/or shrinking target object
		t = actionTarget.transform;
		Vector3 targetPos;
		Vector3 targetScale;
		
		if (moveAxis == "x")
		{
			targetPos = new Vector3(initialPos.x-pushDist, initialPos.y, initialPos.z);
		}
		else
		{
			targetPos = new Vector3(initialPos.x, initialPos.y-pushDist, initialPos.z);
		}

		targetScale = new Vector3(initialScale.x, initialScale.y-shrinkAmount, initialScale.z);
		
		while (Vector3.Distance(t.position, targetPos) > pushSpeed || (Vector3.Distance(t.localScale, targetScale)) < 0)
		{
			Debug.Log(Vector3.Distance(t.position, targetPos));
			if (move)
				t.position = new Vector3(t.position.x-pushSpeed, t.position.y, t.position.z);
			if (shrink)
				t.localScale = new Vector3(t.localScale.x, t.localScale.y-shrinkSpeed, t.localScale.z);
			yield return new WaitForEndOfFrame();
		}
	}
	
	IEnumerator ReverseAction()
	{
		//Button logic
		Transform t = pushableElement.transform;
		Vector3 target = t.position + new Vector3(0, buttonPushDist, 0);
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

		while (Vector3.Distance(t.position, initialPos) > pushSpeed || (Vector3.Distance(t.localScale, initialScale) > 0)) //add shrink
		{
			if (move)
				t.position = new Vector3(t.position.x+pushSpeed, t.position.y, t.position.z);
			if (shrink)
				t.localScale = new Vector3(t.localScale.x, t.localScale.y+shrinkSpeed, t.localScale.z);
			yield return new WaitForEndOfFrame();
		}
	}
}
