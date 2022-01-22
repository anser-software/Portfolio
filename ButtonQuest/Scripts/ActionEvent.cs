using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
[System.Serializable]
public class ActionEvent
{

    public ActionType action;

    //public ActionObjectType objectType;

    public Transform _object;

    //public string _objectTag;

    public bool moveBy;

    public Vector3 moveByVector;

    public Transform targetPosition, targetRotation, targetScale;

    public AnimationCurve movementCurve = AnimationCurve.Linear(0F, 1F, 0F, 1F);

    public bool local;

    public bool useSpeed;

    public float duration;

    public float speed;

    public UnityEvent eventToTrigger;

    public float preDelay, postDelay;

    public ActionEvent() { }

    public ActionEvent(ActionType action, Transform targetPosition, Transform targetRotation, Transform targetScale,
        bool local, float duration, UnityEvent eventToTrigger, float preDelay, float postDelay)
    {
        this.action = action;
        this.targetPosition = targetPosition;
        this.targetRotation = targetRotation;
        this.targetScale = targetScale;
        this.local = local;
        this.duration = duration;
        this.eventToTrigger = eventToTrigger;
        this.preDelay = preDelay;
        this.postDelay = postDelay;
    }
}

public enum ActionType
{
    Move,
    Rotate,
    Scale,
    Event
}

public enum ActionObjectType
{
    SpecificTransform,
    FindWithTag
}