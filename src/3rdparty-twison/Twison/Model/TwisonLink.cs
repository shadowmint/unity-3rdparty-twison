namespace Twison
{
    [System.Serializable]
    public class TwisonLink
    {
        public string name;
        public string link;
        public string pid;

        [System.NonSerialized]
        public TwisonPassage passage;
    }
}