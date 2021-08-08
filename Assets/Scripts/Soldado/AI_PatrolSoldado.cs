using UnityEngine;
using System.Collections;
using Globales;

public class AI_PatrolSoldado : MonoBehaviour {

	public int DesPoint = 0;

	public bool isFocus;
	public bool isPatrol;
	bool isReturn;
	public bool isAttack;
	bool isLife;
	public bool isMoving;
	bool isDeath;

	public NavMeshAgent Agent;

	Animator Anim;

	Transform myTransform;
	Quaternion myQuaternion;

	public Vector3 StartPosition;
	Vector3 LastPosition;

	Quaternion RotationTarget;
	float RotateForce = 1f;

	public AudioClip DeathSound;

	AudioSource Sound;
	public AudioSource Foots;

	public Transform[] PatrolPoint;
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

		isFocus = false;
		isPatrol = true;
		isReturn = false;
		isMoving = false;

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

		//Agent.autoBraking = false;

		Foots = GetComponentInChildren<AudioSource> ();

		GoToNextPoint ();

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

			if(isPatrol)
			{
				Agent.autoBraking = false;

				if (Agent.remainingDistance < 0.8f)
					GoToNextPoint ();
			}

			if(isFocus == true && Target != null)
			{
				isPatrol = false;
				Agent.SetDestination (Target.transform.position);
				Agent.stoppingDistance = 4f;
				Agent.autoBraking = true;
			}
		}
			
		if(!isLife && !isDeath)
		{
			Death ();
			Invoke ("Destroy",3f);
		}
	}

	void LateUpdate()
	{
		if (isAttack)
			Espada.gameObject.GetComponent<BoxCollider> ().enabled = true;
		else
			Espada.gameObject.GetComponent<BoxCollider> ().enabled = false;	
	}

	void Destroy()
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
		isPatrol = false;
		isReturn = false;
		Agent.Stop (true);
		Espada.gameObject.GetComponent<BoxCollider> ().enabled = false;
	}

	void UpdateAnimation()
	{
		Anim.SetBool ("Walk",isPatrol);
		Anim.SetBool ("Run",isFocus);
		Anim.SetBool ("Attack", isAttack);
		Anim.SetBool ("Idle", !isMoving);
	}

	void GoToNextPoint()
	{
		if (PatrolPoint.Length == 0)
			return;

		Agent.destination = PatrolPoint [DesPoint].position;

		DesPoint = (DesPoint + 1) % PatrolPoint.Length;
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

			if (DistanceTarget > 15f)
			{
				Target = null;
				DistanceTarget = 0;
				ReturnInitialPosition ();
			}
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

	void ReturnInitialPosition()
	{
		Agent.SetDestination (StartPosition);
		//isPatrol = true;
		//isFocus = false;
	}
		
	void OnTriggerEnter(Collider Other)
	{
		if(Other.gameObject.tag == "Player" || Other.gameObject.tag == "Principe")			
		{
			isFocus = true;
			Target = Other.gameObject;
		}
	}
}
