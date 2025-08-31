@echo off
REM ==============================================
REM TensorRT10Sharp 完整构建脚本
REM ==============================================

echo ================================================
echo    TensorRT10Sharp 完整构建脚本
echo ================================================
echo.

REM 设置变量
set SCRIPT_DIR=%~dp0
set NATIVE_DIR=%SCRIPT_DIR%Native
set MANAGED_DIR=%SCRIPT_DIR%Managed

echo [INFO] 开始构建 TensorRT10Sharp 完整项目...
echo.

REM 第一步：构建Native项目
echo [STEP 1/2] 构建 Native (C++) 项目...
echo ================================================
if exist "%NATIVE_DIR%\scripts\build.bat" (
    call "%NATIVE_DIR%\scripts\build.bat"
    if %errorlevel% neq 0 (
        echo [ERROR] Native 项目构建失败
        pause
        exit /b 1
    )
) else (
    echo [ERROR] Native 构建脚本不存在: %NATIVE_DIR%\scripts\build.bat
    pause
    exit /b 1
)

echo.
echo [SUCCESS] Native 项目构建完成！
echo.

REM 第二步：构建Managed项目
echo [STEP 2/2] 构建 Managed (C#) 项目...
echo ================================================
if exist "%MANAGED_DIR%\scripts\build.bat" (
    call "%MANAGED_DIR%\scripts\build.bat"
    if %errorlevel% neq 0 (
        echo [ERROR] Managed 项目构建失败
        pause
        exit /b 1
    )
) else (
    echo [ERROR] Managed 构建脚本不存在: %MANAGED_DIR%\scripts\build.bat
    pause
    exit /b 1
)

echo.
echo [SUCCESS] Managed 项目构建完成！
echo.

REM 构建完成
echo ================================================
echo    构建完成！
echo ================================================
echo [SUCCESS] TensorRT10Sharp 完整项目构建成功！
echo.
echo 输出文件:
echo   - Native DLL: %SCRIPT_DIR%trt10.dll
echo   - Managed EXE: %MANAGED_DIR%\bin\Release\net6.0\TensorRT10Sharp.exe
echo.
echo 运行示例:
echo   - 直接运行: %MANAGED_DIR%\scripts\run.bat
echo   - 或者: dotnet run --project %MANAGED_DIR%\TensorRT10Sharp.csproj
echo.
pause 