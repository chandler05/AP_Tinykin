using UnityEngine;
using UnityEngine.SceneManagement;

namespace AP_Tinykin {
    public class ServerSettings {
        public GoalType goal;
        public int tinykinUnlocks; 

        public ServerSettings(GoalType goal, int tinykinUnlocks) {
            this.goal = goal;
            this.tinykinUnlocks = tinykinUnlocks;
        }

        public ServerSettings() {
            goal = GoalType.FindArdwen;
            tinykinUnlocks = 1;
        }
    }
}