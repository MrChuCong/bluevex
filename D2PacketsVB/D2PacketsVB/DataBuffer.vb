Imports System.Text
Imports System.IO

Public MustInherit Class DataBuffer
    Implements IDisposable

    ' Methods
    Public Sub Dispose() Implements IDisposable.Dispose
        Me.Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If (disposing AndAlso (Not Me.m_ms Is Nothing)) Then
            Me.m_ms.Dispose()
            Me.m_ms = Nothing
        End If
    End Sub

    Protected Function GetData() As Byte()
        Dim dst As Byte() = Nothing
        SyncLock Me
            dst = New Byte(Me.m_len - 1) {}
            Buffer.BlockCopy(Me.m_ms.GetBuffer, 0, dst, 0, Me.m_len)
        End SyncLock
        Return dst
    End Function

    Public Sub Clear()
        Me.m_len = 0

        m_ms.Dispose()
        m_ms = New MemoryStream
    End Sub

    Public Sub Insert(ByVal b As Boolean)
        If b Then
            Me.Insert(1)
        Else
            Me.Insert(0)
        End If
    End Sub

    <CLSCompliant(False)> _
    Public Sub Insert(ByVal b As SByte)
        SyncLock Me
            Me.m_ms.WriteByte(CByte(b))
            Me.m_len += 1
        End SyncLock
    End Sub

    Public Sub Insert(ByVal b As Byte())
        SyncLock Me
            Me.m_ms.Write(b, 0, b.Length)
            Me.m_len = (Me.m_len + b.Length)
        End SyncLock
    End Sub

    Public Sub Insert(ByVal s As Short())
        Dim dst As Byte() = New Byte((s.Length * 2) - 1) {}
        Buffer.BlockCopy(s, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    Public Sub Insert(ByVal i As Integer())
        Dim dst As Byte() = New Byte((i.Length * 4) - 1) {}
        Buffer.BlockCopy(i, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    Public Sub Insert(ByVal l As Long())
        Dim dst As Byte() = New Byte((l.Length * 8) - 1) {}
        Buffer.BlockCopy(l, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    Public Sub Insert(ByVal b As Byte)
        SyncLock Me
            Me.m_ms.WriteByte(b)
            Me.m_len += 1
        End SyncLock
    End Sub

    Public Sub Insert(ByVal s As Short)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(s), 0, 2)
            Me.m_len = (Me.m_len + 2)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub Insert(ByVal s As UInt16)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(s), 0, 2)
            Me.m_len = (Me.m_len + 2)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub Insert(ByVal i As UInt32)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(i), 0, 4)
            Me.m_len = (Me.m_len + 4)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub Insert(ByVal b As SByte())
        Dim dst As Byte() = New Byte(b.Length - 1) {}
        Buffer.BlockCopy(b, 0, dst, 0, b.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, b.Length)
            Me.m_len = (Me.m_len + b.Length)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub Insert(ByVal s As UInt16())
        Dim dst As Byte() = New Byte((s.Length * 2) - 1) {}
        Buffer.BlockCopy(s, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    Public Sub Insert(ByVal l As Long)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(l), 0, 8)
            Me.m_len = (Me.m_len + 8)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub Insert(ByVal i As UInt32())
        Dim dst As Byte() = New Byte((i.Length * 4) - 1) {}
        Buffer.BlockCopy(i, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub Insert(ByVal l As UInt64())
        Dim dst As Byte() = New Byte((l.Length * 8) - 1) {}
        Buffer.BlockCopy(l, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    Public Sub Insert(ByVal i As Integer)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(i), 0, 4)
            Me.m_len = (Me.m_len + 4)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub Insert(ByVal l As UInt64)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(l), 0, 8)
            Me.m_len = (Me.m_len + 8)
        End SyncLock
    End Sub

    Public Sub InsertBoolean(ByVal b As Boolean)
        If b Then
            Me.InsertByte(1)
        Else
            Me.InsertByte(0)
        End If
    End Sub

    Public Sub InsertByte(ByVal b As Byte)
        SyncLock Me
            Me.m_ms.WriteByte(b)
            Me.m_len += 1
        End SyncLock
    End Sub

    Public Sub InsertByteArray(ByVal b As Byte())
        SyncLock Me
            Me.m_ms.Write(b, 0, b.Length)
            Me.m_len = (Me.m_len + b.Length)
        End SyncLock
    End Sub

    Public Sub InsertCString(ByVal str As String)
        Me.InsertCString(str, Encoding.ASCII)
    End Sub

    Public Sub InsertCString(ByVal str As String, ByVal enc As Encoding)
        If (str Is Nothing) Then
            Throw New ArgumentNullException("str", "The specified string is null.")
        End If
        If (enc Is Nothing) Then
            Throw New ArgumentNullException("enc", "The specified encoding parameter was null.")
        End If
        Me.Insert(enc.GetBytes(str))
        Me.Insert(New Byte(enc.GetByteCount(New Char(1 - 1) {}) - 1) {})
    End Sub

    Public Sub InsertDwordString(ByVal str As String)
        Me.InsertDwordString(str, 0)
    End Sub

    Public Sub InsertDwordString(ByVal str As String, ByVal padding As Byte)
        If (str.Length > 4) Then
            Throw New ArgumentException("String length was too long; max length 4.", "str")
        End If
        SyncLock Me
            If (str.Length < 4) Then
                Dim num As Integer = (4 - str.Length)
                Dim j As Integer
                For j = 0 To num - 1
                    Me.Insert(padding)
                Next j
            End If
            Dim bytes As Byte() = Encoding.ASCII.GetBytes(str)
            Dim i As Integer = (bytes.Length - 1)
            Do While (i >= 0)
                Me.Insert(bytes(i))
                i -= 1
            Loop
        End SyncLock
    End Sub

    Public Sub InsertInt16(ByVal s As Short)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(s), 0, 2)
            Me.m_len = (Me.m_len + 2)
        End SyncLock
    End Sub

    Public Sub InsertInt16Array(ByVal s As Short())
        Dim dst As Byte() = New Byte((s.Length * 2) - 1) {}
        Buffer.BlockCopy(s, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    Public Sub InsertInt32(ByVal i As Integer)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(i), 0, 4)
            Me.m_len = (Me.m_len + 4)
        End SyncLock
    End Sub

    Public Sub InsertInt32Array(ByVal i As Integer())
        Dim dst As Byte() = New Byte((i.Length * 4) - 1) {}
        Buffer.BlockCopy(i, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    Public Sub InsertInt64(ByVal l As Long)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(l), 0, 8)
            Me.m_len = (Me.m_len + 8)
        End SyncLock
    End Sub

    Public Sub InsertInt64Array(ByVal l As Long())
        Dim dst As Byte() = New Byte((l.Length * 8) - 1) {}
        Buffer.BlockCopy(l, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    Public Sub InsertPascalString(ByVal str As String)
        Me.InsertPascalString(str, Encoding.ASCII)
    End Sub

    Public Sub InsertPascalString(ByVal str As String, ByVal enc As Encoding)
        If (str.Length > &HFF) Then
            Throw New ArgumentException("String length was too long; max length 255.", "str")
        End If
        Me.Insert(CByte((str.Length And &HFF)))
        Me.Insert(enc.GetBytes(str))
    End Sub

    <CLSCompliant(False)> _
    Public Sub InsertSByte(ByVal b As SByte)
        SyncLock Me
            Me.m_ms.WriteByte(CByte(b))
            Me.m_len += 1
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub InsertSByteArray(ByVal b As SByte())
        Dim dst As Byte() = New Byte(b.Length - 1) {}
        Buffer.BlockCopy(b, 0, dst, 0, b.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, b.Length)
            Me.m_len = (Me.m_len + b.Length)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub InsertUInt16(ByVal s As UInt16)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(s), 0, 2)
            Me.m_len = (Me.m_len + 2)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub InsertUInt16Array(ByVal s As UInt16())
        Dim dst As Byte() = New Byte((s.Length * 2) - 1) {}
        Buffer.BlockCopy(s, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub InsertUInt32(ByVal i As UInt32)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(i), 0, 4)
            Me.m_len = (Me.m_len + 4)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub InsertUInt32Array(ByVal i As UInt32())
        Dim dst As Byte() = New Byte((i.Length * 4) - 1) {}
        Buffer.BlockCopy(i, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub InsertUInt64(ByVal l As UInt64)
        SyncLock Me
            Me.m_ms.Write(BitConverter.GetBytes(l), 0, 8)
            Me.m_len = (Me.m_len + 8)
        End SyncLock
    End Sub

    <CLSCompliant(False)> _
    Public Sub InsertUInt64Array(ByVal l As UInt64())
        Dim dst As Byte() = New Byte((l.Length * 8) - 1) {}
        Buffer.BlockCopy(l, 0, dst, 0, dst.Length)
        SyncLock Me
            Me.m_ms.Write(dst, 0, dst.Length)
            Me.m_len = (Me.m_len + dst.Length)
        End SyncLock
    End Sub

    Public Sub InsertWidePascalString(ByVal str As String)
        Me.InsertWidePascalString(str, Encoding.ASCII)
    End Sub

    Public Sub InsertWidePascalString(ByVal str As String, ByVal enc As Encoding)
        If (str.Length > &HFFFF) Then
            Throw New ArgumentException("String length was too long; max length 65535.", "str")
        End If
        Me.Insert(CUShort((str.Length And &HFFFF)))
        Me.Insert(enc.GetBytes(str))
    End Sub

    Public Overrides Function ToString() As String
        Return DataFormatter.Format(Me.GetData, 0, Me.Count)
    End Function

    Public Overridable Function WriteToOutputStream(ByVal str As Stream) As Integer
        SyncLock Me
            Dim data As Byte() = Me.GetData
            str.Write(data, 0, Me.Count)
        End SyncLock
        Return Me.Count
    End Function


    ' Properties
    Public Overridable ReadOnly Property Count() As Integer
        Get
            Return Me.m_len
        End Get
    End Property


    ' Fields
    Private m_len As Integer
    Private m_ms As MemoryStream = New MemoryStream
End Class

Public Class DataFormatter
    ' Methods
    Public Shared Function Format(ByVal data As Byte()) As String
        If (data Is Nothing) Then
            Throw New ArgumentNullException("data", "The specified data buffer was null.")
        End If
        Dim builder As New StringBuilder
        builder.Append("0000   ")
        If (data.Length = 0) Then
            builder.Append("(empty)")
            Return builder.ToString
        End If
        Dim builder2 As New StringBuilder(&H10, &H10)
        Dim i As Integer
        For i = 0 To data.Length - 1
            Dim c As Char = DirectCast(ChrW(data(i)), Char)
            If ((Char.IsLetterOrDigit(c) OrElse Char.IsPunctuation(c)) OrElse (Char.IsSymbol(c) OrElse (c = " "c))) Then
                builder2.Append(c)
            Else
                builder2.Append("."c)
            End If
            builder.AppendFormat("{0:x2} ", data(i))
            If (((i + 1) Mod 8) = 0) Then
                builder.Append(" ")
            End If
            If ((((i + 1) Mod &H10) = 0) OrElse ((i + 1) = data.Length)) Then
                If (((i + 1) = data.Length) AndAlso (((i + 1) Mod &H10) <> 0)) Then
                    Dim num2 As Integer = ((i Mod &H10) * 3)
                    If ((i Mod &H10) > 8) Then
                        num2 += 1
                    End If
                    Dim j As Integer
                    For j = 0 To (&H2F - num2) - 1
                        builder.Append(" "c)
                    Next j
                End If
                builder.AppendFormat("  {0}", builder2.ToString)
                builder2 = New StringBuilder(&H10, &H10)
                builder.Append(Environment.NewLine)
                If (data.Length > (i + 1)) Then
                    builder.AppendFormat("{0:x4}   ", (i + 1))
                End If
            End If
        Next i
        Return builder.ToString
    End Function

    Public Shared Function Format(ByVal data As Byte(), ByVal startIndex As Integer, ByVal length As Integer) As String
        If (data Is Nothing) Then
            Throw New ArgumentNullException("data", "The specified data buffer was null.")
        End If
        Dim builder As New StringBuilder
        builder.Append("0000   ")
        If (data.Length = 0) Then
            builder.Append("(empty)")
            Return builder.ToString
        End If
        Dim builder2 As New StringBuilder(&H10, &H10)
        Dim i As Integer = startIndex
        Do While ((i < data.Length) AndAlso (i < (startIndex + length)))
            Dim c As Char = DirectCast(ChrW(data(i)), Char)
            If ((Char.IsLetterOrDigit(c) OrElse Char.IsPunctuation(c)) OrElse (Char.IsSymbol(c) OrElse (c = " "c))) Then
                builder2.Append(c)
            Else
                builder2.Append("."c)
            End If
            builder.AppendFormat("{0:x2} ", data(i))
            If (((i + 1) Mod 8) = 0) Then
                builder.Append(" ")
            End If
            If ((((i + 1) Mod &H10) = 0) OrElse ((i + 1) = data.Length)) Then
                If (((i + 1) = data.Length) AndAlso (((i + 1) Mod &H10) <> 0)) Then
                    Dim num2 As Integer = ((i Mod &H10) * 3)
                    If ((i Mod &H10) > 8) Then
                        num2 += 1
                    End If
                    Dim j As Integer
                    For j = 0 To (&H2F - num2) - 1
                        builder.Append(" "c)
                    Next j
                End If
                builder.AppendFormat("  {0}", builder2.ToString)
                builder2 = New StringBuilder(&H10, &H10)
                builder.Append(Environment.NewLine)
                If (data.Length > (i + 1)) Then
                    builder.AppendFormat("{0:x4}   ", (i + 1))
                End If
            End If
            i += 1
        Loop
        Return builder.ToString
    End Function

    Public Shared Sub WriteToConsole(ByVal data As Byte())
        Console.WriteLine(DataFormatter.Format(data))
    End Sub

    Public Shared Sub WriteToTrace(ByVal data As Byte())
        Trace.WriteLine(DataFormatter.Format(data))
    End Sub

    Public Shared Sub WriteToTrace(ByVal data As Byte(), ByVal category As String)
        Trace.WriteLine(DataFormatter.Format(data), category)
    End Sub

End Class