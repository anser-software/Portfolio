using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Controller : MonoBehaviour
{
    public static Controller instance { get; private set; }

    public ControlsType controlsType;

    public float coilingDuration;

    [SerializeField]
    private float dragSensitivity;

    private Tape tapeDragging;

    private Vector3 lastTouchWorldPos;

    private Camera mainCam;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;
    }

    public void DragLattice(Tape tape)
    {
        tapeDragging = tape;

        lastTouchWorldPos = GetWorldPos(Input.mousePosition);
    }

    private void LetGoLattice()
    {
        tapeDragging.CoilBack();
        tapeDragging = null;
    }

    private void CheckForTape()
    {
        var ray = mainCam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Dragable"), QueryTriggerInteraction.Collide))
        {
            var tape = hit.collider.GetComponent<Tape>();

            if(tape)
            {
                if(controlsType == ControlsType.Drag)
                    DragLattice(tape);
                else if(controlsType == ControlsType.Tap)
                {
                    tape.CoilUp();
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckForTape();
        }

        if(tapeDragging)
        {
            var touchWorldPos = GetWorldPos(Input.mousePosition);

            if (touchWorldPos == Vector3.zero)
                return;

            var movement = touchWorldPos - lastTouchWorldPos;

            tapeDragging.DragLattice(movement.magnitude * tapeDragging.GetLattice().up * Vector3.Dot(tapeDragging.GetLattice().up, movement.normalized) * dragSensitivity);

            lastTouchWorldPos = touchWorldPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            LetGoLattice();
            return;
        }
    }

    private Vector3 GetWorldPos(Vector3 screenPos)
    {
        var ray = mainCam.ScreenPointToRay(screenPos);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Floor"), QueryTriggerInteraction.Collide))
        {
            return hit.point;
        }
        else
            return Vector3.zero;
    }

}

public enum ControlsType
{
    Drag,
    Tap
}