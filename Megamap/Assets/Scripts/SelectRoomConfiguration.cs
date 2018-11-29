using UnityEngine;

namespace Megamap {

    public class SelectRoomConfiguration : SingletonBehaviour<SelectRoomConfiguration> {

        [Header("Room Configuration")]
        public Material normalMaterial = null;
        public Material hoverMaterial = null;
        public Material errorMaterial = null;

        [Header("Ball Configuration")]
        public GameObject ballPrefab = null;
        public int ballMinimum = 9;
        public int targetRoomBallMinimum = 12;
        public int ballMaximum = 15;

        private int numBallsTargetRoom = 0;
        public int NumBallsTargetRoom { get { return numBallsTargetRoom; } }

        public void RandomizeBallNumbers()
        {
            numBallsTargetRoom = Random.Range(targetRoomBallMinimum, ballMaximum + 1);

        }

        protected override void Awake()
        {
            base.Awake();

            RandomizeBallNumbers();
        }
    }

}
