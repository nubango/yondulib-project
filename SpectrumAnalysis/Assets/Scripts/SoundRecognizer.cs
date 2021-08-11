using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternRecognizer
{
    abstract class SoundRecognizer
    {
        public abstract bool Recognize(float[] array);
    }
}