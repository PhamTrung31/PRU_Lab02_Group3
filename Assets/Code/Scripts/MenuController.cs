using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject buttonContainer;
    public GameObject levelButtonContainer;
    public GameObject optionButtonContainer;
    public GameObject helpContainer;

    void Start()
    {
        buttonContainer.SetActive(true);
        levelButtonContainer.SetActive(false);
        optionButtonContainer.SetActive(false);
        helpContainer.SetActive(false);
    }

    public void OnPlayButtonClicked()
    {
        buttonContainer.SetActive(false);
        levelButtonContainer.SetActive(true);
        optionButtonContainer.SetActive(false);
        helpContainer.SetActive(false);
    }

    public void OnBackButtonClicked()
    {
        levelButtonContainer.SetActive(false);
        buttonContainer.SetActive(true);
        optionButtonContainer.SetActive(false);
        helpContainer.SetActive(false);
    }

    public void OnOptionButtonClicked()
    {
        levelButtonContainer.SetActive(false);
        buttonContainer.SetActive(false);
        optionButtonContainer.SetActive(true);
        helpContainer.SetActive(false);
    }

    public void OnHelpButtonClicked()
    {
        levelButtonContainer.SetActive(false);
        buttonContainer.SetActive(false);
        optionButtonContainer.SetActive(false);
        helpContainer.SetActive(true);
    }
}
