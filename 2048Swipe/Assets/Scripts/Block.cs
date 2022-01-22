using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;

[System.Serializable]
public class Block : MonoBehaviour
{

    [HideInInspector]
    public bool merged = false;

    [SerializeField]
    private int level;

    [SerializeField]
    private MeshRenderer renderer;

    [SerializeField]
    private TrailRenderer trail;

    [SerializeField]
    private Text numberText;


    private bool destroying = false;

    private void Awake()
    {
        if(IsDigitsOnly(numberText.text))
        {
            level = Mathf.RoundToInt(Mathf.Log(float.Parse(numberText.text), 2F)) - 1;
            renderer.sharedMaterial = BlockManager.instance.materialsByBlockLevel[level];
            var c = renderer.sharedMaterial.color;
            var a = trail.sharedMaterial.color.a;
            c.a = a;
            trail.sharedMaterial = trail.material;
            trail.sharedMaterial.color = c;
        }
    }

    public void Initialize(int _level)
    {
        level = _level;
        renderer.sharedMaterial = BlockManager.instance.materialsByBlockLevel[level];
        numberText.text = BlockManager.instance.numbersByBlockLevel[level].ToString();
        ScaleUp();
        Controller.instance.enabledControls = true;
        if (level == BlockManager.instance.targetLevel)
            GameManager.instance.Win();

        if (IsDigitsOnly(numberText.text))
        {
            level = Mathf.RoundToInt(Mathf.Log(float.Parse(numberText.text), 2F)) - 1;
            renderer.sharedMaterial = BlockManager.instance.materialsByBlockLevel[level];
            var c = renderer.sharedMaterial.color;
            var a = trail.sharedMaterial.color.a;
            c.a = a;
            trail.sharedMaterial.color = c;
        }
    }

    public int GetLevel()
    {
        return level;
    }
    public void ScaleUp()
    {
        System.Action<ITween<Vector3>> updateCurrentBlockPos = (t) =>
        {
            transform.localScale = t.CurrentValue;
        };

        TweenFactory.Tween(null, Vector3.zero, transform.localScale,
            1F / Controller.instance.blockScaleSpeed, TweenScaleFunctions.SineEaseOut, updateCurrentBlockPos);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (merged)
            return;

        Block block = col.GetComponent<Block>();

        if (block)
        {
            merged = true;
            block.merged = true;

            Vector3 neededPos;

            if (gameObject.layer == LayerMask.NameToLayer("Block"))
            {
                neededPos = transform.position;
            }
            else
            {
                neededPos = col.transform.position;
            }

            neededPos.x = Mathf.Round(neededPos.x);
            neededPos.z = Mathf.Round(neededPos.z);

            if (level < 11)
                BlockManager.instance.CreateNewBlock(level + 1, neededPos);                

            StartDestroying();
            block.StartDestroying();
        }
    }

    public bool TryMerge(Block block)
    {
        if (block.level == level)
            return true;

        return false;
    }

    public void StartDestroying()
    {
        foreach (var collider in GetComponents<Collider>())
        {
            Destroy(collider);
        }
        System.Action<ITween<Vector3>> updateCurrentBlockPos = (t) =>
        {
            transform.localScale = t.CurrentValue;
        };

        TweenFactory.Tween(null, transform.localScale, Vector3.zero,
            1F/Controller.instance.blockScaleSpeed, TweenScaleFunctions.SineEaseOut, updateCurrentBlockPos);
    }

    private bool IsDigitsOnly(string str)
    {
        if (str.Length < 1)
            return false;

        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }
}
