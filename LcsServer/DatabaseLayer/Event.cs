namespace LcsServer.DatabaseLayer
{
    public class Event
    {
        public int Id { get; set; }
        public string level { get; set; }
        public string deviceId { get; set; }
        public string Description { get; set; }
        public DateTime dateTime { get; set; }
        public string State { get; set; }
    }
}
