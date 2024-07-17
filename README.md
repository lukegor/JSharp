# JSharp

## Technologies
[<img align="left" alt="Csharp" width="36px" src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg" style="padding-right:10px;"/>][csharp]
[<img align="left" alt="dotnet" width="36px" src="https://upload.wikimedia.org/wikipedia/commons/thumb/7/7d/Microsoft_.NET_logo.svg/2048px-Microsoft_.NET_logo.svg.png" style="padding-right:10px;"/>][dotnet]
[<img align="left" alt="wpf" width="36px" src="https://dotnetexpertshome.files.wordpress.com/2019/07/wpf.png" style="padding-right:10px;"/>][wpf]
[<img align="left" alt="xunit" width="36px" src="https://avatars.githubusercontent.com/u/2092016?s=200&v=4" style="padding-right:10px;"/>][xunit]

[csharp]: https://en.wikipedia.org/wiki/C_Sharp_(programming_language)
[dotnet]: https://en.wikipedia.org/wiki/.NET
[wpf]: https://en.wikipedia.org/wiki/Windows_Presentation_Foundation
[xunit]: https://xunit.net/

<br>

## Overview

This project is a comprehensive image processing application designed to perform a wide range of operations on digital images. It provides a user-friendly interface for opening, manipulating, and analyzing images using various techniques and algorithms. The application is built using C# and WPF, leveraging the MVVM (Model-View-ViewModel) design pattern for clean separation of concerns and maintainability.

## User Interface
![ui](https://github.com/user-attachments/assets/710ed3d2-7beb-4bbc-a4b1-2eb09a3e218e)


## Features

### Image Handling

- **Load and Display Images**:
  - Load and display grayscale images.
  - Load and display color images.
- **Save Images**: Save processed images to disk.
- **Duplicate Images**: Create duplicates of loaded images.
- **Copy Image**: Copy image to clipboard.

### Image Conversion

- **Convert Color to Grayscale**: Convert color images to grayscale.
- **Convert RGB to Other Color Spaces**:
  - Convert RGB to HSV and separate into channels.
  - Convert RGB to LAB and separate into channels.
- **Channel Separation**: Separate RGB images into individual channels.

### Image Processing

- **Basic Operations**:
  - Negation
  - Grayscale level reduction (posterization)
- **Histogram-based Operations**:
  - Histogram stretching.
  - Histogram equalization.
- **Filtering**:
  - Convolution:
    - Using predefined masks.
    - Using interactively provided masks.
  - Median filtering
- **Unary and Binary Operations**:
  - Addition
  - Subtraction
  - Blending
  - Logical operations: AND, OR, NOT, XOR
- **Thresholding**: Segment images using thresholding.
- **Morphological Operations**:
  - Basic:
    - Dilation
    - Erosion
    - Morphological opening
    - Morphological closing
  - Advanced:
    - Skeletonization: Reduce the image to its skeletal form.
- **Edge Detection**: Edge detection using the Hough Transform.
- **Image Pyramids**: Image resizing using pyramids (upscaling and downscaling).
- **Image Transformation**:
  - Rotate image by 90 degrees clockwise.
  - Flip image by 180 degrees.
- **Compression**:
  - RLE Compression

### Advanced Image Processing

- **Watershed**: Perform segmentation with Watershed.
- **Inpainting**: Restore parts of images using inpainting.
- **GrabCut**: Extract foreground from selected part of image.

### Analyze

- **Display Histogram**:
  - Graphically display the image histogram with real-time updates.
  - Display histogram data in tabular representation.
- **Profile Line**: Generate and display a profile line of the image.
- **Simple Analysis**: Perform simple image analysis.
- **Detailed Analysis**: Perform detailed image analysis.

## Getting Started

### Option 1: Cloning the Repository (recommended for Developers)

If you want to contribute to the project or explore the source code, follow these steps:

1. **Clone the repository**
    ```sh
    git clone https://github.com/lukegor/JSharp.git
    ```
2. **Navigate to the project directory**
    ```sh
    cd JSharp
    ```
3. **Open the solution file in Visual Studio**
    - Open the `JSharp.sln` file located in the root directory of the project with Visual Studio.

4. **Run the application**
    - Press `F5` or select `Debug` > `Start Debugging`.

### Option 2: Downloading the Release (recommended for End Users)

If you just want to use the application without modifying the source code, follow these steps:

1. **Download the latest release**
    - Go to the [Releases](https://github.com/lukegor/JSharp/releases) page.
    - Download the executable (`.exe`) and the program database file (`.pdb`) for debugging.

2. **Run the application**
    - Navigate to the folder where you downloaded the files.
    - Double-click the executable file (`.exe`) to run the application.

### Prerequisites

Make sure you have the following installed:

- **For developers:**
  - [Visual Studio](https://visualstudio.microsoft.com/) (with the .NET desktop development workload)
  - [.NET SDK](https://dotnet.microsoft.com/download)

- **For end users:**
  - No specific prerequisites (as the application package should include everything needed to run).

### Additional Resources

- **Issues:** Report any issues or feature requests on the [Issues](https://github.com/lukegor/JSharp/issues) page.


## License

This project is licensed under the [Apache License 2.0](https://opensource.org/license/apache-2-0/). See the LICENSE file for more details.
