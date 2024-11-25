using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    private Vector3 _currentVelocity;
    private Vector3 _currentLocalVelocity;
    private Vector3 _currentLocalAngularVelocity;

    private float _angleOfAttack;
    private float _angleOfAttackYaw;

    private Vector3 _localGForce;
    private Vector3 _lastVelocity;

    [SerializeField] private float _maxThrust;
    private float _throttle = 1;

    [SerializeField] private float _throttleSpeed;
    private float _throttleInput;

    private void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;

        CalculateState();
        CalculateGForces(deltaTime);

        UpdateThrust();
        UpdateThrottle(deltaTime);
    }

    private void CalculateState()
    {
        var invRotation = Quaternion.Inverse(_rigidbody.rotation);

        _currentVelocity = _rigidbody.velocity;
        _currentLocalVelocity = invRotation * _currentVelocity;
        _currentLocalAngularVelocity = invRotation * _rigidbody.angularVelocity;

        CalculateAngleOfAttack();
    }

    private void CalculateAngleOfAttack()
    {
        if (_currentLocalVelocity.sqrMagnitude <= 0.1f)
        {
            _angleOfAttack = 0;
            _angleOfAttackYaw = 0;

            return;
        }

        _angleOfAttack = Mathf.Atan2(-_currentLocalVelocity.y, _currentLocalVelocity.z);
        _angleOfAttackYaw = Mathf.Atan2(-_currentLocalVelocity.x, _currentLocalVelocity.z);

        Debug.Log($"Velocity : {_currentLocalVelocity}, AoA: {_angleOfAttack * Mathf.Rad2Deg}");
    }

    private void CalculateGForces(float deltaTime)
    {
        var invRotation = Quaternion.Inverse(_rigidbody.rotation);
        var acceleration = (_currentVelocity - _lastVelocity) / deltaTime;

        _localGForce = invRotation * acceleration;

        _lastVelocity = _currentVelocity;
    }

    private void UpdateThrust()
    {
        var thrust = _throttle * _maxThrust;
        _rigidbody.AddRelativeForce(thrust * Vector3.forward);
    }

    public void SetThrottleInput(float input)
    {
        _throttleInput = input;
    }

    public static class Utilities
    {
        public static float MoveTo(float value, float target, float speed, float deltaTime, float min = 0, float max = 1)
        {
            var diff = target - value;

            var delta = Mathf.Clamp(diff, -speed * deltaTime, speed * deltaTime);

            return Mathf.Clamp(value + delta, min, max);
        }
    }

    private void UpdateThrottle(float dt)
    {
        float target = 0;

        if (_throttleInput > 0)
        {
            target = 1;
        }

        _throttle = Utilities.MoveTo(_throttle, target, _throttleSpeed * Mathf.Abs(_throttleInput), dt);
    }

    
}
