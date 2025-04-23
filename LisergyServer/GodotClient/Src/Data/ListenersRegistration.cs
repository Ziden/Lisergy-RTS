using System;
using System.Reflection;
using Game.Engine.Events.Bus;
using LisergyGodotClient.Src;

namespace LisergyGodotClient.Data;

public interface IAutoRegisterListener : IEventListener
{
	public void OnRegister();
}

public static class ListenersRegistration
{
	public static void LoadAutoRegisterListeners()
	{
		foreach (var t in Assembly.GetAssembly(typeof(IAutoRegisterListener))!.GetTypes())
		{
			if (typeof(IAutoRegisterListener).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
			{
				var instance = (IAutoRegisterListener)Activator.CreateInstance(t);
				instance!.OnRegister();
				ClientServices.Log.Debug($"Event Listener {t.Name} Registered");
			}
		}
	}
}