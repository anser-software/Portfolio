using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using DG.Tweening;
public class Controller : MonoBehaviour
{
    public static Controller instance { get; private set; }

    public float ropeSpeed { get; private set; }

    public System.Action<Vector3> OnRopeTorn;

    public System.Action<Transform> OnStartDragging;

    [SerializeField]
    private PathCreator[] paths;

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float maxMovementMagnitude;

    [SerializeField]
    private float maxOneFrameMoveMagnituge;

    [SerializeField]
    private bool forceCheckPinJump;

    [SerializeField]
    private float disableControlsAfterWinDelay;

    [SerializeField]
    private bool playSound;

    [SerializeField]
    private float minPitch;

    [SerializeField]
    private float maxPitch;

    [SerializeField]
    private bool addPitchMode;

    [SerializeField]
    private float addPitchStep;//, addPitchResetTime;

    [SerializeField]
    private AudioSource cutSFX;

    [SerializeField]
    private float cutSFXCooldown;

    [SerializeField]
    private float hapticCooldown;

    [SerializeField] 
    private bool useHaptic;

    private Transform dragged = null;

    private Camera mainCam;

    private bool enabledControls = true;

    private Vector3 difference;

    private int objectsCut, totalObjectsToCut;

    private Vector3 draggedPosLastFrame;

    private bool canPlayCutSFX = true;

    private bool canPlayHaptic = true;

    private float currentPitch = 1F;

    private int moveCount = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;

        totalObjectsToCut = FindObjectsOfType<Cuttable>().Length;

        GameManager.instance.OnWin += Finish;
        GameManager.instance.OnLose += Lose;
    }

    private void Lose()
    {
        DisableControls();
    }

    private void OnDestroy()
    {
        GameManager.instance.OnWin -= Finish;
        GameManager.instance.OnLose -= Lose;
    }

    public void ObjectCut()
    {
        objectsCut++;

        if (canPlayCutSFX && playSound)
        {
            if(addPitchMode)
            {
                cutSFX.pitch = Mathf.Clamp(currentPitch, 1F, 2F);

                currentPitch += addPitchStep;
            } else 
                cutSFX.pitch = Random.Range(minPitch, maxPitch);

            cutSFX.Play();
            canPlayCutSFX = false;
            DOTween.Sequence().SetDelay(cutSFXCooldown).OnComplete(EnableCutSFX);
        }

        if(useHaptic && canPlayHaptic)
        {
            Taptic.Light();
            canPlayHaptic = false;
            DOTween.Sequence().SetDelay(hapticCooldown).OnComplete(EnableHaptic);
        }

        if (objectsCut >= totalObjectsToCut)
            GameManager.instance.Win();
    }

    private void EnableCutSFX()
    {
        canPlayCutSFX = true;
    }

    private void EnableHaptic()
    {
        canPlayHaptic = true;
    }

    public void TearRope(Vector3 tearPoint)
    {
        OnRopeTorn?.Invoke(tearPoint);

        GameManager.instance.Lose();
    }

    private void Finish()
    {
        DOTween.Sequence().SetDelay(disableControlsAfterWinDelay).OnComplete(DisableControls);
    }

    private void DisableControls()
    {
        enabledControls = false;

        LetGoDragged();
    }

    private void Update()
    {
        if (!enabledControls)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Dragable"), QueryTriggerInteraction.Collide))
            {
                dragged = hit.collider.transform;

                var pin = dragged.GetComponent<Pin>();

                if(pin.locked)
                {
                    dragged = null;
                    return;
                }

                OnStartDragging?.Invoke(dragged);

                if (GameManager.instance.challengeLevel)
                    moveCount++;

                draggedPosLastFrame = dragged.transform.position;

                if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Floor")))
                {
                    var point = hit.point;
                    point.y = dragged.position.y;

                    difference = point - dragged.position;
                }
                //dragged.GetComponent<Ball>().Drag();
            }
        }

        if (dragged)
        {
            MoveDragged();
            CalculateRopeSpeed();
        }

        if (Input.GetMouseButtonUp(0))
        {
            LetGoDragged();
        }
    }

    public void LetGoDragged()
    {
        Debug.Log("HERE");
        currentPitch = 1F;
        if (dragged)
        {
            //dragged.GetComponent<Ball>().StopDragging();
            if (GameManager.instance.challengeLevel)
                dragged.GetComponent<Pin>().Lock();

            if(moveCount >= 2)
            {
                if (GameManager.instance.gameStatus == GameStatus.Playing)
                    GameManager.instance.Lose();
            }


            dragged = null;
        }
    }

    private void MoveDragged()
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Floor")))
        {
            if (forceCheckPinJump && (hit.point - dragged.position).sqrMagnitude > 10F)
                return;

            var point = hit.point - difference;
            point.y = dragged.position.y;

            PathCreator path;
            var targetPos = Vector3.Lerp(dragged.position, point, Time.deltaTime * movementSpeed);
            var deltaPos = targetPos - dragged.position;
            var pin = dragged.GetComponent<Pin>();

            var finalPos = pin.path.path.GetClosestPointOnPath(dragged.position + Vector3.ClampMagnitude(deltaPos, maxMovementMagnitude));

            if(forceCheckPinJump && (dragged.position - finalPos).sqrMagnitude > maxMovementMagnitude * maxMovementMagnitude)
            {
                return;
            }

            dragged.position = finalPos;
        }
    }

    public bool isDragged(Transform t)
    {
        if (dragged == null || dragged != t)
            return false;
        else 
            return true;
    }

    private void CalculateRopeSpeed()
    {
        ropeSpeed = (dragged.transform.position - draggedPosLastFrame).magnitude;

        draggedPosLastFrame = dragged.transform.position;
    }

    public Vector3 GetClosestPointToAnyPath(Vector3 pos, out PathCreator closestPath)
    {
        var targetPoint = Vector3.one * 10000F;

        closestPath = paths[0];

        foreach (var path in paths)
        {
            var currentPoint = path.path.GetClosestPointOnPath(pos);

            if ((currentPoint - pos).sqrMagnitude < (targetPoint - pos).sqrMagnitude)
            {
                targetPoint = currentPoint;
                closestPath = path;
            }
        }

        return targetPoint;
    }

    private Vector3 GetCurrentMouseViewportPos()
    {
        return new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
    }


}
