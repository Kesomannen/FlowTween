using System.Collections.Generic;
using System.Linq;
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
    
    VisualElement _tweenProperties;
    TweenListItemData? _selectedData;

    readonly List<TweenListItemData> _data = new();
    ListView _tweenListView;
    int _tick;
    
    const int RefreshRate = 20;
    
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
        SetupProperties();
        SetupTweenList();

        EditorApplication.update += () => {
            _tick++;
            if (_tick % RefreshRate == 0) {
                Refresh();
            }
        };
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

        _tweenListView.selectionType = SelectionType.Single;
        _tweenListView.selectionChanged += selected => {
            var data = selected.FirstOrDefault();
            if (data == null) ShowProperties(null);
            else ShowProperties(data as TweenListItemData?);
        };
        
        _tweenListView.itemsSource = _data;
        
        VisualElement MakeItem() { 
            var item = _tweenUxml.CloneTree();
            item.Q("tween-owner").Q<ObjectField>().SetEnabled(false);
            return item;
        }

        void BindItem(VisualElement element, int index) {
            var data = _data[index];
            var runnable = data.Runnable;
            
            element.SetEnabled(data.Active);
            element.Q<Label>("tween-name").text = data.Name;
            
            SetOwnerField(element.Q("tween-owner").Q<ObjectField>(), data.Owner);
            
            var progressBar = element.Q("tween-progress").Q<ProgressBar>();
            var progress = runnable.Progress;
            var duration = runnable.TotalDuration;
            progressBar.value = progress;
            progressBar.title = $"{progress * duration:0.#} / {duration:0.#}s ({progress:P0})";
        }
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
        UpdateManager();
        UpdateData();
        _tweenListView.Rebuild();
    }

    void SetupProperties() {
        _tweenProperties = _root.Q("tween-properties");
        _tweenProperties.Q("readonly").SetEnabled(false);

        _tweenProperties.Q<Button>("pause").clicked += () => {
            if (!IsSelectedDataActive(out var data)) return;
            data.Runnable.IsPaused = !data.Runnable.IsPaused;
            RefreshProperties();
        };
        
        _tweenProperties.Q<Button>("cancel").clicked += () => {
            if (!IsSelectedDataActive(out var data)) return;
            _manager.Cancel(data.Runnable, true);
            RefreshProperties();
        };
        
        ShowProperties(null);
    }

    void ShowProperties(TweenListItemData? data) {
        _selectedData = data;
        
        if (!data.HasValue || _data.All(d => d.Runnable != data.Value.Runnable)) {
            _tweenProperties.SetVisible(false);
            return;
        }
        
        var runnable = data.Value.Runnable;
        
        _tweenProperties.SetVisible(true);
        SetOwnerField(_tweenProperties.Q<ObjectField>("owner"), data.Value.Owner);
        
        _tweenProperties.Q<Label>().text = data.Value.Name;
        
        var pauseButton = _tweenProperties.Q<Button>("pause");
        pauseButton.SetEnabled(data.Value.Active);
        pauseButton.Q<MaterialIcon>().IconName = runnable.IsPaused ? "play_arrow" : "pause";
        
        _tweenProperties.Q<Button>("cancel").SetEnabled(data.Value.Active);
        
        _tweenProperties.Q<IntegerField>("loops").value = runnable.Loops ?? -1;
        _tweenProperties.Q<EnumField>("loop-mode").value = runnable.LoopMode;
        _tweenProperties.Q<FloatField>("duration").value = runnable.Duration;
        _tweenProperties.Q<FloatField>("delay").value = runnable.Delay;
        _tweenProperties.Q<Toggle>("ignore-timescale").value = runnable.IgnoreTimescale;
    }
    
    void RefreshProperties() => ShowProperties(_selectedData);
    
    bool IsSelectedDataActive(out TweenListItemData data) {
        data = _selectedData ?? default;
        return data.Active;
    }

    void SetOwnerField(ObjectField field, Object owner) {
        if (owner == null) {
            field.objectType = typeof(Object);
        } else {
            field.objectType = owner.GetType();
            field.value = owner;
        }
    }
    
    struct TweenListItemData {
        public bool Active;
        public Object Owner;
        public Runnable Runnable;

        public string Name {
            get {
                string name;
                
                var type = Runnable.GetType();
                if (type.IsGenericType) {
                    var arguments = type.GetGenericArguments().Select(t => t.Name).ToArray();
                    name = $"{type.Name.Split('`')[0]}<{string.Join(',', arguments)}>";
                } else {
                    name = type.Name;
                }
                
                if (!Active) name += " (inactive)";
                else if (Runnable.IsPaused) name += " (paused)";
                return name;
            }
        }
    }
}