using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class ObjectRepelContainer : MonoBehaviour
    {
        private List<ObjectRepelType> objectsRepelable;

        public List<ObjectRepelType> ObjectsRepelable { get => objectsRepelable; }

        public void Init()
        {
            var foundedObjectRepels = GameObject.FindObjectsOfType<ObjectRepelType>();
            if (foundedObjectRepels != null)
            {
                this.objectsRepelable = foundedObjectRepels.ToList();
            }
            else
            {
                this.objectsRepelable = new List<ObjectRepelType>();
            }

            foreach (var repelAbleObject in this.objectsRepelable)
            {
                repelAbleObject.Init();
            }
        }
    }

}
