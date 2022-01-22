using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AssembleSecondController : MonoBehaviour
{

    public static AssembleSecondController Instance { get; private set; }

    [SerializeField]
    private float height, followSpeed, fallDownDuration, partsMinimumErrorForSnap, finalMoveDuration;

    private Vector3 offset = Vector3.zero;

    private Transform dragTarget;

    private Camera mainCam;

    private DragableAssemblePart[] parts;

    private bool controlsEnabled = true;

    private int snapedParts = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;

        parts = FindObjectsOfType<DragableAssemblePart>();
    }

    private void Update()
    {
        if (!controlsEnabled)
            return;

        if(dragTarget)
        {
            Drag();

            if (Input.GetMouseButtonUp(0))
            {
                Drop();
            }
        }
    }

    private void CheckDragTargetError()
    {
        var dragPart = dragTarget.GetComponent<DragableAssemblePart>();

        float error = Vector3.Distance(dragPart.transform.position, dragPart.targetPos.position);

        if(error <= partsMinimumErrorForSnap)
        {
            dragPart.Snap(finalMoveDuration);

            dragTarget = null;

            snapedParts++;

            if (snapedParts == parts.Length)
                GameManager.instance.Win();
        }
    }

    public void PickUp(Transform target, Vector3 hitPoint)
    {
        dragTarget = target;
        offset = target.position - hitPoint;

        DOTween.Kill(dragTarget.gameObject.name);
    }

    private void Drag()
    {
        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Floor")))
        {
            dragTarget.position = Vector3.Lerp(dragTarget.position, hit.point + Vector3.up * height + offset, Time.deltaTime * followSpeed);

            CheckDragTargetError();
        }

    }

    private void Drop()
    {
        if (!dragTarget)
            return;

        dragTarget.DOLocalMoveY(0F, fallDownDuration).SetEase(Ease.InSine).SetId(dragTarget.gameObject.name);

        dragTarget = null;
    }

}
