Public Class Packet

    Public Enum PacketFlag
        PacketFlag_Dead = 0
        PacketFlag_Normal = 1
        PacketFlag_Hidden = 2
        PacketFlag_Virtual = 3
    End Enum

    Public Data() As Byte
    Public Length As Integer
    Private _Flag As PacketFlag
    Private SetFlagDelegate As SetFlagDelegate
    Private Pointer As IntPtr

    Public Property Flag() As PacketFlag
        Get
            Return _Flag
        End Get
        Set(ByVal value As PacketFlag)
            _Flag = value
            If SetFlagDelegate IsNot Nothing And Pointer <> Nothing Then SetFlagDelegate(Pointer, value)
        End Set
    End Property

    Sub New(ByRef buffer() As Byte, ByVal length As Integer, ByVal pointer As IntPtr, ByVal [delegate] As SetFlagDelegate)
        Me.Pointer = pointer
        SetFlagDelegate = [delegate]
        Data = buffer
        Me.Length = length
        _Flag = PacketFlag.PacketFlag_Normal
    End Sub

    Sub New(ByRef buffer() As Byte, ByVal length As Integer)
        Data = buffer
        Me.Length = length
        _Flag = PacketFlag.PacketFlag_Normal
    End Sub

    Public Overrides Function ToString() As String
        Dim Result As String = ""
        For i As Integer = 0 To Data.Length - 1
            Result += Data(i).ToString("x") & " "
        Next
        Return Result
    End Function

    Public Overloads Function ToString(ByVal Length As Integer) As String
        Dim Result As String = ""
        For i As Integer = 0 To Length - 1
            Result += Data(i).ToString("X2") & " "
        Next
        Return Result
    End Function

    'Public Function ToBinary() As String
    '    Dim Result As String = ""
    '    For i As Integer = 0 To Data.Length - 1
    '        Result += ReverseString(Convert.ToString(Data(i), 2))
    '    Next
    '    Return Result
    'End Function

    'Private Function ReverseString(ByVal text As String) As String
    '    Dim strReversedText As String
    '    Dim arrChar() As Char = text.ToCharArray()
    '    Array.Reverse(arrChar)
    '    strReversedText = arrChar
    '    Return strReversedText
    'End Function

End Class
