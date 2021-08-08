using UnityEngine;
using System.Collections;

public class LookCamera : MonoBehaviour {

	public Transform Target;

	void Update ()
	{

		if(Target == null)
		{
			Target = GameObject.FindWithTag ("MainCamera").transform;
		}
		transform.LookAt (Target);
	}
}
