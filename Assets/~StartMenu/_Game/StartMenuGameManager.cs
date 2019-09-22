using CoreGame;
using UnityEngine;

namespace StartMenu
{
    public class StartMenuGameManager : AsbtractCoreGameManager
    {
        private void Awake()
        {
            GameObject.FindObjectOfType<GameManagerPersistanceInstance>().Init();
            this.OnAwake(LevelType.STARTMENU);
        }

        private void Start()
        {
            this.OnStart();
        }

        private void Update()
        {
            var d = Time.deltaTime;
            this.BeforeTick(d);
        }
    }

}
