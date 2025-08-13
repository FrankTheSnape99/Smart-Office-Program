namespace TDDAssignment
{
    public interface IDoorManager
    {
        string GetStatus();
        bool OpenAllDoors();
        void LockAllDoors(); //L3R1
    }
}