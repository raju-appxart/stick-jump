using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using System.IO;

public class Tapdaq : MonoBehaviour {

	//Interop Delegate
	public delegate void nativeAdDelegate(string _adObject);
	public delegate void interstitialAdDelegate(string _adObject);

	public delegate void Interop_InterstitialDelegateCallBack(string _adObject);
	public delegate void Interop_NativeDelegateCallBack(string _adObject, string _adObject2, string _adObject3);

	[DllImport ("__Internal")]
	private static extern void _GenerateCallBacks(Interop_NativeDelegateCallBack _callback, Interop_InterstitialDelegateCallBack _callback2);

	#if UNITY_IPHONE
	//================================= Interstitials ==================================================
	[DllImport ("__Internal")]
	private static extern void _ConfigureTapdaq(string _appID, string _clientKey, int _freq, int _dur, string _enabledAdTypes, bool testMode);

	[DllImport ("__Internal")]
	private static extern void _FetchInterstitial(interstitialAdDelegate _delegate,int _orientation);

	[DllImport ("__Internal")]
	private static extern void _SendInterstitialClick(int _pointer);
	
	[DllImport ("__Internal")]
	private static extern void _SendInterstitialImpression(int _pointer);

	[DllImport ("__Internal")]
	private static extern void _ShowInterstitial(int _pointer);

	//[DllImport ("__Internal")]
	//private static extern void _ShowInterstitial(int _orientation);


	//================================== Natives =================================================
	[DllImport ("__Internal")]
	private static extern void _FetchNative(nativeAdDelegate _delegate, int _adType, int _adSize, int _orientation); 

	[DllImport ("__Internal")]
	private static extern void _SendNativeClick(int _pointer);
	
	[DllImport ("__Internal")]
	private static extern void _SendNativeImpression(int _pointer);


	#endif

	public enum TDAdType
	{
		interstitial,
		native,
	};
	
	public enum TDOrientation
	{
		portrait,
		landscape,
		universal,
	};

	public enum TDNativeAdUnit
	{
		square,
		newsfeed,
		fullscreen,
		strip,
	};
	
	public enum TDNativeAdSize
	{
		small,
		medium,
		large,
	};

	#region Class Variables

	public static Tapdaq TDinstance;
	private TapdaqSettings settings;
	private static TapdaqView view;

	private string ios_applicationID = ""; 
	private string ios_clientKey = "";

	private string android_applicationID = ""; 
	private string android_clientKey = "";
	
	//Ad frequency cap. X(frequecy) ads per Y(duration) days. i.e. 10 ads per 2 days
	private int frequency = 10; 
	private int duration = 2; 

	private bool _TEST_MODE_ = true;
	private bool showLogMessages = false;

	//-----------Ad Types//
	private bool interstitials = false;
	private bool interstitialPortrait = false;
	private bool interstitialLandscape = false;


	private bool nativeSquareLarge = false;
	private bool nativeSquareMedium = false; 
	private bool nativeSquareSmall = false;
	private bool nativeNewsfeedPortraitLarge = false;
	private bool nativeNewsfeedPortraitMedium = false;
	private bool nativeNewsfeedPortraitSmall = false;
	private bool nativeNewsfeedLandscapeLarge = false;
	private bool nativeNewsfeedLandscapeMedium = false;
	private bool nativeNewsfeedLandscapeSmall = false;
	private bool nativeFullscreenPortraitLarge = false;
	private bool nativeFullscreenPortraitMedium = false; 
	private bool nativeFullscreenPortraitSmall = false;
	private bool nativeFullscreenLandscapeLarge = false;
	private bool nativeFullscreenLandscapeMedium = false;
	private bool nativeFullscreenLandscapeSmall = false;
	private bool nativeStripPortraitLarge = false;
	private bool nativeStripPortraitMedium = false;
	private bool nativeStripPortraitSmall = false;
	private bool nativeStripLandscapeLarge = false;
	private bool nativeStripLandscapeMedium = false;
	private bool nativeStripLandscapeSmall = false;
	//------------

//	public static GameObject interstitialCanvas;
//	public static CanvasGroup group;
//	public static Image interstitialPortraitImage;
//	public static Image interstitialLandscapeImage;
	private static Texture closeImage;

//	public GameObject ns_interstitialCanvas;
//	public CanvasGroup ns_group;
//	public Image ns_interstitialPortraitImage;
//	public Image ns_interstitialLandscapeImage;


	private List<string>enabledAdTypes = new List<string>();
	private string flatList = "";

	private static TDinterstitialAd thisInterstitialAd;
	private static TapdaqInterstitialAd externalInterstitial = new TapdaqInterstitialAd();

	public static bool hasPortraitInterstitial = false;
	public static bool hasLandscapeInterstitial = false;
	public static bool showingInterstitial = false;


	private static TDNativeAd thisNativeAd;
	private static TapdaqNativeAd externalNative = new TapdaqNativeAd();



	///   delegates
	
	public delegate void willDisplayInterstitialDelegate();
	public delegate void didDisplayInterstitialDelegate();
	public delegate void didCloseInterstitialDelegate();
	public delegate void didClickInterstitialDelegate();
	public delegate void didFailToLoadInterstitialDelegate();
	public delegate void hasNoInterstitialsAvailableDelegate();
	public delegate void hasInterstitialsAvailableForOrientationDelegate(string orientation);

