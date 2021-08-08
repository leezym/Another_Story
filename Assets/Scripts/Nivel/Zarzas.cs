using UnityEngine;
using System.Collections;
using Globales;

public class Zarzas : MonoBehaviour {

	Animator Anim;

	public int Count;
	public float TimeCount;

	BoxCollider Box;

	void Start () 
	{
		if (GetComponent<BoxCollider> ())
			Box = GetComponent<BoxCollider> ();
		else
			Debug.LogError ("Nesecita un box collider");

		if (GetComponentInChildren<Animator> ())   // Verificamos si hay componetes de animator en los objetos hijos
			Anim = GetComponentInChildren<Animator> ();	// Asignamos los componentes del animator a la variable
		else
			Debug.LogError ("Nesecita un Animator");	// En caso de no tener un componente de animator, enviamos un mensaje de error
	}

	void Update () 
	{
		UpdateAnimation ();

		TimeCount += Time.deltaTime;

		if (TimeCount > 0 && TimeCount < 7)
			Count = 1;
		
		if (TimeCount > 7 && TimeCount < 13)
		{
			Count = 2;
			Box.size = new Vector3 (1.0f, 1.0f, 1.0f);
		}

		if (TimeCount > 13 && TimeCount < 16) 
		{
			Count = 3;
			Box.size = new Vector3 (1.0f, 1.0f, 1.35f);
		}

		if (TimeCount > 16)
			TimeCount = 0;
	}

	void UpdateAnimation()
	{
		Anim.SetInteger ("Estado",Count);
	}

	void OnCollisionEnter(Collision Other)
	{
		if(Other.gameObject.tag == "Player")
		{
			GameController.Data.lifeFirst.value -= GameController.DamageZarza;
		}

		if (Other.gameObject.tag == "Principe")
		{
			GameController.Data.lifeSecond.value -= GameController.DamageZarza;
		}
	}
}
