using UnityEngine;
using System.Collections;

public class TapdaqInterstitialAd {
	
	
	//This class exposes the tapdaq ad class with a much more accessable constructor
	
	public string applicationId {get; set;}
	
	public string targetingId {get; set;}
	
	public string subscriptionId {get; set;} // (optional) 
	
	
	public string creativeIdentifier {get; set;}
	
	public Tapdaq.TDOrientation creativeOrientation {get; set;} // Can be either `TDOrientationPortrait` or `TDOrientationLandscape
	
	public string creativeResolution {get; set;} // Can be `TDResolution1x`, `TDResolution2x` or `TDResolution3x`
	
	
	public int creativeAspectRatioWidth {get; set;}
	
	public int creativeAspectRatioHeight {get; set;}
	
	public string creativeURL {get; set;}
	
	public Texture2D creativeImage {get; set;}
	
	public int pointer{get; set;}

	public TapdaqInterstitialAd()
	{
		
		applicationId = "This is a test interstitial ad";
		targetingId = "test a";
		subscriptionId = "test b";
		
		creativeIdentifier = "test c";
		creativeOrientation = Tapdaq.TDOrientation.landscape;
		creativeResolution = "2x test";
		
		creativeAspectRatioWidth = 0;
		creativeAspectRatioHeight = 0;
		
		creativeURL = "http://thisisatest.com";
		creativeImage = null;
		pointer = 0;
		
		
	}
	
	public TapdaqInterstitialAd Build(Tapdaq.TDinterstitialAd _adObject)
	{
		applicationId = _adObject.applicationId;
		targetingId = _adObject.targetingId;
		subscriptionId = _adObject.subscriptionId;
		
		creativeIdentifier = _adObject.creativeIdentifier;
		creativeOrientation = _adObject.creativeOrientation;
		creativeResolution = _adObject.creativeResolution;
		
		creativeAspectRatioWidth = _adObject.creativeAspectRatioWidth;
		creativeAspectRatioHeight = _adObject.creativeAspectRatioHeight;
		
		creativeURL = _adObject.creativeURL;
		creativeImage = _adObject.creativeImage;
		pointer = _adObject.pointer;
		
		return this;
	}
	
}
