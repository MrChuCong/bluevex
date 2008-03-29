Imports System.Runtime.InteropServices

Public Class Plugin

    Public Sub New()

    End Sub

    Private Declare Function SetParent Lib "user32" (ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As Integer

    Public Sub InitPlugin(ByVal Funcs As IntPtr)
        Dim info As RedVexInfo = Nothing
        Try
            If Funcs <> IntPtr.Zero Then
                Dim handle As IntPtr = Funcs
                info = Marshal.PtrToStructure(handle, GetType(RedVexInfo))
                If Not info.Equals(Nothing) Then
                    MyRedVexInfo = info
                    Dim RedVexHandle As IntPtr = info.GetWindowHandle.Invoke()
                    Main = New frmMain
                    Main.frmMain_Load(Nothing, Nothing)
                    'Main.Show()
                    'SetParent(Main.Handle, RedVexHandle)
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Class

Public Module MainModule

    Public MyRedVexInfo As RedVexInfo

    Public Delegate Sub WriteLogType(ByVal Text As String)
    Public Delegate Function GetWindowHandleType() As IntPtr

    <Serializable(), StructLayout(LayoutKind.Sequential)> _
     Public Structure RedVexInfo
        Public WriteLog As WriteLogType
        Public GetWindowHandle As GetWindowHandleType
        Public Controler As IntPtr
    End Structure

    Public Delegate Sub RelayDelegate(ByVal bytes() As Byte, ByVal length As Integer, ByVal ProxyPointer As IntPtr, ByVal ModulePointer As IntPtr)
    Public Delegate Function GetMapDelegate(ByVal PID As Int32) As IntPtr

    <Serializable(), StructLayout(LayoutKind.Sequential)> _
    Public Structure FunctionInfo
        Public RelayToClientDelegate As RelayDelegate
        Public RelayToServerDelegate As RelayDelegate
        Public ProxyPointer As IntPtr
        Public ModulePointer As IntPtr
        Public GetMap As GetMapDelegate
    End Structure

    Delegate Sub SetFlagDelegate(ByVal PacketPointer As IntPtr, ByVal flag As Integer)
    <Serializable(), StructLayout(LayoutKind.Sequential)> _
    Public Structure PacketFunctionInfo
        Public SetFlag As SetFlagDelegate
    End Structure

    <Serializable(), StructLayout(LayoutKind.Sequential)> _
    Public Structure MapInfo
        Public X As Integer
        Public Y As Integer
        Public Width As Integer
        Public Height As Integer
        Public LevelNo As Integer
        Public Exit1ID As Integer
        Public Exit1X As Integer
        Public Exit1Y As Integer
        Public Exit2ID As Integer
        Public Exit2X As Integer
        Public Exit2Y As Integer
        Public Exit3ID As Integer
        Public Exit3X As Integer
        Public Exit3Y As Integer
        Public Bytes As IntPtr
    End Structure

End Module
