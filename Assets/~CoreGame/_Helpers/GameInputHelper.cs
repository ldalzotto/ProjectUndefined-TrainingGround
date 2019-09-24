using UnityEngine.InputSystem;

namespace CoreGame
{
    public static class GameInputHelper
    {
        public static InputImageTypeInstanceType GetInputImageType(InputConfigurationInherentData inputConfigurationInherentData)
        {
            InputImageTypeInstanceType InputImageTypeInstanceType = InputImageTypeInstanceType.NONE;
            var keyAttributedButton = inputConfigurationInherentData.GetAssociatedInputKey();
            if (keyAttributedButton != Key.None)
            {
                InputImageTypeInstanceType = InputImageTypeInstanceType.KEY;
            }
            else
            {
                var mouseAttributedButton = inputConfigurationInherentData.GetAssociatedMouseButton();
                if (mouseAttributedButton != MouseButton.NONE)
                {
                    if (mouseAttributedButton == MouseButton.LEFT_BUTTON)
                    {
                        InputImageTypeInstanceType = InputImageTypeInstanceType.LEFT_MOUSE;
                    }
                    else if (mouseAttributedButton == MouseButton.RIGHT_BUTTON)
                    {
                        InputImageTypeInstanceType = InputImageTypeInstanceType.RIGHT_MOUSE;
                    }
                }
                else
                {
                    var scrollAttributedButton = inputConfigurationInherentData.GetAssociatedMouseScroll();
                    if(scrollAttributedButton!= MouseScroll.NONE)
                    {
                        InputImageTypeInstanceType = InputImageTypeInstanceType.SCROLL;
                    }
                }
            }

            return InputImageTypeInstanceType;
        }
    }
}
