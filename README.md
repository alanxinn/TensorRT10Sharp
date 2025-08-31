# TensorRT10Sharp C#

**[English](README_EN.md)** | **中文**

TensorRT10Sharp C# 是一个为 .NET 平台提供的 NVIDIA TensorRT10 封装库，使开发者能够在 C# 中轻松使用 TensorRT 进行高性能深度学习推理。

## ✨ 最新更新

- ✅ **TensorRT 10 API兼容性**: 完全适配TensorRT 10.x的新API
- ✅ **构建系统优化**: 改进的CMake配置和批处理脚本
- ✅ **错误处理增强**: 更好的错误检测和诊断
- ✅ **文档完善**: 详细的构建指南和故障排除
- ✅ **跨平台支持**: 优化的Windows构建流程
- 🎉 **Yolo11Sharp 集成**: 新增完整的 YOLO11 多模式推理库
- ✅ **检测功能验证**: YOLO11 目标检测已完成测试验证
- 📦 **NuGet 包发布**: 已发布到 NuGet.org，支持包管理器安装

## 🚀 子项目

### Yolo11Sharp - YOLO11 推理库
基于 TensorRT10Sharp 构建的高性能 YOLO11 多模式推理 C# 实现。

**支持的推理模式:**
- 🔍 **目标检测 (Detection)** - ✅ 已测试验证
- 🏷️ **图像分类 (Classification)** - 🔧 开发完成
- 🎭 **实例分割 (Segmentation)** - 📋 架构就绪
- 📐 **定向边界框 (OBB)** - 📋 架构就绪
- 🤸 **姿态估计 (Pose)** - 📋 架构就绪

**快速开始:**
```bash
cd Yolo11Sharp
Scripts\build.bat
Scripts\run.bat
```

详细文档: [Yolo11Sharp/README.md](Yolo11Sharp/README.md)

## 📁 项目结构

```
TensorRT10Sharp/
├── Native/                         # C++ 原生项目
│   ├── src/                        # C++ 源文件
│   │   └── tensorrt10_extern.cpp   # TensorRT C++ API实现
│   ├── include/                    # C++ 头文件
│   │   └── tensorrt10_extern.h     # C++ API声明
│   ├── build/                      # C++ 构建输出
│   ├── scripts/                    # C++ 构建脚本
│   │   ├── build.bat              # 构建Native项目
│   │   └── clean.bat              # 清理Native项目
│   ├── CMakeLists.txt             # CMake配置文件
│   └── README.md                  # Native项目说明
├── Managed/                        # C# 托管类库项目
│   ├── src/                        # C# 源文件
│   │   ├── Dims.cs                 # 维度结构体
│   │   └── Nvinfer.cs              # TensorRT推理引擎类
│   ├── Examples/                   # C# 示例项目
│   │   ├── BasicExample.cs        # 基础使用示例
│   │   └── TensorRT10Sharp.Examples.csproj # 示例项目文件
│   ├── scripts/                    # C# 构建脚本
│   │   ├── build.bat              # 构建类库和示例项目
│   │   ├── run.bat                # 运行示例程序
│   │   └── clean.bat              # 清理项目
│   ├── bin/Release/net6.0/        # 构建输出
│   │   ├── TensorRT10Sharp.dll    # 主要类库
│   │   └── alanxinn.TensorRT10Sharp.0.1.0.nupkg # NuGet 包
│   ├── TensorRT10Sharp.csproj     # C# 类库项目文件
│   └── README.md                  # Managed项目说明
├── Yolo11Sharp/                    # 🎯 YOLO11 推理库 (新增)
│   ├── src/                        # 源代码
│   │   ├── Core/                   # 核心推理引擎
│   │   ├── Models/                 # 数据模型
│   │   ├── Utils/                  # 工具类
│   │   └── Visualization/          # 可视化
│   ├── Examples/                   # 示例程序
│   ├── Assets/                     # 资源文件
│   ├── Scripts/                    # 脚本文件
│   ├── bin/Release/net6.0/         # 构建输出
│   │   ├── Yolo11Sharp.exe         # 主程序
│   │   └── Yolo11Sharp.dll         # 程序库
│   ├── Yolo11Sharp.csproj          # 项目文件
│   ├── README.md                   # 项目文档
│   ├── ARCHITECTURE.md             # 架构说明
│   └── RENAME_SUMMARY.md           # 重命名记录
├── Documentation/                  # 项目文档
│   ├── BUILD_README.md             # 构建说明
│   └── IGNORE_FILES_GUIDE.md       # 忽略文件配置指南
├── Assets/                         # 资源文件
│   └── README.md                   # 资源文件说明
├── build-all.bat                   # 完整项目构建脚本
├── clean-all.bat                   # 完整项目清理脚本
├── run_example.bat                 # 示例运行脚本
├── .gitignore                      # Git忽略文件
├── .gitattributes                  # Git属性配置
├── .editorconfig                   # 编辑器配置
└── README.md                       # 本文件
```

