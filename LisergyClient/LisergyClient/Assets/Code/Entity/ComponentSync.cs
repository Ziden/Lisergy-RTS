using Game.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Entity
{
    public static class ComponentSync
    {
        public static void UpdateFrom(this IComponent fromObject, IComponent toObject)
        {
            PropertyInfo[] toObjectProperties = toObject.GetType().GetProperties();
            foreach (PropertyInfo propTo in toObjectProperties)
            {
                PropertyInfo propFrom = fromObject.GetType().GetProperty(propTo.Name);
                if (propFrom != null && propFrom.CanWrite)
                    propTo.SetValue(toObject, propFrom.GetValue(fromObject, null), null);
            }
        }

        public static T Get<T>(this List<IComponent> l) => (T)l.First(lc => lc.GetType()==typeof(T));
    }
}
