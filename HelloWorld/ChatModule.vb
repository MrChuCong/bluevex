Public Class ChatModule
    Implements bluevex.ichatmodule

#Region " Module Info "

    Public ReadOnly Property Author() As String Implements BlueVex.IChatModule.Author
        Get
            Return "Pleh"
        End Get
    End Property

    Public ReadOnly Property AboutInfo() As String Implements BlueVex.IChatModule.AboutInfo
        Get
            Return "Hello World Example Plugin By Pleh"
        End Get
    End Property

    Public ReadOnly Property Name() As String Implements BlueVex.IChatModule.Name
        Get
            Return "Hello World Chat Module"
        End Get
    End Property

    Public ReadOnly Property ReleaseDate() As String Implements BlueVex.IChatModule.ReleaseDate
        Get
            Return "January 2008"
        End Get
    End Property

    Public ReadOnly Property Version() As String Implements BlueVex.IChatModule.Version
        Get
            Return "1.0"
        End Get
    End Property

#End Region

    Private WithEvents Chat As BlueVex.IChat

    Public Sub Initialize(ByRef Chat As BlueVex.IChat) Implements BlueVex.IChatModule.Initialize
        Me.Chat = Chat
    End Sub

    Public Sub Destroy() Implements BlueVex.IChatModule.Destroy

    End Sub

    Public Sub Update() Implements BlueVex.IChatModule.Update

    End Sub

    Private Sub Chat_OnNewsInfoRequest(ByVal Packet As BnetClient.newsinforequest) Handles Chat.OnNewsInfoRequest
        Chat.AddNews("BlueVex" & Chr(10) & "Hello World")
        Chat.AddNews("BlueVex" & Chr(10) & "Hello World2")
    End Sub



End Class
