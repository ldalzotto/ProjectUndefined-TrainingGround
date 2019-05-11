using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ConfigurationEditor
{
    public class ConfigurationEditorUndoHelper
    {
        public static MultipleChoiceHeaderTab<IGenericConfigurationEditor> ConfigurationEditorProfile;
        public static TreeChoiceHeaderTab<IGenericConfigurationEditor> ConfigurationEditorProfileV2;
        public const string UNDO_KEYWORD = "ConfigurationEditorUndoHelper";
        public static void RecordUndo()
        {
            Undo.RecordObject(ConfigurationEditorProfileV2, UNDO_KEYWORD);
        }

    }
}
