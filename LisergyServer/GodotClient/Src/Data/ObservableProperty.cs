using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisergyGodotClient.Src.Data
{
    public class ObservableProperty<T>
    {
        public event Action<T> OnChanged;

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value)) return;
                _value = value;
                OnChanged?.Invoke(value);
            }
        }

        public ObservableProperty(T value)
        {
            _value = value;
        }

        public ObservableProperty()
        {
            _value = default;
        }
    }
}
