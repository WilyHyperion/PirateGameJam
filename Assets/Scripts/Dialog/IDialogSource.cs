using UnityEngine;

public interface IDialogSource
{
    public abstract void ModifyDialog(Dialog dialog, IDialogSource you,Controllable player);
}
