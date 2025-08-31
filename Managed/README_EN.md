# TensorRT10Sharp Managed (C# Library)

**English** | **[中文](README.md)**

This is the managed C# library part of the TensorRT10Sharp project, providing .NET-friendly API interfaces that wrap the functionality of the Native C++ library. It has been refactored into a standard .NET library that can be distributed as a NuGet package and referenced by other projects.

## 📁 Project Structure

```
Managed/
├── src/                           # C# source files
│   ├── Dims.cs                   # Dimension structure
│   └── Nvinfer.cs                # TensorRT inference engine class
├── Examples/                      # Independent example project
│   ├── BasicExample.cs           # Basic usage example
│   └── TensorRT10Sharp.Examples.csproj # Example project file
├── scripts/                       # Build and run scripts
│   ├── build.bat                 # Build script (library + examples)
│   ├── run.bat                   # Run example script
│   └── clean.bat                 # Clean script
├── bin/Release/net6.0/           # Build output
│   ├── TensorRT10Sharp.dll      # Main library
│   └── alanxinn.TensorRT10Sharp.0.1.0.nupkg # NuGet package
├── TensorRT10Sharp.csproj        # C# library project file
└── README.md                     # This file
```

## 🔧 Environment Requirements

### Required Software
1. **.NET 6.0 SDK**
   - Download: https://dotnet.microsoft.com/download
   - Ensure .NET 6.0 or higher is installed

2. **Native DLL Dependencies**
   - Need to build Native project to generate `trt10.dll`
   - Or ensure `trt10.dll` exists in project root directory

### Optional Tools
- **Visual Studio 2022** or **JetBrains Rider** (for IDE development)
- **Visual Studio Code** (lightweight editor)

## 🚀 Build and Run

### Method 1: Using Scripts (Recommended)
```bash
# Build library and example projects
scripts\build.bat

# Run example program
scripts\run.bat

# Clean project
scripts\clean.bat
```

### Method 2: Using .NET CLI
```bash
# Build library
dotnet build --configuration Release

# Build and run example project
dotnet run --project Examples\TensorRT10Sharp.Examples.csproj --configuration Release
```

### Method 3: Using as NuGet Package
```bash
# NuGet package is automatically generated after build
# Location: bin\Release\alanxinn.TensorRT10Sharp.0.1.0.nupkg

# Reference in other projects
dotnet add package alanxinn.TensorRT10Sharp --version 0.1.0 --source path\to\bin\Release
```

### Method 4: Project Reference
```bash
# Direct library project reference in other projects
<ProjectReference Include="path\to\TensorRT10Sharp.csproj" />
```

## 📝 API Usage Guide

### Basic Usage

#### 1. Create Inference Engine
```csharp
using TensorRTSharp;

// Method 1: Load TensorRT engine file directly
using var infer = new Nvinfer("model.engine");

// Method 2: Convert ONNX model first, then load
bool success = Nvinfer.ConvertOnnxToEngine("model.onnx", 1024);
if (success)
{
    using var infer = new Nvinfer("model.engine");
}
```

#### 2. Get Model Information
```csharp
// Get input/output counts
int inputCount = infer.GetInputCount();
int outputCount = infer.GetOutputCount();

// Get input/output names and dimensions
for (int i = 0; i < inputCount; i++)
{
    string inputName = infer.GetInputName(i);
    Dims inputDims = infer.GetBindingDimensions(inputName);
    Console.WriteLine($"Input {i}: {inputName}, Dimensions: {FormatDims(inputDims)}");
}
```

#### 3. Execute Inference
```csharp
// Prepare input data
string inputName = infer.GetInputName(0);
Dims inputDims = infer.GetBindingDimensions(inputName);
float[] inputData = new float[inputDims.GetElementCount()];

// Fill input data...
// inputData[i] = ...;

// Load data and execute inference
infer.LoadInferenceData(inputName, inputData);
infer.Infer();

// Get output results
string outputName = infer.GetOutputName(0);
float[] outputData = infer.GetInferenceResult(outputName);
```

### Dims Structure Usage

