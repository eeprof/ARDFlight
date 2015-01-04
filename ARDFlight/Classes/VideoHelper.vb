Imports System.Runtime.InteropServices

Public NotInheritable Class VideoHelper
	Private Sub New()
	End Sub
	Public Shared Function ConvertPixelFormat(ByVal xpixelFormat As AR.Drone.Video.PixelFormat) As System.Drawing.Imaging.PixelFormat
		Select Case xpixelFormat
			Case AR.Drone.Video.PixelFormat.Gray8
				Return System.Drawing.Imaging.PixelFormat.Format8bppIndexed
			Case AR.Drone.Video.PixelFormat.BGR24
				Return System.Drawing.Imaging.PixelFormat.Format24bppRgb
			Case AR.Drone.Video.PixelFormat.RGB24
				Throw New NotSupportedException()
			Case Else
				Throw New ArgumentOutOfRangeException()
		End Select
	End Function


	Public Shared Function CreateBitmap(ByRef frame As AR.Drone.Video.VideoFrame) As System.Drawing.Bitmap
		Dim pixelFormat As System.Drawing.Imaging.PixelFormat = ConvertPixelFormat(frame.PixelFormat)

		Dim xbitmap = New System.Drawing.Bitmap(frame.Width, frame.Height, pixelFormat)

		If pixelFormat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed Then
			Dim palette As System.Drawing.Imaging.ColorPalette = xbitmap.Palette
			For i As Integer = 0 To palette.Entries.Length - 1
				palette.Entries(i) = System.Drawing.Color.FromArgb(i, i, i)
			Next i
			xbitmap.Palette = palette
		End If
		UpdateBitmap(xbitmap, frame)
		Return xbitmap
	End Function

	Public Shared Sub UpdateBitmap(ByRef xbitmap As System.Drawing.Bitmap, ByRef frame As AR.Drone.Video.VideoFrame)

		Try
			Dim rect = New System.Drawing.Rectangle(0, 0, xbitmap.Width, xbitmap.Height)
			Dim data As System.Drawing.Imaging.BitmapData = xbitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, xbitmap.PixelFormat)
			Marshal.Copy(frame.Data, 0, data.Scan0, frame.Data.Length)
			xbitmap.UnlockBits(data)
		Catch ex As Exception

		End Try

	End Sub
End Class
