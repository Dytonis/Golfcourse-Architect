using GA.UI.Windows.PreviewMenu;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GA.Engine;
using GA.Game;
using System.Collections;

namespace GA.Objects
{
    public class MenuTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ObjectID Object;
        public PreviewMenu Menu;
        private PreviewMenu actual;

        private float overTime = 0;
        private bool over = false;

        public IEnumerator UpdateHoverTime()
        {
            while(over)
            {
                overTime += Time.deltaTime;

                if(overTime >= 0.5f)
                {
                    createPreview();
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void createPreview()
        {
            TileObject obj = TileObject.GetObjectPrefabFromID(Object);
            if (actual == null)
            {
                actual = Instantiate(Menu, GameManager.getUICanvas().transform) as PreviewMenu;
            }
            if (actual != null)
            {
                actual.title.text = obj.UIInfo.UIName;
                actual.description.text = obj.UIInfo.Description;
                actual.price.text = "$" + obj.Cost.ToString();

                string flareText = obj.RarityInfo.Strings[UnityEngine.Random.Range((int)0, obj.RarityInfo.Strings.Length)];
                string subtext = obj.RarityInfo.Subtext;
                Color fillColor = obj.RarityInfo.Color;
                Color tint = obj.RarityInfo.TintColor;

                actual.DrawFlare(flareText, subtext, fillColor, tint);

                //use reflection to search for properties and print them (just to automate this for future additions)
                foreach (System.Reflection.FieldInfo field in typeof(TileObjectEffect).GetFields())
                {
                    float value = (float)field.GetValue(obj.Effect);
                    TileObjectEffectInverseColorAttribute[] attributes = (TileObjectEffectInverseColorAttribute[])field.GetCustomAttributes(typeof(TileObjectEffectInverseColorAttribute), false);
                    bool inverse = false;
                    foreach (TileObjectEffectInverseColorAttribute a in attributes)
                    {
                        inverse = a.Inverse;
                    }

                    string color = "";

                    if (value < 0)
                        if (inverse)
                            color = "<color=#30AA10>";
                        else
                            color = "<color=#990000>";
                    else if (value > 0)
                        if (inverse)
                            color = "<color=#990000>";
                        else
                            color = "<color=#30AA10>";

                    if (!String.IsNullOrEmpty(color))
                    {
                        Text t = Instantiate(actual.GridTextPrefab, actual.GridEffects.transform) as Text;
                        t.text = field.Name + ": \t" + color + (value > 0 ? "+" + value.ToString() : value.ToString()) + "</color>";
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            over = true;
            StartCoroutine(UpdateHoverTime());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (actual != null)
                Destroy(actual.gameObject);

            overTime = 0;
            over = false;
        }
    }
}
