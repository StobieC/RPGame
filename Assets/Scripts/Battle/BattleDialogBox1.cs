using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialog : MonoBehaviour
{
    [SerializeField] Text dialogTextBox;

    public void setDialog(string dialogText)
    {
        dialogTextBox.text = dialogText;
    }
}
