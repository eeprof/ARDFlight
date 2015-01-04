Class Application

	<STAThread> _
	Public Shared Sub Main()


		Dim application = New Application()
		application.InitializeComponent()
		application.Run()


	End Sub

	Private Sub Application_DispatcherUnhandledException(sender As Object, e As Windows.Threading.DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
		e.Handled = True
		'System.Windows.MessageBox.Show(e.Exception.ToString())
	End Sub

	Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup

		If e IsNot Nothing AndAlso e.Args IsNot Nothing AndAlso e.Args.Count > 0 Then

		End If

		StartApp()

	End Sub

	Public Sub StartApp()

		LocalSetting.StartHub(Me)


	End Sub



End Class