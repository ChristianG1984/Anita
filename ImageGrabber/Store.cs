using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;

namespace ImageGrabber
{
    public class Store
    {
        public Store()
        {
            _settings = new Dictionary<string, string>();
            _tempSettings = new Dictionary<string, string>();
        }

        public void ReceiveSettingForStorage(KeyValuePair<string, string> setting)
        {
            if (_isDistributingSettings == true) {
                _tempSettings[setting.Key] = setting.Value;
                return;
            }

            _settings[setting.Key] = setting.Value;
            SaveSettings();
        }

        public void DistributeSettings()
        {
            _isDistributingSettings = true;
            LoadSettings();
            foreach (var setting in _settings) {
                OutputSetting(setting);
            }
            _isDistributingSettings = false;

            if (_tempSettings.Count <= 0) { return; }
            foreach (var setting in _tempSettings) {
                ReceiveSettingForStorage(setting);
            }
            _tempSettings.Clear();
        }

        public event Action<KeyValuePair<string, string>> OutputSetting;

        private void SaveSettings()
        {
            using (var streamWriter = new StreamWriter(FullStoragePath, false, new UTF8Encoding())) {
                foreach (var setting in _settings) {
                    streamWriter.Write(setting.Key);
                    streamWriter.Write(" = ");
                    streamWriter.WriteLine(setting.Value);
                }
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        private void LoadSettings()
        {
            if (File.Exists(FullStoragePath) == false) { return; }
            _settings.Clear();
            using (var streamReader = new StreamReader(FullStoragePath, new UTF8Encoding(), false)) {
                while (streamReader.EndOfStream == false) {
                    var line = streamReader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line) == true) { continue; }
                    var index = line.IndexOf('=');
                    if (index == -1) { continue; }
                    var key = line.Substring(0, index).Trim();
                    var value = line.Substring(index + 1).Trim();

                    try {
                        _settings.Add(key, value);
                    }
                    catch { continue; }
                }
                streamReader.Close();
            }
        }

        private string FullStoragePath
        {
            get { return Path.Combine(StorageDirectory, StorageFileName); }
        }

        private string StorageDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_storageDirectory) == true) {
                    return DefaultDirectory;
                } else {
                    return _storageDirectory;
                }
            }

            set
            {
                _storageDirectory = value;
            }
        }

        private string StorageFileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_storageFileName) == true) {
                    return DefaultFileName;
                } else {
                    return _storageFileName;
                }
            }

            set
            {
                _storageFileName = value;
            }
        }

        private string DefaultDirectory
        {
            get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); }
        }

        private string DefaultFileName
        {
            get { return "Config.conf"; }
        }

        private Dictionary<string, string> _settings;
        private Dictionary<string, string> _tempSettings;
        private string _storageDirectory = string.Empty;
        private string _storageFileName = string.Empty;
        private volatile bool _isDistributingSettings = false;
    }
}
