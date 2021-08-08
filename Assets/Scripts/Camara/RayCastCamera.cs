using UnityEngine;
using System.Collections;

public class RayCastCamera : MonoBehaviour {

	public static float RayDistance = 5f;

	void Update () 
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward),out hit))
		{
			RayDistance = hit.distance;
		}
	}
}
