# TensorRT10Sharp C#

**English** | **[中文](README.md)**

TensorRT10Sharp C# is a NVIDIA TensorRT10 wrapper library for the .NET platform, enabling developers to easily use TensorRT for high-performance deep learning inference in C#.

## ✨ Latest Updates

- ✅ **TensorRT 10 API Compatibility**: Fully adapted to TensorRT 10.x new APIs
- ✅ **Build System Optimization**: Improved CMake configuration and batch scripts
- ✅ **Enhanced Error Handling**: Better error detection and diagnostics
- ✅ **Complete Documentation**: Detailed build guides and troubleshooting
- ✅ **Cross-platform Support**: Optimized Windows build workflow
- 🎉 **Yolo11Sharp Integration**: New complete YOLO11 multi-mode inference library
- ✅ **Detection Feature Verification**: YOLO11 object detection tested and verified
- 📦 **NuGet Package Published**: Published to NuGet.org with package manager support

## 🚀 Subprojects

### Yolo11Sharp - YOLO11 Inference Library
High-performance YOLO11 multi-mode inference C# implementation built on TensorRT10Sharp.

**Supported Inference Modes:**
- 🔍 **Object Detection** - ✅ Tested and Verified
- 🏷️ **Image Classification** - 🔧 Development Complete
- 🎭 **Instance Segmentation** - 📋 Architecture Ready
- 📐 **Oriented Bounding Box (OBB)** - 📋 Architecture Ready
- 🤸 **Pose Estimation** - 📋 Architecture Ready

**Quick Start:**
```bash
cd Yolo11Sharp
Scripts\build.bat
Scripts\run.bat
```

Detailed Documentation: [Yolo11Sharp/README_EN.md](Yolo11Sharp/README_EN.md)

## 📦 NuGet Package Information

### Package Details
- **Package Name**: `alanxinn.TensorRT10Sharp`
- **Version**: `0.1.0`
- **Author**: TensorRTSharp Team
- **License**: MIT
- **Platform**: .NET 6.0, x64 (Windows)
- **Package URL**: https://www.nuget.org/packages/alanxinn.TensorRT10Sharp/
- **Project URL**: https://github.com/alanxinn/TensorRT10Sharp

### Package Features
- 🚀 **High Performance**: C# wrapper based on NVIDIA TensorRT 10
- 🔄 **Smart Conversion**: Automatic ONNX to TensorRT engine conversion
- 🛡️ **Resource Management**: Complete memory management and exception handling
- 📋 **Easy to Use**: Simple C# API interface
- ⚡ **GPU Acceleration**: Full utilization of CUDA GPU performance

### Dependencies
- **.NET 6.0** or higher
- **Windows x64** platform
- **NVIDIA GPU** with CUDA 11.8+ support
- **TensorRT 10.x** runtime libraries
- **Visual C++ Redistributable** (usually pre-installed)

## ⚡ Quick Start

### 0. Quick Installation (Recommended)

If you only want to use the TensorRT10Sharp library, install directly via NuGet:

```bash
# Create new project
dotnet new console -n MyTensorRTApp
cd MyTensorRTApp

# Install NuGet package
dotnet add package alanxinn.TensorRT10Sharp --version 0.1.0

# Start using
# Edit Program.cs, refer to usage examples below
```

### 1. Environment Requirements

#### Native (C++) Project
- **CUDA Toolkit** (Recommended version 11.8)
- **TensorRT** (Recommended version TensorRT-10.13.0.35)
- **Visual Studio 2019/2022** (with C++ development tools)
- **CMake** (version 3.10 or higher)

#### Managed (C#) Library Project
- **.NET 6.0 SDK**

## 📖 Usage Examples

### Installation and Reference

#### Method 1: NuGet Package Reference (Recommended)
```xml
<!-- Add to project file -->
<PackageReference Include="alanxinn.TensorRT10Sharp" Version="0.1.0" />
```

```bash
# Or install using dotnet CLI
dotnet add package alanxinn.TensorRT10Sharp --version 0.1.0
```

#### Method 2: Package Manager Console (Visual Studio)
```powershell
Install-Package alanxinn.TensorRT10Sharp -Version 0.1.0
```

#### Method 3: Project Reference
```xml
<!-- Direct library project reference -->
<ProjectReference Include="path\to\Managed\TensorRT10Sharp.csproj" />
```

### Basic TensorRT Usage

```csharp
using TensorRTSharp;

// Method 1: Load engine file directly
using var infer = new Nvinfer("Assets/yolo11n.engine");

// Method 2: Auto-convert ONNX model (if engine doesn't exist)
// Will automatically detect yolo11n.onnx and convert to yolo11n.engine
using var infer2 = new Nvinfer("yolo11n.engine");

// Method 3: Manual ONNX model conversion
bool success = Nvinfer.ConvertOnnxToEngine("model.onnx", 1024);
if (success)
{
    using var infer3 = new Nvinfer("model.engine");
}

// Get model information
Console.WriteLine($"Input count: {infer.GetInputCount()}");
Console.WriteLine($"Output count: {infer.GetOutputCount()}");

// Prepare input data
string inputName = infer.GetInputName(0);
Dims inputDims = infer.GetBindingDimensions(inputName);
float[] inputData = new float[inputDims.GetElementCount()];

// Load data and execute inference
infer.LoadInferenceData(inputName, inputData);
infer.Infer();

// Get results
string outputName = infer.GetOutputName(0);
float[] result = infer.GetInferenceResult(outputName);
```

### YOLO11 Object Detection Usage (Tested)

