using UnityEngine;
using System.Collections;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

public class ShopButton : MonoBehaviour {

	Animator shopAnim;

	void OnEnable()
	{
		if(shopAnim == null)
		{
			shopAnim = transform.GetComponent<Animator>();
		}
		string purchaseKey1 = "Char_" + 1;
		bool isPurchasedFirst = PlayerPrefs.GetBool(purchaseKey1, false);
		string purchaseKey2 = "Char_" + 2;
		bool isPurchasedSecond = PlayerPrefs.GetBool(purchaseKey2, false);
		
		if(!isPurchasedFirst || !isPurchasedSecond)
		{
			if(Main.Instance.starsCnt >= Constants.PRICE_LOW)
			{
				if(shopAnim != null)
					shopAnim.enabled = true;
			}
			else
			{
				if(shopAnim != null)
					shopAnim.enabled = false;
			}
		}
		else if(Main.Instance.starsCnt >= Constants.PRICE_CHAR)
		{
			if(shopAnim != null)
				shopAnim.enabled = true;
		}
		else
		{
			if(shopAnim != null)
			shopAnim.enabled = false;
		}

	}
	
	void OnDisable()
	{
//		if(shopAnim != null)
//		shopAnim.enabled = false;
	}

	void Start()
	{
		shopAnim = transform.GetComponent<Animator>();
	}

}
