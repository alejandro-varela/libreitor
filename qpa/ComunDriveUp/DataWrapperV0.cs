using System;

namespace ComunDriveUp
{
    public class DataWrapperV0 : DataWrapper
	{
		public DateTime RecvUtc { get; set; }
		public DatosDriveUp Data { get; set; }
    }
}
