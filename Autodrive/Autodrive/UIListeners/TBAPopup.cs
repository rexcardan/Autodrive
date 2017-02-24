using System;
using System.Collections.Generic;
using System.Linq;

namespace Autodrive.UIListeners
{
    public class TbaPopup
    {
        public const int BmClick = 0x00F5;
        private readonly IntPtr _buttonPtr;

        public TbaPopup(IntPtr pointer)
        {
            List<string> messages =
                WinAPI.GetAllChildrenWindowHandles(pointer, 10).Select(WinAPI.GetWindowCaption).ToList();
            Instructions = messages.Last();
            _buttonPtr =
                WinAPI.GetAllChildrenWindowHandles(pointer, 10)
                    .FirstOrDefault(b => WinAPI.GetWindowCaption(b).Contains("OK"));
        }

        public string Instructions { get; set; }

        public void PressOk()
        {
            if (_buttonPtr != IntPtr.Zero)
            {
                WinAPI.PostMessage(_buttonPtr, BmClick, 0, 0);
                WinAPI.PostMessage(_buttonPtr, BmClick, 0, 0);
            }
        }
    }
}