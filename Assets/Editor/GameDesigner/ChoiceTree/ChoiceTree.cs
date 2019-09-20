using ConfigurationEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Editor_GameDesigner
{
    public class GameDesignerChoiceTree : TreePicker<Type>
    {
        private Dictionary<string, Type> configurations;

        public override Dictionary<string, Type> Configurations => this.configurations;

        private GameDesignerEditorProfile GameDesignerEditorProfileRef;

        public GameDesignerChoiceTree(GameDesignerEditorProfile gameDesignerEditorProfileRef)
        {
            GameDesignerEditorProfileRef = gameDesignerEditorProfileRef;
        }

        protected override void OnSelectionChange()
        {
            base.OnSelectionChange();
            this.GameDesignerEditorProfileRef.ChangeCurrentModule((IGameDesignerModule)Activator.CreateInstance(this.GetSelectedConf()));
        }

        private void OnEnable()
        {
            if (this.configurations == null) { this.configurations = new Dictionary<string, Type>(); }
            else { this.configurations.Clear(); }
            this.InitConfigurationModules();
            this.InitOtherGameDesignerModules();
        }

        private void InitConfigurationModules()
        {
            var configurationSerializationClasses =
                typeof(IConfigurationSerialization)
                        .Assembly.GetTypes()
                        .Union(typeof(RTPuzzle.GameManager).Assembly.GetTypes())
                        .Union(typeof(AdventureGame.GameManager).Assembly.GetTypes())
                        .Where(t => typeof(IConfigurationSerialization).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

            foreach (var configurationSerializationClass in configurationSerializationClasses)
            {
                //ConfigurationModule
                var configurationIDType = configurationSerializationClass.BaseType.GetGenericArguments()[0];
                var inherentDataType = configurationSerializationClass.BaseType.GetGenericArguments()[1];

                var configurationModuleType = typeof(ConfigurationModule<,,>);
                configurationModuleType = configurationModuleType.MakeGenericType(new Type[] { configurationSerializationClass, configurationIDType, inherentDataType });

                this.configurations.Add("Configuration.//" + configurationSerializationClass.Name + "Module", configurationModuleType);
            }
        }

        private void InitOtherGameDesignerModules()
        {
            var gameDesginerModuleClasses =
              this.GetType()
                      .Assembly.GetTypes()
                      .Where(t => typeof(IGameDesignerModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

            var modulesDirectory = Directory.GetDirectories(Application.dataPath, "GameDesigner", SearchOption.AllDirectories)[0];

            foreach (var gameDesginerModuleClass in gameDesginerModuleClasses)
            {
                var moduleFiles = Directory.GetFiles(modulesDirectory, gameDesginerModuleClass.Name + ".cs", SearchOption.AllDirectories);

                if (moduleFiles != null && moduleFiles.Length > 0)
                {
                    this.configurations.Add(moduleFiles[0].Replace(modulesDirectory, "")
                                        .Replace("\\Modules\\", "")
                                        .Replace("\\", ".//")
                                        .Replace(".cs", "")
                                        , gameDesginerModuleClass);
                }

            }
        }
    }

}
