namespace Twison
{
    [System.Serializable]
    public class TwisonLoaderOptions
    {
        public bool trimPassages;
        public bool removeLinkTextFromPassages;
        public bool collapseNewLines;
        public int maxNewLineChain;

        public static TwisonLoaderOptions GetDefaultOptions()
        {
            return new TwisonLoaderOptions();
        }
    }
}