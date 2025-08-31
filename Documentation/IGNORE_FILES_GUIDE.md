# 忽略文件配置指南

本文档说明TensorRT10Sharp项目中各种忽略和配置文件的作用和使用方法。

## 📁 配置文件概览

### 1. `.gitignore` - Git忽略文件
控制哪些文件不被Git版本控制系统跟踪。

**主要忽略内容：**
- **编译输出文件**: `bin/`, `obj/`, `*.dll`, `*.exe`, `*.pdb` 等
- **构建目录**: `build/`, `cmake-build-*/`
- **IDE生成文件**: `.vs/`, `.idea/`, `*.user`, `*.suo` 等
- **大型模型文件**: `Assets/*.onnx`, `Assets/*.engine`, `*.plan`
- **临时和缓存文件**: `*.tmp`, `*.log`, `*.cache`
- **系统生成文件**: `Thumbs.db`, `.DS_Store` 等
- **TensorRT和CUDA系统库**: `nvinfer*.dll`, `cudart*.dll` 等

**当前配置特点：**
- ❌ **不保留trt10.dll**: 项目生成的DLL不提交到版本控制
- ✅ **保留源代码**: 所有`.cpp`, `.cs`, `.h`文件被跟踪
- ✅ **保留项目配置**: `*.csproj`, `CMakeLists.txt`等配置文件
- ✅ **保留脚本**: 所有`.bat`构建脚本

### 2. `.gitattributes` - Git属性配置
配置Git如何处理不同类型的文件。

**主要功能：**
- **行尾符处理**: 自动处理Windows/Linux行尾符差异
- **Git LFS配置**: 大型文件使用Git LFS管理
- **文本/二进制识别**: 正确识别文件类型
- **合并策略**: 为不同文件类型设置合并规则

**LFS管理的文件类型：**
```gitattributes
# 模型文件 (大型二进制文件)
*.onnx filter=lfs diff=lfs merge=lfs -text
*.engine filter=lfs diff=lfs merge=lfs -text
*.plan filter=lfs diff=lfs merge=lfs -text
*.pb filter=lfs diff=lfs merge=lfs -text
*.trt filter=lfs diff=lfs merge=lfs -text
*.wts filter=lfs diff=lfs merge=lfs -text

# 权重文件
*.pth filter=lfs diff=lfs merge=lfs -text
*.pt filter=lfs diff=lfs merge=lfs -text
*.ckpt filter=lfs diff=lfs merge=lfs -text
*.safetensors filter=lfs diff=lfs merge=lfs -text

# 压缩文件
*.zip filter=lfs diff=lfs merge=lfs -text
*.tar.gz filter=lfs diff=lfs merge=lfs -text
*.7z filter=lfs diff=lfs merge=lfs -text

# 媒体文件
*.mp4 filter=lfs diff=lfs merge=lfs -text
*.avi filter=lfs diff=lfs merge=lfs -text
*.mov filter=lfs diff=lfs merge=lfs -text
```

### 3. `.editorconfig` - 编辑器配置
统一不同编辑器和IDE的代码格式。

**配置内容：**
- **缩进**: 统一使用4个空格
- **行尾符**: Windows使用CRLF，其他平台使用LF
- **编码**: 统一使用UTF-8
- **代码风格**: C#和C++代码风格规则
- **文件格式**: 自动去除行尾空格，文件末尾添加空行

**示例配置：**
```ini
root = true

[*]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
trim_trailing_whitespace = true
indent_style = space
indent_size = 4

[*.{cs,cpp,h}]
indent_size = 4

[*.{bat,cmd}]
end_of_line = crlf

[*.md]
trim_trailing_whitespace = false
```

## 🚀 使用指南

### 初始化Git LFS
如果项目使用Git LFS管理大型文件，需要先初始化：

```bash
# 安装Git LFS (如果尚未安装)
git lfs install

# 跟踪大型文件类型
git lfs track "*.onnx"
git lfs track "*.engine"
git lfs track "*.plan"

# 查看LFS跟踪的文件
git lfs ls-files

# 查看LFS状态
git lfs status
```

### 检查忽略状态
```bash
# 检查特定文件是否被忽略
git check-ignore trt10.dll
git check-ignore build/

# 查看所有被忽略的文件
git status --ignored

# 查看忽略规则的来源
git check-ignore -v <文件路径>

# 强制添加被忽略的文件（谨慎使用）
git add -f <文件路径>
```

### 验证配置
```bash
# 检查当前忽略的文件
git ls-files --others --ignored --exclude-standard

# 检查LFS跟踪的文件
git lfs ls-files

# 验证行尾符配置
git config core.autocrlf
```

### 编辑器配置
大多数现代编辑器会自动读取`.editorconfig`文件：

- **Visual Studio**: 自动支持，无需额外配置
- **VS Code**: 需要安装EditorConfig扩展
- **JetBrains Rider**: 自动支持
- **CLion**: 自动支持
- **Notepad++**: 需要安装EditorConfig插件

## ⚠️ 注意事项

### 1. 大型文件管理策略

#### 模型文件处理
```bash
# 推荐做法：使用Git LFS
git lfs track "*.onnx"
git add .gitattributes
git add model.onnx
git commit -m "Add model file with LFS"

# 替代方案：外部存储
# 1. 上传到云存储 (Google Drive, OneDrive等)
# 2. 在README中提供下载链接
# 3. 使用脚本自动下载
```

