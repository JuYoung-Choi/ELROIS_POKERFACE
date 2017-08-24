using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScr : MonoBehaviour
{
    Transform[] tempOutlineEnable;
    Transform[] tempOutlineAble;
    Transform FindObjectEnable;
    Transform FindObjectAble;

    float mouseSensitivity = 2f;
    float rotLeftRight;
    bool PlayerSpot;
	bool DealerSpot;

    Vector3 m_PlayerRot;
    RaycastHit temp;

    public Camera m_Cam;
    public Light Player_splight;
	public Light Dealer_splight;
	public UILabel T_text;

    void Start()
    {
        tempOutlineAble = null;
        tempOutlineEnable = null;
        FindObjectEnable = null;

		Player_splight.GetComponent<Light>();
		Dealer_splight.GetComponent<Light>();

        Input.gyro.enabled = true;
    }

    private void FixedUpdate()
    {
		//Vector3 rootPos = new Vector3 (0.6f, -0.459f, -1.990112f);
				
		#if UNITY_EDITOR
		FPRotate();
		//Debug.DrawRay(rootPos, m_Cam.transform.forward * 50.0f, Color.green);
		#elif UNITY_ANDROID
        gyroupdate();
		#endif

		Debug.DrawRay(m_Cam.transform.position, m_Cam.transform.forward * 50.0f, Color.green);
		/*
		Vector3 angle = m_Cam.transform.forward;
		string testText = angle.x + " " + angle.y + " " + angle.z;
		T_text.text = testText;
		*/

		//string testText = "asdf";
		// 카메라의 위치에서 카메라가 바라보는 정면으로 레이를 쏴서 충돌확인

		if (Physics.Raycast (m_Cam.transform.position, m_Cam.transform.forward, out temp, 200.0f)) {
			OutlineChange (temp);
			//testText = temp.transform.name;

			if (tempOutlineEnable == null)
				return;

			if (temp.transform.tag == "PlayerPanel" || temp.transform.tag == "DealerPanel") {
				//캐릭터 밖으로 벗어나면 아웃라인 제거
				for (int i = 0; i < tempOutlineEnable.Length; ++i) {
					if (tempOutlineEnable [i].gameObject.tag == "Outline") {
						tempOutlineEnable [i].GetComponent<cakeslice.Outline> ().eraseRenderer = true;
					}
				}
			}
					
		} else {
			//캐릭터 밖으로 벗어나면 아웃라인 제거
			for (int i = 0; i < tempOutlineAble.Length; ++i) {
				if (tempOutlineAble [i].gameObject.tag == "Outline") {
					tempOutlineAble [i].GetComponent<cakeslice.Outline> ().eraseRenderer = true;
				}
			}
		}

		if (PlayerSpot == true)
			Char_smoothLightOn(temp, 10.0f, 0.05f);

		if (DealerSpot == true)
			Deal_smoothLightOn(temp, 10.0f, 0.05f);
    }

    void gyroupdate()
    {
        m_PlayerRot.y += Input.gyro.rotationRate.y * -1f;
		/*
		//camera rotation limit
		if (m_PlayerRot.y >= 60f)
			return;

		if (m_PlayerRot.y <= -60f)
			return;
		*/

		if (m_PlayerRot.y >= 360f)
			m_PlayerRot.y = 0f;

		m_Cam.transform.eulerAngles = m_PlayerRot;
    }
		
    public void AngleReset()
    {
        m_PlayerRot = Vector3.zero;
    }

	//카메라
    void FPRotate()
    {
        rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
		transform.Rotate(0f, rotLeftRight, 0f);
    }

    //----------------------------------------------------------------------------
    //아웃라인 생성
    //----------------------------------------------------------------------------

    void OutlineChange(RaycastHit temp)
    {
        tempOutlineAble = temp.transform.GetComponentsInChildren<Transform>();

        //수정해야함(Find를 사용한 접근 방식 수정하기)
        if (transform.Find(temp.transform.name) == null)
            return;

	
        //처음 선택 하거나 이전에 선택한 것이 현재 선택한 것과 같을 경우
        if (tempOutlineEnable == null || tempOutlineAble == tempOutlineEnable)
        {
            for (int i = 0; i < tempOutlineAble.Length; i++)
            {
                if (tempOutlineAble[i].gameObject.tag == "Outline")
                {
                    tempOutlineAble[i].GetComponent<cakeslice.Outline>().eraseRenderer = false;
                    tempOutlineEnable = tempOutlineAble;
                    AvatarAble(temp);
                }
            }
        }

        //이전에 선택한 것이 현재 선택한 것과 다를 경우
        if (tempOutlineAble != tempOutlineEnable)
        {
            //이전에 선택 한 것에 라이트 제거
            for (int i = 0; i < tempOutlineEnable.Length; ++i)
            {
                if (tempOutlineEnable[i].gameObject.tag == "Outline")
                {
					tempOutlineEnable [i].GetComponent<cakeslice.Outline> ().eraseRenderer = true;
                    AvatarEnable(temp);

					if (temp.transform.gameObject.tag != "Deal")
						Char_LightOff();
                }
            }

            //현재 선택한 것에 아웃라인 생성
            for (int i = 0; i < tempOutlineAble.Length; i++)
            {
                if (tempOutlineAble[i].gameObject.tag == "Outline")
                {
                    tempOutlineAble[i].GetComponent<cakeslice.Outline>().eraseRenderer = false;
                    tempOutlineEnable = tempOutlineAble;
                    AvatarAble(temp);
                }
            }
        }
    }

    //----------------------------------------------------------------------------
    //캐릭터 교체
    //----------------------------------------------------------------------------

    void AvatarAble(RaycastHit temp)
    {
		if (temp.collider.tag != "Char")
			return;

		if (UICamera.hoveredObject != null)
			return;
		
        if (Input.GetMouseButtonDown(0))
        {
            FindObjectAble = transform.Find(temp.transform.name); //수정해야함(Find를 사용한 접근 방식 수정하기)
            FindObjectAble.gameObject.SetActive(true);

            FindObjectEnable = FindObjectAble;
        }
		PlayerSpot = true;
		DealerSpot = true;
    }

    void AvatarEnable(RaycastHit temp)
    {
		if (temp.collider.tag != "Char")
			return;

		if (UICamera.hoveredObject != null)
			return;
		
        if (Input.GetMouseButtonDown(0))
        {
            if (FindObjectEnable != null)
                FindObjectEnable.gameObject.SetActive(false);
        }
    }


    //----------------------------------------------------------------------------
    //character 조명
    //----------------------------------------------------------------------------

    float Char_newintensity;
    float Char_newColor;

    public void Char_smoothLightOn(RaycastHit temp, float LightOn, float SpotOn)
    {
		Renderer rend = Player_splight.GetComponentInChildren<MeshRenderer>();
		Color T_color = rend.material.GetColor("_TintColor");

        if (Input.GetMouseButtonDown(0))
        {
            if (temp.collider == null)
                return;

            if (temp.transform.gameObject.tag != "Char")
                return;

			if (UICamera.hoveredObject != null)
				return;

			Char_newintensity = LightOn;
			Char_newColor = SpotOn;

			Player_splight.transform.position = new Vector3(temp.transform.position.x, Player_splight.transform.position.y, temp.transform.position.z);
			Player_splight.transform.rotation = Quaternion.Euler(90, temp.transform.eulerAngles.y, 0);
        }
		Player_splight.GetComponent<Light>().intensity = Mathf.Lerp(Player_splight.intensity, Char_newintensity, Time.deltaTime);
		T_color.a = Mathf.Lerp(T_color.a, Char_newColor, Time.deltaTime);

		rend.material.SetColor("_TintColor", T_color);
    }

    public void Char_LightOff()
    {
		Renderer rend = Player_splight.GetComponentInChildren<MeshRenderer>();
		Color T_color = rend.material.GetColor("_TintColor");

		if (UICamera.hoveredObject != null)
			return;

        if (Input.GetMouseButtonDown(0))
        {
			if (Player_splight.GetComponent<Light>().intensity > 2.0f)
            {
				Player_splight.GetComponent<Light>().intensity = 0;

				T_color.a = 0.0001f;
				rend.material.SetColor("_TintColor", T_color);
            }
        }
    }
		
	//----------------------------------------------------------------------------
	//Dealer 조명
	//----------------------------------------------------------------------------

	float Deal_newintensity;
	float Deal_newColor;

	public void Deal_smoothLightOn(RaycastHit temp, float LightOn, float SpotOn)
	{
		Renderer D_rend = Dealer_splight.GetComponentInChildren<MeshRenderer>();
		Color DT_color = D_rend.material.GetColor("_TintColor");

		if (Input.GetMouseButtonDown(0))
		{
			if (temp.collider == null)
				return;

			if (temp.transform.gameObject.tag != "Deal")
				return;

			if (UICamera.hoveredObject != null)
				return;

			Deal_newintensity = LightOn;
			Deal_newColor = SpotOn;

			Dealer_splight.transform.position = new Vector3(temp.transform.position.x, Dealer_splight.transform.position.y, temp.transform.position.z);
			Dealer_splight.transform.rotation = Quaternion.Euler(90, temp.transform.eulerAngles.y, 0);
		}
		Dealer_splight.GetComponent<Light>().intensity = Mathf.Lerp(Dealer_splight.intensity, Deal_newintensity, Time.deltaTime);
		DT_color.a = Mathf.Lerp(DT_color.a, Deal_newColor, Time.deltaTime);

		D_rend.material.SetColor("_TintColor", DT_color);
	}

	public void Deal_LightOff()
	{
		Renderer D_rend = Dealer_splight.GetComponentInChildren<MeshRenderer>();
		Color DT_color = D_rend.material.GetColor("_TintColor");

		if (UICamera.hoveredObject != null)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			if (Dealer_splight.GetComponent<Light>().intensity > 2.0f)
			{
				Dealer_splight.GetComponent<Light>().intensity = 0;

				DT_color.a = 0.0001f;
				D_rend.material.SetColor("_TintColor", DT_color);
			}
		}
	}
}