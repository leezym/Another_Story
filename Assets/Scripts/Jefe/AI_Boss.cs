using UnityEngine;
using System.Collections;
using Globales;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AI_Boss : MonoBehaviour {

	bool isAttack;
	bool isInvoke;
	bool isSpecial = false; 
	bool isFollow;
	bool isLife;
	bool isMoving;
	bool isDeath;
	bool SoundSpecial;

	float DistanceTarget;
	public float Charge;
	float TimeInvoke;

	public GameObject Target;
	public GameObject SpecialArea;
	public GameObject Soldado;

	public Transform[] PosInvoke;

	Transform myTransform;
	Vector3 LastPosition;

	public AudioClip DeathSound;
	public AudioClip []BossSound;

	AudioSource Sound;
	AudioSource FootsSound;
	public AudioSource EspecialAttack;
	float NextSoundTime;

	NavMeshAgent Agent;

	Animator Anim;

	MainGame MG;
	float TimeShow = 0;
	public bool ShowDeadBoss;


	void Awake()
	{
		GameController.Data.lifeBoss = GameObject.FindGameObjectWithTag ("saludBoss").GetComponent <Slider> ();
	}

	void Start ()
	{
		GameController.Data.lifeBoss.value = GameController.Data.lifeBoss.maxValue;
		GameController.Data.LastVitBoss = GameController.Data.lifeBoss.value;

		MG = GameObject.Find ("Script Menu Game").GetComponent<MainGame>();
		ShowDeadBoss = false;

		myTransform = this.transform;
		LastPosition = myTransform.position;

		SpecialArea.SetActive (false);

		isAttack = false;
		isInvoke = false;
		isSpecial = false;
		isFollow = true;
		isMoving = false;

		Charge = 0;

		if (GetComponentInChildren<NavMeshAgent> ())			// Verificamos si posee componetes de animator
			Agent = GetComponentInChildren<NavMeshAgent> ();		//Asignamos  los componentes a la variable
		else
			Debug.LogError ("Nesecita un Nav Mesh Agent");		//En caso de que no poseea componentes enviamos un mesaje de error

		if (GetComponentInChildren<Animator> ())   // Verificamos si hay componetes de animator en los objetos hijos
			Anim = GetComponentInChildren<Animator> ();	// Asignamos los componentes del animator a la variable
		else
			Debug.LogError ("Nesecita un Animator");	// En caso de no tener un componente de animator, enviamos un mensaje de error

		if (GetComponent<AudioSource> ())   		
			Sound = GetComponent<AudioSource> ();	
		else
			Debug.LogError ("Nesecita un AudioSource");	

		FootsSound = GameObject.Find ("FootBoss").GetComponent<AudioSource> ();

	}

	void Update ()
	{
		UpdateAnimation ();
		NextSoundTime += Time.deltaTime;

		if(Target == null)
			Target = GameObject.FindWithTag ("Player");

		if (GameController.Data.lifeBoss.value  > 0)
			isLife = true;
		if (GameController.Data.lifeBoss.value  < 1)
			isLife = false;

		if (myTransform.position != LastPosition)						// Verificamos que la posicion actual sea diferente de la posicion almacenada en la variable lastPosition
			isMoving = true;											// si se cumple dicha condicion, le indicamos que isMoving es verdadera
		else
			isMoving = false;											//En caso de que no se cumpla, indicamos que isMoving es falso

		LastPosition = myTransform.position;	

		if (!isMoving)
			FootsSound.Play ();

		if (GameController.Data.lifeBoss.value != GameController.Data.LastVitBoss) 
		{
			if(NextSoundTime >= BossSound[1].length)
			{
				Sound.clip = BossSound [Random.Range (0, 3)];
				Sound.Play ();
				NextSoundTime = 0;
			}
		}

//		if (Charge >= 3 && isSpecial)
//		{
//			ActiveSound ();
//		}

		GameController.Data.LastVitBoss = GameController.Data.lifeBoss.value;

		if (ShowDeadBoss)
		{
			TimeShow = TimeShow + Time.deltaTime;
		}

		if (TimeShow >= 5) 
		{
			TimeShow = 0;
			ShowDeadBoss = false;
			MG.GetComponent<MainGame> ().showDeadBoss = true;
		}

		if(isLife)
		{
			StartCoroutine (Boss ());
			TimeInvoke = TimeInvoke + Time.deltaTime;
			Distance ();
		}

		if(!isLife && !isDeath)
		{
			Death ();
		}
	}

	void Death()
	{
		isDeath = true;
		Sound.clip = DeathSound;
		Sound.Play ();
		Anim.CrossFade ("Death", 0);
		ShowDeadBoss = true;
		isAttack = false;
		isInvoke = false;
		isSpecial = false;
		isFollow = false;
	}
	 
	void ActiveSound()
	{
		Debug.Log ("sonido");
		isSpecial = false;
		EspecialAttack.Play ();
	}

	IEnumerator Boss()
	{
		if (isFollow)
			Agent.SetDestination (Target.transform.position);

		if(isAttack)
		{
			Invoke ("CountSpecial", 1);
		}

		if(Charge > 2.5f)
		{
			isSpecial = true;
			Agent.Stop (true);
			yield return new WaitForSeconds (2.7f);
			Agent.ResetPath ();
		}

		if(Charge > 2.7f)
		{
			Debug.Log("Explosion");
			SpecialArea.SetActive (true);
		}

		if(isSpecial)
		{
			Invoke ("Change", 2.7f);
		}

		if(TimeInvoke > 25)
		{
			InvokeSolado ();
			isInvoke = true;
			TimeInvoke = 0;
		}

		if(isInvoke)
		{
			isFollow = false;
			Agent.Stop (true);
			//idle
			yield return new WaitForSeconds (5);
			isFollow = true;
			Agent.ResetPath ();
			isInvoke = false;
		}

		yield return new WaitForSeconds (1);
	}

	void Distance()
	{
		if(Target != null)
		{
			DistanceTarget = Vector3.Distance (Target.transform.position, myTransform.position);

			if (DistanceTarget < 4.5f && !isSpecial)
				isAttack = true;
			else
				isAttack = false;
		}
	}

	void CountSpecial()
	{
		Charge = Charge + Time.deltaTime;
	}

	void Change()
	{
		isSpecial = false;
		SpecialArea.SetActive (false);
		Charge = 0;
	}

	void InvokeSolado()
	{
		isFollow = false;

		for( int i = 0; i < PosInvoke.Length; i ++)
		{
			Instantiate (Soldado, PosInvoke [i].transform.position, Quaternion.identity);
		}
	}

	void UpdateAnimation()
	{
		Anim.SetBool ("Follow", isFollow);
		Anim.SetBool ("Special",isSpecial);
		Anim.SetBool ("Attack",isAttack);
	}
}
