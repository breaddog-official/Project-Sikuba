using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using Scripts.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Gameplay.Controllers
{
    public class ControllerPlayer : Controller
    {
        private AbillityMove abillityMove;
        private AbillityJump abillityJump;

        private bool isSubscribed;



        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);


            abillityMove = entity.FindAbillity<AbillityMove>();
            abillityJump = entity.FindAbillity<AbillityJump>();

            Subscribe();
        }

        [ClientCallback]
        private void Update()
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
        private void FixedUpdate()
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
        }

        [ClientCallback]
        private void Unsubscribe()
        {
            // We don't need to control an entity if it's not ours.
            if (!isSubscribed || !IsInitialized || Entity.isOwned == false)
                return;

            isSubscribed = false;


            InputManager.Controls.Game.Jump.performed -= JumpAction;
        }


        private void JumpAction(InputAction.CallbackContext ctx)
        {
            if (abillityJump.AvailableAndNotNull())
                abillityJump.Jump();
        }
    }
}
