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
Imports Catel.Collections
Imports Catel.MVVM.Services
Imports System.Timers
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports ARDFlight.Common


Namespace ViewModels


	Public Class MainWindowViewModel
		Inherits ViewModelBase

#Region "Properties"


#End Region

#Region "Map Data"

		Public Property MapDataInfoGridList() As MapDataInfoCollection
			Get
				Return GetValue(Of MapDataInfoCollection)(MapDataInfoGridListProperty)
			End Get
			Set(Value As MapDataInfoCollection)
				SetValue(MapDataInfoGridListProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly MapDataInfoGridListProperty As PropertyData = RegisterProperty("MapDataInfoGridList", GetType(MapDataInfoCollection), New MapDataInfoCollection)

#End Region

#Region "Vars"

		Private _ViewHandle As IntPtr
		Private _CloseView As Boolean = False
		Private Const _TUISpan As Integer = 1000
		Private _LastTUISpan As Integer = 0
		Private _tmrCoorUpdate As New Timer(_TUISpan)

		
		

#End Region

#Region "C.Tor"

		Public Sub New()

		End Sub

		Public Sub New(Init As Boolean, ViewHandle As IntPtr)

			_ViewHandle = ViewHandle
			InitEngines()

		End Sub

		Private Sub InitEngines()
			_CloseView = False
			LocalSetting.StartPipeServerEngine(True)
			AddHandler _tmrCoorUpdate.Elapsed, AddressOf tmrCoorUpdate_Elapsed
			_tmrCoorUpdate.Start()
		End Sub

		Public Sub DeInitEngines()
			_tmrCoorUpdate.Stop()
			RemoveHandler _tmrCoorUpdate.Elapsed, AddressOf tmrCoorUpdate_Elapsed
			LocalSetting.StartPipeServerEngine(False)
			_CloseView = True
		End Sub

		Public Function CloseState() As Boolean
			Return _CloseView
		End Function

		Private Sub tmrCoorUpdate_Elapsed(sender As Object, e As ElapsedEventArgs)
			If GlobalMapDataInfoCollection IsNot Nothing Then
				MapDataInfoCollection.UpdateFromMapDataInfoCollection(GlobalMapDataInfoCollection, MapDataInfoGridList)
			End If
		End Sub

#End Region

#Region "Commands"



#End Region

	

	End Class


End Namespace

