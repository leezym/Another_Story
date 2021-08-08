using UnityEngine;
using System.Collections;
using Globales;

public class TrampaZarza : MonoBehaviour {

	public GameObject Target;

	public bool Freezing;
	bool Active = false;
	int Count;

	public float TimeDestroy;
	float TimeCount = 0;

	public int Vit = 20;

	PlayerController CC;

	Rigidbody RB;

	Animator Anim;

	AudioSource Audio;

	void Start()
	{
		Freezing = false;
		TimeDestroy = 0;

		if (GetComponentInChildren<Animator> ())   // Verificamos si hay componetes de animator en los objetos hijos
			Anim = GetComponentInChildren<Animator> ();	// Asignamos los componentes del animator a la variable
		else
			Debug.LogError ("Nesecita un Animator");	// En caso de no tener un componente de animator, enviamos un mensaje de error

		if (GetComponent<AudioSource> ())   		
			Audio = GetComponent<AudioSource> ();	
		else
			Debug.LogError ("Nesecita un AudioSource");	
	}
	
	void OnTriggerEnter(Collider Other)
	{
		if (Target == null && Other.gameObject.tag == "Player") 
		{
			Active = true;

			Audio.Play ();

			Target = Other.gameObject;

			Freezing = true;

			if (Target.GetComponent<PlayerController> ())
				CC = Target.GetComponent<PlayerController> ();
			else 
				Debug.LogError ("Nesecita un CharacterController");

			if (Target.GetComponent<Rigidbody> ())
				RB = Target.GetComponent<Rigidbody> ();
			else 
				Debug.LogError ("Nesecita un CharacterController");
		}

		if (Other.gameObject.tag == "Bullet")
		{
			Vit -= GameController.DamagePlayer;
			Destroy (Other.gameObject);
		}
	}
		
	void Update () 
	{
		UpdateAnimation ();

		if(Active)
		{
			TimeCount += Time.deltaTime;
		}

		if (TimeCount > 0 && TimeCount <= 2)
			Count = 1;
		if (TimeCount > 2)
			Count = 2;

		if (Freezing)
		{
			CC.Freezing = true;
			RB.constraints = RigidbodyConstraints.FreezePosition;
			TimeDestroy = TimeDestroy + Time.deltaTime;
		}

		if(TimeDestroy > 5)
		{
			CC.Freezing = false;
			RB.constraints &= ~RigidbodyConstraints.FreezePositionX;
			RB.constraints &= ~RigidbodyConstraints.FreezePositionZ;
		}

		if (TimeDestroy > 6)
			Destroy (gameObject);

		if (Vit == 0 || Vit < 0) 
			Destroy (gameObject);
	}

	void UpdateAnimation()
	{
		Anim.SetInteger ("Estado", Count);
	}
}
