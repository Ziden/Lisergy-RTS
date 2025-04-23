using System;

namespace Game.Engine.Events.Bus
{
	public class ListenerWrapper
	{
		public ListenerWrapper(IEventListener listener, Delegate callback)
		{
			Listener = listener;
			Callback = callback;
		}

		public IEventListener Listener { get; }
		public Delegate Callback { get; }
	}
}