using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HoldToInteract : MonoBehaviour
{
    public GameObject interactionCanvas;
    public Image progressCircleImage;

    void Start()
    {
      
    }

    void Update()
    {
        if (interactionCanvas.activeSelf)
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
                SceneManager.LoadScene("tutorial");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactionCanvas != null) interactionCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactionCanvas != null) interactionCanvas.SetActive(false);
            progressCircleImage.fillAmount = 0f;
        }
    }
}
