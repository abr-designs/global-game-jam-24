using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Toggler : MonoBehaviour
{
    public GameObject[] GameObjects;

    [SerializeField]
    private TMP_Text TMPText;
    // Start is called before the first frame update
    void Start()
    {
        string text = string.Empty;

        for (int i = 0; i < GameObjects.Length; i++)
        {
            text += $"[{i + 1}]{GameObjects[i].name}\n";
        }

        TMPText.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
            return;
        }

        for (int i = 0; i < GameObjects.Length; i++)
        {
            if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), $"Alpha{i + 1}")))
            {
                Activate(i);
                return;
            }
        }
    }

    private void Activate(int index)
    {
        var interactionController = FindObjectOfType<ObjectInteractionController>();
        for (int i = 0; i < GameObjects.Length; i++)
        {
            var activate = i == index;
            
            GameObjects[i].SetActive(activate);

            if (activate)
            {
                GameObjects[i].transform.FindObjectWithName("Hips", out var target);
                interactionController.playerRootTransform = target;
            }
        }
    }
}
