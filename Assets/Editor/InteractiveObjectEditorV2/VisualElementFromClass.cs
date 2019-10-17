using InteractiveObjects;
using RTPuzzle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementFromClass
{
    public static VisualElement BuildVisualElement(object obj, ref List<IListenableVisualElement> CreatedIListenableVisualElements)
    {
        var root = new VisualElement();

        foreach (var field in ReflectionHelper.GetAllFields(obj.GetType()))
        {
            bool continueLoop = false;
            foreach (var customAttribute in field.GetCustomAttributes<A_VEAttribute>())
            {
                switch (customAttribute)
                {
                    case VE_Nested vE_Nested:
                        var childElement = VisualElementFromClass.BuildVisualElement(field.GetValue(obj), ref CreatedIListenableVisualElements);
                        var element = new FoldableElement(childElement, root);
                        element.text = field.Name;
                        root.Add(element);
                        break;
                    case VE_Ignore vE_Ignore:
                        continueLoop = true;
                        break;
                }

                if (continueLoop) { break; }
            }

            if (continueLoop) { continue; }

            IListenableVisualElement IListenableVisualElement = BuildIListenableVisualElementFromMember(obj, field);

            if (IListenableVisualElement != null)
            {
                CreatedIListenableVisualElements.Add(IListenableVisualElement);
                root.Add((VisualElement)IListenableVisualElement);
            }
        }

        return root;
    }

    public static IListenableVisualElement BuildIListenableVisualElementFromMember(object obj, FieldInfo field)
    {
        IListenableVisualElement IListenableVisualElement = null;

        if (field.FieldType == typeof(Vector3))
        {
            IListenableVisualElement = new Vector3ListenableField(obj, field, SanitizeFieldName(field.Name));
        }
        else if (field.FieldType == typeof(bool))
        {
            IListenableVisualElement = new BoolListenableField(obj, field, SanitizeFieldName(field.Name));
        }
        else if (field.FieldType == typeof(float))
        {
            IListenableVisualElement = new FloatListenableField(obj, field, SanitizeFieldName(field.Name));
        }
        else if (field.FieldType == typeof(BoolVariable))
        {
            var boolVariable = (BoolVariable)field.GetValue(obj);
            var f = boolVariable.GetType().GetField("Value", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            IListenableVisualElement = new BoolListenableField(boolVariable, f, SanitizeFieldName(field.Name));
        }
        else if (typeof(CoreInteractiveObject).IsAssignableFrom(field.FieldType))
        {
            IListenableVisualElement = new CoreInteractiveObjectListenableField(obj, field);
        }
        else if (typeof(RangeObjectV2).IsAssignableFrom(field.FieldType))
        {
            IListenableVisualElement = new RangeObjectV2ListenableField(obj, field);
        }
        else if (field.GetCustomAttribute<VE_Array>() != null)
        {
            IListenableVisualElement = new IEnumerableListenableField(obj, field);
        }

        return IListenableVisualElement;
    }

    public static void RemoveAllIListenableVisualElementNested(VisualElement element, ref List<IListenableVisualElement> removedElements)
    {
        List<VisualElement> elementsToRemove = null;
        var childrenEnumerator = element.Children().GetEnumerator();
        int i = 0;
        while (childrenEnumerator.MoveNext())
        {
            if (childrenEnumerator.Current.childCount > 0)
            {
                RemoveAllIListenableVisualElementNested(childrenEnumerator.Current, ref removedElements);
            }

            if (typeof(IListenableVisualElement).IsAssignableFrom(childrenEnumerator.Current.GetType()))
            {
                if (elementsToRemove == null) { elementsToRemove = new List<VisualElement>(); }
                elementsToRemove.Add(childrenEnumerator.Current);
            }
            i += 1;
        }

        if (elementsToRemove != null)
        {
            foreach (var elementToRemove in elementsToRemove)
            {
                if (removedElements == null) { removedElements = new List<IListenableVisualElement>(); }
                removedElements.Add(elementToRemove as IListenableVisualElement);
                element.Remove(elementToRemove);
            }
        }
    }

    public static string SanitizeFieldName(string rawFieldName)
    {
        return rawFieldName.Replace("<", "").Replace(">", "").Replace("k__BackingField", "");
    }
}

public interface IListenableVisualElement
{
    void Refresh();
}
public abstract class ListenableVisualElement<T> : VisualElement, IListenableVisualElement
{
    protected object obj;
    protected FieldInfo field;
    protected T value;

    protected ListenableVisualElement(object obj, FieldInfo field)
    {
        this.obj = obj;
        this.field = field;
    }

    public void Refresh()
    {
        var fieldValue = this.field.GetValue(obj);
        if (fieldValue != null)
        {
            this.value = (T)fieldValue;
            this.OnValueChaged();
        }

    }

    protected abstract void OnValueChaged();
}

class FoldableElement : Foldout
{
    private VisualElement InnerElement;
    public FoldableElement(VisualElement innerElement, VisualElement parent)
    {
        this.value = false;
        this.InnerElement = innerElement;
        this.InnerElement.style.marginLeft = parent.style.marginLeft.value.value + 5f;
        parent.Add(this);
        this.RegisterCallback<ChangeEvent<bool>>(this.OnFoldableChange);
        this.Add(innerElement);
    }

    private void OnFoldableChange(ChangeEvent<bool> evt)
    {
        this.InnerElement.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        evt.StopPropagation();
    }
}

class Vector3ListenableField : ListenableVisualElement<Vector3>
{
    private Vector3Field Vector3Field;

    public Vector3ListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        this.Vector3Field = new Vector3Field(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        this.Add(this.Vector3Field);
    }

    protected override void OnValueChaged()
    {
        this.Vector3Field.value = this.value;
    }
}
class BoolListenableField : ListenableVisualElement<bool>
{
    private VisualElement Text;
    private Toggle Toggle;

    public BoolListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        this.style.flexDirection = FlexDirection.Row;
        this.Text = new Label(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        this.Add(this.Text);
        this.Toggle = new Toggle();
        this.Add(this.Toggle);
    }

    protected override void OnValueChaged()
    {
        this.Toggle.value = this.value;
    }
}
class FloatListenableField : ListenableVisualElement<float>
{
    private FloatField FloatField;

    public FloatListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        this.FloatField = new FloatField(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        this.Add(this.FloatField);
    }

    protected override void OnValueChaged()
    {
        this.FloatField.value = this.value;
    }
}
class CoreInteractiveObjectListenableField : ListenableVisualElement<CoreInteractiveObject>
{
    private ObjectField ObjectField;

    public CoreInteractiveObjectListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        this.ObjectField = new ObjectField(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        this.Add(this.ObjectField);
    }

    protected override void OnValueChaged()
    {
        this.ObjectField.value = this.value.InteractiveGameObject.InteractiveGameObjectParent;
    }
}
class RangeObjectV2ListenableField : ListenableVisualElement<RangeObjectV2>
{
    private ObjectField ObjectField;

    public RangeObjectV2ListenableField(object obj, FieldInfo field) : base(obj, field)
    {
        this.ObjectField = new ObjectField(VisualElementFromClass.SanitizeFieldName(field.Name));
        this.Add(this.ObjectField);
    }

    protected override void OnValueChaged()
    {
        this.ObjectField.value = this.value.RangeGameObjectV2.RangeGameObject;
    }
}
class GameObjectListenableField : ListenableVisualElement<GameObject>
{
    private ObjectField ObjectField;

    public GameObjectListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        this.ObjectField = new ObjectField(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        this.Add(this.ObjectField);
    }

    protected override void OnValueChaged()
    {
        this.ObjectField.value = this.value;
    }
}
class IEnumerableListenableField : ListenableVisualElement<IEnumerable>
{
    private VisualElement RootElement;
    private Dictionary<object, VisualElement> EnumerableElements = new Dictionary<object, VisualElement>();
    private List<IListenableVisualElement> listenableVisualElements = new List<IListenableVisualElement>();

    private List<object> ValueIenumerableAsList = new List<object>();

    public IEnumerableListenableField(object obj, FieldInfo field) : base(obj, field)
    {
        this.RootElement = new VisualElement();
        this.RootElement.style.marginLeft = 5f;
        this.Add(this.RootElement);
    }

    private void ProcessIEnumerable(IEnumerable IEnumerable)
    {
        this.ValueIenumerableAsList.Clear();
        var enumerator = IEnumerable.GetEnumerator();
        int i = 0;
        while (enumerator.MoveNext())
        {
            var obj = enumerator.Current;
            this.ValueIenumerableAsList.Add(obj);
            if (!this.EnumerableElements.ContainsKey(obj))
            {
                var ve = VisualElementFromClass.BuildVisualElement(obj, ref this.listenableVisualElements);
                var foldable = new FoldableElement(ve, this.RootElement);
                foldable.text = this.field.Name + "_" + i;
                this.EnumerableElements.Add(obj, foldable);
            }
            i += 1;
        }

        foreach (var EnumerableElement in this.EnumerableElements.Keys.ToList())
        {
            if (!this.ValueIenumerableAsList.Contains(EnumerableElement))
            {
                List<IListenableVisualElement> RemovedIListenableVisualElements = null;
                VisualElementFromClass.RemoveAllIListenableVisualElementNested(this.EnumerableElements[EnumerableElement], ref RemovedIListenableVisualElements);
                if (RemovedIListenableVisualElements != null)
                {
                    foreach (var RemovedIListenableVisualElement in RemovedIListenableVisualElements)
                    {
                        this.listenableVisualElements.Remove(RemovedIListenableVisualElement);
                    }
                }

                this.RootElement.Remove(this.EnumerableElements[EnumerableElement]);
                this.EnumerableElements.Remove(EnumerableElement);
            }
        }
    }

    protected override void OnValueChaged()
    {
        this.ProcessIEnumerable(this.value);
        foreach (var listenableVisualElement in this.listenableVisualElements)
        {
            listenableVisualElement.Refresh();
        }
    }
}