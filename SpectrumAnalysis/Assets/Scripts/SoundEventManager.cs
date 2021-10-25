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

                if (_clickRecognizer == null || _whistleRecognizer == null)
                {
                    _clickRecognizer = new ClickRecognizer();
                    _whistleRecognizer = new WhistleRecognizer();

                    /*
                    _combos = new List<Combo>();

                    List<ComboName> combo1 = new List<ComboName>();
                    combo1.Add(ComboName.Click);
                    combo1.Add(ComboName.Click);
                    _combos.Add(new Combo("Combo1", combo1.ToArray(), null));

                    List<ComboName> combo2 = new List<ComboName>();
                    combo2.Add(ComboName.Click);
                    combo2.Add(ComboName.Clap);
                    _combos.Add(new Combo("Combo2", combo2.ToArray(), null));

                    List<ComboName> combo3 = new List<ComboName>();
                    combo3.Add(ComboName.Clap);
                    combo3.Add(ComboName.Clap);
                    _combos.Add(new Combo("Combo3", combo2.ToArray(), null));
                    */
                }
            }
        }

        private void Update()
        {
            //Debug.Log(_clickRecognizer.HitIdentifier(_analyzer.logSpectrumSpan.ToArray()));

            Debug.Log(_whistleRecognizer.WhistleIdentifier(_analyzer.logSpectrumSpan.ToArray()));

            //CompareCombos(ComboIdentification(_analyzer.logSpectrumSpan.ToArray()));

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

        //private void CompareCombos(Pair<ComboName[], WhistleData[]> combo)
        //{
        //    if (combo == null)
        //        return;

        //    foreach (Combo c in _combos)
        //    {
        //        Debug.Log(c._name + " -> " + c.Recognizer(combo.First, combo.Second));
        //    }
        //}

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

        private ClickRecognizer _clickRecognizer;
        private WhistleRecognizer _whistleRecognizer;

        //private List<Combo> _combos;
        #endregion
    }
}