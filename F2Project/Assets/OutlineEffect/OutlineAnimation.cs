using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace cakeslice
{
    public class OutlineAnimation : MonoBehaviour
    {
        bool pingPong = false;

        void Update()
        {
            Color c = GetComponent<OutlineEffect>().lineColor;

            if(pingPong) //아웃라인 보이게
            {
                c.a += Time.deltaTime;

                if(c.a >= 1)
                    pingPong = false;
            }
            c.a = Mathf.Clamp01(c.a);
            GetComponent<OutlineEffect>().lineColor = c;
            GetComponent<OutlineEffect>().UpdateMaterialsPublicProperties();
        }
    }
}