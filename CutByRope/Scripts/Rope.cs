using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Obi;
public class Rope : MonoBehaviour
{

    [SerializeField]
    private ObiRope rope;

    [SerializeField]
    private Transform ropeCollider;

    [SerializeField]
    private Transform pin0, pin1;

    //[SerializeField]
    //private Material ropeMat;

    //[SerializeField]
    //private Gradient ropeColor;

    [SerializeField]
    private float maxDistance, catchUpSpeed, drag, acceleration, overshoot;

    [SerializeField]
    private float ropeTearDuration;

    private bool ropeTorn = false;

    private float ropeTearProgress = 0F;

    private float targetSpeed, currentSpeed;

    private Transform targetPin, draggedPin;

    private Vector3 targetPinPos;

    private void Start()
    {
        Controller.instance.OnRopeTorn += CutRope;
        Controller.instance.OnStartDragging += SetDragPin;

        if (pin0 == null)
            pin0 = Knife.instance.pin0;
        if (pin1 == null)
            pin1 = Knife.instance.pin1;

        targetPin = pin0;

        draggedPin = pin1;

        targetPinPos = draggedPin.position;
    }

    private void CutRope(Vector3 tearPoint)
    {
        ropeTorn = true;

        rope.Tear(rope.elements[rope.elements.Count / 2]);
        rope.RebuildConstraintsFromElements();

        DOTween.To(() => ropeTearProgress, x => ropeTearProgress = x, 1F, ropeTearDuration).SetEase(Ease.OutSine);
    }

    private void Update()
    {
        var sqrDistance = Vector3.SqrMagnitude(pin0.position - pin1.position);

        float targetPosDst = Vector3.Magnitude(targetPin.position - targetPinPos);

        if(sqrDistance > maxDistance * maxDistance)
        {
            targetSpeed = catchUpSpeed;

            var path = pin0.GetComponent<Pin>().path.path;

            if (path == pin1.GetComponent<Pin>().path.path)
            {
                var distanceAlongPath0 = path.GetClosestDistanceAlongPath(pin0.position);

                var distanceAlongPath1 = path.GetClosestDistanceAlongPath(pin1.position);

                if (draggedPin == pin0)
                {
                    var direction = distanceAlongPath0 > distanceAlongPath1 ?
                        (path.GetPointAtDistance(distanceAlongPath0 + 0.02F) - path.GetPointAtDistance(distanceAlongPath0)).normalized
                        : (path.GetPointAtDistance(distanceAlongPath1 + 0.02F) - path.GetPointAtDistance(distanceAlongPath1)).normalized;

                    targetPinPos = pin0.position + direction * overshoot;
                }
                else
                {
                    var direction = distanceAlongPath0 > distanceAlongPath1 ?
                        (path.GetPointAtDistance(distanceAlongPath0 + 0.02F) - path.GetPointAtDistance(distanceAlongPath0)).normalized
                        : (path.GetPointAtDistance(distanceAlongPath1 + 0.02F) - path.GetPointAtDistance(distanceAlongPath1)).normalized;

                    targetPinPos = pin1.position + direction * overshoot;
                }
            } else
            {
                if (draggedPin == pin0)
                {
                    var direction = pin0.GetComponent<Pin>().directionOfMotion;

                    targetPinPos = pin0.position + direction * overshoot;
                }
                else
                {
                    var direction = pin1.GetComponent<Pin>().directionOfMotion;

                    targetPinPos = pin1.position + direction * overshoot;
                }
            }

        }

        var pin = targetPin.GetComponent<Pin>();

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        targetPin.position = pin.path.path.GetClosestPointOnPath(targetPin.position + (targetPinPos - targetPin.position).normalized
            * currentSpeed * targetPosDst);

        targetSpeed -= targetSpeed * Time.deltaTime * drag;
        //ropeMat.color = ropeColor.Evaluate(Mathf.InverseLerp(0F, maxDistance * maxDistance, sqrDistance));
    }

    private void SetDragPin(Transform dragPin)
    {
        draggedPin = dragPin;

        if (draggedPin == pin0)
            targetPin = pin1;
        else
            targetPin = pin0;

        currentSpeed = 0F;
    }

    private void LateUpdate()
    {
        if (!ropeTorn)
        {
            ropeCollider.position = (pin0.position + pin1.position) * 0.5F;

            float dist = Vector3.Distance(pin0.position, pin1.position);

            var scale = ropeCollider.localScale;
            scale.z = dist;
            ropeCollider.localScale = scale;

            ropeCollider.transform.LookAt(pin0);
        } 
    }

}
