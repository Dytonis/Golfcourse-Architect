using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game
{
    /// <summary>
    /// Singleton class GameManager used to easily get object references and implement coroutines.
    /// </summary>
    public class GameManager
    {
        public static UIController getUIController()
        {
            return GameObject.FindGameObjectWithTag("uiController").GetComponent<UIController>();
        }

        public static Canvas getUICanvas()
        {
            return GameObject.FindGameObjectWithTag("uiController").GetComponent<Canvas>();
        }

        public static ScreenClick getScreenClicker()
        {
            return GameObject.FindGameObjectWithTag("cameraClicker").GetComponent<ScreenClick>();
        }

        public static ChunkFamily getChunkFamily()
        {
            return GameObject.FindGameObjectWithTag("family").GetComponent<ChunkFamily>();
        }

        public static bool DebugMode = true;
    }
}
