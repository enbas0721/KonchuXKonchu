using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelectCharacter
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager gameManager;

        [SerializeField]
        private GameManagerData gameManagerData = null;

        private void Awake()
        {
            if (gameManager == null)
            {
                gameManager = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public GameManagerData GetGameManagerData()
        {
            return gameManagerData;
        }
    }
}