@echo off
REM ==============================================
REM TensorRT10Sharp Native 清理脚本
REM ==============================================

echo [INFO] 清理 TensorRT10Sharp Native 项目...

REM 设置变量
set SCRIPT_DIR=%~dp0
set PROJECT_ROOT=%SCRIPT_DIR%..
set BUILD_DIR=%PROJECT_ROOT%\build

REM 删除构建目录
if exist "%BUILD_DIR%" (
    echo [INFO] 删除构建目录: %BUILD_DIR%
    rmdir /s /q "%BUILD_DIR%"
    if %errorlevel% equ 0 (
        echo [SUCCESS] 构建目录已删除
    ) else (
        echo [WARNING] 删除构建目录时出现问题
    )
) else (
    echo [INFO] 构建目录不存在，无需清理
)

REM 删除项目根目录的DLL文件
if exist "%PROJECT_ROOT%\..\trt10.dll" (
    echo [INFO] 删除项目根目录的 trt10.dll
    del "%PROJECT_ROOT%\..\trt10.dll"
    if %errorlevel% equ 0 (
        echo [SUCCESS] trt10.dll 已删除
    ) else (
        echo [WARNING] 删除 trt10.dll 时出现问题
    )
) else (
    echo [INFO] 项目根目录中没有 trt10.dll 文件
)

echo [INFO] Native 项目清理完成！
pause 