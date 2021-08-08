using UnityEngine;
using System.Collections;
using Globales;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AI_Principe : MonoBehaviour {

	NavMeshAgent agent;            				//Variable para obtener los componentes del NavMeshAgent

	Animator Anim;								//variable para obtener los componentes del animator

	Scene InfoScene;							// Permite obtener componentes de la escena

	Transform myTransform;						// Almacena la posicion del principe
	Vector3 LastPosition;						// almacena la ultima posicion del principe

	bool isMoving;								// nos indica si el principe se encuentra en movimiento
	bool isFocus;								// nos indica si el principe tiene asignado a un enemigo como target
	bool isAttack;								// nos indica si el principe se encuentra atacando
	bool isLife;								// nos indica si el principe se encuentra con vida
	bool isDeath;

	public GameObject Partner;					//Almacena al jugador principal
	public GameObject Target;					//almacena al enemigo al cual se encuentra haciendo fucus
	public GameObject Espada;					//almacena la espada del principe

	Quaternion RotationTarget;
	float RotateForce = 1f;
	float NextSoundTime = 0;

	public float DistancePartner;				//indica la distancia entre el principe y el jugador
	public float DistanceTarget;				//indica la distancia entre el principe y el enemigo

	int NumClip;

	public AudioClip []PrincipeAudio;
	public AudioClip[] FootSound;
	public AudioClip DeathAudio;

	AudioSource Sound;
	public AudioSource FootsSound;

	MainGame showdead;
	float timeShow = 0;
	bool showDead;
	public PlayerController pj;

	void Awake()
	{
		GameController.Data.lifeSecond = GameObject.FindGameObjectWithTag ("saludSecond").GetComponent<Slider> ();
		InfoScene = SceneManager.GetActiveScene ();
	}

	void Start () 
	{		
		// Accedes para mostrar el menu Game Over
		showdead = GameObject.Find ("Script Menu Game").GetComponent<MainGame>();
		showDead = false;

		if (InfoScene.name == "PrimerNivel(Patio)") {
			GameController.Data.lifeSecond.value = GameController.Data.lifeSecond.maxValue; // Indica la cantidad de vida del jugador
			NumClip = 0;
		} else 
		{
			GameController.Data.lifeSecond.value = GameController.Data.lifeSecond.value;
			NumClip = 1;
		}
			

		myTransform = this.transform;			//le asignamos a la variable la posicion que tiene el principe
		LastPosition = myTransform.position;	// asignamos la anterior posicion del principe

		GameController.Data.LastVitSecond = GameController.Data.lifeSecond.value;

		//inicializamos las variables en falso
		isMoving = false;						
		isFocus = false;
		isAttack = false;

		if (Partner == null)
			Partner = GameObject.FindWithTag ("Player");

		if (GetComponentInChildren<NavMeshAgent> ())				// Verificamos si posee componetes de navmesh
			agent = GetComponentInChildren<NavMeshAgent> ();		//Asignamos  los componentes a la variable
		else
			Debug.LogError ("Nesecita un Nav Mesh Agent");			//En caso de que no poseea componentes enviamos un mesaje de error

		if (GetComponentInChildren<Animator> ())					// Verificamos si posee componetes de animator
			Anim = GetComponentInChildren<Animator> ();				//Asignamos  los componentes a la variable
		else
			Debug.LogError ("Nesecita un animator");				//En caso de que no poseea componentes enviamos un mesaje de error

		if (GetComponent<AudioSource> ())   		
			Sound = GetComponent<AudioSource> ();	
		else
			Debug.LogError ("Nesecita un AudioSource");	

	}


	void Update () 
	{
		UpdateAnimation ();
		NextSoundTime += Time.deltaTime;

		if (GameController.Data.lifeSecond.value > GameController.Data.lifeSecond.maxValue)
			GameController.Data.lifeSecond.value = GameController.Data.lifeSecond.maxValue;

		if (Partner == null)
			Partner = GameObject.FindWithTag ("Player");

		if (pj == null)
			pj = Partner.GetComponent<PlayerController> ();

		if (GameController.Data.lifeSecond.value > 0)
			isLife = true;

		if (GameController.Data.lifeSecond.value < 1)
			isLife = false;

		if (!isMoving)
		{
			FootsSound.clip = FootSound [NumClip];
			FootsSound.Play ();
		}

		if (isLife)													//Verificamos que el principe se encuentre vivo
		{
			// En caso de que se cumpla la condicion llamamos las funciones de movimiento y distancia
			Movement ();
			Distance ();
			Rotation ();

			if (GameController.Data.lifeSecond.value != GameController.Data.LastVitSecond) 
			{
				if (pj.Curar == false)
				{
					if(NextSoundTime >= PrincipeAudio[1].length)
					{
						Sound.clip = PrincipeAudio [Random.Range (0, 2)];
						Sound.Play ();
						NextSoundTime = 0;
					}
				}

				if (pj.Curar == true)
				{
					if(NextSoundTime >= PrincipeAudio[1].length)
					{
						Sound.clip = PrincipeAudio [2];
						Sound.Play ();
						NextSoundTime = 0;
					}
				}
			}

			GameController.Data.LastVitSecond = GameController.Data.lifeSecond.value;
		} 

		if(GameController.Data.lifeSecond.value <= 0 && !isDeath)
		{
			DeathPrincipe ();
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

	void LateUpdate()
	{
		if (isAttack)																//verificamos si se encuentra atacando
			Espada.gameObject.GetComponent<BoxCollider> ().enabled = true;			//si la condicion se cumple activamos el boxcollider que se encuentra ubicado en la espada, esto se hace con el objetivo de que el arma colisione y cause daño solo cuando el principe ataca
		else
			Espada.gameObject.GetComponent<BoxCollider> ().enabled = false;			//en caso de que sea falsa la condicion mantenemos desactivado el boxcollider para no ocasionar daño mientras no se ataca

		if (Target == null)															// verificamos si el target es nulo
		{
			//En caso de que el target sea nulo indicamos que el principe no tiene un objetivo ni tampoco se encuentra atacando
			isFocus = false;
			isAttack = false;
		}
	}

	void DeathPrincipe()
	{
		isDeath = true;

		Sound.clip = DeathAudio;
		Sound.Play ();

		isMoving = false;						
		isFocus = false;
		isAttack = false;
		showDead = true;
		Anim.CrossFade ("Death",0);
	}

	void Movement()														// funcion de movimiento
	{
		if (!isFocus) 													// verificamos si tiene un objetivo
		{
			agent.stoppingDistance = 8f;								// en caso de que no posea un objetivo, le indicaremos que la distancia que hay entre el principe y el destino sera de 8
			agent.SetDestination (Partner.transform.position);					// le asignamos al jugador como destino
		} else
			if(isFocus == true && DistancePartner < 25f && Target != null)				// verificamos que tenga un objetivo y la disatancia entre el principe y el jugador sea menor a 25
			{
				agent.stoppingDistance = 5f;							//si se cumple la condicion,le indicaremos que la distancia a la cual se detendra sera de 5
				agent.SetDestination(Target.transform.position);		//le asignamos como destino a la variable de target
			}

		if (myTransform.position != LastPosition)						// Verificamos que la posicion actual sea diferente de la posicion almacenada en la variable lastPosition
			isMoving = true;											// si se cumple dicha condicion, le indicamos que isMoving es verdadera
		else
			isMoving = false;											//En caso de que no se cumpla, indicamos que isMoving es falso
		
		LastPosition = myTransform.position;							//le indicamos que lastPsition sera igual a la posicion actual
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

	void Distance()
	{
		DistancePartner = Vector3.Distance (Partner.transform.position, myTransform.position);	// Calculamos la distancia entre el principe y el jugador a travez de un vector3.Distance, el cual nos retorna la distancia entre el punto a y el punto b

		if(DistancePartner > 25f)														// Verificamos si la disntancia entre el principe y el jugador es mayor a 25
			isFocus = false;															// si es verdadera la condicion, el principe dejara de tener un objetivo e indicaremos que isFocus es faslo

		if (Target != null)																// Verificamos si el target no es nulo
		{
			DistanceTarget = Vector3.Distance (Target.transform.position, myTransform.position);	//calculamos la distancia entre el principe y el target

			if (DistanceTarget < 5.5f)													//verificamos si la distancia entre el target y el principe es menor a 5.5
				isAttack = true;														// si se cumple la condicion indicamos al principe que ataque 
			else
				isAttack = false;														// de lo contrario, le indicamos al principe que deje de atacar
		} else
		{
			DistancePartner = Vector3.Distance (Partner.transform.position, myTransform.position);
			isFocus = false;
			isAttack = false;
		}
	}

	void UpdateAnimation() 				// funcion para actualizar las animaciones
	{
		Anim.SetBool ("Move",isMoving);
		Anim.SetBool ("Attack", isAttack);
	}
		
	void OnTriggerEnter(Collider Other)				// verificamos que tipo de enemigo ingresa en el rango de deteccion del principe
	{
		if(Other.gameObject.tag == "Brote")			
		{
			Target = Other.gameObject;				// si un objeto entra dentro de nuestro rango y posee el tag de enemigo, le asignamos ese objeto como el nuevo target para el personaje
			isFocus = true;
		}

		if(Other.gameObject.tag == "Soldado")			
		{
			Target = Other.gameObject;				// si un objeto entra dentro de nuestro rango y posee el tag de enemigo, le asignamos ese objeto como el nuevo target para el personaje
			isFocus = true;
		}

		if(Other.gameObject.tag == "SoldadoP")			
		{
			Target = Other.gameObject;				// si un objeto entra dentro de nuestro rango y posee el tag de enemigo, le asignamos ese objeto como el nuevo target para el personaje
			isFocus = true;
		}

		if(Other.gameObject.tag == "Boss")			
		{
			Target = Other.gameObject;				// si un objeto entra dentro de nuestro rango y posee el tag de Boss, le asignamos ese objeto como el nuevo target para el personaje
			isFocus = true;
		}

		if (Other.gameObject.tag == "Explosion") 
			GameController.Data.lifeSecond.value -= GameController.DamageExplosion;	
	}

	void OnTriggerExit(Collider Other)
	{
		if(Other.gameObject.tag == "Brote")
		{	
			Target = null;							// si un objeto sale de nuestro rango y posse el tag de enemigo, le indicamos que deje de utilizarlo como objetivo
			isFocus = false;
			DistanceTarget = 0;
		}

		if(Other.gameObject.tag == "Soldado")
		{	
			Target = null;							// si un objeto sale de nuestro rango y posse el tag de enemigo, le indicamos que deje de utilizarlo como objetivo
			isFocus = false;
			DistanceTarget = 0;
		}

		if(Other.gameObject.tag == "SoldadP")
		{	
			Target = null;							// si un objeto sale de nuestro rango y posse el tag de enemigo, le indicamos que deje de utilizarlo como objetivo
			isFocus = false;
			DistanceTarget = 0;
		}

		if(Other.gameObject.tag == "Boss")
		{	
			Target = null;							// si un objeto sale de nuestro rango y posse el tag de Boss, le indicamos que deje de utilizarlo como objetivo
			isFocus = false;
			DistanceTarget = 0;
		}
	}
}
