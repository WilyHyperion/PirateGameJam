
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogModifier
{
 public abstract void ModifyDialog(Dialog dialog);
   public abstract List<string> AddProfileInformation(Controllable controllable);
}
