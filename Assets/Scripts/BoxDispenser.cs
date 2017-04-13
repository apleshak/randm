using UnityEngine;
using System.Collections;

public class BoxDispenser : MyMonoBehaviour
{
    public Object boxPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (GameObject.FindGameObjectsWithTag("Box").Length == 0)
        {
            Instantiate(boxPrefab, transform.position, Quaternion.identity);
        }
    }
}
