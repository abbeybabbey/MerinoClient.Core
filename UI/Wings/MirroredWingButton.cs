using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MerinoClient.Core.UI.Wings
{
    public class MirroredWingButton
    {
        private readonly WingButton _leftButton;
        private readonly WingButton _rightButton;

        public MirroredWingButton(string text, string tooltip, Action onClick, Transform leftParent, Transform rightParent, Sprite sprite = null, bool arrow = true, bool background = true,
            bool separator = false)
        {
            _leftButton = new WingButton(text, tooltip, onClick, leftParent, sprite, arrow, background, separator);
            _rightButton = new WingButton(text, tooltip, onClick, rightParent, sprite, arrow, background, separator);
        }

        public void Destroy()
        {
            Object.DestroyImmediate(_leftButton.GameObject);
            Object.DestroyImmediate(_rightButton.GameObject);
        }
    }
}
