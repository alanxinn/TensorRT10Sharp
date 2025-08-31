@echo off
REM ==============================================
REM TensorRT10Sharp Native 构建脚本
REM ==============================================

echo [INFO] 开始构建 TensorRT10Sharp Native 项目...

REM 设置变量
set SCRIPT_DIR=%~dp0
set PROJECT_ROOT=%SCRIPT_DIR%..
set BUILD_DIR=%PROJECT_ROOT%\build
set CMAKE_BUILD_TYPE=Release

REM 检查CMake是否安装
cmake --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] CMake 未安装或未添加到PATH中
    echo [INFO] 请从 https://cmake.org/download/ 下载并安装CMake
    pause
    exit /b 1
)

REM 检查Visual Studio是否安装
where cl >nul 2>&1
if %errorlevel% neq 0 (
    echo [INFO] 正在设置 Visual Studio 环境...
    
    REM 尝试找到 Visual Studio 2022
    if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\VC\Auxiliary\Build\vcvars64.bat" (
        call "C:\Program Files\Microsoft Visual Studio\2022\Professional\VC\Auxiliary\Build\vcvars64.bat"
    ) else if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars64.bat" (
        call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars64.bat"
    ) else if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\VC\Auxiliary\Build\vcvars64.bat" (
        call "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\VC\Auxiliary\Build\vcvars64.bat"
    ) else (
        REM 尝试找到 Visual Studio 2019
        if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\VC\Auxiliary\Build\vcvars64.bat" (
            call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\VC\Auxiliary\Build\vcvars64.bat"
        ) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\vcvars64.bat" (
            call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\vcvars64.bat"
        ) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\VC\Auxiliary\Build\vcvars64.bat" (
            call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\VC\Auxiliary\Build\vcvars64.bat"
        ) else (
            echo [ERROR] 未找到 Visual Studio 2019/2022
            echo [INFO] 请安装 Visual Studio 2019 或 2022，并包含 C++ 开发工具
            pause
            exit /b 1
        )
    )
)

REM 创建构建目录
if not exist "%BUILD_DIR%" (
    echo [INFO] 创建构建目录: %BUILD_DIR%
    mkdir "%BUILD_DIR%"
)

REM 进入构建目录
cd /d "%BUILD_DIR%"

REM 配置项目 - 使用Visual Studio 2022生成器
echo [INFO] 配置 CMake 项目...
cmake .. -G "Visual Studio 17 2022" -A x64 -DCMAKE_BUILD_TYPE=%CMAKE_BUILD_TYPE%
if %errorlevel% neq 0 (
    echo [ERROR] CMake 配置失败
    pause
    exit /b 1
)

REM 构建项目
echo [INFO] 构建项目...
cmake --build . --config %CMAKE_BUILD_TYPE% --parallel
if %errorlevel% neq 0 (
    echo [ERROR] 构建失败
    pause
    exit /b 1
)

REM 检查输出文件
if exist "%BUILD_DIR%\bin\%CMAKE_BUILD_TYPE%\trt10.dll" (
    echo [SUCCESS] 构建成功！
    echo [INFO] 输出文件: %BUILD_DIR%\bin\%CMAKE_BUILD_TYPE%\trt10.dll
    
    REM 复制到项目根目录
    copy "%BUILD_DIR%\bin\%CMAKE_BUILD_TYPE%\trt10.dll" "%PROJECT_ROOT%\..\trt10.dll" >nul
    if %errorlevel% equ 0 (
        echo [INFO] 已复制 trt10.dll 到项目根目录
    )
) else (
    echo [ERROR] 未找到输出文件 trt10.dll
    pause
    exit /b 1
)

echo [INFO] Native 项目构建完成！
pause 