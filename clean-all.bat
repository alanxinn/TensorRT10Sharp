@echo off
REM ==============================================
REM TensorRT10Sharp 完整清理脚本
REM ==============================================

echo ================================================
echo    TensorRT10Sharp 完整清理脚本
echo ================================================
echo.

REM 设置变量
set SCRIPT_DIR=%~dp0
set NATIVE_DIR=%SCRIPT_DIR%Native
set MANAGED_DIR=%SCRIPT_DIR%Managed

echo [INFO] 开始清理 TensorRT10Sharp 完整项目...
echo.

REM 第一步：清理Native项目
echo [STEP 1/2] 清理 Native (C++) 项目...
echo ================================================
if exist "%NATIVE_DIR%\scripts\clean.bat" (
    call "%NATIVE_DIR%\scripts\clean.bat"
    if %errorlevel% neq 0 (
        echo [WARNING] Native 项目清理时出现问题，继续执行...
    )
) else (
    echo [INFO] Native 清理脚本不存在，跳过
)

echo.

REM 第二步：清理Managed项目
echo [STEP 2/2] 清理 Managed (C#) 项目...
echo ================================================
if exist "%MANAGED_DIR%\scripts\clean.bat" (
    call "%MANAGED_DIR%\scripts\clean.bat"
    if %errorlevel% neq 0 (
        echo [WARNING] Managed 项目清理时出现问题，继续执行...
    )
) else (
    echo [INFO] Managed 清理脚本不存在，跳过
)

echo.

REM 清理项目根目录的文件
echo [INFO] 清理项目根目录...
if exist "%SCRIPT_DIR%trt10.dll" (
    echo [INFO] 删除 trt10.dll
    del "%SCRIPT_DIR%trt10.dll"
    if %errorlevel% equ 0 (
        echo [SUCCESS] trt10.dll 已删除
    ) else (
        echo [WARNING] 删除 trt10.dll 时出现问题
    )
) else (
    echo [INFO] trt10.dll 不存在
)

REM 清理旧的目录结构（如果存在）
if exist "%SCRIPT_DIR%Core" (
    echo [INFO] 删除旧的 Core 目录...
    rmdir /s /q "%SCRIPT_DIR%Core"
    if %errorlevel% equ 0 (
        echo [SUCCESS] Core 目录已删除
    )
)

if exist "%SCRIPT_DIR%Examples" (
    echo [INFO] 删除旧的 Examples 目录...
    rmdir /s /q "%SCRIPT_DIR%Examples"
    if %errorlevel% equ 0 (
        echo [SUCCESS] Examples 目录已删除
    )
)

if exist "%SCRIPT_DIR%Scripts" (
    echo [INFO] 删除旧的 Scripts 目录...
    rmdir /s /q "%SCRIPT_DIR%Scripts"
    if %errorlevel% equ 0 (
        echo [SUCCESS] Scripts 目录已删除
    )
)

if exist "%SCRIPT_DIR%bin" (
    echo [INFO] 删除根目录的 bin 目录...
    rmdir /s /q "%SCRIPT_DIR%bin"
    if %errorlevel% equ 0 (
        echo [SUCCESS] bin 目录已删除
    )
)

if exist "%SCRIPT_DIR%obj" (
    echo [INFO] 删除根目录的 obj 目录...
    rmdir /s /q "%SCRIPT_DIR%obj"
    if %errorlevel% equ 0 (
        echo [SUCCESS] obj 目录已删除
    )
)

if exist "%SCRIPT_DIR%cmake-build-debug" (
    echo [INFO] 删除 cmake-build-debug 目录...
    rmdir /s /q "%SCRIPT_DIR%cmake-build-debug"
    if %errorlevel% equ 0 (
        echo [SUCCESS] cmake-build-debug 目录已删除
    )
)

echo.
echo ================================================
echo    清理完成！
echo ================================================
echo [SUCCESS] TensorRT10Sharp 完整项目清理完成！
echo.
pause 