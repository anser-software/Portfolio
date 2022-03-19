using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace GenX2D
{
    [CustomEditor(typeof(MapManager2D))]
    [System.Serializable]
    public class MapManagerEditor2D : Editor
    {

        [MenuItem("Component/Map Manager 2D")]
        static void MenuOption()
        {
            if (FindObjectsOfType<MapManager2D>().Length == 0 && Selection.activeGameObject)
                Undo.AddComponent<MapManager2D>(Selection.activeGameObject);
            else
                Debug.LogError("Only one 'MapManager2D' script can exist in the scene.");
        }

        public void Reset()
        {
            if (FindObjectsOfType<MapManager2D>().Length > 1)
            {
                Debug.LogError("Only one 'MapManager2D' script can exist in the scene.");
                DestroyImmediate(target);
            }
        }

        MapManager2D targetMapManager;
        GUIStyle centeredLabel;

        Texture2D[] bitmaskExamples = new Texture2D[16];

        void OnEnable()
        {
            targetMapManager = (MapManager2D)target;

            for (int i = 0; i < bitmaskExamples.Length; i++)
            {
                bitmaskExamples[i] = MeshTextureGenerator.Scale(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GenX Framework 2D/Editor/UI/Bit Mask/" + i + ".png"), 96, 96, FilterMode.Point);
            }
        }

        string[] menuOptions = new string[2] { "General", "Cellular Automata" };

        string[] typeOptions = new string[3] { "Cellular Automata", "Terrain", "Dungeon" };



        public override void OnInspectorGUI()
        {
            centeredLabel = GUI.skin.GetStyle("HeaderLabel");
            centeredLabel.alignment = TextAnchor.UpperCenter;

            menuOptions[1] = typeOptions[serializedObject.FindProperty("typeSelection").intValue];

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            serializedObject.FindProperty("menuSelection").intValue = GUILayout.Toolbar(serializedObject.FindProperty("menuSelection").intValue, menuOptions, "PreButton");
            GUILayout.EndHorizontal();

            if (serializedObject.FindProperty("menuSelection").intValue == 0)
            {

                EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");
                EditorGUILayout.BeginVertical("ObjectFieldThumb");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Generation Type", (GUIStyle)"WhiteBoldLabel");

                serializedObject.FindProperty("typeSelection").intValue = EditorGUILayout.Popup(serializedObject.FindProperty("typeSelection").intValue, typeOptions, "PreDropDown");

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Square Size", (GUIStyle)"WhiteBoldLabel");

                targetMapManager.squareSize = EditorGUILayout.FloatField(targetMapManager.squareSize);

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Generate Colliders", (GUIStyle)"WhiteBoldLabel");

                targetMapManager.generateColliders = EditorGUILayout.Toggle(targetMapManager.generateColliders);

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Allow Destruction", (GUIStyle)"WhiteBoldLabel");

                targetMapManager.allowDestruction = EditorGUILayout.Toggle(targetMapManager.allowDestruction);

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.EndVertical();


                EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                EditorGUILayout.BeginVertical("ObjectFieldThumb");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Use Seed", (GUIStyle)"WhiteBoldLabel");

                targetMapManager.useSeed = EditorGUILayout.Toggle(targetMapManager.useSeed);

                EditorGUILayout.EndHorizontal();



                if (targetMapManager.useSeed)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Seed", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.seed = EditorGUILayout.TextField(targetMapManager.seed);

                    EditorGUILayout.EndHorizontal();
                }
                else
                    targetMapManager.seed = "";

                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                EditorGUILayout.BeginVertical("ObjectFieldThumb");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Smooth the Mesh", (GUIStyle)"WhiteBoldLabel");

                targetMapManager.smoothMesh = EditorGUILayout.Toggle(targetMapManager.smoothMesh);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Mesh Material", (GUIStyle)"WhiteBoldLabel");

                targetMapManager.meshMaterial = (Material)EditorGUILayout.ObjectField(targetMapManager.meshMaterial, typeof(Material), true);

                EditorGUILayout.EndHorizontal();



                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");
                //***Blocks***

                EditorGUILayout.LabelField("BLOCKS", centeredLabel);

                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("New Block Type", "OL Plus"))
                {
                    targetMapManager.allBlockTypes.Add(new BlockType2D());
                }


                foreach (BlockType2D block in targetMapManager.allBlockTypes)
                {
                    EditorGUILayout.BeginHorizontal();
                    block.hideInEditor = EditorGUILayout.Toggle(block.hideInEditor, "Foldout");



                    Rect blockFoldoutRect = GUILayoutUtility.GetLastRect();
                    blockFoldoutRect.x += 5;
                    blockFoldoutRect.y -= 1;

                    GUI.Label(blockFoldoutRect, "\"" + block.name + "\"", "OL header");

                    if (GUILayout.Button("Duplicate", "PreButton"))
                    {
                        targetMapManager.allBlockTypes.Add(new BlockType2D(block));
                        break;
                    }

                    int index = targetMapManager.allBlockTypes.IndexOf(block);

                    if (index > 0)
                    {
                        if (GUILayout.Button("Move Up", "PreButton"))
                        {
                            BlockType2D thisBlock = block;

                            targetMapManager.allBlockTypes.Remove(block);
                            targetMapManager.allBlockTypes.Insert(index - 1, thisBlock);
                            break;
                        }
                    }

                    if (block.hideInEditor)
                    {
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();
                        continue;
                    }
                    else
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginVertical("ObjectFieldThumb");
                    }

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    block.name = EditorGUILayout.TextField(block.name);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Drawing Type", (GUIStyle)"WhiteBoldLabel");

                    block.thisDrawingType = (DrawingType)EditorGUILayout.EnumPopup(block.thisDrawingType, "PreDropDown");

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    if (block.thisDrawingType == DrawingType.OneTexture)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Texture", (GUIStyle)"WhiteBoldLabel");

                        block.mainTexture = (Texture2D)EditorGUILayout.ObjectField(block.mainTexture, typeof(Texture2D), false);

                        EditorGUILayout.EndHorizontal();
                    }
                    else if (block.thisDrawingType == DrawingType.Tilemap)
                    {
                        Rect hideTileRect = EditorGUILayout.GetControlRect();
                        hideTileRect.x += 11;

                        block.hideTileTextures = EditorGUI.Foldout(hideTileRect, block.hideTileTextures, "Tile Textures");


                        if (!block.hideTileTextures)
                        {
                            if (block.tileTextures.Length < 1)
                                block.tileTextures = new Texture2D[16];

                            for (int i = 0; i < block.tileTextures.Length; i++)
                            {
                                EditorGUILayout.BeginHorizontal();

                                GUILayout.Box(bitmaskExamples[i], GUILayout.Width(60), GUILayout.Height(60));
                                block.tileTextures[i] = (Texture2D)EditorGUILayout.ObjectField(string.Empty, block.tileTextures[i], typeof(Texture2D), false);
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    else if (block.thisDrawingType == DrawingType.RandomTexture)
                    {

                        if (GUILayout.Button("Add Texture", "OL Plus"))
                        {
                            block.randomTextures.Add(null);
                        }

                        EditorGUILayout.BeginVertical("ObjectFieldThumb");

                        for (int i = 0; i < block.randomTextures.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();

                            if (GUILayout.Button("Remove", "OL Minus"))
                            {
                                block.randomTextures.RemoveAt(i);
                                break;
                            }

                            block.randomTextures[i] = (Texture2D)EditorGUILayout.ObjectField(string.Empty, block.randomTextures[i], typeof(Texture2D), false);
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.EndVertical();
                    }

                    Rect _foldoutRect = EditorGUILayout.GetControlRect();
                    _foldoutRect.x += 11;

                    block.foldoutInEditor = EditorGUI.Foldout(_foldoutRect, block.foldoutInEditor, "Biomes to generate in");

                    EditorGUILayout.Space();

                    if (!block.foldoutInEditor)
                    {
                        foreach (Biome b in targetMapManager.allBiomes)
                        {
                            if (block.biomes.Contains(b.index))
                            {
                                if (GUILayout.Button(b.name, "sv_label_1"))
                                {
                                    block.biomes.Remove(b.index);
                                }
                            }
                            else
                            {
                                if (GUILayout.Button(b.name, "sv_label_0"))
                                {
                                    block.biomes.Add(b.index);
                                }
                            }
                            EditorGUILayout.Space();
                        }
                    }

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Can Have Caves Inside", (GUIStyle)"WhiteBoldLabel");

                    block.canHaveCavesInside = EditorGUILayout.Toggle(block.canHaveCavesInside);

                    EditorGUILayout.EndHorizontal();



                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Max Y spawn", (GUIStyle)"WhiteBoldLabel");

                    block.maxY = EditorGUILayout.IntField(block.maxY);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Min Y spawn", (GUIStyle)"WhiteBoldLabel");

                    block.minY = EditorGUILayout.IntField(block.minY);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Generation Type", (GUIStyle)"WhiteBoldLabel");

                    block.thisGenType = (GenerationType)EditorGUILayout.EnumPopup(block.thisGenType, "PreDropDown");

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Separator();

                    switch (block.thisGenType)
                    {
                        case GenerationType.Random:
                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Spawn Chance", (GUIStyle)"WhiteBoldLabel");

                            block.spawnChance = Mathf.Clamp(EditorGUILayout.FloatField(block.spawnChance), 0, 100);

                            EditorGUILayout.EndHorizontal();
                            break;
                        case GenerationType.Group:
                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Group Size", (GUIStyle)"WhiteBoldLabel");

                            block.groupSize = EditorGUILayout.IntField(block.groupSize);

                            EditorGUILayout.EndHorizontal();
                            break;
                    }

                    //***Noise Layers***

                    if (block.thisGenType == GenerationType.RangeY)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Y Offset", (GUIStyle)"WhiteBoldLabel");

                        block.yOffset = EditorGUILayout.IntField(block.yOffset);

                        EditorGUILayout.EndHorizontal();

                        if (GUILayout.Button("Add Noise Layer", "OL Plus"))
                        {
                            block.noiseLayers.Add(new NoiseLayer(1, 1, 1));
                        }


                        foreach (NoiseLayer noiseLayer in block.noiseLayers)
                        {
                            EditorGUILayout.BeginVertical("ObjectFieldThumb");

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Scale", (GUIStyle)"WhiteBoldLabel");

                            noiseLayer.scale = EditorGUILayout.FloatField(noiseLayer.scale);

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Magnitude", (GUIStyle)"WhiteBoldLabel");

                            noiseLayer.magnitude = EditorGUILayout.FloatField(noiseLayer.magnitude);

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Exponent", (GUIStyle)"WhiteBoldLabel");

                            noiseLayer.exponent = EditorGUILayout.FloatField(noiseLayer.exponent);

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            if (GUILayout.Button("Remove Layer", "OL Minus"))
                            {
                                block.noiseLayers.Remove(noiseLayer);
                                break;
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                        }
                    }
                    //***End Noise Layers***

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Remove Block", "OL Minus"))
                    {
                        targetMapManager.allBlockTypes.Remove(block);
                        break;
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                //***** STRUCTURES ********

                EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                EditorGUILayout.LabelField("STRUCTURES", centeredLabel);

                if (GUILayout.Button("New Structure", "OL Plus"))
                {
                    targetMapManager.allStructures.Add(new Structure2D());
                }

                targetMapManager.editorHideStructures = EditorGUILayout.Toggle(targetMapManager.editorHideStructures, "Foldout" );

                Rect foldoutRect = GUILayoutUtility.GetLastRect();
                foldoutRect.x += 5;
                foldoutRect.y -= 1;

                GUI.Label(foldoutRect, "Hide All", "OL header");

                if (!targetMapManager.editorHideStructures)
                {
                    foreach (Structure2D structure in targetMapManager.allStructures)
                    {
                        EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                        EditorGUILayout.BeginVertical("ObjectFieldThumb");

                        if (structure.objectPrefab != null)
                            GUILayout.Box(AssetPreview.GetAssetPreview(structure.objectPrefab), GUILayout.Width(64), GUILayout.Height(64));


                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Prefab", (GUIStyle)"WhiteBoldLabel");

                        structure.objectPrefab = (GameObject)EditorGUILayout.ObjectField(structure.objectPrefab, typeof(GameObject), false);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Width", (GUIStyle)"WhiteBoldLabel");

                        structure.width = EditorGUILayout.IntField(structure.width);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Height", (GUIStyle)"WhiteBoldLabel");

                        structure.height = EditorGUILayout.IntField(structure.height);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Biome Dependent", (GUIStyle)"WhiteBoldLabel");

                        structure.biomeDependent = EditorGUILayout.Toggle(structure.biomeDependent);
                        EditorGUILayout.EndHorizontal();


                        if (structure.biomeDependent)
                        {
                            EditorGUILayout.Space();

                            foreach (Biome b in targetMapManager.allBiomes)
                            {
                                if (structure.biomeIndices.Contains(b.index))
                                {
                                    if (GUILayout.Button(b.name, "sv_label_1"))
                                    {
                                        structure.biomeIndices.Remove(b.index);
                                    }
                                }
                                else
                                {
                                    if (GUILayout.Button(b.name, "sv_label_0"))
                                    {
                                        structure.biomeIndices.Add(b.index);
                                    }
                                }
                                EditorGUILayout.Space();
                            }
                        }

                        EditorGUILayout.Space();

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Spawn Chance", (GUIStyle)"WhiteBoldLabel");

                        structure.spawnChance = Mathf.Clamp(EditorGUILayout.IntField(structure.spawnChance), 0, 1000);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Spawn Anywhere", (GUIStyle)"WhiteBoldLabel");

                        structure.spawnAnywhere = EditorGUILayout.Toggle(structure.spawnAnywhere);
                        EditorGUILayout.EndHorizontal();

                        if (!structure.spawnAnywhere)
                        {
                            EditorGUILayout.Space();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Above Ground", (GUIStyle)"WhiteBoldLabel");

                            structure.spawnAboveGround = EditorGUILayout.Toggle(structure.spawnAboveGround);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Below Ground", (GUIStyle)"WhiteBoldLabel");

                            structure.spawnBelowGround = EditorGUILayout.Toggle(structure.spawnBelowGround);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Up", (GUIStyle)"WhiteBoldLabel");

                            structure.spawnOnlyUp = EditorGUILayout.Toggle(structure.spawnOnlyUp);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Down", (GUIStyle)"WhiteBoldLabel");

                            structure.spawnOnlyDown = EditorGUILayout.Toggle(structure.spawnOnlyDown);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Left", (GUIStyle)"WhiteBoldLabel");

                            structure.spawnOnlyLeft = EditorGUILayout.Toggle(structure.spawnOnlyLeft);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Right", (GUIStyle)"WhiteBoldLabel");

                            structure.spawnOnlyRight = EditorGUILayout.Toggle(structure.spawnOnlyRight);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            
                        }

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Inverse", (GUIStyle)"WhiteBoldLabel");

                        structure.inverse = EditorGUILayout.Toggle(structure.inverse);
                        EditorGUILayout.EndHorizontal();

                        if (GUILayout.Button("Remove", "OL Minus"))
                        {
                            targetMapManager.allStructures.Remove(structure);
                            break;
                        }

                        EditorGUILayout.EndVertical();
                    }
                }
            }
            else
            {
                if (serializedObject.FindProperty("typeSelection").intValue == 0)
                {
                    EditorGUILayout.LabelField("CELLULAR AUTOMATA SETTINGS", (GUIStyle)"HeaderLabel");

                    EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                    EditorGUILayout.BeginVertical("ObjectFieldThumb");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Map Width", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.width = EditorGUILayout.IntField(targetMapManager.width);

                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Map Height", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.height = EditorGUILayout.IntField(targetMapManager.height);

                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.EndVertical();

                    EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                    EditorGUILayout.BeginVertical("ObjectFieldThumb");

                    EditorGUILayout.LabelField("Death Threshold", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.deathThreshold = EditorGUILayout.IntSlider(targetMapManager.deathThreshold, 0, 8);

                    EditorGUILayout.LabelField("Birth Threshold", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.birthThreshold = EditorGUILayout.IntSlider(targetMapManager.birthThreshold, 0, 8);

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Chance To Start Alive", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.chanceToStartAlive = EditorGUILayout.IntSlider(targetMapManager.chanceToStartAlive, 0, 100);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Output Generation", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.outputGeneration = EditorGUILayout.IntField(targetMapManager.outputGeneration);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                    EditorGUILayout.BeginVertical("ObjectFieldThumb");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Cave Alive Threshold", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.caveAliveThreshold = EditorGUILayout.IntField(targetMapManager.caveAliveThreshold);

                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Island Alive Threshold", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.islandAliveThreshold = EditorGUILayout.IntField(targetMapManager.islandAliveThreshold);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                }
                else if (serializedObject.FindProperty("typeSelection").intValue == 1)
                {
                    EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                    EditorGUILayout.BeginVertical("ObjectFieldThumb");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Block Texture Size", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.blockTextureSize = EditorGUILayout.IntField(targetMapManager.blockTextureSize);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Chunk Size X", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.chunkSizeX = EditorGUILayout.IntField(targetMapManager.chunkSizeX);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Chunk Size Y", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.chunkSizeY = EditorGUILayout.IntField(targetMapManager.chunkSizeY);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Chunk Unload Distance", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.chunkUnloadDistance = EditorGUILayout.IntField(targetMapManager.chunkUnloadDistance);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();


                    EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                    EditorGUILayout.BeginVertical("ObjectFieldThumb");

                    EditorGUILayout.LabelField("Cave Settings", (GUIStyle)"HeaderLabel");


                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Size of Caves", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.cavesSize = EditorGUILayout.IntField(targetMapManager.cavesSize);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Upper Y Spawn Limit of Caves", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.caveSpawnLimitY = EditorGUILayout.IntField(targetMapManager.caveSpawnLimitY);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Caves Scale", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.caveScale = EditorGUILayout.IntField(targetMapManager.caveScale);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Cave Magnitude",  (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.caveMagnitude = EditorGUILayout.IntField(targetMapManager.caveMagnitude);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                    //***Biomes***

                    EditorGUILayout.LabelField("BIOMES", centeredLabel);

                    if (GUILayout.Button("New Biome", "OL Plus"))
                    {
                        targetMapManager.allBiomes.Add(new Biome("Biome Name", 10, targetMapManager.allBiomes.Count, 0, 0));
                    }

                    targetMapManager.editorHideBiomes = EditorGUILayout.Toggle(targetMapManager.editorHideBiomes, "Foldout");

                    Rect biomefoldoutRect = GUILayoutUtility.GetLastRect();
                    biomefoldoutRect.x += 5;
                    biomefoldoutRect.y -= 1;

                    GUI.Label(biomefoldoutRect, "Hide All", "OL header");

                    if (!targetMapManager.editorHideBiomes)
                    {
                        foreach (Biome b in targetMapManager.allBiomes)
                        {
                            EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");
                            EditorGUILayout.BeginVertical("ObjectFieldThumb");

                            b.name = EditorGUILayout.TextField(b.name);

                            EditorGUILayout.Space();


                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Background Texture", (GUIStyle)"WhiteBoldLabel");

                            b.backgroundTexture = (Texture2D)EditorGUILayout.ObjectField(b.backgroundTexture, typeof(Texture2D), false, GUILayout.Height(16));

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginVertical("flow background");

                            EditorGUILayout.BeginHorizontal();

                            bool noBlocks = true;

                            foreach (BlockType2D block in targetMapManager.allBlockTypes)
                            {
                                if (block.biomes.Contains(b.index))
                                {
                                    if (block.thisDrawingType == DrawingType.Tilemap && block.tileTextures[0] != null)
                                        GUILayout.Box(MeshTextureGenerator.Scale(block.tileTextures[0], 32, 32, FilterMode.Point), GUILayout.Width(40), GUILayout.Height(40));
                                    else
                                        GUILayout.Box(block.mainTexture, GUILayout.Width(40), GUILayout.Height(40));

                                    noBlocks = false;
                                }
                            }

                            if (noBlocks)
                            {
                                EditorGUILayout.LabelField("No blocks assigned", EditorStyles.whiteLabel);
                            }

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Generation Chance", (GUIStyle)"WhiteBoldLabel");

                            SetBiomeGenerationChance(EditorGUILayout.IntField( b.generationChance), targetMapManager.allBiomes.IndexOf(b));

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Min Chunks", (GUIStyle)"WhiteBoldLabel");

                            b.minSize = EditorGUILayout.IntField( b.minSize);

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Max Chunks", (GUIStyle)"WhiteBoldLabel");

                            b.maxSize = EditorGUILayout.IntField(b.maxSize);

                            EditorGUILayout.EndHorizontal();


                            if (GUILayout.Button("Remove", "OL Minus"))
                            {
                                targetMapManager.allBiomes.Remove(b);
                                foreach (BlockType2D block in targetMapManager.allBlockTypes)
                                {
                                    if (block.biomes.Contains(b.index))
                                        block.biomes.Remove(b.index);
                                }
                                break;
                            }

                            EditorGUILayout.EndVertical();
                        }
                    }


                    
                }
                else
                {
                    EditorGUILayout.LabelField("DUNGEON SETTINGS", (GUIStyle)"HeaderLabel");

                    EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                    EditorGUILayout.BeginVertical("ObjectFieldThumb");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Min Room Size", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.minRoomSize = EditorGUILayout.IntField(targetMapManager.minRoomSize);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Max Room Size", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.maxRoomSize = EditorGUILayout.IntField(targetMapManager.maxRoomSize);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Min Room Count", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.minRoomCount = EditorGUILayout.IntField(targetMapManager.minRoomCount);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Max Room Count", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.maxRoomCount = EditorGUILayout.IntField(targetMapManager.maxRoomCount);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Min Distance Between Rooms", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.minDistanceBetweenRooms = EditorGUILayout.IntField(targetMapManager.minDistanceBetweenRooms);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Max Distance Between Rooms", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.maxDistanceBetweenRooms = EditorGUILayout.IntField(targetMapManager.maxDistanceBetweenRooms);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();


                    EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                    EditorGUILayout.BeginVertical("ObjectFieldThumb");

                    EditorGUILayout.LabelField("Border Thickness", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.borderThickness = EditorGUILayout.IntSlider(targetMapManager.borderThickness, 4, 8);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Passage Width", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.passageWidth = EditorGUILayout.IntField(targetMapManager.passageWidth);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Passage Border Thickness", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.passageBorderThickness = EditorGUILayout.IntField(targetMapManager.passageBorderThickness);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();


                    EditorGUILayout.LabelField("", (GUIStyle)"TL LoopSection");

                    EditorGUILayout.BeginVertical("ObjectFieldThumb");

                    EditorGUILayout.LabelField("Room Fullness", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.roomFullness = EditorGUILayout.IntSlider(targetMapManager.roomFullness, 0, 100);

                    EditorGUILayout.LabelField("Room Smoothness", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.roomSmoothness = EditorGUILayout.IntSlider(targetMapManager.roomSmoothness, 0, 10);

                    EditorGUILayout.LabelField("Value Offset", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.valueOffset = EditorGUILayout.IntSlider(targetMapManager.valueOffset, 0, 8);

                    EditorGUILayout.LabelField("Value Difference", (GUIStyle)"WhiteBoldLabel");

                    targetMapManager.valueDifference = EditorGUILayout.IntSlider(targetMapManager.valueDifference, -8, 8);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void SetBiomeGenerationChance(int inputChance, int biomeIndex)
        {
            targetMapManager.allBiomes[biomeIndex].generationChance = Mathf.Clamp(inputChance, 0, 100);

            int[] generationChances = targetMapManager.allBiomes.Select(b => b.generationChance).ToArray();
            int sum = generationChances.Sum();

            if (sum <= 100) return;
            for (int i = 0; i < generationChances.Length; i++)
            {
                if (i != biomeIndex)
                    targetMapManager.allBiomes[i].generationChance = Mathf.Clamp((targetMapManager.allBiomes[i].generationChance / sum) * 100, 0, 100);
            }
        }
    }

}