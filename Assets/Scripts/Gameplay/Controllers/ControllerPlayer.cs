using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using Scripts.Input;
using Scripts.MonoCacher;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Gameplay.Controllers
{
    public class ControllerPlayer : Controller, IMonoCacheUpdate, IMonoCacheFixedUpdate
    {
        private AbillityMove abillityMove;
        private AbillityJump abillityJump;
        private AbillityItemSocket abillityItemSocket;

        private bool isSubscribed;


        public Behaviour Behaviour => this;


        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);

            MonoCacher.MonoCacher.Registrate(this);

            abillityMove = entity.FindAbillity<AbillityMove>();
            abillityJump = entity.FindAbillity<AbillityJump>();
            abillityItemSocket = entity.FindAbillity<AbillityItemSocket>();

            Subscribe();
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
                abillityMove.Move(InputManager.Controls.Game.Move.ReadValue<Vector2>().ConvertInputToVector3());
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
                abillityMove.Move(InputManager.Controls.Game.Move.ReadValue<Vector2>().ConvertInputToVector3());
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


            InputManager.Controls.Game.Jump.performed -= JumpAction;
            InputManager.Controls.Game.Fire.started -= StartUsingAction;
            InputManager.Controls.Game.Fire.canceled -= StopUsingAction;
        }




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
    }
}
