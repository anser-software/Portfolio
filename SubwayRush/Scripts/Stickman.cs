using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
[RequireComponent(typeof(Rigidbody))]
public class Stickman : MonoBehaviour
{

    public bool activatedElevator { get; private set; }

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float force, deceleration, slowDownDelay, walkVelocityAnim, escalatorMoveSpeed;

    [SerializeField]
    private int activationTriggerRadius;

    private Rigidbody rb;

    private Vector3 currentVelocity, targetVelocity;

    private float slowDownTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void SetVelocity(Vector3 velocity)
    {
        currentVelocity = targetVelocity = velocity;

        slowDownTimer = slowDownDelay;
    }

    private Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var stickman = collision.collider.GetComponent<Stickman>();

        if (stickman)
        {
            Vector3 collisionNormal = (transform.position - collision.collider.transform.position).normalized;

            //Vector3 crossProduct = Vector3.Cross(collisionNormal, Vector3.up);

            var collisionVel = stickman.GetVelocity();

            if(collisionVel.sqrMagnitude > rb.velocity.sqrMagnitude)
            {
                return;
            }

            Vector3 directionOfMotion = Vector3.Reflect(rb.velocity.normalized, collisionNormal);

            stickman.SetVelocity(rb.velocity.normalized * force);

            currentVelocity = targetVelocity = directionOfMotion * force;

            slowDownTimer = slowDownDelay;
        }
    }

    public IEnumerator ActivateEscalatorAscension()
    {
        if (activatedElevator)
            yield break;

        activatedElevator = true;

        MoveEscalator();

        var colliders = Physics.OverlapSphere(transform.position + Vector3.up, activationTriggerRadius);

        foreach (var col in colliders)
        {
            var otherStickman = col.GetComponent<Stickman>();

            if (otherStickman && !otherStickman.activatedElevator)
            {
                yield return null;
                StartCoroutine(otherStickman.ActivateEscalatorAscension());
            }
        }
    }

    private void MoveEscalator()
    {
        var points = Escalator.Instance.GetCheckPoints();

        var seq = DOTween.Sequence();

        for (int i = 0; i < points.Length; i++)
        {
            var prevPoint = i == 0 ? transform.position : points[i - 1];
            var targetPoint = points[i];

            var duration = Vector3.Distance(prevPoint, targetPoint) / escalatorMoveSpeed;

            seq.Append(transform.DOMove(targetPoint, duration).SetEase(Ease.Linear));
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = currentVelocity;

        slowDownTimer -= Time.deltaTime;

        if(slowDownTimer <= 0F && currentVelocity.sqrMagnitude > 0F)
            currentVelocity -= currentVelocity * Time.deltaTime * deceleration;
    }

    private void Update()
    {
        transform.forward = Vector3.Lerp(transform.forward, rb.velocity.normalized, Time.deltaTime * 5F);

        animator.SetBool("Run", rb.velocity.sqrMagnitude > walkVelocityAnim * walkVelocityAnim);
    }

}
