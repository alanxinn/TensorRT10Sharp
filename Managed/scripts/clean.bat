@echo off
REM ==============================================
REM TensorRT10Sharp Managed 清理脚本
REM ==============================================

echo [INFO] 清理 TensorRT10Sharp Managed 项目...

REM 设置变量
set SCRIPT_DIR=%~dp0
set PROJECT_ROOT=%SCRIPT_DIR%..
set PROJECT_FILE=%PROJECT_ROOT%\TensorRT10Sharp.csproj

REM 检查.NET SDK是否安装
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK 未安装或未添加到PATH中
    echo [INFO] 请从 https://dotnet.microsoft.com/download 下载并安装.NET 6.0 SDK
    pause
    exit /b 1
)

REM 进入项目目录
cd /d "%PROJECT_ROOT%"

REM 使用dotnet clean清理项目
if exist "%PROJECT_FILE%" (
    echo [INFO] 清理项目构建输出...
    dotnet clean "%PROJECT_FILE%"
    if %errorlevel% equ 0 (
        echo [SUCCESS] 项目清理成功
    ) else (
        echo [WARNING] 项目清理时出现问题
    )
) else (
    echo [INFO] 项目文件不存在，跳过dotnet clean
)

REM 手动删除bin和obj目录
if exist "%PROJECT_ROOT%\bin" (
    echo [INFO] 删除 bin 目录...
    rmdir /s /q "%PROJECT_ROOT%\bin"
    if %errorlevel% equ 0 (
        echo [SUCCESS] bin 目录已删除
    ) else (
        echo [WARNING] 删除 bin 目录时出现问题
    )
) else (
    echo [INFO] bin 目录不存在
)

if exist "%PROJECT_ROOT%\obj" (
    echo [INFO] 删除 obj 目录...
    rmdir /s /q "%PROJECT_ROOT%\obj"
    if %errorlevel% equ 0 (
        echo [SUCCESS] obj 目录已删除
    ) else (
        echo [WARNING] 删除 obj 目录时出现问题
    )
) else (
    echo [INFO] obj 目录不存在
)

echo [INFO] Managed 项目清理完成！
pause 