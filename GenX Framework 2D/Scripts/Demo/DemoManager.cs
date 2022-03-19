using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using GenX2D;

public class DemoManager : MonoBehaviour
{
    public Transform panel;

    public float transSpeed;
    public float transDistance;

    public bool ShowWarnings;
    public static bool showWarnings
    {
        get
        {
            DemoManager dm = FindObjectOfType<DemoManager>();
            if (!dm) return true;
            else return dm.ShowWarnings;
        }
    }

    public static DemoManager demoManager { get { return FindObjectOfType<DemoManager>(); } }

    [Header("Cellular Automata")]
    public Slider deathT;
    public Slider birthT;
    public Slider fullness;
    public Slider caveAlive;
    public Slider islandAlive;

    [Header("Dungeon")]
    public Slider roomSize;
    public Slider roomCount;
    public Slider roomFullness;
    public Slider passageWidth;
    public Slider passageBorder;

    public Toggle smoothMesh;
    public Toggle textureMesh;

    [Header("Smooth Terrain")]
    public Slider scale;
    public Slider magnitude;
    public Slider exponent;

    [Header("Terrain")]
    public Dropdown biome;

    void Start()
    {
        Terrain2D.SetBiome(0);
    }

    GameObject lastCave = null;

    public void MakeCave(int preset)
    {
        Vector3 lastSize = Vector3.one;
        Vector3 lastPos = new Vector3(59, 65, 85);
        
        if (lastCave)
        {
            lastSize = lastCave.transform.localScale;
            lastPos = lastCave.transform.localPosition;

            Destroy(lastCave);
        }

        if (preset == -1)
            lastCave = MapManager2D.CreateCave(lastPos, MapManager2D.mapManager.allBlockTypes[0], 60, 60, (int)deathT.value, (int)birthT.value, (int)fullness.value, 7, (int)caveAlive.value, (int)islandAlive.value);
        else
            lastCave = MapManager2D.CreateCave(lastPos, MapManager2D.mapManager.allBlockTypes[0], 60, 60, (GenerationPreset)preset);

        lastCave.transform.localScale = lastSize;
    }

    GameObject lastDungeon = null;

    public void MakeDungeon()
    {
        Vector3 lastSize = Vector3.one * 0.3F;
        Vector3 lastPos = new Vector3(35, 50, 70);

        if (lastDungeon)
        {
            lastSize = lastDungeon.transform.localScale;
            lastPos = lastDungeon.transform.localPosition;

            Destroy(lastDungeon);
        }

        MapManager2D.mapManager.passageWidth = (int)passageWidth.value;
        MapManager2D.mapManager.passageBorderThickness = (int)passageBorder.value;

        MapManager2D.mapManager.smoothMesh = smoothMesh.isOn;

        if (textureMesh.isOn)
            lastDungeon = MapManager2D.CreateDungeon(lastPos, (int)roomSize.value, (int)roomSize.value, (int)roomCount.value, (int)roomCount.value, 40-(int)roomFullness.value, 3, 1, 6, (int)roomSize.value + 5, (int)roomSize.value + 10, MapManager2D.mapManager.allBlockTypes[3], smooth: smoothMesh.isOn);
        else
            lastDungeon = MapManager2D.CreateDungeon(lastPos, (int)roomSize.value, (int)roomSize.value, (int)roomCount.value, (int)roomCount.value, 40-(int)roomFullness.value, 3, 1, 6, (int)roomSize.value + 5, (int)roomSize.value + 10, smooth: smoothMesh.isOn);


        lastDungeon.transform.localScale = lastSize;
    }

    public void CellularScroll()
    {
        Scroll();

        panel.Find("Cellular Automata").gameObject.SetActive(true);
        panel.Find("Cellular Automata").Find("CELLULAR AUTOMATA").GetComponent<Rigidbody2D>().simulated = true;
        panel.Find("Cellular Automata").Find("Description").GetComponent<Rigidbody2D>().simulated = true;
    }

    public void DungeonScroll()
    {
        Scroll();
        Invoke("DisableCellular", 0.5F);

        panel.Find("Dungeon").gameObject.SetActive(true);
        panel.Find("Dungeon").Find("DUNGEON").GetComponent<Rigidbody2D>().simulated = true;
        panel.Find("Dungeon").Find("Description").GetComponent<Rigidbody2D>().simulated = true;
    }

