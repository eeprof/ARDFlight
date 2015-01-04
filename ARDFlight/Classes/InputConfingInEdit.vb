Public Class InputConfingInEdit

	Public Const _DEFNEWSTATEMAPPING As String = "Press Button"

	Private _CurrentState As KeyValuePair(Of String, ARDrone.Input.InputConfigs.InputConfigState)
	Public ReadOnly Property CurrentState As KeyValuePair(Of String, ARDrone.Input.InputConfigs.InputConfigState)
		Get
			Return _CurrentState
		End Get
	End Property

	Private _CurrentStateName As String = "Not Recognized"
	Public ReadOnly Property CurrentStateName As String
		Get
			Return _CurrentStateName
		End Get
	End Property

	Private _CurrentStateMapping As String = "Not Recognized"
	Public ReadOnly Property CurrentStateMapping As String
		Get
			Return _CurrentStateMapping
		End Get
	End Property

	Public Property NewStateMapping As String = _DEFNEWSTATEMAPPING

	Public Sub New(State As KeyValuePair(Of String, ARDrone.Input.InputConfigs.InputConfigState))

		_CurrentState = State
		_CurrentStateName = State.Value.Name
		_CurrentStateMapping = State.Value.CurrentMapping
		NewStateMapping = State.Value.CurrentMapping
	End Sub

End Class
