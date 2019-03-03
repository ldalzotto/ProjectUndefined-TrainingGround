using System;
using UnityEngine;

public class InOutLayoutCellAnimation : AnimatedLayoutCell
{
    protected override Func<Vector3, Vector3> inAnimation => (position) => position + new Vector3(-100, 0, 0);

    protected override Func<Vector3, Vector3> outAnimation => (position) => position + new Vector3(-100, 0, 0);
}
