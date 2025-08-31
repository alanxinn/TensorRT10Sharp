# TensorRT10Sharp 构建说明

本文档详细说明了如何构建TensorRT10Sharp项目，包括环境配置、依赖安装和构建步骤。

## 🔧 系统要求

### 必需软件
1. **CUDA Toolkit** (推荐版本 11.8)
   - 下载地址: https://developer.nvidia.com/cuda-downloads
   - 确保安装时包含CUDA运行时库和开发工具

2. **TensorRT** (推荐版本 TensorRT-10.13.0.35)
   - 下载地址: https://developer.nvidia.com/tensorrt
   - 确保与CUDA版本兼容
   - 支持TensorRT 10.x系列

3. **Visual Studio 2019/2022** (包含C++开发工具)
   - 下载地址: https://visualstudio.microsoft.com/
   - 必须安装"使用C++的桌面开发"工作负载
   - 推荐使用Visual Studio 2022

4. **CMake** (版本 3.10或更高)
   - 下载地址: https://cmake.org/download/
   - 或通过Visual Studio安装程序安装

5. **.NET 6.0 SDK**
   - 下载地址: https://dotnet.microsoft.com/download
   - 确保安装.NET 6.0或更高版本

### 硬件要求
- **GPU**: NVIDIA GPU (支持CUDA Compute Capability 6.0+)
- **GPU内存**: 至少4GB (推荐8GB或更多)
- **系统内存**: 至少8GB (推荐16GB或更多)
- **存储空间**: 至少10GB可用空间

## 📦 安装步骤

### 1. 安装CUDA和TensorRT

#### CUDA安装
1. 下载CUDA Toolkit 11.8
2. 运行安装程序，选择"自定义安装"
3. 确保勾选以下组件：
   - CUDA Runtime
   - CUDA Development Tools
   - Visual Studio Integration

#### TensorRT安装
1. 下载TensorRT 10.13.0.35 (for CUDA 11.8)
2. 解压到指定目录，如：
   ```
   C:\Program Files\NVIDIA GPU Computing Toolkit\TensorRT-10.13.0.35_cuda-11.8
   ```
3. 将TensorRT的bin和lib目录添加到系统PATH

### 2. 配置环境变量

确保以下环境变量已正确设置：

```cmd
CUDA_PATH=C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.8
TENSORRT_PATH=C:\Program Files\NVIDIA GPU Computing Toolkit\TensorRT-10.13.0.35_cuda-11.8
PATH=%PATH%;%CUDA_PATH%\bin;%TENSORRT_PATH%\lib
```

### 3. 验证安装

```cmd
# 检查CUDA安装
nvcc --version

# 检查NVIDIA驱动
nvidia-smi

# 检查.NET SDK
dotnet --version

# 检查CMake
cmake --version
```

## 🚀 构建步骤

### 方法1: 使用批处理脚本 (推荐)

#### 完整构建
```cmd
# 构建整个项目 (Native + Managed)
build-all.bat

# 清理整个项目
clean-all.bat

# 运行示例程序
run_example.bat
```

#### 分别构建
```cmd
# 构建Native项目
cd Native
scripts\build.bat

# 构建Managed项目
cd ..\Managed
scripts\build.bat
```

### 方法2: 手动构建

#### Native项目构建
```cmd
# 1. 进入Native目录
cd Native

# 2. 创建构建目录
mkdir build
cd build

# 3. 配置CMake项目 (Visual Studio 2022)
cmake .. -G "Visual Studio 17 2022" -A x64 -DCMAKE_BUILD_TYPE=Release

# 4. 构建项目
cmake --build . --config Release --parallel

# 5. 验证输出
dir bin\Release\trt10.dll
```

#### Managed项目构建
```cmd
# 1. 进入Managed目录
cd Managed

# 2. 还原NuGet包
dotnet restore

# 3. 构建项目
dotnet build --configuration Release

# 4. 验证输出
dir bin\x64\Release\net6.0\TensorRT10Sharp.exe
```

## ⚙️ 配置说明

### CMakeLists.txt配置

根据您的安装路径修改`Native/CMakeLists.txt`：

```cmake
# TensorRT和CUDA路径配置
set(TENSORRT_ROOT "C:/Program Files/NVIDIA GPU Computing Toolkit/TensorRT-10.13.0.35_cuda-11.8" CACHE PATH "TensorRT installation directory")
set(CUDA_ROOT "C:/Program Files/NVIDIA GPU Computing Toolkit/CUDA/v11.8" CACHE PATH "CUDA installation directory")

# 库链接配置 (TensorRT 10使用版本化库名)
target_link_libraries(trt10
    # CUDA库
    cudart
    cuda
    
    # TensorRT 10库
    nvinfer_10
    nvonnxparser_10
    nvinfer_plugin_10
)
```

