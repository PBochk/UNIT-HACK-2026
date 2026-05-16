using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(HingeJoint2D))]
public sealed class Flipper : MonoBehaviour
{
    [SerializeField] private InputActionReference flipActionReference;
    [SerializeField] private float hitStrength = 10000f;
    [SerializeField] private bool isReverted = false;

    private HingeJoint2D _hinge;
    private JointMotor2D _motor;
    private int _reversionMultiplier;

    private void Start()
    {
        _hinge = GetComponent<HingeJoint2D>();
        _motor = _hinge.motor;
        _reversionMultiplier = isReverted ? -1 : 1;
    }
    
    private void OnEnable()
    {
        if (flipActionReference != null)
        {
            flipActionReference.action.Enable();
        }
    }
    
    private void OnDisable()
    {
        if (flipActionReference != null)
        {
            flipActionReference.action.Disable();
        }
    }

    private void FixedUpdate()
    {
        bool isPressed = flipActionReference.action.IsPressed();
        _motor.motorSpeed = (isPressed ? hitStrength : -hitStrength)*_reversionMultiplier;
        _hinge.motor = _motor;
    }
}