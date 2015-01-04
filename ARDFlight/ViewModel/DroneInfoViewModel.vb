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
Imports ARDFlight.Common


Namespace ViewModels

	Public Class DroneInfoViewModel
		Inherits ViewModelBase

#Region "Properties"


		Public Property CurrentSystemDataInfo() As SystemDataInfo
			Get
				Return GetValue(Of SystemDataInfo)(CurrentSystemDataInfoProperty)
			End Get
			Set(Value As SystemDataInfo)
				SetValue(CurrentSystemDataInfoProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly CurrentSystemDataInfoProperty As PropertyData = RegisterProperty("CurrentSystemDataInfo", GetType(SystemDataInfo), Nothing)



		Public Property FallBackSystemDataInfo() As SystemDataInfo
			Get
				Return GetValue(Of SystemDataInfo)(FallBackSystemDataInfoProperty)
			End Get
			Set(Value As SystemDataInfo)
				SetValue(FallBackSystemDataInfoProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly FallBackSystemDataInfoProperty As PropertyData = RegisterProperty("FallBackSystemDataInfo", GetType(SystemDataInfo), SystemDataInfo.GetWaitSystemDataInfo)

		Public Property DroneInfoLinks() As LinkCollection
			Get
				Return GetValue(Of LinkCollection)(DroneInfoLinksProperty)
			End Get
			Set(Value As LinkCollection)
				SetValue(DroneInfoLinksProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly DroneInfoLinksProperty As PropertyData = RegisterProperty("DroneInfoLinks", GetType(LinkCollection), Nothing)

		Public Property SelectedLink() As Uri
			Get
				Return GetValue(Of Uri)(SelectedLinkProperty)
			End Get
			Set(Value As Uri)
				SetValue(SelectedLinkProperty, Value)
			End Set
		End Property

		Public Shared ReadOnly SelectedLinkProperty As PropertyData = RegisterProperty("SelectedLink", GetType(Uri), Nothing)








#End Region

#Region "Vars"

		Private _CloseView As Boolean = False
		Private _droneClient As DroneClient
		Private _settings As Settings


#End Region

#Region "C.Tor"

		Public Sub New()

		End Sub

		Public Sub New(Init As Boolean, CurrentDroneClient As DroneClient)
			_droneClient = CurrentDroneClient
			If Init Then
				'	ReadInfo()
			End If

		End Sub


		Public Function CloseState() As Boolean
			Return _CloseView
		End Function



#End Region

#Region "Commands"

		Public ReadOnly Property ReadInfoCommand() As Command
			Get
				Return _ReadInfoCommand
			End Get
		End Property

		Private ReadOnly _ReadInfoCommand As Command = New Command(AddressOf OnReadInfoCommandExecute)

		Private Sub OnReadInfoCommandExecute()
			ReadInfo()

		End Sub

		Public ReadOnly Property CloseCommand() As Command
			Get
				Return _CloseCommand
			End Get
		End Property

		Private ReadOnly _CloseCommand As Command = New Command(AddressOf OnCloseCommandExecute)

		Private Sub OnCloseCommandExecute()
			_CloseView = True
			CloseViewModel(True)
		End Sub


#End Region

#Region "Methods"

		Private Sub ReadInfo()
			Dim configurationTask As Task(Of Settings) = _droneClient.GetConfigurationTask()
			configurationTask.ContinueWith(AddressOf ReadInfoTask)
			configurationTask.Start()
		End Sub

		Private Sub ReadInfoTask(ByVal task As Task(Of Settings))
			If task.Exception IsNot Nothing Then
				Trace.TraceWarning("Get configuration task is faulted with exception: {0}", task.Exception.InnerException.Message)
				LocalSetting.UpdateMonitorTextCommand.Execute("Get configuration task is faulted with exception: " & task.Exception.InnerException.Message)
				Return
			End If
			_settings = task.Result
			CreateInfoData(_settings)


		End Sub

		Private Sub CreateInfoData(CurrentSettings As Settings)

			Dim SysInfo As New SystemDataInfo("Configuration")
			Dim ExtendedTagCounter As Integer = 0
			DumpBranch(SysInfo, CurrentSettings)
			CurrentSystemDataInfo = SysInfo

			Dim Ilinks As New LinkCollection
			Dim IIlink As Uri = Nothing
			Dim cCounter As Integer = 0
			For Each ci In SysInfo.Children
				If SelectedLink Is Nothing AndAlso IIlink Is Nothing Then IIlink = New Uri("/" & cCounter.ToString, UriKind.RelativeOrAbsolute)
				Dim iLink As New Link
				iLink.Source = New Uri("/" & cCounter.ToString, UriKind.RelativeOrAbsolute)
				iLink.DisplayName = ci.KeyName
				Ilinks.Add(iLink)
				cCounter += 1
			Next

			DroneInfoLinks = Ilinks
			If IIlink IsNot Nothing Then SelectedLink = IIlink

		End Sub

		Private Sub DumpBranch(ByRef Xele As SystemDataInfo, ByVal o As Object)
			Dim type As Type = o.GetType()

			For Each fieldInfo As FieldInfo In type.GetFields()

				Dim XAtt As New SystemDataInfo(fieldInfo.Name)
				Dim value As Object = fieldInfo.GetValue(o)


				DumpValue(fieldInfo.FieldType, Xele, XAtt, value)
			Next fieldInfo

			For Each propertyInfo As PropertyInfo In type.GetProperties()
				Dim XAtt As New SystemDataInfo(propertyInfo.Name)
				Dim value As Object = propertyInfo.GetValue(o, Nothing)

				DumpValue(propertyInfo.PropertyType, Xele, XAtt, value)
			Next propertyInfo
		End Sub

		Private Sub DumpValue(ByVal type As Type, ByRef CurrentTag As SystemDataInfo, ByRef CurrentAtt As SystemDataInfo, ByVal value As Object)
			If value Is Nothing Then
				CurrentAtt.CValue = "novalue"
				CurrentTag.Add(CurrentAtt)
			Else
				If type.Namespace.StartsWith("System") OrElse type.IsEnum Then

					If type.Name.ToUpper.Contains("Dictionary".ToUpper) Then
						Dim DicOfTags As New Dictionary(Of String, List(Of Object()))
						Dim NewTagName As String = CurrentTag.Name.ToString & "_Extended"
						Dim ExtendedTag As New SystemDataInfo(NewTagName)

						For Each KV In value
							Dim splkv As String() = Split(KV.Key.ToString, ":")
							Dim KeyName As String = splkv(0)
							Dim KeyProp As String = splkv(1)
							Dim KeyVal As Object = KV.Value
							If Not DicOfTags.ContainsKey(KeyName) Then DicOfTags.Add(KeyName, New List(Of Object()))
							DicOfTags(KeyName).Add(New Object() {KeyProp, KeyVal})
						Next

						For Each T In DicOfTags
							Dim ntag As New SystemDataInfo(T.Key)
							For Each i In T.Value
								Dim XAtt As New SystemDataInfo(i(0), i(1))
								ntag.Add(XAtt)
							Next
							ExtendedTag.Add(ntag)
						Next

						CurrentTag.Add(ExtendedTag)
					ElseIf type.Name.ToUpper.Contains("Concurrent".ToUpper) Then
						'For Each KV In value
						'	Dim XAtt As New XAttribute(Replace(KV.Key.ToString, ":", "_"), KV.Value.ToString)
						'	CurrentTag.Add(XAtt)
						'Next
					Else
						CurrentAtt.CValue = value
						CurrentTag.Add(CurrentAtt)
					End If

				Else
					Dim NewXele As New SystemDataInfo(CurrentAtt.Name)
					DumpBranch(NewXele, value)
					CurrentTag.Add(NewXele)
				End If
			End If
		End Sub

		Private Sub DumpBranchXml(ByRef Xele As XElement, ByVal o As Object, ExtendedTagCounter As Integer)
			Dim type As Type = o.GetType()

			For Each fieldInfo As FieldInfo In type.GetFields()

				Dim XAtt As New XElement(fieldInfo.Name)
				Dim value As Object = fieldInfo.GetValue(o)


				DumpValueXml(fieldInfo.FieldType, Xele, XAtt, value, ExtendedTagCounter)
			Next fieldInfo

			For Each propertyInfo As PropertyInfo In type.GetProperties()
				Dim XAtt As New XElement(propertyInfo.Name)	'	Dim node As TreeNode = nodes.GetOrCreate(propertyInfo.Name)
				Dim value As Object = propertyInfo.GetValue(o, Nothing)

				DumpValueXml(propertyInfo.PropertyType, Xele, XAtt, value, ExtendedTagCounter)	 'DumpValue(propertyInfo.PropertyType, node, value)
			Next propertyInfo
		End Sub

		Private Sub DumpValueXml(ByVal type As Type, ByRef CurrentTag As XElement, ByRef CurrentAtt As XElement, ByVal value As Object, ByRef ExtendedTagCounter As Integer)
			If value Is Nothing Then
				CurrentAtt.Value = "novalue"
				CurrentTag.Add(CurrentAtt)
			Else
				If type.Namespace.StartsWith("System") OrElse type.IsEnum Then



					If type.Name.ToUpper.Contains("Dictionary".ToUpper) Then
						Dim DicOfTags As New Dictionary(Of String, List(Of String()))
						Dim NewTagName As String = CurrentTag.Name.ToString & "_Extended"
						Dim ExtendedTag As XElement

						If CurrentTag.Elements(NewTagName).Any() Then
							ExtendedTag = CurrentTag.Elements(NewTagName)
						Else
							ExtendedTag = New XElement(NewTagName)
						End If


						For Each KV In value
							Dim splkv As String() = Split(KV.Key.ToString, ":")
							Dim KeyName As String = splkv(0)
							Dim KeyProp As String = splkv(1)
							Dim KeyVal As String = KV.Value.ToString
							If Not DicOfTags.ContainsKey(KeyName) Then DicOfTags.Add(KeyName, New List(Of String()))
							DicOfTags(KeyName).Add(New String() {KeyProp, KeyVal})
						Next

						For Each T In DicOfTags
							Dim ntag As New XElement(T.Key)
							For Each i In T.Value
								Dim XAtt As New XElement(i(0), i(1))
								ntag.Add(XAtt)
							Next
							ExtendedTag.Add(ntag)
						Next
						ExtendedTagCounter += 1
						CurrentTag.Add(ExtendedTag)
					ElseIf type.Name.ToUpper.Contains("Concurrent".ToUpper) Then
						'For Each KV In value
						'	Dim XAtt As New XAttribute(Replace(KV.Key.ToString, ":", "_"), KV.Value.ToString)
						'	CurrentTag.Add(XAtt)
						'Next
					Else
						CurrentAtt.Value = value.ToString
						CurrentTag.Add(CurrentAtt)
					End If

				Else
					Dim NewXele As New XElement(CurrentAtt.Name)
					DumpBranchXml(NewXele, value, ExtendedTagCounter)
					CurrentTag.Add(NewXele)
				End If
			End If
		End Sub



#End Region

	End Class

End Namespace

