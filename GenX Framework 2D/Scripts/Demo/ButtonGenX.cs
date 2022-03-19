using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonGenX : MonoBehaviour, IPointerClickHandler
{

    public string methodName;

    public int parameter = -1;

    public bool expand;

    bool isExpanding;

    public float expandingSpeed;

    public float xExpandTarget;
    public float yExpandTarget;

    void Start()
    {
        if (expand)
            Invoke("Expand", 1F);
    }

    void FixedUpdate()
    {
        if (transform.localScale.x < xExpandTarget && isExpanding)
            transform.localScale += Vector3.right * Time.deltaTime * expandingSpeed;
        if (transform.localScale.y < yExpandTarget && isExpanding)
            transform.localScale += Vector3.up * Time.deltaTime * expandingSpeed;


    }

    void Expand()
    {
        isExpanding = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DemoManager.demoManager.SendMessage(methodName, parameter);
    }
}
