using UnityEngine;
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [Header("Basic settings")]
    [SerializeField]
    private float speed = 3.5f;
    [SerializeField]
    private float lookSensitivity = 5f;
    [SerializeField]
    private float thrusterForce = 1000f;

    [Header("Spring settings")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;


    private PlayerMotor motor;
    private ConfigurableJoint joint;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();

        SetJointSettings(jointSpring);
    }

    void Update()
    {
        //Calculate movement velocity as a 3D vector
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;
        //final movement vector combined
        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed;
        //apply movement
        motor.Move(_velocity);

        //calculate rotation as a 3D vector
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _rotation = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        //Apply rotation
        motor.Rotate(_rotation);

        //calculate rotation as a 3D vector
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;//new Vector3(_xRotation, 0f, 0f) * lookSensitivity;

        //Apply rotation
        motor.RotateCamera(_cameraRotationX);

        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump"))
        {
            _thrusterForce = Vector3.up * thrusterForce;
            //kikapcs visszaugrás/húzás
            SetJointSettings(0f);
        }
        else
        {
            SetJointSettings(jointSpring);
        }
        //apply thruster force
        motor.ApplyThruster(_thrusterForce);

    }
    private void SetJointSettings( float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
