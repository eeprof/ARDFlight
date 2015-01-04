Imports Catel.Collections
Imports Catel.Data



Public Class NavigationDataInfo
	Inherits ObservableObject


	Private _IsNumericValue As String = Costants._NO
	Public Property IsNumericValue As String
		Get
			Return _IsNumericValue
		End Get
		Set(Value As String)
			RaisePropertyChanging(Function() IsNumericValue)

			Dim oldValue = _IsNumericValue
			_IsNumericValue = Value
			RaisePropertyChanged(Function() IsNumericValue, oldValue, Value)

		End Set
	End Property

	Private _KeyName As String = String.Empty
	Public Property KeyName As String
		Get
			Return _KeyName
		End Get
		Set(Value As String)
			RaisePropertyChanging(Function() KeyName)

			Dim oldValue = _KeyName
			_KeyName = Value
			RaisePropertyChanged(Function() KeyName, oldValue, Value)

		End Set
	End Property

	Private _Name As String = String.Empty
	Public Property Name As String
		Get
			Return _Name
		End Get
		Set(Value As String)
			RaisePropertyChanging(Function() Name)

			Dim oldValue = _Name
			_Name = Value
			RaisePropertyChanged(Function() Name, oldValue, Value)

		End Set
	End Property

	Private _CValue As String
	Public Property CValue As String
		Get
			Return _CValue
		End Get
		Set(Value As String)
			RaisePropertyChanging(Function() CValue)

			Dim oldValue = _CValue
			_CValue = Value
			RaisePropertyChanged(Function() CValue, oldValue, Value)

		End Set
	End Property

	Private _NumValue As Double = CDbl(0.0)
	Public Property NumValue As Double
		Get
			Return _NumValue
		End Get
		Set(Value As Double)
			RaisePropertyChanging(Function() NumValue)

			Dim oldValue = _NumValue
			_NumValue = Value
			RaisePropertyChanged(Function() NumValue, oldValue, Value)

		End Set
	End Property

	Private _Index As Integer = 0
	Public Property Index As Integer
		Get
			Return _Index
		End Get
		Set(Value As Integer)
			RaisePropertyChanging(Function() Index)

			Dim oldValue = _Index
			_Index = Value
			RaisePropertyChanged(Function() Index, oldValue, Value)

		End Set
	End Property


	Public Sub New()

	End Sub

End Class

Public Enum NavigationDataField As Integer
	DroneClientIsActive
	NavigationDataState
	NavigationDataBatteryLow
	NavigationDataWifiLinkQuality
	BatteryPercentage
	BatteryVoltage
	Roll
	Pitch
	Yaw
	Altitude
	Latitude
	Longitude
	Elevation

End Enum

Public Enum NavigationDataType As Integer
	Empty
	GridData
	HudData
	All
End Enum

Public Class NavigationDataInfoCollection
	Inherits FastObservableCollection(Of NavigationDataInfo)

	Private _NameKeyDic As New Dictionary(Of String, String)
	Private _InternalDic As New Dictionary(Of Integer, Integer)
	Private _NavInfotype As NavigationDataType

	Public Sub New(iNavInfotype As NavigationDataType)

		_NavInfotype = iNavInfotype
		MakeNameKeyDic()
		MakeNavigationDataInfoCollection()

	End Sub

	Private Sub MakeNameKeyDic()

		Select Case _NavInfotype
			Case NavigationDataType.GridData
				_NameKeyDic.Add("DroneClientIsActive", "Connected")
				_NameKeyDic.Add("NavigationDataBatteryLow", "Battery Low")
				_NameKeyDic.Add("BatteryPercentage", "Battery %")
				_NameKeyDic.Add("BatteryVoltage", "Battery V")
				_NameKeyDic.Add("NavigationDataState", "Drone State")
				_NameKeyDic.Add("NavigationDataWifiLinkQuality", "WiFi Qual.")

			Case NavigationDataType.HudData
				_NameKeyDic.Add("Latitude", "LA: ")
				_NameKeyDic.Add("Longitude", "LO: ")
				_NameKeyDic.Add("Altitude", "A: ")
				_NameKeyDic.Add("Elevation", "EL: ")
				_NameKeyDic.Add("Roll", "R: ")
				_NameKeyDic.Add("Pitch", "P: ")
				_NameKeyDic.Add("Yaw", "Y: ")



		End Select

	End Sub

	Private Sub MakeNavigationDataInfoCollection()
		Dim items As Array
		items = System.Enum.GetValues(GetType(NavigationDataField))

		Dim item As Integer
		For Each item In items
			Dim niv As NavigationDataField = item

			If _NavInfotype = NavigationDataType.GridData AndAlso Not CheckIfHudField(niv) Then AddItem(niv)
			If _NavInfotype = NavigationDataType.HudData AndAlso CheckIfHudField(niv) Then AddItem(niv)

		Next
	End Sub

	Private Function CheckIfHudField(niv As NavigationDataField) As Boolean
		Return niv = NavigationDataField.Latitude _
			  Or niv = NavigationDataField.Longitude _
			  Or niv = NavigationDataField.Elevation _
			  Or niv = NavigationDataField.Roll _
			  Or niv = NavigationDataField.Pitch _
			  Or niv = NavigationDataField.Yaw _
			  Or niv = NavigationDataField.Altitude
	End Function

	Private Sub AddItem(niv As NavigationDataField)
		Dim nit As String = niv.ToString
		If _NameKeyDic.ContainsKey(niv.ToString) Then nit = _NameKeyDic(niv.ToString)
		Dim nv As New NavigationDataInfo With {.KeyName = niv.ToString, .Name = nit, .CValue = Nothing, .Index = Me.Count}
		If Not _InternalDic.ContainsKey(CInt(niv)) Then _InternalDic.Add(CInt(niv), Me.Count)
		Me.Add(nv)
	End Sub

	Public Sub UpdateValue(Field As NavigationDataField, Value As Object)

		If Value Is Nothing Then Return
		If Not _InternalDic.ContainsKey(CInt(Field)) Then Return

		Dim Index As Integer = _InternalDic(CInt(Field))

		If IsNumeric(Value) Then
			Me(Index).IsNumericValue = Costants._YES
			Me(Index).CValue = String.Format("{0:0.000}", Value)
			Me(Index).NumValue = CDbl(Value)
		Else
			Me(Index).IsNumericValue = Costants._NO
			Me(Index).CValue = Value.ToString
			Me(Index).NumValue = Double.NaN
		End If

	End Sub

	Public Sub UpdateValue(Index As Integer, Value As Object)

		If Value Is Nothing Then Return
		If Not (Me.Count - 1) >= Index Then Return

		If IsNumeric(Value) Then
			Me(Index).IsNumericValue = Costants._YES
			Me(Index).CValue = String.Format("{0:0.000}", Value)
			Me(Index).NumValue = CDbl(Value)
		Else
			Me(Index).IsNumericValue = Costants._NO
			Me(Index).CValue = Value.ToString
			Me(Index).NumValue = Double.NaN
		End If

	End Sub

End Class
