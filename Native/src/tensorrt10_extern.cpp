//
// Created by alanxinn on 25-8-23.
//

#include "tensorrt10_extern.h"
#include <NvInfer.h>
#include <NvOnnxParser.h>
#include <fstream>
#include <iostream>
#include <memory>
#include <string>
#include <vector>

// 智能指针释放器
struct TRTDestroy {
    template <class T>
    void operator()(T* obj) const {
        if (obj) delete obj;
    }
};

template <class T>
using TRTUniquePtr = std::unique_ptr<T, TRTDestroy>;

// 日志记录器
class Logger : public nvinfer1::ILogger {
public:
    void log(Severity severity, const char* msg) noexcept override {
        if (severity != Severity::kINFO) {
            std::cout << msg << std::endl;
        }
    }
} gLogger;

// Nvinfer实现类
class Nvinfer::Impl {
public:
    TRTUniquePtr<nvinfer1::IRuntime> runtime;
    TRTUniquePtr<nvinfer1::ICudaEngine> engine;
    TRTUniquePtr<nvinfer1::IExecutionContext> context;
    std::vector<void*> buffers;
    std::vector<std::string> inputNames;
    std::vector<std::string> outputNames;
    std::vector<int> inputIndices;
    std::vector<int> outputIndices;
    int inputCount = 0;
    int outputCount = 0;
    cudaStream_t stream = nullptr;
    
    Impl() {
        runtime.reset(nvinfer1::createInferRuntime(gLogger));
    }
    
    ~Impl() {
        if (stream) {
            cudaStreamDestroy(stream);
        }
        for (void* buffer : buffers) {
            if (buffer) {
                cudaFree(buffer);
            }
        }
    }
    
    bool LoadEngine(const char* modelPath) {
        std::ifstream file(modelPath, std::ios::binary);
        if (!file.good()) {
            return false;
        }
        
        file.seekg(0, std::ios::end);
        size_t size = file.tellg();
        file.seekg(0, std::ios::beg);
        
        std::vector<char> engineData(size);
        file.read(engineData.data(), size);
        file.close();
        
        engine.reset(runtime->deserializeCudaEngine(engineData.data(), size));
        if (!engine) {
            return false;
        }
        
        context.reset(engine->createExecutionContext());
        if (!context) {
            return false;
        }
        
        // 创建CUDA流
        cudaStreamCreate(&stream);
        
        // 分析输入输出
        AnalyzeBindings();
        
        // 分配GPU内存
        AllocateBuffers();
        
        return true;
    }
    
private:
    void AnalyzeBindings() {
        // 获取绑定数量
        int numBindings = engine->getNbIOTensors();
        inputCount = 0;
        outputCount = 0;
        
        for (int i = 0; i < numBindings; i++) {
            std::string name = engine->getIOTensorName(i);
            if (engine->getTensorIOMode(name.c_str()) == nvinfer1::TensorIOMode::kINPUT) {
                inputNames.push_back(name);
                inputIndices.push_back(i);
                inputCount++;
            } else {
                outputNames.push_back(name);
                outputIndices.push_back(i);
                outputCount++;
            }
        }
    }
    
    void AllocateBuffers() {
        int numBindings = engine->getNbIOTensors();
        buffers.resize(numBindings, nullptr);
        
        for (int i = 0; i < numBindings; i++) {
            std::string name = engine->getIOTensorName(i);
            nvinfer1::Dims dims = engine->getTensorShape(name.c_str());
            size_t size = 1;
            for (int j = 0; j < dims.nbDims; j++) {
                size *= dims.d[j];
            }
            size *= sizeof(float);
            
            cudaMalloc(&buffers[i], size);
        }
    }
};

// Nvinfer构造函数
Nvinfer::Nvinfer() : pImpl(new Impl()) {}

Nvinfer::Nvinfer(const char* modelPath) : pImpl(new Impl()) {
    if (!pImpl->LoadEngine(modelPath)) {
        throw std::runtime_error("Failed to load TensorRT engine");
    }
}

Nvinfer::~Nvinfer() {
    delete pImpl;
}

// 获取绑定维度
Dims Nvinfer::GetBindingDimensions(int index) {
    Dims result = CreateDims();
    if (index >= 0 && index < pImpl->engine->getNbIOTensors()) {
        std::string name = pImpl->engine->getIOTensorName(index);
        nvinfer1::Dims dims = pImpl->engine->getTensorShape(name.c_str());
        result.nbDims = dims.nbDims;
        for (int i = 0; i < dims.nbDims && i < 8; i++) {
            result.d[i] = dims.d[i];
        }
    }
    return result;
}

