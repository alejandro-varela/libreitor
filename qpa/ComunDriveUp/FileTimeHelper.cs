//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;

//namespace ComunDriveUp
//{
//    public class FileTimeHelper
//    {
//        private string _pathActual = string.Empty;

//        private static string CreateDirectoryNameByDay(DateTime dateTime)
//        {
//            return $"y{dateTime.Year:0000}/mo{dateTime.Month:00}/d{dateTime.Day:00}/";
//        }

//        public  static string CreateFileNameByHour(string baseDir, DateTime dateTime, string extension = "txt")
//        {
//            string nombreArchivo =
//                CreateDirectoryNameByDay(dateTime).TrimEnd('/') + $"/h{dateTime.Hour:00}"
//            ;

//            if (!string.IsNullOrEmpty(extension))
//            {
//                nombreArchivo += $".{extension}";
//            }

//            string path =
//                Path.Combine(baseDir ?? "", nombreArchivo)
//                .Replace("\\", "/")
//            ;

//            return path;
//        }

//        public Tuple<bool, Exception> WriteToFile(string directorioBase, DateTime dateTime, string sData)
//        {
//            try
//            {
//                string path = CreateFileNameByHour(directorioBase, dateTime);

//                if (path != _pathActual)
//                {
//                    string dir = Path.GetDirectoryName(path);

//                    if (dir != null && !Directory.Exists(dir))
//                    {
//                        Directory.CreateDirectory(dir);
//                        _pathActual = path;
//                    }
//                }

//                File.AppendAllText(path, sData);

//                return new Tuple<bool, Exception>(true, null);
//            }
//            catch (Exception exx)
//            {
//                return new Tuple<bool, Exception>(false, exx);
//            }
//        }
//    }
//}


using System;
using System.IO;

namespace ComunDriveUp
{
    public class FileTimeHelper
    {
        public readonly object WriteLock = new object();
        public string CurrentPath { get; private set; } = string.Empty;

        private static string CreateDirectoryNameByDay(DateTime dateTime)
        {
            return $"y{dateTime.Year:0000}/mo{dateTime.Month:00}/d{dateTime.Day:00}/";
        }

        public static string CreateFileNameByHour(string baseDir, DateTime dateTime, string extension = "txt")
        {
            string nombreArchivo =
                CreateDirectoryNameByDay(dateTime).TrimEnd('/') + $"/h{dateTime.Hour:00}"
            ;

            if (!string.IsNullOrEmpty(extension))
            {
                nombreArchivo += $".{extension}";
            }

            string path =
                Path.Combine(baseDir ?? "", nombreArchivo)
                .Replace("\\", "/")
            ;

            return path;
        }

        public Tuple<bool, Exception> WriteToFile(string directorioBase, DateTime dateTime, string sData)
        {
            try
            {
                lock (WriteLock)
                {
                    string path = CreateFileNameByHour(directorioBase, dateTime);

                    if (path != CurrentPath)
                    {
                        string dir = Path.GetDirectoryName(path);

                        if (dir != null && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            CurrentPath = path;
                        }
                    }

                    File.AppendAllText(path, sData);
                }

                return new Tuple<bool, Exception>(true, null);
            }
            catch (Exception exx)
            {
                return new Tuple<bool, Exception>(false, exx);
            }
        }
    }
}
