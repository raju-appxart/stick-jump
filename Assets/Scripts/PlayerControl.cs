using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Rigidbody2D))]
public class PlayerControl : MonoBehaviour {

	public GameObject playerVisual;
	public GameObject dieEffect;
	public Transform [] floorCheckers;
	public Vector2 jumpVelocity;
	public SpriteRenderer playerRenderer;
	public Text score, starCnt;
	Sprite normalPlayer, smallPlayer;
	Rigidbody2D playerBody;
	BoxCollider2D playerCollider;
	Vector3 initialPosition;
	bool isOnGround;
	public static PlayerState m_eCurrentState, m_ePrevState;
	int _screenMidPoint;

	public static int gameScore;

	RuntimePlatform platform = Application.platform;


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

	void Awake()
	{
		playerBody = GetComponent<Rigidbody2D>();
		playerBody.interpolation = RigidbodyInterpolation2D.Interpolate;
		playerBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
	}

	// Use this for initialization
	void Start () 
	{
		m_eCurrentState = PlayerState.Spwan;
		string playerId = "Player" + Main.Instance.playerID + "_";
		normalPlayer = Resources.Load <Sprite> ("Textures/Players/" + playerId+ 0) as Sprite;
		smallPlayer = Resources.Load <Sprite> ("Textures/Players/" + playerId+ 1) as Sprite;
		isOnGround = false;
		_screenMidPoint = Screen.width/2;
		initialPosition = new Vector3(-1, 0.5f, 0);
		playerCollider = transform.GetComponent<BoxCollider2D>();
		SetState(PlayerState.Init);
	}

	void Init()
	{
		Camera.main.orthographicSize = 5;
		playerVisual.SetActive(true);
		if(playerRenderer == null)
		{
			Debug.Log("Found");
			playerRenderer = transform.GetComponentInChildren<SpriteRenderer>();
		}
		transform.position = initialPosition;
		gameScore = 0;
		SetNormalPlayer();
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(m_eCurrentState)
		{
			case PlayerState.Spwan:
				break;
				
			case PlayerState.Init:
				break;
				
			case PlayerState.Gameplay:

			if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer) 
			{
				//					Touch myTouch = Input.GetTouch(0);
				
				Touch[] myTouches = Input.touches;
				for(int i = 0; i < Input.touchCount; i++)
				{
					//						if(myTouches[i].phase == TouchPhase.Began || myTouches[i].phase == TouchPhase.Ended)
					if(myTouches[i].phase == TouchPhase.Began)
					{
						if(myTouches[i].position.x > _screenMidPoint)
						{
							JumpNow();
						}
						else if(myTouches[i].position.x < _screenMidPoint)
						{
							DuckPlayer();
						}
					}
					else if(myTouches[i].phase == TouchPhase.Ended)
					{
						if(myTouches[i].position.x < _screenMidPoint)
						SetNormalPlayer();
					}

				}
				
			} 
			
			else if (platform == RuntimePlatform.WindowsEditor 
			         || platform == RuntimePlatform.OSXEditor) 
			{
				//mouse click
				if (Input.GetMouseButtonDown (0))// || Input.GetMouseButtonUp (0)) 
				{
					if(Input.mousePosition.x > _screenMidPoint)
					{
						JumpNow();
					}
					else if(Input.mousePosition.x < _screenMidPoint)
					{
						DuckPlayer();
					}
					
				}
				else if(Input.GetMouseButtonUp(0))
				{
					if(Input.mousePosition.x < _screenMidPoint)
					{
						SetNormalPlayer();
					}

				}
			}
				break;
				
			case PlayerState.Died:
				break;
		}

