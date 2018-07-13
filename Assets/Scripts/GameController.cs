using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using PlayerPrefs = PreviewLabs.PlayerPrefs;
using Heyzap;
using Prime31;
using System.Runtime.InteropServices;

public class GameController : MonoBehaviour {

	public RectTransform bgPanel;
	public RectTransform homePanel, gameOverPanel, gameHintPanel, tutorialPanel, inGamePanel, settingPanel; 
	public RectTransform ratePanel;
	public Text scoreText, bestScore, starCntText;
	public Button freeGiftBtn, videoBtn, unlockCharBtn;
	public Image starAddMsg;
	public Button soundBtn, rateFBMeIcon;
	public Sprite soundOn, soundOff, rateMe, fbLike;

	public GameObject starEffect, ratingPanel, msgOne, msgTwo, msgThree;

	public static event Action HasWatchedVideo;
	int ratingMsg = 1;

	#if UNITY_IOS
	private List<StoreKitProduct> _products;
	#endif
	bool isRestoring, canRestored;

	void OnEnable()
	{
		GameEventManager.GameInit += GameInit;
		GameEventManager.GameStart += GameStarted;
		GameEventManager.GameOver += GameOver;
		GameController.HasWatchedVideo += AddMsg;
		UniRate.Instance.OnPromptedForRating += OnPromptedForRating;
#if UNITY_IOS
		StoreKitManager.productListReceivedEvent += productListReceivedEvent;
		StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailedEvent;
		StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinishedEvent;
		StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessfulEvent;
		StoreKitManager.purchaseCancelledEvent += purchaseCancelledEvent;
		StoreKitManager.purchaseFailedEvent += purchaseFailedEvent;
#elif UNITY_ANDROID
		GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
#endif
	}
	
	void OnDisable()
	{
		GameEventManager.GameInit -= GameInit;
		GameEventManager.GameStart -= GameStarted;
		GameEventManager.GameOver -= GameOver;
		GameController.HasWatchedVideo -= AddMsg;
		UniRate.Instance.OnPromptedForRating -= OnPromptedForRating;
		#if UNITY_IOS
		StoreKitManager.productListReceivedEvent -= productListReceivedEvent;
		StoreKitManager.restoreTransactionsFailedEvent -= restoreTransactionsFailedEvent;
		StoreKitManager.restoreTransactionsFinishedEvent -= restoreTransactionsFinishedEvent;
		StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessfulEvent;
		StoreKitManager.purchaseCancelledEvent -= purchaseCancelledEvent;
		StoreKitManager.purchaseFailedEvent -= purchaseFailedEvent;
#elif UNITY_ANDROID
		GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		GoogleIABManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		#endif
	}

	// Use this for initialization
	void Start () 
	{
		canRestored = false;
		// array of product ID's from iTunesConnect. MUST match exactly what you have there!
		#if UNITY_IOS
		StoreKitManager.autoConfirmTransactions = true;
		var productIdentifiers = new string[] {Constants.PRODUCT_ID};
		StoreKitBinding.requestProductData( productIdentifiers );
		#elif UNITY_ANDROID
		var skus = new string[] {Constants.PRODUCT_ID};
//		var skus = new string[] { "com.prime31.testproduct", "android.test.purchased", "com.prime31.managedproduct", "com.prime31.testsubscription" };
		GoogleIAB.queryInventory( skus );
		#endif

		HZIncentivizedAd.SetDisplayListener(videoAdListener);
		GameInit();
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(PlayerControl.m_eCurrentState)
		{
			case PlayerState.Init:
				if(Input.GetMouseButtonDown(0))
				{

					#if UNITY_EDITOR
					if(!EventSystem.current.IsPointerOverGameObject())
					{
						Play();
					}
					#else 
					if(!EventSystem.current.IsPointerOverGameObject(0))
					{
						Play();
					}
					#endif
					
				}
				break;
		}


	}

	#region Button Actions
	void Play()
	{
		GameEventManager.TriggerGameStart();
	}



