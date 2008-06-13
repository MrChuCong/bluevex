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
        Dim ASquared As ULong = (PointA.X - PointB.X) * (PointA.X - PointB.X)
        Dim BSquared As ULong = (PointA.Y - PointB.Y) * (PointA.Y - PointB.Y)
        Dim CSquared As ULong = Distance * Distance
        If ASquared * BSquared < CSquared Then Return True Else Return False
    End Function

    Public Function CalculateDistance(ByVal PointA As Point, ByVal PointB As Point) As Integer
        Return Math.Sqrt((PointA.X - PointB.X) * (PointA.X - PointB.X) + (PointA.Y - PointB.Y) * (PointA.Y - PointB.Y))
    End Function
End Module

Public Module GameHelpers

    Public Function GetNextlevel(ByVal AreaID As D2Data.AreaLevel, ByRef ObjectType As Integer) As Integer
        '1 =Level, 2 = Object, 3 = NPC
        ObjectType = 1
        Select Case AreaID
            'Act1
            Case D2Data.AreaLevel.RogueEncampment
                Return D2Data.AreaLevel.BloodMoor
            Case D2Data.AreaLevel.BloodMoor
                Return D2Data.AreaLevel.ColdPlains
            Case D2Data.AreaLevel.ColdPlains
                Return D2Data.AreaLevel.StonyField
            Case D2Data.AreaLevel.StonyField
                Return D2Data.AreaLevel.UndergroundPassageLevel1
            Case D2Data.AreaLevel.UndergroundPassageLevel1
                Return D2Data.AreaLevel.DarkWood
            Case D2Data.AreaLevel.DarkWood
                Return D2Data.AreaLevel.BlackMarsh
            Case D2Data.AreaLevel.BlackMarsh
                Return D2Data.AreaLevel.TamoeHighland
            Case D2Data.AreaLevel.TamoeHighland
                Return D2Data.AreaLevel.MonasteryGate
            Case D2Data.AreaLevel.MonasteryGate
                Return D2Data.AreaLevel.OuterCloister
            Case D2Data.AreaLevel.OuterCloister
                Return D2Data.AreaLevel.Barracks
            Case D2Data.AreaLevel.Barracks
                Return D2Data.AreaLevel.JailLevel1
            Case D2Data.AreaLevel.JailLevel1
                Return D2Data.AreaLevel.JailLevel2
            Case D2Data.AreaLevel.JailLevel2
                Return D2Data.AreaLevel.JailLevel3
            Case D2Data.AreaLevel.JailLevel3
                Return D2Data.AreaLevel.InnerCloister
            Case D2Data.AreaLevel.InnerCloister
                Return D2Data.AreaLevel.Cathedral
            Case D2Data.AreaLevel.Cathedral
                Return D2Data.AreaLevel.CatacombsLevel1
            Case D2Data.AreaLevel.CatacombsLevel1
                Return D2Data.AreaLevel.CatacombsLevel2
            Case D2Data.AreaLevel.CatacombsLevel2
                Return D2Data.AreaLevel.CatacombsLevel3
            Case D2Data.AreaLevel.CatacombsLevel3
                Return D2Data.AreaLevel.CatacombsLevel4
                'Act1 Caves
            Case D2Data.AreaLevel.CaveLevel1
                Return D2Data.AreaLevel.CaveLevel2
            Case D2Data.AreaLevel.HoleLevel1
                Return D2Data.AreaLevel.HoleLevel2
            Case D2Data.AreaLevel.ForgottenTower
                Return D2Data.AreaLevel.TowerCellarLevel1
            Case D2Data.AreaLevel.TowerCellarLevel1
                Return D2Data.AreaLevel.TowerCellarLevel2
            Case D2Data.AreaLevel.TowerCellarLevel2
                Return D2Data.AreaLevel.TowerCellarLevel3
            Case D2Data.AreaLevel.TowerCellarLevel3
                Return D2Data.AreaLevel.TowerCellarLevel4
            Case D2Data.AreaLevel.TowerCellarLevel4
                Return D2Data.AreaLevel.TowerCellarLevel5
            Case D2Data.AreaLevel.PitLevel1
                Return D2Data.AreaLevel.PitLevel2
                'Act2
            Case D2Data.AreaLevel.LutGholein
                Return D2Data.AreaLevel.RockyWaste
            Case D2Data.AreaLevel.RockyWaste
                Return D2Data.AreaLevel.DryHills
            Case D2Data.AreaLevel.DryHills
                Return D2Data.AreaLevel.FarOasis
            Case D2Data.AreaLevel.FarOasis
                Return D2Data.AreaLevel.LostCity
            Case D2Data.AreaLevel.LostCity
                Return D2Data.AreaLevel.ValleyOfSnakes
                'Act2 Caves
            Case D2Data.AreaLevel.SewersLevel1Act2
                Return D2Data.AreaLevel.SewersLevel2Act2
            Case D2Data.AreaLevel.SewersLevel2Act2
                Return D2Data.AreaLevel.SewersLevel3Act2
            Case D2Data.AreaLevel.HallsOfTheDeadLevel1
                Return D2Data.AreaLevel.HallsOfTheDeadLevel2
            Case D2Data.AreaLevel.HallsOfTheDeadLevel2
                Return 60 'HallsOfTheDeadLevel3
            Case D2Data.AreaLevel.MaggotLairLevel1
                Return D2Data.AreaLevel.MaggotLairLevel2
            Case D2Data.AreaLevel.MaggotLairLevel2
                Return D2Data.AreaLevel.MaggotLairLevel3
            Case D2Data.AreaLevel.StonyTombLevel1
                Return D2Data.AreaLevel.StonyTombLevel2
            Case D2Data.AreaLevel.ClawViperTempleLevel1
                Return D2Data.AreaLevel.ClawViperTempleLevel2
            Case D2Data.AreaLevel.HaremLevel1
                Return D2Data.AreaLevel.HaremLevel2
            Case D2Data.AreaLevel.HaremLevel2
                Return D2Data.AreaLevel.PalaceCellarLevel1
            Case D2Data.AreaLevel.PalaceCellarLevel1
                Return D2Data.AreaLevel.PalaceCellarLevel2
            Case D2Data.AreaLevel.PalaceCellarLevel2
                Return D2Data.AreaLevel.PalaceCellarLevel3
            Case D2Data.AreaLevel.PalaceCellarLevel3
                ObjectType = 2
                Return D2Data.GameObjectID.ArcaneSanctuaryPortal
            Case D2Data.AreaLevel.ArcaneSanctuary
                ObjectType = 2
                Return 357
                'Act3
            Case D2Data.AreaLevel.KurastDocks
                Return D2Data.AreaLevel.SpiderForest
            Case D2Data.AreaLevel.SpiderForest
                Return D2Data.AreaLevel.GreatMarsh
            Case D2Data.AreaLevel.GreatMarsh
                Return D2Data.AreaLevel.FlayerJungle
            Case D2Data.AreaLevel.FlayerJungle
                Return D2Data.AreaLevel.LowerKurast
            Case D2Data.AreaLevel.LowerKurast
                Return D2Data.AreaLevel.KurastBazaar
            Case D2Data.AreaLevel.KurastBazaar
                Return D2Data.AreaLevel.UpperKurast
            Case D2Data.AreaLevel.UpperKurast
                Return D2Data.AreaLevel.KurastCauseway
            Case D2Data.AreaLevel.KurastCauseway
                Return D2Data.AreaLevel.Travincal
            Case D2Data.AreaLevel.Travincal
                ObjectType = 2
                Return 386
            Case D2Data.AreaLevel.DuranceOfHateLevel1
                Return D2Data.AreaLevel.DuranceOfHateLevel2
            Case D2Data.AreaLevel.DuranceOfHateLevel2
                Return D2Data.AreaLevel.DuranceOfHateLevel3
            Case D2Data.AreaLevel.FlayerDungeonLevel1
                Return D2Data.AreaLevel.FlayerDungeonLevel2
            Case D2Data.AreaLevel.FlayerDungeonLevel2
                Return D2Data.AreaLevel.FlayerDungeonLevel3
            Case D2Data.AreaLevel.SewersLevel1Act3
                Return D2Data.AreaLevel.SewersLevel2Act3
            Case D2Data.AreaLevel.SwampyPitLevel1
                Return D2Data.AreaLevel.SwampyPitLevel2
            Case D2Data.AreaLevel.SwampyPitLevel2
                Return D2Data.AreaLevel.SwampyPitLevel3
                'A4
            Case D2Data.AreaLevel.ThePandemoniumFortress
                Return D2Data.AreaLevel.OuterSteppes
            Case D2Data.AreaLevel.OuterSteppes
                Return D2Data.AreaLevel.PlainsOfDespair
            Case D2Data.AreaLevel.PlainsOfDespair
                Return D2Data.AreaLevel.CityOfTheDamned
            Case D2Data.AreaLevel.CityOfTheDamned
                Return D2Data.AreaLevel.RiverOfFlame
            Case D2Data.AreaLevel.RiverOfFlame
                Return D2Data.AreaLevel.ChaosSanctuary
            Case D2Data.AreaLevel.ChaosSanctuary
                ObjectType = 2
                Return D2Data.GameObjectID.DiabloStartPoint
                'Act 5
            Case D2Data.AreaLevel.Harrogath
                Return D2Data.AreaLevel.BloodyFoothills
            Case D2Data.AreaLevel.BloodyFoothills
                Return D2Data.AreaLevel.FrigidHighlands
            Case D2Data.AreaLevel.FrigidHighlands
                Return D2Data.AreaLevel.ArreatPlateau
            Case D2Data.AreaLevel.ArreatPlateau
                Return D2Data.AreaLevel.CrystallinePassage
            Case D2Data.AreaLevel.CrystallinePassage
                Return D2Data.AreaLevel.GlacialTrail
            Case D2Data.AreaLevel.GlacialTrail
                Return D2Data.AreaLevel.FrozenTundra
            Case D2Data.AreaLevel.FrozenTundra
                Return D2Data.AreaLevel.TheAncientsWay
            Case D2Data.AreaLevel.TheAncientsWay
                Return D2Data.AreaLevel.ArreatSummit
            Case D2Data.AreaLevel.ArreatSummit
                Return D2Data.AreaLevel.TheWorldStoneKeepLevel1
            Case D2Data.AreaLevel.TheWorldStoneKeepLevel1
                Return D2Data.AreaLevel.TheWorldStoneKeepLevel2
            Case D2Data.AreaLevel.TheWorldStoneKeepLevel2
                Return D2Data.AreaLevel.TheWorldStoneKeepLevel3
            Case D2Data.AreaLevel.TheWorldStoneKeepLevel3
                Return D2Data.AreaLevel.ThroneOfDestruction
            Case D2Data.AreaLevel.ThroneOfDestruction
                ObjectType = 3
                Return D2Data.NPCCode.BaalThrone
            Case D2Data.AreaLevel.NihlathaksTemple
                Return D2Data.AreaLevel.HallsOfAnguish
            Case D2Data.AreaLevel.HallsOfAnguish
                Return D2Data.AreaLevel.HallsOfPain
            Case D2Data.AreaLevel.HallsOfPain
                Return D2Data.AreaLevel.HallsOfVaught
            Case Else : Return Nothing
        End Select
    End Function
    Public Function GetNextQuest(ByVal AreaID As D2Data.AreaLevel, ByRef ObjectType As Integer) As Integer
        '1 =Level, 2 = Object, 3 = NPC
        ObjectType = 1
        Select Case AreaID
            'Act1
            Case D2Data.AreaLevel.BloodMoor
                Return D2Data.AreaLevel.DenOfEvil
            Case D2Data.AreaLevel.ColdPlains
                Return D2Data.AreaLevel.BurialGrounds
            Case D2Data.AreaLevel.StonyField
                ObjectType = 2
                Return 17
            Case D2Data.AreaLevel.UndergroundPassageLevel1
                Return D2Data.AreaLevel.UndergroundPassageLevel2
            Case D2Data.AreaLevel.DarkWood
                ObjectType = 2
                Return 30
            Case D2Data.AreaLevel.BlackMarsh
                Return D2Data.AreaLevel.ForgottenTower
            Case D2Data.AreaLevel.TamoeHighland
                Return D2Data.AreaLevel.PitLevel1
            Case D2Data.AreaLevel.Barracks
                ObjectType = 2
                Return 108
                'Act2
            Case D2Data.AreaLevel.DryHills
                Return D2Data.AreaLevel.HallsOfTheDeadLevel1
            Case D2Data.AreaLevel.FarOasis
                Return D2Data.AreaLevel.MaggotLairLevel1
            Case D2Data.AreaLevel.SewersLevel3Act2
                ObjectType = 2
                Return 355
            Case 60 'HallsOfTheDeadLevel3
                ObjectType = 2
                Return 354
            Case D2Data.AreaLevel.MaggotLairLevel3
                ObjectType = 2
                Return 356
            Case D2Data.AreaLevel.TalRashasTomb1, D2Data.AreaLevel.TalRashasTomb2, D2Data.AreaLevel.TalRashasTomb3, D2Data.AreaLevel.TalRashasTomb4, D2Data.AreaLevel.TalRashasTomb5, D2Data.AreaLevel.TalRashasTomb6, D2Data.AreaLevel.TalRashasTomb7
                ObjectType = 2
                Return 152
            Case D2Data.AreaLevel.ArcaneSanctuary
                ObjectType = 2
                Return 357
                'Act3
            Case D2Data.AreaLevel.SpiderForest
                Return D2Data.AreaLevel.SpiderCavern
            Case D2Data.AreaLevel.FlayerJungle
                ObjectType = 2
                Return 252
            Case D2Data.AreaLevel.KurastBazaar
                ObjectType = 2
                Return 195
            Case D2Data.AreaLevel.UpperKurast
                Return D2Data.AreaLevel.SewersLevel1Act3
            Case D2Data.AreaLevel.SewersLevel2Act3
                ObjectType = 2
                Return 405
            Case D2Data.AreaLevel.SwampyPitLevel3
                ObjectType = 2
                Return 397
            Case D2Data.AreaLevel.FlayerDungeonLevel3
                ObjectType = 2
                Return 406
            Case D2Data.AreaLevel.RuinedTemple
                ObjectType = 2
                Return 193
            Case D2Data.AreaLevel.Travincal
                ObjectType = 2
                Return 386
                'A4
            Case D2Data.AreaLevel.PlainsOfDespair
                ObjectType = 3
                Return 256
            Case D2Data.AreaLevel.RiverOfFlame
                ObjectType = 3
                Return 775
            Case D2Data.AreaLevel.ChaosSanctuary
                ObjectType = 2
                Return D2Data.GameObjectID.DiabloStartPoint
                'A5
            Case D2Data.AreaLevel.BloodyFoothills
                Return 824 'Tile!
            Case D2Data.AreaLevel.CrystallinePassage
                Return D2Data.AreaLevel.FrozenRiver
            Case D2Data.AreaLevel.FrozenRiver
                ObjectType = 3
                Return 793
            Case D2Data.AreaLevel.HallsOfVaught
                ObjectType = 2
                Return D2Data.GameObjectID.NihlathakWildernessStartPosition
        End Select
        Return Nothing
    End Function

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

    Public Structure ChatColors

        Public Const MessedUpWhite As String = "ÿc."
        Public Const BrightWhite As String = "ÿc/"
        Public Const Purple As String = "ÿc;"
        Public Const DarkGreen As String = "ÿc:"
        Public Const LightGrey As String = "ÿc0"
        Public Const Red As String = "ÿc1"
        Public Const SetGreen As String = "ÿc2"
        Public Const MagicBlue As String = "ÿc3"
        Public Const UniqueGold As String = "ÿc4"
        Public Const SocketedGrey As String = "ÿc5"
        Public Const Black As String = "ÿc6"
        Public Const Tan As String = "ÿc7"
        Public Const CraftedOrange As String = "ÿc8"
        Public Const RareYellow As String = "ÿc9"
        Private b As Byte
    End Structure


End Module