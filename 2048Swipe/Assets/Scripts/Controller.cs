using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

public class Controller : MonoBehaviour
{

    public static Controller instance { get; private set; }

    [SerializeField]
    private float minSwipeLength;

    public float timeThreshold = 0.3f;

    public float blockMoveSpeed;

    public float blockScaleSpeed;

    private Vector2 fingerDown;
    private float fingerDownTime;
    private Vector2 fingerUp;
    private float fingerUpTime;

    public Block currentBlock;

    [HideInInspector]
    public bool enabledControls = true;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!enabledControls)
            return;

        if(Input.GetMouseButtonDown(0))
        {
            fingerDown.x = Input.mousePosition.x / Screen.width;
            fingerDown.y = Input.mousePosition.y / Screen.height;
            fingerDownTime = Time.time;
        }
        if(Input.GetMouseButtonUp(0))
        {
            fingerUp = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            fingerUpTime = Time.time;
            float swipeLengthSqr = Vector2.SqrMagnitude(fingerUp - fingerDown);

            float duration = fingerUpTime - fingerDownTime;

            if (duration > timeThreshold) return;

            if (swipeLengthSqr < minSwipeLength * minSwipeLength)
                return;

            TrySwipe(GetSwipeDirection());
        }
    }

    private Vector3 GetSwipeDirection()
    {
        float deltaX = fingerDown.x - fingerUp.x;

        bool right = false;

        if (deltaX > 0)
            right = true;
        

        float deltaY = fingerDown.y - fingerUp.y;

        bool up = false;

        if (deltaY > 0)       
            up = true;
        
        if(Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            if (right)
                return Vector3.left;
            else
                return Vector3.right;
        } else
        {
            if (up)
                return Vector3.back;
            else
                return Vector3.forward;
        }
    }

    private void TrySwipe(Vector3 swipeDirection)
    {
        Debug.Log(swipeDirection);
        RaycastHit hit;

        if (Physics.Raycast(currentBlock.transform.position, swipeDirection, out hit, Mathf.Infinity, LayerMask.GetMask("Wall", "Block")))
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                Swipe(hit.point - swipeDirection * currentBlock.transform.localScale.x * 0.5F, false);
            else
            {
                Block hitBlock = hit.collider.GetComponent<Block>();
                if (hitBlock)
                {
                    bool merge = hitBlock.TryMerge(currentBlock);
                    if (merge)
                    {
                        Swipe(hitBlock.transform.position, merge);
                    }
                    else
                    {
                        Swipe(hit.point - swipeDirection * currentBlock.transform.localScale.x * 0.5F, merge);
                    }
                }
            }
        }
    }

    Block lastBlock;

    private void Swipe(Vector3 finalPoint, bool merge)
    {
        if (!merge && (currentBlock.transform.position - finalPoint).sqrMagnitude < 1F)
            return;

        enabledControls = false;
        lastBlock = currentBlock;
        System.Action<ITween<Vector3>> updateCurrentBlockPos = (t) =>
        {
            if (t.CurrentProgress >= 1F)
                enabledControls = true;

            lastBlock.transform.position = t.CurrentValue;
        };

        TweenFactory.Tween(null, currentBlock.transform.position, finalPoint, Vector3.Distance(currentBlock.transform.position, finalPoint) / blockMoveSpeed, TweenScaleFunctions.SineEaseOut, updateCurrentBlockPos);
    }

}
