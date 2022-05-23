using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NodeMB : MonoBehaviour
{

    private InputAction leftMouseClick;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        leftMouseClick = new InputAction(binding: "<Mouse>/leftButton");
        leftMouseClick.performed += ctx => LeftMouseClicked();
        leftMouseClick.Enable();
    }

    private void LeftMouseClicked()
    {
        Debug.Log("LeftMouseClicked");
    }
}
