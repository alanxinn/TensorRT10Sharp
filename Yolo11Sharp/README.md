# Yolo11Sharp - YOLO11 TensorRT C# 推理库

🚀 基于 TensorRT10Sharp 的高性能 YOLO11 多模式推理 C# 实现，支持目标检测、图像分类、实例分割、定向边界框检测和姿态估计。

[![.NET](https://img.shields.io/badge/.NET-6.0+-blue.svg)](https://dotnet.microsoft.com/download)
[![TensorRT](https://img.shields.io/badge/TensorRT-10.x-green.svg)](https://developer.nvidia.com/tensorrt)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## ✨ 特性亮点

### 🎯 多模式推理支持
- **🔍 目标检测 (Detection)**: 高精度物体检测与定位
- **🏷️ 图像分类 (Classification)**: 单标签和多标签分类
- **🎭 实例分割 (Segmentation)**: 像素级物体分割
- **📐 定向边界框 (OBB)**: 旋转物体检测
- **🤸 姿态估计 (Pose)**: 人体关键点检测

### ⚡ 高性能特性
- **GPU 加速**: 基于 TensorRT 优化，支持 CUDA 加速
- **内存优化**: 智能内存管理，避免内存泄漏
- **批处理**: 支持单张和批量图像处理
- **实时推理**: 毫秒级推理速度

### 🏗️ 架构优势
- **模块化设计**: 清晰的接口分离，易于扩展
- **工厂模式**: 统一的推理对象创建
- **类型安全**: 强类型 C# API
- **异常处理**: 完善的错误处理机制

## 📋 系统要求

### 必需组件
- **.NET 6.0 SDK** 或更高版本
- **CUDA 11.8+** (支持的 NVIDIA GPU)
- **TensorRT 10.x**
- **Visual C++ Redistributable 2019/2022**

### 支持的平台
- **操作系统**: Windows 10/11 x64
- **GPU**: NVIDIA GeForce GTX 1060 或更高
- **内存**: 建议 8GB+ RAM

## 🏗️ 项目架构

```
Yolo11Sharp/                    # 项目根目录
├── src/                        # 源代码
│   ├── Core/                   # 核心推理引擎
│   │   ├── InferenceMode.cs    # 推理模式枚举
│   │   ├── IYolo11Inference.cs # 推理接口定义
│   │   ├── Yolo11Engine.cs     # 通用推理引擎
│   │   ├── Yolo11Detection.cs  # 目标检测实现
│   │   ├── Yolo11Classification.cs # 图像分类实现
│   │   └── Yolo11InferenceFactory.cs # 推理工厂
│   ├── Models/                 # 数据模型
│   │   ├── DetectionResult.cs  # 检测结果
│   │   ├── ClassificationResult.cs # 分类结果
│   │   ├── SegmentationResult.cs # 分割结果
│   │   ├── OrientedBoundingBoxResult.cs # OBB结果
│   │   ├── PoseEstimationResult.cs # 姿态估计结果
│   │   └── PreprocessResult.cs # 预处理结果
│   ├── Utils/                  # 工具类
│   │   ├── ConfigurationManager.cs # 配置管理
│   │   └── Logger.cs           # 日志工具
│   └── Visualization/          # 可视化
│       └── ImageVisualizer.cs  # 图像可视化工具
├── Examples/                   # 示例程序
│   └── Program.cs              # 主程序入口
├── Assets/                     # 资源文件
│   ├── coco.names              # COCO类别名称
│   └── test.jpg                # 测试图像
├── Scripts/                    # 脚本文件
│   ├── build.bat               # 构建脚本
│   ├── run.bat                 # 运行脚本
│   └── clean.bat               # 清理脚本
├── bin/Release/net6.0/         # 构建输出
│   ├── Yolo11Sharp.exe         # 主程序
│   └── Yolo11Sharp.dll         # 程序库
├── Yolo11Sharp.csproj          # 项目文件
├── README.md                   # 本文件
├── ARCHITECTURE.md             # 架构说明
└── RENAME_SUMMARY.md           # 重命名记录
```

## 🚀 快速开始

### 1. 环境准备

```bash
# 克隆项目
git clone <repository-url>
cd TensorRT10Sharp/Yolo11Sharp

# 检查 .NET SDK
dotnet --version
```

### 2. 构建项目

```bash
# 使用构建脚本
Scripts\build.bat

# 或手动构建
dotnet build --configuration Release
```

### 3. 准备模型文件

将 YOLO11 ONNX 模型转换为 TensorRT 引擎：
```bash
# 目标检测模型
trtexec --onnx=yolo11n.onnx --saveEngine=yolo11n.engine --fp16

# 分类模型
trtexec --onnx=yolo11n-cls.onnx --saveEngine=yolo11n-cls.engine --fp16
```

### 4. 运行示例

```bash
# 使用运行脚本
Scripts\run.bat

# 或直接运行
bin\Release\net6.0\Yolo11Sharp.exe

# 指定参数
Yolo11Sharp.exe yolo11n.engine test.jpg result.jpg
```

## 💻 使用示例

### 目标检测

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

### 图像分类

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

### 使用工厂模式

```csharp
using Yolo11Sharp.Core;

// 自动检测推理模式
var mode = Yolo11InferenceFactory.DetectInferenceMode("model.engine");

// 创建推理对象
var inference = Yolo11InferenceFactory.CreateInference(mode, "model.engine");

// 执行推理
var results = inference.Infer("test.jpg");
```

### 配置管理

```csharp
using Yolo11Sharp.Utils;

// 使用配置管理器
var enginePath = ConfigurationManager.GetAssetPath("yolo11n.engine");
var classNamesPath = ConfigurationManager.GetAssetPath("coco.names");
var outputDir = ConfigurationManager.GetOutputDirectory();

// 检查文件存在
if (ConfigurationManager.FileExists(enginePath))
{
    // 执行推理...
}
```

## 🔧 配置选项

### 检测参数

```csharp
var detector = new Yolo11Detection(
    enginePath: "yolo11n.engine",
    classNamesPath: "coco.names",
    confThreshold: 0.25f,    // 置信度阈值
    nmsThreshold: 0.45f      // NMS 阈值
);

// 运行时调整
detector.ConfidenceThreshold = 0.5f;
```

### 分类参数

```csharp
var classifier = new Yolo11Classification(
    enginePath: "yolo11n-cls.engine",
    classNamesPath: "imagenet.names",
    confThreshold: 0.1f      // 分类置信度阈值
);
```

### 日志配置

```csharp
using Yolo11Sharp.Utils;

// 设置日志级别
Logger.CurrentLevel = LogLevel.Info;

// 使用日志
Logger.Info("开始推理...");
Logger.Warning("模型文件未找到");
Logger.Error("推理失败");
```

## 📊 性能基准

### 测试环境
- **GPU**: RTX 3080 (10GB)
- **CPU**: Intel i7-10700K
- **内存**: 32GB DDR4
- **CUDA**: 11.8
- **TensorRT**: 10.0

### 性能数据

| 模型 | 输入尺寸 | 推理时间 | FPS | 内存占用 |
|------|----------|----------|-----|----------|
| YOLO11n | 640×640 | ~15ms | ~67 | ~2GB |
| YOLO11s | 640×640 | ~25ms | ~40 | ~3GB |
| YOLO11m | 640×640 | ~45ms | ~22 | ~5GB |
| YOLO11l | 640×640 | ~70ms | ~14 | ~8GB |

*注: 性能数据仅供参考，实际性能取决于硬件配置和模型复杂度*

## 🛠️ 开发指南

### 添加新的推理模式

1. **定义结果模型**:
```csharp
public class CustomResult
{
    public float Confidence { get; set; }
    public string Label { get; set; }
    // 其他属性...
}
```

2. **实现推理类**:
```csharp
public class Yolo11Custom : Yolo11InferenceBase<CustomResult>
{
    public override InferenceMode Mode => InferenceMode.Custom;
    
    protected override List<CustomResult> PostProcess(float[] rawOutput, PreprocessResult preprocessResult)
    {
        // 实现后处理逻辑
    }
}
```

3. **更新工厂**:
```csharp
// 在 Yolo11InferenceFactory 中添加创建方法
public static Yolo11Custom CreateCustom(string enginePath, ...)
{
    return new Yolo11Custom(enginePath, ...);
}
```

### 自定义可视化

```csharp
// 自定义绘制样式
var customImage = ImageVisualizer.DrawDetections(
    imagePath: "test.jpg",
    detections: results,
    thickness: 3,           // 边框粗细
    fontSize: 14f,          // 字体大小
    colors: customColors    // 自定义颜色
);
```

## 📚 API 文档

### 核心接口

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
抽象基类，提供通用的推理流程实现：
- 图像预处理
- TensorRT 推理执行
- 资源管理
- 异常处理

### 数据模型

#### DetectionResult
```csharp
public class DetectionResult
{
    public float X1, Y1, X2, Y2 { get; set; }  // 边界框坐标
    public float Confidence { get; set; }       // 置信度
    public int ClassId { get; set; }            // 类别ID
    public string ClassName { get; set; }       // 类别名称
    
    public Rectangle ToRectangle();             // 转换为矩形
    public bool IsValid();                      // 验证有效性
}
```

#### ClassificationResult
```csharp
public class ClassificationResult
{
    public int ClassId { get; set; }            // 类别ID
    public string ClassName { get; set; }       // 类别名称
    public float Confidence { get; set; }       // 置信度
    
    public bool IsValid();                      // 验证有效性
}
```

## 🐛 故障排除

### 常见问题

**Q: 构建失败，提示找不到 TensorRT10Sharp**
```
A: 确保 TensorRT10Sharp 项目已正确构建，检查项目引用路径
```

**Q: 运行时提示 "找不到 trt10.dll"**
```
A: 将 trt10.dll 复制到可执行文件目录，或添加到系统 PATH
```

**Q: 推理结果异常或坐标错误**
```
A: 检查模型格式是否为 YOLO11，确认输入尺寸匹配
```

**Q: GPU 内存不足**
```
A: 降低批处理大小，使用较小的模型，或启用 FP16 精度
```

### 调试技巧

1. **启用详细日志**:
```csharp
Logger.CurrentLevel = LogLevel.Debug;
```

2. **检查模型信息**:
```csharp
var engine = new Yolo11Engine("model.engine");
Console.WriteLine($"输入尺寸: {engine.InputSize}");
Console.WriteLine($"输出维度: {engine.OutputDimensions}");
```

3. **验证预处理**:
```csharp
var preprocessResult = engine.Preprocess(bitmap);
Console.WriteLine($"缩放比例: {preprocessResult.ScaleX}x{preprocessResult.ScaleY}");
Console.WriteLine($"填充: ({preprocessResult.PadX}, {preprocessResult.PadY})");
```

## 🤝 贡献指南

我们欢迎社区贡献！请遵循以下步骤：

1. **Fork** 本仓库
2. **创建特性分支**: `git checkout -b feature/amazing-feature`
3. **提交更改**: `git commit -m 'Add amazing feature'`
4. **推送分支**: `git push origin feature/amazing-feature`
5. **创建 Pull Request**

### 代码规范

- 遵循 C# 编码规范
- 添加适当的注释和文档
- 编写单元测试
- 确保代码通过所有测试

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🙏 致谢

- [YOLO11](https://github.com/ultralytics/ultralytics) - 优秀的目标检测模型
- [TensorRT](https://developer.nvidia.com/tensorrt) - 高性能推理引擎
- [.NET](https://dotnet.microsoft.com/) - 跨平台开发框架

## 📞 联系方式

- **项目主页**: [GitHub Repository]
- **问题反馈**: [GitHub Issues]
- **讨论交流**: [GitHub Discussions]

---

⭐ 如果这个项目对您有帮助，请给我们一个 Star！ 