	public void Setting()
	{
		Main.Instance.PlayButtonSound();
		if(Main.Instance.isSound)
		{
			soundBtn.image.sprite = soundOn;
		}
		else
		{
			soundBtn.image.sprite = soundOff;
		}
		settingPanel.gameObject.SetActive(true);

	}


	public void SoundOnOff()
	{
		if(Main.Instance.isSound)
		{
			soundBtn.image.sprite = soundOff;
			Main.Instance.StopBGMusic();
			Main.Instance.isSound = false;
		}
		else
		{
			Main.Instance.PlayButtonSound();
			soundBtn.image.sprite = soundOn;
			Main.Instance.isSound = true;
			Main.Instance.PlayBGMusic();
		}
	}

	public void RestorePurchase()
	{
		Main.Instance.PlayButtonSound();
		isRestoring = true;
		#if UNITY_IOS
		StoreKitBinding.restoreCompletedTransactions();
		#elif UNITY_ANDROID
		if(canRestored)
		{
			Debug.Log("Restored Purchase");
			//			GoogleIAB.purchaseProduct(Constants.PRODUCT_ID, "NoAds");
			GoogleIAB.purchaseProduct(Constants.PRODUCT_ID );
		}
		#endif

	}

	public void ExitSettings()
	{
		isRestoring = false;
		Main.Instance.PlayButtonSound();
		settingPanel.gameObject.SetActive(false);
		PlayerPrefs.SetBool(Constants.SOUND, Main.Instance.isSound);
		PlayerPrefs.Flush();
	}

	public void FBLike()
	{
		Main.Instance.PlayButtonSound();
		if(!Main.Instance.isFBLiked)
		{
			Main.Instance.FBConnect();
			Main.Instance.isFBLiked = true;
			Main.Instance.starsCnt += Constants.REWARD_STARS;
			PlayerPrefs.SetInt(Constants.STAR_TOTAL, Main.Instance.starsCnt);
			PlayerPrefs.SetBool(Constants.FB_LIKE, true);
			Debug.Log("!Main.Instance.isFBLiked = " + PlayerPrefs.GetBool(Constants.FB_LIKE, true));
			rateFBMeIcon.image.sprite = rateMe;

			PlayerPrefs.Flush();
		}
		else
		{
			UniRate.Instance.RateIfNetworkAvailable();
		}


	}

	public void Shop()
	{
		Main.Instance.PlayButtonSound();
		Application.LoadLevelAsync("Shop");
	}

	public void Tutorial()
	{
		Main.Instance.StopBGMusic();
		Main.Instance.PlayButtonSound();
		HZBannerAd.Hide();
//		tutorialPanel.gameObject.SetActive(true);
//		homePanel.gameObject.SetActive(false);
		Application.LoadLevelAsync("Tutorial");
	}

//	public void TutorialExit()
//	{
//		Main.Instance.PlayButtonSound();
//		tutorialPanel.gameObject.SetActive(false);
//		homePanel.gameObject.SetActive(true);
//	}

	public void FreeGift()
	{
		Main.Instance.PlayButtonSound();
		freeGiftBtn.interactable = false;
		GameObject stars = Instantiate(starEffect);
		Destroy(stars, 1);
		videoBtn.interactable = false;
		Main.Instance.starsCnt += Constants.REWARD_STARS;
		PlayerPrefs.SetInt(Constants.STAR_TOTAL, Main.Instance.starsCnt);
		Invoke("UpdateText", 0.25f);
	}


	public void RewardVideo()
	{
		Main.Instance.PlayButtonSound();
		if (HZIncentivizedAd.IsAvailable()) 
		{
			HZIncentivizedAd.Show();
		}

	}

	void AddMsg()
	{
		GameObject stars = Instantiate(starEffect);
		Destroy(stars, 1);
		videoBtn.interactable = false;
		Main.Instance.starsCnt += Constants.REWARD_STARS;
		PlayerPrefs.SetInt(Constants.STAR_TOTAL, Main.Instance.starsCnt);
		Invoke("UpdateText", 0.25f);
	}

