using System;
using System.Text;

namespace LisergyGodotClient.Src.Services;

public interface IClientAnalytics
{
	void TrackError(Exception e);
}

public class GodotAnalyticsService : IClientAnalytics
{
	public void TrackError(Exception e)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Godot Client Exception: {e.GetType().Name}");
		sb.AppendLine($"Message: {e.Message}");

		if (e.StackTrace != null)
			sb.AppendLine(e.StackTrace);

		// Include inner exception if available
		if (e.InnerException != null)
		{
			sb.AppendLine($"Inner Exception: {e.InnerException.GetType().Name}");
			sb.AppendLine($"Inner Message: {e.InnerException.Message}");
			if (e.InnerException.StackTrace != null)
				sb.AppendLine(e.InnerException.StackTrace);
		}

		ClientServices.Log.Error(sb.ToString());
	}
}