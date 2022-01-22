using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{

    [SerializeField]
    private MeshRenderer[] backgrounds;

    [SerializeField]
    private Material[] materials;

    private void Start()
    {
        UIManager.instance.OnShopSelectionChanged += UpdateSkin;

        UpdateSkin();
    }

    private void UpdateSkin()
    {
        foreach (var bg in backgrounds)
        {
            bg.sharedMaterial = materials[PlayerPrefs.GetInt("SelectedBg", 0)];
        }
    }

}
