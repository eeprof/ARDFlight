Public Class DroneInfoItemControl
	Public Property Dataindex As Integer = 0

	Public Sub New()
		InitializeComponent()
	End Sub
	Public Sub New(Parameter As Integer)

		InitializeComponent()
		Dataindex = Parameter

	End Sub

	
End Class
