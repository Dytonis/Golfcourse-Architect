using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.UI.Windows
{
    public class UIMenu : UIElement
    {
        private bool _hidden;
        public bool Hidden
        {
            get
            {
                return _hidden;
            }
        }
        private Vector2 UnhiddenPosition;

        public virtual void OnMenuShow()
        {

        }

        public virtual void OnMenuHide()
        {

        }

        public virtual void OnCloseButtonClicked()
        {
            CloseMenu();
        }

        public virtual void OnMinimizeButtonClicked()
        {

        }

        public virtual void MinimizeMenu()
        {

        }

        public virtual void CloseMenu()
        {
            Destroy(gameObject);
        }

        public virtual void HideMenu()
        {
            if (!Hidden)
            {
                UnhiddenPosition = GetComponent<RectTransform>().anchoredPosition;
                GetComponent<RectTransform>().anchoredPosition = new Vector2(99999, 99999);
                _hidden = true;
                OnMenuHide();
            }
        }

        public virtual void UnhideMenu()
        {
            if (Hidden)
            {
                GetComponent<RectTransform>().anchoredPosition = UnhiddenPosition;
                _hidden = false;
                OnMenuShow();
            }
        }
    }
}