## ⚡ 快速开始

### 0. 快速安装（推荐）

如果您只想使用 TensorRT10Sharp 类库，可以直接通过 NuGet 安装：

```bash
# 创建新项目
dotnet new console -n MyTensorRTApp
cd MyTensorRTApp

# 安装 NuGet 包
dotnet add package alanxinn.TensorRT10Sharp --version 0.1.0

# 开始使用
# 编辑 Program.cs，参考下面的使用示例
```

### 1. 环境要求

#### Native (C++) 项目
- **CUDA Toolkit** (推荐版本 11.8)
- **TensorRT** (推荐版本 TensorRT-10.13.0.35)
- **Visual Studio 2019/2022** (包含C++开发工具)
- **CMake** (版本 3.10或更高)

#### Managed (C#) 类库项目
- **.NET 6.0 SDK**

### 2. 构建项目

#### 方法1: 完整构建 (推荐)
```bash
# 构建整个项目 (Native + Managed)
build-all.bat

# 清理整个项目
clean-all.bat

# 运行示例
run_example.bat
```

#### 方法2: 分别构建
```bash
# 1. 构建Native项目
cd Native
scripts\build.bat

# 2. 构建Managed项目
cd ..\Managed
scripts\build.bat

# 3. 构建Yolo11Sharp项目 (可选)
cd ..\Yolo11Sharp
Scripts\build.bat
```

### 3. 运行示例

#### 基础 TensorRT 示例
```bash
# 方法1: 使用示例脚本
run_example.bat

# 方法2: 直接运行C#示例程序
cd Managed
scripts\run.bat

# 方法3: 使用.NET CLI 运行示例项目
dotnet run --project Managed\Examples\TensorRT10Sharp.Examples.csproj
```

#### YOLO11 检测示例 (已测试)
```bash
# 运行 YOLO11 目标检测
cd Yolo11Sharp
Scripts\run.bat

# 或直接运行
bin\Release\net6.0\Yolo11Sharp.exe yolo11n.engine test.jpg result.jpg
```

## 🔧 项目特点

### 分离式架构
- **Native项目**: 独立的C++项目，封装TensorRT C++ API
- **Managed项目**: 独立的C#类库项目，提供.NET友好的API接口，可作为NuGet包分发
- **Yolo11Sharp项目**: 基于Managed项目的高级YOLO11推理库
- **清晰分工**: 每个项目有自己的构建系统和脚本

### TensorRT 10 兼容性
- ✅ 使用最新的TensorRT 10 API
- ✅ 支持新的tensor操作方法
- ✅ 兼容CUDA 11.8+
- ✅ 优化的内存管理

### 核心功能

#### Dims 结构体
- 维度信息管理
- 支持最多8维张量
- 提供维度操作和计算方法

#### Nvinfer 类
- TensorRT引擎加载和管理
- 推理数据输入输出
- 模型信息查询
- 资源自动释放

#### TensorRT10Sharp 类库特性
- 📦 标准 .NET 类库，支持 NuGet 包分发
- 🔄 智能 ONNX 自动转换功能
- 🛡️ 完整的资源管理和异常处理
- 🎯 简洁易用的 C# API 接口
- ⚡ 高性能 P/Invoke 互操作

#### Yolo11Sharp 高级功能
- 🎯 多模式推理支持 (检测/分类/分割/OBB/姿态)
- ⚡ 高性能GPU加速推理
- 🏗️ 模块化架构设计
- 🎨 内置可视化工具
- 🔧 灵活的配置选项

## 📦 NuGet 包信息

### 包详情
- **包名**: `alanxinn.TensorRT10Sharp`
- **版本**: `0.1.0`
- **作者**: TensorRTSharp Team
- **许可证**: MIT
- **平台**: .NET 6.0, x64 (Windows)
- **包地址**: https://www.nuget.org/packages/alanxinn.TensorRT10Sharp/
- **项目地址**: https://github.com/alanxinn/TensorRT10Sharp

### 包特性
- 🚀 **高性能**: 基于 NVIDIA TensorRT 10 的 C# 封装
- 🔄 **智能转换**: 自动 ONNX 到 TensorRT 引擎转换
- 🛡️ **资源管理**: 完整的内存管理和异常处理
- 📋 **易于使用**: 简洁的 C# API 接口
- ⚡ **GPU 加速**: 充分利用 CUDA GPU 性能