```csharp
// Create dimension structure
Dims dims = new Dims();

// Set dimensions
dims.SetDimension(0, 1);    // batch size
dims.SetDimension(1, 3);    // channels
dims.SetDimension(2, 640);  // height
dims.SetDimension(3, 640);  // width

// Get dimension information
int batchSize = dims.GetDimension(0);
int channels = dims.GetDimension(1);
int totalElements = dims.GetElementCount();

Console.WriteLine($"Dimensions: {dims.nbDims}");
Console.WriteLine($"Total elements: {totalElements}");
```

## 🎯 Example Program

The project includes an independent example project (`Examples/BasicExample.cs`) that demonstrates:

1. **Automatic ONNX Conversion**: If engine file not found, automatically search for ONNX file and convert
2. **Model Loading**: Load TensorRT engine files
3. **Information Query**: Get model input/output information
4. **Inference Execution**: Execute complete inference pipeline
5. **Result Retrieval**: Get and process inference results
6. **Dims Testing**: Test various dimension structure functionalities

### Running Examples
```bash
# Ensure model files exist in project root directory
# - yolo11n.onnx (ONNX model) or
# - yolo11n.engine (TensorRT engine)

# Run example (automatically builds library and examples)
scripts\run.bat

# Or run example project directly
dotnet run --project Examples\TensorRT10Sharp.Examples.csproj
```

## 🔧 Project Configuration

### Project Properties
- **Target Framework**: .NET 6.0
- **Platform**: x64
- **Unsafe Code**: Enabled
- **Output Type**: Library
- **Package ID**: alanxinn.TensorRT10Sharp
- **Version**: 0.1.0
- **License**: MIT
- **NuGet Package**: Auto-generated alanxinn.TensorRT10Sharp.0.1.0.nupkg

### NuGet Package Information
- **Package Name**: alanxinn.TensorRT10Sharp
- **Version**: 0.1.0
- **Author**: TensorRTSharp Team
- **License**: MIT
- **Project URL**: https://github.com/alanxinn/TensorRT10Sharp
- **Tags**: tensorrt, tensorrt10, cuda, deep-learning, inference, gpu, csharp, dotnet
- **Description**: TensorRT10 wrapper for .NET - 为.NET平台提供的NVIDIA TensorRT10封装库

### Dependencies
- **Native Library**: `../trt10.dll` (automatically copied to output directory)
- **Resource Files**: `../Assets/*.*` (excluding large model files)
- **Runtime**: .NET 6.0
- **Platform**: x64 (Windows)

## 🐛 Troubleshooting

### Common Issues

1. **DllNotFoundException**
   ```
   Solution: Ensure trt10.dll exists in output directory or project root
   ```

2. **InvalidOperationException: Failed to create TensorRT inference engine**
   ```
   Solution: Check if CUDA and TensorRT are correctly installed
   ```

3. **ArgumentNullException**
   ```
   Solution: Check if model file path is correct
   ```

### Debugging Tips
- Use Debug configuration build for more debugging information
- Check if Native DLL is correctly loaded
- Verify model files exist and are in correct format

## 📊 Performance Optimization

### Best Practices
1. **Pre-allocate Arrays**: Reuse input/output arrays to avoid frequent allocation
2. **Batch Processing**: Use batching to improve throughput
3. **Asynchronous Processing**: Consider async processing for multiple inference requests
4. **Memory Management**: Timely release of large arrays and objects

### Example Optimization Code
```csharp
// Pre-allocate input/output arrays
float[] inputBuffer = new float[inputSize];
float[] outputBuffer;

// Reuse
for (int i = 0; i < batchCount; i++)
{
    // Fill input data to pre-allocated buffer
    FillInputData(inputBuffer, i);
    
    // Execute inference
    infer.LoadInferenceData(inputName, inputBuffer);
    infer.Infer();
    outputBuffer = infer.GetInferenceResult(outputName);
    
    // Process output...
}
```

## 📚 Related Documentation

- [.NET 6.0 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [P/Invoke Interop](https://docs.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke)
- [TensorRT C# API Guide](../Documentation/README.md) 