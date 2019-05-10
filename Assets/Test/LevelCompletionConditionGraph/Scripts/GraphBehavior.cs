using UnityEngine;
using System.Collections;

public class GraphBehavior : MonoBehaviour
{
    public LevelCompletionConditionGraph LevelCompletionConditionGraph;
    private void Start()
    {

        Debug.Log(LevelCompletionConditionGraph.ResolveGraph());
    }
}
