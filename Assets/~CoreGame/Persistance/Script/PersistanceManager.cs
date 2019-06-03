﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;

namespace CoreGame
{
    public class PersistanceManager : MonoBehaviour
    {

        private AutoSaveIcon AutoSaveIcon;

        private BackgroundWorker persistThread;
        private Queue<Action> persistQueueActions;

        public void Init()
        {
            this.AutoSaveIcon = GameObject.FindObjectOfType<AutoSaveIcon>();
            this.AutoSaveIcon.Init();

            this.persistThread = new BackgroundWorker();
            this.persistThread.DoWork += this.DoWork;
            this.persistThread.RunWorkerCompleted += this.OnTaskCompleted;

            this.persistQueueActions = new Queue<Action>();
        }

        #region External Event
        public void OnPersistRequested(Action persistAction)
        {
            this.AutoSaveIcon.OnSaveStart();
            if (!this.persistThread.IsBusy)
            {
                this.persistThread.RunWorkerAsync(persistAction);
            }
            else
            {
                this.persistQueueActions.Enqueue(persistAction);
            }

        }
        #endregion

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            ((Action)e.Argument).Invoke();
        }

        private void OnTaskCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                UnityEngine.Debug.LogError(e.Error);
            }
            if (this.persistQueueActions.Count > 0)
            {
                var nextAction = this.persistQueueActions.Dequeue();
                if (nextAction != null)
                {
                    this.persistThread.RunWorkerAsync(nextAction);
                }
            } else
            {
                this.AutoSaveIcon.OnSaveEnd();
            }
        }

    }
}
