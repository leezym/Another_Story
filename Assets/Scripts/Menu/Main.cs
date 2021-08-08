using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Globales;

[RequireComponent (typeof(AudioSource))]

public class Main : MonoBehaviour {

	public Canvas main, select, mecanica, opciones, sonido, creditos, cinematicas, Cargar;
	public GameObject buttonBack, buttonRegresar, buttonNext;
	public Texture[] imagenes_Creditos;

	public Toggle toggle_musica, toggle_efectos, toggle_ambiente;
	public Slider slider_musica, slider_efectos, slider_ambiente; 
	public AudioSource clickSound;
	public AudioMixer masterMixer;

	public Sprite[] personajes, selectPersonajes;
	public GameObject annie, jimmy;
	public GameObject buttonGame;

	private AsyncOperation Asyn;

	[HideInInspector]
	public bool Charge = false;

	float TimeCharge = 0;

	[HideInInspector]
	public Scene InfoScene;

	[HideInInspector]
	public int player = 0;
	int cred = 0; 

	[HideInInspector]
	public bool movePlayer;

	public MovieTexture movie;

	public static Main data;

	bool Menu = false;

	void Awake()
	{	
		if (data == null) 
		{
			DontDestroyOnLoad (gameObject);
			data = this;
		}else 
			if(data != this)
			{
				Destroy (gameObject);
			}
		InfoScene = SceneManager.GetActiveScene ();
	}

	void Start()
	{
		Cinematicas ();

		// Seleccion
		annie.GetComponent<Image> ().sprite = personajes [0];
		jimmy.GetComponent<Image> ().sprite = personajes [1];
		buttonGame.GetComponent<Image> ().enabled = false;
		movePlayer = false;

		creditos.GetComponent<RawImage> ().texture = imagenes_Creditos[0];
		buttonBack.GetComponent<Image> ().enabled = false;
		buttonRegresar.GetComponent<Image> ().enabled = true;
		buttonNext.GetComponent<Image> ().enabled = true; 
	}

	void Update()
	{
		InfoScene = SceneManager.GetActiveScene ();

		if (!movie.isPlaying && !Menu) 
		{
			Menu = true;
			Main_menu();
			cinematicas.GetComponent<AudioSource> ().volume = 0;
			this.gameObject.GetComponent<AudioSource> ().Play ();
		}

		if (Charge)
		{
			//StartCoroutine (CargarLevel ());

			TimeCharge += 0.5f;

			if (TimeCharge == 10f) 
			{
				Charge = false;
				TimeCharge = 0;

				if (InfoScene.name == "Menu") 
				{
					SceneManager.LoadScene("PrimerNivel(Patio)");
					Cargar.enabled = false;
				}		
					
				if (InfoScene.name == "PrimerNivel(Patio)") 
				{
					SceneManager.LoadScene("PrimerNivel(Interior)");
					Cargar.enabled = false;
				}		

				if (InfoScene.name == "PrimerNivel(Interior)")
				{
					SceneManager.LoadScene("SegundoNivel");
					Cargar.enabled = false;
				}

				if (InfoScene.name == "SegundoNivel") 
				{
					SceneManager.LoadScene("SalaBoss");
					Cargar.enabled = false;
				}
			}
		}
	}

	IEnumerator CargarLevel()
	{
		//Asyn = SceneManager.LoadScene ("PrimerNivel(Patio)");
		SceneManager.LoadScene ("PrimerNivel(Patio)");


		while(!Asyn.isDone)
		{
			TimeCharge = Asyn.progress;

			if(TimeCharge >= 0.9f)
			{
				Cargar.enabled = false;
			}

			yield return null;
		}

		Asyn.allowSceneActivation = true;
		Charge = false;
		yield return Asyn;
	}

	void playClick()
	{
		clickSound.Play ();	
	}
		
	public void Reset ()
	{
		annie.GetComponent<Image> ().sprite = personajes [0];
		jimmy.GetComponent<Image> ().sprite = personajes [1];
		buttonGame.GetComponent<Image> ().enabled = false;
		player = 0;
	}

