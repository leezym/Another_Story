using UnityEngine;
using System.Collections;
using Globales;

public class DamageEspadaPrincipe : MonoBehaviour {

	AI_Brote Bro;
	AI_PatrolSoldado PS;
	AI_Soldado Sol;
	AI_Boss Boss;

	AudioSource Sound;
	float NextSoundTime = 0;
	public AudioClip []AttackSound;

	void Start()
	{
		if (GetComponent<AudioSource> ())   		
			Sound = GetComponent<AudioSource> ();	
		else
			Debug.LogError ("Nesecita un AudioSource");	
	}

	void OnTriggerEnter(Collider Other)
	{
		if(Other.gameObject.tag == "Brote")
		{
			Bro = Other.gameObject.GetComponentInParent<AI_Brote> ();
			Bro.Vit -= GameController.DamagePartner; 
			Sound.clip = AttackSound [Random.Range (0, 2)];
			Sound.Play ();
		}

		if(Other.gameObject.tag == "SoldadoP")
		{
			PS = Other.gameObject.GetComponentInParent<AI_PatrolSoldado> ();
			PS.Vit -= GameController.DamagePartner; 
			Sound.clip = AttackSound [Random.Range (0, 2)];
			Sound.Play ();
		}

		if(Other.gameObject.tag == "soldado")
		{
			Sol = Other.gameObject.GetComponentInParent<AI_Soldado> ();
			Sol.Vit -= GameController.DamagePartner;
			Sound.clip = AttackSound [Random.Range (0, 2)];
			Sound.Play ();
		}

		if(Other.gameObject.tag == "Boss")
		{
			Boss = Other.gameObject.GetComponentInParent<AI_Boss> ();
			GameController.Data.lifeBoss.value -= (GameController.DamagePartner - 10);
			Sound.clip = AttackSound [Random.Range (0, 2)];
			Sound.Play ();
		}
	}
}
