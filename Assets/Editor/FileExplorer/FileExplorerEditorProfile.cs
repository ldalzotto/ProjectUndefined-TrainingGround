using UnityEngine;
using System.Collections;
using OdinSerializer;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class FileExplorerEditorProfile : SerializedScriptableObject
{
    public Vector2 ScrollPosition;
    public Dictionary<string, DefaultAsset> FoundedFolders = new Dictionary<string, DefaultAsset>();
}