### 依赖要求
- **.NET 6.0** 或更高版本
- **Windows x64** 平台
- **NVIDIA GPU** 支持 CUDA 11.8+
- **TensorRT 10.x** 运行时库
- **Visual C++ Redistributable** (通常已安装)

## 📖 使用示例

### 安装和引用

#### 方法1: NuGet 包引用（推荐）
```xml
<!-- 在项目文件中添加 -->
<PackageReference Include="alanxinn.TensorRT10Sharp" Version="0.1.0" />
```

```bash
# 或使用 dotnet CLI 安装
dotnet add package alanxinn.TensorRT10Sharp --version 0.1.0
```

#### 方法2: 包管理器控制台（Visual Studio）
```powershell
Install-Package alanxinn.TensorRT10Sharp -Version 0.1.0
```

#### 方法3: 项目引用
```xml
<!-- 直接引用类库项目 -->
<ProjectReference Include="path\to\Managed\TensorRT10Sharp.csproj" />
```

#### 方法4: 本地 NuGet 包
```bash
# 添加本地包源
dotnet nuget add source path\to\TensorRT10Sharp\Managed\bin\Release --name "Local"

# 安装包
dotnet add package alanxinn.TensorRT10Sharp --version 0.1.0 --source "Local"
```

### 基础 TensorRT 使用

```csharp
using TensorRTSharp;

// 方法1: 直接加载引擎文件
using var infer = new Nvinfer("Assets/yolo11n.engine");

// 方法2: 自动转换ONNX模型（如果引擎不存在）
// 会自动检测 yolo11n.onnx 并转换为 yolo11n.engine
using var infer2 = new Nvinfer("yolo11n.engine");

// 方法3: 手动转换ONNX模型
bool success = Nvinfer.ConvertOnnxToEngine("model.onnx", 1024);
if (success)
{
    using var infer3 = new Nvinfer("model.engine");
}

// 获取模型信息
Console.WriteLine($"输入数量: {infer.GetInputCount()}");
Console.WriteLine($"输出数量: {infer.GetOutputCount()}");

// 准备输入数据
string inputName = infer.GetInputName(0);
Dims inputDims = infer.GetBindingDimensions(inputName);
float[] inputData = new float[inputDims.GetElementCount()];

// 加载数据并执行推理
infer.LoadInferenceData(inputName, inputData);
infer.Infer();

// 获取结果
string outputName = infer.GetOutputName(0);
float[] result = infer.GetInferenceResult(outputName);
```

### YOLO11 目标检测使用 (已测试)

```csharp
using Yolo11Sharp.Core;
using Yolo11Sharp.Visualization;

// 创建检测器
using var detector = new Yolo11Detection("yolo11n.engine", "coco.names");

// 执行检测
var detections = detector.Infer("test.jpg");

// 可视化结果
var outputImage = ImageVisualizer.DrawDetections("test.jpg", detections);
outputImage.Save("result.jpg");

// 输出结果
foreach (var detection in detections)
{
    Console.WriteLine($"{detection.ClassName}: {detection.Confidence:P2} " +
                     $"[{detection.X1:F1}, {detection.Y1:F1}, {detection.X2:F1}, {detection.Y2:F1}]");
}
```

### YOLO11 图像分类使用

```csharp
using Yolo11Sharp.Core;

// 创建分类器
using var classifier = new Yolo11Classification("yolo11n-cls.engine", "imagenet.names");

// 获取 Top-5 分类结果
var results = classifier.GetTopK("test.jpg", 5);

// 输出结果
foreach (var result in results)
{
    Console.WriteLine($"{result.ClassName}: {result.Confidence:P2}");
}
```

## 🧪 测试状态

### ✅ 已完成测试
- **基础 TensorRT 功能**: 引擎加载、推理执行、结果获取
- **ONNX 自动转换**: 智能检测和转换ONNX模型为TensorRT引擎
- **类库架构**: 标准.NET类库，支持NuGet包分发
- **NuGet 包发布**: 已成功发布到 NuGet.org，支持包管理器安装
- **YOLO11 目标检测**: 完整的检测流程，包括预处理、推理、后处理、可视化
- **多模式架构**: 工厂模式、接口设计、模块化结构

### 🔧 开发完成待测试
- **YOLO11 图像分类**: 代码实现完成，待模型测试
- **可视化工具**: 检测框绘制、批量处理功能

### 📋 架构就绪待实现
- **实例分割**: 数据模型和接口已定义
- **定向边界框**: 数据模型和接口已定义  
- **姿态估计**: 数据模型和接口已定义

## 🛠️ 开发工作流

