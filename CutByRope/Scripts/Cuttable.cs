using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Cuttable : MonoBehaviour
{

    [SerializeField]
    private Rigidbody cutTarget;

    [SerializeField]
    private float minUpwardForce, maxUpwardForce, randomForceMaxAngle, minRandomForce, maxRandomForce, minTorque, maxTorque;

    [SerializeField]
    private Axis torqueAxis;

    [SerializeField]
    private GameObject cutEffect, coinPrefab;

    [SerializeField]
    private float scaleDownDelay, scaleDownDuration;

    private int passedCheckers, totalCheckers;

    private bool cut = false;

    private bool winWave = false;

    private void Start()
    {
        GameManager.instance.OnWin += ForceDestroyCutTargetIfCut;

        totalCheckers = transform.GetComponentsInChildren<CutChecker>().Length;
    }

    private void ForceDestroyCutTargetIfCut()
    {
        if (!cut)
            return;

        DOTween.Kill(cutTarget);

        if(cutTarget)
            DOTween.Sequence().SetDelay(1F).Append(cutTarget.transform.DOScale(Vector3.zero, scaleDownDuration).SetEase(Ease.InOutSine)).OnComplete(DestroyCutTarget);
    }

    public void WinWave(float height, float duration)
    {
        if (winWave)
            return;

        winWave = true;

        DOTween.Sequence().Append(transform.DOBlendableMoveBy(Vector3.up * height, duration * 0.5F).SetEase(Ease.OutSine))
            .Append(transform.DOBlendableMoveBy(Vector3.down * height, duration * 0.5F).SetEase(Ease.InSine));
    }

    public void CheckerPassed(Vector3 position)
    {
        passedCheckers++;

        if(passedCheckers >= totalCheckers)
        {
            cutTarget.isKinematic = false;
            Vector3 randomForce = Quaternion.AngleAxis(Random.Range(-randomForceMaxAngle, randomForceMaxAngle), Vector3.up) * (cutTarget.position - position).normalized
                * -Random.Range(minRandomForce, maxRandomForce);

            cutTarget.AddForce(Vector3.up * Random.Range(minUpwardForce, maxUpwardForce)
                + randomForce * Controller.instance.ropeSpeed, ForceMode.Impulse);

            Vector3 torqueDirection = Vector3.zero;

            switch (torqueAxis)
            {
                case Axis.X:
                    torqueDirection = Vector3.right;
                    break;
                case Axis.Y:
                    torqueDirection = Vector3.up;
                    break;
                default:
                    torqueDirection = Vector3.forward;
                    break;
            }

            cut = true;

            cutTarget.AddRelativeTorque(torqueDirection * Random.Range(minTorque, maxTorque), ForceMode.Impulse);

            if (GameManager.instance.challengeLevel && Random.Range(0F, 1F) < GameManager.instance.coinDropChance)
            {
                var coin = Instantiate(coinPrefab, cutTarget.transform.position, Quaternion.identity);

                coin.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0F, 360F));
            }

            if (cutEffect)
                Instantiate(cutEffect, cutTarget.transform.position, Quaternion.identity);

            Controller.instance.ObjectCut();

            DOTween.Sequence().SetDelay(scaleDownDelay).Append(cutTarget.transform.DOScale(Vector3.zero, scaleDownDuration).SetEase(Ease.InOutSine)).OnComplete(DestroyCutTarget);
        }
    }

    private void DestroyCutTarget()
    {
        Destroy(cutTarget.gameObject);
    }
}

public enum Axis
{
    X,Y,Z
}