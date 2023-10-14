using Game;
using Game.Events;
using MapServer;
using NServiceBus.Logging;
using System.Collections;
using System.Text;
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

public enum Tab { LOGS, EVENTS }

public class StandaloneServerConsoleUI : Window
{
    private static ListView List;
    private static ScrollBarView ScrollView;
    private static TextView TextView;
    private static List<EventEntry> _events = new();
    private static List<LogEntry> _log = new();
    public static bool IsLoaded = false;
    private static Tab Tab = Tab.LOGS;

    public StandaloneServerConsoleUI()
    {
        Title = "Lisergy Standalone Server";

        var logsBtn = new Button()
        {
            Text = "Logs",
            Y = 0,
            X = 0,
            IsDefault = true,
        };
        var eventsBtn = new Button()
        {
            Text = "Events",
            Y = 0,
            X = Pos.Right(logsBtn),
            IsDefault = true,
        };
        var frame = new FrameView()
        {
            AutoSize = true,
            Y = Pos.Bottom(logsBtn),
            X = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        List = new ListView(_log)
        {
            AutoSize = true,
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        List.DrawContent += (e) => {
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

        Log._Debug = m => OnReceiveLog(LogLevel.Debug, m);
        Log._Info = m => OnReceiveLog(LogLevel.Info, m);
        Log._Error = m => OnReceiveLog(LogLevel.Error, m);
      

        logsBtn.Clicked += () => ViewLogs();
        eventsBtn.Clicked += () => ViewEvents();

        this.Loaded += () =>
        {
            IsLoaded = true;
        };
        Add(logsBtn, eventsBtn, frame);
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

    private void ViewLogs()
    {
        Tab = Tab.LOGS;
        List.SetSource(_log);
        UpdateEntryList(_log);
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
            if (_events.Count > 500) _events.RemoveAt(0);
            if (Tab == Tab.EVENTS) UpdateEntryList(_events);
        });
    }


    public void OnReceiveLog(LogLevel level, string msg)
    {
        Application.MainLoop.Invoke(() =>
        {
            _log.Add(new LogEntry()
            {
                Log = msg
            });
            if (_log.Count > 500) _log.RemoveAt(0);
            if(Tab == Tab.LOGS) UpdateEntryList(_log);
        });
    }
}