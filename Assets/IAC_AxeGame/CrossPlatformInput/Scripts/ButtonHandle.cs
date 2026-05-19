using System;
using UnityEngine;

namespace IACFPSController.CrossPlatformInput
{
    public class ButtonHandle : MonoBehaviour
    {

        public string Name;

        void OnEnable()
        {

        }

        public void SetDownState()
        {
            CrossPlatformInputManage.SetButtonDown(Name);
        }


        public void SetUpState()
        {
            CrossPlatformInputManage.SetButtonUp(Name);
        }


        public void SetAxisPositiveState()
        {
            CrossPlatformInputManage.SetAxisPositive(Name);
        }


        public void SetAxisNeutralState()
        {
            CrossPlatformInputManage.SetAxisZero(Name);
        }


        public void SetAxisNegativeState()
        {
            CrossPlatformInputManage.SetAxisNegative(Name);
        }

        public void Update()
        {

        }
    }
}
