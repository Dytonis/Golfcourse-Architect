using GA.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GA.Game;
using System;

namespace GA.UI.Windows.HoleInfoMenu
{
    public class TabTees : UIMenu
    {
        public HoleInfoMenu Menu;
        public RectTransform TabTeesOptionContent;

        public List<TabTeesOptionPartial> PartialList = new List<TabTeesOptionPartial>();

        public void OnAddButtonClicked()
        {
            Game.InputStatus.DisableMainButtons();
            GameManager.getChunkFamily().CurrentHoleCreating = Menu.AppliedHole;
            GameManager.getScreenClicker().ClickAction = new ClickAction(ClickType.PLACE_HOLE_TEES, new Action(() =>
            {
                Menu.UnhideMenu();
                Game.InputStatus.EnableMainButtons();
            }));
            Menu.HideMenu();
            StartCoroutine(IEInputActivatedAction(KeyCode.Escape, () =>
            {
                Menu.UnhideMenu();
                Game.InputStatus.EnableMainButtons();
            }));
        }

        public override void OnElementLoad()
        {
            OnMenuShow();
        }

        public override void OnMenuShow()
        {
            if (PartialList.Count == 0)
                CreateList();
            else UpdateList();
        }

        public void CreateList()
        {
            if (Menu.AppliedHole)
            {
                foreach (Tees t in Menu.AppliedHole.TeesList)
                {
                    if (PartialList.All(x => x.AppliedTees != t)) //if all of the partials in the list are not assigned to this tee (we need to add one)
                    {
                        GameObject p = Instantiate(Resources.Load(ResourceFinder.UIElements.Partials.TabTeesOptionPartial) as GameObject, TabTeesOptionContent);
                        p.GetComponent<TabTeesOptionPartial>().ParentMenu = Menu;
                        PartialList.Add(p.GetComponent<TabTeesOptionPartial>());
                    }
                }
            }
        }

        public void UpdateList()
        {
            for(int i = 0; i < PartialList.Count; i++)
            {
                Destroy(PartialList[i].gameObject);
            }

            PartialList.Clear();

            CreateList();
        }
    }
}
