using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectrumAnalyzer
{
    public abstract class SoundRecognizer : MonoBehaviour
    {
        public abstract void Recognize(float[] array);
    }
}