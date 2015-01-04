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

				If TryCast(sender.DataContext, ViewModels.MainWindowViewModel).CheckIsConnected Then
					e.Cancel = True
				Else
					TryCast(sender.DataContext, ViewModels.MainWindowViewModel).DeInitEngines()
					e.Cancel = Not TryCast(sender.DataContext, ViewModels.MainWindowViewModel).CloseState
				End If

			End If

		End Sub


	End Class

#End Region

#Region "DroneInfoView"

	Public Class DroneInfoViewLoadedBehavior
		Inherits TargetedTriggerAction(Of DroneInfoView)
		Private ClosingHandler As EventHandler(Of CancelEventArgs) = Nothing

		Protected Overrides Sub Invoke(parameter As Object)

			If ClosingHandler Is Nothing Then
				ClosingHandler = New EventHandler(AddressOf Closing)
				AddHandler Me.Target.Closing, Sub(s, e)
												  ClosingHandler(s, e)
											  End Sub

			End If




		End Sub

		Private Sub Closing(sender As Object, e As CancelEventArgs)

			If sender.DataContext IsNot Nothing AndAlso sender.DataContext.GetType = GetType(ViewModels.DroneInfoViewModel) Then
				e.Cancel = Not TryCast(sender.DataContext, ViewModels.DroneInfoViewModel).CloseState
			End If

		End Sub

	End Class

#End Region

#Region "InputSettingsView"

	Public Class InputSettingsViewLoadedBehavior
		Inherits TargetedTriggerAction(Of InputSettingsView)
		Private ClosingHandler As EventHandler(Of CancelEventArgs) = Nothing

		Protected Overrides Sub Invoke(parameter As Object)

			If ClosingHandler Is Nothing Then
				If Me.Target.DataContext IsNot Nothing AndAlso Me.Target.DataContext.GetType = GetType(ViewModels.InputSettingsViewModel) Then
					TryCast(Me.Target.DataContext, ViewModels.InputSettingsViewModel).SetViewHandle(LocalSetting.GetWindowHandle(Me.Target))
				End If

				ClosingHandler = New EventHandler(AddressOf Closing)
				AddHandler Me.Target.Closing, Sub(s, e)
												  ClosingHandler(s, e)
											  End Sub

			End If




		End Sub

		Private Sub Closing(sender As Object, e As CancelEventArgs)

			If sender.DataContext IsNot Nothing AndAlso sender.DataContext.GetType = GetType(ViewModels.InputSettingsViewModel) Then
				TryCast(sender.DataContext, ViewModels.InputSettingsViewModel).DeInitEngines()
				e.Cancel = Not TryCast(sender.DataContext, ViewModels.InputSettingsViewModel).CloseState
			End If

		End Sub


	End Class

#End Region

#Region "DroneInfoItemControl"

	Public Class DroneInfoItemControlLoadedBehavior
		Inherits TargetedTriggerAction(Of DroneInfoItemControl)

		Protected Overrides Sub Invoke(parameter As Object)

			Dim sourcesBinding As New PriorityBinding
			sourcesBinding.FallbackValue = Nothing

			Dim FallBacksourceBinding As New Binding()
			FallBacksourceBinding.IsAsync = True
			FallBacksourceBinding.Path = New PropertyPath("FallBackSystemDataInfo.Children")

			Dim sourceBinding As New Binding()
			sourceBinding.IsAsync = True
			sourceBinding.Path = New PropertyPath("CurrentSystemDataInfo.Children[" & Me.Target.Dataindex.ToString & "].Children")

			sourcesBinding.Bindings.Add(sourceBinding)
			sourcesBinding.Bindings.Add(FallBacksourceBinding)
			BindingOperations.SetBinding(Me.Target.mainlist, TreeView.ItemsSourceProperty, sourcesBinding)


		End Sub

	


	End Class

#End Region


End Namespace

