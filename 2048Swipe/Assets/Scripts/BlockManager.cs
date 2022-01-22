using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour, ISerializationCallbackReceiver
{

    public static BlockManager instance { get; private set; }

    public int targetLevel;

    public int[] numbersByBlockLevel;

    public Material[] materialsByBlockLevel;

    public GameObject[] blockPrefabTypes;

    [Luna.Replay.Playground.LunaPlaygroundField]
    public int blockType;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        var blocks = FindObjectsOfType<Block>();

        for (int i = 0; i < blocks.Length; i++)
        {
            bool current = false;

            if (Controller.instance.currentBlock == blocks[i])
                current = true;

            int level = blocks[i].GetLevel();

            var pos = blocks[i].transform.position;

            DestroyImmediate(blocks[i].gameObject);

            var blockInstance = Instantiate(blockPrefabTypes[blockType], pos, blockPrefabTypes[blockType].transform.rotation);

            var block = blockInstance.GetComponent<Block>();

            block.Initialize(level);

            if (current)
            {
                Controller.instance.currentBlock = block;
            } else
            {
                blockInstance.layer = LayerMask.NameToLayer("Block");
            }
        }
    }

    public void CreateNewBlock(int level, Vector3 position)
    {
        Taptic.Medium();

        var blockInstance = Instantiate(blockPrefabTypes[blockType], position, blockPrefabTypes[blockType].transform.rotation);

        var block = blockInstance.GetComponent<Block>();

        block.Initialize(level);

        Controller.instance.currentBlock = block;
    }

    public void OnBeforeSerialize()
    {
        //throw new System.NotImplementedException();
    }

    public void OnAfterDeserialize()
    {
        //throw new System.NotImplementedException();
    }
}