    public void SmoothTerrainScroll()
    {
        Scroll();

        Invoke("DisableDungeon", 0.5F);

        panel.Find("Smooth Terrain").gameObject.SetActive(true);
        panel.Find("Smooth Terrain").Find("SMOOTH TERRAIN").GetComponent<Rigidbody2D>().simulated = true;
    }

    public void TerrainScroll()
    {
        Scroll();

        foreach (Chunk2D chunk in Resources.FindObjectsOfTypeAll(typeof(Chunk2D)))
        {
            chunk.gameObject.SetActive(true);
            isTerrain = true;
        }

        Invoke("DisableSmoothTerrain", 0.5F);

        panel.Find("Terrain").gameObject.SetActive(true);
        panel.Find("Terrain").Find("TERRAIN").GetComponent<Rigidbody2D>().simulated = true;
    }

    void DisableCellular()
    {
        if (lastCave) Destroy(lastCave);

        panel.Find("Cellular Automata").gameObject.SetActive(false);
    }

    void DisableDungeon()
    {
        panel.Find("Dungeon").gameObject.SetActive(false);
    }

    void DisableSmoothTerrain()
    {
        panel.Find("Smooth Terrain").gameObject.SetActive(false);
    }

    void DisableTerrain()
    {
        panel.Find("Terrain").gameObject.SetActive(false);
    }

    void Scroll()
    {
        if (lastDungeon) Destroy(lastDungeon);
        if (lastSmoothTerrain) Destroy(lastSmoothTerrain);

        MapManager2D.mapManager.smoothMesh = false;
        smoothMesh.isOn = false;
        scrollingDirection = 1;
        translatePanel = true;

        targetXPosPanel = panel.position.x + (Camera.main.aspect * transDistance);
    }

    void BackToCellular()
    {
        Invoke("DisableDungeon", 0.5F);
        panel.Find("Cellular Automata").gameObject.SetActive(true);


        BackScroll();
    }

    void BackToDungeon()
    {
        Invoke("DisableSmoothTerrain", 0.5F);
        panel.Find("Dungeon").gameObject.SetActive(true);

        BackScroll();
    }

    void Restart()
    {
        isTerrain = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Demo");
    }

    void BackScroll()
    {
        if (lastDungeon) Destroy(lastDungeon);
        if (lastSmoothTerrain) Destroy(lastSmoothTerrain);

        isTerrain = false;

        scrollingDirection = -1;
        translatePanel = true;
        targetXPosPanel = panel.position.x - (Camera.main.aspect * transDistance);
    }



    bool translatePanel;

    float targetXPosPanel = 0F;
    int scrollingDirection = 1;

    Vector3 velocity = new Vector3(0, 0, 0);

    void Update()
    {
        if (translatePanel)
        {
            panel.Translate((Vector3.right * transSpeed * scrollingDirection) * Time.deltaTime);

            if ((scrollingDirection > 0 && panel.position.x >= targetXPosPanel) || (scrollingDirection < 0 && panel.position.x <= targetXPosPanel))
            {
                panel.position = new Vector3(targetXPosPanel, panel.position.y, panel.position.z);
                translatePanel = false;
            }
        }

        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.23F && !isTerrain)
            return;

