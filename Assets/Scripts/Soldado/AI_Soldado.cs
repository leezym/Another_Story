using UnityEngine;
using System.Collections;
using Globales;

public class AI_Soldado : MonoBehaviour {

	bool isFocus;
	bool isAttack;
	bool isLife;
	bool isMoving;
	bool isDeath;
		
	int Ran;

	NavMeshAgent Agent;

	Animator Anim;

	Transform myTransform;

	Vector3 StartPosition;
	Vector3 LastPosition;

	Quaternion RotationTarget;
	float RotateForce = 1f;

	public AudioClip DeathSound;

	AudioSource Sound;
	public AudioSource Foots;

	public GameObject Target;
	public GameObject Espada;

	public float DistanceTarget;

	public int Vit = 50;
	int LastVit;

	void Start () 
	{
		myTransform = this.transform;
		LastPosition = myTransform.position;
		StartPosition = myTransform.position;

		LastVit = Vit;

		isFocus = true;
		isAttack = false;
		isMoving = false;

		Ran = Random.Range (1, 3);

		if (GetComponentInChildren<Animator> ())			// Verificamos si posee componetes de animator
			Anim = GetComponentInChildren<Animator> ();		//Asignamos  los componentes a la variable
		else
			Debug.LogError ("Nesecita un animator");		//En caso de que no poseea componentes enviamos un mesaje de error

		if (GetComponentInChildren<NavMeshAgent> ())			// Verificamos si posee componetes de animator
			Agent = GetComponentInChildren<NavMeshAgent> ();		//Asignamos  los componentes a la variable
		else
			Debug.LogError ("Nesecita un Nav Mesh Agent");		//En caso de que no poseea componentes enviamos un mesaje de error	

		if (GetComponent<AudioSource> ())   		
			Sound = GetComponent<AudioSource> ();	
		else
			Debug.LogError ("Nesecita un AudioSource");	

		SelecTarget ();
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

		if (!isMoving)
			Foots.Play ();

		if (Vit != LastVit)
			Sound.Play ();

		LastVit = Vit;
			
		if(isLife)
		{
			Distance ();
			Rotation ();

			if(isFocus == true && Target != null)
				Agent.SetDestination (Target.transform.position);
		}

		if(Vit <= 0 && !isDeath)
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

	void Destroy ()
	{
		Destroy(gameObject);
	}

	void Death()
	{
		isDeath = true;

		//Sound.clip = DeathSound;
		//Sound.Play ();
		Anim.CrossFade ("Death",0);
		isFocus = false;
		isAttack = false;
		Agent.Stop (true);
		Espada.gameObject.GetComponent<BoxCollider> ().enabled = false;
	}

	void LateUpdate()
	{
		if (isAttack)
			Espada.gameObject.GetComponent<BoxCollider> ().enabled = true;
		else
			Espada.gameObject.GetComponent<BoxCollider> ().enabled = false;
	}

	void SelecTarget()
	{
		if(Ran == 1)
			Target = GameObject.FindWithTag ("Player");

		if (Ran == 2)
			Target = GameObject.FindWithTag ("Principe");
	}

	void UpdateAnimation()
	{
		Anim.SetBool ("Run",isFocus);
		Anim.SetBool ("Attack", isAttack);
	}

	void Distance()
	{
		if (Target != null) 
		{
			DistanceTarget = Vector3.Distance (Target.transform.position, myTransform.position);

			if (DistanceTarget < 4.5f)
				isAttack = true;
			else
				isAttack = false;
		}
	}
}
