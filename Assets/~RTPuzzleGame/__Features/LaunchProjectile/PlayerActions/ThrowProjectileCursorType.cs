using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTPuzzle
{
    public class ThrowProjectileCursorType : MonoBehaviour
    {

        public Color CursorInRangeColor;
        public Color CursorOutOfRangeColor;

        private Image cursorImage;

        public static ThrowProjectileCursorType Instanciate(Transform parent)
        {
            var throwProjectileCursorType = MonoBehaviour.Instantiate(
                    PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration.ThrowProjectileCursorTypePrefab, parent);
            throwProjectileCursorType.Init();
            return throwProjectileCursorType;
        }

        public void Init()
        {
            this.cursorImage = GetComponent<Image>();
            this.OnCursorOnProjectileRange();
        }

        public void OnCursorOnProjectileRange()
        {
            this.cursorImage.color = CursorInRangeColor;
        }

        public void OnCursorOutOfProjectileRange()
        {
            this.cursorImage.color = CursorOutOfRangeColor;
        }

    }

}
