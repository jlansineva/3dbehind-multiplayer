using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDialog : MonoBehaviour
{
    public Transform player;
    public string dialogText;
    public float distanceToShowText = 3f;

    public TextMesh text;
    public TextMesh textShadow;

    // Use this for initialization
    void Start()
    {
        SetText(dialogText);
    }

    // Update is called once per frame
    void Update()
    {
        var distance = transform.position - player.position;
        var playerIsInRange = distance.sqrMagnitude <= distanceToShowText * distanceToShowText;

        SetTextVisibility(visible: playerIsInRange);

        transform.rotation = Quaternion.LookRotation(distance, Vector3.up);
    }

    public void SetTextVisibility(bool visible)
    {
        text.GetComponent<Renderer>().enabled = visible;

        if (textShadow != null)
            textShadow.GetComponent<Renderer>().enabled = visible;
    }

    public void SetText(string str)
    {
        str = str.Replace("<br>", "\n");
        text.text = str;

        if (textShadow != null)
            textShadow.text = str;
    }
}
