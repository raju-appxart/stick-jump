using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(TapdaqSettings))]
public class TapdaqSettingsEditor : Editor {

	private TapdaqSettings _tdSettings;
	public bool showLogs;
	public bool testMode;
	public string ios_applicationID;
	public string ios_clientKey;

	public string android_applicationID;
	public string android_clientKey;

	public int frequency;
	public int duration;

	public bool interstitials;
	//public bool interstitialPortrait;
	//public bool interstitialLandscape;

	public bool nativeSquareLarge;
	public bool nativeSquareMedium; 
	public bool nativeSquareSmall;
	public bool nativeNewsfeedPortraitLarge;
	public bool nativeNewsfeedPortraitMedium;
	public bool nativeNewsfeedPortraitSmall;
	public bool nativeNewsfeedLandscapeLarge;
	public bool nativeNewsfeedLandscapeMedium;
	public bool nativeNewsfeedLandscapeSmall;
	public bool nativeFullscreenPortraitLarge;
	public bool nativeFullscreenPortraitMedium; 
	public bool nativeFullscreenPortraitSmall;
	public bool nativeFullscreenLandscapeLarge;
	public bool nativeFullscreenLandscapeMedium;
	public bool nativeFullscreenLandscapeSmall;
	public bool nativeStripPortraitLarge;
	public bool nativeStripPortraitMedium;
	public bool nativeStripPortraitSmall;
	public bool nativeStripLandscapeLarge;
	public bool nativeStripLandscapeMedium;
	public bool nativeStripLandscapeSmall;

	[MenuItem("Tapdaq/Create Tapdaq in scene")]
	static void AddTrackerToScene()
	{
		object[] obj = GameObject.FindObjectsOfType(typeof (GameObject));
		foreach (GameObject o in obj)
		{
			if (o.GetComponent<Tapdaq>()) {
				Debug.LogError("Tapdaq already in scene.");
				return;}
		}
		GameObject holder = (GameObject)Instantiate(Resources.Load("TapdaqV1"));
		holder.name = "TapdaqV1";
		
	}


	void OnEnable () 
	{
		_tdSettings = (TapdaqSettings)target;
	}
	
