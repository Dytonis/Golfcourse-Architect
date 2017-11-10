using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.UI
{
    public class UIElement : MonoBehaviour
    {
        protected UIElement() { }

        internal RectTransform rectTransform
        {
            get
            {
                return GetComponent<RectTransform>();
            }
        }

        public virtual void OnElementLoad()
        {

        }

        public virtual void OnElementClose()
        {

        }

        /// <summary>
        /// Instantiates prefab element
        /// </summary>
        /// <param name="element"></param>
        public static RectTransform Pop(RectTransform element)
        {
            return Instantiate(element, GameObject.FindGameObjectWithTag("uiController").transform);
        }

        public static T Pop<T>(T element) where T : UIElement
        {
            return Instantiate(element, GameObject.FindGameObjectWithTag("uiController").transform) as T;
        }

        /// <summary>
        /// Must be started with MonoBehaviour.StartCoroutine
        /// </summary>
        /// <param name="code">The keycode to invoke the action</param>
        /// <param name="action">The action to be invoked</param>
        /// <returns></returns>
        public IEnumerator IEInputActivatedAction(KeyCode code, Action action)
        {
            bool key = !Input.GetKey(code);

            while (key)
            {
                key = !Input.GetKey(code);
                yield return new WaitForEndOfFrame();
            }

            action.Invoke();
        }
    }
}
