using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

namespace CoreGame
{
    public class PersistanceManager : MonoBehaviour
    {

        private AutoSaveIcon AutoSaveIcon;

        private BinaryFormatter binaryFormatter = new BinaryFormatter();
        private PersistanceManagerThreadObject PersistanceManagerThreadObject;
        // End of processing async event
        private bool NoMorePersistanceEvent;

        public virtual void Init()
        {
            this.AutoSaveIcon = GameObject.FindObjectOfType<AutoSaveIcon>();
            this.AutoSaveIcon.Init();
            if (this.PersistanceManagerThreadObject == null)
            {
                this.PersistanceManagerThreadObject = new PersistanceManagerThreadObject(OnNoMorePersistanceProcessingCallback: this.OnNoMorePersistanceProcessing);
            }
        }

        public virtual void Tick(float d)
        {
            if (this.NoMorePersistanceEvent)
            {
                this.AutoSaveIcon.OnSaveEnd();
                this.NoMorePersistanceEvent = false;
            }
        }

        #region External Event
        public virtual void OnPersistRequested(Action persistAction)
        {
            this.AutoSaveIcon.OnSaveStart();
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
        private Thread peristanceManagerThread;
        private Queue<Action> persistQueueActions;

        private Action OnNoMorePersistanceProcessingCallback;

        public PersistanceManagerThreadObject(Action OnNoMorePersistanceProcessingCallback)
        {
            this.persistQueueActions = new Queue<Action>();
            this.peristanceManagerThread = new Thread(new ThreadStart(this.Main));
            this.peristanceManagerThread.IsBackground = true;
            this.peristanceManagerThread.Start();
            this.OnNoMorePersistanceProcessingCallback = OnNoMorePersistanceProcessingCallback;
        }

        public void OnPersistRequested(Action persistAction)
        {
            lock (this.persistQueueActions)
            {
                this.persistQueueActions.Enqueue(persistAction);
            }
        }

        private int lastFrameCount = 0;

        private void Main()
        {
            while (true)
            {
                int queueCount;
                lock (persistQueueActions)
                {
                    queueCount = persistQueueActions.Count;
                }

                if (queueCount > 0)
                {
                    Action nextAction;
                    lock (this.persistQueueActions)
                    {
                        nextAction = this.persistQueueActions.Dequeue();
                    }

                    if (nextAction != null)
                    {
                        nextAction.Invoke();
                    }
                }

                if (this.lastFrameCount != 0 && queueCount == 0)
                {
                    this.OnNoMorePersistanceProcessingCallback.Invoke();
                }

                this.lastFrameCount = queueCount;
            }
        }
    }

}
