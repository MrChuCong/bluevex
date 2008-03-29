Public Module GlobalModule

    Public LogSocketErrors As Boolean = False

    Public AvailableGUIModules As New Collection
    Public AvailableGameModules As New Collection
    Public AvailableChatModules As New Collection
    Public AvailableRealmModules As New Collection

    Public Main As frmMain
    Public PluginManagerForm As New frmPluginManager

    Public Diablo2WindowHandles As New Collections.Generic.SortedList(Of String, IntPtr)

    'Public Delegate Sub DestroyDelegate(ByVal client As tcpProxy)
    Public Enum ProxyType
        Chat = 1
        Realm = 2
        Game = 3
    End Enum

    'Public ChatListener As TcpProxyListener
    'Public RealmListener As TcpProxyListener
    'Public GameListener As TcpProxyListener

    Public LastCreateGameTime As DateTime
    Public GameCount As Integer = 0

    Function ToShort(ByVal Buffer() As Byte, ByVal Location As Integer) As Short
        Dim BinaryString As String = Convert.ToString(Buffer(Location), 2) & Convert.ToString(Buffer(Location + 1), 2)
        While BinaryString.Length < 16 : BinaryString = "0" & BinaryString : End While
        Return Convert.ToInt16(BinaryString, 2)
    End Function

    Function ToBytes(ByVal value As Short) As Byte()
        Dim BinaryString As String = Convert.ToString(value, 2)
        While BinaryString.Length < 16 : BinaryString = "0" & BinaryString : End While
        Dim Result(1) As Byte
        Result(0) = Convert.ToInt16(BinaryString.Substring(0, 8), 2)
        Result(1) = Convert.ToInt16(BinaryString.Substring(8, 8), 2)
        Return Result
    End Function

End Module

Public Module Log

    Public Delegate Sub WriteLineDelegate(ByVal Text As String)
    Sub WriteLine(ByVal Text As String)
        If Main.InvokeRequired Then
            Main.Invoke(New WriteLineDelegate(AddressOf WriteLine), New Object() {Text})
        Else
            MyRedVexInfo.WriteLog(vbNewLine & Text)
        End If
    End Sub

End Module

Public Module Geometry

    Public Function InRange(ByVal PointA As Point, ByVal PointB As Point, ByVal Distance As Integer) As Boolean
        Dim ASquared As Integer = (PointA.X - PointB.X) * (PointA.X - PointB.X)
        Dim BSquared As Integer = (PointA.Y - PointB.Y) * (PointA.Y - PointB.Y)
        Dim CSquared As Integer = Distance * Distance
        If ASquared * BSquared < CSquared Then Return True Else Return False
    End Function

End Module