### 项目配置文件

#### C#项目配置 (TensorRT10Sharp.csproj)
```xml
<PropertyGroup>
  <TargetFramework>net6.0</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  <OutputType>Exe</OutputType>
</PropertyGroup>
```

## ✅ 验证构建

### 1. 检查生成的文件

构建成功后，应该生成以下文件：

#### Native输出
```
Native/build/bin/Release/
├── trt10.dll          # 主要输出库
└── trt10.lib          # 导入库

项目根目录/
└── trt10.dll          # 复制的库文件
```

#### Managed输出
```
Managed/bin/x64/Release/net6.0/
├── TensorRT10Sharp.exe     # 可执行文件
├── TensorRT10Sharp.dll     # 程序集
├── TensorRT10Sharp.pdb     # 调试符号
└── trt10.dll              # Native库
```

### 2. 运行测试

```cmd
# 运行完整测试
run_example.bat

# 或直接运行C#程序
cd Managed
dotnet run --configuration Release
```

预期输出：
```
TensorRT C# API 测试程序
=======================

1. 测试ONNX模型转换为TensorRT引擎
✗ ONNX模型文件 yolo11n.onnx 不存在

2. 测试加载TensorRT引擎并进行推理
✗ TensorRT引擎文件 yolo11n.engine 不存在

3. 测试Dims结构体功能
✓ Dims结构体功能测试通过

测试完成！
```

## 🐛 常见问题

### 1. CMake配置失败

**错误**: `Could not find CUDA`
```cmd
解决方案:
1. 确保CUDA已正确安装
2. 检查CMAKE_CUDA_COMPILER路径
3. 验证CUDA_PATH环境变量
```

**错误**: `TensorRT path not found`
```cmd
解决方案:
1. 检查TensorRT安装路径
2. 修改CMakeLists.txt中的TENSORRT_ROOT
3. 确保路径中没有空格或特殊字符
```

### 2. 链接错误

**错误**: `Cannot open input file 'nvinfer_10.lib'`
```cmd
解决方案:
1. 确保使用TensorRT 10.x版本
2. 检查库文件名是否包含版本号
3. 验证link_directories路径正确
```

**错误**: `LNK2019: 无法解析的外部符号`
```cmd
解决方案:
1. 检查target_link_libraries配置
2. 确保所有必需的库都已链接
3. 验证库文件版本匹配
```

### 3. 运行时错误

**错误**: `DllNotFoundException: 找不到trt10.dll`
```cmd
解决方案:
1. 确保trt10.dll在程序目录中
2. 检查TensorRT库是否在PATH中
3. 使用run_example.bat进行完整测试
```

**错误**: `CUDA driver version is insufficient`
```cmd
解决方案:
1. 更新NVIDIA驱动程序
2. 确保驱动版本支持CUDA 11.8
3. 重启系统后重试
```

### 4. API兼容性问题

**错误**: `getIOTensorDims不是成员`
```cmd
解决方案:
1. 确保使用TensorRT 10.x版本
2. 检查代码是否使用了新的API
3. 参考API变更文档进行更新
```

## 🔧 高级配置

### 启用调试模式
```cmd
# 构建Debug版本
cmake --build . --config Debug

# 或使用脚本参数
set CMAKE_BUILD_TYPE=Debug
scripts\build.bat
```

### 自定义安装路径
```cmake
# 在CMakeLists.txt中自定义路径
set(TENSORRT_ROOT "D:/TensorRT" CACHE PATH "Custom TensorRT path")
set(CUDA_ROOT "D:/CUDA" CACHE PATH "Custom CUDA path")
```

### 性能优化选项
```cmake
# 启用优化选项
set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} /O2 /Ob2")
set(CMAKE_CUDA_FLAGS_RELEASE "${CMAKE_CUDA_FLAGS_RELEASE} -O3")
```

## 📚 相关资源

- [TensorRT 10官方文档](https://docs.nvidia.com/deeplearning/tensorrt/)
- [CUDA安装指南](https://docs.nvidia.com/cuda/cuda-installation-guide-microsoft-windows/)
- [CMake官方文档](https://cmake.org/documentation/)
- [.NET 6.0文档](https://docs.microsoft.com/en-us/dotnet/)
- [Visual Studio C++文档](https://docs.microsoft.com/en-us/cpp/)

## 🤝 技术支持

如果遇到问题，请：
1. 检查系统要求和依赖项
2. 查看常见问题部分
3. 运行`run_example.bat`进行诊断
4. 提交Issue到GitHub仓库并提供：
   - 详细的错误信息
   - 系统环境信息
   - 构建日志 