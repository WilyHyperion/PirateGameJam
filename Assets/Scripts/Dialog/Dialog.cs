using System;
using System.Collections.Generic;
using UnityEngine;

public class Dialog
{
    /// <summary>
    /// Adds a dialog option to the dialog, or overwrites it if it already exists
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddOption(string name, Action<Controllable, Controllable> action)
    {
        if(options.ContainsKey(name))
        {
            options[name] = action;
            return;
        }
        options.Add(name, action);
    }   
    public Dictionary<string, Action<Controllable, Controllable>> options =     new Dictionary<string, Action<Controllable, Controllable>>();
}
