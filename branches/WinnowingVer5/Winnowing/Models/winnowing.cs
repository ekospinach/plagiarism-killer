using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.IO;


namespace Winnowing.Models
{
    public class info
    {
        public string title;
        public List<int> fingerPrint;
        public List<int> position;
        public info()
        {
            fingerPrint = new List<int>();
            position = new List<int>();
        }
    }

    public struct resultInfo
    {
        public string title;
        public int paperID;
        public double similarity;
        public bool IsBig(resultInfo o)
        {
            if (this.similarity > o.similarity)
                return true;
            else
                return false;
        }
    }

    static public class winnowing
    {
        private const int minThreshold = 6, windowSize = 8, maxThreshold = minThreshold + windowSize - 1;
        //static private int readMark = 0;

        //static public Winnowing()
        //{
        //    //minThreshold = 6;
        //    //windowSize = 8;
        //    //maxThreshold = minThreshold + windowSize - 1;
        //}

        static private String PreProcess(String temp)
        {
            String result = "";
            for (int i = 0; i < temp.Length; ++i)
            {
                if (('a' <= temp[i] && temp[i] <= 'z') ||  // an lower-case letter
                   (0x4E00 <= temp[i] && temp[i] <= 0x9FBF) ||
                   (temp[i] == ' ' && result.Length > 0 &&
                   result[result.Length - 1] != ' '))  //Chinese, Japanese, Korean
                    result += temp[i];
            }
                return result;

        }

        static private void QuickSort(ref info buffer, int p, int r)
        {
            if (p >= r)
                return;
            int i = p, j = r, x = (int)buffer.fingerPrint[p + (r - p) / 2];
            while (true)
            {
                while ((int)buffer.fingerPrint[i] < x && i <= r)
                    i++;
                while ((int)buffer.fingerPrint[j] > x && j >= p)
                    j--;
                if (i < j)
                {
                    int temp = buffer.fingerPrint[i];
                    buffer.fingerPrint[i] = buffer.fingerPrint[j];
                    buffer.fingerPrint[j] = temp;
                    temp = buffer.position[i];
                    buffer.position[i] = buffer.position[j];
                    buffer.position[j] = temp;
                    i++;
                    j--;
                }
                else
                    break;
            }
            QuickSort(ref buffer, p, j);
            QuickSort(ref buffer, j + 1, r);
        }

        static private double Compare(ref info user, ref info benchmark)
        {
            int pStart = 0, nStart, same = 0;
            for (int i = 0; i < user.fingerPrint.Count; i++)
            {
                nStart = pStart;
                while ((nStart < benchmark.fingerPrint.Count) &&
                ((int)user.fingerPrint[i] > (int)benchmark.fingerPrint[nStart]))
                    nStart++;
                if (nStart == benchmark.fingerPrint.Count)
                    break;
                pStart = nStart;
                if (user.fingerPrint[i] == benchmark.fingerPrint[nStart])
                    same++;
            }
            return (double)same / (double)user.fingerPrint.Count;
        }

        static public info CalcHash(ref String txt)
        {
            info buffer = new info();
            buffer.fingerPrint = new List<int>();
            buffer.position = new List<int>();
            info temp = new info();
            temp.fingerPrint = new List<int>();
            temp.position = new List<int>();
            int len = 0;
            for (int i = 0; i < txt.Length; i++)
            {
                if ((txt[i] != ' ') && (i == 0 || txt[i - 1] == ' '))
                    len++;          //求出空格间隔的词数
            }
            for (int i = 0; i <= len - minThreshold; i++)    //calculate hash-values of Strings whose length is k
            {
                String temp1 = "";
                int blank = -1;
                for (int j = 0; j < txt.Length; j++)
                {
                    if ((txt[j] != ' ') && (j == 0 || txt[j - 1] == ' '))
                        blank++;    //确定起始位置i
                    if (blank == i)
                    {
                        int kk = 0;
                        for (int i1 = j; ; i1++)
                        {
                            if (txt[i1] != ' ')
                                temp1 += txt[i1];
                            else
                                if (txt[i1 - 1] != ' ')
                                    kk++;
                            if (kk == minThreshold)
                                break;
                        }
                        break;
                    }
                }
                int hashValue = temp1.GetHashCode();
                temp.fingerPrint.Add(hashValue);
                temp.position.Add(i);
            }
            buffer.fingerPrint.Add(temp.fingerPrint[0]);//find the minimum in every w hash-values
            buffer.position.Add(temp.position[0]);
            for (int i = 1; i < windowSize; i++)
                if ((int)temp.fingerPrint[i] < (int)buffer.fingerPrint[0])
                {
                    buffer.fingerPrint[0] = temp.fingerPrint[i];
                    buffer.position[0] = i;
                }

            int jj = (int)buffer.position[0];
            for (int i = windowSize; i <= (int)temp.fingerPrint.Count - windowSize; i++)
            {
                if (i - jj < windowSize)
                {
                    if ((int)temp.fingerPrint[jj] > (int)temp.fingerPrint[i])
                        jj = i;
                    if (jj != (int)buffer.position[buffer.position.Count - 1])
                    {
                        buffer.fingerPrint.Add(temp.fingerPrint[jj]);
                        buffer.position.Add(temp.position[jj]);
                    }
                }
                else
                {
                    int min = jj + 1;
                    while (jj < i)
                    {
                        jj++;
                        if ((int)temp.fingerPrint[min] > (int)temp.fingerPrint[jj])
                            min = jj;
                    }
                    jj = min;
                    if (jj != (int)buffer.position[buffer.position.Count - 1] && jj < temp.fingerPrint.Count)
                    {
                        buffer.fingerPrint.Add(temp.fingerPrint[jj]);
                        buffer.position.Add(temp.position[jj]);
                    }
                }
            }
            QuickSort(ref buffer, 0, buffer.fingerPrint.Count - 1);
            return buffer;
        }

