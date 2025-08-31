# TensorRT10Sharp Managed (C#)

这是TensorRT10Sharp项目的托管C#部分，提供.NET友好的API接口，封装了Native C++库的功能。

## 📁 项目结构

```
Managed/
├── src/                    # C#源文件
│   ├── Dims.cs            # 维度结构体
│   └── Nvinfer.cs         # TensorRT推理引擎类
├── examples/               # 示例代码
│   └── BasicExample.cs    # 基础使用示例
├── scripts/                # 构建和运行脚本
│   ├── build.bat          # 构建脚本
│   ├── run.bat             # 运行脚本
│   └── clean.bat          # 清理脚本
├── TensorRT10Sharp.csproj  # C#项目文件
└── README.md              # 本文件
```

## 🔧 环境要求

### 必需软件
1. **.NET 6.0 SDK**
   - 下载地址: https://dotnet.microsoft.com/download
   - 确保安装了.NET 6.0或更高版本

2. **Native DLL依赖**
   - 需要先构建Native项目生成`trt10.dll`
   - 或确保项目根目录存在`trt10.dll`文件

### 可选工具
- **Visual Studio 2022** 或 **JetBrains Rider** (用于IDE开发)
- **Visual Studio Code** (轻量级编辑器)

## 🚀 构建和运行

### 方法1: 使用脚本 (推荐)
```bash
# 构建项目
scripts\build.bat

# 运行示例
scripts\run.bat

# 清理项目
scripts\clean.bat
```

### 方法2: 使用.NET CLI
```bash
# 还原依赖
dotnet restore

# 构建项目
dotnet build --configuration Release

# 运行项目
dotnet run --configuration Release
```

## 📝 API使用说明

### 基础用法

#### 1. 创建推理引擎
```csharp
using TensorRTSharp;

// 方法1: 直接加载TensorRT引擎文件
using var infer = new Nvinfer("model.engine");

// 方法2: 先转换ONNX模型再加载
bool success = Nvinfer.ConvertOnnxToEngine("model.onnx", 1024);
if (success)
{
    using var infer = new Nvinfer("model.engine");
}
```

#### 2. 获取模型信息
```csharp
// 获取输入输出数量
int inputCount = infer.GetInputCount();
int outputCount = infer.GetOutputCount();

// 获取输入输出名称和维度
for (int i = 0; i < inputCount; i++)
{
    string inputName = infer.GetInputName(i);
    Dims inputDims = infer.GetBindingDimensions(inputName);
    Console.WriteLine($"输入 {i}: {inputName}, 维度: {FormatDims(inputDims)}");
}
```

#### 3. 执行推理
```csharp
// 准备输入数据
string inputName = infer.GetInputName(0);
Dims inputDims = infer.GetBindingDimensions(inputName);
float[] inputData = new float[inputDims.GetElementCount()];

// 填充输入数据...
// inputData[i] = ...;

// 加载数据并执行推理
infer.LoadInferenceData(inputName, inputData);
infer.Infer();

// 获取输出结果
string outputName = infer.GetOutputName(0);
float[] outputData = infer.GetInferenceResult(outputName);
```

### Dims 结构体使用

```csharp
// 创建维度结构体
Dims dims = new Dims();

// 设置维度
dims.SetDimension(0, 1);    // batch size
dims.SetDimension(1, 3);    // channels
dims.SetDimension(2, 640);  // height
dims.SetDimension(3, 640);  // width

// 获取维度信息
int batchSize = dims.GetDimension(0);
int channels = dims.GetDimension(1);
int totalElements = dims.GetElementCount();

Console.WriteLine($"维度: {dims.nbDims}");
Console.WriteLine($"总元素数: {totalElements}");
```

## 🎯 示例程序

项目包含一个完整的示例程序 (`examples/BasicExample.cs`)，演示了：

1. **ONNX模型转换**: 将ONNX模型转换为TensorRT引擎
2. **模型加载**: 加载TensorRT引擎文件
3. **信息查询**: 获取模型的输入输出信息
4. **推理执行**: 执行完整的推理流程
5. **结果获取**: 获取和处理推理结果
6. **Dims测试**: 测试维度结构体的各种功能

### 运行示例
```bash
# 确保项目根目录有模型文件
# - yolo11n.onnx (ONNX模型)
# - yolo11n.engine (TensorRT引擎，可选)

# 运行示例
scripts\run.bat
```

## 🔧 项目配置

### 项目属性
- **目标框架**: .NET 6.0
- **平台**: x64
- **允许不安全代码**: 是
- **启动对象**: TensorRTSharp.Examples.BasicExample

### 依赖项
- **Native库**: `../trt10.dll` (自动复制到输出目录)
- **资源文件**: `../Assets/*.*` (除大型模型文件外)

## 🐛 故障排除

### 常见问题

1. **DllNotFoundException**
   ```
   解决方案: 确保trt10.dll存在于输出目录或项目根目录
   ```

2. **InvalidOperationException: Failed to create TensorRT inference engine**
   ```
   解决方案: 检查CUDA和TensorRT是否正确安装
   ```

3. **ArgumentNullException**
   ```
   解决方案: 检查模型文件路径是否正确
   ```

### 调试技巧
- 使用Debug配置构建以获得更多调试信息
- 检查Native DLL是否正确加载
- 验证模型文件是否存在且格式正确

## 📊 性能优化

### 最佳实践
1. **预分配数组**: 重复使用输入输出数组，避免频繁分配
2. **批处理**: 使用批处理提高吞吐量
3. **异步处理**: 对于多个推理请求，考虑异步处理
4. **内存管理**: 及时释放大型数组和对象

### 示例优化代码
```csharp
// 预分配输入输出数组
float[] inputBuffer = new float[inputSize];
float[] outputBuffer;

// 重复使用
for (int i = 0; i < batchCount; i++)
{
    // 填充输入数据到预分配的缓冲区
    FillInputData(inputBuffer, i);
    
    // 执行推理
    infer.LoadInferenceData(inputName, inputBuffer);
    infer.Infer();
    outputBuffer = infer.GetInferenceResult(outputName);
    
    // 处理输出...
}
```

## 📚 相关文档

- [.NET 6.0 文档](https://docs.microsoft.com/en-us/dotnet/)
- [P/Invoke 互操作](https://docs.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke)
- [TensorRT C# API 指南](../Documentation/README.md) 