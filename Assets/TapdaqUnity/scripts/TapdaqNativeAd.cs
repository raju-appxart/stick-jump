using UnityEngine;
using System.Collections;

public class TapdaqNativeAd {

	//This class exposes the tapdaq ad class with a much more accessable constructor
	
	public string applicationId {get;private set;}
	
	public string targetingId {get;private set;}
	
	public string subscriptionId {get;private set;} // (optional) 
	//
	
	//--native specific members
	
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
	
	public Tapdaq.TDNativeAdUnit adUnit {get;private set;} // Can be either `TDNativeAdUnitSquare`, `TDNativeAdUnitNewsfeed`, `TDNativeAdUnitFullscreen`, `TDNativeAdUnitStrip`
	
	public Tapdaq.TDNativeAdSize adSize {get;private set;} // Can be either `TDNativeAdSizeSmall`, `TDNativeAdSizeMedium`, `TDNativeAdSizeLarge`
	
	public string iconUrl {get;private set;}
	
	public Texture2D icon {get;private set;}
	//---
	
	public string creativeIdentifier {get;private set;}
	
	public Tapdaq.TDOrientation creativeOrientation {get;private set;} // Can be either `TDOrientationPortrait` or `TDOrientationLandscape
	
	public string creativeResolution {get;private set;} // Can be `TDResolution1x`, `TDResolution2x` or `TDResolution3x`
	
	
	public int creativeAspectRatioWidth {get;private set;}
	
	public int creativeAspectRatioHeight {get;private set;}
	
	public string creativeURL {get;private set;}
	
	public Texture2D creativeImage {get;private set;}
	
	public int pointer{get;private set;}
	
	
	//Constructor
	
	public TapdaqNativeAd()
	{
			
		applicationId = "";
		targetingId = "";
		subscriptionId ="";

		appName = "This is a test native ad";
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
		adUnit = Tapdaq.TDNativeAdUnit.square; // Can be either `TDNativeAdUnitSquare`, `TDNativeAdUnitNewsfeed`, `TDNativeAdUnitFullscreen`, `TDNativeAdUnitStrip`
		adSize = Tapdaq.TDNativeAdSize.small; // Can be either `TDNativeAdSizeSmall`, `TDNativeAdSizeMedium`, `TDNativeAdSizeLarge`
		iconUrl = "http://thisisatesticonurl.com";
		icon = null;
			
		creativeIdentifier = "";
		creativeOrientation = Tapdaq.TDOrientation.landscape;
		creativeResolution = "";
			
		creativeAspectRatioWidth = 1;
		creativeAspectRatioHeight = 1;
			
		creativeURL = "http://thisisatest.com";
		creativeImage = null;
		pointer = 0;
	}

	public TapdaqNativeAd Build(Tapdaq.TDNativeAd _adObject)
	{
		applicationId = _adObject.applicationId;
		targetingId = _adObject.targetingId;
		subscriptionId = _adObject.subscriptionId;

		appName = _adObject.appName;
		appDescription = _adObject.appDescription;
		buttonText = _adObject.buttonText;
		developerName = _adObject.developerName;
		ageRating = _adObject.ageRating;
		appSize = _adObject.appSize;
		averageReview = _adObject.averageReview;
		totalReviews = _adObject.totalReviews;
		category = _adObject.category;
		appVersion = _adObject.appVersion;
		price = _adObject.price;
		currency = _adObject.currency;
		adUnit = _adObject.adUnit; // Can be either `TDNativeAdUnitSquare`, `TDNativeAdUnitNewsfeed`, `TDNativeAdUnitFullscreen`, `TDNativeAdUnitStrip`
		adSize = _adObject.adSize; // Can be either `TDNativeAdSizeSmall`, `TDNativeAdSizeMedium`, `TDNativeAdSizeLarge`
		iconUrl = _adObject.iconUrl;
		icon = _adObject.icon;
		
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
