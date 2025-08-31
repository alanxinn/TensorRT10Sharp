@echo off
REM ==============================================
REM TensorRT10Sharp Managed 运行脚本
REM ==============================================

echo [INFO] 运行 TensorRT10Sharp Managed 项目...

REM 设置变量
set SCRIPT_DIR=%~dp0
set PROJECT_ROOT=%SCRIPT_DIR%..
set PROJECT_FILE=%PROJECT_ROOT%\TensorRT10Sharp.csproj
set BUILD_CONFIG=Release

REM 检查.NET SDK是否安装
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK 未安装或未添加到PATH中
    echo [INFO] 请从 https://dotnet.microsoft.com/download 下载并安装.NET 6.0 SDK
    pause
    exit /b 1
)

REM 检查项目文件是否存在
if not exist "%PROJECT_FILE%" (
    echo [ERROR] 项目文件不存在: %PROJECT_FILE%
    pause
    exit /b 1
)

REM 检查Native DLL是否存在
if not exist "%PROJECT_ROOT%\..\trt10.dll" (
    echo [ERROR] Native DLL 不存在: %PROJECT_ROOT%\..\trt10.dll
    echo [INFO] 请先构建 Native 项目
    pause
    exit /b 1
)

REM 进入项目目录
cd /d "%PROJECT_ROOT%"

REM 检查是否已构建 - 修复路径检查
set OUTPUT_DIR=%PROJECT_ROOT%\bin\x64\%BUILD_CONFIG%\net6.0
if not exist "%OUTPUT_DIR%\TensorRT10Sharp.exe" (
    REM 尝试其他可能的输出路径
    set OUTPUT_DIR=%PROJECT_ROOT%\bin\%BUILD_CONFIG%\net6.0
    if not exist "%OUTPUT_DIR%\TensorRT10Sharp.exe" (
        echo [INFO] 项目尚未构建，正在构建...
        call "%PROJECT_ROOT%\scripts\build.bat"
        if %errorlevel% neq 0 (
            echo [ERROR] 构建失败，无法运行
            pause
            exit /b 1
        )
    )
)

REM 运行项目
echo [INFO] 运行项目...
echo ============================================
dotnet run --project "%PROJECT_FILE%" --configuration %BUILD_CONFIG%
echo ============================================

if %errorlevel% neq 0 (
    echo [ERROR] 运行失败
    pause
    exit /b 1
)

echo [INFO] 程序运行完成！
pause 