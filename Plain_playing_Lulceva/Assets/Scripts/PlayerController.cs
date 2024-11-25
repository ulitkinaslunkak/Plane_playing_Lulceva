using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerInputAsset.IPlayerInputActions
{
    [SerializeField] private PlaneController _planeController;
    PlayerInputAsset _playerInput;

    void Awake()
    {
        _playerInput = new PlayerInputAsset();

        _playerInput.PlayerInput.SetCallbacks(this);
    }

    void OnEnable()
    {
        _playerInput.PlayerInput.Enable();
    }

    void OnDisable()
    {
        _playerInput.PlayerInput.Disable();
    }

    public void OnThrottle(InputAction.CallbackContext context)
    {
        if (_planeController == null)
        {
            return;
        }

        _planeController.SetThrottleInput(context.ReadValue<float>());
    }
}
