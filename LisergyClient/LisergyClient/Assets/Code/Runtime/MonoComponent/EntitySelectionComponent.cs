using Game;
using Game.ECS;
using UnityEngine;

namespace Assets.Code
{
    /// <summary>
    /// Unit selector indicator to indicate which unit you have selected
    /// </summary>
    public class EntitySelectionComponent : MonoBehaviour
    {
        public GameObject SelectorCrystal;
        public GameObject UnitRadial;
        public GameObject BuildingRadial;

        void Update()
        {
            SelectorCrystal.transform.Rotate(0, 0.2f, 0);
        }
    }
}
