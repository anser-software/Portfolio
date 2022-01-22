using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sequencer : MonoBehaviour
{
    [SerializeField]
    private ActionEvent[] actions;

    public void StartSequence()
    {
        StartCoroutine(ExecuteSequence());
    }

    private IEnumerator ExecuteSequence()
    {
        foreach (var action in actions)
        {
            if(action.preDelay > 0F)
                yield return new WaitForSeconds(action.preDelay);

            ExecuteAction(action);

            if (action.postDelay > 0F)
                yield return new WaitForSeconds(action.postDelay);
        }

        Controller.Instance.CompletedSequence();
    }

    private void ExecuteAction(ActionEvent action)
    {
        var obj = action._object;// : GameObject.FindGameObjectWithTag(action._objectTag).transform;

        switch (action.action)
        {
            case ActionType.Move:
                if (action.local)
                {
                    if (action.moveBy)
                    {
                        var duration = action.duration;

                        if (action.useSpeed)
                        {
                            duration = Vector3.Distance(obj.localPosition, obj.localPosition + action.moveByVector) / action.speed;
                        }

                        obj.DOBlendableLocalMoveBy(action.moveByVector, duration).SetEase(action.movementCurve);
                    }
                    else
                    {
                        var duration = action.duration;

                        if (action.useSpeed)
                        {
                            duration = Vector3.Distance(obj.localPosition, action.targetPosition.localPosition) / action.speed;
                        }

                        obj.DOLocalMove(action.targetPosition.localPosition, duration).SetEase(action.movementCurve);
                    }
                }
                else
                {
                    if (action.moveBy)
                    {
                        var duration = action.duration;

                        if (action.useSpeed)
                        {
                            duration = Vector3.Distance(obj.position, obj.position + action.moveByVector) / action.speed;
                        }

                        obj.DOBlendableMoveBy(action.moveByVector, duration).SetEase(Ease.Linear).SetEase(action.movementCurve);
                    }
                    else
                    {
                        var duration = action.duration;

                        if (action.useSpeed)
                        {
                            duration = Vector3.Distance(obj.position, action.targetPosition.position) / action.speed;
                        }

                        obj.DOMove(action.targetPosition.position, duration).SetEase(Ease.Linear).SetEase(action.movementCurve);
                    }
                }
                break;
            case ActionType.Rotate:
                if (action.local)
                {
                    var duration = action.duration;

                    if (action.useSpeed)
                    {
                        duration = Vector3.Distance(obj.localRotation.eulerAngles, action.targetRotation.localRotation.eulerAngles) / action.speed;
                    }

                    obj.DOLocalRotateQuaternion(action.targetRotation.localRotation, duration).SetEase(action.movementCurve);
                }
                else
                {
                    var duration = action.duration;

                    if (action.useSpeed)
                    {
                        duration = Vector3.Distance(obj.rotation.eulerAngles, action.targetRotation.eulerAngles) / action.speed;
                    }

                    obj.DORotateQuaternion(action.targetRotation.rotation, duration).SetEase(action.movementCurve);
                }
                break;
            case ActionType.Scale:
                {
                    var duration = action.duration;

                    if (action.useSpeed)
                    {
                        duration = Vector3.Distance(obj.localScale, action.targetScale.localScale) / action.speed;
                    }

                    obj.DOScale(action.targetScale.localScale, duration).SetEase(action.movementCurve);
                }
                break;
            case ActionType.Event:
                action.eventToTrigger.Invoke();
                break;
        }
    }
}
