using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Button : MyMonoBehaviour
{
    public bool shrink = false;
    public bool move = false;
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

    int ShrinkObjectY (GameObject g)
    {
        StartCoroutine(ShrinkObjectYCoroutine(g, shrinkAmount, shrinkSpeed));
        return 1;
    }

    int GrowObjectY(GameObject g)
    {
        StartCoroutine(ShrinkObjectYCoroutine(g, -shrinkAmount, shrinkSpeed));
        return 1;
    }

    int PushObjectX(GameObject g)
    {
        StartCoroutine(PushObjectXCoroutine(g, pushDist, pushSpeed));
        return 1;
    }

    int PullObjectX(GameObject g)
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

    IEnumerator PushButtonCoroutine ()
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
            DoAction = PushObjectX;

            if (pushed)
            {
                DoAction = PullObjectX;
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

    IEnumerator PushObjectXCoroutine(GameObject g, float amount, float speed)
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
}
