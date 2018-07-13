using UnityEngine;
using System.Collections;

public class ObstacleRoot : MonoBehaviour {

	public Sprite [] allObstacles;
	public SpriteRenderer[] obstacles;

	bool isDeathOnBottom, noDownPower, isScored;

	// Use this for initialization
	void Start () 
	{
		if(Random.Range(0, 10) < 7)
		{
			noDownPower = true;
		}
		isDeathOnBottom = false;
		isScored = false;
		SetObstacles();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(transform.position.x < -3.35f)
		{
			Destroy(gameObject);
		}
		if(!isScored && transform.position.x < -1.4f)
		{
			isScored = true;
			PlayerControl.gameScore++;
		}
	}

	public void SetObstacles()
	{
		int index = 0;
		for(int i = 0; i < obstacles.Length; i++)
		{

			index = Random.Range(0, allObstacles.Length);

			if(i == 0 && index == (int)ObstacleTags.Death)
			{
				//To avoid bottom & mid obstacle as both death 
				isDeathOnBottom = true;
			}
			else if(isDeathOnBottom && i == 1)
			{
				index = Random.Range(0, allObstacles.Length-1);
			}
			else if(index == (int)ObstacleTags.Down && noDownPower)
			{
				index = Random.Range(0, allObstacles.Length-2);
			}
			obstacles[i].sprite = allObstacles[index];
			obstacles[i].tag = ((ObstacleTags)index).ToString();
		}
	}
}


public enum ObstacleTags
{
	None = -1,
	Up,
	Star,
	Down,
	Death,
	Count
}
