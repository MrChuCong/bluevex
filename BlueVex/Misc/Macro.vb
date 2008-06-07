Imports System
Imports System.Runtime.InteropServices
Imports System.Diagnostics



Public Class Macro

    'Function to get the Handle
    Dim DiabloProcess As New Memory.MEC.MemEdit


    'We will use the handle to send messages.
    Dim hWnd As IntPtr

    Sub New(Optional ByVal DiabloPID As Integer = 0)
        DiabloProcess.mOpenDiabloProcess(DiabloPID)
        hWnd = DiabloProcess.netProcHandle.MainWindowHandle
    End Sub

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=False)> _
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As IntPtr
    End Function

    '''summary> 
    ''' Virtual Messages 
    ''' </summary> 
    Private Enum WMessages As Integer
        WM_LBUTTONDOWN = 513
        'Left mousebutton down 
        WM_LBUTTONUP = 514
        'Left mousebutton up 
        WM_LBUTTONDBLCLK = 515
        'Left mousebutton doubleclick 
        WM_RBUTTONDOWN = 516
        'Right mousebutton down 
        WM_RBUTTONUP = 517
        'Right mousebutton up 
        WM_RBUTTONDBLCLK = 518
        'Right mousebutton doubleclick 
        WM_KEYDOWN = 256
        'Key down 
        WM_KEYUP = 257
        'Key up 
    End Enum

    ''' <summary> 
    ''' Virtual Keys 
    ''' </summary> 
    Public Enum Keys As Integer
        VK_LBUTTON = 1
        'Left mouse button 
        VK_RBUTTON = 2
        'Right mouse button 
        VK_CANCEL = 3
        'Control-break processing 
        VK_MBUTTON = 4
        'Middle mouse button (three-button mouse) 
        VK_BACK = 8
        'BACKSPACE key 
        VK_TAB = 9
        'TAB key 
        VK_CLEAR = 12
        'CLEAR key 
        VK_RETURN = 13
        'ENTER key 
        VK_SHIFT = 16
        'SHIFT key 
        VK_CONTROL = 17
        'CTRL key 
        VK_MENU = 18
        'ALT key 
        VK_PAUSE = 19
        'PAUSE key 
        VK_CAPITAL = 20
        'CAPS LOCK key 
        VK_ESCAPE = 27
        'ESC key 
        VK_SPACE = 32
        'SPACEBAR 
        VK_PRIOR = 33
        'PAGE UP key 
        VK_NEXT = 34
        'PAGE DOWN key 
        VK_END = 35
        'END key 
        VK_HOME = 36
        'HOME key 
        VK_LEFT = 37
        'LEFT ARROW key 
        VK_UP = 38
        'UP ARROW key 
        VK_RIGHT = 39
        'RIGHT ARROW key 
        VK_DOWN = 40
        'DOWN ARROW key 
        VK_SELECT = 41
        'SELECT key 
        VK_PRINT = 42
        'PRINT key 
        VK_EXECUTE = 43
        'EXECUTE key 
        VK_SNAPSHOT = 44
        'PRINT SCREEN key 
        VK_INSERT = 45
        'INS key 
        VK_DELETE = 46
        'DEL key 
        VK_HELP = 47
        'HELP key 
        VK_0 = 48
        '0 key 
        VK_1 = 49
        '1 key 
        VK_2 = 50
        '2 key 
        VK_3 = 51
        '3 key 
        VK_4 = 52
        '4 key 
        VK_5 = 53
        '5 key 
        VK_6 = 54
        '6 key 
        VK_7 = 55
        '7 key 
        VK_8 = 56
        '8 key 
        VK_9 = 57
        '9 key 
        VK_A = 65
        'A key 
        VK_B = 66
        'B key 
        VK_C = 67
        'C key 
        VK_D = 68
        'D key 
        VK_E = 69
        'E key 
        VK_F = 70
        'F key 
        VK_G = 71
        'G key 
        VK_H = 72
        'H key 
        VK_I = 73
        'I key 
        VK_J = 74
        'J key 
        VK_K = 75
        'K key 
        VK_L = 76
        'L key 
        VK_M = 77
        'M key 
        VK_N = 78
        'N key 
        VK_O = 79
        'O key 
        VK_P = 80
        'P key 
        VK_Q = 81
        'Q key 
        VK_R = 82
        'R key 
        VK_S = 83
        'S key 
        VK_T = 84
        'T key 
        VK_U = 85
        'U key 
        VK_V = 86
        'V key 
        VK_W = 87
        'W key 
        VK_X = 88
        'X key 
        VK_Y = 89
        'Y key 
        VK_Z = 90
        'Z key 
        VK_NUMPAD0 = 96
        'Numeric keypad 0 key 
        VK_NUMPAD1 = 97
        'Numeric keypad 1 key 
        VK_NUMPAD2 = 98
        'Numeric keypad 2 key 
        VK_NUMPAD3 = 99
        'Numeric keypad 3 key 
        VK_NUMPAD4 = 100
        'Numeric keypad 4 key 
        VK_NUMPAD5 = 101
        'Numeric keypad 5 key 
        VK_NUMPAD6 = 102
        'Numeric keypad 6 key 
        VK_NUMPAD7 = 103
        'Numeric keypad 7 key 
        VK_NUMPAD8 = 104
        'Numeric keypad 8 key 
        VK_NUMPAD9 = 105
        'Numeric keypad 9 key 
        VK_SEPARATOR = 108
        'Separator key 
        VK_SUBTRACT = 109
        'Subtract key 
        VK_DECIMAL = 110
        'Decimal key 
        VK_DIVIDE = 111
        'Divide key 
        VK_F1 = 112
        'F1 key 
        VK_F2 = 113
        'F2 key 
        VK_F3 = 114
        'F3 key 
        VK_F4 = 115
        'F4 key 
        VK_F5 = 116
        'F5 key 
        VK_F6 = 117
        'F6 key 
        VK_F7 = 118
        'F7 key 
        VK_F8 = 119
        'F8 key 
        VK_F9 = 120
        'F9 key 
        VK_F10 = 121
        'F10 key 
        VK_F11 = 122
        'F11 key 
        VK_F12 = 123
        'F12 key 
        VK_SCROLL = 145
        'SCROLL LOCK key 
        VK_LSHIFT = 160
        'Left SHIFT key 
        VK_RSHIFT = 161
        'Right SHIFT key 
        VK_LCONTROL = 162
        'Left CONTROL key 
        VK_RCONTROL = 163
        'Right CONTROL key 
        VK_LMENU = 164
        'Left MENU key 
        VK_RMENU = 165
        'Right MENU key 
        VK_PLAY = 250
        'Play key 
        VK_ZOOM = 251
        'Zoom key 
    End Enum


    ''' <summary> 
    ''' MakeLParam Macro 
    ''' </summary> 
    Private Function MakeLParam(ByVal LoWord As Integer, ByVal HiWord As Integer) As Integer
        Return ((HiWord << 16) Or (LoWord And 65535))
    End Function

    Public Sub SendKey(ByVal Key As Keys)
        SendMessage(hWnd, WMessages.WM_KEYDOWN, Key, 0)
        SendMessage(hWnd, WMessages.WM_KEYUP, Key, 0)
    End Sub

    Public Sub SendString(ByVal TheString As String)
        For i As Integer = 0 To TheString.Length - 1
            SendKey(Char.GetNumericValue(TheString(i)))
        Next
    End Sub
    Public Sub SendClick(ByVal X As Integer, ByVal Y As Integer, Optional ByVal LeftClick As Boolean = True, Optional ByVal DoubleClick As Boolean = False)

        Dim LParam As Integer = MakeLParam(X, Y)

        Dim btnDown As Integer = 0
        Dim btnUp As Integer = 0

        If LeftClick Then
            btnDown = CInt(WMessages.WM_LBUTTONDOWN)
            btnUp = CInt(WMessages.WM_LBUTTONUP)
        Else
            btnDown = CInt(WMessages.WM_RBUTTONDOWN)
            btnUp = CInt(WMessages.WM_RBUTTONUP)
        End If

        SendMessage(hWnd, btnDown, 0, LParam)
        SendMessage(hWnd, btnUp, 0, LParam)
        'If it's a double Click... We send another click!!! w00t
        If DoubleClick Then
            SendMessage(hWnd, btnDown, 0, LParam)
            SendMessage(hWnd, btnUp, 0, LParam)
        End If

    End Sub

    Public Sub ExitGame()
        SendKey(Keys.VK_ESCAPE)
        SendKey(Keys.VK_UP)
        SendKey(Keys.VK_RETURN)
    End Sub

    Public Sub StartToLogin()
        'Clicks the Battle.Net button
        SendClick(400, 355)
    End Sub

    Public Sub LoginToCharSelect(ByVal Username As String, ByVal Password As String)
        'Click On Name textbox
        SendClick(400, 340)
        'Send the Name string
        SendString(Username)
        Threading.Thread.Sleep(50)
        'Click on Pass textbox
        SendClick(390, 390)
        'Send the Pass string
        SendString(Password)
        Threading.Thread.Sleep(50)
        'Connect Button
        SendClick(400, 470)
    End Sub

    Public Sub CharSelectToStart()
        'Exit Button
        SendClick(90, 550)
    End Sub

    Public Sub CharSelectToLobby(ByVal CharPos As Integer)
        'Select the right char
        Select Case CharPos
            Case 0
                '_X_|___
                '___|___
                '___|___
                '___|___
                SendClick(170, 135, , True)
            Case 1
                '___|_X_
                '___|___
                '___|___
                '___|___
                SendClick(440, 135, , True)
            Case 2
                '___|___
                '_X_|___
                '___|___
                '___|___
                SendClick(170, 235, , True)
            Case 3
                '___|___
                '___|_X_
                '___|___
                '___|___
                SendClick(440, 235, , True)

            Case 4
                '___|___
                '___|___
                '_X_|___
                '___|___
                SendClick(170, 325, , True)
            Case 5
                '___|___
                '___|___
                '___|_X_
                '___|___
                SendClick(440, 325, , True)
            Case 6
                '___|___
                '___|___
                '___|___
                '_X_|___
                SendClick(170, 425, , True)
            Case 7
                '___|___
                '___|___
                '___|___
                '___|_X_
                SendClick(440, 425, , True)
        End Select
        'Join Lobby
        'SendClick(700, 560)
    End Sub

    Public Sub LobbyToCharSelect()
        'Press Exit Button
        SendClick(430, 480)
    End Sub

    Public Sub LobbyToCreateGame(ByVal Name As String, ByVal Password As String, ByVal Description As String, ByVal Difficulty As Integer)
        'Create Button
        SendClick(590, 460)
        Threading.Thread.Sleep(50)
        'Name TextBox
        SendClick(460, 155)
        SendString(Name)
        Threading.Thread.Sleep(50)
        'PassWord textBox
        SendClick(450, 210)
        SendString(Password)
        Threading.Thread.Sleep(50)
        Select Case Difficulty
            Case 1 'Normal
                SendClick(440, 377)
            Case 2 'Nightmare
                SendClick(567, 377)
            Case 3 'Hell
                SendClick(708, 377)
        End Select
        Threading.Thread.Sleep(50)
        SendClick(690, 420)
    End Sub

    Public Sub LobbyToChannel(Optional ByVal ChannelName As String = "")
        'Enter Chat
        SendClick(90, 470)

        'User didn't specify a channel, we use battle.net's one
        If ChannelName = "" Then Exit Sub

        'Channel
        SendClick(570, 480)

        'Write ChannelName
        SendString(ChannelName)
        Threading.Thread.Sleep(50)
        SendClick(710, 415)
    End Sub

    Public Sub LobbyToJoinGame()
        SendClick(710, 460)
    End Sub

    Public Sub LobbyToInGame(ByVal GameName As String, Optional ByVal GamePassword As String = "")

        LobbyToJoinGame()
        EnterJoinGameInfos(GameName, GamePassword)
        'Join Game Button
        SendClick(635, 140)
    End Sub

    Public Sub EnterJoinGameInfos(ByVal GameName As String, Optional ByVal GamePassword As String = "")
        'GameName Textbox
        SendClick(445, 140)
        SendString(GameName)
        Threading.Thread.Sleep(50)
        'Password TextBox
        SendClick(635, 140)
        SendString(GamePassword)
    End Sub

    Public Sub ClickPleaseWaitButton()
        SendClick(400, 330)
    End Sub

    Public Sub LobbyToStart()
        LobbyToCharSelect()
        'Exit Button
        CharSelecttostart()
    End Sub

End Class
