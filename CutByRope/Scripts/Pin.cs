using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class Pin : MonoBehaviour
{

    public PathCreator path;
    public bool locked { get; private set; }

    public Vector3 directionOfMotion { get; private set; }

    [SerializeField]
    private GameObject[] skins;

    [SerializeField]
    private GameObject freezePrefab;

    private Vector3 lastPos;

    private void Start()
    {
        UIManager.instance.OnShopSelectionChanged += UpdateSkin;

        UpdateSkin();

        lastPos = transform.position;
    }


    private void UpdateSkin()
    {
        foreach (var skin in skins)
        {
            skin.SetActive(false);
        }

        skins[PlayerPrefs.GetInt("SelectedPin", 0)].SetActive(true);
    }


    public void Lock()
    {
        locked = true;

        Instantiate(freezePrefab, transform.position, Quaternion.identity);
    }

    private void LateUpdate()
    {
        var d = transform.position - lastPos;

        if(d.sqrMagnitude > 0.5F)
        {
            directionOfMotion = d.normalized;
            lastPos = transform.position;
        }
    }

}
