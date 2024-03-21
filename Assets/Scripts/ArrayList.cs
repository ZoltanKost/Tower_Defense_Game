using System;

public class ArrayList<T> {
    public T[] values;
    public T this[int index] {
        get 
        {
            return values[index];
        } 
        set 
        {
            values[index] = value;
        }
    }
    public int Count{get; private set;}
    public ArrayList(){
        values = new T[4];
        Count = 0;
    }
    public ArrayList(int Count){
        values = new T[Count];
        this.Count = 0;
    }
    public ArrayList(ArrayList<T> copy){
        T[] temp = copy.values;
        values = new T[temp.Length];
        for(int i = 0; i < temp.Length; i++){
            values[i] = temp[i];
        }
    }
    public void Add(T item){
        if(Count >= values.Length) Array.Resize(ref values,values.Length*2);
        values[Count] = item;
        Count++;
    }
}