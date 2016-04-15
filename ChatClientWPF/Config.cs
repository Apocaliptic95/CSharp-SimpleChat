using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF
{
    class Config
    {
        private string _value;
        private string _key;

        public string Value
        {
            set { _value = value; }
            get { return _value; }
        }

        public string Key
        {
            set {  }
            get { return _key; }
        }

        public Config(string key, string value)
        {
            _value = value;
            _key = key;
        }
    }
}
