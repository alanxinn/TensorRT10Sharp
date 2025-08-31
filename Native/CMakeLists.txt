cmake_minimum_required(VERSION 3.10)

project(TensorRT10Sharp_Native CUDA CXX)

# 设置C++标准
set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# 编译定义
add_definitions(-std=c++14)
add_definitions(-DTENSORRT_API_EXPORTS)

# 设置CUDA编译器路径（根据实际安装路径调整）
set(CMAKE_CUDA_COMPILER "C:/Program Files/NVIDIA GPU Computing Toolkit/CUDA/v11.8/bin/nvcc.exe")

# 设置TensorRT和CUDA路径（根据实际安装路径调整）
set(TENSORRT_ROOT "C:/Program Files/NVIDIA GPU Computing Toolkit/TensorRT-10.13.0.35_cuda-11.8" CACHE PATH "TensorRT installation directory")
set(CUDA_ROOT "C:/Program Files/NVIDIA GPU Computing Toolkit/CUDA/v11.8" CACHE PATH "CUDA installation directory")

# 检查路径是否存在
if(NOT EXISTS ${TENSORRT_ROOT})
    message(WARNING "TensorRT path not found: ${TENSORRT_ROOT}")
endif()

if(NOT EXISTS ${CUDA_ROOT})
    message(WARNING "CUDA path not found: ${CUDA_ROOT}")
endif()

# 包含目录
include_directories(${PROJECT_SOURCE_DIR}/include)
include_directories(${CUDA_ROOT}/include)
include_directories(${TENSORRT_ROOT}/include)

# 链接目录
link_directories(${CUDA_ROOT}/lib/x64)
link_directories(${TENSORRT_ROOT}/lib)

# 查找源文件
file(GLOB_RECURSE CPP_SRCS ${PROJECT_SOURCE_DIR}/src/*.cpp)
file(GLOB_RECURSE CU_SRCS ${PROJECT_SOURCE_DIR}/src/*.cu)
file(GLOB_RECURSE HEADERS ${PROJECT_SOURCE_DIR}/include/*.h)

# 显示找到的文件
message(STATUS "Found C++ sources: ${CPP_SRCS}")
message(STATUS "Found CUDA sources: ${CU_SRCS}")
message(STATUS "Found headers: ${HEADERS}")

# 创建共享库
add_library(trt10 SHARED
    ${CPP_SRCS}
    ${CU_SRCS}
    ${HEADERS}
)

# 设置目标属性
set_target_properties(trt10 PROPERTIES
    POSITION_INDEPENDENT_CODE ON
    OUTPUT_NAME "trt10"
    PREFIX ""
    SUFFIX ".dll"
)

# 链接库
target_link_libraries(trt10
    # CUDA库
    cudart
    cuda
    
    # TensorRT库
    nvinfer_10
    nvonnxparser_10
    nvinfer_plugin_10
)

# 设置输出目录
set_target_properties(trt10 PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin
    LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib
    ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib
)

# 安装规则
install(TARGETS trt10
    RUNTIME DESTINATION bin
    LIBRARY DESTINATION lib
    ARCHIVE DESTINATION lib
)

install(FILES ${HEADERS}
    DESTINATION include
)

# 复制到项目根目录（用于C#项目）
add_custom_command(TARGET trt10 POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
    $<TARGET_FILE:trt10>
    ${CMAKE_SOURCE_DIR}/../trt10.dll
    COMMENT "Copying trt10.dll to project root"
) 