#include "pch.h"'
#include "OnnxDetectManager.h"
#include <optional>

OrtCppBase::OnnxDetectManager::OnnxDetectManager() {
    m_bUsingGPU = false;
    m_fConfThreshold = 0.0;
    m_fIoUThreshold = 0.0;
    m_strLabelFilePath = "";
    m_wstrOnnxFilePath = L"";
    m_vecLabels = this->readLabels(m_strLabelFilePath);
}

OrtCppBase::OnnxDetectManager::~OnnxDetectManager() {

}

void OrtCppBase::OnnxDetectManager::setSessionOption() {
    
    m_sessionOptions.SetIntraOpNumThreads(1);

    if (m_bUsingGPU)
    {
        OrtCUDAProviderOptions cuda_options{};
        cuda_options.device_id = 0;
        m_sessionOptions.AppendExecutionProvider_CUDA(cuda_options);
    }

    // Sets graph optimization level
    // Available levels are
    // ORT_DISABLE_ALL -> To disable all optimizations
    // ORT_ENABLE_BASIC -> To enable basic optimizations (Such as redundant node
    // removals) ORT_ENABLE_EXTENDED -> To enable extended optimizations
    // (Includes level 1 + more complex optimizations like node fusions)
    // ORT_ENABLE_ALL -> To Enable All possible optimizations
    m_sessionOptions.SetGraphOptimizationLevel(GraphOptimizationLevel::ORT_ENABLE_EXTENDED);

}

std::string OrtCppBase::OnnxDetectManager::getExePath()
{
  /*  char buffer[MAX_PATH];
    GetModuleFileNameA(NULL, buffer, MAX_PATH);
    std::string::size_type pos = std::string(buffer).find_last_of("\\/");

    return std::string(buffer).substr(0, pos);*/
    return std::string();
}

template <typename T>
T clip(const T& n, const T& lower, const T& upper)
{
	return std::max(lower, std::min(n, upper));
}

template <typename T>
T vectorProduct(const std::vector<T>& v)
{
    return accumulate(v.begin(), v.end(), 1, std::multiplies<T>());
}

std::vector<std::string> OrtCppBase::OnnxDetectManager::readLabels(std::string& labelFilepath)
{
    std::vector<std::string> labels;
    std::string line;
    std::ifstream fp(labelFilepath);
    while (std::getline(fp, line))
    {
        labels.push_back(line);
    }
    return labels;
}

cv::Rect2f OrtCppBase::OnnxDetectManager::scaleCoords(const cv::Size& imageShape, cv::Rect2f coords, const cv::Size& imageOriginalShape, bool p_Clip)
{
    cv::Rect2f l_Result;
    float gain = std::min((float)imageShape.height / (float)imageOriginalShape.height,
        (float)imageShape.width / (float)imageOriginalShape.width);

    int pad[2] = { (int)std::round((((float)imageShape.width - (float)imageOriginalShape.width * gain) / 2.0f) - 0.1f),
                 (int)std::round((((float)imageShape.height - (float)imageOriginalShape.height * gain) / 2.0f) - 0.1f) };

    l_Result.x = (int)std::round(((float)(coords.x - pad[0]) / gain));
    l_Result.y = (int)std::round(((float)(coords.y - pad[1]) / gain));

    l_Result.width = (int)std::round(((float)coords.width / gain));
    l_Result.height = (int)std::round(((float)coords.height / gain));

    // // clip coords, should be modified for width and height
    if (p_Clip)
    {
        l_Result.x = clip(l_Result.x, (float)0, (float)imageOriginalShape.width);
        l_Result.y = clip(l_Result.y, (float)0, (float)imageOriginalShape.height);
        l_Result.width = clip(l_Result.width, (float)0, (float)(imageOriginalShape.width - l_Result.x));
        l_Result.height = clip(l_Result.height, (float)0, (float)(imageOriginalShape.height - l_Result.y));
    }
    return l_Result;
}

