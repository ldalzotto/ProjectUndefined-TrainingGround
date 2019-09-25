using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface ILinePositioning
    {
        Vector3 GetEndPosition(Vector3 startPosition);
    }
}
