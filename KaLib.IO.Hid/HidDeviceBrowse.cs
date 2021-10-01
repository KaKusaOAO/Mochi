using System.Collections.Generic;
using KaLib.IO.Hid.Native;

namespace KaLib.IO.Hid
{
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
                    item = item->next;
                }
                
                HidApi.FreeEnumeration(root);
            }

            return result;
        }
    }
}