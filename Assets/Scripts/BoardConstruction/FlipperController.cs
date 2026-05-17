using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(HingeJoint2D))]
public sealed class FlipperController : MonoBehaviour
{
    [Serializable]
    public struct FlipperStateData
    {
        public FlipperMode mode;
        public GameObject visualObject;
    }
    
    public enum FlipperMode
    {
        Normal,
        Extended,
        SuperExtended,
        Short,
        Spiked
    }

    [Header("Main Settings")]
    [SerializeField] private string flipActionName;
    [SerializeField] private float hitStrength = 10000f;
    [SerializeField] private bool isReverted = false;

    [Header("States Configuration")]
    [SerializeField] private FlipperMode defaultMode = FlipperMode.Normal;
    [SerializeField] private List<FlipperStateData> states = new List<FlipperStateData>();

    private HingeJoint2D _hinge;
    private JointMotor2D _motor;
    private int _reversionMultiplier;
    private InputAction _flipAction;

    public FlipperMode CurrentMode { get; private set; }

    private void Start()
    {
        _hinge = GetComponent<HingeJoint2D>();
        _motor = _hinge.motor;
        _reversionMultiplier = isReverted ? -1 : 1;
        
        _flipAction = InputManager.Instance.GetAction(flipActionName);
        
        _motor.motorSpeed = -hitStrength * _reversionMultiplier;
        _hinge.motor = _motor;

        SetMode(defaultMode);
    }

    private void FixedUpdate()
    {
        if (_flipAction == null) return;
        
        bool isPressed = _flipAction.IsPressed();
        _motor.motorSpeed = (isPressed ? hitStrength : -hitStrength) * _reversionMultiplier;
        _hinge.motor = _motor;
    }

    public void SetMode(FlipperMode newMode)
    {
        CurrentMode = newMode;

        foreach (FlipperStateData state in states.Where(state => state.visualObject != null))
        {
            state.visualObject.SetActive(state.mode == newMode);
        }
    }
}