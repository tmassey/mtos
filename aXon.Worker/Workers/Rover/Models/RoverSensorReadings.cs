using System;
using System.Security.Cryptography.X509Certificates;

namespace aXon.Worker
{
	public class RoverSensorReadings
	{
		

		#region Motor pain Reedings

		public double CurrentFrontLeft { get; set; }

		public double CurrentFrontRight { get; set; }

		public double CurrentBackLeft { get; set; }

		public double CurrentBackRight { get; set; }

		#endregion

		#region Hunger Pain Readings

		public double LogicBatteryVoltage { get; set; }

		public double MotionBatteryVoltage { get; set; }

		public double BackupBatteryVoltage { get; set; }

		public double SolarChargeVoltage { get; set; }

		public double SolarChargeCurrent { get; set; }

		#endregion

		#region Internal Clock

		public double HourOfDay { get; set; }

		public double MinuetOfDay { get; set; }

		public double DayofYear { get; set; }

		#endregion

		public double Temprature { get; set; }

		public double LightLevel { get; set; }

		public double Humidity { get; set; }

		public double AudioAmplitude { get; set; }

		public double BarometricPressure { get; set; }


	}
}