	public delegate void didFailToLoadNativeDelegate();
	public delegate void hasNoNativeAdvertAvailableDelegate();
	public delegate void hasNativeAdvertsAvailableForAdUnitDelegate(string adType, string adSize, string orientation);

	public delegate void didFailToConnectToServerDelegate(string message);
	
	public static event willDisplayInterstitialDelegate willDisplayInterstitial;
	public static event didDisplayInterstitialDelegate didDisplayInterstitial;
	public static event didCloseInterstitialDelegate didCloseInterstitial;
	public static event didClickInterstitialDelegate didClickInterstitial;
	public static event didFailToLoadInterstitialDelegate didFailToLoadInterstitial;
	public static event hasNoInterstitialsAvailableDelegate hasNoInterstitialsAvailable;
	public static event hasInterstitialsAvailableForOrientationDelegate hasInterstitialsAvailableForOrientation;

	public static event didFailToLoadNativeDelegate didFailToLoadNative;
	public static event hasNoNativeAdvertAvailableDelegate hasNoNativeAdvertAvailable;
	public static event hasNativeAdvertsAvailableForAdUnitDelegate hasNativeAdvertsAvailableForAdUnit;

	public static event didFailToConnectToServerDelegate didFailToConnectToServer;



	///////

	#endregion
	/// ////////////////////////////////////////////////////////////////

	#region Interstitial Class
	public class TDinterstitialAd
	{
		public string applicationId {get;private set;}
		public string targetingId {get;private set;}
		public string subscriptionId {get;private set;} // (optional) 
		
		public string creativeIdentifier {get;private set;}
		public TDOrientation creativeOrientation {get;private set;} // Can be either `TDOrientationPortrait` or `TDOrientationLandscape
		public string creativeResolution {get;private set;} // Can be `TDResolution1x`, `TDResolution2x` or `TDResolution3x`
		
		
		public int creativeAspectRatioWidth {get;private set;}
		public int creativeAspectRatioHeight {get;private set;}
		
		public string creativeURL {get;private set;}
		public Texture2D creativeImage {get;private set;}
		public int pointer{get;private set;}
		
		public TDinterstitialAd()
		{
			applicationId = "999";
			targetingId = "888";
			subscriptionId = "777";
			
			creativeIdentifier = "555";
			creativeOrientation = (TDOrientation)0;
			creativeResolution = "444";
			
			creativeAspectRatioWidth = 0;
			creativeAspectRatioHeight = 0;
			
			creativeURL = "333";
			creativeImage = null;
			pointer = 0;
		}
		public TDinterstitialAd(string objcAdString)
		{
			string[] adObject = objcAdString.Split(new[]{"<>"},System.StringSplitOptions.None);
			if(adObject.Length == 0)
			{
				Debug.Log("this ad Object is empty, SDK has not initialized yet.");
			}
			
			applicationId = adObject[0];
			targetingId = adObject[1];
			subscriptionId = adObject[2];
			
			creativeIdentifier = adObject[3];
			creativeOrientation = (TDOrientation)System.Int32.Parse(adObject[4]);
			creativeResolution = adObject[5];
			
			creativeAspectRatioWidth = System.Int32.Parse(adObject[6]);
			creativeAspectRatioHeight = System.Int32.Parse(adObject[7]);
			
			creativeURL = adObject[8];
			creativeImage = PathToTexture(adObject[9]);
			pointer = System.Int32.Parse(adObject[10]);
			
		}
		
		//Read I.O. path and build texture.
		private Texture2D PathToTexture(string path)
		{
			switch(creativeResolution)
			{
			case(""):
				break;
				
			}
			
			
			int width = Screen.width;
			int height = Screen.height;
			
			Texture2D tex  = new Texture2D(width,height,TextureFormat.RGBA32,false);
			byte[] imageBytes;
			if(path!=null)
			{	
				imageBytes = File.ReadAllBytes(path);
				tex.LoadImage(imageBytes);
			}
			
			imageBytes = null;
			File.Delete(path);
			
			return tex;
			
		}
		
		
	}
	#endregion
	#region Native Class
	public class TDNativeAd
	{
		public string applicationId {get;private set;}
		public string targetingId {get;private set;}
		public string subscriptionId {get;private set;} // (optional) 

		public string appName {get;private set;}
		public string appDescription {get;private set;}
		public string buttonText {get;private set;}
		public string developerName {get;private set;}
		public string ageRating {get;private set;}
		public string appSize {get;private set;}
		public float averageReview {get;private set;}
		public int totalReviews {get;private set;}
		public string category {get;private set;}
		public string appVersion {get;private set;}
		public float price {get;private set;}
		public string currency {get;private set;}
		public TDNativeAdUnit adUnit {get;private set;} // Can be either `TDNativeAdUnitSquare`, `TDNativeAdUnitNewsfeed`, `TDNativeAdUnitFullscreen`, `TDNativeAdUnitStrip`
		public TDNativeAdSize adSize {get;private set;} // Can be either `TDNativeAdSizeSmall`, `TDNativeAdSizeMedium`, `TDNativeAdSizeLarge`
		public string iconUrl {get;private set;}
		public Texture2D icon {get;private set;}
		
		public string creativeIdentifier {get;private set;}
		public TDOrientation creativeOrientation {get;private set;} // Can be either `TDOrientationPortrait` or `TDOrientationLandscape
		public string creativeResolution {get;private set;} // Can be `TDResolution1x`, `TDResolution2x` or `TDResolution3x`
		
		
		public int creativeAspectRatioWidth {get;private set;}
		public int creativeAspectRatioHeight {get;private set;}
		
