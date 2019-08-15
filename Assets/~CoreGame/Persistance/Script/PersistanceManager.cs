using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

namespace CoreGame
{
    public class PersistanceManager : MonoBehaviour
    {
        
        private BinaryFormatter binaryFormatter = new BinaryFormatter();
        private PersistanceManagerThreadObject PersistanceManagerThreadObject;
        // End of processing async event
        private bool NoMorePersistanceEvent;

        public virtual void Init()
        {
            CoreGameSingletonInstances.AutoSaveIcon.Init();
            if (this.PersistanceManagerThreadObject == null)
            {
                this.PersistanceManagerThreadObject = new PersistanceManagerThreadObject(OnNoMorePersistanceProcessingCallback: this.OnNoMorePersistanceProcessing);
            }
        }
        
        public virtual void Tick(float d)
        {
            if (this.NoMorePersistanceEvent)
            {
                CoreGameSingletonInstances.AutoSaveIcon.OnSaveEnd();
                this.NoMorePersistanceEvent = false;
            }
        }

        #region External Event
        public virtual void OnPersistRequested(Action persistAction)
        {
            CoreGameSingletonInstances.AutoSaveIcon.OnSaveStart();
            this.PersistanceManagerThreadObject.OnPersistRequested(persistAction);
        }
        public void OnNoMorePersistanceProcessing()
        {
            this.NoMorePersistanceEvent = true;
        }
        #endregion

        #region Loading
        public virtual T Load<T>(string folderPath, string dataPath, string filename, string fileExtension)
        {
            return LoadStatic<T>(folderPath, dataPath, filename, fileExtension, this.binaryFormatter);
        }

        public static T LoadStatic<T>(string folderPath, string dataPath, string filename, string fileExtension, BinaryFormatter binaryFormatter)
        {
            if (Directory.Exists(folderPath))
            {
                var directoryFiles = Directory.GetFiles(folderPath, filename + fileExtension);
                if (directoryFiles.Length > 0)
                {
                    using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
                    {
                        UnityEngine.Debug.Log(MyLog.Format("Loaded : " + dataPath));
                        return (T)binaryFormatter.Deserialize(fileStream);
                    }
                }
            }
            return default(T);
        }

        #endregion

    }

    class PersistanceManagerThreadObject
    {
        private bool executingActions;
        private Queue<Action> persistQueueActions;

        private Action OnNoMorePersistanceProcessingCallback;

        public PersistanceManagerThreadObject(Action OnNoMorePersistanceProcessingCallback)
        {
            this.persistQueueActions = new Queue<Action>();
            this.OnNoMorePersistanceProcessingCallback = OnNoMorePersistanceProcessingCallback;
        }

        public void OnPersistRequested(Action persistAction)
        {
            lock (this.persistQueueActions)
            {
                this.persistQueueActions.Enqueue(persistAction);
            }

            if (!this.executingActions)
            {
                this.executingActions = true;
                this.ProcessNextAction();
            }
        }

        private void ProcessNextAction()
        {
            Action nextAction;
            lock (this.persistQueueActions)
            {
                nextAction = this.persistQueueActions.Dequeue();
            }
            Task.Factory.StartNew(() => this.DoTask(nextAction));
        }

        private void DoTask(Action action)
        {
            action.Invoke();

            bool processNextAction = false;
            lock (this.persistQueueActions)
            {
                if (this.persistQueueActions.Count != 0)
                {
                    processNextAction = true;
                }
            }

            if (processNextAction)
            {
                this.ProcessNextAction();
            }
            else
            {
                this.OnNoMorePersistanceProcessingCallback.Invoke();
                this.executingActions = false;
            }
        }

    }

}
