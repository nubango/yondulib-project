using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PatternRecognizer
{
    public interface SoundRecognizer
    {
        public float Recognize(float[] array);
    }
}