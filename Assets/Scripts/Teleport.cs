using UnityEngine;
using System.Collections;

public class Teleport : MyMonoBehaviour {

    public Vector3 currPos;
    public int screenCount = 2;
    public float vertDist;
    public float horizDist;
    bool waitingForClick;
    public MyCamera cam;

	// Use this for initialization
	void Start ()
    {
	    
	}

    // Update is called once per frame
    void Update()
    {
        currPos = gameObject.transform.position;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (screenCount == 2)
            {
                if (currPos.y > 0)
                {
                    gameObject.transform.position += new Vector3(0, -vertDist, 0);
                    //cam.transform.position += new Vector3(0, -vertDist, 0);
                    //cam.lookAt += new Vector3(0, -vertDist, 0);
                }
                else
                {
                    gameObject.transform.position += new Vector3(0, vertDist, 0);
                    //cam.transform.position += new Vector3(0, vertDist, 0);
                    //cam.lookAt += new Vector3(0, vertDist, 0);
                }
            }
            else if (Time.timeScale != 0.0f)
            {
                Time.timeScale = 0.0f;
                waitingForClick = true;
            }
            else
            {
                Time.timeScale = 1.0f;
                waitingForClick = false;
            }
        }

        if (waitingForClick && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 click = Input.mousePosition;
            Debug.Log(click);

            if (click.y >= Screen.height / 2 && currPos.y < 0)
            {
                gameObject.transform.position += new Vector3(0, vertDist, 0);
            }
            else if (click.y < Screen.height / 2 && currPos.y > 0)
            {
                gameObject.transform.position += new Vector3(0, -vertDist, 0);
            }

            if (screenCount != 2 && click.x >= Screen.width / 2 && currPos.x < 0)
            {
                gameObject.transform.position += new Vector3(horizDist, 0, 0);
            }
            else if (screenCount != 2 && click.x < Screen.width / 2 && currPos.x > 0)
            {
                gameObject.transform.position += new Vector3(-horizDist, 0, 0);
            }

            waitingForClick = false;
            Time.timeScale = 1.0f;
        }
    }
}
