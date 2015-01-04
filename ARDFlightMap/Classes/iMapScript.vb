Public Enum GoogleMapType
	None
	RoadMap
	Terrain
	Hybrid
	Satellite
End Enum

Public Interface iMapEventHandler

#Region "Map Events"

	Sub Eve_mousemove(ByVal lat As Double, ByVal lng As Double)
	Event mousemove(ByVal lat As Double, ByVal lng As Double)

	Sub Eve_click(ByVal lat As Double, ByVal lng As Double)
	Event click(ByVal lat As Double, ByVal lng As Double)

	Sub Eve_idle()
	Event idle()

	Sub Eve_completed()
	Event completed()

	Sub Eve_merkeradded(MarkerName As String, MarkerIndex As Integer)
	Event merkeradded(MarkerName As String, MarkerIndex As Integer)

	Sub Eve_merkermoved(MarkerName As String, ByVal lat As Double, ByVal lng As Double)
	Event merkermoved(MarkerName As String, ByVal lat As Double, ByVal lng As Double)

#End Region

End Interface

Public Interface iMapCallScript
	
#Region "Methods"

	Sub InvokeMapScript(ScrName As String, ScrObjects As Object())

#End Region

#Region "Map Calls"


	Sub Call_initialize(iInitialZoom As Integer, iInitialLatitude As Double, iInitialLongitude As Double, iInitialMapType As Integer)

	Sub Call_addmarker(MarkerName As String, ByVal lat As Double, ByVal lng As Double)

	Sub Call_addmarkerwcol(MarkerName As String, MarkerColor As String, ByVal lat As Double, ByVal lng As Double)

	Sub Call_deleteallmarker()

	Sub Call_movemarker(MarkerIndex As Integer, ByVal lat As Double, ByVal lng As Double)

#End Region

#Region "Map Event"

	Sub Eve_mousemove(ByVal lat As Double, ByVal lng As Double)

	Sub Eve_click(ByVal lat As Double, ByVal lng As Double)

	Sub Eve_idle()

	Sub Eve_completed()

	Sub Eve_merkeradded(MarkerName As String, MarkerIndex As Integer)

	Sub Eve_merkermoved(MarkerName As String, ByVal lat As Double, ByVal lng As Double)



#End Region




End Interface


