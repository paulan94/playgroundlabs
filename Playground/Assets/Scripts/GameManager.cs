using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaulLabs
{
    //this class manages game states, starts/ends/resets the game state.
    public class GameManager : MonoBehaviour
    {

        [Range(10, 100)] public int InitNodeMin;

        public int TargetSum;

        public void StartGame()
        {
            //choose random # for target given some tunable range, like 10-100, then generate some tree structure with nodes < than that #.
        }

        private void Start()
        {
            StartGame();
        }
    }
}
