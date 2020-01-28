using System.Collections.Generic;
using UnityEngine;

namespace Twison
{
    [System.Serializable]
    public class TwisonPassage
    {
        public string pid;
        public string name;
        public string text;
        public List<TwisonLink> links;
        public List<string> tags;
        public Vector2 position;
    }
}