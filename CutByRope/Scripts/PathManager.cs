using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{

    [SerializeField]
    private Transform pathParent;

    [SerializeField]
    private Material[] materials;

    private void Start()
    {
        UIManager.instance.OnShopSelectionChanged += UpdateSkin;

        UpdateSkin();
    }

    private void UpdateSkin()
    {
        foreach (var element in pathParent.GetComponentsInChildren<MeshRenderer>())
        {
            element.sharedMaterial = materials[PlayerPrefs.GetInt("SelectedWire", 0)];
        }
    }

}
