

Imports Catel.Collections
Imports Catel.Data
Imports System.Globalization
Imports System.Reflection



Public Class MapDataInfo
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

Public Enum MapDataInfoField As Integer

	Latitude
	Longitude
	Elevation

End Enum


Public Class MapDataInfoCollection
	Inherits FastObservableCollection(Of MapDataInfo)

	Private _NameKeyDic As New Dictionary(Of String, String)
	Private _InternalDic As New Dictionary(Of Integer, Integer)


	Public Sub New()

		MakeNameKeyDic()
		MakeNavigationDataInfoCollection()

	End Sub

	Private Sub MakeNameKeyDic()

		_NameKeyDic.Add("Latitude", "Latitude : ")
		_NameKeyDic.Add("Longitude", "Longitude: ")
		_NameKeyDic.Add("Altitude", "Altitude : ")

	End Sub

	Private Sub MakeNavigationDataInfoCollection()
		Dim items As Array
		items = System.Enum.GetValues(GetType(MapDataInfoField))

		Dim item As Integer
		For Each item In items
			Dim niv As MapDataInfoField = item
			AddItem(niv) 

		Next
	End Sub

	Private Sub AddItem(niv As MapDataInfoField)
		Dim nit As String = niv.ToString
		If _NameKeyDic.ContainsKey(niv.ToString) Then nit = _NameKeyDic(niv.ToString)
		Dim nv As New MapDataInfo With {.KeyName = niv.ToString, .Name = nit, .CValue = Nothing, .Index = Me.Count}
		If Not _InternalDic.ContainsKey(CInt(niv)) Then _InternalDic.Add(CInt(niv), Me.Count)
		Me.Add(nv)
	End Sub

	Public Sub UpdateValue(Field As MapDataInfoField, Value As Object)

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

	Public Shared Function ToXElement(iMapDataInfoCollection As MapDataInfoCollection) As XElement
		Dim Xele As New XElement(iMapDataInfoCollection.GetType.Name)

		For i = 0 To iMapDataInfoCollection.Count - 1
			Dim O As MapDataInfo = iMapDataInfoCollection(i)
			Dim Oele As New XElement(O.GetType.Name)
			For Each Prop In O.GetType.GetProperties()
				Dim iName As String = Prop.Name
				Dim iValue As Object = Prop.GetValue(O, Nothing)


				If IsNumeric(iValue) Then
					Dim Valuestr As String = CDbl(iValue).ToString(CultureInfo.InvariantCulture)
					Dim Natt As New XAttribute(iName, Valuestr)
					Oele.Add(Natt)
				Else
					If iValue Is Nothing Then iValue = "0.0"
					Dim Natt As New XAttribute(iName, iValue.ToString)
					Oele.Add(Natt)
				End If


			Next

			Xele.Add(Oele)

		Next

		Return Xele
	End Function

	Public Shared Function FromXElement(iXele As String) As MapDataInfoCollection

		Dim result As MapDataInfoCollection = Nothing

		Try
			Dim Xele As XElement = XElement.Parse(iXele)
			If Xele.Name = GetType(MapDataInfoCollection).Name Then
				result = FromXElement(Xele)
			End If
		Catch ex As Exception

		End Try

		Return result


	End Function

	Public Shared Function FromXElement(iXele As XElement) As MapDataInfoCollection
		Dim result As MapDataInfoCollection = Nothing

		If iXele.Name = GetType(MapDataInfoCollection).Name Then

			For Each ele As XElement In iXele.Elements
				Dim O As New MapDataInfo

				For Each Att In ele.Attributes
					Dim prop As PropertyInfo = O.GetType.GetProperty(Att.Name.ToString)
					If prop IsNot Nothing Then

						Dim Number As Object = NumberConversion(Att.Value.ToString, prop)

						If Number IsNot Nothing Then
							prop.SetValue(O, Number)
						Else
							prop.SetValue(O, Att.Value.ToString)
						End If

					End If
				Next

				If result Is Nothing Then result = New MapDataInfoCollection : result.Clear()

				result.Add(O)

			Next


		End If

		Return result
	End Function

	Private Shared Function NumberConversion(InValue As Object, P As PropertyInfo) As Object

		Try

			If P.PropertyType.Name = "Double" Then Return Double.Parse(InValue, CultureInfo.InvariantCulture)
			If P.PropertyType.Name.Contains("Int") Then Return Integer.Parse(InValue, CultureInfo.InvariantCulture)
			If P.PropertyType.Name.Contains("Sing") Then Return Single.Parse(InValue, CultureInfo.InvariantCulture)

		Catch ex As Exception

		End Try

		Return Nothing

	End Function

	Public Shared Sub UpdateFromMapDataInfoCollection(ByVal InMapDataInfoCollection As MapDataInfoCollection, ByRef OutMapDataInfoCollection As MapDataInfoCollection)


		Try

			For i = 0 To InMapDataInfoCollection.Count - 1
				Try
					Dim fi As MapDataInfoField = [Enum].Parse(GetType(MapDataInfoField), InMapDataInfoCollection(i).KeyName)

					If InMapDataInfoCollection(i).IsNumericValue = Costants._YES Then
						OutMapDataInfoCollection.UpdateValue(fi, InMapDataInfoCollection(i).NumValue)
					Else
						OutMapDataInfoCollection.UpdateValue(fi, InMapDataInfoCollection(i).CValue)
					End If


				Catch ex As Exception

				End Try


			Next

	
		Catch ex As Exception

		End Try


	End Sub

End Class
