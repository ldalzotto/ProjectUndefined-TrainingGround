using InteractiveObjectTest;
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

        EditorApplication.update += this.Tick;
    }

    private void OnDestroy()
    {
        Instance = null;
        EditorApplication.update -= this.Tick;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoaded()
    {
        Debug.Break();
    }

    private Dictionary<CoreInteractiveObject, InteractiveObjectField> InteractiveObjectFields = new Dictionary<CoreInteractiveObject, InteractiveObjectField>();
    private Dictionary<string, VisualElement> ClassHeaderElements = new Dictionary<string, VisualElement>();

    private void Tick()
    {
        if (Application.isPlaying)
        {
            var allInteractiveObjects = InteractiveObjectV2Manager.Get().InteractiveObjects;
            foreach (var interactiveObject in allInteractiveObjects)
            {

                this.InteractiveObjectFields.TryGetValue(interactiveObject, out InteractiveObjectField InteractiveObjectField);
                if (InteractiveObjectField == null)
                {
                    InteractiveObjectField = new InteractiveObjectField(this.ObjectFieldParent, interactiveObject,
                              OnInteractiveObjectFieldClicked: this.OnInteractiveObjectFieldClicked);
                    this.InteractiveObjectFields.Add(interactiveObject, InteractiveObjectField);

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

                    header.Add(this.InteractiveObjectFields[interactiveObject]);
                }
            }

            this.SelectedInteractiveObjectDetail.OnGui();
        }
    }

    private void OnSearchTextChange(ChangeEvent<string> evt)
    {
        if (Application.isPlaying)
        {
            var allInteractiveObjects = InteractiveObjectV2Manager.Get().InteractiveObjects;

            var allInteractiveObjectsClassname = allInteractiveObjects.ToList()
                   .Select(i => i).Where(i => string.IsNullOrEmpty(this.SeachTextElement.text) || i.InteractiveGameObject.InteractiveGameObjectParent.name.Contains(this.SeachTextElement.text)).ToList()
                   .ConvertAll(i => i.GetType().Name).ToList();
            foreach (var classHeaderElement in this.ClassHeaderElements)
            {
                classHeaderElement.Value.style.display = allInteractiveObjectsClassname.Contains(classHeaderElement.Key) ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }

    private void OnInteractiveObjectFieldClicked(MouseDownEvent MouseDownEvent, InteractiveObjectField InteractiveObjectField)
    {
        foreach (var interactiveObjectField in this.InteractiveObjectFields.Values)
        {
            interactiveObjectField.SetIsSelected(interactiveObjectField == InteractiveObjectField);
            this.SelectedInteractiveObjectDetail.OnInteractiveObjectSelected(InteractiveObjectField);
        }
    }
}

class InteractiveObjectField : VisualElement
{
    private BoolVariable IsSelected;

    private Label ObjectLabel;

    private GameObject ObjectReference;
    public CoreInteractiveObject InteractiveObjectRef { get; private set; }

    private Color InitialBackGroundColor;

    private Action<MouseDownEvent, InteractiveObjectField> OnInteractiveObjectFieldClicked;

    public InteractiveObjectField(VisualElement parent, CoreInteractiveObject interactiveObject, Action<MouseDownEvent, InteractiveObjectField> OnInteractiveObjectFieldClicked = null)
    {
        this.InteractiveObjectRef = interactiveObject;
        this.ObjectReference = interactiveObject.InteractiveGameObject.InteractiveGameObjectParent;
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

    public void OnInteractiveObjectSelected(InteractiveObjectField interactiveObjectField)
    {
        this.CurrentIListenableVisualElementRefrerences.Clear();
        if (this.CurrentElement != null)
        {
            this.Remove(this.CurrentElement);
            this.CurrentElement = null;
        }

        var elem = VisualElementFromClass.BuildVisualElement(interactiveObjectField.InteractiveObjectRef, ref this.CurrentIListenableVisualElementRefrerences);
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