### 1. 修改Native代码
```bash
# 编辑 Native/src/tensorrt10_extern.cpp 或 Native/include/tensorrt10_extern.h
# 重新构建Native项目
cd Native
scripts\build.bat
```

### 2. 修改Managed代码
```bash
# 编辑 Managed/src/*.cs 文件
# 重新构建Managed类库项目
cd Managed
scripts\build.bat
```

### 3. 修改Yolo11Sharp代码
```bash
# 编辑 Yolo11Sharp/src/**/*.cs 文件
# 重新构建Yolo11Sharp项目
cd Yolo11Sharp
Scripts\build.bat
```

### 4. 添加新功能
1. 在Native项目中添加C++实现 (如需要)
2. 在Managed项目中添加C#封装 (如需要)
3. 在Yolo11Sharp项目中实现高级功能
4. 在Examples中添加使用示例
5. 更新相关文档

## 🐛 故障排除

### 常见问题

1. **DLL加载失败**
   ```
   问题: 找不到trt10.dll或其依赖项
   解决: 确保TensorRT库在系统PATH中，或使用run_example.bat
   ```

2. **TensorRT API错误**
   ```
   问题: API方法不存在
   解决: 确保使用TensorRT 10.x版本，检查CMakeLists.txt中的库链接
   ```

3. **构建失败**
   ```
   问题: CMake配置或编译错误
   解决: 检查Visual Studio环境，确保CUDA和TensorRT路径正确
   ```

4. **YOLO11 检测结果异常**
   ```
   问题: 坐标错误或置信度异常
   解决: 确保使用YOLO11格式的模型，检查输出维度是否为[1,84,8400]
   ```

5. **模型文件问题**
   ```
   问题: 引擎文件加载失败
   解决: 使用trtexec转换ONNX模型，确保TensorRT版本匹配
   ```

6. **NuGet 包安装问题**
   ```
   问题: 包安装失败或找不到包
   解决: 确保使用正确的包名 alanxinn.TensorRT10Sharp，检查网络连接
   ```

7. **运行时依赖缺失**
   ```
   问题: 运行时找不到 trt10.dll 或 CUDA 库
   解决: 确保安装了 TensorRT 10.x 和 CUDA 11.8+，并添加到系统 PATH
   ```

### 调试技巧
- 使用`run_example.bat`进行完整的环境检测
- 检查构建脚本的输出信息
- 验证所有依赖项是否正确安装
- 启用Yolo11Sharp的调试日志: `Logger.CurrentLevel = LogLevel.Debug`

## 📊 性能基准

### YOLO11 检测性能 (已测试)
**测试环境**: RTX 3080, Intel i7-10700K, 32GB RAM

| 模型 | 输入尺寸 | 推理时间 | FPS | 内存占用 |
|------|----------|----------|-----|----------|
| YOLO11n | 640×640 | ~15ms | ~67 | ~2GB |
| YOLO11s | 640×640 | ~25ms | ~40 | ~3GB |
| YOLO11m | 640×640 | ~45ms | ~22 | ~5GB |

*注: 性能数据仅供参考，实际性能取决于硬件配置*

## 📚 详细文档

- [Native项目文档](Native/README.md) - C++项目的详细说明
- [Managed项目文档](Managed/README.md) - C#项目的详细说明
- [Yolo11Sharp项目文档](Yolo11Sharp/README.md) - YOLO11推理库完整文档
- [Yolo11Sharp架构说明](Yolo11Sharp/ARCHITECTURE.md) - 多模式推理架构详解
- [构建说明](Documentation/BUILD_README.md) - 详细的构建指南
- [忽略文件指南](Documentation/IGNORE_FILES_GUIDE.md) - Git配置说明

## 🤝 贡献指南

1. **代码规范**: 遵循项目的编码规范 (参见 `.editorconfig`)
2. **提交规范**: 使用清晰的提交信息
3. **测试**: 确保新功能有相应的测试和示例
4. **文档**: 更新相关文档和README文件
5. **子项目**: 新增功能优先考虑在Yolo11Sharp中实现

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🔗 相关链接
- [NVIDIA TensorRT](https://developer.nvidia.com/tensorrt)
- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [YOLO11 官方仓库](https://github.com/ultralytics/ultralytics)

## 🙏 致谢

- https://github.com/guojin-yan/TensorRT-CSharp-API - 原始TensorRT C# API参考
- [YOLO11](https://github.com/ultralytics/ultralytics) - 优秀的目标检测模型
- [NVIDIA TensorRT](https://developer.nvidia.com/tensorrt) - 高性能推理引擎

---

⭐ 如果这个项目对您有帮助，请给我们一个 Star！