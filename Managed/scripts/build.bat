@echo off
REM ==============================================
REM TensorRT10Sharp Managed 构建脚本
REM ==============================================

echo [INFO] 开始构建 TensorRT10Sharp Managed 类库项目...

REM 设置变量
set SCRIPT_DIR=%~dp0
set PROJECT_ROOT=%SCRIPT_DIR%..
set PROJECT_FILE=%PROJECT_ROOT%\TensorRT10Sharp.csproj
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

REM 构建类库项目
echo [INFO] 构建类库项目...
dotnet build "%PROJECT_FILE%" --configuration %BUILD_CONFIG% --no-restore
if %errorlevel% neq 0 (
    echo [ERROR] 类库构建失败
    pause
    exit /b 1
)

REM 检查类库输出文件
set OUTPUT_DIR=%PROJECT_ROOT%\bin\%BUILD_CONFIG%\net6.0
if exist "%OUTPUT_DIR%\TensorRT10Sharp.dll" (
    echo [SUCCESS] 类库构建成功！
    echo [INFO] 输出目录: %OUTPUT_DIR%
    echo [INFO] 类库文件: %OUTPUT_DIR%\TensorRT10Sharp.dll
) else (
    echo [ERROR] 未找到类库输出文件
    echo [INFO] 检查的路径: %OUTPUT_DIR%\TensorRT10Sharp.dll
    pause
    exit /b 1
)

REM 构建示例项目（如果存在）
if exist "%EXAMPLES_PROJECT%" (
    echo [INFO] 构建示例项目...
    dotnet restore "%EXAMPLES_PROJECT%"
    if %errorlevel% neq 0 (
        echo [ERROR] 示例项目 NuGet 包还原失败
        pause
        exit /b 1
    )
    
    dotnet build "%EXAMPLES_PROJECT%" --configuration %BUILD_CONFIG% --no-restore
    if %errorlevel% neq 0 (
        echo [ERROR] 示例项目构建失败
        pause
        exit /b 1
    )
    
    REM 检查示例项目输出
    set EXAMPLES_OUTPUT_DIR=%PROJECT_ROOT%\Examples\bin\%BUILD_CONFIG%\net6.0
    if exist "%EXAMPLES_OUTPUT_DIR%\TensorRT10Sharp.Examples.exe" (
        echo [SUCCESS] 示例项目构建成功！
        echo [INFO] 示例输出目录: %EXAMPLES_OUTPUT_DIR%
        echo [INFO] 示例可执行文件: %EXAMPLES_OUTPUT_DIR%\TensorRT10Sharp.Examples.exe
    ) else (
        echo [WARNING] 示例项目输出文件未找到
    )
) else (
    echo [INFO] 示例项目不存在，跳过构建
)

echo [INFO] Managed 项目构建完成！
pause 