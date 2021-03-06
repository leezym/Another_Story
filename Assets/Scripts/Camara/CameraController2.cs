using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraController2 : MonoBehaviour {
	Camera myCamera;
	Transform camTransform;
	public Transform pivot;
	public Transform character;

	public int charRotationSpeed = 4;
	public int camRotateSpeed = 10;
	public int vertSpeed = 3;
	public bool reverseVertical;


	float offset = -10;
	float farthestZoom = -15;
	float closestZoom = -2;
	float camFollow = 8;
	float camZoom = 1.75f;

	LayerMask mask;

	bool random = false;

	MainGame MG;

	// Use this for initialization
	void OnEnable () 
	{
		random = true;
	}

	void Start()
	{
		MG = GameObject.Find ("Script Menu Game").GetComponent<MainGame> ();
	}

	void Update () 
	{
		if(pivot == null)
			pivot = GameObject.FindWithTag ("Pivot").transform;	    // Buscamos el pivot que se encuentra en el personaje por medio del tag

		if(character == null)
			character = GameObject.FindWithTag ("Player").transform;   // Buscamos al personaje por medio del tag

		if(random)
		{
			myCamera = Camera.main;
			camTransform = myCamera.transform;
			camTransform.position = pivot.TransformPoint(Vector3.forward * offset);
			mask = 1 << LayerMask.NameToLayer("Clippable") | 0 << LayerMask.NameToLayer("NotClippable");
			random = false;
		}
			

//		if(Input.GetMouseButton(0))
//		{
//			//Camera Orbits the charcter
//			float hor = Input.GetAxis("Mouse X");
//			float vert = Input.GetAxis("Mouse Y");
//
//			if(!reverseVertical) vert *= -vertSpeed;
//			else vert *= vertSpeed;
//
//			hor *= camRotateSpeed;
//
//			//CLAMP Vertical Axis
//			float x = pivot.eulerAngles.x;
//			if(vert > 0 && x > 61 && x < 270 )  vert = 0;
//			else if (vert < 0 && x < 300 && x > 180 ) vert = 0;
//
//			pivot.localEulerAngles += new Vector3(vert, hor, 0);
//		} else 
		if(MG.MenuActive == false)
		{
			if(Input.GetMouseButton(1)){
				//Camera Orbits the charcter Vertically, and Character Rotates Horizontal
				//float hor = Input.GetAxis("Mouse X");
				float vert = Input.GetAxis("Mouse Y");
					
				if(!reverseVertical) vert *= -vertSpeed;
				else vert *= vertSpeed;

				//hor *= charRotationSpeed;

				//CLAMP Vertical Axis
				float x = pivot.eulerAngles.x;
				if(vert > 0 && x > 40 && x < 270 )  vert = 0;
				else if (vert < 0 && x < 350 && x > 180 ) vert = 0;

				pivot.localEulerAngles += new Vector3(vert, 0, 0);
				//character.Rotate(0, hor, 0);
			}
		
			//Mouse Zoom
			offset += Input.GetAxis("Mouse ScrollWheel") * camZoom;

			if(Input.GetButtonDown("ResetCamera")) pivot.localRotation = Quaternion.identity;
		}

		//Clamp Zoom
		if(offset > closestZoom) offset = closestZoom;
		else if (offset < farthestZoom) offset = farthestZoom;

		//Central Ray
		float unobstructed = offset;
		Vector3 idealPostion = pivot.TransformPoint(Vector3.forward * offset);

		RaycastHit hit;
		if(Physics.Linecast(pivot.position, idealPostion, out hit, mask.value)){
			unobstructed = -hit.distance + .01f;
		}

		//smooth
		Vector3 desiredPos = pivot.TransformPoint(Vector3.forward * unobstructed);
		Vector3 currentPos = camTransform.position;

		Vector3 goToPos = new Vector3(Mathf.Lerp(currentPos.x, desiredPos.x, camFollow), Mathf.Lerp(currentPos.y, desiredPos.y, camFollow), Mathf.Lerp(currentPos.z, desiredPos.z, camFollow));

		camTransform.localPosition = goToPos;
		camTransform.LookAt(pivot.position);

		//Viewport Bleed prevention
		float c = myCamera.nearClipPlane;
		bool clip = true;
		while(clip){
			Vector3 pos1 = myCamera.ViewportToWorldPoint(new Vector3(0,0,c));
			Vector3 pos2 = myCamera.ViewportToWorldPoint(new Vector3(.5f,0,c));
			Vector3 pos3 = myCamera.ViewportToWorldPoint(new Vector3(1,0,c));
			Vector3 pos4 = myCamera.ViewportToWorldPoint(new Vector3(0,.5f,c));
			Vector3 pos5 = myCamera.ViewportToWorldPoint(new Vector3(1,.5f,c));
			Vector3 pos6 = myCamera.ViewportToWorldPoint(new Vector3(0,1,c));
			Vector3 pos7 = myCamera.ViewportToWorldPoint(new Vector3(.5f,1,c));
			Vector3 pos8 = myCamera.ViewportToWorldPoint(new Vector3(1,1,c));

			Debug.DrawLine(camTransform.position, pos1, Color.yellow);
			Debug.DrawLine(camTransform.position, pos2, Color.yellow);
			Debug.DrawLine(camTransform.position, pos3, Color.yellow);
			Debug.DrawLine(camTransform.position, pos4, Color.yellow);
			Debug.DrawLine(camTransform.position, pos5, Color.yellow);
			Debug.DrawLine(camTransform.position, pos6, Color.yellow);
			Debug.DrawLine(camTransform.position, pos7, Color.yellow);
			Debug.DrawLine(camTransform.position, pos8, Color.yellow);

			if(Physics.Linecast(camTransform.position, pos1, out hit, mask.value)){
				// clip
			} else if(Physics.Linecast(camTransform.position, pos2, out hit, mask.value)){
				// clip
			} else if(Physics.Linecast(camTransform.position, pos3, out hit, mask.value)){
				// clip
			} else if(Physics.Linecast(camTransform.position, pos4, out hit, mask.value)){
				// clip
			} else if(Physics.Linecast(camTransform.position, pos5, out hit, mask.value)){
				// clip
			} else if(Physics.Linecast(camTransform.position, pos6, out hit, mask.value)){
				// clip
			} else if(Physics.Linecast(camTransform.position, pos7, out hit, mask.value)){
				// clip
			} else if(Physics.Linecast(camTransform.position, pos8, out hit, mask.value)){
				// clip
			} else clip = false;

			if(clip) camTransform.localPosition += camTransform.forward * c;
		}
	}

}