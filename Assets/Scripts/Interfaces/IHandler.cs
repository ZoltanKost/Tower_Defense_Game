public interface IHandler{
    public void Tick(float delta);
    public void AnimatorTick(float delta);
    public void Switch(bool active); // activates/deactivates handler
    public void DeactivateEntities(); // loops through entities and deactivates it
    public void ResetEntities(); // deletes all entities and/or resets the Collection 
}