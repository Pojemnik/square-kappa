using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DropdownPanelController : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Dropdown dropdown;

    [HideInInspector]
    public event System.EventHandler<int> DropdownFieldSelected;

    private void Awake()
    {
        dropdown.onValueChanged.AddListener((i) => OnDropdownValueChanged(i));
    }

    public void SetDropdownContent(List<string> options)
    {
        dropdown.options = options.Select((e) => new TMPro.TMP_Dropdown.OptionData(e)).ToList();
    }

    public void SetSelectedOption(int index)
    {
        dropdown.SetValueWithoutNotify(index);
    }

    private void OnDropdownValueChanged(int index)
    {
        if(DropdownFieldSelected != null)
        {
            DropdownFieldSelected(this, index);
        }
    }
}
