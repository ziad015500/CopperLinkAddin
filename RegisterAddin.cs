using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

[ComRegisterFunction]
public static void RegisterFunction(Type t)
{
    string keyPath = @"SOFTWARE\SolidWorks\AddIns\" + t.GUID.ToString("B");

    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(keyPath))
    {
        key.SetValue("", 1);
        key.SetValue("Title", "CopperLink");
        key.SetValue("Description", "Busbar Generator Add-In");
    }
}

[ComUnregisterFunction]
public static void UnregisterFunction(Type t)
{
    string keyPath = @"SOFTWARE\SolidWorks\AddIns\" + t.GUID.ToString("B");
    Registry.LocalMachine.DeleteSubKey(keyPath, false);
}