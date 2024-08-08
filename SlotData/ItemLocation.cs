using UnityEngine;
using UnityEngine.SceneManagement;

namespace AP_Tinykin {
    public class ItemLocation {
        public long ap_id;
        public string item_name;
        public string player_name;
        public int type;

        public ItemLocation(long ap_id, string item_name, string player_name, int type) {
            this.ap_id = ap_id;
            this.item_name = item_name;
            this.player_name = player_name;
            this.type = type;
        }
    }
}