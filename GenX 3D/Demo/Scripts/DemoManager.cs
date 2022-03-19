using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour {

    GameObject lastStructure;

    public GameObject CellularAutomataUI;

    CameraController mainCam;

    //If it's not Terrain it's CA
    bool isModeTerrain = false;

    void Awake ()
    {
        mainCam = Camera.main.GetComponent<CameraController>();
    }

	void Start () {
        Regen();
    }

    public void UnlockCamera()
    {
        mainCam.isLocked = !mainCam.isLocked;

        mainCam.ResetPos();

        GameObject.Find("Unlock Camera").transform.GetChild(0).GetComponent<Text>().text =
            mainCam.isLocked ? "Unlock Camera" : "Lock Camera";
    }

    public void SwitchModes()
    {
        CellularAutomataUI.SetActive(isModeTerrain);

        isModeTerrain = !isModeTerrain;

        GameObject.Find("Mode Switch").transform.GetChild(0).GetComponent<Text>().text =
            isModeTerrain ? "CA" : "Terrain";

        Regen();
    }

    public void Regen()
    {
        if (lastStructure != null)
            Destroy(lastStructure);

        mainCam.ResetPos();

        if (isModeTerrain) lastStructure = VoxelTerrain.main.Create(Vector3.zero);
        else lastStructure = CellularAutomata.main.Create(Vector3.zero);
    }

    public void UpdateGenerationValues()
    {
        Slider[] sliders = FindObjectsOfType<Slider>();

        foreach(Slider sl in sliders)
        {
            switch(sl.gameObject.name)
            {
                case "Width":
                    CellularAutomata.main.width = (int)sl.value;
                    break;
                case "Height":
                    CellularAutomata.main.height = (int)sl.value;
                    break;
                case "Depth":
                    CellularAutomata.main.zDepth = (int)sl.value;
                    break;
                case "Death Threshold":
                    CellularAutomata.main.deathThreshold = (int)sl.value;
                    break;
                case "Birth Threshold":
                    CellularAutomata.main.birthThreshold = (int)sl.value;
                    break;
                case "Fullness":
                    CellularAutomata.main.chanceToStartAlive = 100-(int)sl.value;
                    break;
                case "Generation":
                    CellularAutomata.main.outputGeneration = (int)sl.value;
                    break;
            }
        }
    }
}