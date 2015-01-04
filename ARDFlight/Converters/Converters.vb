
Imports System
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.IO
Imports System.Windows.Data

Namespace appconverters

	<ValueConversion(GetType(System.Drawing.Bitmap), GetType(System.Windows.Media.ImageSource))> _
	Public Class BitmapConverter
		Implements IValueConverter
		Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
			' empty images are empty...
			If value Is Nothing Then
				Return Nothing
			End If

			Dim image As System.Drawing.Bitmap = CType(value, System.Drawing.Bitmap)
			' Winforms Image we want to get the WPF Image from...
			Dim bitmap = New System.Windows.Media.Imaging.BitmapImage()
			bitmap.BeginInit()
			Dim memoryStream As New MemoryStream()
			' Save to a memory stream...
			image.Save(memoryStream, ImageFormat.Bmp)
			' Rewind the stream...
			memoryStream.Seek(0, System.IO.SeekOrigin.Begin)
			bitmap.StreamSource = memoryStream
			bitmap.EndInit()
			Return bitmap
		End Function

		Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
			Return Nothing
		End Function
	End Class

	Public Class InputConfigStateToNameConverter
		Implements IValueConverter
		Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
			Dim Result As String = "Not Recognized"

			Try
				If value Is Nothing Then Return Result
				Dim Kvp As New KeyValuePair(Of String, ARDrone.Input.InputConfigs.InputConfigState)(value.Key, value.Value)
				Result = Kvp.Value.Name
			Catch ex As Exception

			End Try

			Return Result

		End Function

		Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
			Return Nothing
		End Function
	End Class

	Public Class InputConfigStateToCurrentMappingConverter
		Implements IValueConverter
		Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
			Dim Result As String = "Not Mapping"

			Try
				If value Is Nothing Then Return Result
				Dim Kvp As New KeyValuePair(Of String, ARDrone.Input.InputConfigs.InputConfigState)(value.Key, value.Value)
				Result = Kvp.Value.CurrentMapping
			Catch ex As Exception

			End Try

			Return Result

		End Function

		Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
			Return Nothing
		End Function
	End Class

	Public Class PitchToMarginConverter
		Implements IValueConverter
		Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
			Dim Result As New Thickness(0.0, 100.0, 0.0, 0.0)

			Try

				If value IsNot Nothing Then		 'AndAlso parameter IsNot Nothing
					Dim ActualPitch As Double = If(value > 1.0, CDbl(1.0), value)
					ActualPitch = If(value < -1.0, CDbl(-1.0), value)
					ActualPitch = ActualPitch * 100

					ActualPitch = 100 + (ActualPitch)

					Result = New Thickness(0, CDbl(ActualPitch), 0, 0)

				End If

			Catch ex As Exception

			End Try

			Return Result

		End Function

		Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
			Return 1.0
		End Function
	End Class

	Public Class RollToMarginConverter
		Implements IValueConverter
		Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
			Dim Result As New Thickness(100.0, 0.0, 0.0, 0.0)

			Try

				If value IsNot Nothing Then		 'AndAlso parameter IsNot Nothing
					Dim ActualRoll As Double = If(value > 1.0, CDbl(1.0), value)
					ActualRoll = If(value < -1.0, CDbl(-1.0), value)
					ActualRoll = ActualRoll * 100

					'Change sign

					ActualRoll = -ActualRoll
				

				ActualRoll = 100 + (ActualRoll)

				Result = New Thickness(CDbl(ActualRoll), 0.0, 0.0, 0.0)

				End If

			Catch ex As Exception

			End Try

			Return Result

		End Function

		Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
			Return 1.0
		End Function
	End Class

End Namespace


