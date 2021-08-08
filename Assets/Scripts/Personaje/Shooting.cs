using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shooting : MonoBehaviour {

	public float Speed = 100f;       // Indica la velocidad con la que se desplaza la bala

	float TimeShoot;				// indica el tiempo que hay entre cada disparo
	float TimeCharge;				// indica el tiempo de carga del disparo
	float NextSoundTime = 0;

	int Lvl;						// variable que me permite moverme entre el array de las balas

	bool Atack;						// Me indica si me encuentro disparando
	bool Charge;					// me indica si me encuentro cargando el disparo

	public Rigidbody [] Bullet;		// Array de balas del personaje

	public GameObject Spawn;		// Indica el punto de disparo del arma
	public GameObject ChargeEffect;	// Contiene las particulas del efecto de carga del personaje

	public AudioClip[] PlayerAudio;

	AudioSource Audio;				// Obtengo los componentes del auidosource del jugador

	Animator Anim;					// variable para obtener los componentes del animator

	PlayerController Player;		// variable para obtener los componentes del CharacterControlle

	Rigidbody RGB;					// variable para obtener los componentes del Rigidbody

	void Start()
	{
		TimeShoot = 0;				// inicializamos la variable en 0
		Lvl = 0;					// inicializamos la variable en falso
		Atack = false;				// inicializamos la variable en falso
		Charge = false;				// inicializamos la variable en falso

		if (GetComponentInChildren<Animator> ())			// Verificamos si hay componetes de animator en los objetos hijos
			Anim = GetComponentInChildren<Animator> ();		// Asignamos los componentes del animator a la variable
		else
			Debug.LogError ("Nesecita un Animator");		// En caso de no tener un componente de animator, enviamos un mensaje de error

		if (GetComponent<PlayerController> ())			// Verificamos si hay componetes de CharacterControler
			Player = GetComponent<PlayerController> ();	// Asignamos los componentes del CharacterControler a la variable
		else
			Debug.LogError ("Nesecita un Character Controller");	// En caso de no tener un componente de CharacterControler, enviamos un mensaje de error

		if (GetComponentInChildren<Rigidbody> ())			//Verificamos si hay componetes de un rigidbody
			RGB = GetComponentInChildren<Rigidbody> ();		// Obtenemos los componentes del rigidbody
		else
			Debug.LogError ("Nesecita un Animator");		//Enviamos un mensaje en caso de no encontrar el rigidbody

		if (GetComponent<AudioSource> ())   		
			Audio = GetComponent<AudioSource> ();	
		else
			Debug.LogError ("Nesecita un AudioSource");	
	}

	void Update()
	{
		UpdateAnimation ();

		if (Charge)														// Verificamos si se encuentra cargando el ataque el personaje
		{
			Player.Charge = true;										// Le indicamos a la variable de Charge en el charactercontroller que sea verdadera
			RGB.constraints = RigidbodyConstraints.FreezePosition;		// Congelamos los contraints de la posicion del rigidbody
		} 														// En caso de que no se encuentre cargando el ataque
		if(!Charge && !Player.Freezing)
		{
			Player.Charge = false;										// Le indicamos a la variable de Charge en el charactercontroller que sea falsa
			RGB.constraints &= ~RigidbodyConstraints.FreezePositionX;	// Descongelamos los contraints de la posicion X del rigidbody
			RGB.constraints &= ~RigidbodyConstraints.FreezePositionZ;	// Descongelamos los contraints de la posicion Z del rigidbody
		}
			
		TimeShoot += Time.deltaTime;									//Aumentamos el tiempo de disparo
	}

	void FixedUpdate()
	{
		if (Input.GetButton ("Fire1")) {								// Verificamos si se mantiene el boton de disparo presionado
			TimeCharge += Time.deltaTime;								// Aumentamos el tiempo de carga	
			Charge = true;												// Indicamos que nos encontramos cargando
			ChargeEffect.SetActive (true);								// Activamos los efectos de carga
		} else                                                          // En caso de no presionar el boton de disparo
		{
			Charge = false;												// indicamos que no se encuentra cargando
			ChargeEffect.SetActive (false);								// Desactivamos los efectos de carga
		}

		NextSoundTime += Time.deltaTime;
			
		if (Input.GetButtonUp ("Fire1")) {								//Verificamos si presionamos el boton de disparo
			Atack = true;												// indicamos que nos encontramos atacando

			if(NextSoundTime >= PlayerAudio [1].length)
			{
				Audio.clip = PlayerAudio[Random.Range(0,3)];
				Audio.Play ();
				NextSoundTime = 0;
			}


			if (TimeCharge > 0f && TimeCharge < 3f) {					// verificamos que el tiempo de carga se encuentre es ese rango de tiempo							
				Lvl = 0;												// si la condicion es verdadera indicamos que el nivel de disparo es 0 y el tiempo de carga 0
				TimeCharge = 0;
			} 

			if (TimeCharge > 3f) {										// si el tiempo de carga es mayor a 3 
				Lvl = 1;												// indicamos que el nivel de disparo es 1
				TimeCharge = 0;											// indicamos que su tiempo de carga es 0
			}

			if (TimeShoot > 1)											// verificamos si el tiempo de disparo es mayor a 1
			{										
				Invoke ("Shoot",1);									//invocamos la funcion de dispera 1 segundo despues de que la condicion se cumpla
				TimeShoot = 0;											//indicamos que el tiempo de carga es 0
			}
		} else 															// en caso de que no presione el boton de disparo
		{
			Atack = false;												// indicamos que no se encuentra atacando
		}
	}

	void Shoot()														// Funcion de disparo
	{
		Rigidbody Shoot = Instantiate (Bullet [Lvl], Spawn.transform.position, Spawn.transform.rotation) as Rigidbody; //instanciamos la bala teniendo en cuenta su nivel, la posicion de disparo y su rotacion

		Vector3 Fwr = transform.TransformDirection (Vector3.forward);			// le indicamos a travez de un vector 3 la direccion que ha de tomar la bala, en este caso le indicamos que vaya hacia adelante

		Shoot.AddForce (Fwr * Speed);											// le asignamos una fuerza a la bala mediante la direccion y la velocidad
		Lvl = 0;
	}

	void UpdateAnimation()												// funcion para actualizar las animaciones
	{
		Anim.SetBool ("Shoot", Atack);									// le asignamos el valor de la variable attack al controlador de la animacion de disparo
		Anim.SetFloat("Charge", TimeCharge);								// le asignamos el valor de la variable charge al controlador de la animacion de carga
		Anim.SetInteger("Nivel", Lvl);
	}
}
