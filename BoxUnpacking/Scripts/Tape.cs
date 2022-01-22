using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;

public class Tape : MonoBehaviour
{

    public float MaxLatticeLocalY { get { return maxLatticeLocalY; } }
    public float InitialLatticeLocalY { get { return initialLatticeLocalY; } }

    public System.Action OnCoilUp;

    [SerializeField]
    private Transform lattice;

    [SerializeField]
    private float maxLatticeLocalY;

    [SerializeField]
    private IntersectionPointChoke[] intersectionChokePoints;

    [SerializeField]
    private IntersectionRelease[] intersectionReleasePoint;

    private float initialLatticeLocalY;

    private float lastLatticeLocalY;

    private bool dragable = true;

    private bool inAnim = false;

    private void Start()
    {
        lastLatticeLocalY = initialLatticeLocalY = lattice.localPosition.y;
    }

    public Transform GetLattice()
    {
        return lattice;
    }

    public void SetChokePointEnabled(int id, bool enabled)
    {
        if (id >= intersectionChokePoints.Length)
            return;

        Debug.Log("Set point " + id + " to " + enabled.ToString());

        var index = intersectionChokePoints.ToList().IndexOf(intersectionChokePoints.First(p => p.id == id));

        var p = intersectionChokePoints[index];

        p.enabled = enabled;

        intersectionChokePoints[index] = p;

        //CoilUp(1F);
    }

    public void CoilBack()
    {
        if (!dragable || inAnim)
            return;

        inAnim = true;

        lattice.DOLocalMoveY(initialLatticeLocalY, Controller.instance.coilingDuration).SetEase(Ease.InOutSine).OnComplete(() => inAnim = false);
    }

    public bool CoilUp()
    {
        if (!dragable || inAnim)
            return false;

        bool allChokePointsAheadDisabled = true;

        foreach (var chokePoint in intersectionChokePoints)
        {
            if (chokePoint.chokeLatticeLocalY > lattice.localPosition.y && chokePoint.enabled)
            {
                allChokePointsAheadDisabled = false;
                break;
            }
        }

        if (!allChokePointsAheadDisabled)
            return false;

        foreach (var releasePoint in intersectionReleasePoint)
        {
            foreach (var tape in releasePoint.callOnRelease)
            {
                tape.SetChokePointEnabled(releasePoint.chokePointID, false);
            }
        }

        inAnim = true;

        lattice.DOLocalMoveY(maxLatticeLocalY, Controller.instance.coilingDuration).SetEase(Ease.InOutSine).OnComplete(() => inAnim = false);

        dragable = false;

        OnCoilUp?.Invoke();

        return true;
    }

    public void DragLattice(Vector3 translation)
    {
        if (!dragable)
            return;

        lattice.position += translation;

        if(lattice.localPosition.y < initialLatticeLocalY || lattice.localPosition.y > maxLatticeLocalY)
        {
            lattice.position -= translation;
        } else
        {
            foreach (var chokePoint in intersectionChokePoints)
            {
                if(chokePoint.enabled && lattice.localPosition.y >= chokePoint.chokeLatticeLocalY)
                {
                    lattice.position -= translation;
                    break;
                }
            }
            foreach (var releasePoint in intersectionReleasePoint)
            {
                if(lattice.localPosition.y > lastLatticeLocalY)
                {
                    if(lattice.localPosition.y > releasePoint.releaseLatticeLocalY)
                    {
                        foreach (var tape in releasePoint.callOnRelease)
                        {
                            tape.SetChokePointEnabled(releasePoint.chokePointID, false);
                        }
                    }
                } else if(lattice.localPosition.y < lastLatticeLocalY)
                {
                    if (lattice.localPosition.y < releasePoint.releaseLatticeLocalY)
                    {
                        foreach (var tape in releasePoint.callOnRelease)
                        {
                            tape.SetChokePointEnabled(releasePoint.chokePointID, true);
                        }
                    }
                }
            }
        }

        lastLatticeLocalY = lattice.localPosition.y;

        if (Mathf.Abs(initialLatticeLocalY - lastLatticeLocalY) > 1F)
            CoilUp();
    }
}
