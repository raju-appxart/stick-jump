using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour, IAnimationDelegate {

	public GameObject obsRoot, demoHero, stand;
	public GameObject obs_1, obs_2, obs_3;
	public GameObject jumpHide1, jumpHide2, duckHide1, duckHide2;
	public RectTransform finalPanel, howToPanel, jumpAnim, duckAnim, gotItAnim;

	public Sprite standPos, duckPos;
	SpriteRenderer playerRender;

	Vector3 rootPos, initialPosOfHero;
	int _screenMidPoint;
	bool shallRotate, isWaitOfJump, isWaitOfDuck, isJumpComplete, isDuckComplete, isWaitComboJump, isWaitComboDuck, isHalfJumpDone, isHalfDuckDone, isTutorialComplete;

	MoveTo jumpMove, backToGnd, halfJump, backToGndSlow;

	// Use this for initialization
	void Start () 
	{
		initialPosOfHero = demoHero.transform.position;
		playerRender = demoHero.transform.GetComponent<SpriteRenderer>();
		rootPos = obsRoot.transform.position;
		_screenMidPoint = Screen.width/2;
		shallRotate = false;
		isWaitOfJump = isWaitOfDuck = isJumpComplete = isDuckComplete = isTutorialComplete = false;
		Invoke("Rotate", 0.5f);
		obs_2.SetActive(false);

		jumpMove = AnimationManager.MoveTo(demoHero, new Vector3(-0.28f, 3.8f, 0), 1.5f , 1, 0);
		backToGnd = AnimationManager.MoveTo(demoHero, initialPosOfHero, 1.2f , 1, 0);
		backToGndSlow = AnimationManager.MoveTo(demoHero, initialPosOfHero, 1.0f , 1, 0);
		halfJump = AnimationManager.MoveTo(demoHero, new Vector3(-0.28f, 3.6f, 0), 1.5f , 1, 0);
	}

	void Rotate()
	{
		shallRotate = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		AnimationManager.Update();

		if(shallRotate)
		{
			obsRoot.transform.RotateAround(rootPos, Vector3.forward, 1.2f);
		}

		if(obsRoot.transform.rotation.eulerAngles.z >= 50 && !isWaitOfJump)
		{
			//Wait for Jump input
			shallRotate = false;
			isWaitOfJump = true;
			jumpAnim.gameObject.SetActive(true);
		}

		if(obsRoot.transform.rotation.eulerAngles.z > 90 && isWaitOfJump && !isJumpComplete)
		{
			isJumpComplete = true;
			jumpHide1.SetActive(false);
			jumpHide2.SetActive(false);
			obs_2.SetActive(true);
//			shallRotate = false;
		}

		if(obsRoot.transform.rotation.eulerAngles.z >= 150 && !isWaitOfDuck)
		{
			//Wait for Jump input
			shallRotate = false;
			isWaitOfDuck = true;
			duckAnim.gameObject.SetActive(true);
		}

		if(obsRoot.transform.rotation.eulerAngles.z > 180 && isWaitOfDuck && !isDuckComplete)
		{
			isDuckComplete = true;
			obs_1.SetActive(false);
			duckHide1.SetActive(false);
			obs_3.SetActive(true);

//			shallRotate = false;

		}

		if(obsRoot.transform.rotation.eulerAngles.z >= 225 && !isWaitComboJump)
		{
			//Wait for Jump input
			shallRotate = false;
			isWaitComboJump = true;
			jumpAnim.gameObject.SetActive(true);
		}

		if(obsRoot.transform.rotation.eulerAngles.z >= 250 && isWaitComboJump && !isHalfJumpDone)
		{
			isHalfJumpDone = true;
			isWaitComboDuck = true;
			shallRotate = false;
			duckAnim.gameObject.SetActive(true);
		}

		if(obsRoot.transform.rotation.eulerAngles.z > 270 && isWaitComboDuck && !isHalfDuckDone)
		{
			isHalfDuckDone = true;
			duckHide2.SetActive(false);

		}

		if(obsRoot.transform.rotation.eulerAngles.z > 330 && !isTutorialComplete)
		{
//			shallRotate = false;
			obsRoot.SetActive(false);
			isTutorialComplete = true;
			obs_2.SetActive(false);
			gotItAnim.gameObject.SetActive(true);
			Invoke("Final", 2);
		}


		if (Input.GetMouseButtonDown (0))// || Input.GetMouseButtonUp (0)) 
		{
			if(Input.mousePosition.x > _screenMidPoint && isWaitOfJump && !isJumpComplete)
			{
				JumpNow();
			}
			if(Input.mousePosition.x < _screenMidPoint && isWaitOfDuck && !isDuckComplete)
			{
				DuckPlayer();
			}

			if(Input.mousePosition.x > _screenMidPoint && isWaitComboJump && !isHalfJumpDone)
			{
				HalfJumpNow();
			}

			if(Input.mousePosition.x < _screenMidPoint && isWaitComboDuck && !isHalfDuckDone)
			{
				ComboDuckPlayer();
			}
			
		}
		else if(Input.GetMouseButtonUp(0))
		{
			if(Input.mousePosition.x < _screenMidPoint)
			{
//				SetNormalPlayer();
			}
			
		}
	}

	void Final()
	{
		howToPanel.gameObject.SetActive(false);
		demoHero.SetActive(false);
		stand.SetActive(false);
		finalPanel.gameObject.SetActive(true);
	}

	void JumpNow()
	{

		jumpAnim.gameObject.SetActive(false);
		AnimationManager.Play(jumpMove, this, 0, "Jump");

		shallRotate = true;
	}

	void HalfJumpNow()
	{
		jumpAnim.gameObject.SetActive(false);
		AnimationManager.Play(halfJump, this, 0, "HalfJump");
		shallRotate = true;
	}

	void DuckPlayer()
	{
		duckAnim.gameObject.SetActive(false);
		playerRender.sprite = duckPos;
		shallRotate = true;
		Invoke("Normal",1.0f);
	}

	void Normal()
	{
		playerRender.sprite = standPos;
		if(isHalfDuckDone)
		{
			AnimationManager.Play(backToGndSlow, this, 0f, "ToGndSlow");
		}
	}

	void ComboDuckPlayer()
	{

		shallRotate = true;
		duckAnim.gameObject.SetActive(false);
		playerRender.sprite = duckPos;
		Invoke("Normal",1.0f);
	}

	public void Skip()
	{
		obsRoot.SetActive(false);
		isTutorialComplete = true;
		Final();
	}
	
	public void ExitTutorial()
	{
		Main.Instance.PlayBGMusic();
		Application.LoadLevelAsync("Play");
	}

	#region Animation Delegate
	public void RemoveAnims()
	{
		AnimationManager.RemoveAllAnims(gameObject);
	}
	
	public void AnimCallback(string animId, AnimState animState, System.Object data = null)
	{
		if((animState == AnimState.Complete))
		{
			if(animId == "Jump")
			{
				AnimationManager.Play(backToGnd, this, 0f, "ToGnd");
			}
			
		}
		
		if(animState == AnimState.Update)
		{


		}
	}
	#endregion

}


