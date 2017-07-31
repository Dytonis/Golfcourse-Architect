using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public List<UIButton> Buttons = new List<UIButton>();
    public List<UIButton> Majors = new List<UIButton>();
    public List<UIButton> Minors = new List<UIButton>();

    public List<EventSystem> UIElements = new List<EventSystem>();

    public RectTransform BarOutCurrently = null;

    public bool OverUIController = false;

    public void Update()
    {
        EventSystem sys = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        OverUIController = sys.IsPointerOverGameObject();
    }

    public void DeactivateAll()
    {
        foreach (UIButton b in Buttons)
            b.Deactivate();

        if (BarOutCurrently)
        {
            StartCoroutine(MoveRect(BarOutCurrently, new Vector2(-2000, BarOutCurrently.anchoredPosition.y), BarOutCurrently.anchoredPosition, 10));
        }
    }
    public void DeactivateMinors()
    {
        foreach (UIButton b in Minors)
            b.Deactivate();
    }
    public void DeactivateMajors()
    {
        foreach (UIButton b in Majors)
            b.Deactivate();

        if(BarOutCurrently)
        {
            StartCoroutine(MoveRect(BarOutCurrently, new Vector2(-2000, BarOutCurrently.anchoredPosition.y), BarOutCurrently.anchoredPosition, 10));
        }
    }

    public IEnumerator MoveRect(RectTransform rect, Vector2 to, Vector2 from, float speed)
    {
        float t = 0;
        Vector2 pos = from;
        float journeyLength = Vector2.Distance(to, from);
        do
        {
            t += Time.deltaTime * speed;
            pos = Vector2.Lerp(from, to, t);
            rect.anchoredPosition = pos;
            yield return new WaitForEndOfFrame();
        } while (!pos.Equals(to));
    }
}