	public override void OnInspectorGUI() {

		serializedObject.Update();
		GUILayout.Label ("You must have an App ID and Client Key to use Tapdaq", EditorStyles.boldLabel);
		
		if (GUILayout.Button ("Visit Tapdaq.com")) {
			Application.OpenURL ("https://tapdaq.com/dashboard/apps");
		}
		
		GUILayout.Label ("Ad Settings", EditorStyles.boldLabel);


		//Logs
		_tdSettings.showLogs = EditorGUILayout.Toggle ("Show Additional Logs",_tdSettings.showLogs);

		//Test mode
		_tdSettings.testMode = EditorGUILayout.Toggle ("TEST MODE",_tdSettings.testMode);
		GUILayout.Label ("", EditorStyles.boldLabel);
		
		//application ID + client key
		_tdSettings.ios_applicationID = EditorGUILayout.TextField ("iOS Application ID",_tdSettings.ios_applicationID);
		_tdSettings.ios_clientKey = EditorGUILayout.TextField ("iOS Client Key",_tdSettings.ios_clientKey);
		
		GUILayout.Label ("", EditorStyles.boldLabel);

		_tdSettings.android_applicationID = EditorGUILayout.TextField ("Android Application ID",_tdSettings.android_applicationID);
		_tdSettings.android_clientKey = EditorGUILayout.TextField ("Android Client Key",_tdSettings.android_clientKey);
		GUILayout.Label ("", EditorStyles.boldLabel);
		
		//------frequency
		_tdSettings.frequency = (int)EditorGUILayout.Slider ("Frequency", _tdSettings.frequency, 0, 2000);
		
		//-----duration
		_tdSettings.duration = (int)EditorGUILayout.Slider ("Duration", _tdSettings.duration, 0, 2000);

		GUILayout.Label ("", EditorStyles.boldLabel);
		//---GROUP TOGGLE
		//groupEnabled = EditorGUILayout.BeginToggleGroup ("Ad Type Settings", groupEnabled);
		GUILayout.Label ("Enabled Ad Types", EditorStyles.boldLabel);
		GUILayout.Label ("--Interstitials--", EditorStyles.boldLabel);


		//---------intersititals
		_tdSettings.interstitials = EditorGUILayout.Toggle ("\tInterstitials", _tdSettings.interstitials);
		
	
//		//---------intersitital portrait
//		_tdSettings.interstitialPortrait = EditorGUILayout.Toggle ("\tPortrait", _tdSettings.interstitialPortrait);
//
//		//------interstitial landscape
//		_tdSettings.interstitialLandscape = EditorGUILayout.Toggle ("\tLandscape", _tdSettings.interstitialLandscape);


		GUILayout.Label ("--Natives--", EditorStyles.boldLabel);
		GUILayout.Label ("\tSquare", EditorStyles.boldLabel);
		_tdSettings.nativeSquareLarge = EditorGUILayout.Toggle ("\tLarge", _tdSettings.nativeSquareLarge);
		_tdSettings.nativeSquareMedium = EditorGUILayout.Toggle ("\tMedium", _tdSettings.nativeSquareMedium);
		_tdSettings.nativeSquareSmall = EditorGUILayout.Toggle ("\tSmall", _tdSettings.nativeSquareSmall);

		GUILayout.Label ("\tNews Feed Portrait", EditorStyles.boldLabel);
		_tdSettings.nativeNewsfeedPortraitLarge = EditorGUILayout.Toggle ("\tLarge", _tdSettings.nativeNewsfeedPortraitLarge);
		_tdSettings.nativeNewsfeedPortraitMedium = EditorGUILayout.Toggle ("\tMedium", _tdSettings.nativeNewsfeedPortraitMedium);
		_tdSettings.nativeNewsfeedPortraitSmall = EditorGUILayout.Toggle ("\tSmall", _tdSettings.nativeNewsfeedPortraitSmall);

		GUILayout.Label ("\tNews Feed Landscape", EditorStyles.boldLabel);
		_tdSettings.nativeNewsfeedLandscapeLarge = EditorGUILayout.Toggle ("\tLarge", _tdSettings.nativeNewsfeedLandscapeLarge);
		_tdSettings.nativeNewsfeedLandscapeMedium = EditorGUILayout.Toggle ("\tMedium", _tdSettings.nativeNewsfeedLandscapeMedium);
		_tdSettings.nativeNewsfeedLandscapeSmall = EditorGUILayout.Toggle ("\tSmall", _tdSettings.nativeNewsfeedLandscapeSmall);

		GUILayout.Label ("\tFullscreen Portrait", EditorStyles.boldLabel);
		_tdSettings.nativeFullscreenPortraitLarge = EditorGUILayout.Toggle ("\tLarge", _tdSettings.nativeFullscreenPortraitLarge);
		_tdSettings.nativeFullscreenPortraitMedium = EditorGUILayout.Toggle ("\tMedium", _tdSettings.nativeFullscreenPortraitMedium); 
		_tdSettings.nativeFullscreenPortraitSmall = EditorGUILayout.Toggle ("\tSmall", _tdSettings.nativeFullscreenPortraitSmall);

		GUILayout.Label ("\tFullscreen Landscape", EditorStyles.boldLabel);
		_tdSettings.nativeFullscreenLandscapeLarge = EditorGUILayout.Toggle ("\tLarge", _tdSettings.nativeFullscreenLandscapeLarge);
		_tdSettings.nativeFullscreenLandscapeMedium = EditorGUILayout.Toggle ("\tMedium", _tdSettings.nativeFullscreenLandscapeMedium);
		_tdSettings.nativeFullscreenLandscapeSmall = EditorGUILayout.Toggle ("\tSmall", _tdSettings.nativeFullscreenLandscapeSmall);

		GUILayout.Label ("\tStrip Portrait", EditorStyles.boldLabel);
		_tdSettings.nativeStripPortraitLarge = EditorGUILayout.Toggle ("\tLarge", _tdSettings.nativeStripPortraitLarge);
		_tdSettings.nativeStripPortraitMedium = EditorGUILayout.Toggle ("\tMedium", _tdSettings.nativeStripPortraitMedium);
		_tdSettings.nativeStripPortraitSmall = EditorGUILayout.Toggle ("\tSmall", _tdSettings.nativeStripPortraitSmall);

		GUILayout.Label ("\tStrip Landscape", EditorStyles.boldLabel);
		_tdSettings.nativeStripLandscapeLarge = EditorGUILayout.Toggle ("\tLarge", _tdSettings.nativeStripLandscapeLarge);
		_tdSettings.nativeStripLandscapeMedium = EditorGUILayout.Toggle ("\tMedium", _tdSettings.nativeStripLandscapeMedium);
		_tdSettings.nativeStripLandscapeSmall = EditorGUILayout.Toggle ("\tSmall", _tdSettings.nativeStripLandscapeSmall);

		if(GUI.changed)
			EditorUtility.SetDirty(_tdSettings);

	}
}
