using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(HingeJoint2D))]
public sealed class FlipperController : MonoBehaviour
{
    [SerializeField] private string flipActionName;
    [SerializeField] private float hitStrength = 10000f;
    [SerializeField] private bool isReverted = false;

    private HingeJoint2D _hinge;
    private JointMotor2D _motor;
    private int _reversionMultiplier;
    private InputAction _flipAction;

    private void Start()
    {
        _hinge = GetComponent<HingeJoint2D>();
        _motor = _hinge.motor;
        _reversionMultiplier = isReverted ? -1 : 1;
        _flipAction = InputManager.Instance.GetAction(flipActionName);
    }

    private void FixedUpdate()
    {
        if (_flipAction == null) return;
        bool isPressed = _flipAction.IsPressed();
        _motor.motorSpeed = (isPressed ? hitStrength : -hitStrength) * _reversionMultiplier;
        _hinge.motor = _motor;
    }
}