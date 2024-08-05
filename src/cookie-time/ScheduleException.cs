namespace cookie_time;

public class ScheduleException(int day, int hour) : Exception
{
    public override string Message => $"Incorrect remainder found for day:{day} & hour:{hour}";
}