using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Knife : MonoBehaviour
{

    public static Knife instance { get; private set; }

    public Transform pin0, pin1;

    public Vector3 offset;

    [SerializeField]
    private bool ropeMode;

    [SerializeField]
    private float ropeTearDuration;

    [SerializeField]
    private GameObject ropeBurnFX;

    [SerializeField]
    private int tracingIterations;

    [SerializeField]
    private bool destroyCompletelyAfterHit;

    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private GameObject painter;

    [SerializeField]
    private KnifeDestroyed destroyedKnife;

    [SerializeField]
    private GameObject brakeSFX;

    [SerializeField]
    private bool dontShowBreakSFX;

    private bool broken = false;

    [SerializeField]
    private Material[] materials;

    private float ropeTearProgress = 0F;

    private Transform ropeCopy = null;

    private GameObject ropeBurnInst0, ropeBurnInst1;

    private bool ropeTearCompleted = false;

    private Vector3 tearPoint = Vector3.zero;

    private Vector3 lastKnifPos = Vector3.zero;

    private Vector3 lastKnifeForward = Vector3.zero;

    private void Awake()
    {
        instance = this;
    }

    private void UpdateSkin()
    {
        meshRenderer.sharedMaterial = materials[PlayerPrefs.GetInt("SelectedKnife", 0)];
    }

    private void Start()
    {
        Controller.instance.OnRopeTorn += Brake;

        UIManager.instance.OnShopSelectionChanged += UpdateSkin;

        UpdateSkin();

        DOTween.Sequence().SetDelay(0.1F).OnComplete(EnablePainter);
    }

    private void EnablePainter()
    {
        painter.SetActive(true);
    }

    private void OnDestroy()
    {
        Controller.instance.OnRopeTorn -= Brake;
    }

    private void Brake(Vector3 _tearPoint)
    {
        if (ropeMode)
        {
            broken = true;

            tearPoint = _tearPoint;

            DOTween.To(() => ropeTearProgress, x => ropeTearProgress = x, 1F, ropeTearDuration).SetEase(Ease.OutSine);

            ropeCopy = Instantiate(gameObject).transform;

            ropeBurnInst0 = Instantiate(ropeBurnFX, (pin0.position + pin1.position) / 2F, Quaternion.identity);

            ropeBurnInst1 = Instantiate(ropeBurnFX, ropeBurnInst0.transform.position, Quaternion.identity);

            Destroy(ropeCopy.GetComponent<Knife>());
        }
        else
        {
            if (destroyCompletelyAfterHit){
                //gameObject.SetActive(false);
                gameObject.transform.Find("lightnings").gameObject.SetActive(false);
            }else
            {
                GetComponent<MeshRenderer>().enabled = false;

                if (!dontShowBreakSFX)
                    Instantiate(brakeSFX, transform.position, Quaternion.identity);

                var destroyedKnifeInstance = Instantiate(destroyedKnife, transform.position, transform.rotation);
                destroyedKnifeInstance.transform.localScale = transform.localScale;
                destroyedKnifeInstance.ApplyForce(tearPoint);
            }
        }
    }

    private void LateUpdate()
    {
        if(ropeMode && broken)
        {
            RopeTearProgression();

            return;
        }

        CalculateKnifePos();

    }

    private void RopeTearProgression()
    {
        if (ropeTearCompleted)
            return;

        transform.position = pin0.position + offset;
        transform.rotation = Quaternion.LookRotation((pin0.position - pin1.position).normalized, Vector3.up);
        var rot = transform.rotation.eulerAngles;
        rot.x = -90F;
        transform.rotation = Quaternion.Euler(rot);

        var scale = transform.localScale;
        scale.y = Mathf.Lerp(Vector3.Distance(pin0.position, tearPoint), 0F, ropeTearProgress);
        transform.localScale = scale;

        var rope0EndPos = Vector3.Lerp(tearPoint, pin0.position, ropeTearProgress);

        ropeBurnInst0.transform.position = rope0EndPos;


        ropeCopy.position = pin1.position + offset;
        ropeCopy.rotation = Quaternion.LookRotation((pin1.position - pin0.position).normalized, Vector3.up);
        rot = ropeCopy.rotation.eulerAngles;
        rot.x = -90F;
        ropeCopy.rotation = Quaternion.Euler(rot);

        scale = ropeCopy.localScale;
        scale.y = Mathf.Lerp(Vector3.Distance(tearPoint, pin1.position), 0F, ropeTearProgress);
        ropeCopy.localScale = scale;

        var rope1EndPos = Vector3.Lerp(tearPoint, pin1.position, ropeTearProgress);

        ropeBurnInst1.transform.position = rope1EndPos;

        if(ropeTearProgress >= 1F)
        {
            ropeTearCompleted = true;

            //ropeBurnInst0.SetActive(false);
            //ropeBurnInst1.SetActive(false);
        }
    }

    private void CalculateKnifePos()
    {
        var currentPos = pin0.position + offset;

        transform.position = currentPos;

        var currentForward = (pin0.position - pin1.position).normalized;

        transform.rotation = Quaternion.LookRotation(currentForward, Vector3.up);
        var rot = transform.rotation.eulerAngles;
        rot.x = -90F;
        transform.rotation = Quaternion.Euler(rot);

        var scale = transform.localScale;
        scale.y = Vector3.Distance(pin0.position, pin1.position);
        transform.localScale = scale;

        if (lastKnifPos != Vector3.zero)
        {

            for (int i = 0; i < tracingIterations; i++)
            {
                var pos = Vector3.Lerp(lastKnifPos, currentPos, (i + 1F) / tracingIterations);

                var forward = Vector3.Lerp(lastKnifeForward, -currentForward, (i + 1F) / tracingIterations);

                var result = Physics.RaycastAll(pos, forward, Vector3.Distance(pin0.position, pin1.position), Physics.AllLayers, QueryTriggerInteraction.Collide);

                foreach (var hit in result)
                {
                    var cutChecker = hit.collider.GetComponent<CutChecker>();

                    if (cutChecker)
                    {
                        cutChecker.PassChecker();
                    }
                }
            }
        }

        lastKnifPos = currentPos;

        lastKnifeForward = -currentForward;
    }

}
