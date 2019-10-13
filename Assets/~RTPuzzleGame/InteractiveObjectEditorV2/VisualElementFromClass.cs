using InteractiveObjectTest;
using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementFromClass
{
    public static VisualElement BuildVisualElement(object obj, ref List<IListenableVisualElement> CreatedIListenableVisualElements)
    {
        var root = new VisualElement();
        foreach (var member in obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (member.MemberType == MemberTypes.Field || (member.MemberType == MemberTypes.Property && ((PropertyInfo)member).GetGetMethod() != null))
            {
                bool continueLoop = false;
                foreach (var customAttribute in member.GetCustomAttributes<A_VEAttribute>())
                {
                    switch (customAttribute)
                    {
                        case VE_Nested vE_Nested:
                            var childElement = VisualElementFromClass.BuildVisualElement(GetMemberInfoValue(member, obj), ref CreatedIListenableVisualElements);
                            var element = new FoldableElement(childElement, root);
                            element.text = member.Name;
                            root.Add(element);
                            break;
                        case VE_Ignore vE_Ignore:
                            continueLoop = true;
                            break;
                    }

                    if (continueLoop) { break; }
                }

                if (continueLoop) { continue; }

                IListenableVisualElement IListenableVisualElement = null;

                if (GetMemberInfoFieldOrPropertyType(member) == typeof(Vector3))
                {
                    IListenableVisualElement = new Vector3ListenableField(obj, member, SanitizeFieldName(member.Name));
                }
                else if (GetMemberInfoFieldOrPropertyType(member) == typeof(bool))
                {
                    IListenableVisualElement = new BoolListenableField(obj, member, SanitizeFieldName(member.Name));
                }
                else if (GetMemberInfoFieldOrPropertyType(member) == typeof(float))
                {
                    IListenableVisualElement = new FloatListenableField(obj, member, SanitizeFieldName(member.Name));
                }
                else if (GetMemberInfoFieldOrPropertyType(member) == typeof(BoolVariable))
                {
                    var boolVariable = (BoolVariable)GetMemberInfoValue(member, obj);
                    var f = boolVariable.GetType().GetField("Value", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    IListenableVisualElement = new BoolListenableField(boolVariable, f, SanitizeFieldName(member.Name));
                }
                else if (typeof(CoreInteractiveObject).IsAssignableFrom(GetMemberInfoFieldOrPropertyType(member)))
                {
                    IListenableVisualElement = new CoreInteractiveObjectListenableField(obj, member);
                }
                else if (typeof(RangeObjectV2).IsAssignableFrom(GetMemberInfoFieldOrPropertyType(member)))
                {
                    IListenableVisualElement = new RangeObjectV2ListenableField(obj, member);
                }

                if (IListenableVisualElement != null)
                {
                    CreatedIListenableVisualElements.Add(IListenableVisualElement);
                    root.Add((VisualElement)IListenableVisualElement);
                }
            }
        }

        return root;
    }

    public static object GetMemberInfoValue(MemberInfo memberInfo, object obj)
    {
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo)memberInfo).GetValue(obj);
            case MemberTypes.Property:
                return ((PropertyInfo)memberInfo).GetValue(obj);
        }
        return null;
    }

    public static Type GetMemberInfoFieldOrPropertyType(MemberInfo memberInfo)
    {
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo)memberInfo).FieldType;
            case MemberTypes.Property:
                return ((PropertyInfo)memberInfo).PropertyType;
        }
        return null;
    }

    public static string SanitizeFieldName(string rawFieldName)
    {
        return rawFieldName.Replace("<", "").Replace(">", "").Replace("k__BackingField", "");
    }
}

public abstract class A_VEAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class VE_Nested : A_VEAttribute { }
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class VE_Ignore : A_VEAttribute { }

public interface IListenableVisualElement
{
    void Refresh();
}
public abstract class ListenableVisualElement<T> : VisualElement, IListenableVisualElement
{
    private object obj;
    private MemberInfo member;
    protected T value;

    protected ListenableVisualElement(object obj, MemberInfo member)
    {
        this.obj = obj;
        this.member = member;
    }

    public void Refresh()
    {
        var fieldValue = VisualElementFromClass.GetMemberInfoValue(this.member, obj);
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

    public Vector3ListenableField(object obj, MemberInfo member, string label = "") : base(obj, member)
    {
        this.Vector3Field = new Vector3Field(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(member.Name) : label);
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

    public BoolListenableField(object obj, MemberInfo member, string label = "") : base(obj, member)
    {
        this.style.flexDirection = FlexDirection.Row;
        this.Text = new Label(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(member.Name) : label);
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

    public FloatListenableField(object obj, MemberInfo member, string label = "") : base(obj, member)
    {
        this.FloatField = new FloatField(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(member.Name) : label);
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

    public CoreInteractiveObjectListenableField(object obj, MemberInfo member, string label = "") : base(obj, member)
    {
        this.ObjectField = new ObjectField(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(member.Name) : label);
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

    public RangeObjectV2ListenableField(object obj, MemberInfo member) : base(obj, member)
    {
        this.ObjectField = new ObjectField(VisualElementFromClass.SanitizeFieldName(member.Name));
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

    public GameObjectListenableField(object obj, MemberInfo member, string label = "") : base(obj, member)
    {
        this.ObjectField = new ObjectField(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(member.Name) : label);
        this.Add(this.ObjectField);
    }

    protected override void OnValueChaged()
    {
        this.ObjectField.value = this.value;
    }
}