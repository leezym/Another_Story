using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using Globales;

public class NextZone : MonoBehaviour {

	Scene NumberScene;
	Main main;
	bool ChargeImg = false;
	float TimeCount = 0;

	void Start () 
	{
		NumberScene = SceneManager.GetActiveScene ();
		main = GameObject.Find("Script Menu").GetComponent < Main> ();
	}

	void Update()
	{
		NumberScene = SceneManager.GetActiveScene ();

//		Debug.Log (TimeCount);
//		Debug.Log (ChargeImg);
//
//		if (ChargeImg) 
//		{
//			TimeCount = TimeCount + 0.5f;
//
//			if (TimeCount == 10f) 
//			{
//				Debug.Log ("Conteo");
//				main.Cargar.enabled = false;
//				Charge = false;
//				TimeCount = 0;
//			}
//		}
	}

	void OnTriggerEnter(Collider Other)
	{
		if (Other.gameObject.tag == "Player") 
		{
			main.Charge = true;
			main.Cargar.enabled = true;
			main.InfoScene = NumberScene;


//			if (NumberScene.name == "PrimerNivel(Patio)") 
//			{
//				SceneManager.LoadScene("PrimerNivel(Interior)");
//			}			
//
//			if (NumberScene.name == "PrimerNivel(Interior)")
//			{
//				SceneManager.LoadScene("SegundoNivel");
//			}
//
//			if (NumberScene.name == "SegundoNivel") 
//			{
//				SceneManager.LoadScene("SalaBoss");
//			}
		}
	}
}
