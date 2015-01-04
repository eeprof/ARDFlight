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



Namespace ViewModels


	Public Class InputSettingsViewModel
		Inherits ViewModelBase

#Region "Properties"

		'Public Property MappingControlVisibility() As Boolean
		'	Get
		'		Return GetValue(Of Boolean)(MappingControlVisibilityProperty)
		'	End Get
		'	Set(Value As Boolean)
		'		SetValue(MappingControlVisibilityProperty, Value)
		'	End Set
		'End Property

		'Public Shared ReadOnly MappingControlVisibilityProperty As PropertyData = RegisterProperty("MappingControlVisibility", GetType(Boolean), Nothing)

		Public Property InputListString() As List(Of String)
			Get
				Return GetValue(Of List(Of String))(InputListStringProperty)
			End Get
			Set(Value As List(Of String))
				SetValue(InputListStringProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly InputListStringProperty As PropertyData = RegisterProperty("InputListString", GetType(List(Of String)), Nothing)

		Public Property InputListStringSelected() As String
			Get
				Return GetValue(Of String)(InputListStringSelectedProperty)
			End Get
			Set(Value As String)
				SetValue(InputListStringSelectedProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly InputListStringSelectedProperty As PropertyData = RegisterProperty("InputListStringSelected", GetType(String), Nothing)

		Public Property AxesStates() As Dictionary(Of String, ARDrone.Input.InputConfigs.InputConfigState)
			Get
				Return GetValue(Of Dictionary(Of String, ARDrone.Input.InputConfigs.InputConfigState))(AxesStatesProperty)
			End Get
			Set(Value As Dictionary(Of String, ARDrone.Input.InputConfigs.InputConfigState))
				SetValue(AxesStatesProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly AxesStatesProperty As PropertyData = RegisterProperty("AxesStates", GetType(Dictionary(Of String, ARDrone.Input.InputConfigs.InputConfigState)), Nothing)

		Public Property ButtonStates() As Dictionary(Of String, ARDrone.Input.InputConfigs.InputConfigState)
			Get
				Return GetValue(Of Dictionary(Of String, ARDrone.Input.InputConfigs.InputConfigState))(ButtonStatesProperty)
			End Get
			Set(Value As Dictionary(Of String, ARDrone.Input.InputConfigs.InputConfigState))
				SetValue(ButtonStatesProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly ButtonStatesProperty As PropertyData = RegisterProperty("ButtonStates", GetType(Dictionary(Of String, ARDrone.Input.InputConfigs.InputConfigState)), Nothing)

		Public Property StateInEdit() As InputConfingInEdit
			Get
				Return GetValue(Of InputConfingInEdit)(StateInEditProperty)
			End Get
			Set(Value As InputConfingInEdit)
				SetValue(StateInEditProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly StateInEditProperty As PropertyData = RegisterProperty("StateInEdit", GetType(InputConfingInEdit), Nothing)





		Public Property ChangeStateVisibility() As Boolean
			Get
				Return GetValue(Of Boolean)(ChangeStateVisibilityProperty)
			End Get
			Set(Value As Boolean)
				SetValue(ChangeStateVisibilityProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly ChangeStateVisibilityProperty As PropertyData = RegisterProperty("ChangeStateVisibility", GetType(Boolean), Nothing)




#End Region

#Region "Vars"
		
		Private _ViewHandle As IntPtr
		Private _InputEngine As ARDrone.Input.InputManager
		Private _InputList As New List(Of ARDrone.Input.ConfigurableInput)
		Private _CurrentInput As ARDrone.Input.ConfigurableInput
		Private _CloseView As Boolean = False

#End Region

#Region "C.Tor"

		Public Sub New()

		End Sub

		Public Sub SetViewHandle(ViewHandle As IntPtr)
			_ViewHandle = ViewHandle
			InitEngines()
			AddHandler Me.PropertyChanged, AddressOf LocalPropertyChanged
		End Sub

		Public Sub New(Init As Boolean, InputEngine As ARDrone.Input.InputManager)
			_InputEngine = InputEngine
		End Sub

		Private Sub InitEngines()
			StartInputEngine()
			CreateInputList()
		End Sub

		Public Sub DeInitEngines()
			StopInputEngine()
			_CloseView = True
		End Sub

		Private Sub StartInputEngine()

			If _InputEngine Is Nothing Then
				_InputEngine = New ARDrone.Input.InputManager(_ViewHandle)


			End If
			_InputEngine.SwitchInputMode(ARDrone.Input.InputManager.InputMode.RawInput)

			AddHandler _InputEngine.RawInputReceived, New ARDrone.Input.Utils.RawInputReceivedHandler(AddressOf _InputEngine_RawInputReceived)
			AddHandler _InputEngine.NewInputDevice, New ARDrone.Input.Utils.NewInputDeviceHandler(AddressOf _InputEngine_NewInputDevice)
			AddHandler _InputEngine.InputDeviceLost, New ARDrone.Input.Utils.InputDeviceLostHandler(AddressOf _InputEngine_InputDeviceLost)
		End Sub

		Private Sub StopInputEngine()
			If _InputEngine IsNot Nothing Then
				_InputEngine.SwitchInputMode(ARDrone.Input.InputManager.InputMode.NoInput)
				RemoveHandler _InputEngine.RawInputReceived, New ARDrone.Input.Utils.RawInputReceivedHandler(AddressOf _InputEngine_RawInputReceived)
				RemoveHandler _InputEngine.NewInputDevice, New ARDrone.Input.Utils.NewInputDeviceHandler(AddressOf _InputEngine_NewInputDevice)
				RemoveHandler _InputEngine.InputDeviceLost, New ARDrone.Input.Utils.InputDeviceLostHandler(AddressOf _InputEngine_InputDeviceLost)
			End If



		End Sub

		Private Sub LocalPropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs)

			Select Case e.PropertyName
				Case Is = "InputListStringSelected"
					If InputListStringSelected Is Nothing OrElse InputListStringSelected = String.Empty Then
						'MappingControlVisibility = False
					Else
						'MappingControlVisibility = True
						For Each N In _InputList
							If N.DeviceName = InputListStringSelected Then
								_CurrentInput = N
								RefreshInputStates(_CurrentInput)
								Exit For
							End If
						Next
					End If

					
			End Select


		End Sub

		Public Function CloseState() As Boolean
			Return _CloseView
		End Function

#End Region

#Region "Commands"

		Public ReadOnly Property SaveStateCommand() As Command
			Get
				Return _SaveStateCommand
			End Get
		End Property

		Private ReadOnly _SaveStateCommand As Command = New Command(AddressOf OnSaveStateCommandExecute)

		Private Sub OnSaveStateCommandExecute()
			SaveStateInput()
			StateInEdit = Nothing
			ChangeStateVisibility = False
		End Sub

		Public ReadOnly Property CancelStateCommand() As Command
			Get
				Return _CancelStateCommand
			End Get
		End Property

		Private ReadOnly _CancelStateCommand As Command = New Command(AddressOf OnCancelStateCommandExecute)

		Private Sub OnCancelStateCommandExecute()
			StateInEdit = Nothing
			ChangeStateVisibility = False
		End Sub

		Public ReadOnly Property ResetStateCommand() As Command
			Get
				Return _ResetStateCommand
			End Get
		End Property

		Private ReadOnly _ResetStateCommand As Command = New Command(AddressOf OnResetStateCommandExecute)

		Private Sub OnResetStateCommandExecute()
			If StateInEdit IsNot Nothing AndAlso ChangeStateVisibility = True Then
				StateInEdit = New InputConfingInEdit(StateInEdit.CurrentState) With {.NewStateMapping = ""}
			End If
		End Sub


		Public ReadOnly Property CancelCommand() As Command
			Get
				Return _CancelCommand
			End Get
		End Property

		Private ReadOnly _CancelCommand As Command = New Command(AddressOf OnCancelCommandExecute)

		Private Sub OnCancelCommandExecute()


			Me.CloseViewModel(True)
		End Sub

		Public ReadOnly Property SaveCommand() As Command
			Get
				Return _SaveCommand
			End Get
		End Property

		Private ReadOnly _SaveCommand As Command = New Command(AddressOf OnSaveCommandExecute)

		Private Sub OnSaveCommandExecute()
			SaveAllStateInput()
			Me.CloseViewModel(True)
		End Sub



		Public ReadOnly Property ResetCommand() As Command
			Get
				Return _ResetCommand
			End Get
		End Property

		Private ReadOnly _ResetCommand As Command = New Command(AddressOf OnResetCommandExecute)

		Private Sub OnResetCommandExecute()
			If _CurrentInput IsNot Nothing Then
				_CurrentInput.SetDefaultMapping()
				_CurrentInput.SaveMapping()
				RefreshInputStates(_CurrentInput)
			End If




		End Sub




		Public ReadOnly Property ChangeInputCommand() As Command(Of Object)
			Get
				Return _ChangeInputCommand
			End Get
		End Property

		Private ReadOnly _ChangeInputCommand As Command(Of Object) = New Command(Of Object)(AddressOf OnChangeInputCommandExecute)

		Private Sub OnChangeInputCommandExecute(Parameter As Object)
			If Parameter IsNot Nothing AndAlso _CurrentInput IsNot Nothing Then
				Dim Kvp As New KeyValuePair(Of String, ARDrone.Input.InputConfigs.InputConfigState)(Parameter, _CurrentInput.InputConfig.States(Parameter))
				StateInEdit = New InputConfingInEdit(Kvp)
				ChangeStateVisibility = True
			End If

		End Sub


#End Region

#Region "Inputs Events"

		Private Sub SaveStateInput()
			If StateInEdit IsNot Nothing AndAlso ChangeStateVisibility = True Then

				_CurrentInput.Mapping.SetControlProperty(StateInEdit.CurrentState.Key, StateInEdit.NewStateMapping)
			
				_CurrentInput.SaveMapping()
				RefreshInputStates(_CurrentInput)
			End If
		End Sub

		Private Sub SaveAllStateInput()
			For Each I In _InputList
				I.SaveMapping()
			Next
		
		End Sub

		Private Sub CreateInputList()
			_InputList = New List(Of ARDrone.Input.ConfigurableInput)

			For Each CD As ARDrone.Input.GenericInput In _InputEngine.InputDevices
				If TypeOf CD Is ARDrone.Input.ConfigurableInput Then
					AddDeviceToDeviceList(CType(CD, ARDrone.Input.ConfigurableInput))
				End If
			Next

			Dim TmpInputListString As New List(Of String)

			For i As Integer = 0 To _InputList.Count - 1
				TmpInputListString.Add(_InputList(i).DeviceName)
			Next i

			InputListString = TmpInputListString

		End Sub

		Private Sub AddDeviceToDeviceList(ByVal CD As ARDrone.Input.ConfigurableInput)
			Dim foundReplacement As Boolean = False
			For i As Integer = 0 To _InputList.Count - 1
				If _InputList(i).DeviceInstanceId = CD.DeviceInstanceId Then
					CD.CopyMappingFrom(_InputList(i))
					_InputList(i) = CD
					foundReplacement = True
					Exit For
				End If
			Next i

			If (Not foundReplacement) Then
				_InputList.Add(CD)
			End If
		End Sub

		Private Sub RefreshInputStates(CurrentInput As ARDrone.Input.ConfigurableInput)
			If CurrentInput Is Nothing OrElse CurrentInput.InputConfig Is Nothing Then Return
			Dim Map As ARDrone.Input.InputMappings.InputMapping = CurrentInput.Mapping
			Dim Cfg As ARDrone.Input.InputConfigs.InputConfig = CurrentInput.InputConfig
			Dim TmpAxesStates As New Dictionary(Of String, ARDrone.Input.InputConfigs.InputConfigState)
			Dim TmpButtonStates As New Dictionary(Of String, ARDrone.Input.InputConfigs.InputConfigState)

			Dim Ch As String = String.Empty
			For Each F In Cfg.States
				Dim K As String = F.Key
				Dim V As ARDrone.Input.InputConfigs.InputConfigState = F.Value
				If F.Key = ARDrone.Input.InputConfigs.InputConfig._AXESHEADER Then Ch = ARDrone.Input.InputConfigs.InputConfig._AXESHEADER : Continue For
				If F.Key = ARDrone.Input.InputConfigs.InputConfig._BUTTONSHEADER Then Ch = ARDrone.Input.InputConfigs.InputConfig._BUTTONSHEADER : Continue For

				If Ch = ARDrone.Input.InputConfigs.InputConfig._AXESHEADER Then
					V.CurrentMapping = GetInputMappingValue(Map, K)
					TmpAxesStates.Add(K, V)

				End If

				If Ch = ARDrone.Input.InputConfigs.InputConfig._BUTTONSHEADER Then
					V.CurrentMapping = GetInputMappingValue(Map, K)
					TmpButtonStates.Add(K, V)
				End If

			Next

			AxesStates = TmpAxesStates
			ButtonStates = TmpButtonStates

		End Sub

		Private Function GetInputMappingValue(mapping As ARDrone.Input.InputMappings.InputMapping, ByVal inputField As String) As String
			Dim mappings As Dictionary(Of String, String) = mapping.Controls.Mappings

			If mappings.ContainsKey(inputField) Then
				Return mappings(inputField)
			End If

			Return "There is no mapping value named '" & inputField & "'"
		End Function

		Private Sub UpdateInputs(ByVal deviceId As String, ByVal inputValue As String, ByVal isContinuousValue As Boolean)


			If _CurrentInput Is Nothing OrElse _CurrentInput.DeviceInstanceId <> deviceId Then Return
			If StateInEdit Is Nothing Then Return

			Dim mappingset As Boolean = False
			Dim SelectedInputConfigState As ARDrone.Input.InputConfigs.ControlInputConfigState = CType(StateInEdit.CurrentState.Value, ARDrone.Input.InputConfigs.ControlInputConfigState)

			If inputValue IsNot Nothing AndAlso IsControlRecognized(inputValue) Then

				If SelectedInputConfigState.InputValueType = ARDrone.Input.InputControls.InputControl.ControlType.ContinuousValue Then
					'''''''''''
					If (Not isContinuousValue) Then
						If String.IsNullOrWhiteSpace(StateInEdit.NewStateMapping) OrElse String.IsNullOrEmpty(StateInEdit.NewStateMapping) Then
							StateInEdit = New InputConfingInEdit(StateInEdit.CurrentState) With {.NewStateMapping = inputValue}
						Else
							If String.IsNullOrWhiteSpace(inputValue) OrElse String.IsNullOrEmpty(inputValue) Then Return
							Dim civ As String = StateInEdit.NewStateMapping
							civ = Replace(civ, InputConfingInEdit._DEFNEWSTATEMAPPING, "")
							If civ.Contains("-") Then
								Dim civs As String() = Split(civ, "-")
								If civ.Contains(inputValue) Then
									If inputValue = civs(0) Then
										civ = civs(1) & "-" & civs(0)
									ElseIf inputValue = civs(1) Then
										civ = civs(1) & "-" & civs(0)
									End If

								Else
									civ = civs(0) & "-" & inputValue
								End If
							Else
								civ = civ & "-" & inputValue
							End If
							StateInEdit = New InputConfingInEdit(StateInEdit.CurrentState) With {.NewStateMapping = civ}
						End If


						mappingset = True

					Else
						StateInEdit = New InputConfingInEdit(StateInEdit.CurrentState) With {.NewStateMapping = inputValue}
						mappingset = True
					End If
					'''''''''''
				ElseIf SelectedInputConfigState.InputValueType = ARDrone.Input.InputControls.InputControl.ControlType.BooleanValue Then
					If (Not isContinuousValue) Then
						If Not (TypeOf SelectedInputConfigState Is ARDrone.Input.InputConfigs.KeyboardAndDeviceInputConfigState) Then
							StateInEdit = New InputConfingInEdit(StateInEdit.CurrentState) With {.NewStateMapping = inputValue}
							mappingset = True
						ElseIf TypeOf SelectedInputConfigState Is ARDrone.Input.InputConfigs.KeyboardAndDeviceInputConfigState Then
							Dim civ As String = StateInEdit.NewStateMapping
							civ = Replace(civ, InputConfingInEdit._DEFNEWSTATEMAPPING, "")
							civ = Replace(civ, inputValue, "")
							civ = civ & inputValue
							StateInEdit = New InputConfingInEdit(StateInEdit.CurrentState) With {.NewStateMapping = civ}
							mappingset = True
						End If
					End If


				End If

			End If

			If mappingset AndAlso SelectedInputConfigState.DisabledOnInput Then
				Dim i As Integer = 0
			End If

		End Sub

		Private Function IsControlRecognized(ByVal inputValue As String) As Boolean
			Dim SelectedInputConfigState As ARDrone.Input.InputConfigs.ControlInputConfigState = CType(StateInEdit.CurrentState.Value, ARDrone.Input.InputConfigs.ControlInputConfigState)

			If TypeOf SelectedInputConfigState Is ARDrone.Input.InputConfigs.DeviceAndSelectionConfigState Then
				Dim selectionState As ARDrone.Input.InputConfigs.DeviceAndSelectionConfigState = CType(SelectedInputConfigState, ARDrone.Input.InputConfigs.DeviceAndSelectionConfigState)
				If selectionState.ControlsNotRecognized.Contains(inputValue) Then
					Return False
				End If
			End If

			Return True
		End Function

		Private Sub _InputEngine_RawInputReceived(sender As Object, e As ARDrone.Input.Utils.RawInputReceivedEventArgs)
			UpdateInputs(e.DeviceId, e.InputString, e.IsAxis)
		End Sub

		Private Sub _InputEngine_NewInputDevice(sender As Object, e As ARDrone.Input.Utils.NewInputDeviceEventArgs)
			CreateInputList()
		End Sub

		Private Sub _InputEngine_InputDeviceLost(sender As Object, e As ARDrone.Input.Utils.InputDeviceLostEventArgs)
			CreateInputList()
		End Sub


#End Region

#Region "Methods"




#End Region

	End Class


End Namespace
