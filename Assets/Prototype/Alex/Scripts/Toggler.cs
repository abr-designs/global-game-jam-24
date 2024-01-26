using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Toggler : MonoBehaviour
{
    public GameObject[] GameObjects;

    [SerializeField]
    private TMP_Text TMPText;

    private int _activeIndex = -1;

    //============================================================================================================//
    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
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
    
    //============================================================================================================//

    private void UpdateText()
    {
        string text = string.Empty;

        for (int i = 0; i < GameObjects.Length; i++)
        {
            text += $"[{i + 1}]{GameObjects[i].name}{(i == _activeIndex ? " <==" : string.Empty)}\n";
        }

        TMPText.text = text;
    }

    private void Activate(int index)
    {
        _activeIndex = index;
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

        UpdateText();
    }
}
