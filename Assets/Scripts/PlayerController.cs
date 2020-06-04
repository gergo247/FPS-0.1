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
    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;

    public float GetThrusterAmount()
    {
        return thrusterFuelAmount;
    }
    [SerializeField]
    private LayerMask envitonmentMask;

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
        //setting target position for spring, this makes the physics act right when it comes to
        //applying gravity when flying over objects
        RaycastHit _hit;
        if (Physics.Raycast(transform.position, Vector3.down,out _hit,100f, envitonmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);

        }

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
        //calculate the thrusterforce based on player input
        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;
            if (thrusterFuelAmount >= 0.01f)
                _thrusterForce = Vector3.up * thrusterForce;
            //kikapcs visszaugrás/húzás
            SetJointSettings(0f);
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

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
