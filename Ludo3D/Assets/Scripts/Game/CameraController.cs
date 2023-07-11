using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    
    float movementSpeed = 3f;
    float movementBtnSpeed = 0.1f;
    float mouseDragSpeed = 20f;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float speed = mouseDragSpeed * Time.deltaTime;
            transform.position += new Vector3(Input.GetAxis("Mouse X") * speed, 0, Input.GetAxis("Mouse Y") * speed);
        }

        //------------------Code for Zooming Out--------------------
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (transform.position.y < 45f)
            {
                float speed = movementSpeed * Time.deltaTime;
                transform.position += new Vector3(speed, speed * 30, speed * 30);
            }
        }

        //----------------Code for Zooming In-----------------------
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (transform.position.y > 12f)
            {
                float speed = movementSpeed * Time.deltaTime;
                transform.position -= new Vector3(speed, speed * 30, speed * 30);
            }
        }
    }

    public void ZoomInBtnClick()
    {
        if (camera.transform.position.y > 12f)
        {
            camera.transform.position -= new Vector3(movementBtnSpeed, movementBtnSpeed * 30, movementBtnSpeed * 30);
        }
    }

    public void ZoomOutBtnClick()
    {
        if (camera.transform.position.y < 45f)
        {
            camera.transform.position += new Vector3(movementBtnSpeed, movementBtnSpeed * 30, movementBtnSpeed * 30);
        }
    }
}
