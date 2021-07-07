using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectrumAnalyzer
{
    public class ClapRecognizer : SoundRecognizer
    {
        #region UNITY_REGION

        #region public_attributes
        [Tooltip("Limite de volumen a partir del cual empieza a analizar la entrada")]
        public int limit;
        [Tooltip("Tamaño de la ventana en la ventana deslizante")]
        public int windowSize;
        [Tooltip("Array de [0]inicio-[1]fin que representan las regiones en las que se divide el buffer del input \n\n{valores entre 0(inicio del buffer)-100(fin del buffer)} ")]
        public int[] regionsInSpactrumData;
        #endregion

        #region private_attributes
        Utils.Pair<int, int>[] _regions;
        #endregion

        #region private_methods
        private void Start()
        {

        }
        #endregion
        #endregion
        public override void Recognize(float[] array)
        {
            if (regionsInSpactrumData.Length < 2)
                Debug.LogError("regionsInSpactrumData is empty,  please insert at least two items");
            _regions = new Utils.Pair<int, int>[regionsInSpactrumData.Length / 2];
            int j = 0;
            for (int i = 0; i < regionsInSpactrumData.Length; i += 2)
            {
                _regions[j] = new Utils.Pair<int, int>(regionsInSpactrumData[i] * array.Length / 100,
                    regionsInSpactrumData[i + 1] * array.Length / 100);
                j++;
            }

            SlidingWindow(array, windowSize);
        }
    }
}