	public void Main_menu()
	{
		playClick ();
		main.enabled = true;
		select.enabled = false;
		mecanica.enabled = false;
		opciones.enabled = false;
		sonido.enabled = false;
		creditos.enabled = false;
		cinematicas.enabled = false;
	}

	public void Jugar()
	{
		playClick ();
		Time.timeScale = 1;
		this.gameObject.GetComponent<AudioSource> ().volume = 0;
		Charge = true;
		Cargar.enabled = true;
		//SceneManager.LoadScene(1);
	}

	public void Mecanicas()
	{
		playClick ();
		main.enabled = false;
		select.enabled = false;
		mecanica.enabled = true;
		opciones.enabled = false;
		sonido.enabled = false;
		creditos.enabled = false;
		cinematicas.enabled = false;
	}

	public void Opciones()
	{
		if (InfoScene.name == "Menu") {
			playClick ();
			main.enabled = false;
			select.enabled = false;
			mecanica.enabled = false;
			opciones.enabled = true;
			sonido.enabled = false;
			creditos.enabled = false;
			cinematicas.enabled = false;
		} else
		{
			sonido.enabled = false;
		}

	}

	public void Sonido()
	{
		playClick ();
		main.enabled = false;
		select.enabled = false;
		mecanica.enabled = false;
		opciones.enabled = false;
		sonido.enabled = true;
		creditos.enabled = false;
		cinematicas.enabled = false;
	}

	// MUSICA DE FONDO
	public void sounds()
	{
		if (!toggle_musica.isOn)
		{
			playClick ();
			slider_musica.value = slider_musica.minValue;
		}
		else if (toggle_musica.isOn)
		{
			playClick ();
			slider_musica.value = slider_musica.maxValue;
		}
	}

	public void sliderSounds(float levelSound)
	{
		
		masterMixer.SetFloat ("soundFondo", levelSound);
		levelSound = slider_musica.value;

		if (levelSound == slider_musica.minValue) 
		{
			toggle_musica.isOn = false;
		} else 
			if (levelSound != slider_musica.minValue) 
			{
				toggle_musica.isOn = true;
			}
	}

	// EFECTOS DE SONIDO
	public void effects()
	{
		if (!toggle_efectos.isOn)
		{
			playClick ();
			slider_efectos.value = slider_efectos.minValue;
		}
		else if (toggle_efectos.isOn)
		{
			playClick ();
			slider_efectos.value = slider_efectos.maxValue;
		}
	}

	public void slidereffects(float levelSound)
	{
		masterMixer.SetFloat ("soundEfecto", levelSound);
		levelSound = slider_efectos.value;

		if (levelSound == slider_efectos.minValue) 
		{
			toggle_efectos.isOn = false;
		} else 
			if (levelSound != slider_efectos.minValue) 
			{
				toggle_efectos.isOn = true;
			}
	}

	// MUSICA DE AMBIENTE
	public void environment()
	{
		if (!toggle_ambiente.isOn)
		{
			playClick ();
			slider_ambiente.value = slider_ambiente.minValue;
		}
		else if (toggle_ambiente.isOn)
		{
			playClick ();
			slider_ambiente.value = slider_ambiente.maxValue;
		}
	}
		
	public void sliderenvironment(float levelSound)
	{
		masterMixer.SetFloat ("soundAmb", levelSound);
		levelSound = slider_ambiente.value;

		if (levelSound == slider_ambiente.minValue) 
		{
			toggle_ambiente.isOn = false;
		} else 
			if (levelSound != slider_ambiente.minValue) 
			{
				toggle_ambiente.isOn = true;
			}
	}

	public void Creditos()
	{
		playClick ();
		main.enabled = false;
		select.enabled = false;
		mecanica.enabled = false;
		opciones.enabled = false;
		sonido.enabled = false;
		creditos.enabled = true;
		cinematicas.enabled = false;
		cred = 0; 
		buttonBack.GetComponent<Image> ().enabled = false;
		buttonRegresar.GetComponent<Image> ().enabled = true;
		buttonNext.GetComponent<Image> ().enabled = true;
	}

