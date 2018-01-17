#include "pch.h"
#include "OpenCvFuncs.h"
#include <ppltasks.h>
#include <concurrent_vector.h>

using namespace WRCOpenCvFuncs;
using namespace Platform;

using namespace concurrency;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;




namespace JQlocal {
	double DiffPix(Mat& img, int i1, int j1, int i2, int j2) // d_h(n,m) = x(n,m+1) - x(n,m) 
	{
		int r = img.at<Vec3b>(i1, j1)[0] - img.at<Vec3b>(i2, j2)[0];
		int g = img.at<Vec3b>(i1, j1)[1] - img.at<Vec3b>(i2, j2)[1];
		int b = img.at<Vec3b>(i1, j1)[2] - img.at<Vec3b>(i2, j2)[2];
		return 0,299*r + 0.587*g + 0.114*b; // перобразование в черно-белое

	}


	double DiffAver(vector<vector<double>> &v, int c1, int c2, int n, int m, double bi)
	{
		double sum = 0;

		for (int i = 0; i < n - 1; i++)
		{
			for (int j = 0; j < m - 1; j++)
				sum += fabs(v[i*c1][j*c2]) - bi;
		}

		return sum;
	}

	double TransZero(vector<vector<double>> &v, int n, int m)
	{
		double sum = 0;

		for (int i = 0; i < n - 1; i++)
		{
			for (int j = 0; j < m - 2; j++)
				if (v[i][j]==0) sum = 1;

		}
		return sum;
	}
}




OpenCvFuncs::OpenCvFuncs(Platform::String^ url)
{

	std::wstring fooW(url->Begin());
	std::string fooA(fooW.begin(), fooW.end());
	const char* charStr = fooA.c_str();

	image = imread(charStr);
}


double OpenCvFuncs::CalculateJQ()
{
	Mat img = image;
	using namespace JQlocal;

	const int alfa = -246, betta = 262;
	const double y1 = -0.024, y2 = 0.016, y3 = 0.064;

	int m = img.cols, n = img.rows;
	vector<vector<double>> d_h, d_v;
	d_h.resize(n);
	d_v.resize(n);

	// строим матрицы разностей соседних пикселей

	for (int i = 0; i < n - 1; i++)
	{
		d_h[i].resize(m);
		d_v[i].resize(m);
		for (int j = 0; j < m - 1; j++)
		{
			//по строке
			d_h[i][j] = DiffPix(img, i, j + 1, i, j);// d_h(n,m) = x(n,m+1) - x(n,m) 
													 //по столбцу
			d_v[i][j] = DiffPix(img, i + 1, j, i, j);// d_h(n,m) = x(n+1,m) - x(n,m) 
		}

	}

	// Определим среднюю разность между блоками 8х8

	double Bh = 1. / (n*(m / 8 - 1))*DiffAver(d_h, 1, 8, n, m / 8, 0);
	double Bv = 1. / (n*(m / 8 - 1))*DiffAver(d_v, 1, 8, n, m / 8, 0);
	double B = (Bh + Bv) / 2;


	// Оценим энергию сигнала изображения
	/*
	а) Первый фактор - отклонение средней абсолютной разности
	на границе блоков от той же величины, посчитанной для всего
	изображения
	*/

	double Ah = (1. / 7)*(8. / (n*(m - 1)))*DiffAver(d_h, 1, 1, n, m, Bh);
	double Av = (1. / 7)*(8. / (n*(m - 1)))*DiffAver(d_v, 1, 1, n, m, Bv);
	double A = (Ah + Av) / 2;

	// б) Второй фактор число переходов через нуль

	double Zh = 1. / (n*(m - 2))*TransZero(d_h, n, m);
	double Zv = 1. / (n*(m - 2))*TransZero(d_v, n, m);
	double Z = (Zh + Zv) / 2;



	//Eval.Quality = -(alfa + betta*pow(B, y1)*pow(A, y2)*pow(Z, y3));


	char* eval;

	//sprintf(eval, "%lf", -(alfa + betta*pow(B, y1)*pow(A, y2)*pow(Z, y3)));

	return -(alfa + betta*pow(fabs(B), y1)*pow(fabs(A), y2)*pow(fabs(Z), y3));
}


double OpenCvFuncs::GetJQ()
{
	return (CalculateJQ());
}


IplImage* OpenCvFuncs::GetIplImage(Mat image1)
{
	IplImage* image2;
	image2 = cvCreateImage(cvSize(image1.cols, image1.rows), 8, 3);
	IplImage ipltemp = image1;
	cvCopy(&ipltemp, image2);
	return image2;
}

Mat OpenCvFuncs::GetMat(IplImage* ipl)
{
	
	return cv::cvarrToMat(ipl);
}

double OpenCvFuncs::CalculateBlurAnyMat(const Mat img, const Platform::Array<double>^ Mask)
{
	Mat image = img;
	image.convertTo(image, CV_64F);
	normalize(image, image, 0, 1, cv::NORM_MINMAX);
	cv::Mat mean, stddev;

	double mask[9];
	for (int i = 0; i < 9; i++)	mask[i] = Mask[i];

	CvMat kernel_matrix = cvMat(3, 3, CV_32FC1, mask);

	IplImage* Image = GetIplImage(image);
	IplImage* dst = Image;
	// накладываем фильтр
	cvFilter2D(Image, dst, &kernel_matrix, cvPoint(-1, -1));
	cv::meanStdDev(GetMat(dst), mean, stddev);

	return stddev.at<double>(0);
}

double OpenCvFuncs::CalculateBlurLap(Mat image)
{
	Mat image_gray;
	cvtColor(image, image_gray, CV_BGR2GRAY);
	image.convertTo(image_gray, CV_64F);// convert to double
	normalize(image_gray, image_gray, 0, 1, cv::NORM_MINMAX);
	Mat dest;
	cv::Laplacian(image_gray, dest, image_gray.type());
	cv::Mat mean, stddev;
	cv::meanStdDev(image_gray, mean, stddev);


	auto res = stddev.at<double>(0);
	return res;
}

double OpenCvFuncs::GetBlur(const Platform::Array<double>^ CoeffMatrix)
{
	
	IplImage* image2 = cvCreateImage(cvSize(image.cols, image.rows), 8, 3);
	IplImage ipltemp = image;
	cvCopy(&ipltemp, image2);
	double blur[9];
	double blurSum=0;
	int width = image2->width / 3;
	int height = image2->height / 3;
	int k = 0;
	for (int i=1; i<=3; i++)
		for (int j = 1; j <= 3; j++)
		{
			cvSetImageROI(image2, CvRect((i - 1)*width, (j - 1)*height, i*width, j*height));
			IplImage *sub_img = cvCreateImage(cvGetSize(image2), image2->depth, image2->nChannels);
			// Копирование вырезанного объекта в созданный образ
			cvCopy(image2, sub_img, NULL);
			Mat matImg= cv::cvarrToMat(sub_img);
			blur[k] = CalculateBlurLap(matImg)*CoeffMatrix[k];
			blurSum += blur[k];
			k++;
			cvResetImageROI(image2);
		}
	
	return blurSum*100;
}

// -------------------------------------------------------
// Проверка на засвеченные области
// ------------------------------------------------------
bool isOverexposed(Mat& src, float threshold)
{
	Mat mask;
	inRange(src, Scalar::all(254), Scalar::all(255), mask);
	float total = src.cols*src.rows;
	float overexposed = countNonZero(mask);
	float ratio = overexposed / total;
	//cout << "overexposed percentage =" << ratio << endl;
	return (ratio>threshold);
}