#### 引擎文件处理
```bash
# TensorRT引擎文件通常不提交
# 原因：
# 1. 文件很大 (几百MB到几GB)
# 2. 平台相关 (GPU架构、驱动版本)
# 3. 可以从ONNX重新生成

# 建议：在Assets/README.md中说明如何生成
```

### 2. 敏感信息保护

#### 环境变量使用
```csharp
// 好的做法：使用环境变量
string tensorrtPath = Environment.GetEnvironmentVariable("TENSORRT_PATH") 
    ?? @"C:\Program Files\NVIDIA GPU Computing Toolkit\TensorRT-10.13.0.35_cuda-11.8";

// 避免：硬编码绝对路径
string tensorrtPath = @"D:\MyCustomPath\TensorRT"; // 不要这样做
```

#### 配置文件管理
```json
// appsettings.json (可以提交)
{
  "TensorRT": {
    "DefaultPath": "C:\\Program Files\\NVIDIA GPU Computing Toolkit\\TensorRT-10.13.0.35_cuda-11.8",
    "MemoryLimit": 1024
  }
}

// appsettings.local.json (被忽略，不提交)
{
  "TensorRT": {
    "DefaultPath": "D:\\MyCustomPath\\TensorRT",
    "ApiKey": "your-secret-key"
  }
}
```

### 3. 跨平台兼容性

#### 路径处理
```csharp
// 好的做法：使用Path.Combine
string modelPath = Path.Combine("Assets", "model.onnx");

// 避免：硬编码路径分隔符
string modelPath = "Assets\\model.onnx"; // Windows特定
string modelPath = "Assets/model.onnx";  // Unix特定
```

#### 条件编译
```csharp
#if WINDOWS
    const string DefaultTensorRTPath = @"C:\Program Files\NVIDIA GPU Computing Toolkit\TensorRT-10.13.0.35_cuda-11.8";
#elif LINUX
    const string DefaultTensorRTPath = "/usr/local/tensorrt";
#endif
```

## 🔧 自定义配置

### 添加新的忽略规则

#### 临时忽略文件
```gitignore
# 在.gitignore中添加
# 临时测试文件
test_*.onnx
temp_models/
*.tmp

# 个人配置文件
.vscode/settings.json
*.user.json
```

#### 项目特定忽略
```gitignore
# TensorRT10Sharp特定忽略
# 构建产物
trt10.dll
*.engine
build/

# 测试数据
TestData/
Benchmarks/results/
```

### 配置新的LFS文件类型

#### 添加新的大型文件类型
```bash
# 跟踪新的文件类型
git lfs track "*.tensorrt"
git lfs track "*.tflite"
git lfs track "*.mlmodel"

# 更新.gitattributes
git add .gitattributes
git commit -m "Add LFS tracking for new model formats"
```

### 修改编辑器配置

#### 针对特定文件类型
```ini
# .editorconfig中添加
[*.{cmake,CMakeLists.txt}]
indent_size = 2

[*.{json,yml,yaml}]
indent_size = 2

[*.{bat,cmd,ps1}]
end_of_line = crlf
indent_size = 2
```

## 🔍 故障排除

### 常见问题

#### 1. LFS文件未正确跟踪
```bash
# 问题：大文件被直接提交到Git
# 解决：
git rm --cached large_file.onnx
git lfs track "*.onnx"
git add .gitattributes
git add large_file.onnx
git commit -m "Move large file to LFS"
```

#### 2. 忽略规则不生效
```bash
# 问题：文件已被跟踪，忽略规则不起作用
# 解决：先取消跟踪
git rm --cached file_to_ignore
git commit -m "Remove file from tracking"
# 然后添加到.gitignore
```

#### 3. 行尾符问题
```bash
# 问题：Windows和Linux行尾符混乱
# 解决：重新规范化
git add --renormalize .
git commit -m "Normalize line endings"
```

### 验证配置正确性

#### 检查忽略文件
```bash
# 测试忽略规则
echo "test content" > trt10.dll
git status  # 应该不显示trt10.dll

# 测试LFS
git lfs ls-files  # 查看LFS跟踪的文件
```

#### 检查编辑器配置
```bash
# 在不同编辑器中打开同一文件
# 验证缩进和行尾符是否一致
```

## 📚 相关资源

### 官方文档
- [Git忽略文件官方文档](https://git-scm.com/docs/gitignore)
- [Git LFS官方网站](https://git-lfs.github.io/)
- [EditorConfig官方网站](https://editorconfig.org/)
- [Git属性文档](https://git-scm.com/docs/gitattributes)

### 实用工具
- [gitignore.io](https://www.toptal.com/developers/gitignore) - 生成.gitignore文件
- [EditorConfig检查器](https://editorconfig-checker.github.io/) - 验证EditorConfig规则
- [Git LFS迁移工具](https://github.com/git-lfs/git-lfs/wiki/Tutorial#migrating-existing-repository-data-to-lfs)

### 最佳实践指南
- [GitHub大型文件管理指南](https://docs.github.com/en/repositories/working-with-files/managing-large-files)
- [Git工作流最佳实践](https://www.atlassian.com/git/tutorials/comparing-workflows)
- [跨平台开发注意事项](https://docs.microsoft.com/en-us/dotnet/core/compatibility/) 