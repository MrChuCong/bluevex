Public Class GameModule
    Implements BlueVex.IGameModule

#Region " Module Info "

    Public ReadOnly Property Author() As String Implements BlueVex.IGameModule.Author
        Get
            Return "Pleh"
        End Get
    End Property

    Public ReadOnly Property AboutInfo() As String Implements BlueVex.IGameModule.AboutInfo
        Get
            Return "Hello World Example Plugin By Pleh"
        End Get
    End Property

    Public ReadOnly Property Name() As String Implements BlueVex.IGameModule.Name
        Get
            Return "Hello World Game Module"
        End Get
    End Property

    Public ReadOnly Property ReleaseDate() As String Implements BlueVex.IGameModule.ReleaseDate
        Get
            Return "January 2008"
        End Get
    End Property

    Public ReadOnly Property Version() As String Implements BlueVex.IGameModule.Version
        Get
            Return "1.1"
        End Get
    End Property

#End Region

    Private WithEvents Game As BlueVex.IGame

    Public Sub Initialize(ByRef Game As BlueVex.IGame) Implements BlueVex.IGameModule.Initialize
        Me.Game = Game
        AddHandler Game.OnSendMessage, AddressOf OnSendMessage
        AddHandler Game.OnSendPacket, AddressOf OnSendPacket
        AddHandler Game.OnReceivePacket, AddressOf OnReceivePacket
        AddHandler Game.OnSwitchWeapons, AddressOf OnSwitchWeapons
    End Sub

    Sub OnSendMessage(ByVal Packet As GameClient.SendMessage)
        If Packet.Message.StartsWith(".hello") Then
            'Make the Game Client Think Its Received a Message
            Game.ReceiveMessage("ÿc3BlueVex", "Hello World")
            'Send a message to the server that all players will see
            Game.SendMessage("Hello")
        End If
    End Sub

    Sub OnReceivePacket(ByRef Packet As BlueVex.Packet)
        Select Case Packet.Data(0)
            Case Else
                Game.WriteToLog("ToClient: " & Packet.ToString(Packet.Length))
        End Select
    End Sub

    Sub OnSendPacket(ByRef Packet As BlueVex.Packet)
        Select Case Packet.Data(0)
            Case Else
                Game.WriteToLog("ToServer: " & Packet.ToString(Packet.Length))
        End Select
    End Sub

    Sub OnSwitchWeapons(ByVal Packet As GameClient.SwitchWeapons)
        Game.ReceiveMessage("ÿc2BlueVex", "Switched Weapons")
    End Sub

    Private Sub Game_OnItemAction(ByVal Packet As GameServer.ItemAction) Handles Game.OnItemAction
        Game.ReceiveMessage("ÿc1BlueVex", Packet.ToInfoString())
    End Sub

End Class
