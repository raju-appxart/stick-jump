using UnityEngine;
using System.Collections;

public class InterstitialOnBootup : MonoBehaviour {
	
    private bool hasShowedInterstitial;
    private Tapdaq tapdaq;

    void OnEnable(){

		Tapdaq.hasInterstitialsAvailableForOrientation += DisplayInterstitialWhenAvailable;
		Tapdaq.didCloseInterstitial += DidCloseInterstitial;
        hasShowedInterstitial = false;
        tapdaq = GetComponent<Tapdaq>();

    }

    void OnDisable(){

		Tapdaq.hasInterstitialsAvailableForOrientation -= DisplayInterstitialWhenAvailable;
		Tapdaq.didCloseInterstitial -= DidCloseInterstitial;
    }

    void DidCloseInterstitial(){


//		Invoke ("SetHasSHowedInterstitial", 3);


    }

    void DisplayInterstitialWhenAvailable(string orientation){
		

        if(!hasShowedInterstitial){

            var orientationNumber = int.Parse(orientation);

            if(Screen.width < Screen.height && orientationNumber == 0){
                Tapdaq.ShowInterstitial();
                hasShowedInterstitial = true;
            }
            if(Screen.width > Screen.height && orientationNumber == 1){
                Tapdaq.ShowInterstitial();
                hasShowedInterstitial = true;

            }


        }

    }

	void SetHasSHowedInterstitial(){
		hasShowedInterstitial = false;
	}

}