Dims Nvinfer::GetBindingDimensions(const char* nodeName) {
    Dims result = CreateDims();
    nvinfer1::Dims dims = pImpl->engine->getTensorShape(nodeName);
    if (dims.nbDims > 0) {
        result.nbDims = dims.nbDims;
        for (int i = 0; i < dims.nbDims && i < 8; i++) {
            result.d[i] = dims.d[i];
        }
    }
    return result;
}

// 加载推理数据
void Nvinfer::LoadInferenceData(const char* nodeName, float* data, int dataSize) {
    // 通过名称查找索引
    for (int i = 0; i < pImpl->engine->getNbIOTensors(); i++) {
        std::string name = pImpl->engine->getIOTensorName(i);
        if (name == nodeName) {
            LoadInferenceData(i, data, dataSize);
            break;
        }
    }
}

void Nvinfer::LoadInferenceData(int nodeIndex, float* data, int dataSize) {
    if (nodeIndex >= 0 && nodeIndex < pImpl->buffers.size()) {
        cudaMemcpyAsync(pImpl->buffers[nodeIndex], data, dataSize * sizeof(float), 
                       cudaMemcpyHostToDevice, pImpl->stream);
    }
}

// 执行推理
void Nvinfer::Infer() {
    if (pImpl->context) {
        pImpl->context->executeV2(pImpl->buffers.data());
    }
}

// 获取推理结果
float* Nvinfer::GetInferenceResult(const char* nodeName, int* resultSize) {
    // 通过名称查找索引
    for (int i = 0; i < pImpl->engine->getNbIOTensors(); i++) {
        std::string name = pImpl->engine->getIOTensorName(i);
        if (name == nodeName) {
            return GetInferenceResult(i, resultSize);
        }
    }
    *resultSize = 0;
    return nullptr;
}

float* Nvinfer::GetInferenceResult(int nodeIndex, int* resultSize) {
    if (nodeIndex < 0 || nodeIndex >= pImpl->buffers.size()) {
        *resultSize = 0;
        return nullptr;
    }
    
    std::string name = pImpl->engine->getIOTensorName(nodeIndex);
    nvinfer1::Dims dims = pImpl->engine->getTensorShape(name.c_str());
    size_t size = 1;
    for (int i = 0; i < dims.nbDims; i++) {
        size *= dims.d[i];
    }
    
    float* result = new float[size];
    cudaMemcpyAsync(result, pImpl->buffers[nodeIndex], size * sizeof(float), 
                   cudaMemcpyDeviceToHost, pImpl->stream);
    cudaStreamSynchronize(pImpl->stream);
    
    *resultSize = static_cast<int>(size);
    return result;
}

// 释放结果内存
void Nvinfer::FreeResult(float* result) {
    if (result) {
        delete[] result;
    }
}

// 获取输入输出数量
int Nvinfer::GetInputCount() {
    return pImpl->inputCount;
}

int Nvinfer::GetOutputCount() {
    return pImpl->outputCount;
}

// 获取输入输出名称
const char* Nvinfer::GetInputName(int index) {
    if (index >= 0 && index < pImpl->inputNames.size()) {
        return pImpl->inputNames[index].c_str();
    }
    return nullptr;
}

const char* Nvinfer::GetOutputName(int index) {
    if (index >= 0 && index < pImpl->outputNames.size()) {
        return pImpl->outputNames[index].c_str();
    }
    return nullptr;
}

// ONNX转Engine实现
bool OnnxToEngine(const char* modelPath, int memorySize) {
    TRTUniquePtr<nvinfer1::IBuilder> builder(nvinfer1::createInferBuilder(gLogger));
    if (!builder) {
        return false;
    }
    
    const auto explicitBatch = 1U << static_cast<uint32_t>(nvinfer1::NetworkDefinitionCreationFlag::kEXPLICIT_BATCH);
    TRTUniquePtr<nvinfer1::INetworkDefinition> network(builder->createNetworkV2(explicitBatch));
    if (!network) {
        return false;
    }
    
    TRTUniquePtr<nvonnxparser::IParser> parser(nvonnxparser::createParser(*network, gLogger));
    if (!parser) {
        return false;
    }
    
    if (!parser->parseFromFile(modelPath, static_cast<int>(nvinfer1::ILogger::Severity::kWARNING))) {
        return false;
    }
    
    TRTUniquePtr<nvinfer1::IBuilderConfig> config(builder->createBuilderConfig());
    if (!config) {
        return false;
    }
    
    config->setMemoryPoolLimit(nvinfer1::MemoryPoolType::kWORKSPACE, static_cast<size_t>(memorySize) * 1024 * 1024);
    
    if (builder->platformHasFastFp16()) {
        config->setFlag(nvinfer1::BuilderFlag::kFP16);
    }
    
    TRTUniquePtr<nvinfer1::ICudaEngine> engine(builder->buildEngineWithConfig(*network, *config));
    if (!engine) {
        return false;
    }
    
    TRTUniquePtr<nvinfer1::IHostMemory> serializedEngine(engine->serialize());
    if (!serializedEngine) {
        return false;
    }
    
    std::string outputPath = std::string(modelPath);
    size_t pos = outputPath.find_last_of('.');
    if (pos != std::string::npos) {
        outputPath = outputPath.substr(0, pos);
    }
    outputPath += ".engine";
    
    std::ofstream file(outputPath, std::ios::binary);
    if (!file) {
        return false;
    }
    
    file.write(static_cast<const char*>(serializedEngine->data()), serializedEngine->size());
    file.close();
    
    return true;
}

