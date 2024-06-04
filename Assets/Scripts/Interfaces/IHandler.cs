public interface IHandler{
    public void Tick(float delta);
    public void AnimatorTick(float delta);
    public void Switch(bool active); // activates/deactivates handler
    public void ResetEntities(); // Resets all the entities. 
    public void DeactivateEntities(); // Loops through entities and deactivates it
    public void ClearEntities(); // Deletes all the entities;
}