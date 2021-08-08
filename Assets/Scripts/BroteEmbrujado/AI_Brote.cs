using UnityEngine;
using System.Collections;
using Globales;

public class AI_Brote : MonoBehaviour {

	public GameObject Target;
	public GameObject Particles;
	public GameObject Pua;

	public float DistanceTarget;

	public int Vit = 50;
	int LastVit;

	Transform myTransform;

	Vector3 StartPosition;
	Vector3 LastPosition;

	Quaternion RotationTarget;
	float RotateForce = 1f;

	Rigidbody RB;

	NavMeshAgent Agent;

	Animator Anim;

	public AudioClip DeathSound;

	AudioSource Sound;
	//public AudioSource FootsSound;

	bool isFocus;
	bool isAttack;
	bool isInvoked;
	bool isActive;
	bool isLife;
	bool isMoving;
	bool isDeath;

	void Start () 
	{
		myTransform = this.transform;
		LastPosition = myTransform.position;
		StartPosition = myTransform.position;
		LastVit = Vit;

		isFocus = false;
		isAttack = false;
		isInvoked = false;
		isActive = false;
		isMoving = false;

		//FootsSound.enabled = false;

		if (GetComponentInChildren<Animator> ())   // Verificamos si hay componetes de animator en los objetos hijos
			Anim = GetComponentInChildren<Animator> ();	// Asignamos los componentes del animator a la variable
		else
			Debug.LogError ("Nesecita un Animator");	// En caso de no tener un componente de animator, enviamos un mensaje de error

		if (GetComponentInChildren<NavMeshAgent> ())			// Verificamos si posee componetes de animator
			Agent = GetComponentInChildren<NavMeshAgent> ();		//Asignamos  los componentes a la variable
		else
			Debug.LogError ("Nesecita un Nav Mesh Agent");		//En caso de que no poseea componentes enviamos un mesaje de error

		if (GetComponent<AudioSource> ())   		
			Sound = GetComponent<AudioSource> ();	
		else
			Debug.LogError ("Nesecita un AudioSource");	

		Agent.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateAnimation ();

		if (Vit > 0)
			isLife = true;

		if (Vit < 1)
			isLife = false;

		if (myTransform.position != LastPosition)						// Verificamos que la posicion actual sea diferente de la posicion almacenada en la variable lastPosition
			isMoving = true;											// si se cumple dicha condicion, le indicamos que isMoving es verdadera
		else
			isMoving = false;											//En caso de que no se cumpla, indicamos que isMoving es falso

		LastPosition = myTransform.position;	

//		if (!isMoving)
//			FootsSound.Play ();

		if (Vit != LastVit)
			Sound.Play ();

		LastVit = Vit;

		if(isLife)
		{
			Distance ();
			Rotation ();

			if (isFocus && Target != null) 
			{
				Agent.SetDestination (Target.transform.position);
			}
				
		}

		if(!isLife && !isDeath)
		{
			Death ();
			Invoke ("Destroy",3f);
		}
	}

	void Rotation()
	{
		if(Target != null)
		{
			RotationTarget = Quaternion.LookRotation (Target.transform.position - transform.position);
			float RF = Mathf.Min (RotateForce * Time.deltaTime, 1);
			transform.rotation = Quaternion.Lerp (transform.rotation, RotationTarget, RF);
		}
	}

	void UpdateAnimation()
	{
		Anim.SetBool ("Invoke", isInvoked);
		Anim.SetBool ("Focus",isFocus);
		Anim.SetBool ("Attack", isAttack);
	}

	void Destroy()
	{
		Destroy(gameObject);
	}

	void Death()
	{
		isDeath = true;

		Sound.clip = DeathSound;
		Sound.Play ();
		Anim.CrossFade ("Death",0);
		Pua.gameObject.GetComponent<BoxCollider> ().enabled = false;
		isFocus = false;
		isAttack = false;
		isInvoked = false;
		isActive = false;
	}

	void Distance()
	{
		if (Target != null) 
		{
			DistanceTarget = Vector3.Distance (Target.transform.position, myTransform.position);

			if (DistanceTarget < 3.5f)
				isAttack = true;
			else
				isAttack = false;

			if (DistanceTarget > 15f)
			{
				isFocus = false;
				Target = null;
				DistanceTarget = 0;
				ReturnInitialPosition ();
			}
		}
	}

	void DisableParticles()
	{
		Particles.SetActive (false);
	}

	void ReturnInitialPosition()
	{
		Agent.SetDestination (StartPosition);
	}

	void OnTriggerEnter(Collider Other)
	{
		if(Other.gameObject.tag == "Player" && !isActive)			
		{
			isInvoked = true;
			isActive = true;
			isFocus = true;
			Agent.enabled = true;
			Particles.gameObject.SetActive (true);
			Invoke ("DisableParticles",3f);
			Anim.CrossFade ("Invoke",1f);
			Target = Other.gameObject;
		}

		if(Other.gameObject.tag == "Player" && isActive)
		{
			isFocus = true;
			Target = Other.gameObject;
		}

		if(Other.gameObject.tag == "Principe" && !isActive)			
		{
			isInvoked = true;
			isActive = true;
			Agent.enabled = true;
			Anim.CrossFade ("Invoke",1f);
			Target = Other.gameObject;
		}

		if(Other.gameObject.tag == "Principe" && isActive)
		{
			isFocus = true;
			Target = Other.gameObject;
		}
	}
}