// C风格导出函数
extern "C" {
    // Dims创建函数（用于C兼容性）
    TENSORRT_API Dims CreateDims() {
        Dims dims;
        dims.nbDims = 0;
        for (int i = 0; i < 8; i++) {
            dims.d[i] = 0;
        }
        return dims;
    }
    
    // Nvinfer函数
    TENSORRT_API Nvinfer* Nvinfer_Create() {
        try {
            return new Nvinfer();
        } catch (...) {
            return nullptr;
        }
    }
    
    TENSORRT_API Nvinfer* Nvinfer_CreateWithModel(const char* modelPath) {
        try {
            return new Nvinfer(modelPath);
        } catch (...) {
            return nullptr;
        }
    }
    
    TENSORRT_API void Nvinfer_Destroy(Nvinfer* ptr) {
        if (ptr) {
            delete ptr;
        }
    }
    
    TENSORRT_API Dims Nvinfer_GetBindingDimensions(Nvinfer* ptr, int index) {
        if (ptr) {
            return ptr->GetBindingDimensions(index);
        }
        return CreateDims();
    }
    
    TENSORRT_API Dims Nvinfer_GetBindingDimensionsByName(Nvinfer* ptr, const char* nodeName) {
        if (ptr && nodeName) {
            return ptr->GetBindingDimensions(nodeName);
        }
        return CreateDims();
    }
    
    TENSORRT_API void Nvinfer_LoadInferenceDataByName(Nvinfer* ptr, const char* nodeName, float* data, int dataSize) {
        if (ptr && nodeName && data) {
            ptr->LoadInferenceData(nodeName, data, dataSize);
        }
    }
    
    TENSORRT_API void Nvinfer_LoadInferenceData(Nvinfer* ptr, int nodeIndex, float* data, int dataSize) {
        if (ptr && data) {
            ptr->LoadInferenceData(nodeIndex, data, dataSize);
        }
    }
    
    TENSORRT_API void Nvinfer_Infer(Nvinfer* ptr) {
        if (ptr) {
            ptr->Infer();
        }
    }
    
    TENSORRT_API float* Nvinfer_GetInferenceResultByName(Nvinfer* ptr, const char* nodeName, int* resultSize) {
        if (ptr && nodeName && resultSize) {
            return ptr->GetInferenceResult(nodeName, resultSize);
        }
        if (resultSize) {
            *resultSize = 0;
        }
        return nullptr;
    }
    
    TENSORRT_API float* Nvinfer_GetInferenceResult(Nvinfer* ptr, int nodeIndex, int* resultSize) {
        if (ptr && resultSize) {
            return ptr->GetInferenceResult(nodeIndex, resultSize);
        }
        if (resultSize) {
            *resultSize = 0;
        }
        return nullptr;
    }
    
    TENSORRT_API void Nvinfer_FreeResult(float* result) {
        if (result) {
            delete[] result;
        }
    }
    
    TENSORRT_API int Nvinfer_GetInputCount(Nvinfer* ptr) {
        if (ptr) {
            return ptr->GetInputCount();
        }
        return 0;
    }
    
    TENSORRT_API int Nvinfer_GetOutputCount(Nvinfer* ptr) {
        if (ptr) {
            return ptr->GetOutputCount();
        }
        return 0;
    }
    
    TENSORRT_API const char* Nvinfer_GetInputName(Nvinfer* ptr, int index) {
        if (ptr) {
            return ptr->GetInputName(index);
        }
        return nullptr;
    }
    
    TENSORRT_API const char* Nvinfer_GetOutputName(Nvinfer* ptr, int index) {
        if (ptr) {
            return ptr->GetOutputName(index);
        }
        return nullptr;
    }
}