	void UpdateText()
	{
		starAddMsg.gameObject.SetActive(true);
		starCntText.text = "" + Main.Instance.starsCnt;
		CheckUnlockChar();
	}

	public void Replay()
	{
		Main.Instance.PlayButtonSound();
		GameEventManager.TriggerGameInit();
		Main.Instance.PlayBGMusic();
	}

	public void RateMe()
	{
		Main.Instance.PlayButtonSound();
		UniRate.Instance.RateIfNetworkAvailable();
	}

	public void LeaderBoardShow()
	{
		Main.Instance.PlayButtonSound();
//		HZBannerAd.Hide();
		Main.Instance.ShowLeaderBoard();
	}

	public void ShareScore()
	{
		Main.Instance.PlayButtonSound();
		Main.Instance.ShareUniversal(PlayerControl.gameScore);
	}

	public void NoAds()
	{
		Main.Instance.PlayButtonSound();

		#if UNITY_IOS
		if( _products != null && _products.Count > 0 )
		{
			int productIndex = 0 ; //We have only one product	// Random.Range( 0, _products.Count );
			var product = _products[productIndex];
			
			Debug.Log( "preparing to purchase product: " + product.productIdentifier );
			StoreKitBinding.purchaseProduct( product.productIdentifier, 1 );
		}
		#elif UNITY_ANDROID
		GoogleIAB.purchaseProduct(Constants.PRODUCT_ID );
//		GoogleIAB.purchaseProduct( "android.test.purchased" );
		#endif
	}

	#endregion

	#region Game Event Delegates
	void GameInit()
	{

		if(!Main.Instance.isNoads)
		{
//			HZBannerShowOptions showOptions = new HZBannerShowOptions();
//			showOptions.Position = HZBannerShowOptions.POSITION_TOP;
//			HZBannerAd.ShowWithOptions(showOptions);
		}

		int themeIndex = 0;
		if(Main.Instance.totalGames%3 == 0)
		{
			themeIndex = 0;
		}
		else 
		{
			themeIndex = Main.Instance.totalGames%3;
		}

		bgPanel.GetComponent<Image>().sprite = Resources.Load <Sprite> ("Textures/Theme/" +"BG_" + themeIndex) as Sprite;

		freeGiftBtn.gameObject.SetActive(false);
		videoBtn.gameObject.SetActive(false);
		unlockCharBtn.gameObject.SetActive(false);
		starAddMsg.gameObject.SetActive(false);

		homePanel.gameObject.SetActive(true);
		gameOverPanel.gameObject.SetActive(false);
		gameHintPanel.gameObject.SetActive(false);
		tutorialPanel.gameObject.SetActive(false);
		inGamePanel.gameObject.SetActive(false);
		settingPanel.gameObject.SetActive(false);

		if(Main.Instance.isFBLiked)
		{
			rateFBMeIcon.image.sprite = rateMe;

		}
		else
		{
			rateFBMeIcon.image.sprite = fbLike;
		}
	}

	void GameStarted()
	{
		HZIncentivizedAd.Fetch();
//		if(!Main.Instance.isNoads)
//		{
//			HZBannerShowOptions showOptions = new HZBannerShowOptions();
//			showOptions.Position = HZBannerShowOptions.POSITION_TOP;
//			HZBannerAd.ShowWithOptions(showOptions);
//		}
		homePanel.gameObject.SetActive(false);
		gameOverPanel.gameObject.SetActive(false);
		gameHintPanel.gameObject.SetActive(true);
		tutorialPanel.gameObject.SetActive(false);
		inGamePanel.gameObject.SetActive(true);
		Invoke("RemoveHints", 1.5f);

	}
	
