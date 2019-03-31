using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ConfigurationEditor
{
    public class ConfigurationEditorUndoHelper
    {
        public static MultipleChoiceHeaderTab<IGenericConfigurationEditor> ConfigurationEditorProfile;
        public const string UNDO_KEYWORD = "ConfigurationEditorUndoHelper";
        public static void RecordUndo()
        {
            Undo.RecordObject(ConfigurationEditorProfile, UNDO_KEYWORD);
        }

    }
}
