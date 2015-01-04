Imports System.IO
'Imports System.Windows.Forms
Imports System.Security.Permissions
Imports System.Runtime.InteropServices
Imports System.Timers
Imports System.Windows.Threading
Imports System.Reflection


Public Class GoogleMapControl
	Inherits System.Windows.Controls.UserControl
	Implements iMapCallScript





#Region "iProperties"



#End Region

#Region "Vars"

	Const _DRONENAME As String = "AR Drone"
	Const _DRONECOLOR As String = "#FF0000"

	Private WebBrowser1 As System.Windows.Forms.WebBrowser

	Private InitialZoom As Integer
	Private InitialLatitude As Double
	Private InitialLongitude As Double
	Private InitialMapType As GoogleMapType

	Private cmapdir As String = Directory.GetCurrentDirectory
	Private cmapfile As String = Path.Combine(cmapdir, "map.html")
	Private cmapUri As Uri = New Uri(String.Format("file:///{0}/map.html", CurDir))


	Private Const _TUISpan As Integer = 2000
	Private _LastTUISpan As Integer = 0
	Private _tmrCoorUpdate As New DispatcherTimer(DispatcherPriority.Background) With {.Interval = TimeSpan.FromMilliseconds(_TUISpan)}

	Private CurrentLatitude As Double = 0.0
	Private CurrentLongitude As Double = 0.0
	Private CurrentElevation As Double = 0.0

	Private MapIdle As Boolean = True

	Private MarkerDic As New Dictionary(Of String, Integer)

#End Region

#Region "C.tor"

	Public Sub New()
		Me.DataContext = Nothing
		InitControls()
	End Sub

	Private Sub GoogleMapControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
		InitWebBroser()
	End Sub

	Private Sub InitControls()

		Dim MainGrid As New Grid With {.Name = "MainGrid"}
		Dim MainHost As New Forms.Integration.WindowsFormsHost With {.Name = "MainHost"}
		WebBrowser1 = New System.Windows.Forms.WebBrowser With {.Name = "MainWebBrowser"}

		MainHost.Child = WebBrowser1
		MainGrid.Children.Add(MainHost)
		Me.AddChild(MainGrid)

		AddHandler _tmrCoorUpdate.Tick, AddressOf tmrCoorUpdate_Tick


	End Sub

	Private Sub InitWebBroser()

		InitialZoom = 4
		InitialLatitude = 38
		InitialLongitude = -96.5
		InitialMapType = GoogleMapType.RoadMap

		WebBrowser1.ScrollBarsEnabled = False
		WebBrowser1.AllowWebBrowserDrop = False
		WebBrowser1.IsWebBrowserContextMenuEnabled = False
		WebBrowser1.WebBrowserShortcutsEnabled = False
		WebBrowser1.ObjectForScripting = New ObjectForScriptingHelper(Me)
		WebBrowser1.ScriptErrorsSuppressed = False

		AddHandler WebBrowser1.DocumentCompleted, AddressOf WebBrowser1_DocumentCompleted

		WebBrowser1.DocumentText = My.Computer.FileSystem.ReadAllText(cmapfile)

	End Sub

	Private Sub tmrCoorUpdate_Tick(sender As Object, e As EventArgs)
		UpdateCoords()
	End Sub

#End Region

#Region "Browser Events"

	Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As Forms.WebBrowserDocumentCompletedEventArgs)

		Call_initialize(InitialZoom, InitialLatitude, InitialLongitude, CInt(InitialMapType))
	End Sub

#End Region

