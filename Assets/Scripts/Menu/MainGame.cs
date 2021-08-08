using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Globales;

public class MainGame : MonoBehaviour {

	public Canvas pause, game_over, hud, hudBoss, victory, ending;
	public GameObject SpawnPlayer;
	public Texture Jimmy;
	public Texture Annie;
	GameObject Personaje;

	float TimeVictory;

	[HideInInspector]
	public bool showDead;
	[HideInInspector]
	public bool showDeadBoss;
	[HideInInspector]
	public bool MenuActive;

	Main mainP; 

	RawImage SpritePlayer;

	Scene InfoScene;

	public static MainGame data;

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
	}

	void Start()
	{
		showDead = false;
		pause.enabled = false;
		game_over.enabled = false;
		victory.enabled = false;
		hud.enabled = true;
		hudBoss.enabled = false;
		Time.timeScale = 1;

		mainP = GameObject.Find ("Script Menu").GetComponent<Main> ();

		SpritePlayer = GameObject.Find ("FirstPlayer").GetComponent<RawImage> ();

		InfoScene = SceneManager.GetActiveScene ();
	}

	void Update()
	{	
		InfoScene = SceneManager.GetActiveScene ();

		if (GameController.Data.lifeFirst.value == GameController.Data.lifeFirst.minValue || GameController.Data.lifeSecond.value == GameController.Data.lifeSecond.minValue) 
		{
			pause.enabled = false;
			hud.enabled = false;
			if (showDead) 
			{
				game_over.enabled = true;
				Time.timeScale = 0;
			}
		}
		else
			if (Input.GetKeyDown(KeyCode.Return))
			{
				Pausa ();
				MenuActive = true;
			}
		if(InfoScene.name == "SalaBoss")
		{
			hudBoss.enabled = true;

			if (GameController.Data.lifeBoss.value <= GameController.Data.lifeBoss.minValue) 
			{
				pause.enabled = false;
				hud.enabled = false;
				if(showDeadBoss)
				{
					ending.enabled = true;
					TimeVictory += Time.deltaTime;

					if(TimeVictory > 4f)
					{
						victory.enabled = true;
						Time.timeScale = 0;
					}
				}
			}
		}

		if(mainP.player == 1)
		{
			SpritePlayer.texture = Annie;
		}

		if(mainP.player == 2)
		{
			SpritePlayer.texture = Jimmy;
		}

		if (InfoScene.name != "Menu") 
		{
			if (GameController.Data.lifeFirst.value > GameController.Data.lifeFirst.minValue)
				hud.enabled = true;
		}
	}	

	void Pausa()
	{
		Time.timeScale = 0;
		pause.enabled = true;
		game_over.enabled = false;
		hud.enabled = false;
	} 

	public void Continuar()
	{
		Time.timeScale = 1;
		pause.enabled = false;
		game_over.enabled = false;
		hud.enabled = true;
		MenuActive = false;
	}

	public void Restart()
	{
		showDead = false;
		SceneManager.LoadScene ("PrimerNivel(Patio)");
		GameController.Data.lifeFirst.value = GameController.Data.lifeFirst.maxValue;
		GameController.Data.lifeSecond.value = GameController.Data.lifeSecond.maxValue;
		pause.enabled = false;
		game_over.enabled = false;
		victory.enabled = false;
		ending.enabled = false;
		hud.enabled = true;
		hudBoss.enabled = false;
		game_over.enabled = false;
		Time.timeScale = 1;
	}

	public void Sonido()
	{
		Time.timeScale = 0;
		pause.enabled = true;
		game_over.enabled = false;
		victory.enabled = false;
		hud.enabled = false;
		mainP.sonido.enabled = true;
	}

	public void Salir()
	{
		Application.Quit();
	} 

	public void Menu ()
	{
		SceneManager.LoadScene ("Menu");
		mainP.Main_menu ();
		showDead = false;
		pause.enabled = false;
		game_over.enabled = false;
		victory.enabled = false;
		ending.enabled = false;
		hud.enabled = false;
		hudBoss.enabled = false;
		MenuActive = false;
		showDeadBoss = false;
		Time.timeScale = 1;
		TimeVictory = 0;
		mainP.Reset ();
	}
}