	void GameOver()
	{
		Main.Instance.totalGames++;
		scoreText.text = "" + PlayerControl.gameScore;
		if(PlayerControl.gameScore > Main.Instance.bestScore)
		{
			Main.Instance.bestScore = PlayerControl.gameScore;
			PlayerPrefs.SetInt(Constants.BEST_SCORE, Main.Instance.bestScore);
			Main.Instance.PostScoreToLeaderBoard(Main.Instance.bestScore);
		}
		bestScore.text = "BEST " + Main.Instance.bestScore;
		starCntText.text = "" + Main.Instance.starsCnt;


		PlayerPrefs.SetInt(Constants.GAME_COUNT, Main.Instance.totalGames);
		PlayerPrefs.SetInt(Constants.STAR_TOTAL, Main.Instance.starsCnt);
		PlayerPrefs.Flush();

		//Set up rewards button 
		if(Main.Instance.totalGames%5 == 0)
		{
			freeGiftBtn.interactable = true;
			freeGiftBtn.gameObject.SetActive(true);
		}
		else if (HZIncentivizedAd.IsAvailable()) 
		{
			videoBtn.interactable = true;
			videoBtn.gameObject.SetActive(true);
		}  
		else if(!HZIncentivizedAd.IsAvailable())
		{
			videoBtn.gameObject.SetActive(false);
		}

		CheckUnlockChar();

		Invoke("ShowGO", 0.7f);
	}

	void CheckUnlockChar()
	{
		string purchaseKey1 = "Char_" + 1;
		bool isPurchasedFirst = PlayerPrefs.GetBool(purchaseKey1, false);
		string purchaseKey2 = "Char_" + 2;
		bool isPurchasedSecond = PlayerPrefs.GetBool(purchaseKey2, false);
		
		if(!isPurchasedFirst || !isPurchasedSecond)
		{
			if(Main.Instance.starsCnt >= Constants.PRICE_LOW)
			{
				unlockCharBtn.gameObject.SetActive(true);
			}
		}
		else if(Main.Instance.starsCnt >= Constants.PRICE_CHAR)
		{
			unlockCharBtn.gameObject.SetActive(true);
		}
	}

	void ShowGO()
	{
		homePanel.gameObject.SetActive(false);
		gameOverPanel.gameObject.SetActive(true);
		gameHintPanel.gameObject.SetActive(false);
		inGamePanel.gameObject.SetActive(false);

		if(Main.Instance.totalGames%3 == 0 && Main.Instance.totalGames > 1)
		{
			if(!Main.Instance.isNoads)
			HZInterstitialAd.Show();
		}

		UniRate.Instance.LogEvent(true);
//		if(Main.Instance.totalGames == 13)
//		{
//			OnPromptedForRating();
//		}
	}

	void RemoveHints()
	{
		gameHintPanel.gameObject.SetActive(false);
	}
	#endregion

	#region Unirate Delegate
	void OnPromptedForRating()
	{
		msgOne.SetActive(true);
		msgTwo.SetActive(false);
		msgThree.SetActive(false);
		ratingPanel.SetActive(true);
	}


	public void RatingNo()
	{
		Main.Instance.PlayButtonSound();
		if(ratingMsg == 1)
		{
			msgOne.SetActive(false);
			msgTwo.SetActive(false);
			msgThree.SetActive(true);
			ratingMsg = 3;
		}
		else if(ratingMsg == 3)
		{
			UniRate.Instance.SendMessage("UniRateUserDeclinedPrompt");
			ratingPanel.SetActive(false);
		}
		else if(ratingMsg == 2)
		{
			msgTwo.SetActive(false);
			msgThree.SetActive(true);
			ratingMsg = 3;
			//			UniRate.Instance.SendMessage("UniRateUserWantRemind")
		}
	}

	public void RatingYES()
	{
		Main.Instance.PlayButtonSound();
		if(ratingMsg == 1)
		{
			msgOne.SetActive(false);
			msgTwo.SetActive(true);
			msgThree.SetActive(false);
			ratingMsg = 2;
		}
		else if(ratingMsg == 2)
		{
			UniRate.Instance.SendMessage("UniRateUserWantToRate");
			ratingPanel.SetActive(false);
		}
		else if(ratingMsg == 3)
		{
			UniRate.Instance.SendMessage("UniRateUserWantToRate");
			ratingPanel.SetActive(false);
		}
	}
	#endregion


