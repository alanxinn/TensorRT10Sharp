# TensorRT10Sharp Native (C++)

**[English](README_EN.md)** | **中文**

这是TensorRT10Sharp项目的原生C++部分，负责封装TensorRT C++ API并提供C风格的接口供C#调用。

## ✨ TensorRT 10 更新

- ✅ **API兼容性**: 完全适配TensorRT 10.x的新API
- ✅ **方法更新**: 使用`getTensorShape()`替代已弃用的`getIOTensorDims()`
- ✅ **内存管理**: 使用标准C++析构函数替代`destroy()`方法
- ✅ **库链接**: 正确链接TensorRT 10的版本化库文件

## 📁 项目结构

```
Native/
├── src/                    # C++源文件
│   └── tensorrt10_extern.cpp
├── include/                # 头文件
│   └── tensorrt10_extern.h
├── build/                  # 构建输出目录
├── scripts/                # 构建脚本
│   ├── build.bat          # 构建脚本
│   └── clean.bat          # 清理脚本
├── CMakeLists.txt         # CMake配置文件
└── README.md              # 本文件
```

## 🔧 环境要求

### 必需软件
1. **CUDA Toolkit** (推荐版本 11.8)
   - 下载地址: https://developer.nvidia.com/cuda-downloads
   - 确保安装时包含CUDA运行时库

2. **TensorRT** (推荐版本 TensorRT-10.13.0.35)
   - 下载地址: https://developer.nvidia.com/tensorrt
   - 确保与CUDA版本兼容

3. **Visual Studio 2019/2022** (包含C++开发工具)
   - 下载地址: https://visualstudio.microsoft.com/
   - 必须安装"使用C++的桌面开发"工作负载

4. **CMake** (版本 3.10或更高)
   - 下载地址: https://cmake.org/download/
   - 或通过Visual Studio安装程序安装

### 硬件要求
- NVIDIA GPU (支持CUDA)
- 至少4GB GPU内存 (推荐8GB或更多)
- 至少8GB系统内存

## 🚀 构建说明

### 方法1: 使用构建脚本 (推荐)
```bash
# 构建项目
scripts\build.bat

# 清理项目
scripts\clean.bat
```

### 方法2: 手动构建
```bash
# 创建构建目录
mkdir build
cd build

# 配置项目 (使用Visual Studio 2022)
cmake .. -G "Visual Studio 17 2022" -A x64

# 构建项目
cmake --build . --config Release --parallel
```

## 📝 配置说明

### 路径配置
在`CMakeLists.txt`中配置TensorRT和CUDA的安装路径：

```cmake
# 根据实际安装路径调整
set(TENSORRT_ROOT "C:/Program Files/NVIDIA GPU Computing Toolkit/TensorRT-10.13.0.35_cuda-11.8")
set(CUDA_ROOT "C:/Program Files/NVIDIA GPU Computing Toolkit/CUDA/v11.8")
```

### 库链接配置
TensorRT 10使用版本化的库文件名：

```cmake
target_link_libraries(trt10
    # CUDA库
    cudart
    cuda
    
    # TensorRT 10库 (注意版本号)
    nvinfer_10
    nvonnxparser_10
    nvinfer_plugin_10
)
```

### 编译选项
- **C++标准**: C++14
- **平台**: x64
- **配置**: Release (默认)
- **输出**: trt10.dll

## 🔗 API接口

### 主要类和函数

#### Nvinfer 类
```cpp
class Nvinfer {
public:
    Nvinfer();
    explicit Nvinfer(const char* modelPath);
    ~Nvinfer();
    
    // 获取绑定端口的形状信息
    Dims GetBindingDimensions(int index);
    Dims GetBindingDimensions(const char* nodeName);
    
    // 加载推理数据
    void LoadInferenceData(const char* nodeName, float* data, int dataSize);
    void LoadInferenceData(int nodeIndex, float* data, int dataSize);
    
    // 执行推理
    void Infer();
    
    // 获取推理结果
    float* GetInferenceResult(const char* nodeName, int* resultSize);
    float* GetInferenceResult(int nodeIndex, int* resultSize);
    
    // 获取输入输出信息
    int GetInputCount();
    int GetOutputCount();
    const char* GetInputName(int index);
    const char* GetOutputName(int index);
    
    // 释放结果内存
    void FreeResult(float* result);
};
```

#### 工具函数
```cpp
// ONNX转Engine函数
extern "C" bool OnnxToEngine(const char* modelPath, int memorySize);

// Dims创建函数
extern "C" Dims CreateDims();
```

## 🔄 TensorRT 10 API变更

### 已更新的方法
| 旧方法 (TensorRT 8/9) | 新方法 (TensorRT 10) | 说明 |
|----------------------|---------------------|------|
| `getIOTensorDims(index)` | `getTensorShape(name)` | 使用tensor名称获取形状 |
| `isIOTensorInput(index)` | `getTensorIOMode(name)` | 检查tensor输入/输出模式 |
| `getIOTensorIndexByName(name)` | 手动遍历查找 | 该方法已被移除 |
| `obj->destroy()` | `delete obj` | 使用标准C++析构 |

### 实现示例
```cpp
// TensorRT 10 新API使用方式
std::string name = engine->getIOTensorName(i);
nvinfer1::Dims dims = engine->getTensorShape(name.c_str());

if (engine->getTensorIOMode(name.c_str()) == nvinfer1::TensorIOMode::kINPUT) {
    // 处理输入tensor
}
```

## 🐛 故障排除

### 常见问题

1. **CMake配置失败**
   - 检查CUDA和TensorRT路径是否正确
   - 确保Visual Studio已正确安装
   - 验证CMake版本是否满足要求

2. **链接错误**
   ```
   错误: 找不到nvinfer_10.lib
   解决: 确保使用正确的TensorRT 10库文件名
   ```

3. **API编译错误**
   ```
   错误: getIOTensorDims不是成员
   解决: 已更新为getTensorShape，确保使用最新代码
   ```

4. **运行时错误**
   - 检查GPU驱动是否最新
   - 确保CUDA运行时库已安装
   - 验证TensorRT库在系统PATH中

### 调试技巧
- 使用Debug配置构建以获得更多调试信息
- 检查CMake输出中的路径和库信息
- 使用Visual Studio调试器进行问题定位
- 验证所有依赖的DLL文件是否可访问

## 📊 性能优化

### 构建优化
- 使用Release配置获得最佳性能
- 启用并行构建 (`--parallel`)
- 确保使用x64平台

### 运行时优化
- 预分配GPU内存
- 使用CUDA流进行异步操作
- 启用TensorRT的FP16精度（如果GPU支持）

## 📚 相关文档

- [TensorRT 10官方文档](https://docs.nvidia.com/deeplearning/tensorrt/)
- [CUDA编程指南](https://docs.nvidia.com/cuda/)
- [CMake文档](https://cmake.org/documentation/)
- [Visual Studio C++文档](https://docs.microsoft.com/en-us/cpp/)

## 🔄 输出文件

构建成功后，将生成以下文件：
- `build/bin/Release/trt10.dll` - 主要输出库
- `../trt10.dll` - 复制到项目根目录的库文件（供C#项目使用）
- `build/lib/Release/trt10.lib` - 导入库文件 