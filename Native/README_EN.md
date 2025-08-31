# TensorRT10Sharp Native (C++)

**English** | **[中文](README.md)**

This is the native C++ part of the TensorRT10Sharp project, responsible for wrapping the TensorRT C++ API and providing C-style interfaces for C# calls.

## ✨ TensorRT 10 Updates

- ✅ **API Compatibility**: Fully adapted to TensorRT 10.x new APIs
- ✅ **Method Updates**: Using `getTensorShape()` to replace deprecated `getIOTensorDims()`
- ✅ **Memory Management**: Using standard C++ destructors instead of `destroy()` methods
- ✅ **Library Linking**: Correctly linking TensorRT 10 versioned library files

## 📁 Project Structure

```
Native/
├── src/                    # C++ source files
│   └── tensorrt10_extern.cpp
├── include/                # Header files
│   └── tensorrt10_extern.h
├── build/                  # Build output directory
├── scripts/                # Build scripts
│   ├── build.bat          # Build script
│   └── clean.bat          # Clean script
├── CMakeLists.txt         # CMake configuration file
└── README.md              # This file
```

## 🔧 Environment Requirements

### Required Software
1. **CUDA Toolkit** (Recommended version 11.8)
   - Download: https://developer.nvidia.com/cuda-downloads
   - Ensure CUDA runtime libraries are included during installation

2. **TensorRT** (Recommended version TensorRT-10.13.0.35)
   - Download: https://developer.nvidia.com/tensorrt
   - Ensure compatibility with CUDA version

3. **Visual Studio 2019/2022** (with C++ development tools)
   - Download: https://visualstudio.microsoft.com/
   - Must install "Desktop development with C++" workload

4. **CMake** (version 3.10 or higher)
   - Download: https://cmake.org/download/
   - Or install through Visual Studio installer

### Hardware Requirements
- NVIDIA GPU (CUDA support)
- At least 4GB GPU memory (8GB or more recommended)
- At least 8GB system memory

## 🚀 Build Instructions

### Method 1: Using Build Scripts (Recommended)
```bash
# Build project
scripts\build.bat

# Clean project
scripts\clean.bat
```

### Method 2: Manual Build
```bash
# Create build directory
mkdir build
cd build

# Configure project (using Visual Studio 2022)
cmake .. -G "Visual Studio 17 2022" -A x64

# Build project
cmake --build . --config Release --parallel
```

## 📝 Configuration Instructions

### Path Configuration
Configure TensorRT and CUDA installation paths in `CMakeLists.txt`:

```cmake
# Adjust according to actual installation paths
set(TENSORRT_ROOT "C:/Program Files/NVIDIA GPU Computing Toolkit/TensorRT-10.13.0.35_cuda-11.8")
set(CUDA_ROOT "C:/Program Files/NVIDIA GPU Computing Toolkit/CUDA/v11.8")
```

### Library Linking Configuration
TensorRT 10 uses versioned library file names:

```cmake
target_link_libraries(trt10
    # CUDA libraries
    cudart
    cuda
    
    # TensorRT 10 libraries (note version numbers)
    nvinfer_10
    nvonnxparser_10
    nvinfer_plugin_10
)
```

### Compilation Options
- **C++ Standard**: C++14
- **Platform**: x64
- **Configuration**: Release (default)
- **Output**: trt10.dll

## 🔗 API Interface

### Main Classes and Functions

#### Nvinfer Class
```cpp
class Nvinfer {
public:
    Nvinfer();
    explicit Nvinfer(const char* modelPath);
    ~Nvinfer();
    
    // Get binding port shape information
    Dims GetBindingDimensions(int index);
    Dims GetBindingDimensions(const char* nodeName);
    
    // Load inference data
    void LoadInferenceData(const char* nodeName, float* data, int dataSize);
    void LoadInferenceData(int nodeIndex, float* data, int dataSize);
    
    // Execute inference
    void Infer();
    
    // Get inference results
    float* GetInferenceResult(const char* nodeName, int* resultSize);
    float* GetInferenceResult(int nodeIndex, int* resultSize);
    
    // Get input/output information
    int GetInputCount();
    int GetOutputCount();
    const char* GetInputName(int index);
    const char* GetOutputName(int index);
    
    // Free result memory
    void FreeResult(float* result);
};
```

#### Utility Functions
```cpp
// ONNX to Engine conversion function
extern "C" bool OnnxToEngine(const char* modelPath, int memorySize);

// Dims creation function
extern "C" Dims CreateDims();
```

## 🔄 TensorRT 10 API Changes

### Updated Methods
| Old Method (TensorRT 8/9) | New Method (TensorRT 10) | Description |
|---------------------------|-------------------------|-------------|
| `getIOTensorDims(index)` | `getTensorShape(name)` | Get shape using tensor name |
| `isIOTensorInput(index)` | `getTensorIOMode(name)` | Check tensor input/output mode |
| `getIOTensorIndexByName(name)` | Manual traversal search | This method has been removed |
| `obj->destroy()` | `delete obj` | Use standard C++ destruction |

### Implementation Example
```cpp
// TensorRT 10 new API usage
std::string name = engine->getIOTensorName(i);
nvinfer1::Dims dims = engine->getTensorShape(name.c_str());

if (engine->getTensorIOMode(name.c_str()) == nvinfer1::TensorIOMode::kINPUT) {
    // Handle input tensor
}
```

## 🐛 Troubleshooting

### Common Issues

1. **CMake Configuration Failure**
   - Check if CUDA and TensorRT paths are correct
   - Ensure Visual Studio is properly installed
   - Verify CMake version meets requirements

2. **Linking Errors**
   ```
   Error: Cannot find nvinfer_10.lib
   Solution: Ensure using correct TensorRT 10 library file names
   ```

3. **API Compilation Errors**
   ```
   Error: getIOTensorDims is not a member
   Solution: Updated to getTensorShape, ensure using latest code
   ```

4. **Runtime Errors**
   - Check if GPU drivers are up to date
   - Ensure CUDA runtime libraries are installed
   - Verify TensorRT libraries are in system PATH

### Debugging Tips
- Use Debug configuration build for more debugging information
- Check paths and library information in CMake output
- Use Visual Studio debugger for problem localization
- Verify all dependent DLL files are accessible

## 📊 Performance Optimization

### Build Optimization
- Use Release configuration for best performance
- Enable parallel build (`--parallel`)
- Ensure using x64 platform

### Runtime Optimization
- Pre-allocate GPU memory
- Use CUDA streams for asynchronous operations
- Enable TensorRT FP16 precision (if GPU supports)

## 📚 Related Documentation

- [TensorRT 10 Official Documentation](https://docs.nvidia.com/deeplearning/tensorrt/)
- [CUDA Programming Guide](https://docs.nvidia.com/cuda/)
- [CMake Documentation](https://cmake.org/documentation/)
- [Visual Studio C++ Documentation](https://docs.microsoft.com/en-us/cpp/)

## 🔄 Output Files

After successful build, the following files will be generated:
- `build/bin/Release/trt10.dll` - Main output library
- `../trt10.dll` - Library file copied to project root (for C# project use)
- `build/lib/Release/trt10.lib` - Import library file 