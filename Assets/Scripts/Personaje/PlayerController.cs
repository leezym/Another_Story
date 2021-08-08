using UnityEngine;
using System.Collections;
using Globales;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	//float LastVit;					// Indica el ultimo valor de vida del jugador

	public float InputDelay = 0.1f; // Tiempo de respuesta al precionar una tecla
	public float ForwardVel = 12;   // Velocidad de Desplazamiento
	public float RotateVel = 100;	// Velocidad de Rotacion
	public float Speed;

	int NumClip;

	Quaternion targetRotation;  	// Alamcena la rotacion del target

	Rigidbody RB;					// Permite obtener los componentes de un rigidbody

	float ForwardInput, TurnInput;	// Inputs para el movimiento horizontal y vertical
	float NextSoundTime = 0;

	Animator Anim;					// Permite obtener los componentes del animator

	Scene InfoScene;					// Permite obtener componentes de la escena

	public AudioClip[] PlayerAudio;
	public AudioClip[] FootSound;
	public AudioClip DeathSound;

	public AudioSource Foot;		// obtengo los componentes del audiosource de los pasos
	AudioSource Audio;				// Obtengo los componentes del auidosource del jugador

	bool isMoving; 					// Me indica si puede moverse
	bool isLife = true;				// Indicamos si el personaje se encuentra con vida
	bool isDeath;

	[HideInInspector]
	public bool Freezing;			// Me indica si estoy en una trampa
	[HideInInspector]
	public bool Charge;				// Me indica si estoy cargando el ataque basico

	MainGame showdead;
	float timeShow = 0;
	public bool showDead;
	public bool Curar;

	void Awake()
	{
		GameController.Data.lifeFirst = GameObject.FindGameObjectWithTag ("saludPlayer").GetComponent<Slider> ();
		InfoScene = SceneManager.GetActiveScene ();
	}

	void Start()
	{
		// Accedes para mostrar el menu Game Over
		showdead = GameObject.Find ("Script Menu Game").GetComponent<MainGame>();
		showDead = false;
		Curar = false;

		if (InfoScene.name == "PrimerNivel(Patio)") {
			GameController.Data.lifeFirst.value = GameController.Data.lifeFirst.maxValue; // Indica la cantidad de vida del jugador
			NumClip = 0;
		} else 
		{
			GameController.Data.lifeFirst.value = GameController.Data.lifeFirst.value;
			NumClip = 1;
		}

		targetRotation = transform.rotation;  // Asignamos la rotacion del target a la variable
		GameController.Data.LastVitFirst = GameController.Data.lifeFirst.value;				//indicamos que el valor de lasVit sera igual al de la vida actual

		isMoving = false;					// Inicialisamos la variable en falso;
		Freezing = false;					// Inicialisamos la variable en falso;
		Charge = false;						// Inicialisamos la variable en falso;
	
		if (GetComponentInChildren<Animator> ())   		// Verificamos si hay componetes de animator en los objetos hijos
			Anim = GetComponentInChildren<Animator> ();	// Asignamos los componentes del animator a la variable
		else
			Debug.LogError ("Nesecita un Animator");	// En caso de no tener un componente de animator, enviamos un mensaje de error

		if (GetComponent<Rigidbody> ())			//Verificamos si hay componetes de un rigidbody 
			RB = GetComponent<Rigidbody> ();	// Obtenemos los componentes del rigidbody
		else
			Debug.LogError ("Necesita un rigidbody");	//Enviamos un mensaje en caso de no encontrar el rigidbody

		if (GetComponent<AudioSource> ())   		
			Audio = GetComponent<AudioSource> ();	
		else
			Debug.LogError ("Nesecita un AudioSource");	

		//Foot = GameObject.Find ("FootAudio").GetComponent<AudioSource> ();

		ForwardInput = TurnInput = 0;		// Inicializamos los inputs en 0
	}

	void GetInput() 						//Funcion para capturar los valores de los inputs
	{
		ForwardInput = Input.GetAxis ("Vertical");   //Asignamos el valor del eje y a la variable
		TurnInput = Input.GetAxis ("Horizontal");	 //Asignamos el valor del eje x a la variable
	}

	void Update()
	{
		UpdateAnimation ();
		NextSoundTime += Time.deltaTime;
			
		if (GameController.Data.lifeFirst.value > GameController.Data.lifeFirst.maxValue)
			GameController.Data.lifeFirst.value = GameController.Data.lifeFirst.maxValue;

		if (GameController.Data.lifeFirst.value > 0)
			isLife = true;
		
		if (GameController.Data.lifeFirst.value < 1)
			isLife = false;

		if (!isMoving)
		{
			Foot.clip = FootSound [NumClip];
			Foot.Play();
		}
			
		if(isLife)
		{
			GetInput ();

			if(!Freezing)						// Verificamos si la variable Freezing es falsa
				Turn ();						// En caso de ser falsa llamamos la funcion turn

			if (GameController.Data.lifeFirst.value != GameController.Data.LastVitFirst)
			{
				if (!Curar) 
				{
					if (NextSoundTime >= PlayerAudio [0].length) 
					{
						Audio.clip = PlayerAudio [Random.Range(0,2)];
						Audio.Play ();
						NextSoundTime = 0;
					}
				}

				if (Curar) 
				{
					if (NextSoundTime >= PlayerAudio [0].length) 
					{
						Curar = false;
						Audio.clip = PlayerAudio [2];
						Audio.Play ();
						NextSoundTime = 0;
					}
				}

			}

			GameController.Data.LastVitFirst = GameController.Data.lifeFirst.value;
		}

		if(GameController.Data.lifeFirst.value <= 0 && !isDeath)
		{
			DeathPlayer ();
		}

		if (showDead)
		{
			timeShow = timeShow + Time.deltaTime;
		}

		if (timeShow >= 4) 
		{
			timeShow = 0;
			showDead = false;
			showdead.GetComponent<MainGame> ().showDead = true;
		}
	}

	void FixedUpdate()
	{
		if(isLife)
		{
			if (!Freezing)						// Verificamos si la variable Freezing es falsa
				Run ();							// En caso de ser falsa llamamos la funcion Rurn
			else
				isMoving = false;				// en caso de ser verdadera indicamos que isMoving es falso

			if (!Charge)						// Verificamos si la variable Charge es falsa
				Run ();							// En caso de ser falsa llamamos la funcion Rurn
			else
				isMoving = false;				// en caso de ser verdadera indicamos que isMoving es falso
		}
	}

	void DeathPlayer()
	{
		isDeath = true;

		Audio.clip = DeathSound;
		Audio.Play();

		Anim.CrossFade ("Death",0);
		isMoving = false;
		Freezing = false;
		Charge = false;
		showDead = true;
		RB.constraints = RigidbodyConstraints.FreezePositionX;
	}

	void Run()										//Funcion para desplazar el personaje
	{
		if (Mathf.Abs (ForwardInput) > InputDelay) //Verificamos que el movimiento vertical sea mayor al tiempo de delay
		{  
			isMoving = true;												// Volvemos verdadera la variable
			RB.velocity = transform.forward * ForwardInput * ForwardVel;	// Si es verdadero, le asignamos una velocidad al rigidbody
			RB.velocity = RB.velocity.normalized * Speed;
			//RB.velocity = Vector3.ClampMagnitude(RB.velocity, Speed);

		} else
		{
			RB.velocity = Vector3.zero;								// Si es falso, la velocidad del rigidbody sera cero
			isMoving = false;										// Volvemos flasa la variable
		}
	}
		
	void Turn() 									//Funcion para rotar el personaje
	{
		if (Mathf.Abs (TurnInput) > InputDelay) 	//Verificamos que el movimiento horizontal sea mayor al tiempo de delay
			targetRotation *= Quaternion.AngleAxis (RotateVel * TurnInput * Time.deltaTime, Vector3.up);	//si es verdadero, rotaremos el personaje

		transform.rotation = targetRotation;		//Si es falso, la rotacion del personaje sera igual a la rotacion inicial
	}

	void OnTriggerEnter(Collider Other)
	{
		if(Other.gameObject.tag == "Trampa")
			GameController.Data.lifeFirst.value -= GameController.DamageTrampa;

		if (Other.gameObject.tag == "Explosion")
			GameController.Data.lifeFirst.value -= GameController.DamageExplosion;

		if (Other.gameObject.tag == "Salud") 
		{
			Curar = true;
			
			if (GameController.Data.lifeFirst.value < GameController.Data.lifeFirst.maxValue)
			{
				GameController.Data.lifeFirst.value += GameController.Salud;
				Destroy(Other.gameObject);
			}

			if (GameController.Data.lifeSecond.value < GameController.Data.lifeSecond.maxValue) 
			{
				GameController.Data.lifeSecond.value += GameController.Salud;
				Destroy (Other.gameObject);
			}
		}
	}

	void UpdateAnimation()						// Funcion para actualizar las animaciones
	{
		Anim.SetBool ("Movement", isMoving);    // Le asignamos el valor de la variable isMoving al controlador de la animacion Movement
	}	
}
