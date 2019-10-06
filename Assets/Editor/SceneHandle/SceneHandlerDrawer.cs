using CoreGame;
using Editor_MainGameCreationWizard;
using RTPuzzle;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class SceneHandlerDrawer
{
    public static void Draw(object drawableObject, Transform objectTransform, CommonGameConfigurations CommonGameConfigurations, IObjectGizmoDisplayEnableArea IObjectGizmoDisplayEnableArea)
    {
        var fields = drawableObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (fields != null && fields.Length > 0)
        {
            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                var DrawConfigurationAttribute = field.GetCustomAttribute<DrawConfigurationAttribute>() as DrawConfigurationAttribute;
                if (DrawConfigurationAttribute != null)
                {
                    var configurationAsset = CommonGameConfigurations.GetConfiguration(DrawConfigurationAttribute.ConfigurationType);
                    if (configurationAsset != null)
                    {
                        configurationAsset.GetEntryTry((Enum)field.GetValue(drawableObject), out ScriptableObject configurationDataObject);
                        if (configurationDataObject != null)
                        {
                            Draw(configurationDataObject, objectTransform, CommonGameConfigurations, IObjectGizmoDisplayEnableArea);
                        }
                    }
                }

                var DrawDefinitionAttribute = field.GetCustomAttribute<DrawDefinitionAttribute>() as DrawDefinitionAttribute;
                if (DrawDefinitionAttribute != null)
                {
                    var configurationAsset = CommonGameConfigurations.GetConfiguration(DrawDefinitionAttribute.ConfigurationType);
                    if (configurationAsset != null)
                    {
                        configurationAsset.GetEntryTry((Enum)field.GetValue(drawableObject), out ScriptableObject configurationDataObject);
                        if (configurationDataObject != null)
                        {
                            var AbstractObjectDefinitionConfigurationInherentData = ((AbstractObjectDefinitionConfigurationInherentData)configurationDataObject);
                            foreach (var RangeDefinitionModulesActivation in AbstractObjectDefinitionConfigurationInherentData.RangeDefinitionModulesActivation)
                            {
                                if (RangeDefinitionModulesActivation.Value)
                                {
                                    Draw(AbstractObjectDefinitionConfigurationInherentData.RangeDefinitionModules[RangeDefinitionModulesActivation.Key], objectTransform, CommonGameConfigurations, IObjectGizmoDisplayEnableArea);
                                }
                            }
                        }
                    }
                }

                var DrawNestedAttribute = field.GetCustomAttribute<DrawNestedAttribute>() as DrawNestedAttribute;
                if (DrawNestedAttribute != null)
                {
                    Draw(field.GetValue(drawableObject), objectTransform, CommonGameConfigurations, IObjectGizmoDisplayEnableArea);
                }

                var AbstractSceneHandleAttribute = field.GetCustomAttribute<AbstractSceneHandleAttribute>(true) as AbstractSceneHandleAttribute;
                if (AbstractSceneHandleAttribute != null)
                {
                    if (AbstractSceneHandleAttribute.GetType() == typeof(WireArcAttribute))
                    {
                        var WireArcAttribute = (WireArcAttribute)AbstractSceneHandleAttribute;
                        float semiAngle = GetFieldValue<float>(drawableObject, IObjectGizmoDisplayEnableArea, field);

                        SetupColors(WireArcAttribute.GetColor());

                        DrawLabel(field.Name, WireArcAttribute.Radius, objectTransform);
                        Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, semiAngle, WireArcAttribute.Radius);
                        Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, -semiAngle, WireArcAttribute.Radius);
                    }
                    else if (AbstractSceneHandleAttribute.GetType() == typeof(WireCircleAttribute))
                    {
                        var WireCircleAttribute = (WireCircleAttribute)AbstractSceneHandleAttribute;

                        float radius = GetFieldValue<float>(drawableObject, IObjectGizmoDisplayEnableArea, field);

                        SetupColors(WireCircleAttribute.GetColor());

                        DrawLabel(field.Name, radius, objectTransform);
                        DrawLabel(field.Name, radius, objectTransform);
                        Handles.DrawWireDisc(objectTransform.position, objectTransform.up, radius);
                    }
                    else if (AbstractSceneHandleAttribute.GetType() == typeof(WireBoxAttribute))
                    {
                        var WireBoxAttribute = (WireBoxAttribute)AbstractSceneHandleAttribute;

                        var center = (Vector3)drawableObject.GetType().GetField(WireBoxAttribute.CenterFieldName).GetValue(drawableObject);
                        var size = (Vector3)drawableObject.GetType().GetField(WireBoxAttribute.SizeFieldName).GetValue(drawableObject);

                        SetupColors(WireBoxAttribute.GetColor());
                        HandlesHelper.DrawBox(center, size, objectTransform, WireBoxAttribute.GetColor(), drawableObject.GetType().Name, MyEditorStyles.SceneDrawDynamicLabelStyle);
                    }
                    else if (AbstractSceneHandleAttribute.GetType() == typeof(WireFrustumAttribute))
                    {
                        var WireFrustumAttribute = (WireFrustumAttribute)AbstractSceneHandleAttribute;
                        var frustum = (FrustumV2)field.GetValue(drawableObject);
                        SetupColors(WireFrustumAttribute.GetColor());
                        DrawFrustum(frustum, objectTransform, isRounded: false);
                    }
                    else if (AbstractSceneHandleAttribute.GetType() == typeof(WireRoundedFrustumAttribute))
                    {
                        var WireRoundedFrustumAttribute = (WireRoundedFrustumAttribute)AbstractSceneHandleAttribute;
                        var frustum = (FrustumV2)field.GetValue(drawableObject);
                        SetupColors(WireRoundedFrustumAttribute.GetColor());
                        DrawFrustum(frustum, objectTransform, isRounded: true);
                    }
                }
            }
        }
    }

    private static T GetFieldValue<T>(object drawableObject, IObjectGizmoDisplayEnableArea IObjectGizmoDisplayEnableArea, FieldInfo field)
    {
        T value = default(T);
        if (typeof(IByEnumProperty).IsAssignableFrom(field.GetValue(drawableObject).GetType()))
        {
            IByEnumProperty IByEnumProperty = (IByEnumProperty)field.GetValue(drawableObject);
            object objectValue;
            IByEnumProperty.TryGetValue(IObjectGizmoDisplayEnableArea.GetEnumParameter(IByEnumProperty.GetType()), out objectValue);
            if (objectValue != null)
            {
                value = (T)objectValue;
            }
        }
        else
        {
            value = (T)field.GetValue(drawableObject);
        }

        return value;
    }

    private static void SetupColors(Color color)
    {
        Handles.color = color;
        MyEditorStyles.SceneDrawDynamicLabelStyle.normal.textColor = color;
    }

    private static void DrawLabel(string label, float height, Transform objectTransform)
    {
        Handles.Label(objectTransform.position + Vector3.up * height, label, MyEditorStyles.SceneDrawDynamicLabelStyle);
    }

    private static void DrawFrustum(FrustumV2 frustum, Transform transform, bool isRounded)
    {
        //TODO
        /*
        frustum.SetCalculationDataForFaceBasedCalculation(transform.position, transform.rotation, transform.lossyScale);
        var FrustumPointsPositions = frustum.FrustumPointsPositions;

        var oldGizmoColor = Handles.color;
        Handles.color = MyColors.HotPink;
        DrawFace(FrustumPointsPositions.FC1, FrustumPointsPositions.FC2, FrustumPointsPositions.FC3, FrustumPointsPositions.FC4);
        DrawFace(FrustumPointsPositions.FC1, FrustumPointsPositions.FC5, FrustumPointsPositions.FC6, FrustumPointsPositions.FC2);
        DrawFace(FrustumPointsPositions.FC2, FrustumPointsPositions.FC6, FrustumPointsPositions.FC7, FrustumPointsPositions.FC3);
        DrawFace(FrustumPointsPositions.FC3, FrustumPointsPositions.FC7, FrustumPointsPositions.FC8, FrustumPointsPositions.FC4);
        DrawFace(FrustumPointsPositions.FC4, FrustumPointsPositions.FC8, FrustumPointsPositions.FC5, FrustumPointsPositions.FC1);
        DrawFace(FrustumPointsPositions.FC5, FrustumPointsPositions.FC6, FrustumPointsPositions.FC7, FrustumPointsPositions.FC8);

        if (isRounded)
        {
            Handles.DrawWireDisc(transform.position, transform.rotation * Vector3.up, frustum.F2.FaceOffsetFromCenter.z / 2f);
        }

        Handles.color = oldGizmoColor;
        */
    }

    private static void DrawFace(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
    {
        Handles.DrawLine(C1, C2);
        Handles.DrawLine(C2, C3);
        Handles.DrawLine(C3, C4);
        Handles.DrawLine(C4, C1);
    }
}
