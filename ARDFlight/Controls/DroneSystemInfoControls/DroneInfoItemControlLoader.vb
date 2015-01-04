Imports FirstFloor.ModernUI.Windows

Public Class DroneInfoItemControlLoader
	Inherits DefaultContentLoader
	
	Protected Overrides Function LoadContent(ByVal uri As Uri) As Object
		' return a new LoremIpsum user control instance no matter the uri
		Dim Urinint As Integer = CInt(Replace(uri.ToString, "/", ""))
		Return New DroneInfoItemControl(Urinint)
	End Function


End Class
