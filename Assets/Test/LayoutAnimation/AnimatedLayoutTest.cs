using UnityEngine;
using UnityEngine.UI;

public class AnimatedLayoutTest : MonoBehaviour
{

    private AnimatedLayout AnimatedLayout;

    public bool AddElement;
    public bool DeleteElement;
    public AnimatedLayoutCell AddedCellPrefab;

    // Use this for initialization
    void Start()
    {
        AnimatedLayout = GameObject.FindObjectOfType<AnimatedLayout>();
        AnimatedLayout.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (AddElement)
        {
            var cellPrefab = MonoBehaviour.Instantiate(AddedCellPrefab);
            cellPrefab.GetComponent<Image>().color = new Color(Random.Range(0f,1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            AnimatedLayout.AddLayoutElement(cellPrefab, 0);
            AddElement = false;
        }

        if (DeleteElement)
        {
          //  AnimatedLayout.DeleteRandomLayoutElement();
            DeleteElement = false;
        }
        AnimatedLayout.Tick(Time.deltaTime);
    }
}
