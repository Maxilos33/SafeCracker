using System;

namespace SafeCracker
{
    public class SafeCrackerConfig
    {
        public string MinDamageComment = "Changes minimum Damage to reinforced blocks, Vanilla value = 2, Default value = 30";
        public int MinDamage = 30;
        public string MaxDamageComment = "Changes minimum Damage to reinforced blocks, Vanilla value = 2, Default value = 60";
        public int MaxDamage = 60;

        public SafeCrackerConfig()
        {
            MinDamageComment = "Changes minimum Damage to reinforced blocks, Vanilla value = 2, Default value = 30";
            MinDamage = 30;
            MaxDamageComment = "Changes minimum Damage to reinforced blocks, Vanilla value = 2, Default value = 60";
            MaxDamage = 60;
        }
    }
}