using System.Runtime.InteropServices;
using AR.Drone.Data.Navigation.Native.Math;

namespace AR.Drone.Data.Navigation.Native.Options
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public struct navdata_gps_t
	{
		public ushort tag;
		public ushort size;
		public System.Double latitude;
		public System.Double longitude;
		public System.Double elevation;
		public System.Double hdop;
		public System.UInt32 dataAvailable;
		public System.UInt32 zeroValidated;
		public System.UInt32 wptValidated;
		public System.Double lat0;
		public System.Double lon0;
		public System.Double latFuse;
		public System.Double lonFuse;
		public System.UInt32 gpsState;
		public float xTraj;
		public float xRef;
		public float yTraj;
		public float yRef;
		public float thetaP;
		public float phiP;
		public float thetaI;
		public float phiI;
		public float thetaD;
		public float phiD;
		public System.Double vdop;
		public System.Double pdop;
		public float speed;
		public System.UInt32 lastFrameTimestamp; // droneTimeToMilliSeconds(System.UInt32 __),
		public float degree;
		public float degreeMag;
		public float ehpe;
		public float ehve;
		public float c_n0;
		public System.UInt32 nbSatellites;
		public System.Int32 channels; //    timesMap(12, satChannel, reader),
		public System.UInt32 gpsPlugged;
		public System.UInt32 ephemerisStatus;
		public float vxTraj;
		public float vyTraj;
		public System.UInt32 firmwareStatus;
	}
}