	public void NextCreditos()
	{
		playClick ();
		buttonBack.GetComponent<Image> ().enabled = true;
		buttonRegresar.GetComponent<Image> ().enabled = false;

		if(cred < imagenes_Creditos.Length-1)
			cred++;

		if (cred == imagenes_Creditos.Length - 1) {
			buttonNext.GetComponent<Image> ().enabled = false;
		}
		
		creditos.GetComponent<RawImage> ().texture = imagenes_Creditos [cred];
	}

	public void BackCreditos()
	{
		playClick ();

		if (cred > 0)
			cred--;

		if (cred > 0) 
		{
			buttonBack.GetComponent<Image> ().enabled = true;
			buttonRegresar.GetComponent<Image> ().enabled = false;
			buttonNext.GetComponent<Image> ().enabled = true;
		} else {
			buttonBack.GetComponent<Image> ().enabled = false;	
			buttonRegresar.GetComponent<Image> ().enabled = true;	
		}
		
		creditos.GetComponent<RawImage> ().texture = imagenes_Creditos [cred];
	}

	public void Salir()
	{
		playClick ();
		Application.Quit();
	}  	  

	public void Select()
	{
		playClick ();
		main.enabled = false;
		select.enabled = true;
		mecanica.enabled = false;
		opciones.enabled = false;
		sonido.enabled = false;
		creditos.enabled = false;
		cinematicas.enabled = false;
	}

	public void JugarPlayer()
	{
		if (annie.GetComponent<Image> ().sprite == selectPersonajes [0])
		{
			playClick ();
			player = 1; 
			main.enabled = false;
			select.enabled = false;
			mecanica.enabled = false;
			opciones.enabled = false;
			sonido.enabled = false;
			creditos.enabled = false;
			cinematicas.enabled = false;
		}

		if (jimmy.GetComponent<Image> ().sprite == selectPersonajes [1])
		{
			playClick ();
			player = 2;
			main.enabled = false;
			select.enabled = false;
			mecanica.enabled = false;
			opciones.enabled = false;
			sonido.enabled = false;
			creditos.enabled = false;
			cinematicas.enabled = false;
		}		
	}

	public void OnClickSelectA () 
	{			
		if (annie.GetComponent<Image>().sprite == personajes [0]) 
		{
			playClick ();
			annie.GetComponent<Image> ().sprite = selectPersonajes [0];
			jimmy.GetComponent<Image> ().sprite = personajes [1];
			buttonGame.GetComponent<Image> ().enabled = true;
		}
		else
			if (annie.GetComponent<Image>().sprite == selectPersonajes [0]) 
			{
				playClick ();
				annie.GetComponent<Image> ().sprite = personajes [0];
				buttonGame.GetComponent<Image> ().enabled = false;
			}	
	}	

	public void OnClickSelectJ () 
	{
		if (jimmy.GetComponent<Image>().sprite == personajes [1]) 
		{
			playClick ();
			jimmy.GetComponent<Image> ().sprite = selectPersonajes [1];
			annie.GetComponent<Image> ().sprite = personajes [0];
			buttonGame.GetComponent<Image> ().enabled = true;
		}
		else
			if (jimmy.GetComponent<Image>().sprite == selectPersonajes [1]) 
			{
				playClick ();
				jimmy.GetComponent<Image> ().sprite = personajes [1];
				buttonGame.GetComponent<Image> ().enabled = false;
			}			
	}

	void Cinematicas()
	{
		movie.Play ();
		main.enabled = false;
		select.enabled = false;
		mecanica.enabled = false;
		opciones.enabled = false;
		sonido.enabled = false;
		creditos.enabled = false;
		cinematicas.enabled = true;
	}

	public void Skip()
	{
		//playClick ();
		Menu = true;
		Main_menu();
		cinematicas.GetComponent<AudioSource> ().volume = 0;
		this.gameObject.GetComponent<AudioSource> ().Play ();
	}
}