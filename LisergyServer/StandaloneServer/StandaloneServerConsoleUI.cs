using Game.Engine;
using Game.Engine.Events;
using System.Collections;
using Terminal.Gui;

public class LogEntry
{
    public string Log;

    public override string ToString() => Log;
}

public class EventEntry
{
    public IBaseEvent Event;

    public override string ToString() => Event.ToString();

}

public enum Tab { LOGS, EVENTS, GAME, ACCOUNT, WORLD, CHAT }

/// <summary>
/// SO MANY HACKS JUST FOR A SIMPLE UI
/// But well it helps debugging so why not
/// </summary>
public class StandaloneServerConsoleUI : Window
{
    private static ListView List;
    private static ScrollBarView ScrollView;
    private static TextView TextView;

    /// <summary>
    /// TYPES OF LOGS
    /// </summary>
    private static List<LogEntry> _accountLogs = new();
    private static List<LogEntry> _worldLogs = new();
    private static List<LogEntry> _chatLogs = new();
    private static List<LogEntry> _gameLogs = new();
    private static List<EventEntry> _events = new();
    private static List<LogEntry> _allLogs = new();


    public static bool IsLoaded = false;
    private static Tab Tab = Tab.LOGS;

    public StandaloneServerConsoleUI()
    {
        Title = "Lisergy Standalone Server";

        // FIRST COLUMN
        var logsBtn = new Button()
        {
            Text = "All",
            Y = 0,
            X = 0,
            IsDefault = true,
        };
        var eventsBtn = new Button()
        {
            Text = "Events",
            Y = Pos.Bottom(logsBtn),
            X = 0,
            IsDefault = true,
        };

        // SECOND COLUMN
        var gameBtn = new Button()
        {
            Text = "Game Logic",
            Y = 0,
            X = Pos.Right(logsBtn),
            IsDefault = true,
        };
        var accountBtn = new Button()
        {
            Text = "Account Server",
            Y = Pos.Bottom(gameBtn),
            X = Pos.Right(eventsBtn),
            IsDefault = true,
        };

        // THIRD COLUMN
        var worldBtn = new Button()
        {
            Text = "World Server",
            Y = 0,
            X = Pos.Right(gameBtn),
            IsDefault = true,
        };
        var chatBtn = new Button()
        {
            Text = "Chat Server",
            Y = Pos.Bottom(worldBtn),
            X = Pos.Right(accountBtn),
            IsDefault = true,
        };
        var frame = new FrameView()
        {
            AutoSize = true,
            Y = Pos.Bottom(chatBtn),
            X = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        List = new ListView(_allLogs)
        {
            AutoSize = true,
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        List.DrawContent += (e) =>
        {
            ScrollView.Size = List.Source.Count - 1;
            ScrollView.Position = List.TopItem;
            ScrollView.Refresh();
        };
        frame.Add(List);

        ScrollView = new ScrollBarView(List, true);

        TextView = new TextView()
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

        this.Loaded += () =>
        {
            IsLoaded = true;
        };
        Add(logsBtn, eventsBtn, gameBtn, worldBtn, accountBtn, chatBtn, frame);
    }

    private void ViewEvents()
    {
        Tab = Tab.EVENTS;
        List.SetSource(_events);
        if (_events.Count > List.Bounds.Height)
        {
            List.TopItem = _events.Count - List.Bounds.Height;
            List.SelectedItem = List.TopItem;
        }
        List.SetNeedsDisplay();
    }

    private void ViewLogs(Tab tab, List<LogEntry> logs)
    {
        Tab = tab;
        List.SetSource(logs);
        UpdateEntryList(logs);
        List.SetNeedsDisplay();
    }

    private static void UpdateEntryList(ICollection source)
    {
        if (source.Count > List.Bounds.Height)
        {
            List.TopItem = source.Count - List.Bounds.Height;
            List.SelectedItem = List.TopItem;
        }
        List.SetNeedsDisplay();
    }

    public static void OnReceiveEvent(IBaseEvent ev)
    {
        Application.MainLoop.Invoke(() =>
        {
            _events.Add(new EventEntry()
            {
                Event = ev,
            });
            if (_events.Count > 100) _events.RemoveAt(0);
            if (Tab == Tab.EVENTS) UpdateEntryList(_events);
        });
    }

    public static IGameLog HookLogs(IGameLog log)
    {
        var baseLog = (GameLog)log;
        baseLog._Debug = m => OnReceiveLog(baseLog.Tag, 0, m);
        baseLog._Info = m => OnReceiveLog(baseLog.Tag, 1, m);
        baseLog._Error = m => OnReceiveLog(baseLog.Tag, 2, m);
        return log;
    }

    private static void AddLog(Tab forTab, List<LogEntry> log, LogEntry newLog)
    {
        log.Add(newLog);
        if (log.Count > 500) log.RemoveAt(0);
        if (Tab == forTab) UpdateEntryList(log);
    }


    public static void OnReceiveLog(string tag, int level, string msg)
    {
        Application.MainLoop.Invoke(() =>
        {
            var entry = new LogEntry()
            {
                Log = msg
            };
            AddLog(Tab.LOGS, _allLogs, entry);
            if (tag.StartsWith("[Server Game]")) AddLog(Tab.GAME, _gameLogs, entry);
            else if (tag.StartsWith("[Server WORLD]")) AddLog(Tab.WORLD, _worldLogs, entry);
            else if (tag.StartsWith("[Server ACCOUNT]")) AddLog(Tab.ACCOUNT, _accountLogs, entry);
            else if (tag.StartsWith("[Server CHAT]")) AddLog(Tab.CHAT, _chatLogs, entry);
        });
    }
}