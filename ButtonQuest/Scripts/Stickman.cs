using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
[RequireComponent(typeof(Rigidbody))]
public class Stickman : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float force, deceleration, slowDownDelay, walkVelocityAnim;

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
