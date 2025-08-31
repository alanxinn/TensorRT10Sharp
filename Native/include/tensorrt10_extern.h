//
// Created by alanxinn on 25-8-23.
//

#ifndef TENSORRT10_EXTERN_H
#define TENSORRT10_EXTERN_H

#include <string>
#include <vector>

#ifdef _WIN32
#define TENSORRT_API __declspec(dllexport)
#else
#define TENSORRT_API __attribute__((visibility("default")))
#endif

// 维度结构体 - C兼容版本
struct TENSORRT_API Dims {
    int nbDims;
    int d[8];  // 支持最多8维
};

// C风格函数来创建Dims结构体（用于C兼容性）
extern "C" {
    TENSORRT_API Dims CreateDims();
}

// 推理引擎类
class TENSORRT_API Nvinfer {
public:
    Nvinfer();
    explicit Nvinfer(const char* modelPath);
    ~Nvinfer();
    
    // 获取绑定端口的形状信息
    Dims GetBindingDimensions(int index);
    Dims GetBindingDimensions(const char* nodeName);
    
    // 加载推理数据
    void LoadInferenceData(const char* nodeName, float* data, int dataSize);
    void LoadInferenceData(int nodeIndex, float* data, int dataSize);
    
    // 执行推理
    void Infer();
    
    // 获取推理结果
    float* GetInferenceResult(const char* nodeName, int* resultSize);
    float* GetInferenceResult(int nodeIndex, int* resultSize);
    
    // 释放结果内存
    void FreeResult(float* result);
    
    // 获取输入输出数量
    int GetInputCount();
    int GetOutputCount();
    
    // 获取输入输出名称
    const char* GetInputName(int index);
    const char* GetOutputName(int index);

private:
    class Impl;
    Impl* pImpl;
};

// ONNX转Engine函数
extern "C" {
    TENSORRT_API bool OnnxToEngine(const char* modelPath, int memorySize);
}

#endif //TENSORRT10_EXTERN_H
