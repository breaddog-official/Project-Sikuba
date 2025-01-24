using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using Scripts.Input;
using Scripts.MonoCache;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Gameplay.Controllers
{
    public class ControllerPlayer : Controller, IMonoCacheUpdate, IMonoCacheFixedUpdate
    {
        private AbillityMove abillityMove;
        private AbillityJump abillityJump;
        private AbillityRotater abillityRotater;
        private AbillityItemSocket abillityItemSocket;
        private AbillityCamera abillityCamera;

        private bool isSubscribed;


        public Behaviour Behaviour => this;


        public override bool Initialize(Entity entity)
        {
            base.Initialize(entity);

            MonoCacher.Registrate(this);

            abillityMove = entity.FindAbillity<AbillityMove>();
            abillityJump = entity.FindAbillity<AbillityJump>();
            abillityRotater = entity.FindAbillity<AbillityRotater>();
            abillityItemSocket = entity.FindAbillity<AbillityItemSocket>();
            abillityCamera = entity.FindAbillity<AbillityCamera>();

            Subscribe();

            return true;
        }

        [ClientCallback]
        public void UpdateCached()
        {
            // We don't need to control an entity if it's not ours
            if (!IsInitialized || Entity.isOwned == false)
                return;

            // Invokes non physics movement
            if (abillityMove.AvailableAndNotNull() && abillityMove.IsPhysicsMovement() == false)
            {
                MoveAction();
            }

            if (abillityRotater.AvailableAndNotNull() && abillityRotater.IsPhysicsRotater() == false)
            {
                RotateAction();
            }
        }

        [ClientCallback]
        public void FixedUpdateCached()
        {
            // We don't need to control an entity if it's not ours.
            if (!IsInitialized || Entity.isOwned == false)
                return;

            Subscribe();

            // Invokes physics movement
            if (abillityMove.AvailableAndNotNull() && abillityMove.IsPhysicsMovement() == true)
            {
                MoveAction();
            }

            if (abillityRotater.AvailableAndNotNull() && abillityRotater.IsPhysicsRotater() == true)
            {
                RotateAction();
            }
        }


        private void OnEnable() => Subscribe();
        private void OnDisable() => Unsubscribe();


        [ClientCallback]
        private void Subscribe()
        {
            // We don't need to control an entity if it's not ours.
            if (isSubscribed || !IsInitialized || Entity.isOwned == false)
                return;

            isSubscribed = true;


            InputManager.Controls.Game.Move.started += StartMoveAction;
            InputManager.Controls.Game.Move.canceled += StopMoveAction;
            InputManager.Controls.Game.Jump.performed += JumpAction;
            InputManager.Controls.Game.Fire.started += StartUsingAction;
            InputManager.Controls.Game.Fire.canceled += StopUsingAction;
        }

        [ClientCallback]
        private void Unsubscribe()
        {
            // We don't need to control an entity if it's not ours.
            if (!isSubscribed || !IsInitialized || Entity.isOwned == false)
                return;

            isSubscribed = false;


            InputManager.Controls.Game.Move.started -= StartMoveAction;
            InputManager.Controls.Game.Move.canceled -= StopMoveAction;
            InputManager.Controls.Game.Jump.performed -= JumpAction;
            InputManager.Controls.Game.Fire.started -= StartUsingAction;
            InputManager.Controls.Game.Fire.canceled -= StopUsingAction;
        }




        #region Actions

        private void JumpAction(InputAction.CallbackContext ctx = default)
        {
            if (abillityJump.AvailableAndNotNull())
                abillityJump.Jump();
        }

        private void StartUsingAction(InputAction.CallbackContext ctx = default)
        {
            if (abillityItemSocket.AvailableAndNotNull() && abillityItemSocket.HasItem())
                abillityItemSocket.EquippedItem.StartUsing();
        }

        private void StopUsingAction(InputAction.CallbackContext ctx = default)
        {
            if (abillityItemSocket.AvailableAndNotNull() && abillityItemSocket.HasItem())
                abillityItemSocket.EquippedItem.StopUsing();
        }

        private void MoveAction(InputAction.CallbackContext ctx = default)
        {
            abillityMove.Move(InputManager.Controls.Game.Move.ReadValue<Vector2>().ConvertInputToVector3());
        }

        private void StartMoveAction(InputAction.CallbackContext ctx = default)
        {
            abillityMove.StartMove();
        }

        private void StopMoveAction(InputAction.CallbackContext ctx = default)
        {
            abillityMove.StopMove();
        }

        private void RotateAction(InputAction.CallbackContext ctx = default)
        {
            InputAction look = InputManager.Controls.Game.Look;
            InputAction lookPosition = InputManager.Controls.Game.LookPosition;

            if (look.ReadValue<Vector2>().sqrMagnitude > 0)
            {
                abillityRotater.Rotate(look.ReadValue<Vector2>().ConvertInputToVector3());
            }
            else
            {
                abillityRotater.RotateToPoint(lookPosition.ReadValue<Vector2>());
            }
        }

        #endregion
    }
}
