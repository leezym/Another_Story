using UnityEngine;
using System.Collections;
using Globales;

public class DamageAxe : MonoBehaviour {

	AI_Principe Prin;
	CharacterController Player;

	AudioSource Sound;

	void Start()
	{
		if (GetComponent<AudioSource> ())   		
			Sound = GetComponent<AudioSource> ();	
		else
			Debug.LogError ("Nesecita un AudioSource");	
	}

	void OnCollisionEnter(Collision Other)
	{
		if(Other.gameObject.tag == "Player")
		{
			Player = Other.gameObject.GetComponent<CharacterController> ();
			GameController.Data.lifeFirst.value -= GameController.DamageBoss;
			Sound.Play ();
		}


		if(Other.gameObject.tag == "Principe")
		{
			Prin = Other.gameObject.GetComponentInParent<AI_Principe> ();
			GameController.Data.lifeSecond.value -= GameController.DamageBoss;
			Sound.Play ();
		}
	}
}
