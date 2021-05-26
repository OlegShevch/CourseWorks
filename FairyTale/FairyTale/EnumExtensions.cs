using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FairyTale.Properties;

namespace FairyTale
{
    public class EnumExtensions
    {
        public static string GetItem<TE>(int value)
        {
            return Resources.ResourceManager.GetString(string.Format("{0}_{1}", typeof(TE).Name, Enum.GetName(typeof(TE), value)));
        }

        public static TE Parse<TE>(string name)
        {
            return (TE) Enum.Parse(typeof(TE), name);
        }

        public static IEnumerable<EnumItem> GetItems<TE>()
        {
            return Enum.GetValues(typeof(TE)).Cast<int>().Select(item => new EnumItem
            {
                Id = item,
                Name = Resources.ResourceManager.GetString(string.Format("{0}_{1}", typeof(TE).Name, Enum.GetName(typeof(TE), item)))
            });
        }
    }

    public class EnumItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}