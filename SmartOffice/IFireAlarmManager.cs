namespace TDDAssignment
{
    public interface IFireAlarmManager
    {
        string GetStatus();
        bool SetAlarm(bool AlarmOn);//L3R2
    }
}