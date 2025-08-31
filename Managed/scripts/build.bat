@echo off
REM ==============================================
REM TensorRT10Sharp Managed 构建脚本
REM ==============================================

echo [INFO] 开始构建 TensorRT10Sharp Managed 项目...

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

REM 显示.NET版本信息
echo [INFO] .NET SDK 版本:
dotnet --version

REM 检查项目文件是否存在
if not exist "%PROJECT_FILE%" (
    echo [ERROR] 项目文件不存在: %PROJECT_FILE%
    pause
    exit /b 1
)

REM 检查Native DLL是否存在
if not exist "%PROJECT_ROOT%\..\trt10.dll" (
    echo [WARNING] Native DLL 不存在: %PROJECT_ROOT%\..\trt10.dll
    echo [INFO] 请先构建 Native 项目或确保 trt10.dll 存在于项目根目录
    echo [INFO] 继续构建 Managed 项目...
)

REM 进入项目目录
cd /d "%PROJECT_ROOT%"

REM 还原NuGet包
echo [INFO] 还原 NuGet 包...
dotnet restore "%PROJECT_FILE%"
if %errorlevel% neq 0 (
    echo [ERROR] NuGet 包还原失败
    pause
    exit /b 1
)

REM 构建项目
echo [INFO] 构建项目...
dotnet build "%PROJECT_FILE%" --configuration %BUILD_CONFIG% --no-restore
if %errorlevel% neq 0 (
    echo [ERROR] 构建失败
    pause
    exit /b 1
)

REM 检查输出文件 - 修复路径检查
set OUTPUT_DIR=%PROJECT_ROOT%\bin\x64\%BUILD_CONFIG%\net6.0
if exist "%OUTPUT_DIR%\TensorRT10Sharp.exe" (
    echo [SUCCESS] 构建成功！
    echo [INFO] 输出目录: %OUTPUT_DIR%
    echo [INFO] 可执行文件: %OUTPUT_DIR%\TensorRT10Sharp.exe
) else (
    REM 尝试其他可能的输出路径
    set OUTPUT_DIR=%PROJECT_ROOT%\bin\%BUILD_CONFIG%\net6.0
    if exist "%OUTPUT_DIR%\TensorRT10Sharp.exe" (
        echo [SUCCESS] 构建成功！
        echo [INFO] 输出目录: %OUTPUT_DIR%
        echo [INFO] 可执行文件: %OUTPUT_DIR%\TensorRT10Sharp.exe
    ) else (
        echo [ERROR] 未找到输出文件
        echo [INFO] 检查的路径:
        echo [INFO]   %PROJECT_ROOT%\bin\x64\%BUILD_CONFIG%\net6.0\TensorRT10Sharp.exe
        echo [INFO]   %PROJECT_ROOT%\bin\%BUILD_CONFIG%\net6.0\TensorRT10Sharp.exe
        pause
        exit /b 1
    )
)

echo [INFO] Managed 项目构建完成！
pause 