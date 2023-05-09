using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TabInputField : MonoBehaviour
{

    public List<InputField> listInputField;

    public int inputSelected;

    public InputAction TAB;

    private void Awake()
    {
        TAB.performed += ctx => TabAction();
    }

    public void TabAction()
    {
        inputSelected = -1;
        for (int i = 0; i < listInputField.Count; i++)
        {
            if (listInputField[i].isFocused)
            {
                inputSelected = i;
                break;
            }
        }

        inputSelected++;
        if (inputSelected < listInputField.Count)
        {
            listInputField[inputSelected].Select();
            listInputField[inputSelected].ActivateInputField();
        }
    }

    private void OnEnable()
    {
        TAB.Enable();
    }

    private void OnDisable()
    {
        TAB.Disable();
    }

}
