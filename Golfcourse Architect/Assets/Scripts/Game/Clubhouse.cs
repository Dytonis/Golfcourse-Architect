using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game
{
    public class Clubhouse : ActorBuilding
    {
        public Transform InitialMoveSpot;
        public Transform GolferInitialSpawnPoint;

        public void OpenDoorThenClose(float time)
        {
            StartCoroutine(OpenThenClose(time));
        }

        private IEnumerator OpenThenClose(float time)
        {
            OpenDoor();
            yield return new WaitForSeconds(time);
            CloseDoor();
        }

        public void OpenDoor()
        {
            AnimationController.SetTrigger("OpenDoor");
        }

        public void CloseDoor()
        {
            AnimationController.SetTrigger("CloseDoor");
        }
    }
}
