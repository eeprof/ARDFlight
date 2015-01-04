
Imports System.Reflection
Imports FirstFloor.ModernUI.Presentation
Imports System.ComponentModel.Composition.Hosting
Imports Catel.MVVM
Imports ARDFlight.Common

Module LocalSetting

#Region "Global Vars"

	Public AppWindow As MainWindowView
	Public _AppModule As Application
	Public ReadOnly App_Copyright As String = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).LegalCopyright.ToString()
	Public GlobalMapDataInfoCollection As New MapDataInfoCollection

	Private _MapSever As MapPipeServer = Nothing


#End Region

#Region "Theme Manager"

	Public Sub SetTheme()

		LocalSetting.SetThemeSource(My.Settings.SelectedThemeSource, False)

		LocalSetting.SetThemeFontSize(My.Settings.SelectedThemeFontSize)

		LocalSetting.SetThemeColor(Color.FromArgb(&HFF, &HAD, &HFF, &H2F))	' "#FFADFF2F") '.Settings.SelectedThemeColor


	End Sub

	Public Sub SetThemeSource(Source As Uri, Optional SaveSetting As Boolean = True)

		'If Source Is Nothing Then
		Source = AppearanceManager.DarkThemeSource
		SaveSetting = True
		'	End If

		AppearanceManager.Current.ThemeSource = Source

		My.Settings.SelectedThemeSource = Source
		If SaveSetting Then My.Settings.Save()

	End Sub

	Public Sub SetThemeFontSize(xFontSize As String, Optional SaveSetting As Boolean = True)

		If String.IsNullOrEmpty(xFontSize) OrElse String.IsNullOrWhiteSpace(xFontSize) Then
			xFontSize = AppearanceManager.Current.FontSize.ToString
			SaveSetting = True
		End If

		If xFontSize = FontSize.Large.ToString Then AppearanceManager.Current.FontSize = FontSize.Large
		If xFontSize = FontSize.Small.ToString Then AppearanceManager.Current.FontSize = FontSize.Small

		My.Settings.SelectedThemeFontSize = xFontSize
		If SaveSetting Then My.Settings.Save()

	End Sub

	Public Sub SetThemeColor(Colorx As Color, Optional SaveSetting As Boolean = True)

		If Colorx = Nothing OrElse Colorx.ToString = "#00000000" Then
			Colorx = AppearanceManager.Current.AccentColor
			SaveSetting = True
		End If

		AppearanceManager.Current.AccentColor = Colorx

		My.Settings.SelectedThemeColor = Colorx
		If SaveSetting Then My.Settings.Save()

	End Sub


#End Region

#Region "Start Application Window"

	Public Sub StartHub(ByVal AppModule As Application)

		_AppModule = AppModule
		_AppModule.ShutdownMode = ShutdownMode.OnLastWindowClose

		If AppWindow Is Nothing Then

			LocalSetting.SetTheme()

			AppWindow = New MainWindowView
			AddHandler AppWindow.Closed, AddressOf AppWindow_Closed
			AppWindow.Show()

		Else

			AppWindow.Show()
			AppWindow.Activate()

		End If


	End Sub

	Private Sub AppWindow_Closed(sender As Object, e As EventArgs)
		Environment.Exit(0)
	End Sub

	Public Function GetWindowHandle(ByVal window As Window) As IntPtr
		Dim helper As New System.Windows.Interop.WindowInteropHelper(window)
		Return helper.Handle
	End Function

#End Region

#Region "Pipe Server Methods"

	Public Sub StartPipeServerEngine(Init As Boolean)

		If Init Then
			If _MapSever Is Nothing Then _MapSever = New MapPipeServer(Nothing)
			AddHandler _MapSever.DataDecoded, AddressOf MapSever_DataDecoded
			_MapSever.Start()
		End If

		If _MapSever IsNot Nothing AndAlso Init = False Then
			RemoveHandler _MapSever.DataDecoded, AddressOf MapSever_DataDecoded
			_MapSever.Stop()

		End If

	End Sub

	Private Sub MapSever_DataDecoded(Data As MapDataInfoCollection)
		If Data IsNot Nothing Then
			MapDataInfoCollection.UpdateFromMapDataInfoCollection(Data, GlobalMapDataInfoCollection)
		End If
	End Sub


#End Region



End Module