```csharp
using Yolo11Sharp.Core;
using Yolo11Sharp.Visualization;

// Create detector
using var detector = new Yolo11Detection("yolo11n.engine", "coco.names");

// Execute detection
var detections = detector.Infer("test.jpg");

// Visualize results
var outputImage = ImageVisualizer.DrawDetections("test.jpg", detections);
outputImage.Save("result.jpg");

// Output results
foreach (var detection in detections)
{
    Console.WriteLine($"{detection.ClassName}: {detection.Confidence:P2} " +
                     $"[{detection.X1:F1}, {detection.Y1:F1}, {detection.X2:F1}, {detection.Y2:F1}]");
}
```

## 🧪 Test Status

### ✅ Completed and Tested
- **Basic TensorRT Functions**: Engine loading, inference execution, result retrieval
- **ONNX Auto-conversion**: Smart detection and conversion of ONNX models to TensorRT engines
- **Library Architecture**: Standard .NET library with NuGet package distribution support
- **NuGet Package Publishing**: Successfully published to NuGet.org with package manager installation support
- **YOLO11 Object Detection**: Complete detection pipeline including preprocessing, inference, post-processing, visualization
- **Multi-mode Architecture**: Factory pattern, interface design, modular structure

### 🔧 Development Complete, Awaiting Testing
- **YOLO11 Image Classification**: Code implementation complete, awaiting model testing
- **Visualization Tools**: Detection box drawing, batch processing functionality

### 📋 Architecture Ready, Awaiting Implementation
- **Instance Segmentation**: Data models and interfaces defined
- **Oriented Bounding Box**: Data models and interfaces defined
- **Pose Estimation**: Data models and interfaces defined

## 🐛 Troubleshooting

### Common Issues

1. **DLL Loading Failure**
   ```
   Issue: Cannot find trt10.dll or its dependencies
   Solution: Ensure TensorRT libraries are in system PATH, or use run_example.bat
   ```

2. **TensorRT API Errors**
   ```
   Issue: API methods not found
   Solution: Ensure using TensorRT 10.x version, check library linking in CMakeLists.txt
   ```

3. **Build Failures**
   ```
   Issue: CMake configuration or compilation errors
   Solution: Check Visual Studio environment, ensure CUDA and TensorRT paths are correct
   ```

4. **YOLO11 Detection Result Anomalies**
   ```
   Issue: Coordinate errors or confidence anomalies
   Solution: Ensure using YOLO11 format model, check output dimensions are [1,84,8400]
   ```

5. **Model File Issues**
   ```
   Issue: Engine file loading failure
   Solution: Use trtexec to convert ONNX model, ensure TensorRT version compatibility
   ```

6. **NuGet Package Installation Issues**
   ```
   Issue: Package installation failure or package not found
   Solution: Ensure correct package name alanxinn.TensorRT10Sharp, check network connection
   ```

7. **Runtime Dependencies Missing**
   ```
   Issue: Cannot find trt10.dll or CUDA libraries at runtime
   Solution: Ensure TensorRT 10.x and CUDA 11.8+ are installed and added to system PATH
   ```

### Debugging Tips
- Use `run_example.bat` for complete environment detection
- Check build script output information
- Verify all dependencies are correctly installed
- Enable Yolo11Sharp debug logging: `Logger.CurrentLevel = LogLevel.Debug`

## 📊 Performance Benchmarks

### YOLO11 Detection Performance (Tested)
**Test Environment**: RTX 3080, Intel i7-10700K, 32GB RAM

| Model | Input Size | Inference Time | FPS | Memory Usage |
|-------|------------|----------------|-----|--------------|
| YOLO11n | 640×640 | ~15ms | ~67 | ~2GB |
| YOLO11s | 640×640 | ~25ms | ~40 | ~3GB |
| YOLO11m | 640×640 | ~45ms | ~22 | ~5GB |

*Note: Performance data is for reference only, actual performance depends on hardware configuration*

## 📚 Detailed Documentation

- [Native Project Documentation](Native/README_EN.md) - Detailed C++ project documentation
- [Managed Project Documentation](Managed/README_EN.md) - Detailed C# project documentation
- [Yolo11Sharp Project Documentation](Yolo11Sharp/README_EN.md) - Complete YOLO11 inference library documentation
- [Yolo11Sharp Architecture Documentation](Yolo11Sharp/ARCHITECTURE_EN.md) - Multi-mode inference architecture details
- [Build Documentation](Documentation/BUILD_README_EN.md) - Detailed build guide
- [Ignore Files Guide](Documentation/IGNORE_FILES_GUIDE_EN.md) - Git configuration documentation

## 🤝 Contributing Guidelines

1. **Code Standards**: Follow project coding conventions (see `.editorconfig`)
2. **Commit Standards**: Use clear commit messages
3. **Testing**: Ensure new features have corresponding tests and examples
4. **Documentation**: Update relevant documentation and README files
5. **Subprojects**: Prioritize implementing new features in Yolo11Sharp

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Related Links
- [NVIDIA TensorRT](https://developer.nvidia.com/tensorrt)
- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [YOLO11 Official Repository](https://github.com/ultralytics/ultralytics)

## 🙏 Acknowledgments

- https://github.com/guojin-yan/TensorRT-CSharp-API - Original TensorRT C# API reference
- [YOLO11](https://github.com/ultralytics/ultralytics) - Excellent object detection model
- [NVIDIA TensorRT](https://developer.nvidia.com/tensorrt) - High-performance inference engine

---

⭐ If this project helps you, please give us a Star! 