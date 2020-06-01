using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private float cameraRotaitonLimit = 85f;



    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    private Vector3 thrusterForce = Vector3.zero;


    private Rigidbody rigidbody;

    //Get a force vector for our thrusters
    public void ApplyThruster(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }
    //gets a movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    //gets a rotational vector
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }
    //gets a rotational vector for the camera
    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }
    #region private methods
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    //run every physics iteration
    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    //perform movement based on velocity variable
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
        }
        if (thrusterForce != Vector3.zero)
        {
            rigidbody.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }
        //perform rotation
    void PerformRotation()
    {
            rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(rotation));
        if (camera != null)
        {
            //set rotation and clamp it 
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotaitonLimit, cameraRotaitonLimit);
            //apply our rotation to the transform of our camera
            camera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }
    #endregion private methods
}
