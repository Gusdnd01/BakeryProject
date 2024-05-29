using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerInput
{
    Jump,
    Dash,
    PrimaryAttack,
    SwordAura
}

[CreateAssetMenu(menuName = "SO/InputReader")]
public class InputReader : ScriptableObject, Controls.IPlayerActions
{
    private Dictionary<string, InputBuffer> _bufferDic = new Dictionary<string, InputBuffer>();

    public float XInput { get; private set; }
    public float YInput { get; private set; }

    public event Action JumpEvent;
    public event Action DashEvent;
    public event Action PrimaryAttackEvent;
    public event Action SwordAuraEvent;

    public Controls _controls;
    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }
        _controls.Player.Enable();
    }

    public void UpdateBuffer()
    {
        foreach (InputBuffer buffer in _bufferDic.Values)
        {
            if (buffer.CheckIfValid(out PlayerInput type))
            {
                Action action = null;
                switch (type)
                {
                    case PlayerInput.Jump:
                        action = JumpEvent;
                        break;
                    case PlayerInput.Dash:
                        action = DashEvent;
                        break;
                    case PlayerInput.PrimaryAttack:
                        action = PrimaryAttackEvent;
                        break;
                    case PlayerInput.SwordAura:
                        action = SwordAuraEvent;
                        break;
                }

                if (action != null && buffer.Trigger)
                {
                    action();
                    return;
                }
            }
        }
    }
    private void Input(string actionName)
    {
        if (!_bufferDic.ContainsKey(actionName))
        {
            _bufferDic.Add(actionName, new InputBuffer());
        }
        _bufferDic[actionName].GetInput(Time.time, Enum.Parse<PlayerInput>(actionName));
    }

    #region ActionTypeButton
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Input(context.action.name);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Input(context.action.name);
        }
    }

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Input(context.action.name);
        }
    }

    public void OnSwordAura(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Input(context.action.name);
        }
    }
    #endregion
    #region ActionTypeValue
    public void OnXMovement(InputAction.CallbackContext context)
    {
        XInput = context.ReadValue<float>();
    }

    public void OnYMovement(InputAction.CallbackContext context)
    {
        YInput = context.ReadValue<float>();
    }
    #endregion


}