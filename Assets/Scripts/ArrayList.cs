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
    public int lastIndex{get; private set;}
    public ArrayList(){
        values = new T[4];
        lastIndex = 0;
    }
    public ArrayList(ArrayList<T> copy){
        T[] temp = copy.values;
        values = new T[temp.Length];
        for(int i = 0; i < temp.Length; i++){
            values[i] = temp[i];
        }
    }
    public void Add(T item){
        if(lastIndex >= values.Length) Array.Resize(ref values,values.Length*2);
        values[lastIndex] = item;
        lastIndex++;
    }
    public T GetLast(){
        if(lastIndex == 0) return default;
        return values[lastIndex - 1];
    }
    public T[] Reverse(){
        T[] result = new T[lastIndex];
        for(int i = 1; i <= lastIndex; i++){
            result[lastIndex - i] = values[lastIndex];
        }
        return result;
    }
}