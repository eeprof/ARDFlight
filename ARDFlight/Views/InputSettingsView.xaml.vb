Imports FirstFloor.ModernUI.Windows.Controls
Imports Catel.MVVM
Imports Catel
Imports System.ComponentModel
Imports Catel.Windows.Controls.MVVMProviders

Public Class InputSettingsView
	Inherits ModernWindow
	Implements IViewModelContainer

#Region "Events"

	Public Event ViewModelChanged As EventHandler(Of EventArgs) Implements IViewModelContainer.ViewModelChanged
	Public Event PropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Implements ComponentModel.INotifyPropertyChanged.PropertyChanged
	Public Event ViewModelPropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Implements IViewModelContainer.ViewModelPropertyChanged
	Public Event ViewLoaded(sender As Object, e As EventArgs) Implements IViewModelContainer.ViewLoaded
	Public Event ViewLoading(sender As Object, e As EventArgs) Implements IViewModelContainer.ViewLoading
	Public Event ViewUnloaded(sender As Object, e As EventArgs) Implements IViewModelContainer.ViewUnloaded
	Public Event ViewUnloading(sender As Object, e As EventArgs) Implements IViewModelContainer.ViewUnloading

#End Region

#Region "Properties"

	Private mvvmBehavior As New WindowBehavior With {.ViewModelType = GetType(ViewModels.InputSettingsViewModel)}

	Public ReadOnly Property ViewModel As IViewModel Implements IViewModelContainer.ViewModel
		Get
			Return mvvmBehavior.ViewModel
		End Get
	End Property


#End Region

#Region "C.tor"

	Public Sub New()
		InitializeComponent()

		System.Windows.Interactivity.Interaction.GetBehaviors(Me).Add(mvvmBehavior)

		AddHandler mvvmBehavior.ViewModelChanged, Sub(sender, e)
															RaiseEvent ViewModelChanged(sender, e)
														End Sub
		AddHandler mvvmBehavior.ViewModelPropertyChanged, Sub(sender, e)
																	RaiseEvent ViewModelPropertyChanged(sender, e)
																End Sub



	End Sub

	Protected Overrides Sub OnPropertyChanged(ByVal e As DependencyPropertyChangedEventArgs)
		MyBase.OnPropertyChanged(e)

		RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(e.Property.Name))
	End Sub

	

	

#End Region

End Class

