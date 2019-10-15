using Editor_MainGameCreationWizard;
using InteractiveObjectTest;
using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractiveObjectDebugger : EditorWindow
{
    public static InteractiveObjectDebugger Instance;

    [MenuItem("InteractiveObject/InteractiveObjectDebugger")]
    public static void Init()
    {
        var wnd = GetWindow<InteractiveObjectDebugger>();
        Instance = wnd;
        wnd.Show();
    }
    private CommonGameConfigurations CommonGameConfigurations;

    private VisualElement RootElement;

    private VisualElement LeftPanel;

    private ScrollView ObjectFieldParent;
    private TextField SeachTextElement;
    private SelectedInteractiveObjectDetail SelectedInteractiveObjectDetail;

    private void OnEnable()
    {
        this.CommonGameConfigurations = new CommonGameConfigurations();
        EditorInformationsHelper.InitProperties(ref this.CommonGameConfigurations);

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
        SceneView.duringSceneGui += this.SceneTick;
    }

    private void OnDestroy()
    {
        Instance = null;
        EditorApplication.playModeStateChanged -= this.OnPlayModeStateChanged;
        EditorApplication.update -= this.Tick;
        SceneView.duringSceneGui -= this.SceneTick;
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
        InteractiveObjectEventsManager.Get().RegisterOnInteractiveObjectCreatedEventListener(this.OnInteractiveObjectCreated);
        InteractiveObjectEventsManager.Get().RegisterOnInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
        RangeEventsManager.Get().RegisterOnRangeObjectCreatedEventListener(this.OnInteractiveObjectCreated);
        RangeEventsManager.Get().RegisterOnRangeObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
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
            foreach (var rangeObject in allRangeObjects)
            {
                this.OnInteractiveObjectCreated(rangeObject);
            }

            this.SelectedInteractiveObjectDetail.OnGui();
        }
    }

    private void SceneTick(SceneView sceneView)
    {
        foreach (var interactiveObjectField in this.ListenableObjectFields.Values)
        {
            interactiveObjectField.SceneTick(this.CommonGameConfigurations);
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
                   .Select(i => i).Where(i => string.IsNullOrEmpty(this.SeachTextElement.text) || i.InteractiveGameObject.InteractiveGameObjectParent.name.ToLower().Contains(this.SeachTextElement.text.ToLower())).ToList()
                   .ConvertAll(i => i.GetType().Name)
                   .Union(
                        allRangeObjects.Select(r => r).Where(r => string.IsNullOrEmpty(this.SeachTextElement.text) || r.RangeGameObjectV2.RangeGameObject.name.ToLower().Contains(this.SeachTextElement.text.ToLower())).ToList()
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
    private ObjectFieldIconBar ObjectFieldIconBar;

    public GameObject ObjectReference { get; private set; }
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

        this.ObjectFieldIconBar = new ObjectFieldIconBar(this);

        this.ObjectLabel = new Label(this.ObjectReference.name);
        this.ObjectLabel.style.marginLeft = 10f;

        this.Add(this.ObjectLabel);


        parent.Add(this);

        this.style.flexDirection = FlexDirection.Row;

        this.RegisterCallback<MouseEnterEvent>(this.OnMouseEnter);
        this.RegisterCallback<MouseOutEvent>(this.OnMouseExit);
        this.RegisterCallback<MouseDownEvent>(this.OnMouseDown);
    }

    public void SetIsSelected(bool value) { this.IsSelected.SetValue(value); }

    public void SceneTick(CommonGameConfigurations CommonGameConfigurations)
    {
        if (this.ObjectFieldIconBar.IsSceneHandleEnabled() && this.ObjectReference != null)
        {
            SceneHandlerDrawer.Draw(this.ListenedObjectRef, this.ObjectReference.transform, CommonGameConfigurations);
        }
    }


    private void OnMouseDown(MouseDownEvent MouseDownEvent)
    {
        Selection.activeGameObject = this.ObjectReference;
        if (this.OnInteractiveObjectFieldClicked != null) { this.OnInteractiveObjectFieldClicked.Invoke(MouseDownEvent, this); }
        MouseDownEvent.StopPropagation();
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

class ObjectFieldIconBar : VisualElement
{
    private VisualElement Root;

    private ObjectFieldSelectionIcon SceneHandleSelection;

    public bool IsSceneHandleEnabled()
    {
        return this.SceneHandleSelection.Selected.GetValue();
    }

    public ObjectFieldIconBar(VisualElement parent)
    {
        this.Root = new VisualElement();
        this.Root.style.alignSelf = Align.FlexEnd;
        this.Root.style.flexDirection = FlexDirection.Row;
        this.SceneHandleSelection = new ObjectFieldSelectionIcon(this.Root, "G");
        this.Add(Root);
        this.style.marginLeft = 5f;
        parent.Add(this);
    }
}

public class ObjectFieldSelectionIcon : VisualElement
{
    private Color initialBackgroundColor;
    public BoolVariable Selected { get; private set; }

    public ObjectFieldSelectionIcon(VisualElement parent, string label)
    {
        this.Selected = new BoolVariable(false, this.OnSelected, this.OnUnSelected);
        this.initialBackgroundColor = this.style.backgroundColor.value;
        var text = new Label(label);
        this.Add(text);
        this.RegisterCallback<MouseDownEvent>(this.OnClicked);
        parent.Add(this);
    }

    private void OnClicked(MouseDownEvent evt)
    {
        this.Selected.SetValue(!this.Selected.GetValue());
        evt.StopPropagation();
    }

    private void OnSelected()
    {
        this.style.backgroundColor = Color.yellow;
    }
    private void OnUnSelected()
    {
        this.style.backgroundColor = this.initialBackgroundColor;
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