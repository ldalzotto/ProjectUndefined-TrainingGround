using InteractiveObjectTest;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractiveObjectExplorer : EditorWindow
{
    [MenuItem("InteractiveObject/InteractiveObjectExplorer")]
    public static void Init()
    {
        var wnd = GetWindow<InteractiveObjectExplorer>();
        wnd.Show();
    }

    private VisualElement RootElement;
    private TextField SearchTextField;

    private List<A_InteractiveObjectInitializer> InteractiveObjectInitializers = new List<A_InteractiveObjectInitializer>();
    private Dictionary<A_InteractiveObjectInitializer, InterativeObjectInitializerLine> InteractiveObjectInitializerLines = new Dictionary<A_InteractiveObjectInitializer, InterativeObjectInitializerLine>();

    private void OnEnable()
    {
        Debug.Log("InteractiveObjectExplorer OnEnable");
        this.RootElement = new VisualElement();

        this.SearchTextField = new TextField();
        this.SearchTextField.RegisterCallback<ChangeEvent<string>>(this.OnSearchStringChange);
        this.RootElement.Add(this.SearchTextField);

        rootVisualElement.Add(this.RootElement);
        SceneView.duringSceneGui += this.SceneTick;
    }

    private void OnDisable()
    {
        Debug.Log("InteractiveObjectExplorer OnDisable");
        SceneView.duringSceneGui += this.SceneTick;
    }

    private void OnGUI()
    {
        if (Event.current.rawType == EventType.ContextClick)
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Refresh"), true, this.Refresh);
            genericMenu.ShowAsContext();
        }
    }

    private void SceneTick(SceneView sceneView)
    {
        foreach (var InteractiveObjectInitializerLine in InteractiveObjectInitializerLines)
        {
            if (InteractiveObjectInitializerLine.Value.IsGizmoSelected())
            {
                SceneHandlerDrawer.Draw(InteractiveObjectInitializerLine.Key, InteractiveObjectInitializerLine.Key.transform);
            }
        }
    }

    private void OnSearchStringChange(ChangeEvent<string> evt)
    {

    }

    private void Refresh()
    {
        this.InteractiveObjectInitializers = GameObject.FindObjectsOfType<A_InteractiveObjectInitializer>().ToList();

        foreach (var interactiveObjectInitializer in this.InteractiveObjectInitializers)
        {
            if (!this.InteractiveObjectInitializerLines.ContainsKey(interactiveObjectInitializer))
            {
                this.InteractiveObjectInitializerLines[interactiveObjectInitializer] = new InterativeObjectInitializerLine(this.RootElement, interactiveObjectInitializer.gameObject, this.OnInteractiveObjectLineClicked);
            }
        }

        foreach (var interactiveObjectInitializerKey in this.InteractiveObjectInitializerLines.Keys.ToList())
        {
            if (!this.InteractiveObjectInitializers.Contains(interactiveObjectInitializerKey))
            {
                this.RootElement.Remove(this.InteractiveObjectInitializerLines[interactiveObjectInitializerKey]);
                this.InteractiveObjectInitializerLines.Remove(interactiveObjectInitializerKey);
            }
        }
    }

    private void OnInteractiveObjectLineClicked(InterativeObjectInitializerLine InterativeObjectInitializerLine)
    {
        foreach(var interactiveObjectLine in this.InteractiveObjectInitializerLines.Values)
        {
            interactiveObjectLine.IsSelected.SetValue(interactiveObjectLine == InterativeObjectInitializerLine);
        }
    }

}

class InterativeObjectInitializerLine : VisualElement
{
    public BoolVariable IsSelected { get; private set; }
    private Color InitialBackGroundColor;
    private Color SelectedBackgroundColor;

    private Action<InterativeObjectInitializerLine> OnClickedExtern;
    private GameObject GameObjectReference;

    private ObjectFieldSelectionIcon GizmoIcon;
    private Label Label;

    public bool IsGizmoSelected() { return this.GizmoIcon.Selected.GetValue(); }

    public InterativeObjectInitializerLine(VisualElement parent, GameObject GameObject, Action<InterativeObjectInitializerLine> OnClickedExtern)
    {
        this.GameObjectReference = GameObject;
        this.OnClickedExtern = OnClickedExtern;
        this.IsSelected = new BoolVariable(false, this.OnSelected, this.OnUnSelected);

        this.style.flexDirection = FlexDirection.Row;
        this.GizmoIcon = new ObjectFieldSelectionIcon(this, "G");
        this.Label = new Label(GameObject.name);
        this.Add(this.Label);
        parent.Add(this);
        
        this.RegisterCallback<MouseEnterEvent>(this.OnMouseEnter);
        this.RegisterCallback<MouseOutEvent>(this.OnMouseExit);
        this.RegisterCallback<MouseDownEvent>(this.OnMouseDown);
    }

    private void OnMouseDown(MouseDownEvent MouseDownEvent)
    {
        Selection.activeGameObject = this.GameObjectReference;
        this.OnClickedExtern.Invoke(this);
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

    private void OnSelected()
    {
        this.style.backgroundColor = Color.cyan;
    }

    private void OnUnSelected()
    {
        this.style.backgroundColor = this.InitialBackGroundColor;
    }

}
