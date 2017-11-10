using GA.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GA.Game;
using System;
using GA.UI.Windows.HoleInfoMenu;

namespace GA.UI
{
    public class TabTeesOptionPartial : UIElement
    {
        public bool Default;
        public HoleInfoMenu ParentMenu;
        public Tees AppliedTees;

        public void OnChangePositionClicked()
        {
            Game.InputStatus.DisableMainButtons();
            GameManager.getChunkFamily().CurrentHoleCreating = ParentMenu.AppliedHole;
            GameManager.getScreenClicker().ClickAction = new ClickAction(ClickType.PLACE_HOLE_TEES, new Action(() =>
            {
                ParentMenu.UnhideMenu();
                Game.InputStatus.EnableMainButtons();
            }));
            ParentMenu.HideMenu();
            StartCoroutine(IEInputActivatedAction(KeyCode.Escape, () =>
            {
                ParentMenu.UnhideMenu();
                Game.InputStatus.EnableMainButtons();
            }));
        }

        public void OnRemoveButtonClicked()
        {
            if (ParentMenu.AppliedHole)
            {
                if (ParentMenu.AppliedHole.TeesList.Contains(AppliedTees))
                {
                    ParentMenu.AppliedHole.TeesList.Remove(AppliedTees);
                    AppliedTees.CreateFencing();
                }
                else if (GameManager.DebugMode)
                {
                    GameManager.getUIController().MessageBar.QueuePopMessage("DEBUG: No tee assigned to TabTeesOptionPartial " + gameObject.name, 3);
                }
            }
            else
            {
                GameManager.getUIController().MessageBar.QueuePopMessage("DEBUG: No hole assigned to " + ParentMenu.GetType().ToString() + " " + ParentMenu.name, 3);
            }
        }
    }
}