using System;

public class RingBuffer
{
	private readonly byte[] _buffer;
	private int _head;
	private int _tail;

	public RingBuffer(int capacity)
	{
		_buffer = new byte[capacity];
		_head = 0;
		_tail = 0;
		Size = 0;
	}

	public int Capacity => _buffer.Length;
	public int Size { get; private set; }

	public void Write(byte[] data)
	{
		if (data.Length > Capacity)
			throw new ArgumentException("Data length exceeds buffer capacity");

		foreach (var b in data)
		{
			_buffer[_tail] = b;
			_tail = (_tail + 1) % Capacity;
			if (Size < Capacity)
				Size++;
			else
				_head = (_head + 1) % Capacity; // Overwrite the oldest data
		}
	}

	public byte[] Read(int length)
	{
		if (length > Size)
			throw new ArgumentException("Length exceeds buffer size");

		var result = new byte[length];
		for (var i = 0; i < length; i++)
		{
			result[i] = _buffer[_head];
			_head = (_head + 1) % Capacity;
		}

		Size -= length;
		return result;
	}

	public void Clear()
	{
		_head = 0;
		_tail = 0;
		Size = 0;
	}
}