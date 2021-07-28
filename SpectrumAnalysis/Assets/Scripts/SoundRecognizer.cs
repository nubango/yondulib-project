using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternRecognizer
{
    public abstract class SoundRecognizer : MonoBehaviour
    {
        public abstract bool Recognize(float[] array);
    }
}