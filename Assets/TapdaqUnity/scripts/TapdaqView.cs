using UnityEngine;
using System.Collections;

public class TapdaqView : MonoBehaviour {
	
	private float x1 = Screen.width*0.1f;
	private float y1 = Screen.height*0.1f;
	private float w1 = Screen.width*0.8f;
	private float h1 = Screen.height*0.8f;
	private Rect adRect = new Rect(Screen.width*0.1f,Screen.width*0.1f,Screen.width*0.8f,Screen.height*0.8f);
	private Rect closeRect = new Rect(Screen.width*0.8f,Screen.width*0.1f,Screen.width*0.2f,Screen.height*0.2f);

	private Rect bgRect;
	private Color color;
	private Color colorFade;
	private float fadeCounter = 0;

	private Texture adImage;
	private Texture closeImage;
	private Texture bgSquare;
	
	private bool imageReady = false;
	private bool clickable = false;
	private Tapdaq tdScript;
	
	void Start()
	{
		Tapdaq.didCloseInterstitial += Dismiss;
		tdScript = GetComponent<Tapdaq>();
		bgSquare = Resources.Load("tdSquare") as Texture;
		bgRect = new Rect(0,0,Screen.width,Screen.height);
	}
	
	public void PrepareView(Texture tex, Texture closeTex)
	{
		
		adImage = tex;
		closeImage = closeTex;
		
		float wp = adImage.width/(float)Screen.width;
		float hp = adImage.height/(float)Screen.height;
		float rwp = (1-wp)/2;
		float rhp = (1-hp)/2;
		
		if(adImage.width < Screen.width*0.9f)
		{
			x1 = Screen.width*rwp;
			y1 = Screen.height*rhp;
			w1 = Screen.width*wp;
			h1 = Screen.height*hp;
		}
		else{
			x1 = Screen.width*0.1f;
			y1 = Screen.height*0.1f;
			w1 = Screen.width*0.8f;
			h1 = Screen.height*0.8f;
		}
		
		adRect = new Rect(x1,y1,w1,h1);
		closeRect = new Rect((adRect.x+adRect.width)-(Screen.width*0.08f/2),adRect.y-(Screen.width*0.08f/2),Screen.width*0.08f,Screen.width*0.08f);
		
		if(closeRect.x >= Screen.width || closeRect.x < 0)
		{
			closeRect.x = 10;
		}
		
		if(closeRect.y > Screen.height || closeRect.y < 0)
		{
			closeRect.y  = 10;
		}
		bgRect = new Rect(0,0,Screen.width,Screen.height);
		
		
		imageReady = true;


		fadeCounter = 0;
		StartCoroutine( ClickDelay() );
	}
	
	public void Dismiss()
	{
		imageReady = false;
		enabled = false;
		Tapdaq.showingInterstitial = false;
		clickable = false;
	}
	
	IEnumerator ClickDelay()
	{
		yield return new WaitForSeconds(0.5f);
		clickable = true;
	}
	
	void OnGUI ()
	{
		colorFade = GUI.color;
		colorFade.a = fadeCounter;
		if(fadeCounter < 1)
		{
			fadeCounter += 0.5f*Time.deltaTime;
			colorFade.a = fadeCounter;
		}

		GUI.color = colorFade;

		color = GUI.backgroundColor;
		GUI.backgroundColor = Color.clear;

		if(adImage != null && closeImage != null)
		{
			if(imageReady)
			{
				GUI.DrawTexture(bgRect,bgSquare,ScaleMode.StretchToFill,true, 0);
				GUI.DrawTexture(adRect, adImage, ScaleMode.ScaleToFit, true,0);
				GUI.DrawTexture(closeRect,closeImage);
				
				if(clickable)
				{
					if (GUI.Button(closeRect,""))
					{
						Dismiss ();
					}
					
					if (GUI.Button(adRect,""))
					{
						if(Tapdaq.showingInterstitial)
						{
							tdScript._SendInterstitialClick();
						}
						Dismiss();
					}
					if (GUI.Button(bgRect,""))
					{
						Dismiss ();
					}
					
				}
				
			}
			
		}

		GUI.backgroundColor = color;
		colorFade.a = 1;
		GUI.color = colorFade;
		
	}
	
	
}