Public Module GameHelpers

    'Public Function isUseless(ByVal type As NPCType) As Boolean
    '    Select Case type
    '        Case NPCType.Gorgon : Return True      ' Unused
    '        Case NPCType.Gorgon2 : Return True  ' Unused
    '        Case NPCType.Gorgon3 : Return True  ' Unused
    '        Case NPCType.Gorgon4 : Return True  ' Unused
    '        Case NPCType.ChaosHorde : Return True  ' Unused
    '        Case NPCType.ChaosHorde2 : Return True  ' Unused
    '        Case NPCType.ChaosHorde3 : Return True  ' Unused
    '        Case NPCType.ChaosHorde4 : Return True  ' Unused
    '        Case NPCType.DarkGuard : Return True  ' Unused
    '        Case NPCType.DarkGuard2 : Return True  ' Unused
    '        Case NPCType.DarkGuard3 : Return True  ' Unused
    '        Case NPCType.DarkGuard4 : Return True  ' Unused
    '        Case NPCType.DarkGuard5 : Return True  ' Unused
    '        Case NPCType.BloodMage : Return True  ' Unused
    '        Case NPCType.BloodMage2 : Return True  ' Unused
    '        Case NPCType.BloodMage3 : Return True  ' Unused
    '        Case NPCType.BloodMage4 : Return True  ' Unused
    '        Case NPCType.BloodMage5 : Return True  ' Unused
    '        Case NPCType.FireBeast : Return True  ' Unused
    '        Case NPCType.IceGlobe : Return True  ' Unused
    '        Case NPCType.LightningBeast : Return True  ' Unused
    '        Case NPCType.PoisonOrb : Return True  ' Unused
    '        Case NPCType.Chicken : Return True  ' Dummy
    '        Case NPCType.Rat : Return True  ' Dummy
    '        Case NPCType.Rogue : Return True  ' Dummy
    '        Case NPCType.HellMeteor : Return True  ' Dummy
    '        Case NPCType.Bird : Return True  ' Dummy
    '        Case NPCType.Bird2 : Return True  ' Dummy
    '        Case NPCType.Bat : Return True  ' Dummy
    '        Case NPCType.Cow : Return True  ' Dummy
    '        Case NPCType.Camel : Return True  ' Dummy
    '        Case NPCType.Act2Male : Return True  ' Dummy
    '        Case NPCType.Act2Female : Return True  ' Dummy
    '        Case NPCType.Act2Child : Return True  ' Dummy
    '        Case NPCType.Act2Guard : Return True  ' Dummy
    '        Case NPCType.Act2Vendor : Return True  ' Dummy
    '        Case NPCType.Act2Vendor2 : Return True  ' Dummy
    '        Case NPCType.Bug : Return True  ' Dummy
    '        Case NPCType.Scorpion : Return True  ' Dummy
    '        Case NPCType.Rogue2 : Return True  ' Dummy
    '        Case NPCType.Rogue3 : Return True  ' Dummy
    '        Case NPCType.Familiar : Return True  ' Dummy
    '        Case NPCType.Act3Male : Return True  ' Dummy
    '        Case NPCType.Act3Female : Return True  ' Dummy
    '        Case NPCType.Snake : Return True  ' Dummy
    '        Case NPCType.Parrot : Return True  ' Dummy
    '        Case NPCType.Fish : Return True  ' Dummy
    '        Case NPCType.EvilHole : Return True  ' Dummy
    '        Case NPCType.EvilHole2 : Return True  ' Dummy
    '        Case NPCType.EvilHole3 : Return True  ' Dummy
    '        Case NPCType.EvilHole4 : Return True  ' Dummy
    '        Case NPCType.EvilHole5 : Return True  ' Dummy
    '        Case NPCType.InvisoSpawner : Return True  ' Dummy
    '        Case NPCType.MiniSpider : Return True  ' Dummy
    '        Case NPCType.BoneWall : Return True  ' Dummy
    '        Case NPCType.SevenTombs : Return True  ' Dummy
    '        Case NPCType.SpiritMummy : Return True  ' Dummy
    '        Case NPCType.Act2Guard4 : Return True  ' Dummy
    '        Case NPCType.Act2Guard5 : Return True  ' Dummy
    '        Case NPCType.Window : Return True  ' Dummy
    '        Case NPCType.Window2 : Return True  ' Dummy
    '        Case NPCType.MephistoSpirit : Return True  ' Dummy
    '        Case NPCType.InvisiblePet : Return True  ' Dummy
    '        Case NPCType.DemonHole : Return True  ' Dummy
    '        Case NPCType.FireboltTrap : Return True  ' A trap
    '        Case NPCType.HorzMissileTrap : Return True  ' A trap
    '        Case NPCType.VertMissileTrap : Return True  ' A trap
    '        Case NPCType.PoisonCloudTrap : Return True  ' A trap
    '        Case NPCType.LightningTrap : Return True  ' A trap
    '        Case NPCType.MeleeTrap : Return True  ' A trap
    '        Case NPCType.Hephasto : Return True  ' ????
    '        Case NPCType.MinionExp : Return True  ' ????
    '        Case NPCType.SlayerExp : Return True  ' ????
    '        Case NPCType.SuccubusExp : Return True  ' ????


    '        Case NPCType.DeckardCain : Return True  ' In Trist?
    '        Case NPCType.DarkWanderer : Return True  ' Act 3 guy outside town
    '        Case NPCType.Tyrael : Return True  ' Kill Him in act 4 for quest?
    '        Case NPCType.InjuredBarbarian : Return True  ' Dummy
    '        Case NPCType.InjuredBarbarian2 : Return True  ' Dummy
    '        Case NPCType.InjuredBarbarian3 : Return True  ' Dummy
    '        Case NPCType.Act5Townguard : Return True  ' Dummy
    '        Case NPCType.Act5Townguard2 : Return True  ' Dummy
    '        Case NPCType.Tyrael3 : Return True  ' Appears when u kill baal?

    '        Case NPCType.NecroSkeleton : Return True  ' Necro summon
    '        Case NPCType.NecroMage : Return True  ' Necro summon

    '        Case NPCType.Hydra : Return True
    '        Case NPCType.Hydra2 : Return True
    '        Case NPCType.Hydra3 : Return True

    '        Case NPCType.Guard : Return True  ' Act 2 merc?
    '        Case NPCType.IronWolf : Return True  ' Act 3 merc?
    '        Case NPCType.Act5Hireling1Hand : Return True  ' Act 5 merc?
    '        Case NPCType.Act5Hireling2Hand : Return True ' Act 5 merc?
    '        Case Else
    '            Return False
    '    End Select
    'End Function

    'Public Function isTownFolk(ByVal type As NPCType) As Boolean
    '    Select Case type
    '        Case NPCType.Charsi, _
    '            NPCType.Gheed, _
    '         NPCType.Warriv, _
    '         NPCType.DeckardCain5, _
    '         NPCType.Kashya, _
    '         NPCType.Akara, _
    '         NPCType.Greiz, _
    '         NPCType.Elzix, _
    '         NPCType.Drognan, _
    '         NPCType.Kaelan, _
    '         NPCType.Jerhyn, _
    '         NPCType.Warriv2, _
    '         NPCType.Lysander, _
    '         NPCType.Fara, _
    '         NPCType.DeckardCain2, _
    '         NPCType.Atma, _
    '         NPCType.Geglash, _
    '         NPCType.Meshif, _
    '         NPCType.Alkor, _
    '         NPCType.Asheara, _
    '         NPCType.Ormus, _
    '         NPCType.DeckardCain3, _
    '         NPCType.Meshif2, _
    '         NPCType.Hratli, _
    '         NPCType.Tyrael2, _
    '         NPCType.DeckardCain4, _
    '         NPCType.Halbu, _
    '         NPCType.Jamella, _
    '         NPCType.Malah, _
    '         NPCType.QualKehk, _
    '         NPCType.Larzuk, _
    '         NPCType.DeckardCain6, _
    '         NPCType.Drehya ' Anya?? wtf??  and who the hell is Drehya2?
    '            Return True
    '        Case Else
    '            Return False
    '    End Select
    'End Function

    'Public Function isMonster(ByVal type As NPCType) As Boolean
    '    If isTownFolk(type) Or isUseless(type) Then
    '        Return False
    '    Else
    '        Return True
    '    End If
    'End Function

End Module