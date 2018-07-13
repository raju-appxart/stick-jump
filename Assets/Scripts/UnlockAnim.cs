using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UnlockAnim : MonoBehaviour {

	public Button unlockBtn;
	public Sprite sprite1, sprite2;
	bool isChanged;

	void OnEnable()
	{
		InvokeRepeating("Animate", 0.35f, 0.35f);
	}

	void OnDisable()
	{
		CancelInvoke("Animate");
	}
	// Use this for initialization
	void Start () {
	}

	void Animate()
	{
		if(!isChanged)
		{
			isChanged = true;
			unlockBtn.image.sprite = sprite2;
		}
		else
		{
			isChanged = false;
			unlockBtn.image.sprite = sprite1;
		}
	}
}
