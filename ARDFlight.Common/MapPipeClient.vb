Imports System.IO.Pipes
Imports System.Text
Imports Catel.Collections
Imports Catel.Data
Imports System.IO

Public Class MapPipeClient

	Public Shared Sub SedMapData(Data As String, ServerName As String)
		Dim TH As New Threading.Thread(Sub() ThSedMapData(Data, ServerName))
		TH.Priority = Threading.ThreadPriority.Lowest
		TH.Start()
	End Sub

	Private Shared Sub ThSedMapData(Data As String, ServerName As String)
		Using CliPipe As NamedPipeClientStream = New NamedPipeClientStream(".", ServerName, PipeDirection.Out, PipeOptions.Asynchronous)

			Try
				CliPipe.Connect(500)

				Using sw As StreamWriter = New StreamWriter(CliPipe)
					sw.WriteLine(Data)
				End Using

			Catch ex As Exception


			End Try


		End Using


	End Sub

End Class
