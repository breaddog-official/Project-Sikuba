using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using Scripts.Gameplay.Listners;
using System.Collections.Generic;

namespace Scripts.Gameplay.Controllers
{
    public class Controller_Player : Controller
    {
        private IEnumerable<IListnerMove> listnersMove;
        private IEnumerable<IListnerFire> listnersFire;

        public override void Initialize(Entity entity)
        {
            listnersMove = GetAsInterfaces<IListnerMove>(entity.Abillities);
            listnersFire = GetAsInterfaces<IListnerFire>(entity.Abillities);
        }
    }
}
