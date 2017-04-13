using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using EntityAction = System.Func<UnityEngine.GameObject, UnityEngine.GameObject, System.Collections.IEnumerator>;

public class MyMonoBehaviour : MonoBehaviour 
{
	/* Utilities - only safe to access after Awake finishes initializing non-MonoBehavious. */
	public static bool showDebug = true;
    public static MyMonoBehaviour instance;
	public static GameObject player;
	public static EntityDescriptor playerDescriptor;
	
	/* Necessary to avoid multiple instantiations. */
	static bool initializedNonMonos = false;
	static bool updatedNonMonos = false;

	/* Instantiates non-MonoBehaviours and utilities. */
	void Awake ()
	{
		if (!initializedNonMonos)
		{
			if (showDebug) Debug.Log("_____MyMonoBehaviour: Performing first-time initialization_____");
			
			//Set this immediately so that subsequent initializations of player and minions don't make a loop
			initializedNonMonos = true;
			instance = this;
			
			if (showDebug) Debug.Log("_____MyMonoBehaviour: First-time initialization complete_____");
			Debug.Log ("");
		}
	}
	
	/* Updates all non-MonoBehaviours. */
	//TODO remove the return when ready to test commanders
	void Update ()
	{
		return;
	}
	
	//After all updates have run reset update status on commanders
	void LateUpdate ()
	{
		
	}

    public void RestartLevel ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
