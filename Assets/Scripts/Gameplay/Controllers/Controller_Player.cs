using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;

namespace Scripts.Gameplay.Controllers
{
    public class Controller_Player : Controller
    {
        private AbillityMove abillityMove;

        public override void Initialize(Entity entity)
        {
            abillityMove = entity.FindAbillity<AbillityMove>();
        }
    }
}
