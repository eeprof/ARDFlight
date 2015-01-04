Public Class InputsHelper

	Public Const _ROLL As String = "ROLL"
	Public Const _PITCH As String = "PITCH"
	Public Const _YAW As String = "YAW"
	Public Const _GAZ As String = "GAZ"

	Public Shared Function GetAxisProgressModeValues() As Dictionary(Of String, Single)
		Dim result As New Dictionary(Of String, Single)
		result.Add(_ROLL, 0.05F)
		result.Add(_PITCH, 0.05F)
		result.Add(_YAW, 0.25F)
		result.Add(_GAZ, 0.25F)
		Return result
	End Function

	Public Shared Function GetAxisOffsetValues() As Dictionary(Of String, Single)
		Dim result As New Dictionary(Of String, Single)
		result.Add(_ROLL, 0.2F)
		result.Add(_PITCH, 0.2F)
		result.Add(_YAW, 0.2F)
		result.Add(_GAZ, 0.2F)
		Return result
	End Function

	Public Shared Function GetAxisGainValues() As Dictionary(Of String, Single)
		Dim result As New Dictionary(Of String, Single)
		result.Add(_ROLL, 1.0F)
		result.Add(_PITCH, 1.0F)
		result.Add(_YAW, 2.0F)
		result.Add(_GAZ, 2.0F)
		Return result
	End Function

End Class
