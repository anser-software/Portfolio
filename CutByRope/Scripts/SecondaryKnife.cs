using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SecondaryKnife : MonoBehaviour
{

    [SerializeField]
    private Knife primaryKnife;

    [SerializeField] [Range(0F,1F)]
    private float followSpeed;

    [SerializeField]
    private float followTimeBetweenSnapshots;

    [SerializeField]
    private float followDelay;

    [SerializeField]
    private GameObject painter;

    private Queue<Vector3> pin0Poses = new Queue<Vector3>(), pin1Poses = new Queue<Vector3>();

    private Transform pin0, pin1;

    private Vector3 offset;

    private bool broken = false;

    private Vector3 pos0, pos1, targetPos0, targetPos1;

    private float timer = 0F;

    private float totalWaitTimer = 0F;

    private void Start()
    {
        pin0 = primaryKnife.pin0;
        pin1 = primaryKnife.pin1;
        offset = primaryKnife.offset;

        transform.parent = null;

        pos0 = pin0.position;
        pos1 = pin1.position;

        targetPos0 = pos0;
        targetPos1 = pos1;

        transform.position = pos0 + offset;
        transform.rotation = Quaternion.LookRotation((pos0 - pos1).normalized, Vector3.up);

        var rot = transform.rotation.eulerAngles;
        rot.x = -90F;
        transform.rotation = Quaternion.Euler(rot);

        var scale = transform.localScale;
        scale.y = Vector3.Distance(pos0, pos1);
        transform.localScale = scale;

        DOTween.Sequence().SetDelay(0.1F).OnComplete(EnablePainter);
    }

    private void EnablePainter()
    {
        painter.SetActive(true);
    }

    private void LateUpdate()
    {
        if (broken)
            return;

        if (totalWaitTimer < followDelay)
        {
            totalWaitTimer += Time.deltaTime;
        }

        timer += Time.deltaTime;

        if (timer > followTimeBetweenSnapshots)
        {
            timer = 0F;
            UpdatePinPoses();
        }

        if (totalWaitTimer >= followDelay)
        {
            pos0 = Vector3.Lerp(pos0, targetPos0, followSpeed);
            pos1 = Vector3.Lerp(pos1, targetPos1, followSpeed);

            transform.position = pos0 + offset;
            transform.rotation = Quaternion.LookRotation((pos0 - pos1).normalized, Vector3.up);
            var rot = transform.rotation.eulerAngles;
            rot.x = -90F;
            transform.rotation = Quaternion.Euler(rot);

            //set scale

            var scale = transform.localScale;
            scale.y = Vector3.Distance(pos0, pos1);
            transform.localScale = scale;
        }
    }

    private void UpdatePinPoses()
    {
        pin0Poses.Enqueue(pin0.position);
        pin1Poses.Enqueue(pin1.position);

        if(totalWaitTimer >= followDelay)
        {
            targetPos0 = pin0Poses.Dequeue();
            targetPos1 = pin1Poses.Dequeue();
        }
    }

}
