Imports Catel.Collections
Imports Catel.Data
Imports System.Globalization


Public Class SystemDataInfo
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

	Private _CValue As Object
	Public Property CValue As Object
		Get
			Return _CValue
		End Get
		Set(Value As Object)
			RaisePropertyChanging(Function() CValue)
			RaisePropertyChanging(Function() CValueString)

			Dim oldValue = _CValue
			Dim oldValuestr = CValueString

			_CValue = Value

			RaisePropertyChanged(Function() CValue, oldValue, Value)
			RaisePropertyChanged(Function() CValueString, oldValuestr, CValueString)

		End Set
	End Property
	Public ReadOnly Property CValueString As String
		Get
			Dim Nval As String = ""
			If Not CValue Is Nothing Then
				Try
					If IsNumeric(CValue) Then
						Nval = CSng(CValue).ToString(CultureInfo.InvariantCulture) 'String.Format("{0:#.#}", CSng(CValue))
					Else
						Nval = CValue.ToString
					End If
				Catch ex As Exception
					Nval = CValue.ToString
				End Try

			End If
			Return Nval
		End Get
	End Property

	Private _Children As New List(Of SystemDataInfo)
	Public ReadOnly Property Children As IList(Of SystemDataInfo)
		Get
			Return _Children
		End Get
	End Property

	Private IntDic As New Dictionary(Of String, Integer)

	Public Sub New()

	End Sub

	Public Sub New(iName As String)
		_KeyName = iName
		_Name = iName
	End Sub

	Public Sub New(iName As String, iValue As Object)
		_KeyName = iName
		_Name = iName
		_CValue = iValue
	End Sub

	Public Sub Add(NewItem As SystemDataInfo)
		If Not IntDic.ContainsKey(NewItem.KeyName) Then
			_Children.Add(NewItem)
			IntDic.Add(NewItem.KeyName, _Children.Count - 1)
		Else
			_Children(IntDic(NewItem.KeyName)).Add(NewItem)
		End If
	End Sub

	Public Sub Update(iKeyName As String, iValue As Object)

		If _KeyName = iKeyName Then
			If iValue IsNot Nothing Then CValue = iValue
		Else
			If IntDic.ContainsKey(iKeyName) Then
				_Children(IntDic(iKeyName)).Update(iKeyName, iValue)
			Else
				For Each I In _Children
					I.Update(iKeyName, iValue)
				Next
			End If

		End If


	End Sub

	Public Shared Function GetWaitSystemDataInfo() As SystemDataInfo
		Dim result As New SystemDataInfo("Wait Data...")
		result.Add(New SystemDataInfo("Wait Data..."))
		Return result
	End Function

End Class
