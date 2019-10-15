using CoreGame;
using Editor_MainGameCreationWizard;
using RTPuzzle;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class SceneHandlerDrawer
{
    public static void Draw(object drawableObject, Transform objectTransform, CommonGameConfigurations CommonGameConfigurations)
    {
        if (drawableObject.GetType().GetCustomAttribute<SceneHandleDrawAttribute>(true) != null)
        {
            var fields = ReflectionHelper.GetAllFields(drawableObject.GetType());
            foreach (var field in fields)
            {
                var DrawConfigurationAttribute = field.GetCustomAttribute<DrawConfigurationAttribute>() as DrawConfigurationAttribute;
                if (DrawConfigurationAttribute != null)
                {
                    var configurationAsset = CommonGameConfigurations.GetConfiguration(DrawConfigurationAttribute.ConfigurationType);
                    if (configurationAsset != null)
                    {
                        configurationAsset.GetEntryTry((Enum)field.GetValue(drawableObject), out ScriptableObject configurationDataObject);
                        if (configurationDataObject != null)
                        {
                            Draw(configurationDataObject, objectTransform, CommonGameConfigurations);
                        }
                    }
                }
                /*
                  var DrawConfigurationAttribute = field.GetCustomAttribute<DrawConfigurationAttribute>() as DrawConfigurationAttribute;
                  if (DrawConfigurationAttribute != null)
                  {
                      var configurationAsset = CommonGameConfigurations.GetConfiguration(DrawConfigurationAttribute.ConfigurationType);
                      if (configurationAsset != null)
                      {
                          configurationAsset.GetEntryTry((Enum)field.GetValue(drawableObject), out ScriptableObject configurationDataObject);
                          if (configurationDataObject != null)
                          {
                              Draw(configurationDataObject, objectTransform, CommonGameConfigurations);
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
                                      Draw(AbstractObjectDefinitionConfigurationInherentData.RangeDefinitionModules[RangeDefinitionModulesActivation.Key], objectTransform, CommonGameConfigurations);
                                  }
                              }
                          }
                      }
                  }
                  */

                var DrawNestedAttribute = field.GetCustomAttribute<DrawNestedAttribute>() as DrawNestedAttribute;
                if (DrawNestedAttribute != null)
                {
                    Draw(field.GetValue(drawableObject), objectTransform, CommonGameConfigurations);
                }

                var AbstractSceneHandleAttribute = field.GetCustomAttribute<AbstractSceneHandleAttribute>(true) as AbstractSceneHandleAttribute;
                if (AbstractSceneHandleAttribute != null)
                {
                    if (AbstractSceneHandleAttribute.GetType() == typeof(WireArcAttribute))
                    {
                        var WireArcAttribute = (WireArcAttribute)AbstractSceneHandleAttribute;
                        float semiAngle = GetFieldValue<float>(drawableObject, field);

                        SetupColors(WireArcAttribute.GetColor());

                        DrawLabel(field.Name, WireArcAttribute.Radius, objectTransform);
                        Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, semiAngle, WireArcAttribute.Radius);
                        Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, -semiAngle, WireArcAttribute.Radius);
                    }
                    else if (AbstractSceneHandleAttribute.GetType() == typeof(WireCircleAttribute))
                    {
                        var WireCircleAttribute = (WireCircleAttribute)AbstractSceneHandleAttribute;

                        float radius = GetFieldValue<float>(drawableObject, field);

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

    private static T GetFieldValue<T>(object drawableObject, FieldInfo field)
    {
        T value = default(T);
        /*
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
        */
        value = (T)field.GetValue(drawableObject);
        // }

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
        frustum.CalculateFrustumWorldPositionyFace(out FrustumPointsPositions LocalFrustumPointPositions, new TransformStruct { WorldPosition = Vector3.zero, WorldRotation = Quaternion.identity, LossyScale = Vector3.one });
        var frustumWorldPositions = new RangeFrustumWorldPositioning { LocalFrustumPositions = LocalFrustumPointPositions }.GetWorldFrustumPositions(transform.localToWorldMatrix);
        DrawFace(frustumWorldPositions.FC1, frustumWorldPositions.FC2, frustumWorldPositions.FC3, frustumWorldPositions.FC4);
        DrawFace(frustumWorldPositions.FC1, frustumWorldPositions.FC5, frustumWorldPositions.FC6, frustumWorldPositions.FC2);
        DrawFace(frustumWorldPositions.FC2, frustumWorldPositions.FC6, frustumWorldPositions.FC7, frustumWorldPositions.FC3);
        DrawFace(frustumWorldPositions.FC3, frustumWorldPositions.FC7, frustumWorldPositions.FC8, frustumWorldPositions.FC4);
        DrawFace(frustumWorldPositions.FC4, frustumWorldPositions.FC8, frustumWorldPositions.FC5, frustumWorldPositions.FC1);
        DrawFace(frustumWorldPositions.FC5, frustumWorldPositions.FC6, frustumWorldPositions.FC7, frustumWorldPositions.FC8);

        if (isRounded)
        {
            Handles.DrawWireDisc(transform.position, transform.up, frustum.FaceDistance);
        }
    }

    private static void DrawFace(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
    {
        Handles.DrawLine(C1, C2);
        Handles.DrawLine(C2, C3);
        Handles.DrawLine(C3, C4);
        Handles.DrawLine(C4, C1);
    }
}