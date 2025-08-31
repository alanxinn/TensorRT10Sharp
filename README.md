# TensorRT10Sharp C#

TensorRT10Sharp C# 是一个为 .NET 平台提供的 NVIDIA TensorRT10 封装库，使开发者能够在 C# 中轻松使用 TensorRT 进行高性能深度学习推理。

## ✨ 最新更新

- ✅ **TensorRT 10 API兼容性**: 完全适配TensorRT 10.x的新API
- ✅ **构建系统优化**: 改进的CMake配置和批处理脚本
- ✅ **错误处理增强**: 更好的错误检测和诊断
- ✅ **文档完善**: 详细的构建指南和故障排除
- ✅ **跨平台支持**: 优化的Windows构建流程

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
├── Managed/                        # C# 托管项目
│   ├── src/                        # C# 源文件
│   │   ├── Dims.cs                 # 维度结构体
│   │   └── Nvinfer.cs              # TensorRT推理引擎类
│   ├── examples/                   # C# 示例代码
│   │   └── BasicExample.cs        # 基础使用示例
│   ├── scripts/                    # C# 构建脚本
│   │   ├── build.bat              # 构建Managed项目
│   │   ├── run.bat                # 运行示例程序
│   │   └── clean.bat              # 清理Managed项目
│   ├── TensorRT10Sharp.csproj     # C# 项目文件
│   └── README.md                  # Managed项目说明
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

### 1. 环境要求

#### Native (C++) 项目
- **CUDA Toolkit** (推荐版本 11.8)
- **TensorRT** (推荐版本 TensorRT-10.13.0.35)
- **Visual Studio 2019/2022** (包含C++开发工具)
- **CMake** (版本 3.10或更高)

#### Managed (C#) 项目
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
```

### 3. 运行示例

```bash
# 方法1: 使用示例脚本
run_example.bat

# 方法2: 直接运行C#程序
cd Managed
scripts\run.bat

# 方法3: 使用.NET CLI
dotnet run --project Managed\TensorRT10Sharp.csproj
```

## 🔧 项目特点

### 分离式架构
- **Native项目**: 独立的C++项目，封装TensorRT C++ API
- **Managed项目**: 独立的C#项目，提供.NET友好的API接口
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

## 📖 使用示例

```csharp
using TensorRTSharp;

// 创建推理引擎
using var infer = new Nvinfer("Assets/yolo11n.engine");

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
# 重新构建Managed项目
cd Managed
scripts\build.bat
```

### 3. 添加新功能
1. 在Native项目中添加C++实现
2. 在Managed项目中添加C#封装
3. 在examples中添加使用示例
4. 更新相关文档

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

### 调试技巧
- 使用`run_example.bat`进行完整的环境检测
- 检查构建脚本的输出信息
- 验证所有依赖项是否正确安装

## 📚 详细文档

- [Native项目文档](Native/README.md) - C++项目的详细说明
- [Managed项目文档](Managed/README.md) - C#项目的详细说明
- [构建说明](Documentation/BUILD_README.md) - 详细的构建指南
- [忽略文件指南](Documentation/IGNORE_FILES_GUIDE.md) - Git配置说明

## 🤝 贡献指南

1. **代码规范**: 遵循项目的编码规范 (参见 `.editorconfig`)
2. **提交规范**: 使用清晰的提交信息
3. **测试**: 确保新功能有相应的测试和示例
4. **文档**: 更新相关文档和README文件

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🔗 相关链接
- [NVIDIA TensorRT](https://developer.nvidia.com/tensorrt)
- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) 