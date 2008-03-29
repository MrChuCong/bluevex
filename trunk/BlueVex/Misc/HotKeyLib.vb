'This library is from vbaccelerator.com
'http://www.vbaccelerator.com/home/NET/Code/Libraries/Windows_Messages/Hot_Key_Form/HotKeyForm.asp
'Unused parts have been removed

Public Delegate Sub HotKeyPressedEventHandler(ByVal sender As Object, ByVal e As HotKeyPressedEventArgs)

Public Class HotKeyPressedEventArgs
    Inherits EventArgs
    Private m_hotKey As HotKey

    Public ReadOnly Property HotKey() As HotKey
        Get
            HotKey = m_hotKey
        End Get
    End Property

    Friend Sub New(ByVal hotKey As HotKey)
        m_hotKey = hotKey
    End Sub

End Class

Public Class HotKeyCollection
    Inherits System.Collections.CollectionBase

    Private ownerForm As System.Windows.Forms.Form

    Protected Overrides Sub OnClear()
        Dim htk As HotKey
        For Each htk In Me.InnerList
            RemoveHotKey(htk)
        Next
        MyBase.OnClear()
    End Sub

    Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal item As Object)
        ' validate item is a hot key:
        Dim htk As HotKey = New HotKey()
        If (item.GetType().IsInstanceOfType(htk)) Then
            ' check if the name, keycode and modifiers have been set up:
            htk = item
            ' throws ArgumentException if there is a problem:
            htk.Validate()
            ' throws Unable to add HotKeyException:
            AddHotKey(htk)
            ' ok
            MyBase.OnInsert(index, item)
        Else
            Throw New InvalidCastException("Invalid object.")
        End If

    End Sub

    Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal item As Object)
        ' get the item to be removed:
        Dim htk As HotKey = item
        'RemoveHotKey(htk)
        MyBase.OnRemove(index, item)
    End Sub

    Protected Overrides Sub OnSet(ByVal index As Integer, ByVal oldItem As Object, ByVal newItem As Object)
        ' remove old hot key:
        Dim htk As HotKey = oldItem
        RemoveHotKey(htk)

        ' add new hotkey:
        htk = newItem
        AddHotKey(htk)

        MyBase.OnSet(index, oldItem, newItem)
    End Sub

    Protected Overrides Sub OnValidate(ByVal item As Object)
        Dim htk As HotKey = item
        htk.Validate()
    End Sub

    Public Sub Add(ByVal hotKey As HotKey)
        ' throws argument exception:
        hotKey.Validate()
        ' throws unable to add hot key exception:
        AddHotKey(hotKey)
        ' assuming all is well:
        Me.InnerList.Add(hotKey)
    End Sub

    Default Public ReadOnly Property Item(ByVal index As Integer) As Integer
        Get
            Item = Me.InnerList.Item(index)
        End Get
    End Property

    Delegate Sub RemoveHotKeyDelegate(ByVal hotKey As HotKey)
    Public Sub RemoveHotKey(ByVal hotKey As HotKey)
        If Main.InvokeRequired Then
            Main.Invoke(New RemoveHotKeyDelegate(AddressOf RemoveHotKey), New Object() {hotKey})
        Else
            If Main.HotKeys.List.Contains(hotKey) Then
                '// remove the hot key:
                UnmanagedMethods.UnregisterHotKey(ownerForm.Handle, hotKey.AtomId.ToInt32())
                '// unregister the atom:
                UnmanagedMethods.GlobalDeleteAtom(hotKey.AtomId)
                'Remove the key
                Main.HotKeys.List.Remove(hotKey)
            End If
        End If
    End Sub

    Delegate Sub AddHotKeyDelegate(ByVal hotKey As HotKey)
    Private Sub AddHotKey(ByVal hotKey As HotKey)
        If Main.InvokeRequired Then
            Main.Invoke(New AddHotKeyDelegate(AddressOf AddHotKey), New Object() {hotKey})
        Else

            If Not Main.HotKeys.List.Contains(hotKey) Then
                ' generate the id:
                Dim atomName As String = hotKey.Name + "_" + UnmanagedMethods.GetTickCount().ToString()
                If (atomName.Length > 255) Then
                    atomName = atomName.Substring(0, 255)
                End If
                ' Create a new atom:
                Dim id As IntPtr = UnmanagedMethods.GlobalAddAtom(atomName)
                If (id.Equals(IntPtr.Zero)) Then
                    ' failed
                    Throw New HotKeyAddException("Failed to add GlobalAtom for HotKey")
                Else
                    ' succeeded:
                    Dim ret As Boolean = UnmanagedMethods.RegisterHotKey( _
                      ownerForm.Handle, _
                      id.ToInt32(), _
                      hotKey.Modifiers, _
                      hotKey.KeyCode)
                    If Not (ret) Then
                        ' Remove the atom:
                        UnmanagedMethods.GlobalDeleteAtom(id)
                        ' failed
                        Throw New HotKeyAddException("Failed to register HotKey")
                    Else
                        hotKey.AtomName = atomName
                        hotKey.AtomId = id
                    End If
                End If
            End If
        End If
    End Sub

    Public Sub New(ByVal ownerForm As System.Windows.Forms.Form)
        Me.ownerForm = ownerForm
    End Sub

