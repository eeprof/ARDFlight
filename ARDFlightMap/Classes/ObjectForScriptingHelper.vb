Imports System.Reflection
Imports System.Security.Permissions
Imports System.Runtime.InteropServices

<PermissionSet(SecurityAction.Demand, Name:="FullTrust")> _
<ComVisible(True)> _
Public Class ObjectForScriptingHelper
	Implements iMapEventHandler





	Public Sub New(ByRef ParentObject As Object)
		Dim ObjectForScriptingEvents As EventInfo() = GetType(ObjectForScriptingHelper).GetEvents

		For Each EV As EventInfo In ObjectForScriptingEvents
			Dim EveName As String = EV.Name
			Dim CallName As String = "Eve_" & EveName

			Dim Evi As EventInfo = Me.GetType.GetEvent(EveName)
			Dim Meth As MethodInfo = ParentObject.GetType.GetMethod(CallName)

			If Evi Is Nothing Then Continue For
			If Meth Is Nothing Then Continue For


			Dim EviDelegate As Type = Evi.EventHandlerType
			Dim d As [Delegate] = [Delegate].CreateDelegate(EviDelegate, ParentObject, Meth)

			Evi.AddEventHandler(Me, d)

		Next

	End Sub

	Public Event click(lat As Double, lng As Double) Implements iMapEventHandler.click

	Public Sub Eve_click(lat As Double, lng As Double) Implements iMapEventHandler.Eve_click
		RaiseEvent click(lat, lng)
	End Sub

	Public Event idle() Implements iMapEventHandler.idle

	Public Sub Eve_Idle() Implements iMapEventHandler.Eve_idle
		RaiseEvent idle()
	End Sub

	Public Event mousemove(lat As Double, lng As Double) Implements iMapEventHandler.mousemove

	Public Sub Eve_MouseMove(lat As Double, lng As Double) Implements iMapEventHandler.Eve_mousemove
		RaiseEvent mousemove(lat, lng)
	End Sub

	Public Event completed() Implements iMapEventHandler.completed

	Public Sub Eve_completed() Implements iMapEventHandler.Eve_completed
		RaiseEvent completed()
	End Sub

	Public Event merkeradded(MarkerName As String, MarkerIndex As Integer) Implements iMapEventHandler.merkeradded

	Public Sub Eve_merkeradded(MarkerName As String, MarkerIndex As Integer) Implements iMapEventHandler.Eve_merkeradded
		RaiseEvent merkeradded(MarkerName, MarkerIndex)
	End Sub

	Public Event merkermoved(MarkerName As String, lat As Double, lng As Double) Implements iMapEventHandler.merkermoved

	Public Sub Eve_merkermoved(MarkerName As String, lat As Double, lng As Double) Implements iMapEventHandler.Eve_merkermoved
		RaiseEvent merkermoved(MarkerName, lat, lng)
	End Sub

End Class
