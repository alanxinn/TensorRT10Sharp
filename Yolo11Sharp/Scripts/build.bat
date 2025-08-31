@echo off
echo ====================================
echo YOLO11 TensorRT C# 项目构建脚本
echo ====================================

:: 设置编码为UTF-8
chcp 65001 > nul

:: 切换到项目根目录
cd /d "%~dp0.."

:: 检查.NET SDK
echo 检查 .NET SDK...
dotnet --version > nul 2>&1
if %errorlevel% neq 0 (
    echo 错误: 未找到 .NET SDK，请先安装 .NET 6.0 SDK 或更高版本
    echo 下载地址: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo .NET SDK 版本:
dotnet --version

:: 检查必需文件
echo.
echo 检查必需文件...
if not exist "../trt10.dll" (
    echo 警告: 未找到 trt10.dll，请确保已构建 Native 项目
)

if not exist "../Managed/TensorRT10Sharp.csproj" (
    echo 错误: 未找到 TensorRT10Sharp.csproj，请检查项目结构
    pause
    exit /b 1
)

:: 清理之前的构建
echo.
echo 清理之前的构建...
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"

:: 还原依赖包
echo.
echo 还原 NuGet 包...
dotnet restore
if %errorlevel% neq 0 (
    echo 错误: NuGet 包还原失败
    pause
    exit /b 1
)

:: 构建项目 (Release)
echo.
echo 构建项目 (Release 配置)...
dotnet build --configuration Release --verbosity minimal
if %errorlevel% neq 0 (
    echo 错误: Release 构建失败
    pause
    exit /b 1
)

:: 检查输出文件
echo.
echo 检查输出文件...
set RELEASE_DIR=bin\Release\net6.0

if exist "%RELEASE_DIR%\Yolo11Csharp.exe" (
    echo ✓ Release 版本构建成功: %RELEASE_DIR%\Yolo11Csharp.exe
) else (
    echo ✗ Release 版本构建失败
    pause
    exit /b 1
)

:: 检查依赖文件
echo.
echo 检查依赖文件...
if exist "%RELEASE_DIR%\trt10.dll" (
    echo ✓ trt10.dll 已复制到输出目录
) else (
    echo ✗ trt10.dll 未找到，运行时可能出错
)

if exist "%RELEASE_DIR%\System.Drawing.Common.dll" (
    echo ✓ System.Drawing.Common.dll 已复制
) else (
    echo ✗ System.Drawing.Common.dll 未找到
)

if exist "%RELEASE_DIR%\coco.names" (
    echo ✓ coco.names 已复制
) else (
    echo ✗ coco.names 未找到
)

:: 显示构建信息
echo.
echo ====================================
echo 构建完成！
echo ====================================
echo.
echo 输出目录: %RELEASE_DIR%
echo.
echo 使用方法:
echo   cd %RELEASE_DIR%
echo   Yolo11Csharp.exe
echo.
echo 注意事项:
echo   1. 确保 GPU 驱动和 CUDA 已正确安装
echo   2. 将 YOLO11 模型文件 (.engine 或 .onnx) 放在可执行文件目录
echo   3. 准备测试图像文件
echo.

pause 