	#region StoreKit Delegates

	#if UNITY_IOS
	
	[DllImport("__Internal")]
	private static extern void CreateAlertForRestore(string title, string body, string okbtn);
	

	void productListReceivedEvent( List<StoreKitProduct> productList )
	{
		_products = productList;
		Debug.Log("_products count = " + _products.Count + _products[0].productIdentifier);
	}
	
	void purchaseSuccessfulEvent( StoreKitTransaction transaction )
	{
		if(transaction.productIdentifier == Constants.PRODUCT_ID)
		{
//			Debug.Log("Product purchased");
			HZBannerAd.Hide();
			Main.Instance.isNoads = true;
			PlayerPrefs.SetBool(Constants.NO_ADS, Main.Instance.isNoads);
			PlayerPrefs.Flush();
			
			if(isRestoring)
			{
				CreateAlertForRestore("Purchase Restored","Ads removed", "Ok");
			}
			
//			if(Main.Instance.isNoads)
//			{
//				if(noAddButtonHome != null)
//				{
//					noAddButtonHome.interactable = false;
//				}
//			}
//

		}
	}
	
	void OnAlertFinish(string clickedBtn)
	{
		if (clickedBtn == "Ok") 
		{
			Debug.Log("Close Panel");
//			CloseIapPanel();
		} 
	}
	
	void OnAlertCancel()
	{
		Debug.Log(" Cancelled");
	}
	
	void purchaseFailedEvent( string error )
	{
		Debug.Log( "purchaseFailedEvent: " + error );
	}
	
	void purchaseCancelledEvent( string error )
	{
		Debug.Log( "purchaseCancelledEvent: " + error );
	}
	
	void restoreTransactionsFailedEvent( string error )
	{
		Debug.Log( "restoreTransactionsFailedEvent: " + error );
	}
	
	
	void restoreTransactionsFinishedEvent()
	{
		Debug.Log( "restoreTransactionsFinished" );
	}

	#elif UNITY_ANDROID
	void queryInventorySucceededEvent( List<GooglePurchase> purchases, List<GoogleSkuInfo> skus )
	{
		Debug.Log( string.Format( "queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count ) );
		Prime31.Utils.logObject( purchases );
		Prime31.Utils.logObject( skus );
		for(int i = 0; i < purchases.Count; i++)
		{
			Debug.Log("Order id = " + purchases[i].productId +"   State = "+ purchases[i].purchaseState); 
			if(purchases[i].purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
			{
				//Can Restore purchse
				Debug.Log("Can Restore purchse");
				canRestored = true;
			}
		}
		
	}
	
	
	void queryInventoryFailedEvent( string error )
	{
		Debug.Log( "queryInventoryFailedEvent: " + error );
	}
	
	void purchaseSucceededEvent( GooglePurchase purchase )
	{
		Debug.Log( "purchaseSucceededEvent: " + purchase );
		if(purchase.productId == Constants.PRODUCT_ID)
		{
			Debug.Log("No ads purchased");
			HZBannerAd.Hide();
			Main.Instance.isNoads = true;
			PlayerPrefs.SetBool(Constants.NO_ADS, Main.Instance.isNoads);
			PlayerPrefs.Flush();
			
			
			if(isRestoring)
			{
				#if UNITY_ANDROID
				EtceteraAndroid.showAlert("Purchase Restored", "Ads removed", "Ok");
				#elif UNITY_IPHONE
				CreateAlertForRestore("Purchase Restored","Ads removed", "Ok");
				#endif
				
			}
			
		}
		
		
	}
	#endif

	#endregion
	


	#region Reward Video Delegate
	HZIncentivizedAd.AdDisplayListener videoAdListener = delegate(string adState, string adTag)
	{
		if ( adState.Equals("incentivized_result_complete") ) 
		{
			// The user has watched the entire video and should be given a reward.
			HasWatchedVideo();
		}
		if ( adState.Equals("incentivized_result_incomplete") ) 
		{
			// The user did not watch the entire video and should not be given a   reward.
		}
	};
	#endregion
}