        if (lastCave)
        {
            float lastCaveSize = lastCave.transform.localScale.x;

            if (Input.GetAxis("Mouse ScrollWheel") > 0 && lastCaveSize < 5F)
            {
                lastCave.transform.localScale += (Vector3.one - Vector3.forward) * Time.deltaTime * 10;
                lastCave.transform.localPosition -= (Vector3.one - Vector3.forward) * Time.deltaTime * 300;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && lastCaveSize > 1F)
            {
                lastCave.transform.localScale -= (Vector3.one - Vector3.forward) * Time.deltaTime * 10;
                lastCave.transform.localPosition += (Vector3.one - Vector3.forward) * Time.deltaTime * 300;
            }

            if (lastCaveSize < 1F) lastCave.transform.localScale = Vector3.one;


            if (Input.GetMouseButton(0))
            {
                velocity.x = Input.GetAxis("Mouse X");
                velocity.y = Input.GetAxis("Mouse Y");
                velocity *= 2;

                lastCave.transform.localPosition += Vector3.Scale(Vector3.one, velocity);
            }
        }
        if (lastDungeon)
        {
            float lastDungeonSize = lastDungeon.transform.localScale.x;

            if (Input.GetAxis("Mouse ScrollWheel") > 0 && lastDungeonSize < 2.15F)
            {
                lastDungeon.transform.localScale += (Vector3.one - Vector3.forward) * Time.deltaTime * 3;
                lastDungeon.transform.localPosition -= (Vector3.one - Vector3.forward) * Time.deltaTime * 300;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && lastDungeonSize > 0.3F)
            {
                lastDungeon.transform.localScale -= (Vector3.one - Vector3.forward) * Time.deltaTime * 3;
                lastDungeon.transform.localPosition += (Vector3.one - Vector3.forward) * Time.deltaTime * 300;
            }

            if (lastDungeonSize < 0.3F) lastDungeon.transform.localScale = Vector3.one * 0.3F;


            if (Input.GetMouseButton(0))
            {
                velocity.x = Input.GetAxis("Mouse X");
                velocity.y = Input.GetAxis("Mouse Y");
                velocity *= 2F;

                lastDungeon.transform.localPosition += Vector3.Scale(Vector3.one, velocity);
            }
        }

        if (isTerrain)
        {
            if (Input.GetMouseButton(0))
            {
                velocity.x = -Input.GetAxis("Mouse X");
                velocity.y = -Input.GetAxis("Mouse Y");

                velocity.x = Mathf.Clamp(velocity.x, -0.75F, 0.75F);
                velocity.y = Mathf.Clamp(velocity.y, -0.75F, 0.75F);

                Camera.main.transform.position += velocity * 2;
            }

            float orthoSize = Camera.main.orthographicSize;

            if (-Input.GetAxis("Mouse ScrollWheel") > 0 && orthoSize < 44F)
            {
                Camera.main.orthographicSize += Time.deltaTime * 80;
            }
            else if (-Input.GetAxis("Mouse ScrollWheel") < 0 && orthoSize > 10F)
            {
                Camera.main.orthographicSize -= Time.deltaTime * 80;
            }
        }
    }

    GameObject lastSmoothTerrain = null;

    public void MakeSmoothTerrain()
    {
        if (lastSmoothTerrain)
            Destroy(lastSmoothTerrain);

        Resources.UnloadUnusedAssets();

        List<NoiseLayer> peaksLayer = new List<NoiseLayer>();

        peaksLayer.Add(new NoiseLayer(scale.value, magnitude.value, exponent.value));

        int[] peaks = Terrain2D.GeneratePeaks(MapManager2D.random.Next(-10000, 10000), 110, peaksLayer);

        Vector2[] original = Terrain2D.SmoothPeaks(peaks);

        GameObject newG = new GameObject("mesh");

        newG.AddComponent<MeshFilter>().mesh = MeshMaker.MakeMesh(original, 1, 700);

        newG.AddComponent<MeshRenderer>().material = new Material(Shader.Find("MapGen2D/TextureColorUnlit"));
        newG.GetComponent<MeshRenderer>().material.color = new Color(70F / 255F, 50F / 225F, 35F / 255F);
        newG.transform.position = (Vector3.up * 80) + (Vector3.forward * 40) + (Vector3.right * 45) + (Camera.main.transform.position - new Vector3(86, 95, -10));

        GameObject grass = new GameObject("mesh");

        grass.AddComponent<MeshFilter>().mesh = MeshMaker.MakeMesh(original, 1, 2);

        grass.AddComponent<MeshRenderer>().material = new Material(Shader.Find("MapGen2D/TextureColorUnlit"));
        grass.GetComponent<MeshRenderer>().material.color = new Color(30F / 255F, 120F / 255F, 30F / 255F);

        grass.transform.position = newG.transform.position + Vector3.back;
        grass.transform.parent = newG.transform;

        lastSmoothTerrain = newG;
    }

    bool isTerrain;

    public void MakeTerrain()
    {
        Camera.main.orthographicSize = 30F;
        StartCoroutine(MapManager2D.mapManager.GenerateChunk(2, 5, 0.4F));

        Invoke("SetTerrain", 3F);
    }

    void SetTerrain()
    {
        isTerrain = true;
    }

    public void ChangeBiome()
    {
        Terrain2D.SetBiome(biome.value);
    }
}
