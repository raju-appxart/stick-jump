using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class example : MonoBehaviour {


	private bool interstitialShown = false;
	public bool interstitialsAvailable = false;
	public GUITexture button;
	public Image image;
	private bool nativeAdCreated = false;


	private TapdaqNativeAd nativeAd = new TapdaqNativeAd();
	public GameObject nativeCanvas;

	//Subscribe to the tapdaq events
	void OnEnable()
	{
		Tapdaq.willDisplayInterstitial += MyListenerMethod;
		Tapdaq.didDisplayInterstitial += MyListenerMethod_2;
		Tapdaq.didCloseInterstitial += MyListenerMethod_3;
		Tapdaq.didClickInterstitial += MyListenerMethod_4;
		Tapdaq.didFailToLoadInterstitial += MyListenerMethod_5;
		Tapdaq.hasNoInterstitialsAvailable += MyListenerMethod_6;
		//NOTE - this event will be called everytime an interstitial is cached and ready to be shown. 
		//       which can happen multiple times in a few frames
		Tapdaq.hasInterstitialsAvailableForOrientation += MyListenerMethod_7;


		Tapdaq.didFailToLoadNative += MyListenerMethod_8;
		Tapdaq.hasNoNativeAdvertAvailable += MyListenerMethod_9;
		//NOTE - this event will be called everytime a native is cached and ready to be shown. 
		//       which can happen multiple times in a few frames
		Tapdaq.hasNativeAdvertsAvailableForAdUnit += MyListenerMethod_10;
	}

	//Unsubscribe to the tapdaq events
	void OnDisable()
	{
		Tapdaq.willDisplayInterstitial -= MyListenerMethod;
		Tapdaq.didDisplayInterstitial -= MyListenerMethod_2;
		Tapdaq.didCloseInterstitial -= MyListenerMethod_3;
		Tapdaq.didClickInterstitial -= MyListenerMethod_4;
		Tapdaq.didFailToLoadInterstitial -= MyListenerMethod_5;
		Tapdaq.hasNoInterstitialsAvailable -= MyListenerMethod_6;
		Tapdaq.hasInterstitialsAvailableForOrientation -= MyListenerMethod_7;


		Tapdaq.didFailToLoadNative -= MyListenerMethod_8;
		Tapdaq.hasNoNativeAdvertAvailable -= MyListenerMethod_9;
		Tapdaq.hasNativeAdvertsAvailableForAdUnit -= MyListenerMethod_10;
	}



	void Start () 
	{

	}
	

	void Update () 
	{
		if(Input.GetMouseButtonUp(0))
		{
			if(button.HitTest(Input.mousePosition))
			{
				ShowInterstitial();
			}
		}
	}


	public void ShowInterstitial()
	{
		Tapdaq.ShowInterstitial();
	}
	
	public void SendNativeClick()
	{
		print ("Native Ad clicked");
		Tapdaq.SendNativeClick(nativeAd);
	}

	void MyListenerMethod()
	{
		// Example
		Debug.Log("Plugin about to show interstitial");
	}
	void MyListenerMethod_2()
	{
		// Example
		Debug.Log("Plugin has shown an interstitial");
	}
	void MyListenerMethod_3()
	{
		// Example
		Debug.Log("An interstitial was closed");
		//interstitialShown = false;
	}
	void MyListenerMethod_4()
	{
		// Example
		Debug.Log("An interstitial was clicked");
	}
	void MyListenerMethod_5()
	{
		// Example
		Debug.Log("Failed to load interstitials");
	}

	void MyListenerMethod_6()
	{
		// Example
		Debug.Log("Interstitials unavailable");
	}
	void MyListenerMethod_7(string orientation)
	{
		// Example: //Here we use a bool to avoid rapid fire interstitial spam. 'orientation' will either be PORTRAIT or LANDSCAPE
		if(!interstitialShown)
		{
			int orient = int.Parse(orientation);
			Debug.Log(string.Format("An interstitial for {0} orientation is available",(Tapdaq.TDOrientation)orient));
			interstitialShown = true;

			Tapdaq.ShowInterstitial();
			interstitialsAvailable = true;
		}
	}


	//NATIVE Event handling

	void MyListenerMethod_8()
	{
		// Example
		Debug.Log("Failed to load Natives");
	}
	
	void MyListenerMethod_9()
	{
		// Example
		Debug.Log("Has No Natives Available");
	}

	void MyListenerMethod_10(string adType, string adSize, string orientation) 
	{
		//this example method attempts to fetch and display the first available native ad, regardless of category
		int unit = int.Parse(adType);
		int size = int.Parse(adSize);
		int orient = int.Parse(orientation);

		Debug.Log ("Ad type: "+((Tapdaq.TDNativeAdUnit)unit).ToString());
		Debug.Log ("adsize: "+((Tapdaq.TDNativeAdSize)size).ToString());
		Debug.Log ("orientation: "+((Tapdaq.TDOrientation)orient).ToString());
		if(!nativeAdCreated)
		{


			//fetch a native based on the first available type
			nativeAd = Tapdaq.GetNativeAd((Tapdaq.TDNativeAdUnit)unit, (Tapdaq.TDNativeAdSize)size, (Tapdaq.TDOrientation)orient);

			//if not given sufficient loading time, the native ad can return a null object
			if(nativeAd.creativeImage != null)
			{
				nativeCanvas.SetActive(true);
				//Display the image from the nativeAd to a Canvas Image object
				image.sprite = Sprite.Create(nativeAd.creativeImage,new Rect(0,0,nativeAd.creativeImage.width,nativeAd.creativeImage.height),new Vector2(0.5f,0.5f));
				Tapdaq.SendNativeImpression(nativeAd);
				nativeAdCreated = true;

			}
		}

	}
	
}
