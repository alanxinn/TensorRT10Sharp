using System;
using System.Runtime.InteropServices;

namespace TensorRTSharp
{
    /// <summary>
    /// 维度结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Dims
    {
        public int nbDims;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public int[] d;
        
        public Dims()
        {
            nbDims = 0;
            d = new int[8];
        }
        
        /// <summary>
        /// 获取指定维度的值
        /// </summary>
        /// <param name="index">维度索引</param>
        /// <returns>维度值</returns>
        public int GetDimension(int index)
        {
            if (index >= 0 && index < nbDims && index < 8)
            {
                return d[index];
            }
            return 0;
        }
        
        /// <summary>
        /// 设置指定维度的值
        /// </summary>
        /// <param name="index">维度索引</param>
        /// <param name="value">维度值</param>
        public void SetDimension(int index, int value)
        {
            if (index >= 0 && index < 8)
            {
                if (index >= nbDims)
                {
                    nbDims = index + 1;
                }
                d[index] = value;
            }
        }
        
        /// <summary>
        /// 获取总元素数量
        /// </summary>
        /// <returns>总元素数量</returns>
        public int GetElementCount()
        {
            int count = 1;
            for (int i = 0; i < nbDims; i++)
            {
                count *= d[i];
            }
            return count;
        }
    }
} 