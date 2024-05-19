# JSharp

## Technologies
[<img align="left" alt="Csharp" width="36px" src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg" style="padding-right:10px;"/>][csharp]
[<img align="left" alt="dotnet" width="36px" src="https://upload.wikimedia.org/wikipedia/commons/thumb/7/7d/Microsoft_.NET_logo.svg/2048px-Microsoft_.NET_logo.svg.png" style="padding-right:10px;"/>][dotnet]

[csharp]: https://en.wikipedia.org/wiki/C_Sharp_(programming_language)
[dotnet]: https://en.wikipedia.org/wiki/.NET

<br>

## Overview

This project is a comprehensive image processing application designed to perform a wide range of operations on digital images. It provides a user-friendly interface for opening, manipulating, and analyzing images using various techniques and algorithms. The application is built using C# and WPF, leveraging the MVVM (Model-View-ViewModel) design pattern for clean separation of concerns and maintainability.

## Features

### Image Handling

- **Load and Display Images**:
  - Load and display grayscale images.
  - Load and display color images.
- **Save Images**: Save processed images to disk.
- **Duplicate Images**: Create duplicates of loaded images.

### Histogram Operations

- **Display Histogram**:
  - Graphically display the image histogram with real-time updates.
  - Display histogram data in tabular representation.
- **Histogram Modifications**:
  - Histogram stretching.
  - Histogram equalization.

### Image Conversion

- **Convert Color to Grayscale**: Convert color images to grayscale.
- **Convert RGB to Other Color Spaces**:
  - Convert RGB to HSV and separate into channels.
  - Convert RGB to LAB and separate into channels.
- **Channel Separation**: Separate RGB images into individual channels.

### Image Manipulation

- **Basic Operations**:
  - Negation
  - Grayscale level reduction (posterization)
- **Filtering**:
  - Convolution:
    - Using predefined masks.
    - Using interactively provided masks.
  - Median filtering
- **Morphological Operations**:
  - Dilation
  - Erosion
  - Morphological opening
  - Morphological closing
- **Edge Detection**: Edge detection using the Hough Transform.
- **Image Pyramids**: Image resizing using pyramids (upscaling and downscaling).

### Mathematical Operations

- **Unary and Binary Operations**:
  - Addition
  - Subtraction
  - Blending
  - Logical operations: AND, OR, NOT, XOR

### Advanced Image Processing

- **Skeletonization**: Reduce the image to its skeletal form.
- **Profile Line**: Generate and display a profile line of the image.
- **Thresholding**: Segment images using thresholding.
- **Inpainting**: Restore parts of images using inpainting.

## Getting Started

To get started with the application, follow these steps:

1. Clone the repository.
2. Open the solution in your preferred C# development environment.
3. Build the solution to restore dependencies and compile the application.
4. Run the application.

## License

This project is licensed under the [Apache License 2.0](https://opensource.org/license/apache-2-0/). See the LICENSE file for more details.