void OrtCppBase::OnnxDetectManager::getBestClassInfo(const cv::Mat& p_Mat, const int& numClasses, float& bestConf, int& bestClassId)
{
    bestClassId = 0;
    bestConf = 0;

    if (p_Mat.rows && p_Mat.cols)
    {
        for (int i = 0; i < numClasses; i++)
        {
            if (p_Mat.at<float>(0, i + 4) > bestConf)
            {
                bestConf = p_Mat.at<float>(0, i + 4);
                bestClassId = i;
            }
        }
    }
}

std::vector<Detection> OrtCppBase::OnnxDetectManager::postprocessing(const cv::Size& resizedImageShape, const cv::Size& originalImageShape, std::vector<Ort::Value>& outputTensors, const float& confThreshold, const float& iouThreshold)
{
    std::vector<cv::Rect> boxes;
    std::vector<cv::Rect> nms_boxes;
    std::vector<float> confs;
    std::vector<int> classIds;

    auto* rawOutput = outputTensors[0].GetTensorData<float>();
    std::vector<int64_t> outputShape = outputTensors[0].GetTensorTypeAndShapeInfo().GetShape();
    size_t count = outputTensors[0].GetTensorTypeAndShapeInfo().GetElementCount();

    cv::Mat l_Mat = cv::Mat(outputShape[1], outputShape[2], CV_32FC1, (void*)rawOutput);
    cv::Mat l_Mat_t = l_Mat.t();
    //std::vector<float> output(rawOutput, rawOutput + count);

    // for (const int64_t& shape : outputShape)
    //     std::cout << "Output Shape: " << shape << std::endl;

    // first 5 elements are box[4] and obj confidence
    int numClasses = l_Mat_t.cols - 4;

    // only for batch size = 1

    for (int l_Row = 0; l_Row < l_Mat_t.rows; l_Row++)
    {
        cv::Mat l_MatRow = l_Mat_t.row(l_Row);
        float objConf;
        int classId;

        getBestClassInfo(l_MatRow, numClasses, objConf, classId);

        if (objConf > confThreshold)
        {
            float centerX = (l_MatRow.at<float>(0, 0));
            float centerY = (l_MatRow.at<float>(0, 1));
            float width = (l_MatRow.at<float>(0, 2));
            float height = (l_MatRow.at<float>(0, 3));
            float left = centerX - width / 2;
            float top = centerY - height / 2;

            float confidence = objConf;
            cv::Rect2f l_Scaled = scaleCoords(resizedImageShape, cv::Rect2f(left, top, width, height), originalImageShape, true);

            // Prepare NMS filtered per class id's
            nms_boxes.emplace_back((int)std::round(l_Scaled.x) + classId * 7680, (int)std::round(l_Scaled.y) + classId * 7680,
                (int)std::round(l_Scaled.width), (int)std::round(l_Scaled.height));
            boxes.emplace_back((int)std::round(l_Scaled.x), (int)std::round(l_Scaled.y),
                (int)std::round(l_Scaled.width), (int)std::round(l_Scaled.height));
            confs.emplace_back(confidence);
            classIds.emplace_back(classId);
        }
    }

    std::vector<int> indices;
    cv::dnn::NMSBoxes(nms_boxes, confs, confThreshold, iouThreshold, indices);
    // std::cout << "amount of NMS indices: " << indices.size() << std::endl;

    std::vector<Detection> detections;

    for (int idx : indices)
    {
        Detection det;
        det.box = boxes[idx];
        det.conf = confs[idx];
        det.classId = classIds[idx];
        detections.emplace_back(det);
    }

    return detections;
}

