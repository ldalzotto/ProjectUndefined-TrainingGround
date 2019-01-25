using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[ExecuteInEditMode]
public class CutsceneTimelinePOIData : MonoBehaviour
{
    [Header("Gizmos Data")]
    public float DirectionLineLenght = 10f;

    public CutsceneId CutsceneId;

    private Transform playerStartingTransform;
    private PlayableDirector playableDirector;

    public Transform PlayerStartingTransform { get => playerStartingTransform; }
    public PlayableDirector PlayableDirector { get => playableDirector; }

    private void Start()
    {
        this.playerStartingTransform = transform;
        this.playableDirector = GetComponent<PlayableDirector>();
    }

    private void OnDrawGizmos()
    {

        var labelStyle = GUI.skin.GetStyle("Label");
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.yellow;
#if UNITY_EDITOR
        Handles.Label(playerStartingTransform.position + new Vector3(0, 3, 0), CutsceneId.ToString(), labelStyle);
#endif
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerStartingTransform.position, 1f);
        Gizmos.DrawLine(playerStartingTransform.position, playerStartingTransform.position + DirectionLineLenght * playerStartingTransform.forward);
    }
}
