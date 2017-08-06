using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBar : MonoBehaviour
{

    public RawImage Image;
    public Text TextBox;
    public UIController controller;

    public List<PopMessageQueue> queue = new List<PopMessageQueue>();

	public void QueuePopMessage(string text, float time)
    {
        queue.Add(new PopMessageQueue() { text = text, time = time });
    }

    public void Update()
    {
        if(queue.Count > 0)
        {
            if(queue[0].showing == false)
            {
                TextBox.text = "";
                StartCoroutine(controller.MoveRect(this.GetComponent<RectTransform>(), new Vector2(-1, -1), new Vector2(-1, 144), 5f, FinishedMovingOut));
                queue[0] = new PopMessageQueue { text = queue[0].text, showing = true, time = queue[0].time };
            }
        }
    }

    public void FinishedMovingOut()
    {
        StartCoroutine(TypeMessage(queue[0].text, queue[0].time));
    }

    public void FinishedMessage()
    {
        StartCoroutine(controller.MoveRect(this.GetComponent<RectTransform>(), new Vector2(-1, 144), new Vector2(-1, -1), 5f, FinishedMovingIn));
    }

    public void FinishedMovingIn()
    {
        if (queue.Count > 0)
            queue.RemoveAt(0);
    }

    public IEnumerator TypeMessage(string text, float time)
    {
        char[] list = text.ToCharArray();
        float timePer = time / list.Length;

        for (int i = 0; i < list.Length; i++)
        {
            TextBox.text += list[i];
            yield return new WaitForSeconds(timePer);
        }

        yield return new WaitForSeconds(7.5f);

        FinishedMessage();
    }
}

public struct PopMessageQueue
{
    public string text;
    public float time;
    public bool showing;
}
