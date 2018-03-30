//// Helpers/Settings.cs
//using Plugin.Settings;
//using Plugin.Settings.Abstractions;

using System;


namespace SimulatedDevice.Utils
{
    public interface ISettings
    {
        bool AddOrUpdateValue<T>(string key, T value);

        T GetValueOrDefault<T>(string key, T defaultValue);

        void Remove(string key);
    }
}