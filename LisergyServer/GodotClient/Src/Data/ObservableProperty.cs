using System;
using System.Collections.Generic;

namespace LisergyGodotClient.Src.Data;

public class ObservableProperty<T>
{
	private T _value;

	public ObservableProperty(T value)
	{
		_value = value;
	}

	public ObservableProperty()
	{
		_value = default;
	}

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

	public event Action<T> OnChanged;
}