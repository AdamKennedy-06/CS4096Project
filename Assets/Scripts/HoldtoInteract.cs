using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HoldToInteract : MonoBehaviour
{
    public GameObject interactionCanvas;
    public Image progressCircleImage;
    public string levelToLoad;

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange)
        {
            if (Input.GetKey(KeyCode.E))
            {
                progressCircleImage.fillAmount += Time.deltaTime;
            }
            else
            {
                progressCircleImage.fillAmount = 0f;
            }

            if (progressCircleImage.fillAmount >= 1f)
            {
                SceneManager.LoadScene(levelToLoad);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionCanvas != null) interactionCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionCanvas != null) interactionCanvas.SetActive(false);
            progressCircleImage.fillAmount = 0f;
        }
    }
}
