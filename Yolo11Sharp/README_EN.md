# Yolo11Sharp - YOLO11 TensorRT C# Inference Library

**English** | **[中文](README.md)**

🚀 High-performance YOLO11 multi-mode inference C# implementation based on TensorRT10Sharp, supporting object detection, image classification, instance segmentation, oriented bounding box detection, and pose estimation.

[![.NET](https://img.shields.io/badge/.NET-6.0+-blue.svg)](https://dotnet.microsoft.com/download)
[![TensorRT](https://img.shields.io/badge/TensorRT-10.x-green.svg)](https://developer.nvidia.com/tensorrt)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## ✨ Key Features

### 🎯 Multi-Mode Inference Support
- **🔍 Object Detection**: High-precision object detection and localization
- **🏷️ Image Classification**: Single-label and multi-label classification
- **🎭 Instance Segmentation**: Pixel-level object segmentation
- **📐 Oriented Bounding Box (OBB)**: Rotated object detection
- **🤸 Pose Estimation**: Human keypoint detection

### ⚡ High-Performance Features
- **GPU Acceleration**: TensorRT optimization with CUDA acceleration support
- **Memory Optimization**: Smart memory management, avoiding memory leaks
- **Batch Processing**: Support for single and batch image processing
- **Real-time Inference**: Millisecond-level inference speed

### 🏗️ Architecture Advantages
- **Modular Design**: Clear interface separation, easy to extend
- **Factory Pattern**: Unified inference object creation
- **Type Safety**: Strongly-typed C# API
- **Exception Handling**: Comprehensive error handling mechanism

## 📋 System Requirements

### Required Components
- **.NET 6.0 SDK** or higher
- **CUDA 11.8+** (supported NVIDIA GPU)
- **TensorRT 10.x**
- **Visual C++ Redistributable 2019/2022**

### Supported Platforms
- **Operating System**: Windows 10/11 x64
- **GPU**: NVIDIA GeForce GTX 1060 or higher
- **Memory**: Recommended 8GB+ RAM

## 🏗️ Project Architecture

```
Yolo11Sharp/                    # Project root directory
├── src/                        # Source code
│   ├── Core/                   # Core inference engine
│   │   ├── InferenceMode.cs    # Inference mode enumeration
│   │   ├── IYolo11Inference.cs # Inference interface definition
│   │   ├── Yolo11Engine.cs     # Generic inference engine
│   │   ├── Yolo11Detection.cs  # Object detection implementation
│   │   ├── Yolo11Classification.cs # Image classification implementation
│   │   └── Yolo11InferenceFactory.cs # Inference factory
│   ├── Models/                 # Data models
│   │   ├── DetectionResult.cs  # Detection results
│   │   ├── ClassificationResult.cs # Classification results
│   │   ├── SegmentationResult.cs # Segmentation results
│   │   ├── OrientedBoundingBoxResult.cs # OBB results
│   │   ├── PoseEstimationResult.cs # Pose estimation results
│   │   └── PreprocessResult.cs # Preprocessing results
│   ├── Utils/                  # Utility classes
│   │   ├── ConfigurationManager.cs # Configuration management
│   │   └── Logger.cs           # Logging utility
│   └── Visualization/          # Visualization
│       └── ImageVisualizer.cs  # Image visualization tool
├── Examples/                   # Example programs
│   └── Program.cs              # Main program entry
├── Assets/                     # Resource files
│   ├── coco.names              # COCO class names
│   └── test.jpg                # Test image
├── Scripts/                    # Script files
│   ├── build.bat               # Build script
│   ├── run.bat                 # Run script
│   └── clean.bat               # Clean script
├── bin/Release/net6.0/         # Build output
│   ├── Yolo11Sharp.exe         # Main program
│   └── Yolo11Sharp.dll         # Program library
├── Yolo11Sharp.csproj          # Project file
├── README.md                   # This file
├── ARCHITECTURE.md             # Architecture documentation
└── RENAME_SUMMARY.md           # Rename record
```

## 🚀 Quick Start

### 1. Environment Setup

```bash
# Clone project
git clone <repository-url>
cd TensorRT10Sharp/Yolo11Sharp

# Check .NET SDK
dotnet --version
```

### 2. Build Project

```bash
# Use build script
Scripts\build.bat

# Or manual build
dotnet build --configuration Release
```

### 3. Prepare Model Files

Convert YOLO11 ONNX models to TensorRT engines:
```bash
# Object detection model
trtexec --onnx=yolo11n.onnx --saveEngine=yolo11n.engine --fp16

# Classification model
trtexec --onnx=yolo11n-cls.onnx --saveEngine=yolo11n-cls.engine --fp16
```

### 4. Run Examples

```bash
# Use run script
Scripts\run.bat

# Or run directly
bin\Release\net6.0\Yolo11Sharp.exe

# Specify parameters
Yolo11Sharp.exe yolo11n.engine test.jpg result.jpg
```

## 💻 Usage Examples

### Object Detection

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

### Image Classification

```csharp
using Yolo11Sharp.Core;

// Create classifier
using var classifier = new Yolo11Classification("yolo11n-cls.engine", "imagenet.names");

// Get Top-5 classification results
var results = classifier.GetTopK("test.jpg", 5);

// Output results
foreach (var result in results)
{
    Console.WriteLine($"{result.ClassName}: {result.Confidence:P2}");
}
```

### Using Factory Pattern

```csharp
using Yolo11Sharp.Core;

// Auto-detect inference mode
var mode = Yolo11InferenceFactory.DetectInferenceMode("model.engine");

// Create inference object
var inference = Yolo11InferenceFactory.CreateInference(mode, "model.engine");

// Execute inference
var results = inference.Infer("test.jpg");
```

### Configuration Management

```csharp
using Yolo11Sharp.Utils;

// Use configuration manager
var enginePath = ConfigurationManager.GetAssetPath("yolo11n.engine");
var classNamesPath = ConfigurationManager.GetAssetPath("coco.names");
var outputDir = ConfigurationManager.GetOutputDirectory();

// Check file existence
if (ConfigurationManager.FileExists(enginePath))
{
    // Execute inference...
}
```

## 🔧 Configuration Options

### Detection Parameters

```csharp
var detector = new Yolo11Detection(
    enginePath: "yolo11n.engine",
    classNamesPath: "coco.names",
    confThreshold: 0.25f,    // Confidence threshold
    nmsThreshold: 0.45f      // NMS threshold
);

// Runtime adjustment
detector.ConfidenceThreshold = 0.5f;
```

### Classification Parameters

```csharp
var classifier = new Yolo11Classification(
    enginePath: "yolo11n-cls.engine",
    classNamesPath: "imagenet.names",
    confThreshold: 0.1f      // Classification confidence threshold
);
```

### Logging Configuration

```csharp
using Yolo11Sharp.Utils;

// Set log level
Logger.CurrentLevel = LogLevel.Info;

// Use logging
Logger.Info("Starting inference...");
Logger.Warning("Model file not found");
Logger.Error("Inference failed");
```

## 📊 Performance Benchmarks

### Test Environment
- **GPU**: RTX 3080 (10GB)
- **CPU**: Intel i7-10700K
- **Memory**: 32GB DDR4
- **CUDA**: 11.8
- **TensorRT**: 10.0

### Performance Data

| Model | Input Size | Inference Time | FPS | Memory Usage |
|-------|------------|----------------|-----|--------------|
| YOLO11n | 640×640 | ~15ms | ~67 | ~2GB |
| YOLO11s | 640×640 | ~25ms | ~40 | ~3GB |
| YOLO11m | 640×640 | ~45ms | ~22 | ~5GB |
| YOLO11l | 640×640 | ~70ms | ~14 | ~8GB |

*Note: Performance data is for reference only, actual performance depends on hardware configuration and model complexity*

## 🛠️ Development Guide

### Adding New Inference Modes

1. **Define Result Model**:
```csharp
public class CustomResult
{
    public float Confidence { get; set; }
    public string Label { get; set; }
    // Other properties...
}
```

2. **Implement Inference Class**:
```csharp
public class Yolo11Custom : Yolo11InferenceBase<CustomResult>
{
    public override InferenceMode Mode => InferenceMode.Custom;
    
    protected override List<CustomResult> PostProcess(float[] rawOutput, PreprocessResult preprocessResult)
    {
        // Implement post-processing logic
    }
}
```

3. **Update Factory**:
```csharp
// Add creation method in Yolo11InferenceFactory
public static Yolo11Custom CreateCustom(string enginePath, ...)
{
    return new Yolo11Custom(enginePath, ...);
}
```

### Custom Visualization

```csharp
// Custom drawing style
var customImage = ImageVisualizer.DrawDetections(
    imagePath: "test.jpg",
    detections: results,
    thickness: 3,           // Border thickness
    fontSize: 14f,          // Font size
    colors: customColors    // Custom colors
);
```

## 📚 API Documentation

### Core Interfaces

#### IYolo11Inference<TResult>
```csharp
public interface IYolo11Inference<TResult> : IDisposable
{
    InferenceMode Mode { get; }
    (int Width, int Height) InputSize { get; }
    float ConfidenceThreshold { get; set; }
    List<TResult> Infer(string imagePath);
    List<TResult> Infer(Bitmap bitmap);
}
```

#### Yolo11InferenceBase<TResult>
Abstract base class providing common inference pipeline implementation:
- Image preprocessing
- TensorRT inference execution
- Resource management
- Exception handling

### Data Models

#### DetectionResult
```csharp
public class DetectionResult
{
    public float X1, Y1, X2, Y2 { get; set; }  // Bounding box coordinates
    public float Confidence { get; set; }       // Confidence score
    public int ClassId { get; set; }            // Class ID
    public string ClassName { get; set; }       // Class name
    
    public Rectangle ToRectangle();             // Convert to rectangle
    public bool IsValid();                      // Validate validity
}
```

#### ClassificationResult
```csharp
public class ClassificationResult
{
    public int ClassId { get; set; }            // Class ID
    public string ClassName { get; set; }       // Class name
    public float Confidence { get; set; }       // Confidence score
    
    public bool IsValid();                      // Validate validity
}
```

## 🐛 Troubleshooting

### Common Issues

**Q: Build fails with "TensorRT10Sharp not found"**
```
A: Ensure TensorRT10Sharp project is properly built, check project reference paths
```

**Q: Runtime error "trt10.dll not found"**
```
A: Copy trt10.dll to executable directory, or add to system PATH
```

**Q: Abnormal inference results or coordinate errors**
```
A: Check if model format is YOLO11, confirm input size matches
```

**Q: GPU memory insufficient**
```
A: Reduce batch size, use smaller models, or enable FP16 precision
```

### Debugging Tips

1. **Enable Verbose Logging**:
```csharp
Logger.CurrentLevel = LogLevel.Debug;
```

2. **Check Model Information**:
```csharp
var engine = new Yolo11Engine("model.engine");
Console.WriteLine($"Input size: {engine.InputSize}");
Console.WriteLine($"Output dimensions: {engine.OutputDimensions}");
```

3. **Verify Preprocessing**:
```csharp
var preprocessResult = engine.Preprocess(bitmap);
Console.WriteLine($"Scale ratio: {preprocessResult.ScaleX}x{preprocessResult.ScaleY}");
Console.WriteLine($"Padding: ({preprocessResult.PadX}, {preprocessResult.PadY})");
```

## 🤝 Contributing Guidelines

We welcome community contributions! Please follow these steps:

1. **Fork** this repository
2. **Create feature branch**: `git checkout -b feature/amazing-feature`
3. **Commit changes**: `git commit -m 'Add amazing feature'`
4. **Push branch**: `git push origin feature/amazing-feature`
5. **Create Pull Request**

### Code Standards

- Follow C# coding conventions
- Add appropriate comments and documentation
- Write unit tests
- Ensure code passes all tests

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [YOLO11](https://github.com/ultralytics/ultralytics) - Excellent object detection model
- [TensorRT](https://developer.nvidia.com/tensorrt) - High-performance inference engine
- [.NET](https://dotnet.microsoft.com/) - Cross-platform development framework

## 📞 Contact

- **Project Homepage**: [GitHub Repository]
- **Issue Reporting**: [GitHub Issues]
- **Discussion**: [GitHub Discussions]

---

⭐ If this project helps you, please give us a Star! 