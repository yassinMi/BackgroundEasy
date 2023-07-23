using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mi.Common
{
    [Serializable]
    /// <summary>
    /// a serializable dictionary of key, grid sizes to allo saving the user dimentions , meant to nested as a singlton ConfigService property, 
    /// </summary>
    public class GridSpliterStateDictionary
    {

        private Dictionary<string, int> _sizes;
        [XmlIgnore]
        /// <summary>
        /// bridge between the serializable list and the consumer interface (the inexer), this should ot be public
        /// </summary>
        Dictionary<string, int> Sizes {
            get
            {
                if(_sizes== null)
                {
                    _sizes = new Dictionary<string, int>();
                    foreach (var item in KeySizePairs)
                    {
                        _sizes.Add(item.Key, item.Size);
                    }
                }
                return _sizes;
            }
        }

        public int? this[string key]
        {
            get
            {
                 return Sizes.ContainsKey(key)? Sizes[key]: (int?)null;
            }
            set
            {
                if(value == null)
                {
                    bool remove = Sizes.Remove(key);
                    if (remove)
                    {
                        push();
                    }
                }
                else
                {
                    Sizes[key] = value.Value;
                    push();
                }
                
            }
        }
        /// <summary>
        /// back to the list
        /// </summary>
        private void push()
        {
            this.KeySizePairs = this.Sizes.Select(s => new KeySizePair() { Size = s.Value, Key = s.Key }).ToArray();
        }

        [Serializable]
        public class KeySizePair
        {
            public string Key { get; set; }
            public int Size { get; set; }
        }
        /// <summary>
        /// this is for the serialization and should not be used, use the indexer [string] 
        /// </summary>
        [XmlArray("SizeDict")]
        [XmlArrayItem("Size")]
        public KeySizePair[] KeySizePairs { get; set; } = new KeySizePair[] { };
    }
}
