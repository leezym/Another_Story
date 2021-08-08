using UnityEngine;
using System.Collections;
using Globales;

public class DamageSwordS : MonoBehaviour {

	public AI_Principe Prin;
	public CharacterController Player;

	AudioSource Sound;

	void Start()
	{
		if (GetComponent<AudioSource> ())   		
			Sound = GetComponent<AudioSource> ();	
		else
			Debug.LogError ("Nesecita un AudioSource");	
	}

	void OnTriggerEnter(Collider Other)
	{
		if(Other.gameObject.tag == "Player")
		{
			Player = Other.gameObject.GetComponent<CharacterController> ();
			GameController.Data.lifeFirst.value -= GameController.DamageSoldado; 
			Sound.Play ();
		}


		if(Other.gameObject.tag == "Principe")
		{
			Prin = Other.gameObject.GetComponentInParent<AI_Principe> ();
			GameController.Data.lifeSecond.value -= GameController.DamageSoldado; 
			Sound.Play ();
		}
	}
}
