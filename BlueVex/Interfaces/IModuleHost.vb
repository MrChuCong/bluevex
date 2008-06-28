Imports System.Runtime.InteropServices
Public MustInherit Class IModuleHost

    Friend LoadedModules As New Collection
    Friend ClientPID As New Integer

    Friend Functions As FunctionInfo = Nothing
    Sub New(ByVal Funcs As IntPtr)
        Try
            If Funcs <> IntPtr.Zero Then
                Dim handle As IntPtr = Funcs
                Functions = Marshal.PtrToStructure(handle, GetType(FunctionInfo))
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        LoadModules()
    End Sub

    MustOverride Sub LoadModules()

    Sub Destroy()
        'Add Dispose Code Here
        For Each obj As Object In LoadedModules
            obj.destroy()
        Next
        LoadedModules.Clear()

    End Sub

    Sub Update()
        For Each obj As Object In LoadedModules
            obj.update()
        Next
    End Sub

    Sub OnRelayDataToServer(ByVal bytes() As Byte, ByVal PacketPointer As IntPtr, ByVal Funcs As IntPtr)
        Dim PacketFunctions As PacketFunctionInfo = Nothing
        If Funcs <> IntPtr.Zero Then
            Dim handle As IntPtr = Funcs
            PacketFunctions = Marshal.PtrToStructure(handle, GetType(PacketFunctionInfo))
        End If
        Dim Packet As New Packet(bytes, bytes.Length, PacketPointer, PacketFunctions.SetFlag)
        InterptetPacketToServer(Packet)
    End Sub
    Sub OnRelayDataToClient(ByVal bytes() As Byte, ByVal PacketPointer As IntPtr, ByVal Funcs As IntPtr)
        Dim PacketFunctions As PacketFunctionInfo = Nothing
        If Funcs <> IntPtr.Zero Then
            Dim handle As IntPtr = Funcs
            PacketFunctions = Marshal.PtrToStructure(handle, GetType(PacketFunctionInfo))
        End If
        Dim Packet As New Packet(bytes, bytes.Length, PacketPointer, PacketFunctions.SetFlag)
        InterptetPacketToClient(Packet)
    End Sub

    Sub RelayDataToClient(ByVal bytes() As Byte, ByVal length As Integer)
        If Not Functions.Equals(Nothing) Then
            Functions.RelayToClientDelegate.Invoke(bytes, length, Functions.ProxyPointer, Functions.ModulePointer)
        End If
    End Sub

    Sub RelayDataToServer(ByVal bytes() As Byte, ByVal length As Integer)
        If Not Functions.Equals(Nothing) Then
            Functions.RelayToServerDelegate.Invoke(bytes, length, Functions.ProxyPointer, Functions.ModulePointer)
        End If
    End Sub

    MustOverride Sub InterptetPacketToServer(ByRef Packet As Packet)
    MustOverride Sub InterptetPacketToClient(ByRef Packet As Packet)

End Class
