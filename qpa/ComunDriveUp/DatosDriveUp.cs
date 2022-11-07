using System;

namespace ComunDriveUp
{
    public class DatosDriveUp
	{
		public double	AcceleratorPedalPosition	{ get; set; } // "acceleratorPedalPosition":0.0,
		public double	ActualGear					{ get; set; } // "actualGear":2, // ex int
		public double	AvgFuelEco					{ get; set; } // "avgFuelEco":0,
		public bool		BrakeSwitch					{ get; set; } // "brakeSwitch":true,
		public double	CatalystTankLevel			{ get; set; } // "catalystTankLevel":0, // ex int
		public bool		ClutchSwitch				{ get; set; } // "clutchSwitch":false,
		public bool		CruiseActive				{ get; set; } // "cruiseActive":false,
		public bool		EcoDriving					{ get; set; } // "ecoDriving":false,
		public double	EngineSpeed					{ get; set; } // "engineSpeed":770.38,
		public double	EngineTemperature			{ get; set; } // "engineTemperature":87, // ex int
		public double	EngineTorque				{ get; set; } // "engineTorque":0, // ex int
		public double	FuelLevel					{ get; set; } // "fuelLevel":70.8,
		public double	FuelRate					{ get; set; } // "fuelRate":2.15,
		public double	InstantFuelEco				{ get; set; } // "instantFuelEco":0,
		public double	Lat							{ get; set; } // "lat":-34.4856466,
		public double	Lng							{ get; set; } // "lng":-58.5025633,
		public double	Odometer					{ get; set; } // "odometer":268177.56,
		public string	Plate						{ get; set; } // "plate":"F4172AD226XJ",
		public double	PtoState					{ get; set; } // "ptoState":0, // ex int
		public DateTime	Recordedat					{ get; set; } // "recordedat":"2022-06-09T14:59:58Z",
		public double	RetarderPercentTorque		{ get; set; } // "retarderPercentTorque":0, // ex int
		public double	Speed						{ get; set; } // "speed":9, // ex int
		public double	TurboPressure				{ get; set; } // "turboPressure":0

		public int Ficha
		{
			get
			{
				bool IsNumeric(char c) => c >= '0' && c <= '9';

				if (Plate != null &&
					Plate.Length >= 5 &&
					IsNumeric(Plate[1]) &&
					IsNumeric(Plate[2]) &&
					IsNumeric(Plate[3]) &&
					IsNumeric(Plate[4])
				)
				{
					return int.Parse(Plate.Substring(1, 4));
				}
				else
				{
					return 0;
				}
			}
		}

		public DateTime FechaLocal
		{
			get
			{
				return Recordedat.ToLocalTime();
			}
		}
	}
}
