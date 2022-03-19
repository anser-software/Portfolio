using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float sensitivity, speed;

    float _yRot, _xRot, _scrollWheel;

    Transform rig;

    public bool isLocked = true;

    Quaternion initRot;

    void Awake()
    {
        rig = transform.parent;

        initRot = transform.localRotation;
    }

    public void ResetPos()
    {
        float maxTransVal = Mathf.Max(CellularAutomata.main.width, CellularAutomata.main.height);

        rig.position = new Vector3(maxTransVal * 0.5F, maxTransVal * 1.1F, -maxTransVal);
        rig.localRotation = Quaternion.identity;

        transform.localRotation = initRot;
    }

    void Update()
    {
        if (isLocked)
            return;

        _yRot = Input.GetAxis("Mouse X") * sensitivity;
        _xRot = Input.GetAxis("Mouse Y") * sensitivity;

        transform.rotation *= Quaternion.Euler(new Vector3(-_xRot, 0, 0));
        rig.rotation *= Quaternion.Euler(new Vector3(0, _yRot, 0));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            rig.position += ((transform.parent.forward * Input.GetAxis("Vertical")) +
                (transform.parent.right * Input.GetAxis("Horizontal"))) * speed * Time.deltaTime;
        }
        else
        {
            rig.position += ((transform.forward * Input.GetAxis("Vertical")) +
                (transform.right * Input.GetAxis("Horizontal"))) * speed * Time.deltaTime;
        }

        _scrollWheel = -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;

        if (_scrollWheel != 0)
        {
            rig.position += Vector3.up * _scrollWheel * speed * 5F;
        }
    }
}
