using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Heyzap;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

public class ShopController : MonoBehaviour {

	// Public variables
	public RectTransform bgPanel;
	public SpriteRenderer spinnerSprite;
	public RectTransform scrollContent;	//To hold scroll panel
	public Image [] characters;
	public RectTransform center; 	// Center to compare the distance for each button
	public Text nameOfChar;	//Current selection character's name
	public Text totalStarText, priceText;
	public RectTransform needText, pricePanel;
	public Button lockBtn, playBtn;
	public string []names;


	// Private variables
	private float[] distances;	// All buttons distances to the center
	private bool dragging;	// Will be true while we drag the panel
	private int buttonDistance;	// Will hold the distance between buttons
	private int currentSelectedChar;	// To hold the number of the button, with samllest distance to center, its current selection
	private bool[] purchaseInfo;	// Purchase info of characters
	Color fadedColor, highLightColor;


	// Use this for initialization
	void Start () 
	{
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
		spinnerSprite.sprite = Resources.Load <Sprite> ("Textures/Theme/" +"PF_" + themeIndex) as Sprite;

		int bttnLength = characters.Length;
		distances = new float[bttnLength];
		purchaseInfo = new bool[bttnLength];

		// Get the distance betwwen buttons
		buttonDistance = (int)Mathf.Abs(characters[1].GetComponent<RectTransform>().anchoredPosition.x 
		                                - characters[0].GetComponent<RectTransform>().anchoredPosition.x );


		totalStarText.text =  "" + Main.Instance.starsCnt;
		needText.gameObject.SetActive(false);
		GetPurchaseStatus();

		if(!Main.Instance.isNoads)
		{
			HZBannerShowOptions showOptions = new HZBannerShowOptions();
			showOptions.Position = HZBannerShowOptions.POSITION_TOP;
			HZBannerAd.ShowWithOptions(showOptions);
		}


		Main.Instance.shopScrollPosX = PlayerPrefs.GetFloat("ScrolPos", 0);
		scrollContent.anchoredPosition = new Vector2(Main.Instance.shopScrollPosX, 0);
	}

	void GetPurchaseStatus()
	{
		fadedColor = new Color(100/255.0f, 100/255.0f, 100/255.0f, 200/255.0f); 
		highLightColor = new Color(1, 1, 1, 1); 
		string purchaseKey = "Char_" + 0;
		for(int i = 0; i < characters.Length; i++)
		{
			bool isPurchased = false;
			purchaseKey = "Char_" + i;
			if(i == 0)
			{
				isPurchased = true;
			}
			else
			{
				isPurchased = PlayerPrefs.GetBool(purchaseKey, false);
			}

			if(!isPurchased)
			{
				characters[i].color = fadedColor; 
			}
			else
			{
				characters[i].color = highLightColor; 
//				if(Main.Instance.playerID == i)
				{
					//TODO: Adjust scroll content to show current selection
//					currentSelction = GameData.currentBirdId;
//					isSelected = true;
//					bgImage.color = new Color(58/255.0f, 191/255.0f, 236/255.0f);
				}
			}

			purchaseInfo[i] = isPurchased;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{

		for(int i = 0; i < characters.Length; i++)
		{
			distances[i] = Mathf.Abs(center.transform.position.x - characters[i].transform.position.x);
			if(i == currentSelectedChar)
			{
				characters[i].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1f);
				nameOfChar.text = names[i];
				if(purchaseInfo[i])
				{
					playBtn.gameObject.SetActive(true);
					lockBtn.gameObject.SetActive(false);
					characters[i].color = highLightColor; 
					priceText.text = "";
					pricePanel.gameObject.SetActive(false);
				}
				else
				{
					playBtn.gameObject.SetActive(false);
					lockBtn.gameObject.SetActive(true);
					characters[i].color = fadedColor; 
					if(currentSelectedChar > 2)
					{
						priceText.text = "100  TO UNLOCK";
					}
					else
					{
						priceText.text = "20  TO UNLOCK";
					}
					pricePanel.gameObject.SetActive(true);
				}


			}
			else
			{
				characters[i].GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
				if(purchaseInfo[i])
				{
					characters[i].color = highLightColor; 
				}
				else
				{
					characters[i].color = fadedColor; 
				}

			}
		}

		float minDistance = Mathf.Min(distances);	// Get the min distance

		for(int a = 0; a < characters.Length; a++)
		{
			if(minDistance == distances[a])
			{
				currentSelectedChar = a;
			}
		}

		if(!dragging)
		{
			LerpToButton(currentSelectedChar * - buttonDistance);
		}
	}

	void LerpToButton(int position)
	{
		float newX = Mathf.Lerp(scrollContent.anchoredPosition.x, position, Time.deltaTime*15f);
		Vector2 newPosition = new Vector2(newX, scrollContent.anchoredPosition.y);
		scrollContent.anchoredPosition = newPosition;

	}

	public void StartDrag()
	{
		needText.gameObject.SetActive(false);
		dragging = true;
	}

	public void EndDrag()
	{
		dragging = false;
	}

	public void BuyCharacter()
	{
		if(dragging)
			return;

		Main.Instance.PlayButtonSound();

		string purchaseKey = "Char_" + currentSelectedChar;
		bool isPurchased = PlayerPrefs.GetBool(purchaseKey, false);
		if(!isPurchased)
		{
			if(currentSelectedChar >2 && Main.Instance.starsCnt >= Constants.PRICE_CHAR)
			{
				playBtn.gameObject.SetActive(true);
				lockBtn.gameObject.SetActive(false);
				characters[currentSelectedChar].color = highLightColor;
				Main.Instance.starsCnt -= Constants.PRICE_CHAR;
				totalStarText.text =  "" + Main.Instance.starsCnt;
				purchaseInfo[currentSelectedChar] = true;
				PlayerPrefs.SetBool(purchaseKey, true);
			}
			else if(currentSelectedChar <=2 && Main.Instance.starsCnt >= Constants.PRICE_LOW)
			{
				playBtn.gameObject.SetActive(true);
				lockBtn.gameObject.SetActive(false);
				characters[currentSelectedChar].color = highLightColor;
				Main.Instance.starsCnt -= Constants.PRICE_LOW;
				totalStarText.text =  "" + Main.Instance.starsCnt;
				purchaseInfo[currentSelectedChar] = true;
				PlayerPrefs.SetBool(purchaseKey, true);
			}
			else
			{
	 			needText.gameObject.SetActive(true);
			}
			
		}
	}

	public void PlayGame()
	{
		Main.Instance.PlayButtonSound();
		Main.Instance.playerID = currentSelectedChar;
		PlayerPrefs.SetInt(Constants.PLAYER_ID, Main.Instance.playerID);
		PlayerPrefs.Flush();
		Main.Instance.shopScrollPosX = scrollContent.anchoredPosition.x;
		PlayerPrefs.SetFloat("ScrolPos", Main.Instance.shopScrollPosX);
		Application.LoadLevelAsync("Play");
	}

	public void ExitStore()
	{
		Main.Instance.PlayButtonSound();
		PlayerPrefs.Flush();
		Application.LoadLevelAsync("Play");
	}

}
