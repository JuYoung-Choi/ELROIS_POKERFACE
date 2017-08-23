using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerScr : MonoBehaviour
{
    Transform[] tempOutlineEnable = null;
    Transform FindObjectEnable = null;

    float mouseSensitivity = 2f;
    float rotLeftRight;

    Vector3 m_PlayerRot;

    public Camera m_Cam;
    public Light splight;
    public Plane cone;
    public UILabel m_TestUI;

    RaycastHit temp;

    // Use this for initialization
    void Start()
    {
        splight.GetComponent<Light>();
        Input.gyro.enabled = true;
        //Debug.Log(outlineShader);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        ///gyroupdate();
        FPRotate();

        Debug.DrawRay(m_Cam.transform.position, m_Cam.transform.forward * 50.0f, Color.green);

        // 카메라의 위치에서 카메라가 바라보는 정면으로 레이를 쏴서 충돌확인
        if (Physics.Raycast(m_Cam.transform.position, m_Cam.transform.forward, out temp, 200.0f))
        {
            m_TestUI.text = temp.collider.name;
            OutlineChange(temp);
        }
        else
            m_TestUI.text = "";
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

    //아웃라인 생성
    void OutlineChange(RaycastHit temp)
    {
        Transform[] tempOutlineAble = temp.transform.GetComponentsInChildren<Transform>();

        if (transform.Find(temp.transform.name) != null)
        {
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

            if (tempOutlineAble != tempOutlineEnable)
            {
                for (int i = 0; i < tempOutlineEnable.Length; ++i)
                {
                    if (tempOutlineEnable[i].gameObject.tag == "Outline")
                    {
                        tempOutlineEnable[i].GetComponent<cakeslice.Outline>().eraseRenderer = true;
                        AvatarEnable(temp);
                    }
                }

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
    }

    //캐릭터 교체
    void AvatarAble(RaycastHit temp)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Transform FindObjectAble = transform.Find(temp.transform.name);
            FindObjectAble.gameObject.SetActive(true);

            FindObjectEnable = FindObjectAble;
        }
        smoothLightOn(temp, 4.0f, 0.25f);
    }

    void AvatarEnable(RaycastHit temp)
    {
        Renderer rend = splight.GetComponentInChildren<MeshRenderer>();
        Color T_color = rend.material.GetColor("_TintColor");

        if (Input.GetMouseButtonDown(0))
        {
            if (FindObjectEnable != null)
                FindObjectEnable.gameObject.SetActive(false);

            if (splight.GetComponent<Light>().intensity > 2.0f) { 
                splight.GetComponent<Light>().intensity = 0;

                T_color.a = 0.0001f;
                rend.material.SetColor("_TintColor", T_color);
            }
        }
    }

    //조명
    float newintensity;
    float newColor;

    public void smoothLightOn(RaycastHit temp, float LightOn, float SpotOn)
    {
        Renderer rend = splight.GetComponentInChildren<MeshRenderer>();
        Color T_color = rend.material.GetColor("_TintColor");

        if (Input.GetMouseButtonDown(0))
        {
            newintensity = LightOn;
            newColor = SpotOn;
            splight.transform.position = new Vector3(temp.transform.position.x, splight.transform.position.y, splight.transform.position.z);
        }
        splight.GetComponent<Light>().intensity = Mathf.Lerp(splight.intensity, newintensity, Time.deltaTime * 0.1f);

        T_color.a = Mathf.Lerp(0.0001f, newColor, Time.timeScale * 0.05f);
        rend.material.SetColor("_TintColor", T_color);
    }
}
