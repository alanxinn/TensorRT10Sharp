@echo off
REM ==============================================
REM TensorRT10Sharp Examples 运行脚本
REM ==============================================

echo [INFO] 运行 TensorRT10Sharp 示例程序...

REM 设置变量
set SCRIPT_DIR=%~dp0
set PROJECT_ROOT=%SCRIPT_DIR%..
set EXAMPLES_PROJECT=%PROJECT_ROOT%\Examples\TensorRT10Sharp.Examples.csproj
set BUILD_CONFIG=Release

REM 检查.NET SDK是否安装
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK 未安装或未添加到PATH中
    echo [INFO] 请从 https://dotnet.microsoft.com/download 下载并安装.NET 6.0 SDK
    pause
    exit /b 1
)

REM 检查示例项目是否存在
if not exist "%EXAMPLES_PROJECT%" (
    echo [ERROR] 示例项目文件不存在: %EXAMPLES_PROJECT%
    echo [INFO] 请先构建项目或检查项目结构
    pause
    exit /b 1
)

REM 检查Native DLL是否存在
if not exist "%PROJECT_ROOT%\..\trt10.dll" (
    echo [WARNING] Native DLL 不存在: %PROJECT_ROOT%\..\trt10.dll
    echo [INFO] 请先构建 Native 项目或确保 trt10.dll 存在于项目根目录
    echo [INFO] 继续运行示例程序...
)

REM 进入项目目录
cd /d "%PROJECT_ROOT%"

REM 检查是否已构建
set EXAMPLES_OUTPUT_DIR=%PROJECT_ROOT%\Examples\bin\%BUILD_CONFIG%\net6.0
if not exist "%EXAMPLES_OUTPUT_DIR%\TensorRT10Sharp.Examples.exe" (
    echo [INFO] 示例程序未构建，正在构建...
    call "%SCRIPT_DIR%build.bat"
    if %errorlevel% neq 0 (
        echo [ERROR] 构建失败，无法运行示例
        pause
        exit /b 1
    )
)

REM 运行示例程序
echo [INFO] 启动示例程序...
echo [INFO] 输出目录: %EXAMPLES_OUTPUT_DIR%
echo.

REM 使用 dotnet run 运行示例项目
dotnet run --project "%EXAMPLES_PROJECT%" --configuration %BUILD_CONFIG%

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] 示例程序运行失败
    pause
    exit /b 1
)

echo.
echo [INFO] 示例程序运行完成！
pause 