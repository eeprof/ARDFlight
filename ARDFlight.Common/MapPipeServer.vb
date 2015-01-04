Imports System.IO.Pipes
Imports System.Text
Imports Catel.Collections
Imports Catel.Data
Imports Catel.MVVM


Public Class MapPipeServer
	Inherits ObservableObject

	Public Const _DEFMAPSERVERNAME As String = "ARDFLIGHTMAPSERVER"

	Public Event DataDecoded(Data As MapDataInfoCollection)

	Private _pipeName As String = String.Empty
	Public Property pipeName As String
		Get
			Return _pipeName
		End Get
		Set(Value As String)
			RaisePropertyChanging(Function() pipeName)

			Dim oldValue = _pipeName
			_pipeName = Value
			RaisePropertyChanged(Function() pipeName, oldValue, Value)

		End Set
	End Property

	Private ReadOnly BufferSize As Integer = 256
	Private _CloseConnection As Boolean = False

	Private _DecodeDataCommand As Command(Of Object) = Nothing

	Public Sub New(iDecodeDataCommand As Command(Of Object))
		_DecodeDataCommand = iDecodeDataCommand
		pipeName = _DEFMAPSERVERNAME
	End Sub

	Public Sub New(iPipeName As String, iDecodeDataCommand As Command(Of Object))
		_DecodeDataCommand = iDecodeDataCommand
		pipeName = iPipeName
	End Sub

	Public Sub [Start]()
		_CloseConnection = False
		Dim NT As New Threading.Thread(AddressOf CreatePipeServer)
		NT.Priority = Threading.ThreadPriority.Lowest
		NT.Start()
	End Sub

	Public Sub [Stop]()
		_CloseConnection = True
	End Sub

	Delegate Sub DecodeDataDelegate(ByVal Data As String)

	Private Sub DecodeData(ByVal Data As String)
		Try
			Dim Result As MapDataInfoCollection = MapDataInfoCollection.FromXElement(Data)
			If _DecodeDataCommand IsNot Nothing Then _DecodeDataCommand.Execute(Result)
			RaiseEvent DataDecoded(Result)
		Catch ex As Exception

		End Try

	End Sub

	Private Sub CreatePipeServer()
		Dim decdr As Decoder = Encoding.Default.GetDecoder()
		Dim bytes(BufferSize) As Byte
		Dim chars(BufferSize) As Char
		Dim numBytes As Integer = 0
		Dim msg As StringBuilder = New StringBuilder()
		Dim doDecodeData = New DecodeDataDelegate(AddressOf DecodeData)
		Dim pipeServer As NamedPipeServerStream
		Try
			pipeServer = New NamedPipeServerStream(pipeName, PipeDirection.In, 1, _
												   PipeTransmissionMode.Message, _
												   PipeOptions.Asynchronous)
			While True
				pipeServer.WaitForConnection()
				Do
					msg.Length = 0
					Do
						numBytes = pipeServer.Read(bytes, 0, BufferSize)
						If numBytes > 0 Then
							Dim numChars As Integer = _
										 decdr.GetCharCount(bytes, 0, numBytes)
							decdr.GetChars(bytes, 0, numBytes, chars, 0, False)
							msg.Append(chars, 0, numChars)
						End If
					Loop Until (numBytes = 0) Or (pipeServer.IsMessageComplete)
					decdr.Reset()
					If numBytes > 0 Then
						doDecodeData.Invoke(msg.ToString)
					End If
				Loop Until numBytes = 0
				pipeServer.Disconnect()
				If _CloseConnection Then Exit While
			End While

			If pipeServer IsNot Nothing Then
				pipeServer.Dispose()
			End If



		Catch ex As Exception
			Debug.WriteLine(ex.Message)
		End Try
	End Sub

End Class
