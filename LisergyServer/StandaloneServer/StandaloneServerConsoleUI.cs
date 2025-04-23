using System.Collections;
using System.Collections.Generic;
using Game.Engine;
using Game.Engine.Events;
using Terminal.Gui;

public class LogEntry
{
	public string Log = string.Empty;

	public override string ToString()
	{
		return Log;
	}
}

public class EventEntry
{
	public IBaseEvent Event = null!;

	public override string ToString()
	{
		return Event?.ToString() ?? string.Empty;
	}
}

public enum Tab
{
	LOGS,
	EVENTS,
	GAME,
	ACCOUNT,
	WORLD,
	CHAT
}

/// <summary>
///     SO MANY HACKS JUST FOR A SIMPLE UI
///     But well it helps debugging so why not
/// </summary>
public class StandaloneServerConsoleUI : Window
{
	private static ListView? List;
	private static ScrollBarView? ScrollView;
	private static TextView? TextView;

    /// <summary>
    ///     TYPES OF LOGS
    /// </summary>
    private static readonly List<LogEntry> _accountLogs = new();

	private static readonly List<LogEntry> _worldLogs = new();
	private static readonly List<LogEntry> _chatLogs = new();
	private static readonly List<LogEntry> _gameLogs = new();
	private static readonly List<EventEntry> _events = new();
	private static readonly List<LogEntry> _allLogs = new();

	public static bool IsLoaded;
	private static Tab Tab = Tab.LOGS;

	public StandaloneServerConsoleUI()
	{
		Title = "Lisergy Standalone Server";

		// FIRST COLUMN
		var logsBtn = new Button
		{
			Text = "All",
			Y = 0,
			X = 0,
			IsDefault = true
		};
		var eventsBtn = new Button
		{
			Text = "Events",
			Y = Pos.Bottom(logsBtn),
			X = 0,
			IsDefault = true
		};

		// SECOND COLUMN
		var gameBtn = new Button
		{
			Text = "Game Logic",
			Y = 0,
			X = Pos.Right(logsBtn),
			IsDefault = true
		};
		var accountBtn = new Button
		{
			Text = "Account Server",
			Y = Pos.Bottom(gameBtn),
			X = Pos.Right(eventsBtn),
			IsDefault = true
		};

		// THIRD COLUMN
		var worldBtn = new Button
		{
			Text = "World Server",
			Y = 0,
			X = Pos.Right(gameBtn),
			IsDefault = true
		};
		var chatBtn = new Button
		{
			Text = "Chat Server",
			Y = Pos.Bottom(worldBtn),
			X = Pos.Right(accountBtn),
			IsDefault = true
		};
		var frame = new FrameView
		{
			AutoSize = true,
			Y = Pos.Bottom(chatBtn),
			X = 0,
			Width = Dim.Fill(),
			Height = Dim.Fill()
		};
		List = new ListView(_allLogs)
		{
			AutoSize = true,
			X = 0,
			Y = 0,
			Width = Dim.Fill(),
			Height = Dim.Fill()
		};
		List.DrawContent += e =>
		{
			if (ScrollView != null && List != null)
			{
				ScrollView.Size = List.Source.Count - 1;
				ScrollView.Position = List.TopItem;
				ScrollView.Refresh();
			}
		};
		frame.Add(List);

		ScrollView = new ScrollBarView(List, true);

		TextView = new TextView
		{
			Y = Pos.Bottom(logsBtn),
			X = 0,
			Width = Dim.Fill(),
			Height = Dim.Fill()
		};

		logsBtn.Clicked += () => ViewLogs(Tab.LOGS, _allLogs);
		gameBtn.Clicked += () => ViewLogs(Tab.GAME, _gameLogs);
		accountBtn.Clicked += () => ViewLogs(Tab.ACCOUNT, _accountLogs);
		worldBtn.Clicked += () => ViewLogs(Tab.WORLD, _worldLogs);
		chatBtn.Clicked += () => ViewLogs(Tab.CHAT, _chatLogs);
		eventsBtn.Clicked += () => ViewEvents();

		Loaded += () => { IsLoaded = true; };
		Add(logsBtn, eventsBtn, gameBtn, worldBtn, accountBtn, chatBtn, frame);
	}

	private void ViewEvents()
	{
		Tab = Tab.EVENTS;
		List?.SetSource(_events);
		if (List != null && _events.Count > List.Bounds.Height)
		{
			List.TopItem = _events.Count - List.Bounds.Height;
			List.SelectedItem = List.TopItem;
		}

		List?.SetNeedsDisplay();
	}

	private void ViewLogs(Tab tab, List<LogEntry> logs)
	{
		Tab = tab;
		List?.SetSource(logs);
		UpdateEntryList(logs);
		List?.SetNeedsDisplay();
	}

	private static void UpdateEntryList(ICollection source)
	{
		if (List != null && source.Count > List.Bounds.Height)
		{
			List.TopItem = source.Count - List.Bounds.Height;
			List.SelectedItem = List.TopItem;
			List.SetNeedsDisplay();
		}
	}

	public static void OnReceiveEvent(IBaseEvent ev)
	{
		if (Application.MainLoop == null) return;

		Application.MainLoop.Invoke(() =>
		{
			_events.Add(new EventEntry
			{
				Event = ev
			});
			if (_events.Count > 100) _events.RemoveAt(0);
			if (Tab == Tab.EVENTS && List != null) UpdateEntryList(_events);
		});
	}

	public static IGameLog HookLogs(IGameLog log)
	{
		if (log == null) return null!;

		var baseLog = (GameLog) log;
		baseLog._Debug = m => OnReceiveLog(baseLog.Tag, 0, m);
		baseLog._Info = m => OnReceiveLog(baseLog.Tag, 1, m);
		baseLog._Error = m => OnReceiveLog(baseLog.Tag, 2, m);
		return log;
	}

	private static void AddLog(Tab forTab, List<LogEntry> log, LogEntry newLog)
	{
		if (log == null || newLog == null) return;

		log.Add(newLog);
		if (log.Count > 500) log.RemoveAt(0);
		if (Tab == forTab && List != null) UpdateEntryList(log);
	}

	public static void OnReceiveLog(string tag, int level, string msg)
	{
		if (Application.MainLoop == null) return;

		Application.MainLoop.Invoke(() =>
		{
			var entry = new LogEntry
			{
				Log = msg ?? string.Empty
			};
			AddLog(Tab.LOGS, _allLogs, entry);

			tag = tag ?? string.Empty;

			if (tag.StartsWith("[Server Game]")) AddLog(Tab.GAME, _gameLogs, entry);
			else if (tag.StartsWith("[Server WORLD]")) AddLog(Tab.WORLD, _worldLogs, entry);
			else if (tag.StartsWith("[Server ACCOUNT]")) AddLog(Tab.ACCOUNT, _accountLogs, entry);
			else if (tag.StartsWith("[Server CHAT]")) AddLog(Tab.CHAT, _chatLogs, entry);
		});
	}
}