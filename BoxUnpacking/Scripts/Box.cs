using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using PathCreation;

public class Box : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject confetti;

    [SerializeField]
    private float playAnimDelay, winCallDelay;

    [SerializeField]
    private GameObject[] objectsToScaleDown;

    [SerializeField]
    private float scaleDuration, scaleInterval;

    [SerializeField]
    private WinPart[] winParts;

    [SerializeField]
    private PathCreator path;

    [SerializeField]
    private float partSpeed, delayBeforePath, delayAfterPath, finalPartMoveDuration;

    private float distanceTravelled;

    private bool partFollowPath;

    private void Start()
    {
        GameManager.instance.OnWinTapesCoiledUp += PlayWinAnim;

        var currentPartIndex = PlayerPrefs.GetInt("CurrentPart", 0);

        for (int i = 0; i < winParts.Length; i++)
        {
            if (i == currentPartIndex)
            {
                winParts[i].partInBox.SetActive(true);
            }
            if(i < currentPartIndex)
            {
                winParts[i].partOnObject.GetComponent<MeshRenderer>().sharedMaterial = winParts[i].partMat;
            }
        }
    }

    private void PlayWinAnim()
    {
        var finishSequence = DOTween.Sequence();

        finishSequence.AppendInterval(playAnimDelay);

        var rnd = new System.Random();
        var shuffledObjectsToScaleDown = objectsToScaleDown.OrderBy(item => rnd.Next()).ToArray();

        foreach (var obj in shuffledObjectsToScaleDown)
        {
            finishSequence.AppendInterval(scaleInterval);
            finishSequence.Join(obj.transform.DOScale(0F, scaleDuration));
            finishSequence.AppendCallback(() => Destroy(obj));
        }

        finishSequence.AppendCallback(() => animator.enabled = true);

        finishSequence.AppendInterval(winCallDelay);

        finishSequence.OnComplete(() =>
        {
            SetPart();
        });
    }

    void Update()
    {
        if (!partFollowPath)
            return;

        distanceTravelled += partSpeed * Time.deltaTime;
        winParts[PlayerPrefs.GetInt("CurrentPart", 0)].partInBox.transform.position = path.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
        //winParts[PlayerPrefs.GetInt("CurrentPart", 0)].partInBox.transform.rotation = path.path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction.Stop);

        if(distanceTravelled > path.path.length)
        {
            partFollowPath = false;

            DOTween.Sequence().SetDelay(delayAfterPath).OnComplete(() => 
            {
                winParts[PlayerPrefs.GetInt("CurrentPart", 0)].partInBox.transform
                .DOMove(winParts[PlayerPrefs.GetInt("CurrentPart", 0)].partOnObject.transform.position, finalPartMoveDuration).OnComplete(() =>
                {
                    PlayerPrefs.SetInt("CurrentPart", PlayerPrefs.GetInt("CurrentPart", 0) + 1);

                    if (PlayerPrefs.GetInt("CurrentPart") >= winParts.Length)
                    {
                        PlayerPrefs.SetInt("CurrentPart", 0);

                        GameManager.instance.LoadAssemble();

                        return;
                    }

                    GameManager.instance.Win();
                    confetti.SetActive(true);
                });

            });
        }
    }

    private void SetPart()
    {
        DOTween.Sequence().SetDelay(delayBeforePath).OnComplete(() => partFollowPath = true);      
    }

}

[System.Serializable]
public class WinPart
{
    public GameObject partOnObject;

    public GameObject partInBox;

    public Material partMat;
}