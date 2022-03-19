using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoxelTerrain))]
[System.Serializable]
public class VoxelTerrainEditor : Editor
{

    [MenuItem("Component/GenX 3D/Voxel Terrain")]
    static void MenuOption()
    {
        Undo.AddComponent<VoxelTerrain>(Selection.activeGameObject);
    }

    VoxelTerrain targetScript;

    GameObject testInstance = null;

    void OnEnable()
    {
        targetScript = (VoxelTerrain)target;
    }

    public override void OnInspectorGUI()
    {
        if (VoxelTerrain.main == null)
            VoxelTerrain.main = targetScript;

        EditorGUILayout.LabelField("", (GUIStyle)"ChannelStripAttenuationBar");

        EditorGUILayout.BeginVertical("ObjectFieldThumb");

        targetScript.size = EditorGUILayout.IntField("Size", targetScript.size);

        EditorGUILayout.EndVertical();


        EditorGUILayout.LabelField("", (GUIStyle)"ChannelStripAttenuationBar");

        EditorGUILayout.BeginVertical("ObjectFieldThumb");

        targetScript.useSeed = EditorGUILayout.Toggle("Use Seed", targetScript.useSeed);

        if (targetScript.useSeed)
            targetScript.seed = EditorGUILayout.TextField("Seed", targetScript.seed);

        EditorGUILayout.EndVertical();


        EditorGUILayout.LabelField("", (GUIStyle)"ChannelStripAttenuationBar");

        EditorGUILayout.BeginVertical("ObjectFieldThumb");

        targetScript.caveNoise.scale = EditorGUILayout.FloatField("Caves Scale", targetScript.caveNoise.scale);
        targetScript.caveNoise.magnitude = EditorGUILayout.FloatField("Caves Magnitude", targetScript.caveNoise.magnitude);
        targetScript.caveNoise.exponent = EditorGUILayout.FloatField("Caves Exponent", targetScript.caveNoise.exponent);

        EditorGUILayout.Space();

        targetScript.caveSize = EditorGUILayout.FloatField("Caves Size", targetScript.caveSize);

        EditorGUILayout.EndVertical();


        EditorGUILayout.LabelField("", (GUIStyle)"ChannelStripAttenuationBar");

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("ObjectFieldThumb");

        if (GUILayout.Button("Create Test Object", (GUIStyle)"PreButton"))
        {
            DestroyImmediate(testInstance);

            Voxel[,,] map = targetScript.GenerateTerrain(0, 0, 0);

            if ((map.GetLength(0) * map.GetLength(1) * map.GetLength(2)) * 2 < 65000)
            {
                testInstance = new GameObject("Voxel Terrain Test");

                Material[] mats = new Material[targetScript.blockTypes.Count];

                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = targetScript.blockTypes[i].material;
                }

                testInstance.AddComponent<MeshRenderer>().materials = mats;


                testInstance.AddComponent<MeshFilter>().mesh = MeshMakerBlocks.GenerateMesh(map, targetScript.blockTypes.Count);
            }
            else
            {
                Mesh[] meshes = MeshMakerBlocks.GenerateMeshes(map, targetScript.blockTypes.Count);

                testInstance = new GameObject("Voxel Terrain Test");

                for (int i = 0; i < meshes.Length; i++)
                {
                    GameObject currentMesh = new GameObject(i.ToString());

                    Material[] mats = new Material[targetScript.blockTypes.Count];

                    for (int j = 0; j < mats.Length; j++)
                    {
                        mats[j] = targetScript.blockTypes[j].material;
                    }

                    currentMesh.AddComponent<MeshRenderer>().materials = mats;

                    currentMesh.AddComponent<MeshFilter>().mesh = meshes[i];

                    currentMesh.transform.parent = testInstance.transform;
                }
            }
        }

        EditorGUILayout.EndVertical();


        EditorGUILayout.Space();

        EditorGUILayout.LabelField("BLOCKS", (GUIStyle)"OL header");


        EditorGUILayout.LabelField("", (GUIStyle)"ChannelStripAttenuationBar");

        foreach (BlockType block in VoxelTerrain.main.blockTypes)
        {

            EditorGUILayout.BeginVertical("ObjectFieldThumb");

            EditorGUILayout.BeginVertical();

            block.name = EditorGUILayout.TextField("Name", block.name);

            if (GUILayout.Button("Duplicate", "PreButton"))
            {
                targetScript.blockTypes.Add(new BlockType(block));
                break;
            }

            int index = targetScript.blockTypes.IndexOf(block);

            if (index > 0)
            {
                if (GUILayout.Button("Move Up", "PreButton"))
                {
                    BlockType thisBlock = block;

                    targetScript.blockTypes.Remove(block);
                    targetScript.blockTypes.Insert(index - 1, thisBlock);
                    break;
                }
            }

            block.hideInEditor = EditorGUILayout.Toggle(block.hideInEditor, (GUIStyle)"Foldout");

            if (block.hideInEditor)
            {
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndVertical();
                continue;
            }

            block.material = (Material)EditorGUILayout.ObjectField("Material", block.material, typeof(Material), false);

            EditorGUILayout.Space();

            block.canHaveCavesInside = EditorGUILayout.Toggle("Can Have Caves Inside", block.canHaveCavesInside);

            EditorGUILayout.Space();

            block.yOffset = EditorGUILayout.IntField("Y Offset", block.yOffset);
            block.minY = EditorGUILayout.IntField("Min Y", block.minY);
            block.maxY = EditorGUILayout.IntField("Max Y", block.maxY);

            EditorGUILayout.Space();

            block.genType = (GenerationType)EditorGUILayout.EnumPopup("Generation Type", block.genType);

            switch (block.genType)
            {
                case GenerationType.Range:
                    if (GUILayout.Button("Add Noise Layer", "OL Plus"))
                    {
                        block.noiseLayers.Add(new NoiseLayer());
                    }

                    EditorGUILayout.BeginVertical("ObjectFieldThumb");

                    foreach (NoiseLayer n in block.noiseLayers)
                    {
                        n.scale = EditorGUILayout.FloatField("Scale", n.scale);
                        n.magnitude = EditorGUILayout.FloatField("Magnitude", n.magnitude);
                        n.exponent = EditorGUILayout.FloatField("Exponent", n.exponent);

                        if (GUILayout.Button("Remove Layer", "OL Minus"))
                        {
                            block.noiseLayers.Remove(n);
                            break;
                        }
                    }

                    EditorGUILayout.EndVertical();
                    break;
                case GenerationType.Group:
                    block.groupSize = EditorGUILayout.IntSlider("Group Size", block.groupSize, 0, 100);
                    break;
                case GenerationType.Random:
                    block.spawnChance = Mathf.Clamp(EditorGUILayout.FloatField("Spawn Chance", block.spawnChance), 0F, 100F);
                    break;
            }

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Remove Block", "OL Minus"))
            {
                targetScript.blockTypes.Remove(block);
                break;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }


        if (GUILayout.Button("New Block Type", "OL Plus"))
        {
            targetScript.blockTypes.Add(new BlockType());
        }
    }
}