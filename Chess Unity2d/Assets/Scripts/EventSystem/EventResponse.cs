public abstract class EventResponse
{
    public abstract string Serialize();
    public abstract EventResponse Deserialize(string obj);
}