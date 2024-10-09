using Scripts.Extensions;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using Scripts.Input;
using UnityEngine;

namespace Scripts.Gameplay.Controllers
{
    public class ControllerPlayer : Controller
    {
        private AbillityMove abillityMove;

        public override void Initialize(Entity entity)
        {
            abillityMove = entity.FindAbillity<AbillityMove>();
        }

        private void Update()
        {
            // Invokes non physics movement
            if (abillityMove.AvailableAndNotNull() && abillityMove.IsPhysicsMovement() == false)
            {
                abillityMove.Move(InputManager.Controls.Game.Move.ReadValue<Vector3>());
            }
        }

        private void FixedUpdate()
        {
            // Invokes physics movement
            if (abillityMove.AvailableAndNotNull() && abillityMove.IsPhysicsMovement() == true)
            {
                abillityMove.Move(InputManager.Controls.Game.Move.ReadValue<Vector3>());
            }
        }
    }
}
