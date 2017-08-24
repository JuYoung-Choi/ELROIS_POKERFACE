using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTint : MonoBehaviour {

	UISprite Tintsprite;
	Color TintColor;

	bool color_Tint;
	int count_Tint;

	// Use this for initialization
	void Start () {
		Tintsprite = gameObject.GetComponent<UISprite> ();
		Tintsprite.color = new Color(1, 1, 1, 0); //검은색
		color_Tint = false;
		count_Tint = 0;
	}
	
	// Update is called once per frame
	void Update () {
		TintColor = Tintsprite.color;

		if (count_Tint > 6) {
			Tintsprite.color = new Color (1, 1, 1, 0);
			return;
		}

		if (color_Tint == true) {
			TintColor.a = Mathf.Lerp (TintColor.a, 1, Time.deltaTime * 1.25f);
			Tintsprite.color = TintColor;
			if (TintColor.a > 0.9)
				color_Tint = false;
		} else if (color_Tint == false) {
			TintColor.a = Mathf.Lerp (TintColor.a, 0, Time.deltaTime * 1.25f);
			Tintsprite.color = TintColor;
			if (TintColor.a < 0.05) {
				color_Tint = true;
				count_Tint++;
			}
		}
	}
}
