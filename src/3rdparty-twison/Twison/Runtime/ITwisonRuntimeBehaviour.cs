using System.Collections.Generic;
using Twison;

namespace DefaultNamespace
{
    public interface ITwisonRuntimeBehaviour
    {
        List<TwisonLink> Filter(TwisonPassage active);
        void OnEnterState(TwisonPassage active);
        void OnExitState(TwisonPassage active);
    }
}