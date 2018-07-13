using UnityEngine;
using System.Collections;
using PlayerPrefs = PreviewLabs.PlayerPrefs;
using System;
using System.IO;
#if UNITY_ANDROID
using Prime31;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
using UnityEngine.SocialPlatforms.GameCenter;
using System.Runtime.InteropServices;
using Heyzap; 


public class Main : MonoBehaviour {

	[HideInInspector]
	public Texture2D MyImage;

	public AudioSource audioSource, bgAudioSource;
	public AudioClip buttonSound, blastSound, jumpSound, duckSound ;


	[HideInInspector]
	public bool isTutorial, isSound, isNoads, isFBLiked;

	[HideInInspector]
	public int bestScore, starsCnt, totalGames;

	[HideInInspector]
	public int playerID;

	[HideInInspector]
	public float shopScrollPosX;



	const float splashHoldTime = 2.0f;

	//Android Notifications
	private int _fourHrNotificationId;
	private int _eightHrNotificationId;
	private int _oneDayNotificationId;
	private int _threeDayNotificationId;
	private int _sevenDayNotificationId;
	private int _fourteenDayNotificationId;
	private int _thirtyDayNotificationId;
	
	long fourHr = 	14400;	//14400
	long eightHr = 28800;
	long oneDay = 86400;
	long threeDay = 259200;
	long sevenDay = 604800;
	long fourteenDay = 1209600;
	long thirtyDay = 2592000;
	
	string msg_h4 = "Ready to continue the challenge? Play Stick Jump!";
	string msg_h8 = "Take a break, set a new high score. Play Stick Jump!";
	string msg_d1 = "Play Stick Jump and unlock new characters!";
	string msg_d3 = "Can you beat your friend’s high score on Stick Jump? Play now!";
	string msg_d7 = "Speed and skill is what it takes to win.  Play Stick Jump!";
	string msg_d14 = "Collect stars to unlock fun characters, Stick Jump awaits!";
	string msg_d30 = "We’ve new characters, check out Stick Jump!";


	private static Main instance = null;
	public static Main Instance 
	{
		get { return instance; }
	}
	
	void Awake() 
	{
		if (instance != null && instance != this) 
		{
			Destroy(this.gameObject);
			return;
		} 
		else 
		{
			instance = this;
		}
		
	}

	// Use this for initialization
	void Start () 
	{
		Application.targetFrameRate = 60;
		PlayerPrefs.EnableEncryption(true);
		Instance.isTutorial = PlayerPrefs.GetBool(Constants.TUTORIAL, true);
		Instance.bestScore = PlayerPrefs.GetInt(Constants.BEST_SCORE, 0);
		Instance.starsCnt = PlayerPrefs.GetInt(Constants.STAR_TOTAL, 0);
		Instance.totalGames = PlayerPrefs.GetInt(Constants.GAME_COUNT, 0);
		Instance.playerID = PlayerPrefs.GetInt(Constants.PLAYER_ID, 0);
		Instance.isSound = PlayerPrefs.GetBool(Constants.SOUND, true);
		Instance.isNoads = PlayerPrefs.GetBool(Constants.NO_ADS, false);
		Instance.isFBLiked = PlayerPrefs.GetBool(Constants.FB_LIKE, false);

		audioSource  = transform.GetComponent<AudioSource>();
		#if UNITY_ANDROID
		PlayGamesPlatform.Activate();
		EtceteraAndroid.cancelNotification( Constants.H4 );
		EtceteraAndroid.cancelNotification( Constants.H8 );
		EtceteraAndroid.cancelNotification( Constants.D1 );
		EtceteraAndroid.cancelNotification( Constants.D3 );
		EtceteraAndroid.cancelNotification( Constants.D7 );
		EtceteraAndroid.cancelNotification( Constants.D14 );
		EtceteraAndroid.cancelNotification( Constants.D30 );
		var key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA19JDG1hIuDeEDYaOxZyqYlv4OgX5+7+/+AcTRNMMhImND9UZsa2Ymh7xzQVUPYlyXOVSBW3RbenhZXWJPA3ZXZvk68hMACGIWGe2ZEFZqYiSELnk1C4XmKeTReC8Z5Y/NgSpSyzI++rvsSuA4j3Xe6SL3lbGUa+eOzmi42yJFU91zWdje6nA2XQFmMk6KqyaRT4Dwh2ntssPP0mQl6JiwJ8nSfp8JodIepYndBgNQ/OLBcko5ktFl1fIzqHJU0ZP6K7kF6i+vPe8H5M3Oal2Mw15/zfLIN+nE1R2IbauOQS7DmCKrMxTqPjUbajbOXJ4An77IhtwSrMkksDjet3/2wIDAQAB";
		GoogleIAB.init( key );
		
		#elif UNITY_IOS 
		
		UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert|UnityEngine.iOS.NotificationType.Badge);
		
		UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications ();
		
		// clear badge number
		UnityEngine.iOS.LocalNotification temp = new UnityEngine.iOS.LocalNotification();
		temp.fireDate = DateTime.Now;
		temp.applicationIconBadgeNumber = -1;
		temp.alertBody = "";
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(temp);
		#endif

		//Initialize Heyzap
		HeyzapAds.Start("790edd987b2facf1eb7117dfc97d354b", HeyzapAds.FLAG_NO_OPTIONS);

		Invoke("SignIn", 0.5f);
		Invoke("Play", splashHoldTime);
		DontDestroyOnLoad(gameObject);



	}

	void Play()
	{
		if(Instance.totalGames == 0)
		{
			Application.LoadLevelAsync("Tutorial");
		}
		else
		{
			Main.Instance.PlayBGMusic();
			Application.LoadLevelAsync("Play");
		}

	}
	
	void OnApplicationPause(bool pauseStatus) 
	{
		if(pauseStatus)
		{
			#if UNITY_ANDROID
			Main.Instance._fourHrNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.fourHr, "Stick Jump", Main.Instance.msg_h4, "Stick Jump", "four-hour-note", "small_icon", "large_icon", Constants.H4 );
//			Main.Instance._eightHrNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.eightHr, "Stick Jump", Main.Instance.msg_h8, "Stick Jump", "eight-hour-note",  "small_icon", "large_icon",Constants.H8 );
			Main.Instance._oneDayNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.oneDay, "Stick Jump", Main.Instance.msg_d1, "Stick Jump", "one-day-note", "small_icon", "large_icon", Constants.D1 );
			Main.Instance._threeDayNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.threeDay, "Stick Jump", Main.Instance.msg_d3, "Stick Jump", "three-day-note", "small_icon", "large_icon", Constants.D3 );
			Main.Instance._sevenDayNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.sevenDay, "Stick Jump", Main.Instance.msg_d7, "Stick Jump", "seven-day-note", "small_icon", "large_icon", Constants.D7 );
			Main.Instance._fourteenDayNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.fourteenDay, "Stick Jump", Main.Instance.msg_d14, "Stick Jump", "fourteen-day-note", "small_icon", "large_icon", Constants.D14 );
			Main.Instance._thirtyDayNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.thirtyDay, "Stick Jump", Main.Instance.msg_d30, "Stick Jump", "thirty-day-note", "small_icon", "large_icon", Constants.D30 );
			#elif UNITY_IOS 
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_h4, 4));
			//				UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_h8, 8));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_d1, 24));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_d3, 72));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_d7, 168));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_d14, 336));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_d30, 720));
			#endif
		}
		else
		{
			#if UNITY_ANDROID
			EtceteraAndroid.cancelNotification( Constants.H4 );
//			EtceteraAndroid.cancelNotification( Constants.H8 );
			EtceteraAndroid.cancelNotification( Constants.D1 );
			EtceteraAndroid.cancelNotification( Constants.D3 );
			EtceteraAndroid.cancelNotification( Constants.D7 );
			EtceteraAndroid.cancelNotification( Constants.D14 );
			EtceteraAndroid.cancelNotification( Constants.D30 );

			#elif UNITY_IOS
			NotificationServices.ClearLocalNotifications();
			NotificationServices.CancelAllLocalNotifications ();
			#endif
		}
		
	}
	
	
	void OnApplicationQuit()
	{
		
		#if UNITY_ANDROID
		Main.Instance._fourHrNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.fourHr, "Stick Jump", Main.Instance.msg_h4, "Stick Jump", "four-hour-note", "small_icon", "large_icon", Constants.H4 );
