using UnityEngine;

public class DiscussionTestScript : MonoBehaviour
{
    public bool triggerOpen;

    private ChoicePopup ChoicePopup;

    // Update is called once per frame
    void Update()
    {
        if (triggerOpen)
        {
            triggerOpen = false;


            ChoicePopup = Instantiate(PrefabContainer.Instance.ChoicePopupPrefab, GameObject.FindObjectOfType<Canvas>().transform);

            // ChoicePopup.OnChoicePopupAwake(discussionChoices, new Vector2(100, 100));
        }

        if (ChoicePopup != null)
        {
            ChoicePopup.Tick(Time.deltaTime);
        }
    }
}
