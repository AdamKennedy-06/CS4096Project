using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class MainMenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public ThirdPersonController movementScript;

    // Start is called before the first frame update
    void Start()
    { 
   

}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTheGame()
    {
        if (menuCanvas != null) menuCanvas.SetActive(false);
        if (movementScript != null) movementScript.enabled = true;

    }


}