//		Main.Instance._eightHrNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.eightHr, "Stick Jump", Main.Instance.msg_h8, "Stick Jump", "eight-hour-note", "small_icon", "large_icon", Constants.H8 );
		Main.Instance._oneDayNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.oneDay, "Stick Jump", Main.Instance.msg_d1, "Stick Jump", "one-day-note", "small_icon", "large_icon", Constants.D1 );
		Main.Instance._threeDayNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.threeDay, "Stick Jump",Main.Instance.msg_d3, "Stick Jump", "three-day-note", "small_icon", "large_icon", Constants.D3 );
		Main.Instance._sevenDayNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.sevenDay, "Stick Jump", Main.Instance.msg_d7, "Stick Jump", "seven-day-note", "small_icon", "large_icon", Constants.D7 );
		Main.Instance._fourteenDayNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.fourteenDay, "Stick Jump", Main.Instance.msg_d14, "Stick Jump", "fourteen-day-note","small_icon", "large_icon", Constants.D14 );
		Main.Instance._thirtyDayNotificationId = EtceteraAndroid.scheduleNotification( Main.Instance.thirtyDay, "Stick Jump", Main.Instance.msg_d30, "Stick Jump", "thirty-day-note", "small_icon", "large_icon", Constants.D30 );
		#elif UNITY_IOS 
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_h4, 4));
		//			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_h8, 8));
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_d1, 24));
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_d3, 72));
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_d7, 168));
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_d14, 336));
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(Main.Instance.msg_d30, 720));
		#endif
		
		PlayerPrefs.Flush();
		
	}

	#region Game Center
	void SignIn()
	{
		#if UNITY_ANDROID
		Social.localUser.Authenticate((bool success) => {
			// handle success or failure
			if(success)
			{
				Main.Instance.PostScoreToLeaderBoard(Main.Instance.bestScore);
			}
			else
			{
				Debug.Log("LOG_IN FALED Play Service Google");
				
			}
			
			
		});
		
		#elif UNITY_IOS
		Social.localUser.Authenticate (Main.Instance.ProcessAuthentication);
		#endif
	}


	public void ShowLeaderBoard()
	{
		if (Social.localUser.authenticated) 
		{
			//			Debug.Log("showing LB UI");
			#if UNITY_ANDROID
			((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(Constants.LEADER_BOARD_ID);
			#elif UNITY_IOS
			Social.ShowLeaderboardUI();
			#endif
		}
		else
		{
			//			Debug.Log("Not signed so login");
			SignIn();
		}
		
		
	}
	
	public void PostScoreToLeaderBoard( int score )
	{
		
		if (Social.localUser.authenticated) 
		{
			#if UNITY_ANDROID
			Social.ReportScore (score, Constants.LEADER_BOARD_ID, (bool success) =>
			                    {
				if (success) 
				{
					
				} 
				else 
				{
					//					Debug.Log ("Add Score Fail");
				}
			});
			
			#elif UNITY_IOS
			Social.ReportScore (score, Constants.LEADER_BOARD_ID, (bool success) =>
			                    {
				if (success) 
				{
					
				} 
				else 
				{
					//					Debug.Log ("Add Score Fail");
				}
			});
			#endif
		} 
		
	}

	#endregion

	#if UNITY_IOS
	UnityEngine.iOS.LocalNotification getNotification(string notif, int time)
	{
		UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification();
		notification.fireDate = DateTime.Now.AddHours(time);
		//		notification.applicationIconBadgeNumber = 1;
		notification.alertBody = notif;
		notification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
		return notification;
	}
	
	void ProcessAuthentication (bool success) 
	{
		if (success) 
		{
			//			Debug.Log ("Authenticated, checking achievements");
			
			Main.Instance.PostScoreToLeaderBoard(Main.Instance.bestScore);
		}
		else
			Debug.Log ("Failed to authenticate");
	}
	
	#endif

	public void FBConnect()
	{
		#if UNITY_ANDROID
		Application.OpenURL("fb://page/135020303263673");	//Puissant page
		#elif UNITY_IOS
//		GotoFacebookPage("1413755088947982");	//1Touch Fb Page
		GotoFacebookPage("478546532205724");	
		#endif
	}

	#region General Share Delegates

	#if UNITY_IPHONE

	[DllImport("__Internal")]
	private static extern void MediaShareIos (string iosPath, string message);

	[DllImport("__Internal")]
	private static extern void TextShareIos (string message);

	[DllImport("__Internal")]
	private static extern void GotoFacebookPage(string pageID);
	#endif


	string pathToShareImg;
	string subject = "Come & Play this Awesome Game with Me!";
	string appUrl = "http://onelink.to/stickjump";
	string body = "" ;

	IEnumerator SaveScreenShot()
	{

		// create the texture
		yield return new WaitForEndOfFrame();
		MyImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24,true);//* 0.38f
		// put buffer into texture
		MyImage.ReadPixels(new Rect(0f, 0f , Screen.width, Screen.height),0,0);// * 0.8f
		// apply
		MyImage.Apply();
		yield return new WaitForEndOfFrame();
		byte[] bytes = MyImage.EncodeToPNG();
		pathToShareImg = Application.persistentDataPath + "/stickShare.png";
		File.WriteAllBytes(pathToShareImg, bytes);
		yield return new WaitForEndOfFrame();

		#if UNITY_ANDROID
		StartCoroutine(Main.Instance.ShareAndroidMedia());

		#elif UNITY_IPHONE
		MainDriver.Instance.ShareIosMedia();


		#endif

	}


	public  void ShareUniversal(int score)
	{
		Main.Instance.body = "I Scored " + score + " in #stickjump. Can you beat my score? @puissantapps " ;
		Main.Instance.body += Main.Instance.appUrl;
		StartCoroutine(Main.Instance.SaveScreenShot());

	}



	IEnumerator ShareAndroidText()
	{
		yield return new WaitForEndOfFrame();
		#if UNITY_ANDROID
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		intentObject.Call<AndroidJavaObject>("setType", "text/plain");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), Main.Instance.subject);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Egg Drop");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), Main.Instance.body);

		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		currentActivity.Call("startActivity", intentObject);
		#endif
	}


	IEnumerator ShareAndroidMedia ()
	{

		yield return new WaitForEndOfFrame();
		#if UNITY_ANDROID

		Debug.Log("ShareAndroidMedia");


		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		intentObject.Call<AndroidJavaObject>("setType", "image/*");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), Main.Instance.subject);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Stick Jump");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), Main.Instance.body);

		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaClass fileClass = new AndroidJavaClass("java.io.File");

		AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", pathToShareImg);// Set Image Path Here

		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);

		bool fileExist = fileObject.Call<bool>("exists");
		if (fileExist)
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		currentActivity.Call("startActivity", intentObject);


		#endif

	}

	void ShareIosMedia()
	{
		#if UNITY_IPHONE
		byte[] bytes = MyImage.EncodeToPNG();
		//		string path = Application.persistentDataPath + "/MyImage.png";
		File.WriteAllBytes(pathToShareImg, bytes);
		//		string path_ =  "MyImage.png";
		//		StartCoroutine(ScreenshotHandler.Save(path_, "Media Share", true));
		string shareMessage = MainDriver.Instance.body;
		MediaShareIos (path, shareMessage);
		#endif

	}

	void ShareIosText()
	{
		string shareMessage = Main.Instance.body;
		#if UNITY_IPHONE
		TextShareIos(shareMessage);
		#endif
	}


	#endregion


	#region Sounds
	public void PlayBGMusic()
	{
		if(Instance.bgAudioSource != null && Main.Instance.isSound)
		{
			Instance.bgAudioSource.volume = 0.3f;
			Instance.bgAudioSource.Play();
		}
		else
		{
			Debug.Log("Sound source is null");
		}
	}

	public void StopBGMusic()
	{
		if(Instance.bgAudioSource != null)
		{
			Instance.bgAudioSource.Stop();
		}
	}

	public void PlayButtonSound()
	{
		if(Instance.audioSource != null && Main.Instance.isSound)
		{
			Instance.audioSource.PlayOneShot(Main.Instance.buttonSound);
		}

	}
	
	public void PlayBlastSound()
	{
		if(Instance.audioSource != null && Main.Instance.isSound)
		{
			Instance.audioSource.PlayOneShot(Main.Instance.blastSound);
		}
	}
	
	public void PlayJumpSound()
	{
		if(Instance.audioSource != null && Main.Instance.isSound)
		{
			Instance.audioSource.PlayOneShot(Main.Instance.jumpSound);
		}
	}

	public void PlayDuckSound()
	{
		if(Instance.audioSource != null && Main.Instance.isSound)
		{
			Instance.audioSource.PlayOneShot(Main.Instance.duckSound);
		}
	}
	
	#endregion


}
