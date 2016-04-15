using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace ChatClientWPF
{
    class ConfigFile
    {
        private List<Config> _configList = new List<Config>();

        public ConfigFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader("config.Stefan"))
                {
                    String line;
                    while ((line = sr.ReadLine())!=null)
                    {
                        String[] lineArray = line.Split('=');
                        _configList.Add(new Config(lineArray[0], lineArray[1]));
                    }
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                _configList.Add(new Config("nick", "NoName"));
                _configList.Add(new Config("ip", "127.0.0.1"));
                _configList.Add(new Config("port", "1"));
            }
        }

        public void Change(string key, string value)
        {
            _configList.Single(x => x.Key.Equals(key)).Value=value;  
        }

        public Config Search(string key)
        {
            return _configList.Find(x => x.Key.Equals(key));
        }

        public void Save()
        {
            using (StreamWriter file = new StreamWriter("config.Stefan"))
            {
                foreach (var element in _configList)
                {
                    file.WriteLine(element.Key+"="+element.Value);
                }
                file.Close();
            }
        }
    }
}
