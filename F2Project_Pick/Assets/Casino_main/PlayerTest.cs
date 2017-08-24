using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    Vector3 m_PlayerRot;
    // Use this for initialization
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        gyroupdate();
    }

    void gyroupdate()
    {
        m_PlayerRot.x -= Input.gyro.rotationRate.x;
        m_PlayerRot.y += Input.gyro.rotationRate.y * -1f;

        if (m_PlayerRot.x >= 360f)
            m_PlayerRot.x = 0f;

        if (m_PlayerRot.y >= 360f)
            m_PlayerRot.y = 0f;

        transform.eulerAngles = m_PlayerRot;
    }

}
