using System.Collections.Generic;
using UnityEngine;

public abstract class ScenarioInitialisation : MonoBehaviour
{
    public abstract List<ScenarioNode> InitialScenarioNodes();
}
