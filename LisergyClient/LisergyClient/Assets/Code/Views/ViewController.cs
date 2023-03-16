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
        public Dictionary<IEntity, IEntityView> _views = new Dictionary<IEntity, IEntityView>();

        public T GetView<T>(IEntity id) where T : IEntityView
        {
            if(_views.TryGetValue(id, out var v)) {
                return (T)v;
            }
            return default(T);
        }

        public T AddView<T>(IEntity id, T view) where T : IEntityView
        {
            _views[id] = view;
            return view;
        }
    }
}