End Class

Public Class HotKeyAddException
    Inherits System.Exception

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As System.Exception)
        MyBase.New(message, innerException)
    End Sub
End Class

Public Class HotKey
    Public Enum HotKeyModifiers As Integer
        None = 0
        ALT = &H1
        CONTROL = &H2
        SHIFT = &H4
        WIN = &H8
    End Enum
    Private m_name As String
    Private m_atomName As String
    Private m_atomId As IntPtr
    Private m_keyCode As Keys
    Private m_modifiers As HotKeyModifiers

    Friend Property AtomId() As IntPtr
        Get
            AtomId = m_atomId
        End Get
        Set(ByVal Value As IntPtr)
            m_atomId = Value
        End Set
    End Property

    Friend Property AtomName() As String
        Get
            AtomName = m_atomName
        End Get
        Set(ByVal Value As String)
            m_atomName = Value
        End Set
    End Property

    Public Property Name() As String
        Get
            Name = m_name
        End Get
        Set(ByVal Value As String)
            m_name = Value
        End Set
    End Property

    Public Property KeyCode() As Keys
        Get
            KeyCode = m_keyCode
        End Get
        Set(ByVal Value As Keys)
            m_keyCode = Value
        End Set
    End Property

    Public Property Modifiers() As HotKeyModifiers
        Get
            Modifiers = m_modifiers
        End Get
        Set(ByVal Value As HotKeyModifiers)
            m_modifiers = Value
        End Set
    End Property

    Public Sub Validate()
        Dim msg As String = ""
        'If (Name Is Null) Then
        'msg = "Name parameter cannot be null"
        'End If
        If (m_name.Trim().Length = 0) Then
            msg = "Name parameter cannot be zero length"
        End If
        If ((KeyCode = Keys.Alt) Or _
         (KeyCode = Keys.Control) Or _
         (KeyCode = Keys.Shift) Or _
         (KeyCode = Keys.ShiftKey) Or _
         (KeyCode = Keys.ControlKey)) Then
            msg = "KeyCode cannot be set to a modifier key"
        End If
        If (msg.Length > 0) Then
            Throw New ArgumentException(msg)
        End If
    End Sub

    Public Sub New()

    End Sub

    Public Sub New( _
        ByVal name As String, _
        ByVal keyCode As Keys, _
        ByVal modifiers As HotKeyModifiers _
        )
        m_name = name
        m_keyCode = keyCode
        m_modifiers = modifiers
    End Sub

End Class

Friend Class UnmanagedMethods

    Friend Const WM_HOTKEY As Integer = &H312

    Public Declare Auto Function RegisterHotKey Lib "user32" _
        (ByVal hWnd As IntPtr, _
        ByVal id As Integer, _
        ByVal fsModifiers As Integer, _
        ByVal vk As Integer _
        ) As Boolean
    Public Declare Auto Function UnregisterHotKey Lib "user32" _
        (ByVal hWnd As IntPtr, _
        ByVal id As Integer _
        ) As Boolean
    Public Declare Auto Function GlobalAddAtom Lib "kernel32" _
        (ByVal lpString As String _
        ) As IntPtr
    Public Declare Auto Function GlobalDeleteAtom Lib "kernel32" _
        (ByVal nAtom As IntPtr _
        ) As IntPtr
    Public Declare Auto Function GetTickCount Lib "kernel32" () As Integer

    Public Declare Sub keybd_event Lib "user32.dll" (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Integer, ByVal dwExtraInfo As Integer)
    Public Declare Function MapVirtualKey Lib "user32" Alias "MapVirtualKeyA" (ByVal wCode As Integer, ByVal wMapType As Integer) As Integer
    Public Declare Function GetForegroundWindow Lib "user32" Alias "GetForegroundWindow" () As Integer

End Class