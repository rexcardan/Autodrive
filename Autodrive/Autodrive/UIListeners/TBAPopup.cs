using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Autodrive.UIListeners
{
    public class TbaPopup
    {
        public const int BmClick = 0x00F5;
        private readonly IntPtr _buttonPtr;

        public ManualResetEvent ResetEvent { get; set; } = new ManualResetEvent(true);

        public TbaPopup(IntPtr pointer, bool outOfLimitsPopup = false)
        {
            List<string> messages =
                WinAPI.GetAllChildrenWindowHandles(pointer, 10).Select(WinAPI.GetWindowCaption).ToList();
            Instructions = messages.Last();
            if (outOfLimitsPopup)
            {
                //BUTTON SAYS YES
                _buttonPtr =
                 WinAPI.GetAllChildrenWindowHandles(pointer, 10)
                   .FirstOrDefault(b => WinAPI.GetWindowCaption(b).Contains("Yes"));
            }
            else
            {
                //BUTTON SAYS OK
                _buttonPtr =WinAPI.GetAllChildrenWindowHandles(pointer, 10)
                   .FirstOrDefault(b => WinAPI.GetWindowCaption(b).Contains("OK"));
            }
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