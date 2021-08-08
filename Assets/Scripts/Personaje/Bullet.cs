using UnityEngine;
using System.Collections;
using Globales;

public class Bullet : MonoBehaviour {

	// Variables para obtener los componentes de los diferentes enemigos a los cuales la bala colisiona
	AI_Brote Bro;
	AI_PatrolSoldado PS;
	AI_Soldado Sol;
	AI_Boss Boss;

	void Update () 
	{
		Destroy (gameObject, 3f);  // Destruimos el objeto despues de 3 seg
	}


	// Verificamos las colisiones de la bala
	void OnCollisionEnter(Collision Other)
	{
		if(Other.gameObject.tag == "Escenario")
		{
			Destroy (gameObject);										// Si la bala colisiona con algun componente de escenario, la destruimos
		}

		if(Other.gameObject.tag == "Brote")
		{
			Bro = Other.gameObject.GetComponentInParent<AI_Brote> (); 	// Obtenemos los componentes del brote
			Bro.Vit -= GameController.DamagePlayer; 					// Accedemos a la vida del enemigo y disminuimos segun la cantidad de daño de la bala
			Destroy (gameObject);										//destruimos la bala
		}

		if(Other.gameObject.tag == "SoldadoP")
		{
			PS = Other.gameObject.GetComponentInParent<AI_PatrolSoldado> ();	// Obtenemos los componentes del Soldado patruya
			PS.Vit -= GameController.DamagePlayer;								// Accedemos a la vida del enemigo y disminuimos segun la cantidad de daño de la bala
			Destroy (gameObject);												//destruimos la bala
		}

		if(Other.gameObject.tag == "Soldado")
		{
			Sol = Other.gameObject.GetComponentInParent<AI_Soldado> ();			// Obtenemos los componentes del Soldado
			Sol.Vit -= GameController.DamagePlayer;								// Accedemos a la vida del enemigo y disminuimos segun la cantidad de daño de la bala
			Destroy (gameObject);												//destruimos la bala
		}

		if(Other.gameObject.tag == "Boss")
		{
			Boss = Other.gameObject.GetComponentInParent<AI_Boss> ();			// Obtenemos los componentes del Jefe
			GameController.Data.lifeBoss.value -= (GameController.DamagePlayer - 18);							// Accedemos a la vida del enemigo y disminuimos segun la cantidad de daño de la bala
			Destroy (gameObject);												//destruimos la bala
		}
	}
}
