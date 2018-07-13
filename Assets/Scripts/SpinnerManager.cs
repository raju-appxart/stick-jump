using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SpinnerManager : MonoBehaviour {

	/// <summary>
	/// The rotation speed.
	/// </summary>
	public static float rotationSpeed = 0.8f; 

	public GameObject obstacle;

	/// <summary>
	/// The radius if spinner.
	/// </summary>
	const float radius = 3.0f;

	/// <summary>
	/// The spwan angle of obstacle.
	/// </summary>
	const float spwanAngle  = 270;

	/// <summary>
	/// The spwan position of obstacles.
	/// </summary>
	Vector3 spwanPosition;

	/// <summary>
	/// The spwan rotation.
	/// </summary>
	Quaternion spwanRotation;

	private List<GameObject> currentObstacles;


	void OnEnable()
	{
		GameEventManager.GameInit += GameInit;
		GameEventManager.GameStart += GameStarted;
		GameEventManager.GameOver += GameOver;
	}
	
	void OnDisable()
	{
		GameEventManager.GameInit -= GameInit;
		GameEventManager.GameStart -= GameStarted;
		GameEventManager.GameOver -= GameOver;
	}


	// Use this for initialization
	void Start () 
	{
		currentObstacles = new List<GameObject>();
		CalculateRequiredParameters();
		GameInit();
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(PlayerControl.m_eCurrentState)
		{
			case PlayerState.Gameplay:
				transform.RotateAround(transform.position, Vector3.forward, rotationSpeed);
				break;
		}

	}

	/// <summary>
	/// Rotates the spinner.
	/// </summary>
	void RotateSpinner()
	{
		//Spinner 
	}

	void CalculateRequiredParameters()
	{
		spwanPosition = transform.position + new Vector3(radius*Mathf.Cos(Mathf.Deg2Rad*spwanAngle),
		                                                 radius*Mathf.Sin(Mathf.Deg2Rad*spwanAngle), 0);

		spwanRotation = Quaternion.Euler(new Vector3(0, 0, spwanAngle));
	}

	int i = 0;
	void NewObstacle()
	{
		GameObject newObs = Instantiate(obstacle, spwanPosition, spwanRotation) as GameObject;
		newObs.transform.parent = transform;
		newObs.name = "OBS_" + i;
		i++;
		currentObstacles.Add(newObs);
	}

	#region Game Event Delegates
	void GameInit()
	{
		transform.rotation = Quaternion.Euler(Vector3.zero);
		int themeIndex = 0;
		if(Main.Instance.totalGames%3 == 0)
		{
			themeIndex = 0;
		}
		else 
		{
			themeIndex = Main.Instance.totalGames%3;
		}
		
		transform.GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("Textures/Theme/" +"PF_" + themeIndex) as Sprite;
	}

	void GameStarted()
	{
		rotationSpeed = Constants.START_SPEED;
		InvokeRepeating("NewObstacle", 0.25f, 1.55f);
	}

	void GameOver()
	{
		CancelInvoke("NewObstacle");
		for(int i = 0; i < currentObstacles.Count; i ++)
		{
			Destroy(currentObstacles[i]);
		}
		currentObstacles.Clear();
	}
	#endregion

}
