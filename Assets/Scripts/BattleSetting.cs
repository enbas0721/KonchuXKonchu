using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SelectCharacter
{
    public class BattleSetting : MonoBehaviour
    {
        private GameManagerData gameManagerData;

        private static readonly Joycon.Button[] m_buttons = Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

        private List<Joycon> m_joycons;
        private Joycon m_joyconL_1P;
        private Joycon m_joyconR_1P;
        private Joycon m_joyconL_2P;
        private Joycon m_joyconR_2P;

        // 選択したキャラクタを表す変数
        private int character_1P = 0;
        private int character_2P = 1;

        private bool selected_1P = false;
        private bool selected_2P = false;

        // Start is called before the first frame update
        void Start()
        {
            gameManagerData = FindObjectOfType<GameManager>().GetGameManagerData();

            m_joycons = JoyconManager.Instance.j;

            if (m_joycons == null || m_joycons.Count <= 0) return;

            m_joyconL_1P = m_joycons.Find(c => c.isLeft);
            m_joyconR_1P = m_joycons.Find(c => !c.isLeft);
            m_joyconL_2P = m_joycons.Find(c => c.isLeft);
            m_joyconR_2P = m_joycons.Find(c => !c.isLeft);

            gameManagerData.SetJoyConL_1P(m_joyconL_1P);
            gameManagerData.SetJoyConR_1P(m_joyconR_1P);
            gameManagerData.SetJoyConL_2P(m_joyconL_2P);
            gameManagerData.SetJoyConR_2P(m_joyconR_2P);

        }
        // Update is called once per frame
        void Update()
        {
            if(selected_1P && selected_2P)
            {
                SceneManager.LoadScene("Main");
            }
            // AかBどちらを押すかによってかぶとむし、くわがたをわける
            // 押したらselected=true
        }
    }
}