		public string creativeURL {get;private set;}
		public Texture2D creativeImage {get;private set;}
		
		public int pointer{get;private set;}
		
		public TDNativeAd()
		{
			applicationId = "";
			targetingId = "";
			subscriptionId = "";

			appName = "";
			appDescription = "";
			buttonText = "";
			developerName = "";
			ageRating = "";
			appSize = "";
			averageReview = 0;
			totalReviews = 0;
			category = "";
			appVersion = "";
			price = 0;
			currency = "";
			adUnit = TDNativeAdUnit.fullscreen; // Can be either `TDNativeAdUnitSquare`, `TDNativeAdUnitNewsfeed`, `TDNativeAdUnitFullscreen`, `TDNativeAdUnitStrip`
			adSize = TDNativeAdSize.large; // Can be either `TDNativeAdSizeSmall`, `TDNativeAdSizeMedium`, `TDNativeAdSizeLarge`
			iconUrl = "";
			icon = null;
			
			creativeIdentifier = "";
			creativeOrientation = TDOrientation.landscape;
			creativeResolution = "";
			
			creativeAspectRatioWidth = 1;
			creativeAspectRatioHeight = 1;
			
			creativeURL = "";
			creativeImage = null;
			pointer = 0;
		}
		
		public TDNativeAd(string objcAdString)
		{
			string[] adObject = objcAdString.Split(new[]{"<>"},System.StringSplitOptions.None);
			if(adObject.Length == 0)
			{
				Debug.Log("this ad Object is empty, SDK has not initialized yet.");
			}
			else{
			applicationId = adObject[0];
			targetingId = adObject[1];
			subscriptionId = adObject[2];

			appName = adObject[3];
			appDescription = adObject[4];
			buttonText = adObject[5];
			developerName = adObject[6];
			ageRating = adObject[7];
			appSize = adObject[8];
			averageReview = float.Parse(adObject[9]);
			totalReviews = System.Int32.Parse(adObject[10]);
			category = adObject[11];
			appVersion = adObject[12];
			price = float.Parse(adObject[13]);
			currency = adObject[14];
			adUnit = (TDNativeAdUnit)System.Int32.Parse(adObject[15]); // Can be either `TDNativeAdUnitSquare`, `TDNativeAdUnitNewsfeed`, `TDNativeAdUnitFullscreen`, `TDNativeAdUnitStrip`
			adSize = (TDNativeAdSize)System.Int32.Parse(adObject[16]); // Can be either `TDNativeAdSizeSmall`, `TDNativeAdSizeMedium`, `TDNativeAdSizeLarge`
			iconUrl = adObject[17];
			icon = PathToTexture(adObject[18]);
			
			creativeIdentifier = adObject[19];
			creativeOrientation = (TDOrientation)System.Int32.Parse(adObject[20]);
			creativeResolution = adObject[21];
			
			creativeAspectRatioWidth = System.Int32.Parse(adObject[22]);
			creativeAspectRatioHeight = System.Int32.Parse(adObject[23]);
			
			creativeURL = adObject[24];
			creativeImage = PathToTexture(adObject[25]);
			pointer = System.Int32.Parse(adObject[26]);
			}
			
			
		}
		
		//Read I.O. path and build texture.
		private Texture2D PathToTexture(string path)
		{
			switch(creativeResolution)
			{
			case(""):
				break;
				
			}
			
			int width = Screen.width;
			int height = Screen.height;
			
			Texture2D tex  = new Texture2D(width,height,TextureFormat.RGBA32,false);
			byte[] imageBytes;
			if(path!=null)
			{	
				imageBytes = File.ReadAllBytes(path);
				tex.LoadImage(imageBytes);
			}
			
			imageBytes = null;
			File.Delete(path);
			
			return tex;
			
		}
	}
	#endregion
	#region Android Listener Class
#if UNITY_ANDROID
	class NativeAdFetchCallback : AndroidJavaProxy
	{
		public NativeAdFetchCallback() : base("com.nerd.TapdaqUnityPlugin.TapdaqUnity$NativeAdFetchListener") { }
		void onFetchFinished(string _adObj)
		{
			BuildAndroidNativeAd(_adObj);
		}
	}
#endif

	#endregion

