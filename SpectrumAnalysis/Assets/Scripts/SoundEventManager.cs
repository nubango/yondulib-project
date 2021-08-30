using System;
using System.Collections.Generic;
using UnityEngine;
using Lasp;

/*
 Clase combo:
    - diccionario que contiene el combo (no se de que tipo van a ser pero tienen que entrar los tipos de sibidos diferentes -> guardar frecuencia y duracion de alguna manera y distinguir los silbidos entre ellos y entre palmas y chasquidos) 

(igual esta clase no se tiene que usar) Clase WhistleFrequencyRecognizer: cada vez que se llame al metodo recognize hay que saignar los valores frequency y frequencyDuration que queramos identificar


Esta clase identifica los combos y gestiona cuando ha acabado un combo y cuando no (el tiempo que hay silencio)

El metodo de la clase combo recibe un atributo ComboName y otro WhistleData (si no es silbido los valores son negarivos)
Cada combo se encarga de saber si la secuencia que lleva es la correcta o no. Si se pasa un ComboName de tipo Silence entonces se devuelve true o false (si coincide o
no) y se reinicia todo.

(Como está hecho lo de los silbidos no me acaba de convencer porque para detectar un ascenso o descenso de frecuencia no se si como lo planteo va a funcionar)

 */


namespace PatternRecognizer
{
    /*
     * Clase que se encarga de gestionar los SoundRecognizers y les proporciona el array de frecuencias a cada uno
     * **/
    public class SoundEventManager : MonoBehaviour
    {
        #region UNITY_REGION
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;

                if (_analyzer == null)
                    _analyzer = GetComponent<SpectrumAnalyzer>();

                if (_clapRecognizer == null || _clickRecognizer == null || _whistleRecognizer == null)
                {
                    _clapRecognizer = new ClickRecognizer("Clap", 140, 40, 10, 100);
                    _clickRecognizer = new ClickRecognizer("Click", 100, 30, 2, 18);
                    _whistleRecognizer = new WhistleRecognizer();
                    _whistleFrequencyRecognizer = new WhistleFrequencyRecognizer(0, 0);
                }

            }
        }

        private void Update()
        {
            // Debug----------------------------------------------
            //_clickRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray());
            Debug.Log(_clickRecognizer._name + " - " + _clickRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray()));

            //_clapRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray());
            Debug.Log(_clapRecognizer._name + " - " + _clapRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray()));

            var aux = _whistleRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray());
            if (aux != null)
                Debug.Log(aux.First + " " + aux.Second);
            // Debug----------------------------------------------

            ComboComparison(ComboIdentification(_analyzer.logSpectrumSpan.ToArray()));

        }
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

        #region PRIVATE_METHODS
        private Pair<ComboName[], WhistleData[]> ComboIdentification(float[] array)
        {
            // analizamos el buffer y vamos concretando que patrones se identifican, si hay mucho silencio o no se identifica nada
            // significa que el combo ha acabado, entonces se devuelve. _currentcombo y currentdata sirven para ir guardando el combo
            // entre vueltas de bucle y no perder la informacion de la vuelta anterior

            return null;
        }

        private void ComboComparison(Pair<ComboName[], WhistleData[]> combo)
        {
            if (combo == null)
                return;

            foreach(Combo c in _combos)
            {
                c.Recognizer(combo.First, combo.Second);
            }
        }

        #endregion

        #region SINGLETON_REGION
        /*
         * sigleton pattern in unity: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
         * **/
        private static SoundEventManager _instance = null;
        public static SoundEventManager Instance => _instance;
        #endregion

        #region PRIVATE_ATTRIBUTES
        // Clase que contiene el array con el sonido de entrada
        private SpectrumAnalyzer _analyzer = null;

        private WhistleRecognizer _whistleRecognizer;
        private WhistleFrequencyRecognizer _whistleFrequencyRecognizer;
        private ClickRecognizer _clapRecognizer;
        private ClickRecognizer _clickRecognizer;

        private Combo[] _combos;

        private ComboName[] _currentCombo;
        private WhistleData[] _currentData;
        #endregion
    }
}