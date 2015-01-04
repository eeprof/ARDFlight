Imports Catel.Collections
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
Imports System.Reflection

Public Class DroneInputCommand
	Inherits ObservableObject

	Private _UpText As String = String.Empty
	Public Property UpText As String
		Get
			Return _UpText
		End Get
		Set(Value As String)
			RaisePropertyChanging(Function() UpText)

			Dim oldValue = _UpText
			_UpText = Value
			RaisePropertyChanged(Function() UpText, oldValue, Value)

		End Set
	End Property

	Private _DownText As String = String.Empty
	Public Property DownText As String
		Get
			Return _DownText
		End Get
		Set(Value As String)
			RaisePropertyChanging(Function() DownText)

			Dim oldValue = _DownText
			_DownText = Value
			RaisePropertyChanged(Function() DownText, oldValue, Value)

		End Set
	End Property

	Private _IsEnabled As Boolean = False
	Public ReadOnly Property IsEnabled As Boolean
		Get
			Return _IsEnabled
		End Get
		
	End Property

	Private _InputCommand As Command
	Public ReadOnly Property InputCommand As Command
		Get
			Return _InputCommand
		End Get
	End Property

	Public Sub New(iUptext As String, iCommand As Command)
		_UpText = iUptext
		_InputCommand = iCommand
	End Sub

	Public Sub New(iUptext As String, iDownText As String, iCommand As Command)
		_UpText = iUptext
		_DownText = iDownText
		_InputCommand = iCommand
	End Sub

	Public Sub CheckIsEnabled()
		If _InputCommand IsNot Nothing Then
			Dim Value As Boolean = False
			If _InputCommand.CanExecute() Then Value = True

		

		RaisePropertyChanging(Function() IsEnabled)

			Dim oldValue = _IsEnabled
			_IsEnabled = Value
			RaisePropertyChanged(Function() IsEnabled, oldValue, Value)


		End If
	End Sub

End Class
