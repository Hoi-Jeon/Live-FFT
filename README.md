# Live FFT and Spectrogram
The purpose of this hobby project is for creating a GUI in Laptop, which can display either the time or frequency data coming from TI Launchpad microcontroller (MCU). The animated gif below shows what this programs can do.

![LiveFFT](Demo/LiveFFT.gif)

## Data acquisition in TI [TM4C123GXL](https://www.ti.com/tool/EK-TM4C123GXL)
- High Performance TM4C123GH6PM MCU
- Communication with Laptop using UART Serial communication
- Used one 12-bit ADC with microphone [KY-037](http://sensorkit.en.joy-it.net/index.php?title=KY-037_Microphone_sensor_module_(high_sensitivity)) is used
- FFT is done with [CMSIS DSP library](https://www.ti.com/lit/an/spma041g/spma041g.pdf?ts=1591993440214&ref_url=https%253A%252F%252Fwww.google.de%252F)

## Analog low pass filter
[Circuit](Demo/LPF_Circuit.gif)
Low pass filter design
Magnitude plot
[Circuit](Demo/LPF_Circuit_Magnitude.gif)
Picture
[Circuit](Demo/LPF_Circuit_Picture.gif)


## Graphical User Interface in C#
GUI is written in C# and its main function is to get either time data (ADC) or freqeuncy data (FFT) from TI Launchpad, and to dispaly in C# Chart.
- The version of Visual studio is Community 2019
  - NET Framework 4.7.2
  - WinForms (Not WPF)
- Time domain data (ADC)
  - ADC sampling frequency: 4096 Hz
  - Get 1 value from 2 byte, since TI Launchpad has 12bit ADC 
- Frequency domain data
  - Number of FFT line: 128
  - Highest frequency: up to 2048 Hz, but surely, the data between 1700 Hz and 2048 Hz would be contaminated with an aliasing
  - [A-weighting](https://en.wikipedia.org/wiki/A-weighting) is implemented
- Color map
  - "Jet" scale is used
  - One can code other scales with a book [Practical C# Charts and Graphics (Second Edition)](https://books.google.de/books/about/Practical_C_Charts_and_Graphics_Second_E.html?id=Z06wDwAAQBAJ&redir_esc=y)

## Open points
- Increasing a sampling frequency
- Improving the analog low pass filer performance
- 800 Hz noise coming from Microphone (KY-39) through TI Launch pad

## Remarks
Some parts of C# codes are got from internet search and I always tried to copy the codes **with reference** and **without violating any copyright**. Nevertheless, please let me know if there is any possible copyright infringement issue in the codes. Then, I will remove it from the repository immediately.
