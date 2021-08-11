using System;
using System.Collections.Generic;
using UnityEngine;
using Lasp;

/*
 Clase combo:
    - diccionario que contiene el combo (no se de que tipo van a ser pero tienen que entrar los tipos de sibidos diferentes -> guardar frecuencia y duracion de alguna manera y distinguir los silbidos entre ellos y entre palmas y chasquidos) 

Clase WhistleFrequencyRecognizer: cada vez que se llame al metodo recognize hay que saignar los valores frequency y frequencyDuration que queramos identificar
 */


namespace PatternRecognizer
{
    /*
     * Clase que se encarga de gestionar los SoundRecognizers y les proporciona el array de frecuencias a cada uno
     * **/
    public class SoundEventManager : MonoBehaviour
    {
        #region SINGLETON_REGION
        /*
         * sigleton pattern in unity: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
         * **/
        private static SoundEventManager _instance = null;
        public static SoundEventManager Instance => _instance;
        #endregion

        #region UNITY_REGION
        #region private_methods
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                if (_clapRecognizer == null || _clickRecognizer == null || _whistleRecognizer == null)
                {
                    _clapRecognizer = new ClickRecognizer("Clap", 140, 40, 10, 100);
                    _clickRecognizer = new ClickRecognizer("Click", 100, 30, 2, 18);
                    _whistleRecognizer = new WhistleRecognizer();
                    _whistleFrequencyRecognizer = new WhistleFrequencyRecognizer(0, 0);
                }

                if (_analyzer == null)
                    _analyzer = GetComponent<SpectrumAnalyzer>();

                _instance = this;
            }
        }

        private void Update()
        {
            //_clickRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray());
            Debug.Log(_clickRecognizer._name + " - " + _clickRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray()));

            //_clapRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray());
            Debug.Log(_clapRecognizer._name + " - " + _clapRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray()));

            var aux = _whistleRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray());
            if (aux != null)
                Debug.Log(aux.First + " " + aux.Second);
        }
        #endregion
        #endregion

        #region PUBLIC_METHODS
        //public void AddRecognizer(SoundRecognizer sr) => _soundRecognizers.Add(sr);

        //public bool RemoveRecognizer(SoundRecognizer sr)
        //{
        //    if (_soundRecognizers.Count > 0)
        //        return _soundRecognizers.Remove(sr);

        //    return false;
        //}

        //public List<SoundRecognizer> GetRecognizers() => _soundRecognizers;

        public void SetReconizerDelegate()
        {
        }
        public int GetResolution() => _analyzer.resolution;
        #endregion

        #region PRIVATE_ATTRIBUTES
        // Clase que contiene el array con el sonido de entrada
        private SpectrumAnalyzer _analyzer = null;

        private WhistleRecognizer _whistleRecognizer;
        private WhistleFrequencyRecognizer _whistleFrequencyRecognizer;
        private ClickRecognizer _clapRecognizer;
        private ClickRecognizer _clickRecognizer;
        #endregion
    }
}