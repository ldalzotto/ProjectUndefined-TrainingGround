using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionMigration : EditorWindow
{
    [MenuItem("Migration/PlayerPositionMigration")]
    static void Init()
    {
        PlayerPositionMigration window = (PlayerPositionMigration)EditorWindow.GetWindow(typeof(PlayerPositionMigration));
        window.Show();
    }

    private Dictionary<string, TransformBinarry> playerPositions;

    private void OnEnable()
    {
        this.playerPositions = new Dictionary<string, TransformBinarry>();
    }

    private const string positionFilePath = "C:\\Users\\Loic\\Documents\\projects\\SoulsLike\\No Entry_0.0.71_Reset\\Assets\\Tests\\Migration\\PlayerPosition\\SavedPosition.txt";
    private RTPuzzle.PlayerManager puzzlePlayerPrefab;

    private void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        if (GUILayout.Button("FIND POSITIONS"))
        {
            this.playerPositions.Clear();
            for (var i = 9; i < 14; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                Debug.Log(scenePath);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var playerObject = GameObject.FindObjectOfType<RTPuzzle.PlayerManager>();
                if (playerObject != null)
                {
                    this.playerPositions.Add(scenePath, new TransformBinarry(playerObject.transform));
                }
            }
        }
        if (GUILayout.Button("SAVE POSITIONS"))
        {
            List<PlayerPositionBinarry> positionsToSave = new List<PlayerPositionBinarry>();
            Debug.Log(this.playerPositions.Count);
            foreach (var playerPosition in this.playerPositions)
            {
                positionsToSave.Add(new PlayerPositionBinarry(playerPosition.Key, playerPosition.Value));
            }


            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fileStream = File.Open(positionFilePath, FileMode.OpenOrCreate))
            {
                binaryFormatter.Serialize(fileStream, positionsToSave);
            }
        }


        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("UPDATE PROJECT...");
        EditorGUILayout.Separator();

        if (GUILayout.Button("MIGRATE SCENES"))
        {
            this.puzzlePlayerPrefab = AssetFinder.SafeSingleAssetFind<RTPuzzle.PlayerManager>("PuzzlePlayer");

            if (this.puzzlePlayerPrefab != null)
            {
                List<PlayerPositionBinarry> positionsToSave = null;
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (FileStream fileStream = File.Open(positionFilePath, FileMode.Open))
                {
                    positionsToSave = (List<PlayerPositionBinarry>)binaryFormatter.Deserialize(fileStream);
                }

                if (positionsToSave != null)
                {
                    for (var i = 9; i < 14; i++)
                    {
                        var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                        var playerObject = GameObject.FindObjectOfType<RTPuzzle.PlayerManager>();
                        if (playerObject != null)
                        {
                            MonoBehaviour.DestroyImmediate(playerObject);
                        }
                        else
                        {
                            foreach (var positionToSave in positionsToSave)
                            {
                                if (positionToSave.ScenePath == scenePath)
                                {
                                    var instanciatedPlayer = (GameObject)PrefabUtility.InstantiatePrefab(this.puzzlePlayerPrefab.gameObject);
                                    var formattedTransform = positionToSave.Transform.Format();
                                    instanciatedPlayer.transform.position = formattedTransform.position;
                                    instanciatedPlayer.transform.rotation = formattedTransform.rotation;
                                    instanciatedPlayer.transform.localScale = formattedTransform.localScale;

                                    EditorUtility.SetDirty(instanciatedPlayer);
                                    EditorSceneManager.SaveOpenScenes();
                                }
                            }
                        }
                    }
                }
            }
        }

        if (GUILayout.Button("DELETE SAVED DATA"))
        {
            File.Delete(positionFilePath);
        }
    }

    [System.Serializable]
    public struct PlayerPositionBinarry
    {
        [SerializeField]
        public string ScenePath;

        [SerializeField]
        public TransformBinarry Transform;

        public PlayerPositionBinarry(string scenePath, TransformBinarry transform)
        {
            ScenePath = scenePath;
            Transform = transform;
        }
    }
}
