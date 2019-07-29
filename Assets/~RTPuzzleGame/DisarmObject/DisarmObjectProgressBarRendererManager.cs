using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class DisarmObjectProgressBarRendererManager : MonoBehaviour
    {
        #region External Dependencies
        private InteractiveObjectContainer InteractiveObjectContainer;
        private PuzzleStaticConfigurationContainer PuzzleStaticConfigurationContainer;
        #endregion

        private Dictionary<DisarmObjectModule, DisarmObjectProgressBarType> disarmProgresionbars;

        public void Init()
        {
            #region External Depedencies
            this.InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            this.PuzzleStaticConfigurationContainer = GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>();
            #endregion

            this.disarmProgresionbars = new Dictionary<DisarmObjectModule, DisarmObjectProgressBarType>();
        }

        public void Tick(float d)
        {
            foreach (var disarmObject in this.InteractiveObjectContainer.DisarmObjectModules)
            {
                var disarmPercentage = disarmObject.GetDisarmPercentage01();
                if (disarmPercentage > 0)
                {
                    if (!this.disarmProgresionbars.ContainsKey(disarmObject))
                    {
                        this.disarmProgresionbars[disarmObject] = MonoBehaviour.Instantiate(PrefabContainer.Instance.DisarmObjectProgressBarBasePrefab, this.transform);
                        this.disarmProgresionbars[disarmObject].Init();
                    }
                    this.disarmProgresionbars[disarmObject].SetPosition(disarmObject.GetProgressBarDisplayPosition());
                    this.disarmProgresionbars[disarmObject].SetDisarmPercentage(disarmPercentage);
                }
            }
        }

        internal void OnDisarmObjectDestroyed(DisarmObjectModule disarmObjectModule)
        {
            if (this.disarmProgresionbars.ContainsKey(disarmObjectModule))
            {
                MonoBehaviour.Destroy(this.disarmProgresionbars[disarmObjectModule].gameObject);
                this.disarmProgresionbars.Remove(disarmObjectModule);
            }
        }
    }

}
