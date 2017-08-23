using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScr : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }

   private void OnMouseOver()
   {
       //transform.GetComponent<MeshRenderer>().material.shader = Shader.Find("Toon/LitOutline");
       transform.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/cardShader_off");
   }

   private void OnMouseExit()
   {
      // transform.GetComponent<MeshRenderer>().material.shader = Shader.Find("Toon/Lit");
      transform.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/cardShader");
   }
}
