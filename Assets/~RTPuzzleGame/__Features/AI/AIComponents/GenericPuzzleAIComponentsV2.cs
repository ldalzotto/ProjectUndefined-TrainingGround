using System;
using System.Collections.Generic;
using System.Linq;

namespace RTPuzzle
{
    [System.Serializable]
    public class GenericPuzzleAIComponentsV2 : AbstractObjectDefinitionConfigurationInherentData
    {
        private static List<Type> AbstractAIComponentTypes;

        private List<Type> GetAbstractAIComponentTypes()
        {
            if (AbstractAIComponentTypes == null)
            {
                AbstractAIComponentTypes = typeof(AbstractAIComponent)
                    .Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(AbstractAIComponent)) && !t.IsAbstract)
                    .ToList();
            }
            return AbstractAIComponentTypes;
        }

        public override List<Type> ModuleTypes => GetAbstractAIComponentTypes();
    }
}

