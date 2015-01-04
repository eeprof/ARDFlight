Imports System.Windows.Interactivity
Imports System.ComponentModel

Namespace appbehaviors

#Region "MainWindowView"

	Public Class MainWindowViewLoadedBehavior
		Inherits TargetedTriggerAction(Of MainWindowView)
		Private ClosingHandler As EventHandler(Of CancelEventArgs) = Nothing

		Protected Overrides Sub Invoke(parameter As Object)

			If ClosingHandler Is Nothing Then
				Me.Target.DataContext = New ViewModels.MainWindowViewModel(True, LocalSetting.GetWindowHandle(Me.Target))

				ClosingHandler = New EventHandler(AddressOf Closing)
				AddHandler Me.Target.Closing, Sub(s, e)
												  ClosingHandler(s, e)
											  End Sub

			End If




		End Sub

		Private Sub Closing(sender As Object, e As CancelEventArgs)

			If sender.DataContext IsNot Nothing AndAlso sender.DataContext.GetType = GetType(ViewModels.MainWindowViewModel) Then

				If TryCast(sender.DataContext, ViewModels.MainWindowViewModel).CloseState Then
					e.Cancel = False
				Else
					TryCast(sender.DataContext, ViewModels.MainWindowViewModel).DeInitEngines()
					e.Cancel = Not TryCast(sender.DataContext, ViewModels.MainWindowViewModel).CloseState
				End If

			End If

		End Sub



	End Class

#End Region




End Namespace