	void Awake()
	{
		if (TDinstance != null) { // Ensuring no clones
			Destroy(gameObject);
			return;
		}
		//Make the plugin available across all scenes
		TDinstance = this;
		GameObject.DontDestroyOnLoad (gameObject);
		settings = GetComponent<TapdaqSettings>();
		view = GetComponent<TapdaqView>();
		closeImage = Resources.Load("tdCircle") as Texture;


		//Apply All Settings
		showLogMessages = settings.showLogs;
		_TEST_MODE_ = settings.testMode;

		ios_applicationID = settings.ios_applicationID;
		ios_clientKey = settings.ios_clientKey;
		android_applicationID = settings.android_applicationID;
		android_clientKey = settings.android_clientKey;

		frequency = settings.frequency;
		duration = settings.duration;

		interstitials = settings.interstitials;
		//interstitialPortrait = settings.interstitialPortrait;
		//interstitialLandscape = settings.interstitialLandscape;


		nativeSquareLarge = settings.nativeSquareLarge;
		nativeSquareMedium = settings.nativeSquareMedium;
		nativeSquareSmall = settings.nativeSquareSmall;

		nativeNewsfeedPortraitLarge = settings.nativeNewsfeedPortraitLarge;
		nativeNewsfeedPortraitMedium = settings.nativeNewsfeedPortraitMedium;
		nativeNewsfeedPortraitSmall = settings.nativeNewsfeedPortraitSmall;

		nativeNewsfeedLandscapeLarge = settings.nativeNewsfeedLandscapeLarge;
		nativeNewsfeedLandscapeMedium = settings.nativeNewsfeedLandscapeMedium;
		nativeNewsfeedLandscapeSmall = settings.nativeNewsfeedLandscapeSmall;

		nativeFullscreenPortraitLarge = settings.nativeFullscreenPortraitLarge;
		nativeFullscreenPortraitMedium = settings.nativeFullscreenPortraitMedium; 
		nativeFullscreenPortraitSmall = settings.nativeFullscreenPortraitSmall;

		nativeFullscreenLandscapeLarge = settings.nativeFullscreenLandscapeLarge;
		nativeFullscreenLandscapeMedium = settings.nativeFullscreenLandscapeMedium;
		nativeFullscreenLandscapeSmall = settings.nativeFullscreenLandscapeSmall;

		nativeStripPortraitLarge = settings.nativeStripPortraitLarge;
		nativeStripPortraitMedium = settings.nativeStripPortraitMedium;
		nativeStripPortraitSmall = settings.nativeStripPortraitSmall;

		nativeStripLandscapeLarge = settings.nativeStripLandscapeLarge;
		nativeStripLandscapeMedium = settings.nativeStripLandscapeMedium;
		nativeStripLandscapeSmall = settings.nativeStripLandscapeSmall;


		if(showLogMessages)
		{
			Debug.Log("TapdaqSDK/Test Mode Active? -- "+_TEST_MODE_);
#if UNITY_IPHONE
			Debug.Log("TapdaqSDK/Application ID -- "+ios_applicationID);
			Debug.Log("TapdaqSDK/Client Key -- "+ios_clientKey);
#elif UNITY_ANDROID

			Debug.Log("TapdaqSDK/Application ID -- "+android_applicationID);
			Debug.Log("TapdaqSDK/Client Key -- "+android_clientKey);
#endif

			Debug.Log("TapdaqSDK/Ad Frequency -- "+frequency);
			Debug.Log("TapdaqSDK/Ad Duration -- "+duration);

			//if(interstitialPortrait)Debug.Log("TapdaqSDK/Portrait Interstitials enabled");
			//if(interstitialLandscape)Debug.Log("TapdaqSDK/Landscape Interstitials enabled");

			if(interstitials)
			{
				if(Screen.width >= Screen.height)
				{
					Debug.Log("TapdaqSDK/Landscape Interstitials enabled");
				}
				else
				{
					Debug.Log("TapdaqSDK/Portrait Interstitials enabled");
				}
			}

			if(nativeSquareLarge)Debug.Log("TapdaqSDK/Native Square Large enabled");
			if(nativeSquareMedium)Debug.Log("TapdaqSDK/Native Square Medium enabled");
			if(nativeSquareSmall)Debug.Log("TapdaqSDK/Native Square Small enabled");

			if(nativeNewsfeedPortraitLarge)Debug.Log("TapdaqSDK/Native News Feed Portrait Large enabled");
			if(nativeNewsfeedPortraitMedium)Debug.Log("TapdaqSDK/Native News Feed Portrait Medium enabled");
			if(nativeNewsfeedPortraitSmall)Debug.Log("TapdaqSDK/Native News Feed Portrait Small enabled");

			if(nativeNewsfeedLandscapeLarge)Debug.Log("TapdaqSDK/Native News Feed Landscape Large enabled");
			if(nativeNewsfeedLandscapeMedium)Debug.Log("TapdaqSDK/Native News Feed Landscape Medium enabled");
			if(nativeNewsfeedLandscapeSmall)Debug.Log("TapdaqSDK/Native News Feed Landscape Small enabled");

			if(nativeFullscreenPortraitLarge)Debug.Log("TapdaqSDK/Native Full Screen Portrait Large enabled");
			if(nativeFullscreenPortraitMedium)Debug.Log("TapdaqSDK/Native Full Screen Portrait Medium enabled");
			if(nativeFullscreenPortraitSmall)Debug.Log("TapdaqSDK/Native Full Screen Portrait Small enabled");

			if(nativeFullscreenLandscapeLarge)Debug.Log("TapdaqSDK/Native Full Screen Landscape Large enabled");
			if(nativeFullscreenLandscapeMedium)Debug.Log("TapdaqSDK/Native Full Screen Landscape Medium enabled");
			if(nativeFullscreenLandscapeSmall)Debug.Log("TapdaqSDK/Native Full Screen Landscape Small enabled");

			if(nativeStripPortraitLarge)Debug.Log("TapdaqSDK/Native Strip Portrait Large enabled");
			if(nativeStripPortraitMedium)Debug.Log("TapdaqSDK/Native Strip Portrait Medium enabled");
			if(nativeStripPortraitSmall)Debug.Log("TapdaqSDK/Native Strip Portrait Small enabled");

			if(nativeStripLandscapeLarge)Debug.Log("TapdaqSDK/Native Strip Landscape Large enabled");
			if(nativeStripLandscapeMedium)Debug.Log("TapdaqSDK/Native Strip Landscape Medium enabled");
			if(nativeStripLandscapeSmall)Debug.Log("TapdaqSDK/Native Strip Landscape Small enabled");

		}

//		group = ns_group;
//		interstitialCanvas = ns_interstitialCanvas;
//		interstitialPortraitImage = ns_interstitialPortraitImage;
//		interstitialLandscapeImage = ns_interstitialLandscapeImage;

		//Initialize tapdaq and commence ad caching
		BuildEnabledAdTypesList();
#if UNITY_IPHONE
		GenerateCallbacks();
		TDinitialize(ios_applicationID, ios_clientKey,frequency,duration,flatList);
#elif UNITY_ANDROID
		TDinitialize(android_applicationID, android_clientKey,frequency,duration,flatList);
#endif

		externalInterstitial = new TapdaqInterstitialAd();
		externalNative = new TapdaqNativeAd();

	}
	


