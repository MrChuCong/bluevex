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

    Public Function isBoss(ByVal type As D2Data.NPCCode)

        Select Case type

            Case D2Data.NPCCode.Andariel : Return True
            Case D2Data.NPCCode.Duriel : Return True
            Case D2Data.NPCCode.Mephisto : Return True
            Case D2Data.NPCCode.Diablo : Return True
            Case D2Data.NPCCode.BaalCrab : Return True
            Case D2Data.NPCCode.BaalCrabToStairs : Return True

                'Secondary Bosses.
            Case D2Data.NPCCode.Nihlathak : Return True
            Case D2Data.NPCCode.TheSmith : Return True
            Case D2Data.NPCCode.Tyrael : Return True
            Case D2Data.NPCCode.Summoner : Return True
            Case D2Data.NPCCode.Hephasto : Return True
            Case D2Data.NPCCode.Radament : Return True

                'Special Events
            Case D2Data.NPCCode.DiabloClone : Return True
            Case D2Data.NPCCode.UberBaal : Return True
            Case D2Data.NPCCode.UberDiablo : Return True
            Case D2Data.NPCCode.UberDuriel : Return True
            Case D2Data.NPCCode.UberIzual : Return True
            Case D2Data.NPCCode.UberMephisto : Return True
            Case Else
                Return False
        End Select
    End Function
    Public Function isUseless(ByVal type As D2Data.NPCCode) As Boolean
        Select Case type
            Case D2Data.NPCCode.Gorgon : Return True    'Unused
            Case D2Data.NPCCode.Gorgon2 : Return True   'Unused
            Case D2Data.NPCCode.Gorgon3 : Return True   'Unused
            Case D2Data.NPCCode.Gorgon4 : Return True   'Unused
            Case D2Data.NPCCode.ChaosHorde : Return True   'Unused
            Case D2Data.NPCCode.ChaosHorde2 : Return True   'Unused
            Case D2Data.NPCCode.ChaosHorde3 : Return True   'Unused
            Case D2Data.NPCCode.ChaosHorde4 : Return True   'Unused
            Case D2Data.NPCCode.DarkGuard : Return True   'Unused
            Case D2Data.NPCCode.DarkGuard2 : Return True   'Unused
            Case D2Data.NPCCode.DarkGuard3 : Return True   'Unused
            Case D2Data.NPCCode.DarkGuard4 : Return True   'Unused
            Case D2Data.NPCCode.DarkGuard5 : Return True   'Unused
            Case D2Data.NPCCode.BloodMage : Return True   'Unused
            Case D2Data.NPCCode.BloodMage2 : Return True   'Unused
            Case D2Data.NPCCode.BloodMage3 : Return True   'Unused
            Case D2Data.NPCCode.BloodMage4 : Return True   'Unused
            Case D2Data.NPCCode.BloodMage5 : Return True   'Unused
            Case D2Data.NPCCode.FireBeast : Return True   'Unused
            Case D2Data.NPCCode.IceGlobe : Return True   'Unused
            Case D2Data.NPCCode.LightningBeast : Return True   'Unused
            Case D2Data.NPCCode.PoisonOrb : Return True   'Unused
            Case D2Data.NPCCode.Chicken : Return True   'Dummy
            Case D2Data.NPCCode.Rat : Return True   'Dummy
            Case D2Data.NPCCode.Rogue : Return True   'Dummy
            Case D2Data.NPCCode.HellMeteor : Return True   'Dummy
            Case D2Data.NPCCode.Bird : Return True   'Dummy
            Case D2Data.NPCCode.Bird2 : Return True   'Dummy
            Case D2Data.NPCCode.Bat : Return True   'Dummy
            Case D2Data.NPCCode.Cow : Return True   'Dummy
            Case D2Data.NPCCode.Camel : Return True   'Dummy
            Case D2Data.NPCCode.Act2Male : Return True   'Dummy
            Case D2Data.NPCCode.Act2Female : Return True   'Dummy
            Case D2Data.NPCCode.Act2Child : Return True   'Dummy
            Case D2Data.NPCCode.Act2Guard : Return True   'Dummy
            Case D2Data.NPCCode.Act2Vendor : Return True   'Dummy
            Case D2Data.NPCCode.Act2Vendor2 : Return True   'Dummy
            Case D2Data.NPCCode.Bug : Return True   'Dummy
            Case D2Data.NPCCode.Scorpion : Return True   'Dummy
            Case D2Data.NPCCode.Rogue2 : Return True   'Dummy
            Case D2Data.NPCCode.Rogue3 : Return True   'Dummy
            Case D2Data.NPCCode.Familiar : Return True   'Dummy
            Case D2Data.NPCCode.Act3Male : Return True   'Dummy
            Case D2Data.NPCCode.Act3Female : Return True   'Dummy
            Case D2Data.NPCCode.Snake : Return True   'Dummy
            Case D2Data.NPCCode.Parrot : Return True   'Dummy
            Case D2Data.NPCCode.Fish : Return True   'Dummy
            Case D2Data.NPCCode.EvilHole : Return True   'Dummy
            Case D2Data.NPCCode.EvilHole2 : Return True   'Dummy
            Case D2Data.NPCCode.EvilHole3 : Return True   'Dummy
            Case D2Data.NPCCode.EvilHole4 : Return True   'Dummy
            Case D2Data.NPCCode.EvilHole5 : Return True   'Dummy
            Case D2Data.NPCCode.InvisoSpawner : Return True   'Dummy
            Case D2Data.NPCCode.MiniSpider : Return True   'Dummy
            Case D2Data.NPCCode.BoneWall : Return True   'Dummy
            Case D2Data.NPCCode.SevenTombs : Return True   'Dummy
            Case D2Data.NPCCode.SpiritMummy : Return True   'Dummy
            Case D2Data.NPCCode.Act2Guard4 : Return True   'Dummy
            Case D2Data.NPCCode.Act2Guard5 : Return True   'Dummy
            Case D2Data.NPCCode.Window : Return True   'Dummy
            Case D2Data.NPCCode.Window2 : Return True   'Dummy
            Case D2Data.NPCCode.MephistoSpirit : Return True   'Dummy
            Case D2Data.NPCCode.InvisiblePet : Return True   'Dummy
            Case D2Data.NPCCode.DemonHole : Return True   'Dummy
            Case D2Data.NPCCode.FireboltTrap : Return True   'A trap
            Case D2Data.NPCCode.HorzMissileTrap : Return True   'A trap
            Case D2Data.NPCCode.VertMissileTrap : Return True   'A trap
            Case D2Data.NPCCode.PoisonCloudTrap : Return True   'A trap
            Case D2Data.NPCCode.LightningTrap : Return True   'A trap
            Case D2Data.NPCCode.MeleeTrap : Return True   'A trap
            Case D2Data.NPCCode.Hephasto : Return True   '????
            Case D2Data.NPCCode.MinionExp : Return True   '????
            Case D2Data.NPCCode.SlayerExp : Return True   '????
            Case D2Data.NPCCode.SuccubusExp : Return True   '????

            Case D2Data.NPCCode.DeckardCain : Return True   'In Trist?
            Case D2Data.NPCCode.DarkWanderer : Return True   'Act 3 guy outside town
                'Case D2Data.NPCCode.Tyrael : Return True   'Kill Him in act 4 for quest?
            Case D2Data.NPCCode.InjuredBarbarian : Return True   'Dummy
            Case D2Data.NPCCode.InjuredBarbarian2 : Return True   'Dummy
            Case D2Data.NPCCode.InjuredBarbarian3 : Return True   'Dummy
            Case D2Data.NPCCode.Act5Townguard : Return True   'Dummy
            Case D2Data.NPCCode.Act5Townguard2 : Return True   'Dummy
            Case D2Data.NPCCode.Tyrael3 : Return True   'Appears when u kill baal?

            Case D2Data.NPCCode.NecroSkeleton : Return True   'Necro summon
            Case D2Data.NPCCode.NecroMage : Return True   'Necro summon

            Case D2Data.NPCCode.Hydra : Return True
            Case D2Data.NPCCode.Hydra2 : Return True
            Case D2Data.NPCCode.Hydra3 : Return True

            Case D2Data.NPCCode.Guard : Return True   'Act 2 merc?
            Case D2Data.NPCCode.IronWolf : Return True   'Act 3 merc?
            Case D2Data.NPCCode.Act5Hireling1Hand : Return True   'Act 5 merc?
            Case D2Data.NPCCode.Act5Hireling2Hand : Return True  'Act 5 merc?

            Case Else
                Return False
        End Select
    End Function
    Public Function isTownFolk(ByVal type As D2Data.NPCCode) As Boolean
        Select Case type
            Case D2Data.NPCCode.Charsi, _
                D2Data.NPCCode.Gheed, _
             D2Data.NPCCode.Warriv, _
             D2Data.NPCCode.DeckardCain5, _
             D2Data.NPCCode.Kashya, _
             D2Data.NPCCode.Akara, _
             D2Data.NPCCode.Greiz, _
             D2Data.NPCCode.Elzix, _
             D2Data.NPCCode.Drognan, _
             D2Data.NPCCode.Kaelan, _
             D2Data.NPCCode.Jerhyn, _
             D2Data.NPCCode.Warriv2, _
             D2Data.NPCCode.Lysander, _
             D2Data.NPCCode.Fara, _
             D2Data.NPCCode.DeckardCain2, _
             D2Data.NPCCode.Atma, _
             D2Data.NPCCode.Geglash, _
             D2Data.NPCCode.Meshif, _
             D2Data.NPCCode.Alkor, _
             D2Data.NPCCode.Asheara, _
             D2Data.NPCCode.Ormus, _
             D2Data.NPCCode.DeckardCain3, _
             D2Data.NPCCode.Meshif2, _
             D2Data.NPCCode.Hratli, _
             D2Data.NPCCode.Tyrael2, _
             D2Data.NPCCode.DeckardCain4, _
             D2Data.NPCCode.Halbu, _
             D2Data.NPCCode.Jamella, _
             D2Data.NPCCode.Malah, _
             D2Data.NPCCode.QualKehk, _
             D2Data.NPCCode.Larzuk, _
             D2Data.NPCCode.DeckardCain6, _
             D2Data.NPCCode.Drehya ' Anya?? wtf??  and who the hell is Drehya2?
                Return True
            Case Else
                Return False
        End Select
    End Function
    Public Function isMonster(ByVal type As D2Data.NPCCode) As Boolean
        If isTownFolk(type) Or isUseless(type) Or isBoss(type) Then
            Return False
        Else
            Return True
        End If
    End Function

End Module