using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game
{
    public static class InputStatus
    {
        private static bool _acs;
        public static bool AllowCameraScroll
        {
            get
            {
                return _acs;
            }
            set
            {
                if (!LockCameraControl)
                    _acs = value;
            }
        }
        private static bool _acm;
        public static bool AllowCameraMovement
        {
            get
            {
                return _acm;
            }
            set
            {
                if (!LockCameraControl)
                    _acm = value;
            }
        }
        private static bool _acr;
        public static bool AllowCameraRotation
        {
            get
            {
                return _acr;
            }
            set
            {
                if (!LockCameraControl)
                    _acr = value;
            }
        }

        public static bool LockCameraControl = false;
        public static void DisableMainButtons()
        {
            UIController controller = GameObject.FindGameObjectWithTag("uiController").GetComponent<UIController>();

            controller.DisableMajors();
        }

        public static void EnableMainButtons()
        {
            UIController controller = GameObject.FindGameObjectWithTag("uiController").GetComponent<UIController>();

            controller.EnableMajors();
        }
    }
}