		score.text  = "" + gameScore;
	}


	void FixedUpdate () 
	{
		switch(m_eCurrentState)
		{
			case PlayerState.Gameplay:
			isOnGround = IsGrounded ();
			break;
		}
	}


	#region Jump Calculations
	private bool IsGrounded() 
	{
		//get distance to ground, from centre of collider (where floorcheckers should be)
		float dist = 0.9f ;//GetComponent<Collider>().bounds.extents.y + 0.1f;
//		dist = playerCollider.bounds.extents.y + 0.1f;
//		Debug.Log("dist = " + dist);
		//check whats at players feet, at each floorcheckers position
		foreach (Transform check in floorCheckers)
		{
//			RaycastHit hit;
//			if(Physics.Raycast(check.position, Vector3.down, out hit, dist + 1f))
//			RaycastHit2D hit = Physics2D.Raycast(check.position, Vector3.down,dist, 9);
			RaycastHit2D hit = Physics2D.Raycast(check.position, Vector2.down, dist, 1 << LayerMask.NameToLayer("Spinner"));
			if(hit.collider != null)
			{
				//yes our feet are on something
//				Debug.Log("True return = " + hit.transform.name );
				return true;
			}
		}
		//no none of the floorchecks hit anything, we must be in the air
//		Debug.Log("False return");
		return false;
	}


	void JumpNow()
	{
		if(isOnGround)
		{
			Main.Instance.PlayJumpSound();
			playerBody.velocity = new Vector2(0, 0f);
			playerBody.AddRelativeForce(jumpVelocity, ForceMode2D.Impulse);
		}

	}

	void SetNormalPlayer()
	{
		playerRenderer.sprite = normalPlayer;
		playerCollider.offset = new Vector2(0.2f, 0);
		playerCollider.size = new Vector2(1.2f, 1.4f);

	}

	void DuckPlayer()
	{
		Main.Instance.PlayDuckSound();
		playerRenderer.sprite = smallPlayer;
		playerCollider.offset = new Vector2(0.2f, -0.27f);
		playerCollider.size = new Vector2(1, 1);
	}

	#endregion


	#region Player State Transitions
	void SetState(PlayerState newState)
	{
		if(m_eCurrentState != newState)
		{
			m_ePrevState = m_eCurrentState;
			m_eCurrentState = newState;
			StateChange();
		}
		else
		{
			Debug.Log("m_eCurrentState = " + m_eCurrentState);
		}
	}

	void StateChange()
	{
		switch(m_eCurrentState)
		{
			case PlayerState.Spwan:
				break;

			case PlayerState.Init:
				Init();

				break;

			case PlayerState.Gameplay:
				starCnt.text = "" + Main.Instance.starsCnt;
				break;

			case PlayerState.Died:
				playerVisual.SetActive(false);
				Main.Instance.StopBGMusic();
				GameEventManager.TriggerGameOver();
				break;
		}
	}
	#endregion

	#region Collision Delegates
	void OnTriggerEnter2D(Collider2D collider)
	{
		if(m_eCurrentState != PlayerState.Gameplay)
			return;

		if(collider.gameObject.CompareTag(Constants.TAG_SPEED_UP))
		{
			collider.gameObject.SetActive(false);
			AdjustSpeed(Constants.SPEED_CHANGE);
		}
		else if(collider.gameObject.CompareTag(Constants.TAG_SPEED_DOWN))
		{
			collider.gameObject.SetActive(false);
			AdjustSpeed(-Constants.SPEED_CHANGE);
		}
		else if(collider.gameObject.CompareTag(Constants.TAG_BONUS))
		{
			collider.gameObject.SetActive(false);
			Main.Instance.starsCnt++;
			starCnt.text = "" + Main.Instance.starsCnt;
		}
		else if(collider.gameObject.CompareTag(Constants.TAG_DEATH))
		{
			ShowDieEffect(collider.transform.position);
			SetState(PlayerState.Died);
		}

	}

	void OnCollisionExit2D(Collision2D collider)
	{
		if(collider.gameObject.CompareTag(Constants.TAG_SPINNER))
		{
		}

	}
	#endregion

	#region Collection Impact
	void AdjustSpeed(float speedChange)
	{
//		if(speedChange > 0 && Camera.main.orthographicSize < 6)
//		{
//			Camera.main.orthographicSize += 0.25f;
//		}
//		else if(Camera.main.orthographicSize > 5)
//		{
//			Camera.main.orthographicSize -= 0.25f;
//		}
	


		SpinnerManager.rotationSpeed += speedChange;
		if(SpinnerManager.rotationSpeed <= Constants.MIN_SPEED)
		{
			SpinnerManager.rotationSpeed = Constants.MIN_SPEED;
		}
		else if(SpinnerManager.rotationSpeed > Constants.MAX_SPEED)
		{
			SpinnerManager.rotationSpeed = Constants.MAX_SPEED;
		}
	}

	void ShowDieEffect(Vector3 pos)
	{
		GameObject effect = Instantiate(dieEffect, pos, Quaternion.identity) as GameObject;
		Main.Instance.PlayBlastSound();

//		Destroy(effect, 0.8f);
	}


	#endregion

	#region Game Event Delegates
	void GameInit()
	{
		SetState(PlayerState.Init);
	}

	void GameStarted()
	{
		Invoke("PlayGame", 0.2f);
	}

	void PlayGame()
	{
		SetState(PlayerState.Gameplay);
	}
	
	void GameOver()
	{
		//TODO: show game over menu here
	}
	#endregion

}

public enum PlayerState
{
	None = -1,
	Spwan,
	Init,
	Gameplay,
	Died,
	Count
}
