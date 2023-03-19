using Game;
using Game.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Views
{
    public class ViewController
    {
        public Dictionary<object, IEntityView> _views = new Dictionary<object, IEntityView>();

        public T GetView<T>(object id) where T : IEntityView
        {
            if(_views.TryGetValue(id, out var v)) {
                return (T)v;
            }
            return default(T);
        }

        public IEntityView GetView(object id)
        {
            if (_views.TryGetValue(id, out var v))
            {
                return v;
            }
            return null;
        }

        public T AddView<T>(object id, T view) where T : IEntityView
        {
            _views[id] = view;
            return view;
        }
    }
}
