using System;
using System.Collections.Generic;
using UnityEngine;

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

        #region private_attributes
        private Lasp.SpectrumAnalyzer _analyzer = null;
        #endregion

        #region public_attributes
        public SoundRecognizer[] soundRecognizers;
        #endregion

        #region private_methods
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                if (_soundRecognizers == null)
                    _soundRecognizers = new List<SoundRecognizer>();

                foreach (SoundRecognizer s in soundRecognizers)
                    _soundRecognizers.Add(s);

                _instance = this;
            }
        }

        private void Start()
        {
            if (_analyzer == null)
                _analyzer = GetComponent<Lasp.SpectrumAnalyzer>();
        }

        private void Update()
        {
            foreach (SoundRecognizer sr in _soundRecognizers)
            {
                Debug.Log(sr.name + " combo: "+ (((ClickRecognizer)sr).combo+1) +  " - " + sr.Recognize(_analyzer.logSpectrumSpan.ToArray()));
            }
        }
        #endregion

        #region public_methods
        public int GetResolution() => _analyzer.resolution;
        #endregion

        #endregion

        #region PUBLIC_METHODS
        public void AddRecognizer(SoundRecognizer sr) => _soundRecognizers.Add(sr);

        public bool RemoveRecognizer(SoundRecognizer sr)
        {
            if (_soundRecognizers.Count > 0)
                return _soundRecognizers.Remove(sr);

            return false;
        }

        public List<SoundRecognizer> GetRecognizers() => _soundRecognizers;

        public void SetReconizerDelegate()
        {
        }

        #endregion

        #region PRIVATE_ATTRIBUTES
        private List<SoundRecognizer> _soundRecognizers = null;
        #endregion
    }
}