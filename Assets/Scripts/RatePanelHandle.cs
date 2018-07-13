using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RatePanelHandle : MonoBehaviour {

	public RectTransform enjoyPanel;
	public RectTransform ratingPanel;
	public RectTransform feedbackPanel;


	void OnEnable()
	{
		if(!enjoyPanel.gameObject.activeSelf)
		{
			enjoyPanel.gameObject.SetActive(true);
		}
		
		if(ratingPanel.gameObject.activeSelf)
		{
			ratingPanel.gameObject.SetActive(false);
		}
		
		if(feedbackPanel.gameObject.activeSelf)
		{
			feedbackPanel.gameObject.SetActive(false);
		}
	}

	// Use this for initialization
	void Start () 
	{
		if(!enjoyPanel.gameObject.activeSelf)
		{
			enjoyPanel.gameObject.SetActive(true);
		}

		if(ratingPanel.gameObject.activeSelf)
		{
			ratingPanel.gameObject.SetActive(false);
		}

		if(feedbackPanel.gameObject.activeSelf)
		{
			feedbackPanel.gameObject.SetActive(false);
		}
	}



	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKey(KeyCode.Escape))
		{
			RemindMeClicked();
			transform.gameObject.SetActive(false);
		}
	}


	#region Button Actions
	public void EnjoyNo()
	{
		if(enjoyPanel.gameObject.activeSelf)
		{
			enjoyPanel.gameObject.SetActive(false);
		}
		if(!feedbackPanel.gameObject.activeSelf)
		{
			feedbackPanel.gameObject.SetActive(true);
		}

	}

	public void EnjoyYes()
	{
		if(enjoyPanel.gameObject.activeSelf)
		{
			enjoyPanel.gameObject.SetActive(false);
		}
		
		if(!ratingPanel.gameObject.activeSelf)
		{
			ratingPanel.gameObject.SetActive(true);
		}

	}

	public void RatingNo()
	{
		if(ratingPanel.gameObject.activeSelf)
		{
			ratingPanel.gameObject.SetActive(false);
		}
		RemindMeClicked();
	}

	public void RatingYes()
	{
		if(ratingPanel.gameObject.activeSelf)
		{
			ratingPanel.gameObject.SetActive(false);
		}
		//Go To rate page
		RateMeClicked();
	}

	public void FeedbackNo()
	{
		if(feedbackPanel.gameObject.activeSelf)
		{
			feedbackPanel.gameObject.SetActive(false);
		}
		//Reming again
		RemindMeClicked();

	}

	public void FeedbackYes()
	{
		if(feedbackPanel.gameObject.activeSelf)
		{
			feedbackPanel.gameObject.SetActive(false);
		}
		//Go To rate page
		RateMeClicked();

	}
	#endregion


	#region Unirate Calls
	void RateMeClicked()
	{
		UniRate.Instance.SendMessage("UniRateUserWantToRate");
		//		rateAnimator.SetTrigger("Hide");
		transform.gameObject.SetActive(false);
	}
	
	void RemindMeClicked()
	{
		UniRate.Instance.SendMessage("UniRateUserWantRemind");
		//		rateAnimator.SetTrigger("Hide");
		transform.gameObject.SetActive(false);
	}
	
	void CancelClicked()
	{
		UniRate.Instance.SendMessage("UniRateUserDeclinedPrompt");
		//		rateAnimator.SetTrigger("Hide");
	}
	#endregion
}
