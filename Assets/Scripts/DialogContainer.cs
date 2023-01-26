using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "DialogContainer", menuName = "Configs/DialogContainer")]
public class DialogContainer : ScriptableObjectInstaller
{
    [SerializeField] private List<Dialog> dialogs;

    public List<Dialog> Dialogs => dialogs;
}