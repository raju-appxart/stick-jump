using UnityEngine;
using System.Collections;

public class Constants  {

#if UNITY_IOS
	public const string LEADER_BOARD_ID = "com.appsolutegames.stickjump.leaderboard";
	public const string PRODUCT_ID = "com.appsolutegames.stickjump.noads";

#elif UNITY_ANDROID
	public const string LEADER_BOARD_ID = "CgkI9rHY2oYcEAIQAA";
	public const string PRODUCT_ID = "remove_ads";
#endif

	//Noification related constants
	public const int H4 = 4;
	public const int H8 = 8;
	public const int D1 = 1;
	public const int D3 = 3;
	public const int D7 = 7;
	public const int D14 = 14;
	public const int D30 = 30;

	//Tags
	public const string TAG_SPINNER = "Spinner";
	public const string TAG_PLAYER = "Player";
	public const string TAG_SPEED_UP = "Up";
	public const string TAG_SPEED_DOWN = "Down";
	public const string TAG_BONUS = "Star";
	public const string TAG_DEATH = "Death";

	//Spinner Speeds
	public const float START_SPEED = 1.2f;
	public const float MIN_SPEED = 1.0f;
	public const float MAX_SPEED = 2.0f;// 1.6f;
	public const float SPEED_CHANGE = 0.2f;



	//Rewards
	public const int REWARD_STARS = 10;

	//Shop
	public const int PRICE_LOW = 20;
	public const int PRICE_CHAR = 100;

	//Player prefs
	public const string PLAYER_ID = "Pid";
	public const string STAR_TOTAL = "Stars";
	public const string BEST_SCORE = "Best";
	public const string TUTORIAL = "Help";
	public const string GAME_COUNT = "Games";
	public const string SOUND = "Sound";
	public const string NO_ADS = "Noad";
	public const string FB_LIKE = "FbLike";
}
