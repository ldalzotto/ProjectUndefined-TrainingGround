﻿using InteractiveObjectTest;
using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractiveObjectEditorV2 : EditorWindow
{
    public static InteractiveObjectEditorV2 Instance;

    [MenuItem("InteractiveObjectEditorV2/InteractiveObjectEditorV2")]
    public static void Init()
    {
        var wnd = GetWindow<InteractiveObjectEditorV2>();
        Instance = wnd;
        wnd.Show();
    }

    private VisualElement RootElement;

    private VisualElement LeftPanel;

    private ScrollView ObjectFieldParent;
    private TextField SeachTextElement;
    private SelectedInteractiveObjectDetail SelectedInteractiveObjectDetail;

    private void OnEnable()
    {
        this.RootElement = new VisualElement();
        this.RootElement.style.flexDirection = FlexDirection.Row;

        this.LeftPanel = new VisualElement();
        this.LeftPanel.style.flexDirection = FlexDirection.Column;
        this.LeftPanel.style.alignSelf = Align.FlexStart;

        this.SeachTextElement = new TextField();
        this.SeachTextElement.RegisterCallback<ChangeEvent<string>>(this.OnSearchTextChange);
        this.LeftPanel.Add(this.SeachTextElement);

        this.ObjectFieldParent = new ScrollView(ScrollViewMode.Vertical);
        this.LeftPanel.Add(this.ObjectFieldParent);

        this.RootElement.Add(this.LeftPanel);
        this.SelectedInteractiveObjectDetail = new SelectedInteractiveObjectDetail(this.RootElement);

        rootVisualElement.Add(this.RootElement);

        EditorApplication.playModeStateChanged += this.OnPlayModeStateChanged;
        EditorApplication.update += this.Tick;
    }

    private void OnDestroy()
    {
        Instance = null;
        EditorApplication.playModeStateChanged -= this.OnPlayModeStateChanged;
        EditorApplication.update -= this.Tick;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoaded()
    {

        Debug.Break();
    }

    private Dictionary<object, ListenedObjectField> ListenableObjectFields = new Dictionary<object, ListenedObjectField>();
    private Dictionary<string, VisualElement> ClassHeaderElements = new Dictionary<string, VisualElement>();

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            this.OnPlayModeEnter();
        }
    }

    private void OnPlayModeEnter()
    {
        InteractiveObjectV2Manager.Get().RegisterOnInteractiveObjectCreatedEventListener(this.OnInteractiveObjectCreated);
        InteractiveObjectV2Manager.Get().RegisterOnInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
        RangeObjectV2Manager.Get().RegisterOnRangeObjectCreatedEventListener(this.OnInteractiveObjectCreated);
        RangeObjectV2Manager.Get().RegisterOnRangeObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
    }

    private void Tick()
    {
        if (Application.isPlaying)
        {

            var allInteractiveObjects = InteractiveObjectV2Manager.Get().InteractiveObjects;
            foreach (var interactiveObject in allInteractiveObjects)
            {
                this.OnInteractiveObjectCreated(interactiveObject);
            }

            var allRangeObjects = RangeObjectV2Manager.Get().RangeObjects;
            foreach(var rangeObject in allRangeObjects)
            {
                this.OnInteractiveObjectCreated(rangeObject);
            }

            this.SelectedInteractiveObjectDetail.OnGui();
        }
    }

    private void OnInteractiveObjectCreated(object interactiveObject)
    {
        this.ListenableObjectFields.TryGetValue(interactiveObject, out ListenedObjectField InteractiveObjectField);
        if (InteractiveObjectField == null)
        {
            InteractiveObjectField = new ListenedObjectField(this.ObjectFieldParent, interactiveObject,
                      OnInteractiveObjectFieldClicked: this.OnInteractiveObjectFieldClicked);
            this.ListenableObjectFields.Add(interactiveObject, InteractiveObjectField);

            this.ClassHeaderElements.TryGetValue(interactiveObject.GetType().Name, out VisualElement header);
            if (header == null)
            {
                header = new VisualElement();
                var headerText = new TextElement()
                {
                    text = interactiveObject.GetType().Name,
                };

                headerText.style.borderTopWidth = 3f;
                headerText.style.borderBottomWidth = 3f;
                headerText.style.unityFontStyleAndWeight = FontStyle.Bold;

                header.Add(headerText);
                this.ClassHeaderElements[interactiveObject.GetType().Name] = header;
                this.ObjectFieldParent.Add(header);
            }

            header.Add(this.ListenableObjectFields[interactiveObject]);
        }
    }

    private void OnInteractiveObjectDestroyed(object CoreInteractiveObject)
    {
        this.ListenableObjectFields.TryGetValue(CoreInteractiveObject, out ListenedObjectField InteractiveObjectField);
        if (InteractiveObjectField != null)
        {
            if (this.SelectedInteractiveObjectDetail.CurrentInteracitveObjectFieldSelected == InteractiveObjectField)
            {
                this.SelectedInteractiveObjectDetail.ResetElement();
            }
            this.ClassHeaderElements.TryGetValue(CoreInteractiveObject.GetType().Name, out VisualElement header);
            if (header != null)
            {
                header.Remove(InteractiveObjectField);
            }

            this.ListenableObjectFields.Remove(CoreInteractiveObject);
        }
    }


    private void OnSearchTextChange(ChangeEvent<string> evt)
    {
        if (Application.isPlaying)
        {
            var allInteractiveObjects = InteractiveObjectV2Manager.Get().InteractiveObjects;
            var allRangeObjects = RangeObjectV2Manager.Get().RangeObjects;

            var allInteractiveObjectsClassname = allInteractiveObjects.ToList()
                   .Select(i => i).Where(i => string.IsNullOrEmpty(this.SeachTextElement.text) || i.InteractiveGameObject.InteractiveGameObjectParent.name.Contains(this.SeachTextElement.text)).ToList()
                   .ConvertAll(i => i.GetType().Name)
                   .Union(
                        allRangeObjects.Select(r => r).Where(r => string.IsNullOrEmpty(this.SeachTextElement.text) || r.RangeGameObjectV2.RangeGameObject.name.Contains(this.SeachTextElement.text)).ToList()
                            .ConvertAll(r => r.GetType().Name)
                    )
                   .ToList();
            foreach (var classHeaderElement in this.ClassHeaderElements)
            {
                classHeaderElement.Value.style.display = allInteractiveObjectsClassname.Contains(classHeaderElement.Key) ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }

    private void OnInteractiveObjectFieldClicked(MouseDownEvent MouseDownEvent, ListenedObjectField InteractiveObjectField)
    {
        foreach (var interactiveObjectField in this.ListenableObjectFields.Values)
        {
            interactiveObjectField.SetIsSelected(interactiveObjectField == InteractiveObjectField);
            this.SelectedInteractiveObjectDetail.OnInteractiveObjectSelected(InteractiveObjectField);
        }
    }
}

class ListenedObjectField : VisualElement
{
    private BoolVariable IsSelected;

    private Label ObjectLabel;

    private GameObject ObjectReference;
    public object ListenedObjectRef { get; private set; }

    private Color InitialBackGroundColor;

    private Action<MouseDownEvent, ListenedObjectField> OnInteractiveObjectFieldClicked;

    public ListenedObjectField(VisualElement parent, object listenedField, Action<MouseDownEvent, ListenedObjectField> OnInteractiveObjectFieldClicked = null)
    {
        this.ListenedObjectRef = listenedField;

        switch (listenedField)
        {
            case CoreInteractiveObject coreInteractiveObject:
                this.ObjectReference = coreInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent;
                break;
            case RangeObjectV2 rangeObjectV2:
                this.ObjectReference = rangeObjectV2.RangeGameObjectV2.RangeGameObject;
                break;
        }

        this.OnInteractiveObjectFieldClicked = OnInteractiveObjectFieldClicked;
        this.IsSelected = new BoolVariable(false, this.OnInteractiveObjectSelected, this.OnInteractiveObjetDeSelected);

        this.InitialBackGroundColor = this.style.backgroundColor.value;

        this.ObjectLabel = new Label(this.ObjectReference.name);
        this.ObjectLabel.style.marginLeft = 10f;

        this.Add(this.ObjectLabel);

        parent.Add(this);

        this.RegisterCallback<MouseEnterEvent>(this.OnMouseEnter);
        this.RegisterCallback<MouseOutEvent>(this.OnMouseExit);
        this.RegisterCallback<MouseDownEvent>(this.OnMouseDown);
    }

    public void SetIsSelected(bool value) { this.IsSelected.SetValue(value); }

    private void OnMouseDown(MouseDownEvent MouseDownEvent)
    {
        Selection.activeGameObject = this.ObjectReference;
        if (this.OnInteractiveObjectFieldClicked != null) { this.OnInteractiveObjectFieldClicked.Invoke(MouseDownEvent, this); }
    }

    private void OnMouseEnter(MouseEnterEvent MouseEnterEvent)
    {
        if (!this.IsSelected.GetValue())
        {
            this.style.backgroundColor = Color.gray;
        }
    }

    private void OnMouseExit(MouseOutEvent MouseOutEvent)
    {
        if (!this.IsSelected.GetValue())
        {
            this.style.backgroundColor = this.InitialBackGroundColor;
        }
    }

    private void OnInteractiveObjectSelected()
    {
        this.style.backgroundColor = Color.cyan;
    }

    private void OnInteractiveObjetDeSelected()
    {
        this.style.backgroundColor = this.InitialBackGroundColor;
    }
}

class SelectedInteractiveObjectDetail : VisualElement
{
    public ListenedObjectField CurrentInteracitveObjectFieldSelected { get; private set; }
    private VisualElement CurrentElement;
    private List<IListenableVisualElement> CurrentIListenableVisualElementRefrerences = new List<IListenableVisualElement>();

    public SelectedInteractiveObjectDetail(VisualElement parent)
    {
        parent.Add(this);
        this.style.flexGrow = 2f;
    }

    public void OnGui()
    {
        foreach (var IListenableVisualElement in this.CurrentIListenableVisualElementRefrerences)
        {
            IListenableVisualElement.Refresh();
        }
    }

    public void ResetElement()
    {
        this.CurrentInteracitveObjectFieldSelected = null;
        this.CurrentIListenableVisualElementRefrerences.Clear();
        if (this.CurrentElement != null)
        {
            this.Remove(this.CurrentElement);
            this.CurrentElement = null;
        }
    }

    public void OnInteractiveObjectSelected(ListenedObjectField interactiveObjectField)
    {
        this.ResetElement();
        this.CurrentInteracitveObjectFieldSelected = interactiveObjectField;

        var elem = VisualElementFromClass.BuildVisualElement(interactiveObjectField.ListenedObjectRef, ref this.CurrentIListenableVisualElementRefrerences);
        this.Add(elem);
        this.CurrentElement = elem;
    }
}

class NoSpaceToggle : Toggle
{
    public NoSpaceToggle(VisualElement parent, string label = "")
    {
        this.label = label;
        if (!string.IsNullOrEmpty(label))
        {
            this.Q<Label>().style.minWidth = 0f;
        }
        parent.Add(this);
    }
}