	[MonoPInvokeCallback(typeof(Interop_InterstitialDelegateCallBack))]
	private static void InterstitialDelegateCallBack(string _orientation)
	{
		TDinstance._hasInterstitialsAvailableForOrientation(_orientation);
	}
	
	[MonoPInvokeCallback(typeof(Interop_NativeDelegateCallBack))]
	private static void NativeDelegateCallBack(string _adObject, string _adObject2, string _adObject3)
	{
		//Debug.Log("stranger things have happened "+_adObject+", "+_adObject2+", "+_adObject3);
		TDinstance._hasNativeAdvertsAvailableForAdUnit(_adObject, _adObject2, _adObject3);
	}

	private void GenerateCallbacks()
	{
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			Debug.Log("TapdaqSDK/Dummy: Generating Callbacks");
		}
		
		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_GenerateCallBacks(NativeDelegateCallBack, InterstitialDelegateCallBack);
		}
		#endif
	}
	
	private void TDinitialize(string appID, string clientKey, int freq, int dur, string enabledAdTypes)
	{
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			Debug.Log("TapdaqSDK/Dummy: Initialize");
		}

		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Debug.Log("TapdaqSDK/Initializing");

			_ConfigureTapdaq(appID, clientKey, freq, dur, enabledAdTypes, _TEST_MODE_);

		}
		#endif

		#if UNITY_ANDROID
		string _path = Application.persistentDataPath.Substring( 0, Application.persistentDataPath.Length - 5 );
		_path = _path.Substring( 0, _path.LastIndexOf( '/' ) );
		_path = Path.Combine( _path, "Documents/" );

		if(Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject act = jc.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject appCtx = act.Call<AndroidJavaObject>("getApplicationContext");
				using (AndroidJavaClass mHumbleAssistantClass = new AndroidJavaClass("com.nerd.TapdaqUnityPlugin.TapdaqUnity"))
				{
					mHumbleAssistantClass.CallStatic("SetDataPath",appCtx,_path);
					mHumbleAssistantClass.CallStatic("InitiateTapdaq",act,appID,clientKey,enabledAdTypes, freq, dur, _TEST_MODE_);
				}
			}
		}
		#endif
	}

	
	void BuildEnabledAdTypesList()
	{
//		if(interstitialPortrait == true) enabledAdTypes.Add("interstitialPortrait");
//		if(interstitialLandscape == true) enabledAdTypes.Add("interstitialLandscape");
		if(Screen.width >= Screen.height)
		{
			//Landscape
			interstitialLandscape = true;
			enabledAdTypes.Add("interstitialLandscape");
		}
		else
		{
			//Portrait
			interstitialPortrait = true;
			enabledAdTypes.Add("interstitialPortrait");
		}


		if(nativeSquareLarge == true) enabledAdTypes.Add("nativeSquareLarge");
		if(nativeSquareMedium == true) enabledAdTypes.Add("nativeSquareMedium");
		if(nativeSquareSmall == true) enabledAdTypes.Add("nativeSquareSmall");
		if(nativeNewsfeedPortraitLarge == true) enabledAdTypes.Add("nativeNewsfeedPortraitLarge");
		if(nativeNewsfeedPortraitMedium == true) enabledAdTypes.Add("nativeNewsfeedPortraitMedium");
		if(nativeNewsfeedPortraitSmall == true) enabledAdTypes.Add("nativeNewsfeedPortraitSmall");
		if(nativeNewsfeedLandscapeLarge == true) enabledAdTypes.Add("nativeNewsfeedLandscapeLarge");
		if(nativeNewsfeedLandscapeMedium == true) enabledAdTypes.Add("nativeNewsfeedLandscapeMedium");
		if(nativeNewsfeedLandscapeSmall == true) enabledAdTypes.Add("nativeNewsfeedLandscapeSmall");
		if(nativeFullscreenPortraitLarge == true) enabledAdTypes.Add("nativeFullscreenPortraitLarge");
		if(nativeFullscreenPortraitMedium == true) enabledAdTypes.Add("nativeFullscreenPortraitMedium");
		if(nativeFullscreenPortraitSmall == true) enabledAdTypes.Add("nativeFullscreenPortraitSmall");
		if(nativeFullscreenLandscapeLarge == true) enabledAdTypes.Add("nativeFullscreenLandscapeLarge");
		if(nativeFullscreenLandscapeMedium == true) enabledAdTypes.Add("nativeFullscreenLandscapeMedium");
		if(nativeFullscreenLandscapeSmall == true) enabledAdTypes.Add("nativeFullscreenLandscapeSmall");
		if(nativeStripPortraitLarge == true) enabledAdTypes.Add("nativeStripPortraitLarge");
		if(nativeStripPortraitMedium == true) enabledAdTypes.Add("nativeStripPortraitMedium");
		if(nativeStripPortraitSmall == true) enabledAdTypes.Add("nativeStripPortraitSmall");
		if(nativeStripLandscapeLarge == true) enabledAdTypes.Add("nativeStripLandscapeLarge");
		if(nativeStripLandscapeMedium == true) enabledAdTypes.Add("nativeStripLandscapeMedium");
		if(nativeStripLandscapeSmall == true) enabledAdTypes.Add("nativeStripLandscapeSmall");

		//Array must contain at least 1 value;
		if(enabledAdTypes.Count == 0) enabledAdTypes.Add("interstitialPortrait");

#if UNITY_IPHONE
		foreach(string s in enabledAdTypes)
		{
			flatList += s+"\n";
		}
#elif UNITY_ANDROID
		foreach(string s in enabledAdTypes)
		{
			flatList += s+"<>";
		}
#endif
	
	}
	

	[MonoPInvokeCallback(typeof(interstitialAdDelegate))]
	private static void BuildInterstitialAd2(string _adObject)
	{
		thisInterstitialAd = new TDinterstitialAd(_adObject);
		externalInterstitial.Build(thisInterstitialAd);
		if(externalInterstitial.creativeOrientation == TDOrientation.portrait) hasPortraitInterstitial = true;
		else if (externalInterstitial.creativeOrientation == TDOrientation.landscape) hasLandscapeInterstitial = true;
	}

	

	[MonoPInvokeCallback(typeof(nativeAdDelegate))]
	private static void BuildNativeAd(string _adObject)
	{
		if(_adObject != "_FAILED")
		{
			thisNativeAd = new TDNativeAd(_adObject);
			externalNative.Build(thisNativeAd);
		}
		else
		{
			externalNative = new TapdaqNativeAd();
		}
			
	}

	public static void BuildAndroidNativeAd(string _adObject)
	{
		if(_adObject != "_FAILED")
		{
			thisNativeAd = new TDNativeAd(_adObject);
			externalNative.Build(thisNativeAd);
		}
		else
		{
			externalNative = new TapdaqNativeAd();
		}

	}

	public void FetchFailed(string msg)
	{
		print (msg);
		Debug.Log("+ unable to fetch more ads");
	}



	

	public static void ShowInterstitial()
	{
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			Debug.Log("TapdaqSDK/Dummy: Showinterstitial");
			//fire off event
			if(Tapdaq.TDinstance)
			{
				TDinstance._willDisplayInterstitial("will show Interstitial");
				if(!showingInterstitial)DummyShowInterstitial();
			}

		}

		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			//fire off event
			if(Tapdaq.TDinstance)
			{
				//TDinstance._willDisplayInterstitial("will show Interstitial");
				_ShowInterstitial(0);
				//if(!showingInterstitial)PrepareInterstitialForShowing();
			}
		}
		#endif
	#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject act = jc.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject appCtx = act.Call<AndroidJavaObject>("getApplicationContext");
				using (AndroidJavaClass mHumbleAssistantClass = new AndroidJavaClass("com.nerd.TapdaqUnityPlugin.TapdaqUnity"))
				{
					mHumbleAssistantClass.CallStatic("ShowInterstitial",act);
				}
			}
		}
	#endif

	}

	private static void DummyShowInterstitial()
	{
		if(Screen.width <= Screen.height)
		{

			//interstitialCanvas.SetActive(true);
			//interstitialLandscapeImage.gameObject.SetActive(false);
			//interstitialPortraitImage.gameObject.SetActive(true);
			//interstitialPortraitImage.sprite = Resources.Load("Tapdaqeditortestadportrait",typeof(Sprite))as Sprite;
			view.enabled = true;
			view.PrepareView(Resources.Load("Tapdaqeditortestadportrait",typeof(Texture)) as Texture,closeImage);

			//DoCoroutine("FadeInterstitialGroupAlpha");
			TDinstance._didDisplayInterstitial("Interstitial Shown");
			showingInterstitial = true;
		}
		else if(Screen.width > Screen.height)
		{

//			interstitialCanvas.SetActive(true);
//			interstitialLandscapeImage.gameObject.SetActive(true);
//			interstitialPortraitImage.gameObject.SetActive(false);
//			interstitialLandscapeImage.sprite = Resources.Load("Tapdaqeditortestadlandscape",typeof(Sprite))as Sprite;
			view.enabled = true;
			view.PrepareView(Resources.Load("Tapdaqeditortestadlandscape",typeof(Texture)) as Texture,closeImage);
		
			//DoCoroutine("FadeInterstitialGroupAlpha");
			TDinstance._didDisplayInterstitial("Interstitial Shown");
			showingInterstitial = true;

		}
	}

	private static void PrepareInterstitialForShowing()
	{

		if(Screen.width <= Screen.height)
		{
			//portrait
			#if UNITY_IPHONE
			_FetchInterstitial(BuildInterstitialAd2,(int)TDOrientation.portrait);
			#endif

			if(hasPortraitInterstitial)
			{

				/*interstitialCanvas.SetActive(true);
				//interstitialLandscapeImage.gameObject.SetActive(false);
				//interstitialPortraitImage.gameObject.SetActive(true);
				//interstitialPortraitImage.sprite = Sprite.Create(externalInterstitial.creativeImage,new Rect(0,0,externalInterstitial.creativeImage.width,externalInterstitial.creativeImage.height),new Vector2(0.5f,0.5f));
				/DoCoroutine("FadeInterstitialGroupAlpha");
				*/

				view.enabled = true;
				view.PrepareView(externalInterstitial.creativeImage,closeImage);
			
				SendInterstitialImpression(externalInterstitial);
				//fire off event
				TDinstance._didDisplayInterstitial("Interstitial Shown");
				showingInterstitial = true;

				hasPortraitInterstitial = false;
			}
			else
			{
				Debug.Log("Has No PORTRAIT interstitials");
			}

		}
		else if(Screen.width > Screen.height)
		{
			#if UNITY_IPHONE
			_FetchInterstitial(BuildInterstitialAd2,(int)TDOrientation.landscape);
			#endif

			if(hasLandscapeInterstitial)
			{
				/*interstitialCanvas.SetActive(true);
				//interstitialLandscapeImage.gameObject.SetActive(true);
				//interstitialPortraitImage.gameObject.SetActive(false);
				//
				//interstitialLandscapeImage.sprite = Sprite.Create(externalInterstitial.creativeImage,new Rect(0,0,externalInterstitial.creativeImage.width,externalInterstitial.creativeImage.height),new Vector2(0.5f,0.5f));
				//				
				//DoCoroutine("FadeInterstitialGroupAlpha");
				*/

				view.enabled = true;
				view.PrepareView(externalInterstitial.creativeImage,closeImage);
				SendInterstitialImpression(externalInterstitial);
				//fire off event
				TDinstance._didDisplayInterstitial("Interstitial Shown");
				showingInterstitial = true;

				hasLandscapeInterstitial = false;
			}
			else
			{
				Debug.Log("Has No LANDSCAPE interstitials");
			}
		}
		else
		{
			Debug.Log("Still loading Interstitials");
		}
	}

	public static TapdaqNativeAd GetNativeAd(TDNativeAdUnit adType, TDNativeAdSize adSize, TDOrientation orientation)
	{
		#if UNITY_IPHONE
		_FetchNative(BuildNativeAd,(int)adType, (int)adSize, (int)orientation);
		#endif	
		#if UNITY_ANDROID
		using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaClass mHumbleAssistantClass = new AndroidJavaClass("com.nerd.TapdaqUnityPlugin.TapdaqUnity"))
			{
				mHumbleAssistantClass.CallStatic("FetchNativeAd",(int)adType, (int)adSize, (int)orientation,new NativeAdFetchCallback());
			}
		}
		#endif

		return externalNative;
	}

	public static void SendInterstitialClick(TapdaqInterstitialAd ad)
	{
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			Debug.Log("Tapdaq Dummy: Send Interstitial Click");
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			#if UNITY_IPHONE
			_SendInterstitialClick(ad.pointer);
			#endif
		}
		
	}
	
	public static void SendInterstitialImpression(TapdaqInterstitialAd ad)
	{
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			Debug.Log("Tapdaq Dummy: Send Interstitial Impression");
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			#if UNITY_IPHONE
			_SendInterstitialImpression(ad.pointer);
			#endif
		}
	}

	//helpers

	public void _SendInterstitialClick()
	{
		SendInterstitialClick(externalInterstitial);
		//DismissInterstitial();
		showingInterstitial = false;
		view.Dismiss();
	}

	public void _SendInterstitialImpression()
	{
		SendInterstitialImpression(externalInterstitial);
	}

	public static void SendNativeImpression(TapdaqNativeAd ad)
	{
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			Debug.Log("Tapdaq Dummy: Send Native Impression");
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			#if UNITY_IPHONE
			_SendNativeImpression(ad.pointer);
			#endif

		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			#if UNITY_ANDROID
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject act = jc.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject appCtx = act.Call<AndroidJavaObject>("getApplicationContext");
				using (AndroidJavaClass mHumbleAssistantClass = new AndroidJavaClass("com.nerd.TapdaqUnityPlugin.TapdaqUnity"))
				{
					mHumbleAssistantClass.CallStatic("SendNativeImpression",ad.pointer,appCtx);
				}
			}
			#endif
		}
	}

	public static void SendNativeClick(TapdaqNativeAd ad)
	{
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			Debug.Log("Tapdaq Dummy: Send Native Click");
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			#if UNITY_IPHONE
			_SendNativeClick(ad.pointer);
			#endif

		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			#if UNITY_ANDROID
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject act = jc.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject appCtx = act.Call<AndroidJavaObject>("getApplicationContext");
				using (AndroidJavaClass mHumbleAssistantClass = new AndroidJavaClass("com.nerd.TapdaqUnityPlugin.TapdaqUnity"))
				{
					mHumbleAssistantClass.CallStatic("SendNativeClick",ad.pointer,appCtx);
				}
			}
			#endif
		}
	}

	public void DismissInterstitial()
	{
		if(showingInterstitial == true)
		{
			//group.alpha = 0;
			showingInterstitial = false;
			//interstitialCanvas.SetActive(false);

			//fire off event
			TDinstance._didCloseInterstitial("Interstitial Closed");
		}

	}

