@echo off
REM ==============================================
REM TensorRT10Sharp 示例运行脚本
REM ==============================================

echo ================================================
echo    TensorRT10Sharp 示例运行脚本
echo ================================================
echo.

REM 设置变量
set SCRIPT_DIR=%~dp0
set MANAGED_DIR=%SCRIPT_DIR%Managed

echo [INFO] 运行 TensorRT10Sharp 示例程序...
echo.

REM 检查Managed项目是否存在
if not exist "%MANAGED_DIR%\scripts\run.bat" (
    echo [ERROR] Managed 运行脚本不存在: %MANAGED_DIR%\scripts\run.bat
    echo [INFO] 请确保项目结构完整
    pause
    exit /b 1
)

REM 检查是否已构建
if not exist "%SCRIPT_DIR%trt10.dll" (
    echo [INFO] 项目尚未构建，正在执行完整构建...
    call "%SCRIPT_DIR%build-all.bat"
    if %errorlevel% neq 0 (
        echo [ERROR] 构建失败，无法运行示例
        pause
        exit /b 1
    )
    echo.
)

REM 运行示例
echo [INFO] 启动示例程序...
echo ================================================
call "%MANAGED_DIR%\scripts\run.bat"

echo.
echo ================================================
echo    示例运行完成！
echo ================================================
echo.
echo 提示:
echo   - 要重新构建项目，请运行: build-all.bat
echo   - 要清理项目，请运行: clean-all.bat
echo   - 要查看更多选项，请查看 Managed\scripts\ 目录
echo.
pause 