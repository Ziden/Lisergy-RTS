using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Assets.Code;
using System.Linq;

public class LogWindow : EditorWindow
{
    private Rect upperPanel;
    private Rect lowerPanel;
    private Rect resizer;
    private Rect menuBar;

    private float sizeRatio = 0.5f;
    private bool isResizing;

    private float resizerHeight = 5f;
    private float menuBarHeight = 20f;

    private bool collapse = false;
    private bool clearOnPlay = false;
    private bool errorPause = false;
    private bool showLog = false;
    private bool showWarnings = false;
    private bool showErrors = false;

    private Vector2 upperPanelScroll;
    private Vector2 lowerPanelScroll;

    private GUIStyle resizerStyle;
    private GUIStyle boxStyle;
    private GUIStyle textAreaStyle;

    private Texture2D boxBgOdd;
    private Texture2D boxBgEven;
    private Texture2D boxBgSelected;
    private Texture2D icon;
    private Texture2D errorIcon;
    private Texture2D errorIconSmall;
    private Texture2D warningIcon;
    private Texture2D warningIconSmall;
    private Texture2D infoIcon;
    private Texture2D infoIconSmall;

    private List<StackedLog> logs;
    private StackedLog selectedLog;
    public static LogWindow Window;

    [MenuItem("Window/Lisergy/Logs")]
    private static void OpenWindow()
    {
        LogWindow window = GetWindow<LogWindow>();
        window.titleContent = new GUIContent("Lisergy Logs");
        Window = window;
    }

    private void OnEnable()
    {
        errorIcon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;
        warningIcon = EditorGUIUtility.Load("icons/console.warnicon.png") as Texture2D;
        infoIcon = EditorGUIUtility.Load("icons/console.infoicon.png") as Texture2D;

        errorIconSmall = EditorGUIUtility.Load("icons/console.erroricon.sml.png") as Texture2D;
        warningIconSmall = EditorGUIUtility.Load("icons/console.warnicon.sml.png") as Texture2D;
        infoIconSmall = EditorGUIUtility.Load("icons/console.infoicon.sml.png") as Texture2D;

        resizerStyle = new GUIStyle();
        resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

        boxStyle = new GUIStyle();
        boxStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);

        boxBgOdd = EditorGUIUtility.Load("builtin skins/darkskin/images/cn entrybackodd.png") as Texture2D;
        boxBgEven = EditorGUIUtility.Load("builtin skins/darkskin/images/cnentrybackeven.png") as Texture2D;
        boxBgSelected = EditorGUIUtility.Load("builtin skins/darkskin/images/menuitemhover.png") as Texture2D;

        textAreaStyle = new GUIStyle();
        textAreaStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
        textAreaStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/projectbrowsericonareabg.png") as Texture2D;

