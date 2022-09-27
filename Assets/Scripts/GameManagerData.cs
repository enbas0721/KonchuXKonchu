using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SelectCharacter
{
    [CreateAssetMenu(fileName ="GameManagerData", menuName ="GameManagerData")]
    public class GameManagerData : ScriptableObject
    {
        [SerializeField]
        private string next_scene_name;

        private int character_num;

        // private static readonly Joycon.Button[] m_buttons = Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];
        private Joycon m_joyconL_1P;
        private Joycon m_joyconR_1P;
        private Joycon m_joyconL_2P;
        private Joycon m_joyconR_2P;

        private void OnEnable()
        {
            if (SceneManager.GetActiveScene().name == "CharacterTitle")
            {
                next_scene_name = "";
                character_num = 0;

                m_joyconL_1P = null;
                m_joyconR_1P = null;
                m_joyconL_2P = null;
                m_joyconR_2P = null;
            }
        }

        public void SetNextSceneName(string next_scene_name)
        {
            this.next_scene_name = next_scene_name;
        }

        public string GetNextSceneName()
        {
            return next_scene_name;
        }
        public void SetCharacterNum(int character_num)
        {
            this.character_num = character_num;
        }

        public int GetCharacterNum()
        {
            return character_num;
        }

        public void SetJoyConR_1P(Joycon m_joyconR_1P)
        {
            this.m_joyconR_1P = m_joyconR_1P;
        }

        public Joycon GetJoyconR_1P()
        {
            return m_joyconR_1P;
        }
        public void SetJoyConL_1P(Joycon m_joyconL_1P)
        {
            this.m_joyconL_1P = m_joyconL_1P;
        }
        public Joycon GetJoyconL_1P()
        {
            return m_joyconL_1P;
        }

        public void SetJoyConR_2P(Joycon m_joyconR_2P)
        {
            this.m_joyconR_2P = m_joyconR_2P;
        }
        public Joycon GetJoyconR_2P()
        {
            return m_joyconR_2P;
        }
        public void SetJoyConL_2P(Joycon m_joyconL_2P)
        {
            this.m_joyconL_2P = m_joyconL_2P;
        }
        public Joycon GetJoyconL_2P()
        {
            return m_joyconL_2P;
        }
    }
}
