using CoreGame;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace StartMenu
{
    public class NewGameButton : MonoBehaviour
    {
        private void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(() =>
            {
                var startLevelManager = CoreGameSingletonInstances.StartLevelManager;

                //Destroy all saved data
                var persistanceDirectory = new DirectoryInfo(Application.persistentDataPath);
                foreach (var directory in persistanceDirectory.GetDirectories())
                {
                    directory.Delete(true);
                }
                CoreGameSingletonInstances.LevelTransitionManager.OnStartMenuToLevel(startLevelManager.GetStartLevelID());
            });
        }
    }
}
