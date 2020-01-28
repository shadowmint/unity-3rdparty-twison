using System;

namespace Twison.Errors
{
    public class TwisonValidationError : Exception
    {
        public TwisonStory Story { get; set; }

        public TwisonPassage Passage { get; set; }

        public TwisonLink Link { get; set; }

        public TwisonValidationError(string error, TwisonStory story) : base(error)
        {
            Story = story;
        }

        public TwisonValidationError(string error, TwisonLink link) : base(error)
        {
            Link = link;
        }

        public TwisonValidationError(string error, TwisonPassage passage) : base(error)
        {
            Passage = passage;
        }
    }
}