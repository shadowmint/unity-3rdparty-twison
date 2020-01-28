using System.Collections.Generic;

namespace Twison
{
    [System.Serializable]
    public class TwisonStory
    {
        public string ifid;
        public string name;
        public string creator;
        public string creatorVersion;
        public string startnode;
        public List<TwisonPassage> passages;

        [System.NonSerialized] 
        public TwisonPassage startNodePassage;
    }
}