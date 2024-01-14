using System.Collections.Generic;
using FlowTween;
using FlowTween.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TweenManagerControlPanel : EditorWindow {
    [SerializeField] VisualTreeAsset _mainUxml;
    [SerializeField] VisualTreeAsset _tweenUxml;

    TweenManager _manager;
    VisualElement _root;

    readonly List<TweenListItemData> _data = new();
    ListView _tweenListView;

    [MenuItem("Window/FlowTween/Control Panel")]
    public static void ShowPanel() {
        var window = GetWindow<TweenManagerControlPanel>();
        window.titleContent = new GUIContent("Flow Tween Control Panel");
        window.minSize = new Vector2(640, 340);
    }

    public void CreateGUI() {
        _root = _mainUxml.CloneTree();
        rootVisualElement.Add(_root);
        
        UpdateManager();
        SetupTweenList();
    }

    void UpdateManager() {
        var helpMessage = _root.Q<VisualElement>("help-message");
        
        if (!Application.isPlaying)
        {
            SetHelpMessage("info", "Enter play mode to enable the control panel", Color.white);
            _root.Q("tweens").SetEnabled(false);
            return;
        }

        _manager = TweenManager.Singleton;

        if (_manager == null)
        {
            SetHelpMessage("error", "Tween manager not found! (have you disabled it?)", Color.red);
            _root.Q("tweens").SetEnabled(false);
        }
        else
        {
            helpMessage.SetVisible(false);
            _root.Q("tweens").SetEnabled(true);
        }
        
        void SetHelpMessage(string icon, string message, Color iconColor) {
            helpMessage.SetVisible(true);
            helpMessage.Q<Label>().text = message;
        
            var iconElement = helpMessage.Q<MaterialIcon>();
            iconElement.style.color = iconColor;
            iconElement.IconName = icon;
        }
    }

    void SetupTweenList() {
        _tweenListView = _root.Q<ListView>();
        
        _tweenListView.makeItem = MakeItem;
        _tweenListView.bindItem = BindItem;
        _tweenListView.unbindItem = UnbindItem;
        
        _tweenListView.itemsSource = _data;
        
        VisualElement MakeItem() {
            return _tweenUxml.CloneTree();
        }

        void BindItem(VisualElement element, int index) {
            var data = _data[index];
            var runnable = data.Runnable;
            
            element.SetEnabled(data.Active);
            element.Q<Label>("tween-name").text = data.Name;
            
            
            var ownerField = element.Q("tween-owner").Q<ObjectField>();
            if (data.Owner == null) {
                ownerField.SetEnabled(false);
                ownerField.objectType = typeof(Object);
            } else {
                ownerField.SetEnabled(true);
                ownerField.objectType = data.Owner.GetType();
                ownerField.value = data.Owner;
            }
            
            
            var progressBar = element.Q("tween-progress").Q<ProgressBar>();
            var progress = runnable.Progress;
            var duration = runnable.TotalDuration;
            progressBar.value = progress;
            progressBar.title = $"{progress * duration:0} / {duration:0}s ({progress:P0}%)";
            
            
            var controls = element.Q("tween-controls");
            var pauseButton = controls.Q<Button>("pause");
            pauseButton.Q<MaterialIcon>().IconName = runnable.IsPaused ? "play_arrow" : "pause";
            pauseButton.RegisterCallback<ClickEvent, int>(OnPauseButtonClicked, index);
            
            controls.Q<Button>("cancel").RegisterCallback<ClickEvent, int>(OnCancelButtonClicked, index);
        }
        
        void UnbindItem(VisualElement element, int index) {
            var controls = element.Q("tween-controls");
            controls.Q<Button>("pause").UnregisterCallback<ClickEvent, int>(OnPauseButtonClicked);
            controls.Q<Button>("cancel").UnregisterCallback<ClickEvent, int>(OnCancelButtonClicked);
        }
    }
    
    void OnPauseButtonClicked(ClickEvent evt, int index) {
        var runnable = _data[index].Runnable;
        runnable.IsPaused = !runnable.IsPaused;
    }
    
    void OnCancelButtonClicked(ClickEvent evt, int index) {
        _manager.Cancel(_data[index].Runnable, true);
    }

    void UpdateData() {
        _data.Clear();

        if (_manager == null) return;
        
        foreach (var tween in _manager.ActiveTweens) {
            _data.Add(new TweenListItemData {
                Active = true,
                Owner = tween.Owner,
                Runnable = tween.Runnable
            });
        }
        
        foreach (var (_, queue) in _manager.InactiveTweens) {
            foreach (var tween in queue) {
                _data.Add(new TweenListItemData {
                    Active = false,
                    Runnable = tween
                });
            }
        }
    }

    void Refresh() {
        UpdateData();
        _tweenListView.Rebuild();
    }

    struct TweenListItemData {
        public bool Active;
        public Object Owner;
        public Runnable Runnable;

        public string Name {
            get {
                var name = Runnable.GetType().Name;
                if (!Active) name += " (inactive)";
                return name;
            }
        }
    }
}