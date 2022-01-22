using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AssembleController : MonoBehaviour
{

    public static AssembleController Instance { get; private set; }

    [SerializeField]
    private GameObject tapePrefab, smallConfetti;

    [SerializeField]
    private float height;

    [SerializeField]
    private Material[] tapeMats;

    [SerializeField]
    private GameObject oldLaptop, finishedLaptop;

    [SerializeField]
    private float scaleDuration, finishDelay;

    private Transform currentTape = null;

    private AssembleTapeEndpoint startATE;

    private Camera mainCam;

    private int totalATECount, deactivatedATECount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;

        totalATECount = FindObjectsOfType<AssembleTapeEndpoint>().Length;
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            if(currentTape)
            {
                var touchWorldPos = GetTouchWorldPos();

                touchWorldPos.y = currentTape.position.y;

                var dif = touchWorldPos - currentTape.position;

                currentTape.forward = dif.normalized;

                var scale = currentTape.localScale;

                scale.z = dif.magnitude / 4F;

                currentTape.localScale = scale;
            }
        }
    }

    private Vector3 GetTouchWorldPos()
    {
        var ray = mainCam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Floor"), QueryTriggerInteraction.Ignore))
        {
            return hit.point + Vector3.up * height;
        }

        return Vector3.zero;
    }

    public void StartDragging(AssembleTapeEndpoint startPoint)
    {
        currentTape = Instantiate(tapePrefab, startPoint.transform.position + Vector3.up * height, Quaternion.identity).transform;

        currentTape.GetComponentInChildren<MeshRenderer>().sharedMaterial = tapeMats[Random.Range(0, tapeMats.Length)];

        startATE = startPoint;
    }

    public void TrySnapping(AssembleTapeEndpoint endPoint)
    {
        if (currentTape == null || endPoint != startATE.GetLinkedPoint())
            return;

        var targetPoint = endPoint.transform.position;

        Instantiate(smallConfetti, targetPoint, Quaternion.identity);

        var dif = targetPoint - currentTape.position;

        currentTape.forward = dif.normalized;

        var scale = currentTape.localScale;

        scale.z = dif.magnitude / 4F;

        currentTape.localScale = scale;

        currentTape.parent = oldLaptop.transform;

        currentTape = null;

        endPoint.Deactivate();

        startATE.Deactivate();
    }

    public void ATEDeactivated()
    {
        deactivatedATECount++;

        if (deactivatedATECount == totalATECount)
            CompletedAssemble();
    }

    private void CompletedAssemble()
    {
        var ates = GameObject.FindGameObjectsWithTag("AssembleTape");

        foreach (var a in ates)
        {
            if (a)
                Destroy(a);
        }

        var seq = DOTween.Sequence();

        seq.Append(oldLaptop.transform.DOScale(0F, scaleDuration).SetEase(Ease.InSine));
        seq.Join(DOTween.Sequence().SetDelay(finishDelay).OnComplete(() => GameManager.instance.Win()));
        seq.Append(finishedLaptop.transform.DOScale(1F, scaleDuration).SetEase(Ease.OutSine));
    }
}