//	IEnumerator FadeInterstitialGroupAlpha()
//	{
//
//		float fade = 0;
//		while(fade <= 1.1f)
//		{
//			group.alpha = fade;
//			fade+=0.04f;
//			if(group.alpha >= 1) fade = 1.2f;
//			yield return new WaitForFixedUpdate();
//		}
//
//	}

	public static void DoCoroutine(string method)
	{
		TDinstance.StopCoroutine(method);
		TDinstance.StartCoroutine(method); //this will launch the coroutine on our instance
	}


	//Called by objc for every change of orientation
	public void _orientationChangeNotification(string msg)
	{
		//DismissInterstitial();
		showingInterstitial = false;
		view.Dismiss();
	}

	public void _applicationEnterBackgroundNotification(string msg)
	{
		//DismissInterstitial();
		showingInterstitial = false;
		view.Dismiss();
	}

//Delegate Events

	/// <summary>
	/// _notifies the plugin. Called when the sdk has interstitials ready for processing. Used internally only
	/// </summary>
	/// <param name="orientation">Orientation.</param>


	// Called before interstitial is shown
	public void _willDisplayInterstitial(string msg)
	{
		if(willDisplayInterstitial != null)
		{
			willDisplayInterstitial();
		}
	}

	// Called after interstitial is shown
	public void _didDisplayInterstitial(string msg)
	{
		if(didDisplayInterstitial != null)
		{
			didDisplayInterstitial();
		}
	}

	// Called when interstitial is closed
	public void _didCloseInterstitial(string msg)
	{
		if(didCloseInterstitial != null)
		{
			didCloseInterstitial();
		}
	}

	// Called when interstitial is clicked
	public void _didClickInterstitial(string msg)
	{
		//callbacks aint firing fomr android

		if(didClickInterstitial != null)
		{
			didClickInterstitial();
		}
	}

	// Called with an error occurs when requesting
	// interstitials from the Tapdaq servers
	public void _didFailToLoadInterstitial(string msg)
	{
		if(didFailToLoadInterstitial != null)
		{
			didFailToLoadInterstitial();
		}
	}

	// Called when the request for interstitials was successful,
	// but no interstitials were found
	public void _hasNoInterstitialsAvailable(string msg)
	{
		if(hasNoInterstitialsAvailable != null)
		{
			hasNoInterstitialsAvailable();
		}
	}

	// Called when the request for interstitials was successful
	// and 1 or more interstitials were found
	//NOTE - this event will be called everytime an interstitial is cached and ready to be shown. 
	//       which can happen multiple times in a few frames
	public void _hasInterstitialsAvailableForOrientation(string orientation)
	{
		if(hasInterstitialsAvailableForOrientation != null)
		{
			hasInterstitialsAvailableForOrientation(orientation);
		}
	}

	//NATIVE DELEGATES
	// Called with an error occurs when requesting
	// natives from the Tapdaq servers
	public void _didFailToLoadNativeAdverts()
	{
		if(didFailToLoadNative != null)
		{
			didFailToLoadNative();
		}
	}

	// Called when the request for natives was successful,
	// but no Natives of the requested spec were found
	public void _hasNoNativeAdvertsAvailable()
	{
		if(hasNoNativeAdvertAvailable != null)
		{
			hasNoNativeAdvertAvailable();
		}
	}
	
	// Called when the request for natives was successful
	// and 1 or more natives were found
	//NOTE - this event will be called everytime a native is cached and ready to be shown. 
	//       which can happen multiple times in a few frames
	public void _hasNativeAdvertsAvailableForAdUnit(string unit, string size, string orientation)
	{
		if(hasNativeAdvertsAvailableForAdUnit !=null)
		{
			hasNativeAdvertsAvailableForAdUnit(unit, size, orientation);
		}
	}

	public void _Android_hasNoNativeAdvertsAvailable(string msg)
	{
		if(hasNoNativeAdvertAvailable != null)
		{
			hasNoNativeAdvertAvailable();
		}
	}

	public void _Android_hasNativeAdvertsAvailableForUnit(string msg)
	{

		string[] adObject = msg.Split(new[]{"<>"},System.StringSplitOptions.None);

		string adUnit;
		string adSize;
		string adOrientation;

		switch(adObject[0])
		{
		case("_1X1"):
		{
			adUnit = "0";//"square";
			adOrientation = "2";//"universal";
		}
			break;

		case("_2X1"):
		case("_1X2"):
		{
			adUnit = "1";//"newsfeed";
			if(adObject[0] == "_2X1")
				adOrientation = "0";//"portrait";
			else
				adOrientation = "1";//"landscape";
		}
			break;

		case("_2X3"):
		case("_3X2"):
		{
			adUnit = "2";//"fullscreen";
			if(adObject[0] == "_2X3")
				adOrientation = "0";//"portrait";
			else
				adOrientation = "1";//"landscape";
		}
			break;

		case("_5X1"):
		case("_1X5"):
		{
			adUnit = "3";//"strip";
			if(adObject[0] == "_5X1")
				adOrientation = "0";//"portrait";
			else
				adOrientation = "1";//"landscape";
		}
			break;
		default:
			adUnit = "0";//"square";
			adOrientation = "0";//"portrait";
			break;
		}

		switch(adObject[1])
		{
		case("0"):
		{
			adSize = "0";//"small";
		}
			break;

		case("1"):
		{
			adSize = "1";//"medium";
		}
			break;

		case("2"):
		{
			adSize = "2";//"large";
		}
			break;

		default:
			adSize = "3";//"small";
			break;
		}


		if(hasNativeAdvertsAvailableForAdUnit !=null)
		{
			hasNativeAdvertsAvailableForAdUnit(adUnit, adSize, adOrientation);
		}
	}

	public void _FailedToConnectToServer(string msg)
	{
		if(didFailToConnectToServer != null)
		{
			didFailToConnectToServer(msg);
		}
	}

	public void _UnexpectedErrorHandler(string msg)
	{
		Debug.Log(msg);
	}

	

}
