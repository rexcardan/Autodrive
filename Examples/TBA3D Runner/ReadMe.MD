This program is a task listener that runs on a Windows machine running the PTW MP3 Mephysto scanning software. Its design is to let Mephysto do most of the work, but allows substantial further automation by enabling linac control.

##How To Use It:
1. Hook up an Autodrive compatible [RS232 to P/S2 interface device](http://www.vetra.com/335text.html)
2. Start a Mephysto task list and beam on manually
3. Start this code and watch the magic

##How It Works
This example works by using the Windows messaging system to await popups that Mepyhysto generates. It parses the popups which instruct the user what to do next. It can handle:
* Energy change
* FOV change
* Accessory change (you will hear a beep and it will wait for the user to affirm a physical accessory change occured on the machine)

Based on the prompt, it then uses the rest of Autodrive linac controllers to push the commands the service mode of the linac.
