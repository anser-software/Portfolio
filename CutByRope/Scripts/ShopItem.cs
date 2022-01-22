using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopItem : MonoBehaviour
{

    public bool isUnlocked { get; private set; }

    public bool isSelected { get; private set; }

    public Image icon;

    [SerializeField]
    private GameObject selection, locked;

    [SerializeField]
    private Button button;

    [System.NonSerialized]
    public int index;

    public void Initialize(int _index)
    {
        index = _index;
    }

    public void Unlock()
    {
        locked.SetActive(false);

        isUnlocked = true;

        button.interactable = true;
    }

    public void Select()
    {
        selection.SetActive(true);

        isSelected = true;
    }

    public void UnSelect()
    {
        selection.SetActive(false);

        isSelected = false;
    }

    public void Click()
    {
        ShopManager.instance.SelectItem(index);
    }

}
