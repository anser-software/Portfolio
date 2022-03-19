using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CellularAutomata))]
[System.Serializable]
public class CellularAutomataEditor : Editor {

    [MenuItem("Component/GenX 3D/Cellular Automata")]
    static void MenuOption()
    {
        Undo.AddComponent<CellularAutomata>(Selection.activeGameObject);      
    }

    CellularAutomata targetScript;

    GameObject testInstance = null;

    Material testInstMat;

    void OnEnable()
    {
        testInstance = GameObject.Find("Cellular Automata Test");

        targetScript = (CellularAutomata)target;
        testInstMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/GenX 3D/Demo/Materials/Grass.mat");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical("ObjectFieldThumb");

        targetScript.width = EditorGUILayout.IntField("Width", targetScript.width);
        targetScript.height = EditorGUILayout.IntField("Height", targetScript.height);
        targetScript.zDepth = EditorGUILayout.IntField("Depth", targetScript.zDepth);

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("", (GUIStyle)"ChannelStripAttenuationBar");

        EditorGUILayout.BeginVertical("ObjectFieldThumb");

        targetScript.material = (Material)EditorGUILayout.ObjectField("Material", targetScript.material, typeof(Material), true);
        EditorGUILayout.Space();

        targetScript.useSeed = EditorGUILayout.Toggle("Use Seed", targetScript.useSeed);

        if (targetScript.useSeed)
            targetScript.seed = EditorGUILayout.TextField("Seed", targetScript.seed);

        EditorGUILayout.Space();

        targetScript.deathThreshold = EditorGUILayout.IntSlider("Death Threshold", targetScript.deathThreshold, 0, 26);
        targetScript.birthThreshold = EditorGUILayout.IntSlider("Birth Threshold", targetScript.birthThreshold, 0, 26);
        targetScript.chanceToStartAlive = EditorGUILayout.IntSlider("Chance To Start Alive", targetScript.chanceToStartAlive, 0, 100);
        targetScript.outputGeneration = EditorGUILayout.IntField("Output Generation", targetScript.outputGeneration);

        EditorGUILayout.Space();

        targetScript.invert = EditorGUILayout.Toggle("Invert", targetScript.invert);

        //if (targetScript.invert)
            targetScript.makeHollow = EditorGUILayout.Toggle("Make Hollow", targetScript.makeHollow);

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Create Test Object", (GUIStyle)"PreButton"))
        {
            DestroyImmediate(testInstance);          

            bool[,,] map = targetScript.Generate();

            if ((map.GetLength(0) * map.GetLength(1) * map.GetLength(2)) * 2 < 65000)
            {
                testInstance = new GameObject("Cellular Automata Test");

                if(targetScript.material == null)
                    testInstance.AddComponent<MeshRenderer>().material = testInstMat;
                else
                    testInstance.AddComponent<MeshRenderer>().material = targetScript.material;

                testInstance.AddComponent<MeshFilter>().mesh = MeshMakerBlocks.GenerateMesh(map);
            }
            else
            {
                Mesh[] meshes = MeshMakerBlocks.GenerateMeshes(map);

                testInstance = new GameObject("Cellular Automata Test");

                for (int i = 0; i < meshes.Length; i++)
                {
                    GameObject currentMesh = new GameObject(i.ToString());

                    if (targetScript.material == null)
                        currentMesh.AddComponent<MeshRenderer>().material = testInstMat;
                    else
                        currentMesh.AddComponent<MeshRenderer>().material = targetScript.material;

                    currentMesh.AddComponent<MeshFilter>().mesh = meshes[i];

                    currentMesh.transform.parent = testInstance.transform;
                }
            }

        }

        if (testInstance != null)
        {
            if (GUILayout.Button("Destroy", (GUIStyle)"PreButton"))
            {
                DestroyImmediate(testInstance);
            }
        }
    }

}
