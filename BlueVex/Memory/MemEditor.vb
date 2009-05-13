Imports System
Imports System.Runtime.InteropServices
Imports System.Diagnostics

Namespace Memory



    ''' <summary>
    ''' Easy-to-use class to deal with memory.
    ''' </summary> 
    Public Class MemEditor
        Implements IDisposable
        Public Success As Boolean = False

        Private Structure MemoryRegion
            Public baseAddress As Long
            Public dwLength As Long
            Public Sub New(ByVal lAddress As Long, ByVal lLength As Long)
                baseAddress = lAddress
                dwLength = lLength
            End Sub
            Public Sub New(ByVal startAddress As Long, ByVal endAddress As Long, ByVal nullVar As Integer)
                baseAddress = startAddress
                dwLength = endAddress - startAddress
            End Sub
        End Structure

        Private Structure vmProtect
            Public vmAccess As UInteger
            Public Sub New(ByVal vmpAccess As UInteger)
                vmAccess = vmpAccess
            End Sub
        End Structure

#Region "Declares"

        Public lastError As String
        Public netProcHandle As Process

        Private hProc As IntPtr = IntPtr.Zero
        Private bytesRW As IntPtr
        Private bBuff As Byte()
        Private Const dwAllAccess As UInteger = 2035711
#End Region

#Region "Process Handling"


        Sub New(Optional ByVal DiabloPID As Integer = 0)

            If DiabloPID <> 0 Then

                Process.EnterDebugMode()

                'PID defined! No need to find it :D
                netProcHandle = Process.GetProcessById(DiabloPID)

                hProc = Tools.OpenProcess(dwAllAccess, False, CInt(netProcHandle.Id))
                Success = True
                Exit Sub
            Else
                'No PID? We Got to find a random one :(
                Try
                    Dim D2HWND As Integer = Tools.FindWindow("Diablo II", Nothing)

                    If D2HWND < 1 Then
                        lastError = "Diablo window not found"
                        Success = False
                        Exit Sub
                    End If

                    Dim processes As Process() = Process.GetProcesses
                    'For each avalaible process
                    For Each Process As Process In processes
                        'Is it one we want?
                        If Process.MainWindowHandle = D2HWND Then
                            'Get the PID
                            netProcHandle = Process.GetProcessById(Process.Id)



                            'Open It
                            hProc = Tools.OpenProcess(dwAllAccess, False, CInt(netProcHandle.Id))
                            Success = True
                            Exit Sub
                        End If
                    Next
                Catch ex As Exception
                    Success = False
                    Exit Sub
                End Try
            End If
            Success = False
        End Sub


        Public Function mCloseProcess() As Int32
            If hProc <> IntPtr.Zero Then
                Return Tools.CloseHandle(hProc)
            Else
                lastError = "No process handle is currently open to close"
                Return -1
            End If
        End Function
#End Region

#Region "Module Handling"
        Public Function GetModuleCollection() As ProcessModuleCollection
            If netProcHandle IsNot Nothing Then
                Return netProcHandle.Modules
            Else
                Return Nothing
            End If
        End Function
        Public Function GetModule(ByVal moduleName As String) As ProcessModule
            If netProcHandle IsNot Nothing Then
                Dim tPM As ProcessModule = Nothing
                For Each pM As ProcessModule In netProcHandle.Modules
                    If moduleName.ToLower() = pM.ModuleName.ToLower() Then
                        tPM = pM
                    End If
                Next
                Return tPM
            Else
                lastError = "No process handle open to enumerate."
                Return Nothing
            End If
        End Function

        Public Function GetModuleBaseAddress(ByVal moduleName As String) As IntPtr
            If netProcHandle IsNot Nothing Then
                Return GetModule(moduleName).BaseAddress
            Else
                Return IntPtr.Zero
            End If
        End Function
        Public Function GetModuleEntryPoint(ByVal moduleName As String) As IntPtr
            If netProcHandle IsNot Nothing Then
                Return GetModule(moduleName).EntryPointAddress
            Else
                Return IntPtr.Zero
            End If
        End Function
        Public Function GetModuleMemSize(ByVal moduleName As String) As Int32
            If netProcHandle IsNot Nothing Then
                Return GetModule(moduleName).ModuleMemorySize
            Else
                Return -1
            End If
        End Function
#End Region

#Region "ReadMemory()"
        Public Function ReadMemoryByte(ByVal address As IntPtr) As Byte
            bBuff = New Byte(0) {}
            Tools.ReadProcessMemory(hProc, address, bBuff, 1, bytesRW)
            Return bBuff(0)
        End Function
        Public Function ReadMemoryShort(ByVal address As IntPtr) As Short
            bBuff = New Byte(1) {}
            Tools.ReadProcessMemory(hProc, address, bBuff, 2, bytesRW)
            Return BitConverter.ToInt16(bBuff, 0)
        End Function
        Public Function ReadMemoryInt(ByVal address As IntPtr) As Integer
            bBuff = New Byte(3) {}
            Tools.ReadProcessMemory(hProc, address, bBuff, 4, bytesRW)
            Return BitConverter.ToInt32(bBuff, 0)
        End Function
        Public Function ReadMemoryLong(ByVal address As IntPtr) As Long
            Return (ReadMemoryLong(address, 8))
        End Function
        Public Function ReadMemoryLong(ByVal address As IntPtr, ByVal length As Integer) As Long
            bBuff = New Byte(length - 1) {}
            Tools.ReadProcessMemory(hProc, address, bBuff, CInt(length), bytesRW)
            If length >= 8 Then
                Return BitConverter.ToInt64(bBuff, 0)
            Else
                Return BitConverter.ToInt32(bBuff, 0)
            End If
        End Function
        Public Function ReadMemoryFloat(ByVal address As IntPtr) As Single
            bBuff = New Byte(3) {}
            Tools.ReadProcessMemory(hProc, address, bBuff, 8, bytesRW)
            Return BitConverter.ToSingle(bBuff, 0)
        End Function
        Public Function ReadMemoryDouble(ByVal address As IntPtr) As Double
            bBuff = New Byte(7) {}
            Tools.ReadProcessMemory(hProc, address, bBuff, 8, bytesRW)
            Return BitConverter.ToDouble(bBuff, 0)
        End Function
        Public Function ReadMemoryString(ByVal address As IntPtr, ByVal stringLength As Integer) As String
            bBuff = New Byte(stringLength - 1) {}
            Tools.ReadProcessMemory(hProc, address, bBuff, CInt(stringLength), bytesRW)
            Return BitConverter.ToString(bBuff, 0)
        End Function
        Public Function ReadMemoryAOB(ByVal address As IntPtr, ByVal length As Long) As Byte()
            bBuff = New Byte(length - 1) {}
            Tools.ReadProcessMemory(hProc, address, bBuff, CInt(length), bytesRW)
            Return bBuff
            'BitConverter.ToInt32(bBuff, 0);
        End Function

        Public Function ReadMemoryStruct(ByVal Address As IntPtr, ByVal MyType As Type) As Object
            'Read the Size of the struct from memory.
            Dim Buff() As Byte = ReadMemoryAOB(Address, Marshal.SizeOf(MyType))
            'Convert it into the type we want.
            Return Tools.BytesToStruct(Buff, MyType)

        End Function

#End Region
#Region "WriteMemory()"

        ' Byte (UInt8)
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Byte) As Int32
            bBuff = New Byte(0) {}
            bBuff(0) = data
            Return Tools.WriteProcessMemory(hProc, address, bBuff, 1, bytesRW)
        End Function

        ' Byte[] (*UInt8)
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Byte()) As Int32
            Return WriteMemory(address, data, False)
        End Function
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Byte(), ByVal isAOB As Boolean) As Int32
            If Not isAOB Then
                Return Tools.WriteProcessMemory(hProc, address, data, CInt(data.Length), bytesRW)
            Else
                Return Tools.WriteProcessMemory(hProc, address, ReverseByteArray(data), CInt(data.Length), bytesRW)
            End If
        End Function

        ' Short (Int16)
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Short) As Int32
            Return WriteMemory(address, data, 2)
        End Function
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Short, ByVal length As Integer) As Int32
            bBuff = BitConverter.GetBytes(data)
            Return Tools.WriteProcessMemory(hProc, address, bBuff, CInt(length), bytesRW)
        End Function

        ' Integer (Int32)
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Integer) As Int32
            Return WriteMemory(address, data, 4)
        End Function
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Integer, ByVal length As Integer) As Int32
            bBuff = BitConverter.GetBytes(data)
            Return Tools.WriteProcessMemory(hProc, address, bBuff, CInt(length), bytesRW)
        End Function

        ' Long (Int64)
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Long) As Int32
            Return WriteMemory(address, data, 8)
        End Function
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Long, ByVal length As Integer) As Int32
            bBuff = BitConverter.GetBytes(data)
            Return Tools.WriteProcessMemory(hProc, address, bBuff, CInt(length), bytesRW)
        End Function

        ' Float (single)
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Single) As Int32
            Return WriteMemory(address, data, 8)
        End Function
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Single, ByVal length As Integer) As Int32
            bBuff = BitConverter.GetBytes(data)
            Return Tools.WriteProcessMemory(hProc, address, bBuff, CInt(length), bytesRW)
        End Function

        ' Double (double)
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Double) As Int32
            Return WriteMemory(address, data, 8)
        End Function
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As Double, ByVal length As Integer) As Int32
            bBuff = BitConverter.GetBytes(data)
            Return Tools.WriteProcessMemory(hProc, address, bBuff, CInt(length), bytesRW)
        End Function

        ' String (auto-length unicode string)
        Public Function WriteMemory(ByVal address As IntPtr, ByVal data As String) As Int32
            bBuff = New Byte(data.Length - 1) {}
            For i As Integer = 0 To data.Length - 1
                bBuff(i) = CByte(Char.ConvertToUtf32(data, i))
            Next

            Return Tools.WriteProcessMemory(hProc, address, bBuff, CInt(bBuff.Length), bytesRW)
        End Function
#End Region

#Region "Misc Functions"

        Public Function ConstructAOB(ByVal szBytes As String) As Byte()
            Return ConstructAOB(szBytes, " "c)
        End Function
        Public Function ConstructAOB(ByVal szBytes As String, ByVal czDelimiter As Char) As Byte()
            Dim tStr As String() = szBytes.Split(czDelimiter)
            Dim tByt As Byte() = New Byte(tStr.Length - 1) {}
            For i As Integer = 0 To tStr.Length - 1
                tByt(i) = CByte(Integer.Parse(tStr(i)))
            Next
            Return tByt
        End Function

        Public Function ReverseByteArray(ByVal array As Byte()) As Byte()
            Dim tByt As Byte() = New Byte(array.Length - 1) {}
            For i As Integer = 0 To array.Length - 1
                tByt(5 - i) = array(i)
            Next
            Return tByt
        End Function
#End Region

        Public Sub Dispose()
            If hProc <> IntPtr.Zero Then
                mCloseProcess()
                netProcHandle.Dispose()
            End If
        End Sub
        Public Sub Dispose1() Implements System.IDisposable.Dispose

        End Sub
    End Class

End Namespace


Namespace Memory.Tools
    Public Module Tools

        <DllImport("kernel32.dll", SetLastError:=True)> _
Public Function OpenProcess(ByVal dwDesiredAccess As UInt32, ByVal bInheritHandle As Boolean, ByVal dwProcessId As UInt32) As IntPtr
        End Function
        <DllImport("kernel32.dll")> _
        Public Function CloseHandle(ByVal hObject As IntPtr) As Int32
        End Function
        <DllImport("kernel32.dll")> _
        Public Function ReadProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, <[In](), Out()> _
ByVal buffer As Byte(), ByVal size As UInt32, ByRef lpNumberOfBytesRead As IntPtr) As Int32
        End Function
        <DllImport("kernel32.dll")> _
        Public Function WriteProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, <[In](), Out()> _
ByVal buffer As Byte(), ByVal size As UInt32, ByRef lpNumberOfBytesWritten As IntPtr) As Int32
        End Function
        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Function VirtualProtectEx(ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As UIntPtr, ByVal flNewProtect As UInteger, ByRef lpflOldProtect As UInteger) As Boolean
        End Function
        Public Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
        Public Declare Function GetWindowThreadProcessId Lib "user32" (ByVal hwnd As Long, ByRef lpdwProcessId As Long) As Long

        Private Declare Function SendMessageByString Lib "user32" Alias "SendMessageA" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As String) As Integer
        Private Const WM_SETTEXT As Short = &HCS


        Public Sub SetWindowName(ByVal Hwnd As Integer, ByVal NewName As String)
            SendMessageByString(Hwnd, WM_SETTEXT, 0, NewName)
        End Sub

        Public Function BytesToStruct(ByVal Buff() As Byte, ByVal MyType As System.Type) As Object

            Dim MyGC As GCHandle = GCHandle.Alloc(Buff, GCHandleType.Pinned)
            'Marshals data from an unmanaged block of memory 
            'to a newly allocated managed object of the specified type.

            Dim Obj As Object = _
                Marshal.PtrToStructure(MyGC.AddrOfPinnedObject, MyType)
            'Free GChandle to avoid memory leaks
            MyGC.Free()
            Return Obj
        End Function

        Public Function StructToBytes(ByVal Struct As Object) As Byte()
            Dim ByteArray() As Byte

            Dim Ptr As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(Struct))
            ReDim ByteArray(Marshal.SizeOf(Struct) - 1)
            'now copy strcutre to Ptr pointer 
            Marshal.StructureToPtr(Struct, Ptr, False)
            Marshal.Copy(Ptr, ByteArray, 0, Marshal.SizeOf(Struct))
            'now use ByteArray ready for use 
            Marshal.FreeHGlobal(Ptr)
            Return ByteArray
        End Function

        Public Function DecToHex(ByVal DecVal As Double) As String
            DecToHex = String.Empty

            If DecVal = 0 Then Return "0"
            Dim a As Double, b As Double, c As String, d As Double
            a = DecVal
            For b = 1 To Int(Math.Log(DecVal) / Math.Log(16)) + 1
                d = CDbl(a Mod 16)
                Select Case d
                    Case 0 To 9
                        c = d
                    Case Else
                        c = Chr(55 + d)
                End Select
                DecToHex = c & DecToHex
                a = CDbl(Int(a / 16))
            Next b

        End Function
        Public Function HexToDec(ByVal HexVal As String) As Double
            If HexVal = "" Or HexVal = "0" Then Return 0
            Dim TotalDec As Double, a As Double, c As Double
            For a = 1 To Len(HexVal)
                Select Case (Mid(HexVal, a, 1))
                    Case 0 To 9
                        c = (Mid(HexVal, a, 1))
                    Case Else
                        c = (Asc(Mid(HexVal, a, 1)) - 55)
                End Select
                TotalDec = (TotalDec * 16) + c
            Next a
            HexToDec = TotalDec
        End Function

    End Module
End Namespace