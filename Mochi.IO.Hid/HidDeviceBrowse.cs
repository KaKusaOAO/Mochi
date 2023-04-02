using System.Collections.Generic;
using Mochi.IO.Hid.Native;

namespace Mochi.IO.Hid;

public class HidDeviceBrowse
{
    public static List<HidDeviceInfo> Browse()
    {
        List<HidDeviceInfo> result = new();
        unsafe
        {
            var item = HidApi.Enumerate();
            var root = item;
            while (item != null)
            { 
                var entry = new HidDeviceInfo(*item);
                result.Add(entry);
                item = item->Next;
            }
                
            HidApi.FreeEnumeration(root);
        }

        return result;
    }
}