void OrtCppBase::OnnxDetectManager::letterbox(const cv::Mat& image, cv::Mat& outImage, const cv::Size& newShape, const cv::Scalar& color, bool auto_, bool scaleFill, bool scaleUp, int stride)
{
    cv::Size shape = image.size();
    float r = std::min((float)newShape.height / (float)shape.height,
        (float)newShape.width / (float)shape.width);
    if (!scaleUp)
        r = std::min(r, 1.0f);

    float ratio[2]{ r, r };
    int newUnpad[2]{ (int)std::round((float)shape.width * r),
                     (int)std::round((float)shape.height * r) };

    auto dw = (float)(newShape.width - newUnpad[0]);
    auto dh = (float)(newShape.height - newUnpad[1]);

    if (auto_)
    {
        dw = (float)((int)dw % stride);
        dh = (float)((int)dh % stride);
    }
    else if (scaleFill)
    {
        dw = 0.0f;
        dh = 0.0f;
        newUnpad[0] = newShape.width;
        newUnpad[1] = newShape.height;
        ratio[0] = (float)newShape.width / (float)shape.width;
        ratio[1] = (float)newShape.height / (float)shape.height;
    }

    dw /= 2.0f;
    dh /= 2.0f;

    if (shape.width != newUnpad[0] || shape.height != newUnpad[1])
    {
        cv::resize(image, outImage, cv::Size(newUnpad[0], newUnpad[1]));
    }

    int top = int(std::round(dh - 0.1f));
    int bottom = int(std::round(dh + 0.1f));
    int left = int(std::round(dw - 0.1f));
    int right = int(std::round(dw + 0.1f));
    cv::copyMakeBorder(outImage, outImage, top, bottom, left, right, cv::BORDER_CONSTANT, color);
}

int OrtCppBase::OnnxDetectManager::calculate_product(const std::vector<int64_t>& v)
{
    int total = 1;
    for (auto& i : v)
        total *= i;
    return total;
}

std::string OnnxDetectManager::test()
{
	return "Onnx Detect Manager";
}

void OrtCppBase::OnnxDetectManager::Inference(cv::Mat* pMat)
{
    if (pMat == NULL)
        return;

    Ort::AllocatorWithDefaultOptions ort_alloc;

    size_t numInputNodes = m_session.GetInputCount();
    size_t numOutputNodes = m_session.GetOutputCount();

    // Input
    Ort::AllocatedStringPtr prtStringIn = m_session.GetInputNameAllocated(0, ort_alloc);
    char inputName[256];
    sprintf_s(inputName, "%s", prtStringIn.get());

    Ort::TypeInfo inputTypeInfo = m_session.GetInputTypeInfo(0);
    auto inputTensorInfo = inputTypeInfo.GetTensorTypeAndShapeInfo();

    ONNXTensorElementDataType inputType = inputTensorInfo.GetElementType();

    std::vector<int64_t> inputDims = inputTensorInfo.GetShape();
    if (inputDims.at(0) == -1)
    {
        std::cout << "Got dynamic batch size. Setting input batch size to "
            << BATCHSIZE << "." << std::endl;
        inputDims.at(0) = BATCHSIZE;
    }

    // Output
    Ort::AllocatedStringPtr prtStringOut = m_session.GetOutputNameAllocated(0, ort_alloc);
    char outputName[256];
    sprintf_s(outputName, "%s", prtStringOut.get());

    Ort::TypeInfo outputTypeInfo = m_session.GetOutputTypeInfo(0);
    auto outputTensorInfo = outputTypeInfo.GetTensorTypeAndShapeInfo();

    ONNXTensorElementDataType outputType = outputTensorInfo.GetElementType();

    std::vector<int64_t> outputDims = outputTensorInfo.GetShape();
    if (outputDims.at(0) == -1)
    {
        std::cout << "Got dynamic batch size. Setting output batch size to "
            << BATCHSIZE << "." << std::endl;
        outputDims.at(0) = BATCHSIZE;
    }

    cv::Mat resizedImage, floatImage;
    letterbox(*pMat, resizedImage, cv::Size(640, 640),
        cv::Scalar(114, 114, 114), false,
        false, true, 32);
    resizedImage.convertTo(floatImage, CV_32FC3, 1 / 255.0);
    float* blob = new float[floatImage.cols * floatImage.rows * floatImage.channels()];
    cv::Size floatImageSize{ floatImage.cols, floatImage.rows };
    std::vector<cv::Mat> chw(floatImage.channels());
    for (int i = 0; i < floatImage.channels(); ++i)
    {
        chw[i] = cv::Mat(floatImageSize, CV_32FC1, blob + i * floatImageSize.width * floatImageSize.height);
    }
    cv::split(floatImage, chw);

    std::vector<float> inputTensorValues(blob, blob + 3 * floatImageSize.width * floatImageSize.height);
    size_t inputTensorSize = vectorProduct(inputDims);
    size_t outputTensorSize = vectorProduct(outputDims);
    std::vector<float> outputTensorValues(outputTensorSize);

    std::vector<const char*> inputNames{ inputName };
    std::vector<const char*> outputNames{ outputName };
    std::vector<Ort::Value> inputTensors;
    std::vector<Ort::Value> outputTensors;

    Ort::MemoryInfo memoryInfo = Ort::MemoryInfo::CreateCpu(
        OrtAllocatorType::OrtArenaAllocator, OrtMemType::OrtMemTypeDefault);
    inputTensors.push_back(Ort::Value::CreateTensor<float>(
        memoryInfo, inputTensorValues.data(), inputTensorSize, inputDims.data(),
        inputDims.size()));
    outputTensors.push_back(Ort::Value::CreateTensor<float>(
        memoryInfo, outputTensorValues.data(), outputTensorSize,
        outputDims.data(), outputDims.size()));

    clock_t time_req;

    time_req = clock();

    m_session.Run(Ort::RunOptions{ nullptr }, inputNames.data(),
        inputTensors.data(), 1 /*Number of inputs*/, outputNames.data(),
        outputTensors.data(), 1 /*Number of outputs*/);

    m_dOverallTime = 1000.0 * double(clock() - time_req) / double(CLOCKS_PER_SEC);

    std::vector<Detection> result = postprocessing(cv::Size(640, 640), (*pMat).size(), outputTensors, m_fConfThreshold, m_fIoUThreshold);
    

    m_classIds.clear();
    m_boxes.clear();
    m_scores.clear();

    for (auto detection : result)
    {
        m_classIds.push_back(detection.classId);
        m_boxes.push_back(detection.box);
        m_scores.push_back(detection.conf);
    }

    // release memory
    delete[] blob;
    blob = NULL;
}