#Region "Map Scripts"

	Public Sub InvokeMapScript(ScrName As String, ScrObjects() As Object) Implements iMapCallScript.InvokeMapScript
		MapIdle = False
		WebBrowser1.Document.InvokeScript(ScrName, ScrObjects)
	End Sub

	Public Sub Call_addmarker(MarkerName As String, lat As Double, lng As Double) Implements iMapCallScript.Call_addmarker
		Dim Scr As String = System.Reflection.MethodInfo.GetCurrentMethod().Name
		Dim OBJS As Object() = New Object() {MarkerName, lat, lng}
		InvokeMapScript(Scr, OBJS)
	End Sub

	Public Sub Call_addmarkerwcol(MarkerName As String, MarkerColor As String, lat As Double, lng As Double) Implements iMapCallScript.Call_addmarkerwcol
		Dim Scr As String = System.Reflection.MethodInfo.GetCurrentMethod().Name
		Dim OBJS As Object() = New Object() {MarkerName, MarkerColor, lat, lng}
		InvokeMapScript(Scr, OBJS)
	End Sub

	Public Sub Call_initialize(iInitialZoom As Integer, iInitialLatitude As Double, iInitialLongitude As Double, iInitialMapType As Integer) Implements iMapCallScript.Call_initialize
		Dim Scr As String = System.Reflection.MethodInfo.GetCurrentMethod().Name
		Dim OBJS As Object() = New Object() {iInitialZoom, iInitialLatitude, iInitialLongitude, iInitialMapType}
		InvokeMapScript(Scr, OBJS)
	End Sub

	Public Sub Call_deleteallmarker() Implements iMapCallScript.Call_deleteallmarker
		Dim Scr As String = System.Reflection.MethodInfo.GetCurrentMethod().Name
		InvokeMapScript(Scr, Nothing)
	End Sub

	Public Sub Call_movemarker(MarkerIndex As Integer, lat As Double, lng As Double) Implements iMapCallScript.Call_movemarker
		Dim Scr As String = System.Reflection.MethodInfo.GetCurrentMethod().Name
		Dim OBJS As Object() = New Object() {MarkerIndex, lat, lng}
		InvokeMapScript(Scr, OBJS)
	End Sub

#End Region

#Region "Map Events"

	Public Sub Eve_click(lat As Double, lng As Double) Implements iMapCallScript.Eve_click
		Dim i As String = "lat/lng: " & CStr(Math.Round(lat, 4)) & " , " & CStr(Math.Round(lng, 4))
	End Sub

	Public Sub Eve_idle() Implements iMapCallScript.Eve_idle
		If Not _tmrCoorUpdate.IsEnabled Then _tmrCoorUpdate.Start()
		MapIdle = True
	End Sub

	Public Sub Eve_mousemove(lat As Double, lng As Double) Implements iMapCallScript.Eve_mousemove
		Dim i As String = "lat/lng: " & CStr(Math.Round(lat, 4)) & " , " & CStr(Math.Round(lng, 4))
		Debug.WriteLine(i)
	End Sub

	Public Sub Eve_completed() Implements iMapCallScript.Eve_completed
		If Not _tmrCoorUpdate.IsEnabled Then _tmrCoorUpdate.Start()
		MapIdle = True
	End Sub

	Public Sub Eve_merkeradded(MarkerName As String, MarkerIndex As Integer) Implements iMapCallScript.Eve_merkeradded
		Dim i As String = Now.ToShortTimeString & " : Merker " & MarkerName & " Added"
		If Not MarkerDic.ContainsKey(MarkerName) Then MarkerDic.Add(MarkerName, MarkerIndex - 1)
		Debug.WriteLine(i)
	End Sub

	Public Sub Eve_merkermoved(MarkerName As String, lat As Double, lng As Double) Implements iMapCallScript.Eve_merkermoved
		Dim i As String = Now.ToShortTimeString & " : Merker " & MarkerName & " Move to " & "lat/lng: " & CStr(Math.Round(lat, 4)) & " , " & CStr(Math.Round(lng, 4))
		Debug.WriteLine(i)
	End Sub

	

#End Region

#Region "Methods"

	Private Sub UpdateCoords()
		Try

			CurrentLatitude = LocalSetting.GlobalMapDataInfoCollection(0).NumValue
			CurrentLongitude = LocalSetting.GlobalMapDataInfoCollection(1).NumValue
			CurrentElevation = LocalSetting.GlobalMapDataInfoCollection(2).NumValue

			If MapIdle AndAlso CurrentLatitude <> 0 AndAlso CurrentLongitude <> 0 Then
				If Not MarkerDic.ContainsKey(_DRONENAME) Then
					Call_addmarkerwcol(_DRONENAME, _DRONECOLOR, CurrentLatitude, CurrentLongitude)
				Else
					Call_movemarker(MarkerDic(_DRONENAME), CurrentLatitude, CurrentLongitude)
				End If


			End If


		Catch ex As Exception

		End Try
	End Sub


#End Region





	
End Class



