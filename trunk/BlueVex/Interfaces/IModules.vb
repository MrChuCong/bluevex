Public Interface IGameModule
    Sub Initialize(ByRef Game As IGame)
    Sub Destroy()
    Sub Update()
    ReadOnly Property AboutInfo() As String
    ReadOnly Property Author() As String
    ReadOnly Property Name() As String
    ReadOnly Property ReleaseDate() As String
    ReadOnly Property Version() As String
End Interface

Public Interface IChatModule
    Sub Initialize(ByRef Chat As IChat)
    Sub Destroy()
    Sub Update()
    ReadOnly Property AboutInfo() As String
    ReadOnly Property Author() As String
    ReadOnly Property Name() As String
    ReadOnly Property ReleaseDate() As String
    ReadOnly Property Version() As String
End Interface

Public Interface IRealmModule
    Sub Initialize(ByRef Realm As IRealm)
    Sub Destroy()
    ReadOnly Property AboutInfo() As String
    ReadOnly Property Author() As String
    ReadOnly Property Name() As String
    ReadOnly Property ReleaseDate() As String
    ReadOnly Property Version() As String
End Interface

Public Interface IGUIModule
    Sub Initialize(ByRef GUI As IGUI)
    ReadOnly Property AboutInfo() As String
    ReadOnly Property Author() As String
    ReadOnly Property Name() As String
    ReadOnly Property ReleaseDate() As String
    ReadOnly Property Version() As String
End Interface