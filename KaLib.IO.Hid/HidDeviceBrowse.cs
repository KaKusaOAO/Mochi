using System.Collections.Generic;
using KaLib.IO.Hid.Native;

namespace KaLib.IO.Hid
{
    public class HidDeviceBrowse
    {
        public static List<HidDeviceInfo> Browse()
        {
            var result = new List<HidDeviceInfo>();
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
}