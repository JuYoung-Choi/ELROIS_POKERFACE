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
    bool spotLight;

    Vector3 m_PlayerRot;
    RaycastHit temp;

    public Camera m_Cam;
    public Light splight;
    public UILabel m_TestUI;


    void Start()
    {
        tempOutlineAble = null;
        tempOutlineEnable = null;
        FindObjectEnable = null;

        splight.GetComponent<Light>();

        Input.gyro.enabled = true;
    }

    private void FixedUpdate()
    {
        //gyroupdate();
        FPRotate();

        Debug.DrawRay(m_Cam.transform.position, m_Cam.transform.forward * 50.0f, Color.green);

        // 카메라의 위치에서 카메라가 바라보는 정면으로 레이를 쏴서 충돌확인
        if (Physics.Raycast(m_Cam.transform.position, m_Cam.transform.forward, out temp, 200.0f))
        {
            m_TestUI.text = temp.collider.name;
            OutlineChange(temp);
        }
        else
        {
            if (tempOutlineEnable == null)
                return;

            //캐릭터 밖으로 벗어나면 아웃라인 제거
            for (int i = 0; i < tempOutlineAble.Length; ++i)
            {
                if (tempOutlineAble[i].gameObject.tag == "Outline")
                {
                    tempOutlineAble[i].GetComponent<cakeslice.Outline>().eraseRenderer = true;
                }
            }
            m_TestUI.text = "";
        }

        if (spotLight == true)
            smoothLightOn(temp, 10.0f, 0.1f);
    }

    void gyroupdate()
    {
        m_PlayerRot.x -= Input.gyro.rotationRate.x;
        m_PlayerRot.y += Input.gyro.rotationRate.y * -1f;

        if (m_PlayerRot.x >= 360f)
            m_PlayerRot.x = 0f;

        if (m_PlayerRot.y >= 360f)
            m_PlayerRot.y = 0f;

        m_Cam.transform.eulerAngles = m_PlayerRot;
    }

    //카메라
    public void AngleReset()
    {
        m_PlayerRot = Vector3.zero;
    }

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
                    AvatarEnable(temp);
                    LightOff();
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
        if (Input.GetMouseButtonDown(0))
        {
            FindObjectAble = transform.Find(temp.transform.name); //수정해야함(Find를 사용한 접근 방식 수정하기)
            FindObjectAble.gameObject.SetActive(true);

            FindObjectEnable = FindObjectAble;
        }
        spotLight = true;
    }

    void AvatarEnable(RaycastHit temp)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (FindObjectEnable != null)
                FindObjectEnable.gameObject.SetActive(false);
        }
    }


    //----------------------------------------------------------------------------
    //조명
    //----------------------------------------------------------------------------

    float newintensity;
    float newColor;

    public void smoothLightOn(RaycastHit temp, float LightOn, float SpotOn)
    {
        Renderer rend = splight.GetComponentInChildren<MeshRenderer>();
        Color T_color = rend.material.GetColor("_TintColor");

        if (Input.GetMouseButtonDown(0))
        {
            if (temp.collider == null)
                return;

            if (temp.transform.gameObject.tag != "Char")
                return;

            newintensity = LightOn;
            newColor = SpotOn;

            splight.transform.position = new Vector3(temp.transform.position.x, splight.transform.position.y, temp.transform.position.z);
            splight.transform.rotation = Quaternion.Euler(90, temp.transform.eulerAngles.y, 0);
        }
        splight.GetComponent<Light>().intensity = Mathf.Lerp(splight.intensity, newintensity, Time.deltaTime);
        T_color.a = Mathf.Lerp(T_color.a, newColor, Time.deltaTime);

        rend.material.SetColor("_TintColor", T_color);
    }

    public void LightOff()
    {
        Renderer rend = splight.GetComponentInChildren<MeshRenderer>();
        Color T_color = rend.material.GetColor("_TintColor");

        if (Input.GetMouseButtonDown(0))
        {
            if (splight.GetComponent<Light>().intensity > 2.0f)
            {
                splight.GetComponent<Light>().intensity = 0;

                T_color.a = 0.0001f;
                rend.material.SetColor("_TintColor", T_color);
            }
        }
    }
}