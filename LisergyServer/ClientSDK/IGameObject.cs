using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSDK
{
    using Game.World;

    public interface IGameObject
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public bool Visible { get; set; }
        void AddChild(IGameObject child);
        void DestroyChild(IGameObject child);
        void Destroy();
        T Get<T>() where T : class;
    }
}
