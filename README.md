# QR Reader Project

## Overview
This project implements a QR code reader using OpenCV and C# for recognizing QR codes under various conditions on photos. The reader can handle different scenarios such as QR codes on plain backgrounds, overlaid on images, rotated, or skewed. The project is designed to detect and decode QR codes even with imperfect contrast or color.

## Features
- Works with QR codes placed over images.
- Handles QR codes with imperfect contrast (not perfectly white or black).
- Supports QR code detection for rotated.
- Supports a lot of QR code detection when the QR code is skewed.

## Testing Methodology
The QR code reader was tested by comparing its output to the expected result. The testing covered various QR code scenarios, such as:
1. Basic black and white QR codes.
2. QR codes overlaid on color images with imperfect white and black colors.
3. Rotated QR codes.
4. Skewed QR codes.

For each test case, the QR code reader's output was automatically compared with the expected output. The validation was done by checking the decoded result programmatically, ensuring that the reader's output matched the expected result in terms of decoded QR content. This was done by verifying the decoded data against predefined expected values for each image, ensuring accuracy and reliability.

## Future Work
- Improving the handling of severely skewed or perspective-distorted QR codes.
- Enhancing image preprocessing to better manage noisy or complex backgrounds.
- Adding more advanced algorithms for high-distortion scenarios (e.g., damaged QR codes).
