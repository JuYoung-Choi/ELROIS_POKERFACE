using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUI : MonoBehaviour {

	public GameObject P_panel;
	public GameObject D_panel;

	RaycastHit temp;

	public Camera m_Cam;

	// Use this for initialization
	void Start () {
		P_panel.SetActive (false);
		D_panel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Physics.Raycast (m_Cam.transform.position, m_Cam.transform.forward, out temp, 200.0f)) 
		{
			if (temp.transform.tag == "DealerPanel")
				Change_DealerUI ();

			else if (temp.transform.tag == "PlayerPanel")
				Change_PlayerUI ();

			/*
			if (hit.transform.name.ToLower().Contains("dealer")) 
			{
				Change_DealerUI ();
			}
			else if (hit.transform.name.ToLower().Contains("player"))
			{
				Change_PlayerUI ();
			}
			*/
		}
	}

	void Change_DealerUI(){
		P_panel.SetActive (false);
		D_panel.SetActive (true);
	}

	void Change_PlayerUI(){
		P_panel.SetActive(true);
		D_panel.SetActive (false);
	}
}