#pragma region Setter, Getter
void OrtCppBase::OnnxDetectManager::setLabelFielPath(std::string labelFilePath) {
    if (!labelFilePath.empty())
        m_strLabelFilePath = labelFilePath;
    m_vecLabels = readLabels(m_strLabelFilePath);
}
std::string OrtCppBase::OnnxDetectManager::getLabelFilePath() {
    return m_strLabelFilePath;
}
void OrtCppBase::OnnxDetectManager::setOnnxFilePath(std::wstring onnxFilePath) {
    if (!onnxFilePath.empty())
    {
        m_wstrOnnxFilePath = onnxFilePath;

        m_env = Ort::Env(OrtLoggingLevel::ORT_LOGGING_LEVEL_WARNING, INSTANCENAME.c_str());
        m_session = Ort::Session(m_env, m_wstrOnnxFilePath.c_str(), m_sessionOptions);
    }
}
std::wstring OrtCppBase::OnnxDetectManager::getOnnxFilePath() {
    return m_wstrOnnxFilePath;
}
void OrtCppBase::OnnxDetectManager::setUsingGPU(bool b) {
    m_bUsingGPU = b;
}
bool OrtCppBase::OnnxDetectManager::getUsingGPU() {
    return m_bUsingGPU;
}
void OrtCppBase::OnnxDetectManager::setConfThreshold(float conf) {
    m_fConfThreshold = conf;
}
float OrtCppBase::OnnxDetectManager::getConfThreshold() {
    return m_fConfThreshold;
}
void OrtCppBase::OnnxDetectManager::setIoUThreshold(float iou) {
    m_fIoUThreshold = iou;
}
float OrtCppBase::OnnxDetectManager::getIoUThreshold() {
    return m_fIoUThreshold;
}

double OnnxDetectManager::getDetectionResults(std::vector<int>& classIds, std::vector<float>& scores, std::vector<cv::Rect>& boxes)
{
    classIds = m_classIds;
    scores = m_scores;
    boxes = m_boxes;

    return m_dOverallTime;
}
#pragma endregion
