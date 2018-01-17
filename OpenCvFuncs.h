#pragma once

#include <opencv2\imgcodecs.hpp>
#include <opencv2\imgproc.hpp>
#include <vector>
#include <string>



#include <collection.h>
#include <ppl.h>
#include <amp.h>
#include <amp_math.h>

using namespace std;
using namespace cv;

namespace WRCOpenCvFuncs
{		
	public ref class OpenCvFuncs sealed
	{			
		cv::Mat image;	

		IplImage* GetIplImage(Mat);
		Mat GetMat(IplImage*);
		double CalculateJQ(void);
		double CalculateBlurLap(cv::Mat);
		double CalculateBlurAnyMat(const cv::Mat, const Platform::Array<double>^);


	public:
		OpenCvFuncs(Platform::String^);		

		double GetJQ(void);
		double GetBlur(const Platform::Array<double>^ CoeffMat);
	};
}
