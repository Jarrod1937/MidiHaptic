Read ME - MIDIHAPTIC

What is this project?
The idea is that one can passively learn via haptic feedback to produce muscle memory. This project takes generic MIDI files and interprets them into per finger haptic feedback to induce such memory effects. This software is meant to be paired with hardware, which communicates via serial. For those seeking to just experiment with the program, a precompiled binary exists with

What Hardware?
The hardware can be custom made to anything desired but must accept a stream of HEX codes delimited by a '+' to signify the end of a sequence. The serial output, by default, is a number representing each finger to be played. The left pinky is 1 and the right pinky is 10.

Example Firmware?
The 'midihaptic.ino' file contains basic code developed for the Arduino platform, the platform originally used for prototyping. Here you can see individual characters from the serial stream are paired and then interpreted from HEX to their decimal value, which is then used to turn a specific digital output to HIGH. Other platforms can easily be used as well.
Note that the MIDIHaptic code can easily output the finger and key HEX pairs which can be further used to give spatial awareness when playing an actual piano. Likewise, one could also output the velocity figures to soften or strengthen the haptic feedback based on the needed force of the key press.

How to Use?
Compile the code, or use the precompiled binary in the debug bin folder. With the program running input your max finger span in terms of the maximum number of white keys you can span with one hand, then click change. From here, you can click 'Load MIDI File' or drag and drop a midi file into the program. The program will then interpret the file and enable the 'play' button when finished. One thing to note is that the software sometimes has an issue determining the proper playspeed, this is why the software allows both an increase and a decrease playback option. From here, if you have hardware hooked up, make sure the software has assigned the proper COM port and that serial is enabled. If you have no hardware, make sure the serial output is disabled or you will receive an error. Press the 'play' button to start. You can also adjust the number of 'chunks', default is 1. If you change chunk count to 2, the file will be split into 2 equal lengths, same for 3 and so on. You can then select the chunk segment you wish to practice. In actuality, passive learning via feedback shouldn't be done at a realtime pace, you can adjust the playback speed down for this. Likewise, you can disable audio playback so you just have the feedback with no audio. There is also a 'sticky key' feature which is enabled by default. Whereas a key press may only last a short period of time, sticky keys will keep the graphical display from clearing the hit key until the next round of active keys comes around. This allows one to more easily see the keys being hit. 


Piano Samples:
Unable to find a license for the piano samples that were derived from the University of Iowa, the samples are included with their thanks. The piano audio samples are cleaned up versions of the notes provided by them at the URL:
http://theremin.music.uiowa.edu/MISpiano.html

Dependencies:
The software largely only relies on .NET and it's base includes, however, for audio blending purposes DirectX (DX) runtime is required to be installed. In particular, the Microsoft.DirectX.AudioVideoPlayback class is used, which handles the audio blending automatically.



License:
This project is licensed under GNU Public License V3.0

    Copyright (C) 2014  Jarrod Christman

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

