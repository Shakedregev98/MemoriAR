using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class NameToCandle : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField addYourNameInputField;
    public TextMeshProUGUI outputField;
   

    public void AddNameToTheCandle()
    {
        if (addYourNameInputField != null)
        {
            outputField.text = addYourNameInputField.text;
        }
        else
        {
            outputField.text = string.Empty;
        }
    }
}
