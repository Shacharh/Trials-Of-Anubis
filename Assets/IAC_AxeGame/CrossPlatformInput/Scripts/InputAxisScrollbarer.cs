using System;
using UnityEngine;

namespace IACFPSController.CrossPlatformInput
{
    public class InputAxisScrollbarer : MonoBehaviour
    {
        public string axis;

	    void Update() { }

	    public void HandleInput(float value)
        {
            CrossPlatformInputManage.SetAxis(axis, (value*2f) - 1f);
        }
    }
}
