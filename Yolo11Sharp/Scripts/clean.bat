@echo off
echo ====================================
echo YOLO11 项目清理脚本
echo ====================================

:: 设置编码为UTF-8
chcp 65001 > nul

:: 切换到项目根目录
cd /d "%~dp0.."

echo 清理构建输出...

:: 清理构建目录
if exist "bin" (
    echo 删除 bin 目录...
    rmdir /s /q "bin"
)

if exist "obj" (
    echo 删除 obj 目录...
    rmdir /s /q "obj"
)

:: 清理输出文件
echo 清理输出文件...
del /q "*.jpg" 2>nul
del /q "*.png" 2>nul
del /q "result.*" 2>nul

:: 清理日志文件
if exist "*.log" (
    echo 删除日志文件...
    del /q "*.log"
)

:: 清理临时文件
if exist "*.tmp" (
    echo 删除临时文件...
    del /q "*.tmp"
)

echo.
echo ====================================
echo 清理完成！
echo ====================================
echo.
echo 已清理的内容:
echo   - 构建输出目录 (bin, obj)
echo   - 输出图像文件
echo   - 日志和临时文件
echo.

pause 