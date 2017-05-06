using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Button : MyMonoBehaviour
{
    public bool shrink = false;
    public bool move = false;
    public string moveAxis = "x";
    bool pushable;
    public bool pushed;
    public GameObject actionTarget;
    public Func<GameObject, int> DoAction;
    public GameObject pushableElement;
    public float buttonPushDist = 0.5f;
    public float buttonPushSpeed = 0.1f;
    public float pushSpeed = 0.1f;
    public float pushDist = 1.0f;
    public float shrinkSpeed = 0.1f;
    public float shrinkAmount = 0.25f;

    // Use this for initialization
    void Start()
    {
        Func<GameObject, int> defaultAction = g => 1;
        DoAction = defaultAction;
        pushable = true;
        pushed = false;
	}

    public int ShrinkObjectY (GameObject g)
    {
        StartCoroutine(ShrinkObjectYCoroutine(g, shrinkAmount, shrinkSpeed));
        return 1;
    }

    public int GrowObjectY(GameObject g)
    {
        StartCoroutine(ShrinkObjectYCoroutine(g, -shrinkAmount, shrinkSpeed));
        return 1;
    }

    public int PushObjectX(GameObject g)
    {
        StartCoroutine(PushObjectXCoroutine(g, pushDist, pushSpeed));
        return 1;
    }
    
	public int PushObjectY(GameObject g)
	{
		StartCoroutine(PushObjectYCoroutine(g, pushDist, pushSpeed));
		return 1;
	}

	public int PullObjectY(GameObject g)
	{
		StartCoroutine(PushObjectYCoroutine(g, -pushDist, pushSpeed));
		return 1;
	}
	
    public int PullObjectX(GameObject g)
    {
        StartCoroutine(PushObjectXCoroutine(g, -pushDist, pushSpeed));
        return 1;
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.tag == "Player" && pushable)
        {
            Console.WriteLine("Pushing button..");
            PushButton();
        }
    }

    public virtual IEnumerator PushButtonCoroutine ()
    {
        Transform t = pushableElement.transform;
        Vector3 target = t.position + new Vector3(0, -buttonPushDist, 0);
        float accum = 0.0f;

        while (t != null && t.position != target)
        {
            yield return new WaitForEndOfFrame();
            accum += buttonPushSpeed;
            t.position = Vector3.Lerp(t.position, target, accum);
        }

        if (shrink)
        {
            DoAction = ShrinkObjectY;

            if (pushed)
            {
                DoAction = GrowObjectY;
            }

            DoAction(actionTarget);
        }
        if (move)
        {
        	if (moveAxis == "x")
        	{
				DoAction = PushObjectX;
        	}
            else
            {
				DoAction = PushObjectY;
            }

            if (pushed)
            {
            	if (moveAxis == "x")
            	{
					DoAction = PullObjectX;
            	}
            	else
        		{
					DoAction = PullObjectY;
        		}
            }
            
            DoAction(actionTarget);
        }

        pushed = !pushed;

        target = t.position + new Vector3(0, buttonPushDist, 0);
        accum = 0.0f;

        while (t != null && t.position != target)
        {
            yield return new WaitForEndOfFrame();
            accum += buttonPushSpeed;
            t.position = Vector3.Lerp(t.position, target, accum);
        }

        pushable = true;
    }

    public void PushButton()
    {
        pushable = false;
        StartCoroutine(PushButtonCoroutine());
    }

    public static IEnumerator ShrinkObjectYCoroutine(GameObject g, float amount, float speed)
    {
        Transform t = g.transform;
        Vector3 target = t.localScale - new Vector3(0, amount, 0);
        float accum = 0.0f;

        while (t != null && t.localScale != target)
        {
            yield return new WaitForEndOfFrame();
            accum += speed;
            t.localScale = Vector3.Lerp(t.localScale, target, accum);
        }
    }

    public IEnumerator PushObjectXCoroutine(GameObject g, float amount, float speed)
    {
        Transform t = g.transform;
        Vector3 target = t.position - new Vector3(amount, 0, 0);
        float accum = 0.0f;

        while (t != null && t.position != target)
        {
            yield return new WaitForEndOfFrame();
            accum += speed;
            t.position = Vector3.Lerp(t.position, target, accum);
        }
    }
    
	public IEnumerator PushObjectYCoroutine(GameObject g, float amount, float speed)
	{
		Transform t = g.transform;
		Vector3 target = t.position - new Vector3(0, amount, 0);
		float accum = 0.0f;
		
		while (t != null && t.position != target)
		{
			yield return new WaitForEndOfFrame();
			accum += speed;
			t.position = Vector3.Lerp(t.position, target, accum);
		}
	}
}