        static private String Split(String input)
        {
            NICTCLAS nictclas = null;

            String dictFolderPath = @"C:\Users\Cyber\Desktop\WinnowingVer5\Winnowing\CacheFolder\";//载入词典路径 
            try
            {
                nictclas = new NICTCLAS(dictFolderPath);
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);

                //Response.End();
            }
            nictclas.OperateType = eOperateType.OnlySegment;

            String result = "";
            nictclas.ParagraphProcessing(input, ref result);
            //nictclas = null;
            return result;
        }

        static public List<resultInfo> Process(String fileName)
        {
            String text = "";
            List<resultInfo> result = new List<resultInfo>();
            try
            {
                using (StreamReader input = new StreamReader(fileName, System.Text.Encoding.Default))
                {
                    String line;
                    while ((line = input.ReadLine()) != null)
                    {
                        text += line;
                    }
                    input.Close();
                }
            }
            catch (Exception e)
            {
                return result;
            }
            
            //StreamWriter sw = new StreamWriter("D:\\tmp.txt", false, System.Text.Encoding.UTF8);
            //sw.WriteLine(processedText);
            //sw.Close();
            //StreamReader sr = new StreamReader("D:\\tmp.txt", System.Text.Encoding.Default);
            //processedText = sr.ReadLine();
            //sr.Close();

            //byte[] tmpByte = System.Text.Encoding.UTF8.GetBytes(processedText);
            //processedText = System.Text.Encoding.Unicode.GetString(tmpByte);

          //  String spiltText = text;
            String spiltText = Split(text);
            String processedText = PreProcess(spiltText);
            info target = CalcHash(ref processedText);

            StreamWriter sw = new StreamWriter(@"C:\Users\Cyber\Desktop\WinnowingVer5\06.txt");
            String outputText = target.fingerPrint[0].ToString();
            for (int i = 1; i < target.fingerPrint.Count; i++)
                outputText += ' ' + target.fingerPrint[i].ToString();
            sw.WriteLine(outputText);
            sw.Close();

            resultInfo tmp = new resultInfo();
            paperinfoRepositoryNewVer zizi = new paperinfoRepositoryNewVer();
            List<info> allInfo = zizi.FindAllInfo();
            foreach (info item in allInfo){
                info source = item;
                tmp.similarity = Compare(ref target, ref source);
                tmp.title = item.title;
                result.Add(tmp);
          
            }


            //tmp.paperID = paperInfoRepository.SetNext();
            //while(tmp.paperID !=0)
            //{
            //    info source = paperInfoRepository.GetCurrent();
            //    tmp.similarity = Compare(ref target, ref source);
            //    if(tmp.similarity>0.1)
            //    {
            //        bool isInsert = false;
            //        for (int i = result.Count; i > 0; i--)
            //            if (!tmp.IsBig((resultInfo)result[i - 1]))
            //            {
            //                result.Insert(i, tmp);
            //                isInsert = true;
            //                break;
            //            }
            //        if(!isInsert)
            //        {
            //            result.Insert(0, tmp);
            //        }
            //        if (result.Count > 10)
            //            result.RemoveAt(10);
            //    }
            //    tmp.paperID = paperInfoRepository.SetNext();
            //}

            return result;
        }

    }
}
