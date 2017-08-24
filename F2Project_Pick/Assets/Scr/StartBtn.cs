using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBtn : MonoBehaviour {

	public AudioSource Btn_sound;
	public AudioClip Btn_clip;

	public UISprite Player_Start_Btn;
	public UISprite Dealer_Start_Btn;
	public UISprite Cancle_Btn;

	public GameObject Wait;

	void Start(){
		Wait.SetActive (false);
	}

	//Player start button event
	public void Player_Start(){
		//Button sound	
		Btn_sound.clip = Btn_clip;
		Btn_sound.Play();

		Wait.SetActive (true);
	}

	//Dealer start button event
	public void Dealer_Start(){
		//Button sound	
		Btn_sound.clip = Btn_clip;
		Btn_sound.Play();

		Wait.SetActive (true);
	}

	public void Wait_Cancle(){
		//Button sound	
		Btn_sound.clip = Btn_clip;
		Btn_sound.Play();

		Wait.SetActive (false);
	}
}
