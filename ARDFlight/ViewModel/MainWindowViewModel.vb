Imports FirstFloor.ModernUI.Presentation
Imports FirstFloor.ModernUI.Windows.Controls
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.Composition
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Input

Imports Catel.MVVM
Imports Catel.Data
Imports System.Windows.Threading
Imports System.Data
Imports Catel.Collections

Imports Catel.MVVM.Services

Imports AR.Drone.Client
Imports AR.Drone.Client.Command
Imports AR.Drone.Client.Configuration
Imports AR.Drone.Data
Imports AR.Drone.Data.Navigation
Imports AR.Drone.Data.Navigation.Native
Imports AR.Drone.Media
Imports AR.Drone.Video
Imports AR.Drone.Avionics
Imports AR.Drone.Avionics.Objectives
Imports AR.Drone.Avionics.Objectives.IntentObtainers
Imports System.Timers
Imports System.Drawing
Imports ARDFlight.Common



Namespace ViewModels

	Public Class MainWindowViewModel
		Inherits ViewModelBase


#Region "Properties"

		Private SbOutText As New StringBuilder("")
		Public Property OutputText() As String
			Get
				Return GetValue(Of String)(OutputTextProperty)
			End Get
			Set(Value As String)
				SetValue(OutputTextProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly OutputTextProperty As PropertyData = RegisterProperty("OutputText", GetType(String), String.Empty)

		Public Property VideoImage() As System.Drawing.Bitmap
			Get
				Return GetValue(Of System.Drawing.Bitmap)(VideoImageProperty)
			End Get
			Set(Value As System.Drawing.Bitmap)
				SetValue(VideoImageProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly VideoImageProperty As PropertyData = RegisterProperty("VideoImage", GetType(System.Drawing.Bitmap), Nothing)

		Public Property DroneInputsCommadListRight() As FastObservableCollection(Of DroneInputCommand)
			Get
				Return GetValue(Of FastObservableCollection(Of DroneInputCommand))(DroneInputsCommadListRightProperty)
			End Get
			Set(Value As FastObservableCollection(Of DroneInputCommand))
				SetValue(DroneInputsCommadListRightProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly DroneInputsCommadListRightProperty As PropertyData = RegisterProperty("DroneInputsCommadListRight", GetType(FastObservableCollection(Of DroneInputCommand)), Nothing)

		Public Property DroneInputsCommadListLeft() As FastObservableCollection(Of DroneInputCommand)
			Get
				Return GetValue(Of FastObservableCollection(Of DroneInputCommand))(DroneInputsCommadListLeftProperty)
			End Get
			Set(Value As FastObservableCollection(Of DroneInputCommand))
				SetValue(DroneInputsCommadListLeftProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly DroneInputsCommadListLeftProperty As PropertyData = RegisterProperty("DroneInputsCommadListLeft", GetType(FastObservableCollection(Of DroneInputCommand)), Nothing)



		Public Property HudOpacity() As Double
			Get
				Return GetValue(Of Double)(HudOpacityProperty)
			End Get
			Set(Value As Double)
				SetValue(HudOpacityProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly HudOpacityProperty As PropertyData = RegisterProperty("HudOpacity", GetType(Double), 0.6)




#End Region

#Region "Navigation Data"

		Public Property NavigationDataList() As NavigationDataInfoCollection
			Get
				Return GetValue(Of NavigationDataInfoCollection)(NavigationDataListProperty)
			End Get
			Set(Value As NavigationDataInfoCollection)
				SetValue(NavigationDataListProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly NavigationDataListProperty As PropertyData = RegisterProperty("NavigationDataList", GetType(NavigationDataInfoCollection), New NavigationDataInfoCollection(NavigationDataType.GridData))

		Public Property HudNavigationDataList() As NavigationDataInfoCollection
			Get
				Return GetValue(Of NavigationDataInfoCollection)(HudNavigationDataListProperty)
			End Get
			Set(Value As NavigationDataInfoCollection)
				SetValue(HudNavigationDataListProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly HudNavigationDataListProperty As PropertyData = RegisterProperty("HudNavigationDataList", GetType(NavigationDataInfoCollection), New NavigationDataInfoCollection(NavigationDataType.HudData))


#End Region

#Region "Properties Inputs"

		Public Property Roll() As Double
			Get
				Return GetValue(Of Double)(RollProperty)
			End Get
			Set(Value As Double)
				SetValue(RollProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly RollProperty As PropertyData = RegisterProperty("Roll", GetType(Double), 0.0)


		Public Property Yaw() As Double
			Get
				Return GetValue(Of Double)(YawProperty)
			End Get
			Set(Value As Double)
				SetValue(YawProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly YawProperty As PropertyData = RegisterProperty("Yaw", GetType(Double), 0.0)


		Public Property Pitch() As Double
			Get
				Return GetValue(Of Double)(PitchProperty)
			End Get
			Set(Value As Double)
				SetValue(PitchProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly PitchProperty As PropertyData = RegisterProperty("Pitch", GetType(Double), 0.0)


		Public Property Gaz() As Double
			Get
				Return GetValue(Of Double)(GazProperty)
			End Get
			Set(Value As Double)
				SetValue(GazProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly GazProperty As PropertyData = RegisterProperty("Gaz", GetType(Double), 0.0)



#End Region

#Region "Vars"
		Private _DroneAddress As String = "192.168.1.1"

		Private _droneClient As DroneClient
		Private _videoPacketDecoderWorker As VideoPacketDecoderWorker
		Private _frame As VideoFrame
		Private _navigationData As NavigationData
		Private _packetRecorderWorker As PacketRecorder
		Private _navigationPacket As NavigationPacket
		Private _frameBitmap As Bitmap
		Private _frameNumber As UInteger

		Private _settings As Settings



		Private _ViewHandle As IntPtr
		Private _InputEngine As ARDrone.Input.InputManager

		Private _CloseView As Boolean = False
		Private InConnection As Boolean = False
		Private PMV As Dictionary(Of String, Single) = InputsHelper.GetAxisProgressModeValues
		Private AOV As Dictionary(Of String, Single) = InputsHelper.GetAxisOffsetValues
		Private AGV As Dictionary(Of String, Single) = InputsHelper.GetAxisGainValues

		Private Const _TVISpan As Integer = 20
		Private Const _TRTSpan As Integer = 100
		Private Const _TUISpan As Integer = 1000
		Private _LastTUISpan As Integer = 0

		Private _tmrVideoUpdate As New Timer(_TVISpan)
		Private _tmrStateUpdate As New Timer(_TRTSpan)

		Private _MapDataInfoGridList As New MapDataInfoCollection

		Private ARDFlightProcessName As String = "ARDFlightMap"
		Private ARDFlightMapDir As String = IO.Path.Combine(IO.Directory.GetCurrentDirectory, "ARDFlightMap")
		Private ARDFlightMapExe As String = IO.Path.Combine(ARDFlightMapDir, ARDFlightProcessName & ".exe")


#End Region

#Region "C.Tor"

		Public Sub New()

		End Sub

		Public Sub New(Init As Boolean, ViewHandle As IntPtr)

			LocalSetting.UpdateMonitorTextCommand = New Command(Of String)(AddressOf OnUpdateMonitorTextCommandExecute)
			DroneInputsCommadListRight = GetDroneInputsCommadListRight()
			DroneInputsCommadListLeft = GetDroneInputsCommadListLeft()
			UpdateOutputText("System Ready ")
			_ViewHandle = ViewHandle
			RegisterVisualizerService(GetChildrenViews)
			InitEngines()

		End Sub

		Private Sub InitEngines()

			StartVideoEngine(True)
			StartDroneDataEngine(True)
			StartInputEngine(True)


		End Sub

		Public Sub DeInitEngines()
			UpdateOutputText("Stop All Systems")
			CloseGoogleMap()
			StartVideoEngine(False)
			StartDroneDataEngine(False)
			StartInputEngine(False)
			_CloseView = True
		End Sub

		Private Sub StartVideoEngine(Init As Boolean)
			If Init Then
				_videoPacketDecoderWorker = New VideoPacketDecoderWorker(PixelFormat.BGR24, True, AddressOf OnVideoPacketDecoded)
				_videoPacketDecoderWorker.Start()
				AddHandler _tmrVideoUpdate.Elapsed, AddressOf tmrVideoUpdate_Elapsed
				_tmrVideoUpdate.Start()
				AddHandler _videoPacketDecoderWorker.UnhandledException, AddressOf UnhandledException
			Else
				_videoPacketDecoderWorker.Stop()
				_tmrVideoUpdate.Stop()
				RemoveHandler _tmrVideoUpdate.Elapsed, AddressOf tmrVideoUpdate_Elapsed
				RemoveHandler _videoPacketDecoderWorker.UnhandledException, AddressOf UnhandledException
				_videoPacketDecoderWorker.Dispose()

			End If


		End Sub

		Private Sub StartDroneDataEngine(Init As Boolean)

			If Init Then
				_droneClient = New DroneClient(_DroneAddress)
				AddHandler _droneClient.NavigationPacketAcquired, AddressOf OnNavigationPacketAcquired
				AddHandler _droneClient.VideoPacketAcquired, AddressOf OnVideoPacketAcquired
				AddHandler _droneClient.NavigationDataAcquired, Sub(data) _navigationData = data
				AddHandler _tmrStateUpdate.Elapsed, AddressOf tmrStateUpdate_Elapsed
				_tmrStateUpdate.Start()
			Else
				RemoveHandler _droneClient.NavigationPacketAcquired, AddressOf OnNavigationPacketAcquired
				RemoveHandler _droneClient.VideoPacketAcquired, AddressOf OnVideoPacketAcquired
				RemoveHandler _tmrStateUpdate.Elapsed, AddressOf tmrStateUpdate_Elapsed
				_tmrStateUpdate.Stop()

			End If


		End Sub

		Private Sub StartInputEngine(Init As Boolean)
			If Init Then
				_InputEngine = New ARDrone.Input.InputManager(_ViewHandle)
				_InputEngine.SwitchInputMode(ARDrone.Input.InputManager.InputMode.ControlInput)

				AddHandler _InputEngine.NewInputState, AddressOf _InputEngine_NewInputState
				AddHandler _InputEngine.NewInputDevice, AddressOf _InputEngine_NewInputDevice
				AddHandler _InputEngine.InputDeviceLost, AddressOf _InputEngine_InputDeviceLost
			Else

				_InputEngine.SwitchInputMode(ARDrone.Input.InputManager.InputMode.NoInput)

				RemoveHandler _InputEngine.NewInputState, AddressOf _InputEngine_NewInputState
				RemoveHandler _InputEngine.NewInputDevice, AddressOf _InputEngine_NewInputDevice
				RemoveHandler _InputEngine.InputDeviceLost, AddressOf _InputEngine_InputDeviceLost
				_InputEngine.Dispose()

			End If


		End Sub

		Public Function CloseState() As Boolean
			Return _CloseView
		End Function

#End Region

#Region "Inputs Events"

		Private Sub _InputEngine_NewInputState(sender As Object, e As ARDrone.Input.Utils.NewInputStateEventArgs)
			UpdateInputValues(e.CurrentInputState)
			SendInputToDrone(e.CurrentInputState)
		End Sub

		Private Sub _InputEngine_NewInputDevice(sender As Object, e As ARDrone.Input.Utils.NewInputDeviceEventArgs)
			UpdateOutputText("New input device: " & e.DeviceName)
		End Sub

		Private Sub _InputEngine_InputDeviceLost(sender As Object, e As ARDrone.Input.Utils.InputDeviceLostEventArgs)
			UpdateOutputText("Lost input device: " & e.DeviceName)
		End Sub

		Private Sub SendInputToDrone(e As ARDrone.Input.Utils.InputState)

			Dim FMode As FlightMode = FlightMode.Progressive
			Dim NewRollValue As Single = NewAxisValue(e.Roll, InputsHelper._ROLL, FMode)
			Dim NewPitchValue As Single = NewAxisValue(e.Pitch, InputsHelper._PITCH, FMode)
			Dim NewYawValue As Single = NewAxisValue(e.Yaw, InputsHelper._YAW, FMode)
			Dim NewGazValue As Single = NewAxisValue(-e.Gaz, InputsHelper._GAZ, FMode)


			If Not _droneClient.IsConnected Then Return

			If e.CameraSwap Then OnChangeCameraCommandExecute()
			If e.TakeOff Then OnTakeoffCommandExecute()
			If e.Land Then OnLandCommandExecute()
			If e.Hover Then OnHoverCommandExecute()
			If e.Emergency Then OnEmergencyCommandExecute()
			If e.FlatTrim Then OnFlatTrimCommandExecute()


			_droneClient.Progress(FMode, NewRollValue, NewPitchValue, NewYawValue, NewGazValue)


		End Sub

		Private Function NewAxisValue(RequestValue As Single, AxisName As String, ByRef FMode As FlightMode) As Single
			Dim Result As Single
			Dim AxisOffsetValue As Single = AOV(AxisName)
			Dim ProgressModeValue As Single = PMV(AxisName)
			Dim AxisGainValue As Single = AGV(AxisName)
			Dim IsKeyboardInput As Boolean = False

			If RequestValue > 0 AndAlso RequestValue > AxisOffsetValue Then

				If RequestValue >= 1 Then
					FMode = FlightMode.Progressive
					Result = 1
					If IsKeyboardInput Then Result = ProgressModeValue
				Else
					Result = RequestValue - AxisOffsetValue
				End If

			End If

			If RequestValue < 0 AndAlso RequestValue < -AxisOffsetValue Then

				If RequestValue <= -1 Then
					FMode = FlightMode.Progressive
					Result = -1
					If IsKeyboardInput Then Result = -ProgressModeValue
				Else
					Result = RequestValue + AxisOffsetValue
				End If


			End If








			'Console.WriteLine(AxisName & " IN:" & RequestValue.ToString & " OUT:" & Result.ToString)

			Return Result / AxisGainValue
		End Function

		Private Sub UpdateInputValues(e As ARDrone.Input.Utils.InputState)
			Roll = CDbl(e.Roll)
			Pitch = CDbl(-e.Pitch)
			Yaw = CDbl(-e.Yaw)
			Gaz = CDbl(-e.Gaz)
		End Sub

		Private Sub ShowInputSettings()
			_InputEngine.SwitchInputMode(ARDrone.Input.InputManager.InputMode.NoInput)

			Dim Nv As ViewModels.InputSettingsViewModel
			Nv = New ViewModels.InputSettingsViewModel(True, _InputEngine)
			Dim Result As Boolean = CBool(UiService.ShowDialog(Nv))


			_InputEngine.SwitchInputMode(ARDrone.Input.InputManager.InputMode.ControlInput)

		End Sub


#End Region

#Region "VideoPacketDecoder Events"

		Private Sub OnVideoPacketDecoded(ByVal frame As VideoFrame)
			_frame = frame
		End Sub

		Private Sub UnhandledException(ByVal sender As Object, ByVal exception As Exception)
			UpdateOutputText("Unhandled Exception " & exception.ToString)
		End Sub

		Private Sub tmrVideoUpdate_Elapsed(sender As Object, e As ElapsedEventArgs)
			Dim nb As Bitmap

			If _frame Is Nothing OrElse _frameNumber = _frame.Number Then
				Return
			End If

			_frameNumber = _frame.Number

			If _frameBitmap Is Nothing Then
				_frameBitmap = VideoHelper.CreateBitmap(_frame)
				nb = _frameBitmap.Clone
			Else
				VideoHelper.UpdateBitmap(_frameBitmap, _frame)
				nb = _frameBitmap.Clone
			End If

			VideoImage = nb

			'Throw New NotImplementedException
		End Sub


#End Region

#Region "droneClient Events"

		Private Sub OnNavigationPacketAcquired(ByVal packet As NavigationPacket)
			If _packetRecorderWorker IsNot Nothing AndAlso _packetRecorderWorker.IsAlive Then
				_packetRecorderWorker.EnqueuePacket(packet)
			End If

			_navigationPacket = packet
		End Sub

		Private Sub OnVideoPacketAcquired(ByVal packet As VideoPacket)
			If _packetRecorderWorker IsNot Nothing AndAlso _packetRecorderWorker.IsAlive Then
				_packetRecorderWorker.EnqueuePacket(packet)
			End If
			If _videoPacketDecoderWorker.IsAlive Then
				_videoPacketDecoderWorker.EnqueuePacket(packet)
			End If
		End Sub

		Private Sub tmrStateUpdate_Elapsed(sender As Object, e As ElapsedEventArgs)

			_LastTUISpan = _LastTUISpan + _TRTSpan

			If Not _LastTUISpan >= _TUISpan Then


				If Not _navigationData Is Nothing Then

					HudNavigationDataList.UpdateValue(NavigationDataField.Longitude, _navigationData.GPS.longitude)
					HudNavigationDataList.UpdateValue(NavigationDataField.Latitude, _navigationData.GPS.latitude)
					HudNavigationDataList.UpdateValue(NavigationDataField.Elevation, _navigationData.GPS.elevation)
					HudNavigationDataList.UpdateValue(NavigationDataField.Roll, (CDbl(_navigationData.Roll)))
					HudNavigationDataList.UpdateValue(NavigationDataField.Pitch, (CDbl(_navigationData.Pitch)))
					HudNavigationDataList.UpdateValue(NavigationDataField.Yaw, (CDbl(_navigationData.Yaw)))
					HudNavigationDataList.UpdateValue(NavigationDataField.Altitude, (CDbl(_navigationData.Altitude)))

				End If


			Else
				_LastTUISpan = 0

				For Each BR In DroneInputsCommadListRight
					BR.CheckIsEnabled()
				Next
				For Each BR In DroneInputsCommadListLeft
					BR.CheckIsEnabled()
				Next

				If Not _navigationData Is Nothing Then

					Try
						If InConnection AndAlso _droneClient.IsConnected Then
							InConnection = False
							UpdateOutputText("Drone Connected ")
						End If

						NavigationDataList.UpdateValue(NavigationDataField.DroneClientIsActive, _droneClient.IsConnected)
						NavigationDataList.UpdateValue(NavigationDataField.NavigationDataBatteryLow, _navigationData.Battery.Low)
						NavigationDataList.UpdateValue(NavigationDataField.BatteryPercentage, (CDbl(_navigationData.Battery.Percentage)))
						NavigationDataList.UpdateValue(NavigationDataField.BatteryVoltage, (CDbl(_navigationData.Battery.Voltage)))
						NavigationDataList.UpdateValue(NavigationDataField.NavigationDataState, _navigationData.State.ToString)
						NavigationDataList.UpdateValue(NavigationDataField.NavigationDataWifiLinkQuality, (CDbl(_navigationData.Wifi.LinkQuality)))

						_MapDataInfoGridList.UpdateValue(MapDataInfoField.Longitude, _navigationData.GPS.longitude)
						_MapDataInfoGridList.UpdateValue(MapDataInfoField.Latitude, _navigationData.GPS.latitude)
						_MapDataInfoGridList.UpdateValue(MapDataInfoField.Elevation, _navigationData.GPS.elevation)

					

						ARDFlight.Common.MapPipeClient.SedMapData(MapDataInfoCollection.ToXElement(_MapDataInfoGridList).ToString, ARDFlight.Common.MapPipeServer._DEFMAPSERVERNAME)


					Catch ex As Exception

					End Try

				End If
			End If





		End Sub

#End Region

#Region "External Command Executes"

		Private Sub OnUpdateMonitorTextCommandExecute(Parameter As String)

			If Parameter IsNot Nothing Then UpdateOutputText(Parameter)

		End Sub


#End Region

#Region "Commands"

		'GeneralSettingsCommand
		Public ReadOnly Property GeneralSettingsCommand() As Command
			Get
				Return _GeneralSettingsCommand
			End Get
		End Property

		Private ReadOnly _GeneralSettingsCommand As Command = New Command(AddressOf OnGeneralSettingsCommandExecute, Function() Not CheckIsConnected() And Not InConnection)

		Private Sub OnGeneralSettingsCommandExecute()
			Dim testc As Integer = 0
			' TODO: Handle command logic here
		End Sub

		'InputActionCommand
		Public ReadOnly Property InputActionCommand() As Command(Of Command)
			Get
				Return _InputActionCommand
			End Get
		End Property

		Private ReadOnly _InputActionCommand As Command(Of Command) = New Command(Of Command)(AddressOf OnInputActionCommandExecute)

		Private Sub OnInputActionCommandExecute(Parameter As Command)
			If Parameter IsNot Nothing Then
				Parameter.Execute()
			End If
		End Sub

		'ShutdownCommand
		Public ReadOnly Property ShutdownCommand() As Command
			Get
				Return _ShutdownCommand
			End Get
		End Property

		Private ReadOnly _ShutdownCommand As Command = New Command(AddressOf OnShutdownCommandExecute, Function() CheckIsConnected() Or InConnection)

		Private Sub OnShutdownCommandExecute()
			_droneClient.Stop()
			UpdateOutputText("DisConnect")
			InConnection = False

		End Sub

		'ReadSettingsCommand
		Public ReadOnly Property ReadSettingsCommand() As Command
			Get
				Return _ReadSettingsCommand
			End Get
		End Property

		Private ReadOnly _ReadSettingsCommand As Command = New Command(AddressOf OnReadSettingsCommandExecute, Function() CheckIsConnected() And Not InConnection)

		Private Sub OnReadSettingsCommandExecute()
			ReadSettings()
		End Sub

		'InputSettingsCommand
		Public ReadOnly Property InputSettingsCommand() As Command
			Get
				Return _InputSettingsCommand
			End Get
		End Property

		Private ReadOnly _InputSettingsCommand As Command = New Command(AddressOf OnInputSettingsCommandExecute, Function() Not CheckIsConnected() And Not InConnection)

		Private Sub OnInputSettingsCommandExecute()

			ShowInputSettings()


		End Sub

		'ShowConfigCommand
		Public ReadOnly Property ShowConfigCommand() As Command
			Get
				Return _ShowConfigCommand
			End Get
		End Property

		Private ReadOnly _ShowConfigCommand As Command = New Command(AddressOf OnShowConfigCommandExecute, Function() CheckIsConnected() And Not InConnection)

		Private Sub OnShowConfigCommandExecute()

			ShowDroneInfo()
		End Sub

		'ConnectCommand
		Public ReadOnly Property ConnectCommand() As Command
			Get
				Return _ConnectCommand
			End Get
		End Property

		Private ReadOnly _ConnectCommand As Command = New Command(AddressOf OnConnectCommandExecute, Function() Not CheckIsConnected() And Not InConnection)

		Private Sub OnConnectCommandExecute()
			InConnection = True
			_droneClient.Start()
			UpdateOutputText("Connecting")
		End Sub

		'TakeoffCommand
		Public ReadOnly Property TakeoffCommand() As Command
			Get
				Return _TakeoffCommand
			End Get
		End Property

		Private ReadOnly _TakeoffCommand As Command = New Command(AddressOf OnTakeoffCommandExecute, Function() CheckIsConnected() And Not InConnection)

		Private Sub OnTakeoffCommandExecute()
			_droneClient.Takeoff()
			UpdateOutputText("Takeoff")
		End Sub

		'LandCommand
		Public ReadOnly Property LandCommand() As Command
			Get
				Return _LandCommand
			End Get
		End Property

		Private ReadOnly _LandCommand As Command = New Command(AddressOf OnLandCommandExecute, Function() CheckIsConnected() And Not InConnection)

		Private Sub OnLandCommandExecute()
			_droneClient.Land()
			UpdateOutputText("Land")
		End Sub

		'HoverCommand
		Public ReadOnly Property HoverCommand() As Command
			Get
				Return _HoverCommand
			End Get
		End Property

		Private ReadOnly _HoverCommand As Command = New Command(AddressOf OnHoverCommandExecute, Function() CheckIsConnected() And Not InConnection)

		Private Sub OnHoverCommandExecute()
			_droneClient.Hover()
			UpdateOutputText("Hover")
		End Sub

		'EmergencyCommand
		Public ReadOnly Property EmergencyCommand() As Command
			Get
				Return _EmergencyCommand
			End Get
		End Property

		Private ReadOnly _EmergencyCommand As Command = New Command(AddressOf OnEmergencyCommandExecute, Function() CheckIsConnected() And Not InConnection)

		Private Sub OnEmergencyCommandExecute()
			_droneClient.Emergency()
			UpdateOutputText("Emergency")
		End Sub

		'ResetEmergencyCommand
		Public ReadOnly Property ResetEmergencyCommand() As Command
			Get
				Return _ResetEmergencyCommand
			End Get
		End Property

		Private ReadOnly _ResetEmergencyCommand As Command = New Command(AddressOf OnResetEmergencyCommandExecute, Function() CheckIsConnected() And Not InConnection)

		Private Sub OnResetEmergencyCommandExecute()
			_droneClient.ResetEmergency()
			UpdateOutputText("Reset Emergency")
		End Sub

		'FlatTrimCommand
		Public ReadOnly Property FlatTrimCommand() As Command
			Get
				Return _FlatTrimCommand
			End Get
		End Property

		Private ReadOnly _FlatTrimCommand As Command = New Command(AddressOf OnFlatTrimCommandExecute, Function() CheckIsConnected() And Not InConnection)

		Private Sub OnFlatTrimCommandExecute()
			_droneClient.FlatTrim()
			UpdateOutputText("Flat Trim")
		End Sub

		'ChangeCameraCommand
		Public ReadOnly Property ChangeCameraCommand() As Command
			Get
				Return _ChangeCameraCommand
			End Get
		End Property

		Private ReadOnly _ChangeCameraCommand As Command = New Command(AddressOf OnChangeCameraCommandExecute, Function() CheckIsConnected() And Not InConnection)

		Private Sub OnChangeCameraCommandExecute()
			Dim crconfig As New Settings
			crconfig.Video.Channel = VideoChannelType.Next
			_droneClient.Send(crconfig)
			UpdateOutputText("Change Camera")
		End Sub

		'ShowMapCommad
		Public ReadOnly Property ShowMapCommand() As Command
			Get
				Return _ShowMapCommand
			End Get
		End Property

		Private ReadOnly _ShowMapCommand As Command = New Command(AddressOf OnShowMapCommandExecute)

		Private Sub OnShowMapCommandExecute()

			ShowGoogleMap()
		End Sub




#End Region

#Region "VisualizerService Registration"

		Private UiService As IUIVisualizerService
		Private Sub RegisterVisualizerService(Vms As List(Of KeyValuePair(Of Type, Type)))

			UiService = TryCast(DependencyResolver.Resolve(GetType(IUIVisualizerService)), IUIVisualizerService)


			For Each V In Vms

				Try
					UiService.Unregister(V.Key)
					UiService.Register(V.Key, V.Value)
				Catch ex As Exception

				End Try

			Next

		End Sub

		Private Sub DeregisterVisualizerService(Vms As List(Of KeyValuePair(Of Type, Type)))

			If UiService IsNot Nothing Then
				For Each V In Vms

					UiService.Unregister(V.Key)
				Next
			End If


		End Sub

		Private Function GetChildrenViews() As List(Of KeyValuePair(Of Type, Type))
			Dim Result As New List(Of KeyValuePair(Of Type, Type))

			Result.Add(New KeyValuePair(Of Type, Type)(GetType(ViewModels.InputSettingsViewModel), GetType(InputSettingsView)))
			Result.Add(New KeyValuePair(Of Type, Type)(GetType(ViewModels.DroneInfoViewModel), GetType(DroneInfoView)))

			Return Result
		End Function

#End Region

#Region "Methods"

		Private Sub UpdateOutputText(NewText As String)

			SbOutText.Insert(0, Now.ToShortTimeString & " " & NewText & Environment.NewLine)


			OutputText = SbOutText.ToString

		End Sub

		Private Sub ReadSettings()
			Dim configurationTask As Task(Of Settings) = _droneClient.GetConfigurationTask()
			configurationTask.ContinueWith(AddressOf ReadSettingsTask)


			configurationTask.Start()
		End Sub

		Private Sub ReadSettingsTask(ByVal task As Task(Of Settings))
			If task.Exception IsNot Nothing Then
				Trace.TraceWarning("Get configuration task is faulted with exception: {0}", task.Exception.InnerException.Message)
				UpdateOutputText("Get configuration task is faulted with exception: " & task.Exception.InnerException.Message)
				Return
			End If

			_settings = task.Result
			Dim lat As Object = _settings.Gps.Latitude
		End Sub

		Public Function CheckIsConnected() As Boolean
			Dim result As Boolean = False

			Try
				If _droneClient IsNot Nothing AndAlso _droneClient.IsConnected Then result = True
			Catch ex As Exception

			End Try

			Return result
		End Function

		Private Sub ShowDroneInfo()

			_InputEngine.SwitchInputMode(ARDrone.Input.InputManager.InputMode.NoInput)

			Dim Nv As ViewModels.DroneInfoViewModel
			Nv = New ViewModels.DroneInfoViewModel(True, _droneClient)
			Dim Result As Boolean = CBool(UiService.ShowDialog(Nv))

			_InputEngine.SwitchInputMode(ARDrone.Input.InputManager.InputMode.ControlInput)

			'ShowInfoWindow()

		End Sub

		Private Function GetDroneInputsCommadListLeft() As FastObservableCollection(Of DroneInputCommand)
			Dim Result As New FastObservableCollection(Of DroneInputCommand)

			Result.Add(New DroneInputCommand("Connect", "..", ConnectCommand))
			Result.Add(New DroneInputCommand("Take off", "..", TakeoffCommand))
			Result.Add(New DroneInputCommand("Emergency", "..", EmergencyCommand))
			Result.Add(New DroneInputCommand("Hover", "..", HoverCommand))
			Result.Add(New DroneInputCommand("Camera", "Switch", ChangeCameraCommand))
			Result.Add(New DroneInputCommand("Drone", "Config", ReadSettingsCommand)) '	Dim lat As Object = _settings.Gps.Latitude	GeneralSettingsCommand




			Return Result
		End Function

		Private Function GetDroneInputsCommadListRight() As FastObservableCollection(Of DroneInputCommand)
			Dim Result As New FastObservableCollection(Of DroneInputCommand)

			Result.Add(New DroneInputCommand("Disconnect", "..", ShutdownCommand))
			Result.Add(New DroneInputCommand("Land", "..", LandCommand))
			Result.Add(New DroneInputCommand("Emergency", "Reset", ResetEmergencyCommand))
			Result.Add(New DroneInputCommand("Flat trim", "..", FlatTrimCommand))
			Result.Add(New DroneInputCommand("Drone", "Info", ShowConfigCommand))
			Result.Add(New DroneInputCommand("Input", "Config", InputSettingsCommand))
			Result.Add(New DroneInputCommand("Show", "Map", ShowMapCommand))

			Return Result
		End Function

		Private Sub ShowGoogleMap()

			Try
				Dim p = Process.GetProcessesByName(ARDFlightProcessName)
				If p.Count > 0 Then
					MsgBox("ARD Flight Map Running ", MsgBoxStyle.Information, "ARD Flight")
				Else
					Dim NP As New ProcessStartInfo(ARDFlightMapExe)
					NP.WorkingDirectory = ARDFlightMapDir
					Process.Start(NP)
				End If
			Catch ex As Exception

			End Try


		End Sub

		Private Sub CloseGoogleMap()

			Try
				Dim p As Process() = Process.GetProcessesByName(ARDFlightProcessName)
				If p.Count > 0 Then
					For Each pp In p
						pp.Kill()
					Next				
				End If
			Catch ex As Exception

			End Try


		End Sub


#End Region










	End Class

End Namespace

