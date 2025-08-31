@echo off
echo ====================================
echo YOLO11 检测运行脚本
echo ====================================

:: 设置编码为UTF-8
chcp 65001 > nul

:: 切换到项目根目录
cd /d "%~dp0.."

set RELEASE_DIR=bin\Release\net6.0

:: 检查可执行文件
if not exist "%RELEASE_DIR%\Yolo11Sharp.exe" (
    echo 错误: 未找到可执行文件，请先构建项目
    echo 运行: Scripts\build.bat
    pause
    exit /b 1
)

:: 切换到输出目录
cd "%RELEASE_DIR%"

echo 当前目录: %CD%
echo.
echo 当前目录文件:
dir /b *.jpg *.png *.engine *.onnx *.names 2>nul

echo.
echo 开始检测...
echo ====================================

:: 运行检测程序
Yolo11Sharp.exe %*

echo.
echo ====================================
echo 检测完成！
echo.

:: 检查输出文件
if exist result.jpg (
    echo ✓ 结果图像已生成: result.jpg
    echo 请查看 result.jpg 验证检测结果
) else (
    echo ℹ 未找到默认结果图像，可能使用了自定义输出路径
)

if exist Output\ (
    echo.
    echo Output 目录内容:
    dir /b Output\*.jpg Output\*.png 2>nul
)

echo.
pause 