        logs = new List<StackedLog>();
        selectedLog = null;
        Window = this;
        //Application.logMessageReceived += LogMessageReceived;
    }

    private void OnDisable()
    {
        //Application.logMessageReceived -= LogMessageReceived;
    }

    private void OnDestroy()
    {
        //Application.logMessageReceived -= TagRemoved;
    }

    private void OnGUI()
    {
        DrawMenuBar();
        DrawUpperPanel();
        DrawLowerPanel();
        DrawResizer();

        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void DrawMenuBar()
    {
        menuBar = new Rect(0, 0, position.width, menuBarHeight);

        GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
        GUILayout.BeginHorizontal();

        GUILayout.Button(new GUIContent("Clear"), EditorStyles.toolbarButton, GUILayout.Width(35));
        GUILayout.Space(5);

        collapse = GUILayout.Toggle(collapse, new GUIContent("Collapse"), EditorStyles.toolbarButton, GUILayout.Width(50));
        clearOnPlay = GUILayout.Toggle(clearOnPlay, new GUIContent("Clear On Play"), EditorStyles.toolbarButton, GUILayout.Width(70));
        errorPause = GUILayout.Toggle(errorPause, new GUIContent("Error Pause"), EditorStyles.toolbarButton, GUILayout.Width(60));

        GUILayout.FlexibleSpace();

        showLog = GUILayout.Toggle(showLog, new GUIContent("L", infoIconSmall), EditorStyles.toolbarButton, GUILayout.Width(30));
        showWarnings = GUILayout.Toggle(showWarnings, new GUIContent("W", warningIconSmall), EditorStyles.toolbarButton, GUILayout.Width(30));
        showErrors = GUILayout.Toggle(showErrors, new GUIContent("E", errorIconSmall), EditorStyles.toolbarButton, GUILayout.Width(30));

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void DrawLog(StackedLog log, bool oddLine, int tabSize)
    {
        if (DrawBox(log.info, log.type, oddLine, log.isSelected, tabSize))
        {
            if (selectedLog != null)
            {
                selectedLog.isSelected = false;
            }

            log.isSelected = true;
            selectedLog = log;
            GUI.changed = true;
        }
    }

    private bool _gray = true;

    private void RenderLogs(List<StackedLog> logs, int tab)
    {
        logs.ForEach(log =>
        {
            DrawLog(log, _gray, tab);
            _gray = !_gray;

            if (true || log.isSelected)
            {
                tab = tab + 1;
                RenderLogs(log.Children, tab);
            }
        });
    }

    private void DrawUpperPanel()
    {
        upperPanel = new Rect(0, menuBarHeight, position.width, (position.height * sizeRatio) - menuBarHeight);

        GUILayout.BeginArea(upperPanel);
        upperPanelScroll = GUILayout.BeginScrollView(upperPanelScroll);
        var tab = 0;
        RenderLogs(logs, tab);
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DrawLowerPanel()
    {
        lowerPanel = new Rect(0, (position.height * sizeRatio) + resizerHeight, position.width, (position.height * (1 - sizeRatio)) - resizerHeight);

        GUILayout.BeginArea(lowerPanel);
        lowerPanelScroll = GUILayout.BeginScrollView(lowerPanelScroll);

        if (selectedLog != null)
        {
            GUILayout.TextArea(selectedLog.message, textAreaStyle);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DrawResizer()
    {
        resizer = new Rect(0, (position.height * sizeRatio) - resizerHeight, position.width, resizerHeight * 2);

        GUILayout.BeginArea(new Rect(resizer.position + (Vector2.up * resizerHeight), new Vector2(position.width, 2)), resizerStyle);
        GUILayout.EndArea();

        EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeVertical);
    }

    private bool DrawBox(string content, LogType boxType, bool isOdd, bool isSelected, int tabSize)
    {
        if (isSelected)
        {
            boxStyle.normal.background = boxBgSelected;
        }
        else
        {
            if (isOdd)
            {
                boxStyle.normal.background = boxBgOdd;
            }
            else
            {
                boxStyle.normal.background = boxBgEven;
            }
        }

        switch (boxType)
        {
            case LogType.Error: icon = errorIcon; break;
            case LogType.Exception: icon = errorIcon; break;
            case LogType.Assert: icon = errorIcon; break;
            case LogType.Warning: icon = warningIcon; break;
            case LogType.Log: icon = infoIcon; break;
        }

        return GUILayout.Button(new GUIContent(content, icon), boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));
    }

    private void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0 && resizer.Contains(e.mousePosition))
                {
                    isResizing = true;
                }
                break;

            case EventType.MouseUp:
                isResizing = false;
                break;
        }

        Resize(e);
    }

    private void Resize(Event e)
    {
        if (isResizing)
        {
            sizeRatio = e.mousePosition.y / position.height;
            Repaint();
        }
    }

    public void TagRemoved(string tag, List<string> taglogs)
    {
        taglogs.Reverse();
        StackedLog l = new StackedLog(false, tag, taglogs != null ? string.Join("\n", taglogs) : "", LogType.Log);
        logs.Add(l);
    }
}

public class StackedLog
{
    public bool isSelected;
    public string info;
    public string message;
    public LogType type;
    public List<StackedLog> Children = new List<StackedLog>();
    public StackedLog Parent;

    public void SetParent(StackedLog log)
    {
        Parent = log;
        log.Children.Add(this);
    }

    public StackedLog(bool isSelected, string info, string message, LogType type)
    {
        this.isSelected = isSelected;
        this.info = info;
        this.message = message;
        this.type = type;
    }
}