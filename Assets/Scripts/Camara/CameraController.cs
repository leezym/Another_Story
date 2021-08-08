using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform Target;

	[System.Serializable]
	public class PositionSettings
	{
		public Vector3 TargetPosOffSet = new Vector3 (0, 3.4f, 0);
		public float DistanceFromTarget = -8;
		public float ZoomSmooth = 10;
		public float ZoomStep = 2;
		public float MaxZoom = -2;
		public float MinZoom = -15;
		public bool SmoothFollow = true;
		public float Smooth = 0.05f;

		[HideInInspector]
		public float NewDistance = -8;
		[HideInInspector]
		public float AdjustmentDistance = -8;
	}

	[System.Serializable]
	public class OrbitSettings
	{
		public float xRotation = -20;
		public float yRotation = -180;
		public float MaxRotation = 25;
		public float MinRotation = -85;
		public float vOrbitSmooth = 150;
		public float hOrbitSmooth = 150;
	}

	[System.Serializable]
	public class InputSettings
	{
		public string MOUSE_ORBIT = "MouseOrbit";
		public string MOUSE_ORBIT_VERTICAL = "MouseOrbitVertical";
		public string ORBIT_HORIZONTAL_SNAP = "OrbitHorizontalSnap";
		public string ORBIT_HORIZONTAL = "OrbitHorizontal";
		public string ORBIT_VERTICAL = "OrbitVertical";
		public string ZOOM = "Mouse ScrollWheel";
	}

	[System.Serializable]
	public class DebugSettings
	{
		public bool DrawDesiredCollisionLines = true;
		public bool DrawAdjustedCollisionLines = true;
	}

	public PositionSettings position = new PositionSettings ();
	public OrbitSettings orbit = new OrbitSettings();
	public InputSettings input = new InputSettings ();
	public DebugSettings debug = new DebugSettings ();
	public collisionHandler collision = new collisionHandler ();

	Vector3 TargetPos = Vector3.zero;
	Vector3 Destination = Vector3.zero;
	Vector3 adjustedDestination = Vector3.zero;
	Vector3 CamVel = Vector3.zero;
	Vector3 previousMousePos = Vector3.zero;
	Vector3 currentMousePos = Vector3.zero;

	CharacterController CharController;

	float vOrbitInput;
	float hOrbitInput;
	float ZoomInpunt;
	float hOrbitSnapInpunt;
	float MouseOrbitInput;
	float VMouseOrbitInpunt;

	void Start()
	{
		SetCameraTarget (Target);

		vOrbitInput = hOrbitInput = ZoomInpunt = hOrbitSnapInpunt = MouseOrbitInput = VMouseOrbitInpunt = 0;

		MoveToTarget ();

		previousMousePos = currentMousePos = Input.mousePosition;

		collision.Initialize (Camera.main);
		collision.UpdateCameraClipPoints (transform.position, transform.rotation, ref collision.AdjustedCameraClipPoints);
		collision.UpdateCameraClipPoints (Destination, transform.rotation, ref collision.DesiredCameraClipPoints);
	}

	void SetCameraTarget(Transform t)
	{
		Target = t;

		if (Target != null) 
		{
			if (Target.GetComponent<CharacterController> ()) 
			{
				CharController = Target.GetComponent<CharacterController> ();
			} else
				Debug.LogError ("Asignele un PlayerController a la camara");
		} else
			Debug.LogError ("Asignele un Target a la camara");
	}

	void GetInput()
	{
		vOrbitInput = Input.GetAxisRaw (input.ORBIT_VERTICAL);
		hOrbitInput = Input.GetAxisRaw (input.ORBIT_HORIZONTAL);
		hOrbitSnapInpunt = Input.GetAxisRaw (input.ORBIT_HORIZONTAL_SNAP);
		MouseOrbitInput = Input.GetAxisRaw (input.MOUSE_ORBIT);
		VMouseOrbitInpunt = Input.GetAxisRaw (input.MOUSE_ORBIT_VERTICAL);
		ZoomInpunt = Input.GetAxisRaw (input.ZOOM);
	}

	void Update()
	{
		GetInput ();
		ZoomInOnTarget ();
	}

	void FixedUpdate()
	{
		//Movimiento Camara
		MoveToTarget ();
		//Rotacion Camara
		LookAtTarget ();
		// Player input orbit
		OrbitTarget ();
		//MouseOrbitTarget ();

		collision.UpdateCameraClipPoints (transform.position, transform.rotation, ref collision.AdjustedCameraClipPoints);
		collision.UpdateCameraClipPoints (Destination, transform.rotation, ref collision.DesiredCameraClipPoints);

		//draw debug lines
		for(int i = 0; i < 5; i++)
		{
			if(debug.DrawDesiredCollisionLines)
			{
				Debug.DrawLine (TargetPos, collision.DesiredCameraClipPoints[i], Color.white);
			}

			if(debug.DrawAdjustedCollisionLines)
			{
				Debug.DrawLine (TargetPos, collision.AdjustedCameraClipPoints [i], Color.green);
			}
		}	

		collision.CheckColliding (TargetPos); // Using raycast
		position.AdjustmentDistance = collision.GetAdjustedDistanceWhitRayFrom(TargetPos);
	}

	void MoveToTarget()
	{
		TargetPos = Target.position + Vector3.up * position.TargetPosOffSet.y + Vector3.forward * position.TargetPosOffSet.z + transform.TransformDirection (Vector3.right * position.TargetPosOffSet.x);
		Destination = Quaternion.Euler (orbit.xRotation, orbit.yRotation + Target.eulerAngles.y, 0) * -Vector3.forward * position.DistanceFromTarget;
		Destination += Target.position;

		if (collision.Colliding) 
		{
			adjustedDestination = Quaternion.Euler (orbit.xRotation, orbit.yRotation + Target.eulerAngles.y, 0) * -Vector3.forward * position.AdjustmentDistance;
			adjustedDestination += TargetPos;

			if (position.SmoothFollow)
			{
				//use smooth damp function
				transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref CamVel, position.Smooth);

			} else
				transform.position = adjustedDestination;
		} 
		else 
		{
			if (position.SmoothFollow)
			{
				//use smooth damp function
				transform.position = Vector3.SmoothDamp(transform.position, Destination, ref CamVel, position.Smooth);
			
			} else
				transform.position = Destination;
		}
	}

	void LookAtTarget()
	{
		Quaternion TargetRotation = Quaternion.LookRotation (TargetPos - transform.position);
		transform.rotation = Quaternion.Lerp (transform.rotation, TargetRotation, 100 * Time.deltaTime);
	}

	void OrbitTarget()
	{
		if(hOrbitSnapInpunt > 0)
		{
			orbit.yRotation = -100;
		}

		orbit.xRotation += -vOrbitInput * orbit.vOrbitSmooth * Time.deltaTime;
		orbit.yRotation += -hOrbitInput * orbit.hOrbitSmooth * Time.deltaTime;

		if(orbit.xRotation > orbit.MaxRotation)
		{
			orbit.xRotation = orbit.MaxRotation;
		}

		if(orbit.xRotation < orbit.MinRotation)
		{
			orbit.xRotation = orbit.MinRotation;
		}
	}

	void ZoomInOnTarget()
	{
		position.DistanceFromTarget += ZoomInpunt * position.ZoomSmooth * Time.deltaTime;

		if(position.DistanceFromTarget > position.MaxZoom)
		{
			position.DistanceFromTarget = position.MaxZoom;
		}

		if(position.DistanceFromTarget < position.MinZoom)
		{
			position.DistanceFromTarget = position.MinZoom;
		}
	}


	[System.Serializable]
	public class collisionHandler
	{
		public LayerMask collisionLayer;

		[HideInInspector]
		public bool Colliding = false;
		[HideInInspector]
		public Vector3[] AdjustedCameraClipPoints;
		[HideInInspector]
		public Vector3[] DesiredCameraClipPoints;

		Camera came;

		public void Initialize(Camera cam)
		{
			came = cam;
			AdjustedCameraClipPoints = new Vector3[5];
			DesiredCameraClipPoints = new Vector3[5];
		}

		public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
		{
			if(!came)
				return;

			// Limpiar el contenido del array
			intoArray = new Vector3[5];

			float z = came.nearClipPlane;
			float x = Mathf.Tan (came.fieldOfView / 3.41f) * z;
			float y = x / came.aspect;

			//top left
			intoArray[0] = (atRotation * new Vector3(-x,y,z)) + cameraPosition;// añanade una rotacion al punto relativo de la camera

			//top right
			intoArray[1] = (atRotation * new Vector3(x,y,z)) + cameraPosition;

			//bottom left
			intoArray[2] = (atRotation * new Vector3(-x,-y,z)) + cameraPosition;

			//bottom right
			intoArray[3] = (atRotation * new Vector3(x,-y,z)) + cameraPosition;

			//camera position
			intoArray[4] = cameraPosition - came.transform.forward;
		}

		bool CollisionDetectedAtClipPoints(Vector3[] ClipPoints, Vector3 FromPosition)
		{
			for (int i = 0; i < ClipPoints.Length; i++) 
			{
				Ray ray = new Ray(FromPosition,ClipPoints[i] - FromPosition);
				float Distance = Vector3.Distance(ClipPoints[i], FromPosition);

				if(Physics.Raycast(ray,Distance,collisionLayer))
				{
					return true;
				}	
			}
			return false;
		}

		public float GetAdjustedDistanceWhitRayFrom(Vector3 From)
		{
			float Distance = -1;

			for(int i = 0; i < DesiredCameraClipPoints.Length; i++)
			{
				Ray ray = new Ray(From,DesiredCameraClipPoints[i] - From);
				RaycastHit hit;

				if(Physics.Raycast(ray, out hit))
				{
					if (Distance == -1)
						Distance = hit.distance;
					else 
					{
						if (hit.distance < Distance)
							Distance = hit.distance;
					}
				}
			}

			if (Distance == -1)
				return 0;
			else
				return Distance;
		}

		public void CheckColliding(Vector3 TargetPosition)
		{
			if (CollisionDetectedAtClipPoints (DesiredCameraClipPoints, TargetPosition))
			{
				Colliding = true;
			} 
			else 
			{
				Colliding = false;
			}
		}
	}
}
