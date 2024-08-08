using System.Collections.Generic;
namespace AP_Tinykin {
    public class ItemMap {
        public static Dictionary<long, string> SmallLiftables = new Dictionary<long, string> {
            {308202203, "SmallLiftable_PlayButton"},
            {308202204, "SmallLiftable_SphinxNose"},
            {308202208, "SmallLiftableBase_Picture"},
            {308202206, "SmallLiftable_Diamond"},
            {308202207, "SmallLiftable_ScrewDriver"}
        };

        public static Dictionary<long, string[]> Mail = new Dictionary<long, string[]> {
            {308202205, new string[] {"SmallLiftable_Letter_Under_Couch", "SmallLiftable_Letter_Bellow_HiFi", "SmallLiftable_Letter_under_Fragil", "SmallLiftable_Letter_Under_Piano"}}
        };

        public static Dictionary<long, string[]> Pollen = new Dictionary<long, string[]> {
            {308202210, new string[] {"Soap_Undercouch", "Soap_Sphinx", "Soap_Guirofrog", "Soap_Candles", "MasterPollen_PostOffice"}}
        };

        public static Dictionary<long, string> ItemsPerRoom = new Dictionary<long, string> {
            {308202203, "02_LivingRoom_v4"},
            {308202204, "02_LivingRoom_v4"},
            {308202208, "02_LivingRoom_v4"},
            {308202206, "02_LivingRoom_v4"},
            {308202207, "02_LivingRoom_v4"},
            {308202205, "02_LivingRoom_v4"},
            {308202210, "02_LivingRoom_v4"}
        };
    }
}
