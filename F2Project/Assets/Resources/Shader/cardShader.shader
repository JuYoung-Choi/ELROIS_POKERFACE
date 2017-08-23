Shader "Custom/cardShader" {
	Properties{
		_Color("Main Color", COLOR) = (1,1,1,1)
		_MainTex("Base Texture", 2D) = "white"{}
		//_Cube("Cubemap", CUBE) = "" {}
	}
		SubShader{
		//Cull Off
			pass {
				Material{}
				SetTexture[_MainTex]{}
		}
	}
}
