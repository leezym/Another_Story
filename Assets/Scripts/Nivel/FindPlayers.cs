using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Globales;

public class FindPlayers : MonoBehaviour {

	GameObject SpawnJimmy;
	GameObject SpawnAnnie;
	GameObject Personaje;
	Main mainP; 

	void Awake ()
	{
		Personaje = GameObject.FindWithTag ("Player");
		SpawnJimmy = GameObject.Find ("SpawnJimmy");
		SpawnAnnie = GameObject.Find ("SpawnAnnie");
	}

	void Start()
	{	
		mainP = GameObject.Find ("Script Menu").GetComponent<Main> ();

		if (mainP.player == 1)
		{
			Personaje = Resources.Load ("Annie") as GameObject;
			Debug.LogWarning ("Annie Select");
			Instantiate (Personaje, SpawnAnnie.transform.position, SpawnAnnie.transform.rotation);
		}

		if(mainP.player == 2)
		{
			Personaje = Resources.Load ("Jimmy") as GameObject;	
			Debug.LogWarning ("Jimmy Select");
			Instantiate (Personaje, SpawnJimmy.transform.position, SpawnJimmy.transform.rotation);
		}

	}

}
