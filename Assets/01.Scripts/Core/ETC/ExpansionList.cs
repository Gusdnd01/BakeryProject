using System;
using System.Collections.Generic;

[Serializable]
public class ExpansionList<T> : List<T>
{
    public event EventHandler ListChanged;
    public event EventHandler ListAdded;
    public event EventHandler ListRemoved;

    public new void Add(T item)
    {
        base.Add(item);
        ListChanged?.Invoke(this, EventArgs.Empty);
        ListAdded?.Invoke(this, EventArgs.Empty);
    }

    public new void Remove(T item) 
    { 
        base.Remove(item);
        ListChanged?.Invoke(this, EventArgs.Empty);
        ListRemoved?.Invoke(this, EventArgs.Empty);
    }
}
