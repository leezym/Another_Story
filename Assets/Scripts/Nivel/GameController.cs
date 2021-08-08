using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Globales
{
	public class GameController : MonoBehaviour {

		public static GameController Data;

		public static int DamagePlayer = 50;
		public static int DamagePartner = 15;

		public static int DamageBrote = 3;
		public static int DamageSoldado = 5;
		public static int DamageBoss = 8;
		public static int DamageExplosion = 15;

		public static int DamageTrampa = 3;
		public static int DamageZarza = 3;

		public static int Salud = 30;

		public float LastVitFirst;
		public Slider lifeFirst;

		public float LastVitSecond;
		public Slider lifeSecond;

		public float LastVitBoss;
		public Slider lifeBoss;

		void Awake()
		{
			if (Data == null) 
			{
				DontDestroyOnLoad (gameObject);
				Data = this;
			}else 
				if(Data != this)
				{
					Destroy (gameObject);
				}
		}
	}	
}
