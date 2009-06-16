Imports System
Imports D2Data
Imports ETUtils
Imports D2PacketsVB.D2Packets

Namespace GameClient

    Public Class GCPacket
        Inherits D2Packet

        ' Fields
        Public ReadOnly PacketID As GameClientPacket

        ' Methods
        Public Sub New(ByVal Id As GameClientPacket)
            Me.PacketID = Id
        End Sub

        Public Sub New(ByVal Data() As Byte)
            Me.PacketID = Data(0)

            Dim packetData(Data.Length - 1) As Byte
            Buffer.BlockCopy(Data, 1, packetData, 0, Data.Length - 1)

            MyBase.InsertByteArray(packetData)
        End Sub

        Public Overrides ReadOnly Property Data() As Byte()
            Get
                Dim BaseData As Byte() = MyBase.GetData
                Dim PacketBuf(BaseData.Length) As Byte

                PacketBuf(0) = CByte(PacketID)
                Buffer.BlockCopy(BaseData, 0, PacketBuf, 1, BaseData.Length)

                Return PacketBuf
            End Get
        End Property

    End Class


    Public MustInherit Class GoToLocation
        Inherits GCPacket

        Protected m_x As UInt16
        Protected m_y As UInt16

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_x = BitConverter.ToUInt16(data, 1)
            Me.m_y = BitConverter.ToUInt16(data, 3)
        End Sub

        Public Sub New(ByVal Id As D2Packets.GameClientPacket)
            MyBase.New(GameClientPacket.IdentifyGambleItem)
        End Sub

        Public ReadOnly Property x() As UInt32
            Get
                Return m_x
            End Get
        End Property

        Public ReadOnly Property y() As UInt32
            Get
                Return m_y
            End Get
        End Property

    End Class


    Public MustInherit Class GoToTarget
        Inherits GCPacket

        Protected m_uid As UInt32
        Protected m_unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            m_unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            m_uid = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal Id As D2Packets.GameClientPacket)
            MyBase.New(GameClientPacket.IdentifyGambleItem)
        End Sub

        Public ReadOnly Property uid() As UInt32
            Get
                Return m_uid
            End Get
        End Property

        Public ReadOnly Property unitType() As UInt32
            Get
                Return m_unitType
            End Get
        End Property


    End Class


    Public Class AddBeltItem
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly x As UInt16
        Public ReadOnly y As UInt16

        Public Sub New(ByVal uid As UInteger, ByVal x As UShort, ByVal y As UShort)
            MyBase.New(GameClientPacket.AddBeltItem)

            InsertUInt32(uid)
            InsertUInt16(x)
            InsertUInt16(y)

            Me.uid = uid
            Me.x = x
            Me.y = y

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
            Me.x = CUShort((data(5) Mod 4))
            Me.y = CUShort((data(5) / 4))
        End Sub

    End Class

    <Flags()> _
    Public Enum BuyFlags As UShort
        FillStack = 32768
        None = 0
    End Enum

    Public Class BuyItem
        Inherits GCPacket

        Public ReadOnly cost As UInt32
        Public ReadOnly dealerUID As UInt32
        Public ReadOnly flags As BuyFlags
        Public ReadOnly itemUID As UInt32
        Public ReadOnly tradeType As TradeType

        Public Sub New(ByVal dealerUID As UInteger, ByVal itemUID As UInteger, ByVal cost As UInteger, ByVal flags As BuyFlags)
            MyBase.New(GameClientPacket.BuyItem)

            InsertUInt32(dealerUID)
            InsertUInt32(itemUID)

            InsertInt16(0)

            InsertUInt16(flags)
            InsertUInt32(cost)

            Me.dealerUID = dealerUID
            Me.itemUID = itemUID
            Me.cost = cost
            Me.tradeType = tradeType.BuyItem
            Me.flags = flags

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.dealerUID = BitConverter.ToUInt32(data, 1)
            Me.itemUID = BitConverter.ToUInt32(data, 5)
            Me.tradeType = DirectCast(BitConverter.ToUInt16(data, 9), TradeType)
            Me.flags = DirectCast(BitConverter.ToUInt16(data, 11), BuyFlags)
            Me.cost = BitConverter.ToUInt32(data, 13)
        End Sub

    End Class

    Public Class CainIdentifyItems
        Inherits GCPacket

        Public ReadOnly uid As UInt32

        Public Sub New(ByVal uid As UInteger)
            MyBase.New(GameClientPacket.CainIdentifyItems)
            InsertUInt32(uid)
            Me.uid = uid
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
        End Sub

    End Class

    Public Class CastLeftSkill
        Inherits GCPacket

        Public ReadOnly x As UInt16
        Public ReadOnly y As UInt16

        Public Sub New(ByVal x As UShort, ByVal y As UShort)
            MyBase.New(GameClientPacket.CastLeftSkill)

            InsertUInt16(x)
            InsertUInt16(y)

            Me.x = x
            Me.y = y
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.x = BitConverter.ToUInt16(data, 1)
            Me.y = BitConverter.ToUInt16(data, 3)
        End Sub

    End Class

    Public Class CastLeftSkillOnTarget
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.CastLeftSkillOnTarget)

            InsertInt32(unitType)
            InsertUInt32(uid)

            Me.unitType = unitType
            Me.uid = uid

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub

    End Class

    Public Class CastLeftSkillOnTargetStopped
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.CastLeftSkillOnTargetStopped)
            InsertInt32(unitType)
            InsertUInt32(uid)

            Me.unitType = unitType
            Me.uid = uid

        End Sub

    End Class

    Public Class CastRightSkill
        Inherits GCPacket

        Public ReadOnly x As UInt16
        Public ReadOnly y As UInt16

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.x = BitConverter.ToUInt16(data, 1)
            Me.y = BitConverter.ToUInt16(data, 3)
        End Sub


        Public Sub New(ByVal x As UShort, ByVal y As UShort)
            MyBase.New(GameClientPacket.CastRightSkill)
            InsertUInt16(x)
            InsertUInt16(y)
            Me.x = x
            Me.y = y

        End Sub

    End Class

    Public Class CastRightSkillOnTarget
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.CastRightSkillOnTarget)

            InsertInt32(unitType)
            InsertUInt32(uid)

            Me.unitType = unitType
            Me.uid = uid

        End Sub

    End Class

    Public Class CastRightSkillOnTargetStopped
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.CastRightSkillOnTargetStopped)
            InsertInt32(unitType)
            InsertUInt32(uid)

            Me.unitType = unitType
            Me.uid = uid

        End Sub

    End Class

    Public Class ChangeMercEquipment
        Inherits GCPacket

        Public ReadOnly location As EquipmentLocation
        Public ReadOnly unequip As Boolean

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.location = BitConverter.ToUInt16(data, 1)
            If (Me.location <> EquipmentLocation.NotApplicable) Then
                Me.unequip = True
            End If
        End Sub

        Public Sub New(ByVal location As EquipmentLocation)
            MyBase.New(GameClientPacket.ChangeMercEquipment)
            InsertUInt16(location)

            Me.location = location
            If (location <> EquipmentLocation.NotApplicable) Then
                Me.unequip = True
            End If
        End Sub

    End Class

    Public Class ClickButton
        Inherits GCPacket

        Public ReadOnly button As GameButton
        Public ReadOnly complement As UInt16

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.button = BitConverter.ToUInt32(data, 1)
            Me.complement = BitConverter.ToUInt16(data, 5)
        End Sub

        Public Sub New(ByVal button As GameButton, ByVal complement As UShort)
            MyBase.New(GameClientPacket.ClickButton)

            InsertUInt32(button)
            InsertUInt16(complement)

            Me.button = button
            Me.complement = complement

        End Sub

    End Class

    Public Class CloseQuest
        Inherits GCPacket

        Public ReadOnly quest As QuestType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.quest = BitConverter.ToUInt16(data, 1)
        End Sub

        Public Sub New(ByVal quest As QuestType)
            MyBase.New(GameClientPacket.CloseQuest)
            InsertUInt16(quest)
            Me.quest = quest

        End Sub

    End Class

    Public Class DisplayQuestMessage
        Inherits GCPacket

        Public ReadOnly message As UInt32
        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
            Me.message = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal message As UInteger)
            MyBase.New(GameClientPacket.DisplayQuestMessage)

            InsertUInt32(uid)
            InsertUInt32(message)

            Me.uid = uid
            Me.message = message

        End Sub

    End Class

    Public Class DropGold
        Inherits GCPacket

        Public ReadOnly amount As UInt32
        Public ReadOnly meUID As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.meUID = BitConverter.ToUInt32(data, 1)
            Me.amount = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal amount As UInteger, ByVal meUID As UInteger)
            MyBase.New(GameClientPacket.DropGold)

            InsertUInt32(meUID)
            InsertUInt32(amount)

            Me.amount = amount
            Me.meUID = meUID

        End Sub

    End Class

    Public Class DropItem
        Inherits GCPacket

        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
        End Sub

        Public Sub New(ByVal uid As UInteger)
            MyBase.New(GameClientPacket.DropItem)
            InsertUInt32(uid)
            Me.uid = uid
        End Sub

    End Class

    Public Class DropItemToContainer
        Inherits GCPacket

        Public ReadOnly container As ItemContainerGC
        Public ReadOnly uid As UInt32
        Public ReadOnly x As UInt16
        Public ReadOnly y As UInt16

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
            Me.x = data(5)
            Me.y = data(9)
            Me.container = data(13)
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal container As ItemContainerGC, ByVal x As Byte, ByVal y As Byte)
            MyBase.New(GameClientPacket.DropItemToContainer)

            InsertUInt32(uid)
            InsertInt32(x)
            InsertInt32(y)
            InsertInt32(container)

            Me.uid = uid
            Me.container = container
            Me.x = x
            Me.y = y

        End Sub
    End Class

    Public Class EmbedItem
        Inherits GCPacket

        Public ReadOnly objectUID As UInt32
        Public ReadOnly subjectUID As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.subjectUID = BitConverter.ToUInt32(data, 1)
            Me.objectUID = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal subjectUID As UInteger, ByVal objectUID As UInteger)
            MyBase.New(GameClientPacket.EmbedItem)
            InsertUInt32(subjectUID)
            InsertUInt32(objectUID)

            Me.subjectUID = subjectUID
            Me.objectUID = objectUID

        End Sub

    End Class

    Public Class EnterGame
        Inherits GCPacket

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Sub New()
            MyBase.New(GameClientPacket.EnterGame)
        End Sub

    End Class

    Public Class EquipItem
        Inherits GCPacket

        Public ReadOnly location As EquipmentLocation
        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
            Me.location = data(5)
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal location As EquipmentLocation)
            MyBase.New(GameClientPacket.EquipItem)

            InsertUInt32(uid)
            InsertInt32(location)

            Me.uid = uid
            Me.location = location

        End Sub

    End Class

    Public Class ExitGame
        Inherits GCPacket

        Public Sub New()
            MyBase.New(GameClientPacket.ExitGame)
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

    End Class

    Public Class GameLogonRequest
        Inherits GCPacket

        Public ReadOnly charClass As CharacterClass
        Public ReadOnly d2GShash As UInt32
        Public ReadOnly d2GSToken As UInt16
        Public ReadOnly name As String
        Public ReadOnly version As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.d2GShash = BitConverter.ToUInt32(data, 1)
            Me.d2GSToken = BitConverter.ToUInt16(data, 5)
            Me.charClass = data(7)
            Me.version = BitConverter.ToUInt32(data, 8)
            Me.name = ByteConverter.GetNullString(data, &H15)
        End Sub

        ' Methods
        Public Sub New(ByVal D2GSHash As UInteger, ByVal D2GsToken As UInteger, ByVal CharName As String, ByVal [Class] As CharacterClass)
            MyBase.New(GameClientPacket.GameLogonRequest)

            InsertUInt32(D2GSHash)
            InsertUInt16(D2GsToken)
            InsertByte([Class])

            InsertUInt32(&HC)

            Dim Unknown() As Byte = New Byte(8) {&H50, &HCC, &H5D, &HED, &HB6, &H19, &HA5, &H91, 0}
            InsertByteArray(Unknown)

            InsertCString(CharName)

            Dim Filler(14 - CharName.Length) As Byte
            InsertByteArray(Filler)

            Me.d2GShash = D2GSHash
            Me.d2GSToken = D2GsToken
            Me.charClass = charClass
            Me.version = version
            Me.name = name
        End Sub

    End Class

    Public Class GoToTownFolk
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType
        Public ReadOnly x As UInt32
        Public ReadOnly y As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
            Me.x = BitConverter.ToUInt32(data, 9)
            Me.y = BitConverter.ToUInt32(data, 13)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger, ByVal x As UInteger, ByVal y As UInteger)
            MyBase.New(GameClientPacket.GoToTownFolk)

            InsertInt32(unitType)
            InsertUInt32(uid)
            InsertUInt32(x)
            InsertUInt32(y)

            Me.unitType = unitType
            Me.uid = uid
            Me.x = x
            Me.y = y

        End Sub

    End Class

    Public Class HireMercenary
        Inherits GCPacket

        Public ReadOnly dealerUID As UInt32
        Public ReadOnly mercID As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.dealerUID = BitConverter.ToUInt32(data, 1)
            Me.mercID = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal dealerUID As UInteger, ByVal mercID As UInteger)
            MyBase.New(GameClientPacket.HireMercenary)

            InsertUInt32(dealerUID)
            InsertUInt32(mercID)

            Me.dealerUID = dealerUID
            Me.mercID = mercID

        End Sub

    End Class

    Public Class HoverUnit
        Inherits GCPacket

        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
        End Sub

        Public Sub New(ByVal uid As UInteger)
            MyBase.New(GameClientPacket.HoverUnit)
            InsertUInt32(uid)
            Me.uid = uid
        End Sub
    End Class

    Public Class IdentifyGambleItem
        Inherits GCPacket

        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
        End Sub

        Public Sub New(ByVal uid As UInteger)
            MyBase.New(GameClientPacket.IdentifyGambleItem)
            InsertUInt32(uid)
            Me.uid = uid

        End Sub

    End Class

    Public Class IdentifyItem
        Inherits GCPacket

        Public ReadOnly itemUID As UInt32
        Public ReadOnly scrollUID As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.itemUID = BitConverter.ToUInt32(data, 1)
            Me.scrollUID = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal itemUID As UInteger, ByVal scrollUID As UInteger)
            MyBase.New(GameClientPacket.IdentifyItem)
            InsertUInt32(itemUID)
            InsertUInt32(scrollUID)

            Me.itemUID = itemUID
            Me.scrollUID = scrollUID

        End Sub

    End Class

    Public Class IncrementAttribute
        Inherits GCPacket

        Public ReadOnly attribute As StatType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.attribute = BitConverter.ToUInt16(data, 1)
        End Sub

        Public Sub New(ByVal attribute As StatType)
            MyBase.New(GameClientPacket.IncrementAttribute)
            InsertUInt16(attribute)
            Me.attribute = attribute
        End Sub

    End Class

    Public Class IncrementSkill
        Inherits GCPacket

        Public ReadOnly skill As SkillType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.skill = DirectCast(BitConverter.ToUInt16(data, 1), SkillType)
        End Sub

        Public Sub New(ByVal skill As SkillType)
            MyBase.New(GameClientPacket.IncrementSkill)
            InsertUInt16(skill)
            Me.skill = skill

        End Sub

    End Class

    Public Class InventoryItemToBelt
        Inherits GCPacket

        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
        End Sub

        Public Sub New(ByVal uid As UInteger)
            MyBase.New(GameClientPacket.InventoryItemToBelt)
            InsertUInt32(uid)
            Me.uid = uid
        End Sub

    End Class

    Public Enum ItemContainerGC
        Cube = 3
        Inventory = 0
        Stash = 4
        Trade = 2
    End Enum

    Public Class ItemToCube
        Inherits GCPacket

        Public ReadOnly cubeUID As UInt32
        Public ReadOnly itemUID As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.itemUID = BitConverter.ToUInt32(data, 1)
            Me.cubeUID = BitConverter.ToUInt32(data, 5)
        End Sub


        Public Sub New(ByVal itemUID As UInteger, ByVal cubeUID As UInteger)
            MyBase.New(GameClientPacket.ItemToCube)
            InsertUInt32(itemUID)
            InsertUInt32(cubeUID)

            Me.itemUID = itemUID
            Me.cubeUID = cubeUID

        End Sub

    End Class

    Public Enum PartyAction
        AcceptInvite = 8
        CancelInvite = 7
        Invite = 6
    End Enum

    Public Class PartyRequest
        Inherits GCPacket

        Public ReadOnly action As PartyAction
        Public ReadOnly playerUID As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.action = data(1)
            Me.playerUID = BitConverter.ToUInt32(data, 2)
        End Sub


        Public Sub New(ByVal action As PartyAction, ByVal playerUID As UInteger)
            MyBase.New(GameClientPacket.PartyRequest)
            InsertByte(action)
            InsertUInt32(playerUID)
            Me.action = action
            Me.playerUID = playerUID

        End Sub

    End Class

    Public Class PickItem
        Inherits GCPacket

        Public ReadOnly requestID As UInt32
        Public ReadOnly toCursor As Boolean
        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.requestID = BitConverter.ToUInt32(data, 1)
            Me.uid = BitConverter.ToUInt32(data, 5)
            If (data(9) = 1) Then
                Me.toCursor = True
            End If
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal toCursor As Boolean, ByVal requestID As UInteger)
            MyBase.New(GameClientPacket.PickItem)

            InsertUInt32(requestID)
            InsertUInt32(uid)
            InsertInt32(toCursor)

            Me.uid = uid
            Me.toCursor = toCursor
            Me.requestID = requestID

        End Sub

    End Class

    Public Class PickItemFromContainer
        Inherits GCPacket

        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
        End Sub

        Public Sub New(ByVal uid As UInteger)
            MyBase.New(GameClientPacket.PickItemFromContainer)
            InsertUInt32(uid)
            Me.uid = uid
        End Sub

    End Class

    Public Class Ping
        Inherits GCPacket

        Public ReadOnly tickCount As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.tickCount = BitConverter.ToUInt32(data, 1)
        End Sub

        Public Sub New(ByVal tickCount As UInteger, ByVal unknown5 As Long)
            MyBase.New(GameClientPacket.Ping)
            InsertUInt32(tickCount)
            InsertUInt64(unknown5)
            Me.tickCount = tickCount
        End Sub

    End Class

    Public Enum PlayerRelationType
        Hostile = 4
        Loot = 1
        Mute = 2
        Squelch = 3
    End Enum

    Public Class RecastLeftSkill
        Inherits GCPacket

        Public ReadOnly x As UInt16
        Public ReadOnly y As UInt16

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.x = BitConverter.ToUInt16(data, 1)
            Me.y = BitConverter.ToUInt16(data, 3)
        End Sub

        Public Sub New(ByVal x As UShort, ByVal y As UShort)
            MyBase.New(GameClientPacket.RecastLeftSkill)
            InsertUInt16(x)
            InsertUInt16(y)

            Me.x = x
            Me.y = y
        End Sub

    End Class

    Public Class RecastLeftSkillOnTarget
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.RecastLeftSkillOnTarget)

            InsertInt32(unitType)
            InsertUInt32(uid)

            Me.unitType = unitType
            Me.uid = uid

        End Sub

    End Class

    Public Class RecastLeftSkillOnTargetStopped
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.RecastLeftSkillOnTargetStopped)

            InsertInt32(unitType)
            InsertUInt32(uid)
            Me.unitType = unitType
            Me.uid = uid

        End Sub

    End Class

    Public Class RecastRightSkill
        Inherits GCPacket

        Public ReadOnly x As UInt16
        Public ReadOnly y As UInt16

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.x = BitConverter.ToUInt16(data, 1)
            Me.y = BitConverter.ToUInt16(data, 3)
        End Sub

        Public Sub New(ByVal x As UShort, ByVal y As UShort)
            MyBase.New(GameClientPacket.RecastRightSkill)
            InsertUInt16(x)
            InsertUInt16(y)
            Me.x = x
            Me.y = y
        End Sub
    End Class

    Public Class RecastRightSkillOnTarget
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub


        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.RecastRightSkillOnTarget)
            InsertInt32(unitType)
            InsertUInt32(uid)
            Me.unitType = unitType
            Me.uid = uid

        End Sub

    End Class

    Public Class RecastRightSkillOnTargetStopped
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.RecastRightSkillOnTargetStopped)

            InsertInt32(unitType)
            InsertUInt32(uid)
            Me.unitType = unitType
            Me.uid = uid

        End Sub

    End Class

    Public Class RemoveBeltItem
        Inherits GCPacket

        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
        End Sub

        Public Sub New(ByVal uid As UInteger)
            MyBase.New(GameClientPacket.RemoveBeltItem)
            InsertUInt32(uid)
            Me.uid = uid
        End Sub

    End Class

    Public Enum RepairType
        RepairAll
        RepairItem
    End Enum

    Public Class RequestQuestLog
        Inherits GCPacket

        Public Sub New()
            MyBase.New(GameClientPacket.RequestQuestLog)
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(GameClientPacket.RequestQuestLog)
        End Sub

    End Class

    Public Class RequestReassign
        Inherits GCPacket

        Public ReadOnly meUID As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(data(1), UnitType)
            Me.meUID = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal meUID As UInteger)
            MyBase.New(GameClientPacket.RequestReassign)
            InsertInt32(unitType)
            InsertUInt32(meUID)
            Me.unitType = unitType
            Me.meUID = meUID

        End Sub
    End Class

    Public Class Respawn
        Inherits GCPacket

        Public Sub New()
            MyBase.New(GameClientPacket.Respawn)
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(GameClientPacket.Respawn)
        End Sub
    End Class

    Public Class ResurrectMerc
        Inherits GCPacket

        Public ReadOnly dealerUID As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.dealerUID = BitConverter.ToUInt32(data, 1)
        End Sub

        Public Sub New(ByVal dealerUID As UInteger)
            MyBase.New(GameClientPacket.ResurrectMerc)
            InsertUInt32(dealerUID)
            Me.dealerUID = dealerUID

        End Sub

    End Class

    Public Class RunToLocation
        Inherits GCPacket

        Public ReadOnly x As UInt16
        Public ReadOnly y As UInt16

        Public Sub New(ByVal x As UShort, ByVal y As UShort)
            MyBase.New(GameClientPacket.RunToLocation)
            InsertUInt16(x)
            InsertUInt16(y)
            Me.x = x
            Me.y = y
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            x = BitConverter.ToUInt16(data, 1)
            y = BitConverter.ToUInt16(data, 3)
        End Sub

    End Class

    Public Class RunToTarget
        Inherits GoToTarget

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Sub New(ByVal unittype As UnitType, ByVal uid As UInteger)

            MyBase.New(D2Packets.GameClientPacket.RunToTarget)

            InsertInt32(unittype)
            InsertUInt32(uid)

        End Sub

    End Class

    Public Class SelectSkill
        Inherits GCPacket

        Public ReadOnly chargedItemUID As UInt32
        Public ReadOnly hand As SkillHand
        Public Shared ReadOnly NULL_UInt32 As UInt32
        Public ReadOnly skill As SkillType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.skill = DirectCast(BitConverter.ToUInt16(data, 1), SkillType)
            If (data(4) = &H80) Then
                Me.hand = SkillHand.Left
            End If
            Me.chargedItemUID = BitConverter.ToUInt32(data, 5)
            If (Me.chargedItemUID = UInt32.MaxValue) Then
                Me.chargedItemUID = 0
            End If
        End Sub

        Public Sub New(ByVal skill As SkillType, ByVal hand As SkillHand, Optional ByVal chargedItemUID As UInteger = UInteger.MaxValue)
            MyBase.New(GameClientPacket.SelectSkill)

            InsertUInt16(skill)
            InsertByte(0)
            InsertByte(IIf((hand = SkillHand.Left), CByte(128), CByte(0)))
            InsertUInt32(chargedItemUID)

            Me.skill = skill
            Me.hand = hand
            Me.chargedItemUID = chargedItemUID

        End Sub

    End Class


    ''' <summary>
    ''' To Check
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SellItem
        Inherits GCPacket

        Public ReadOnly cost As UInt32
        Public ReadOnly dealerUID As UInt32
        Public ReadOnly itemUID As UInt32
        Public ReadOnly tradeType As TradeType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.dealerUID = BitConverter.ToUInt32(data, 1)
            Me.itemUID = BitConverter.ToUInt32(data, 5)

            Me.tradeType = DirectCast(CUShort(BitConverter.ToUInt32(data, 9)), TradeType)

            Me.cost = BitConverter.ToUInt32(data, 13)
        End Sub

        Public Sub New(ByVal dealerUID As UInteger, ByVal itemUID As UInteger, ByVal cost As UInteger)
            MyBase.New(GameClientPacket.SellItem)

            InsertUInt32(dealerUID)
            InsertUInt32(itemUID)
            InsertInt32(4)
            InsertUInt32(cost)

        End Sub

    End Class

    Public Class SendCharacterSpeech
        Inherits GCPacket

        Public ReadOnly speech As GameSound

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.speech = BitConverter.ToUInt16(data, 1)
        End Sub

        Public Sub New(ByVal speech As GameSound)
            MyBase.New(GameClientPacket.SendCharacterSpeech)

            InsertUInt16(speech)
            Me.speech = speech
        End Sub

    End Class

    Public Class SendMessage
        Inherits GCPacket

        Public ReadOnly message As String
        Public ReadOnly recipient As String
        Public ReadOnly type As GameMessageType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.type = DirectCast(BitConverter.ToUInt16(data, 1), GameMessageType)
            Me.message = ByteConverter.GetNullString(data, 3)
            If (Me.type = GameMessageType.GameWhisper) Then
                Me.recipient = ByteConverter.GetNullString(data, (4 + Me.message.Length))
            End If
        End Sub

        Public Sub New(ByVal type As GameMessageType, ByVal message As String, Optional ByVal recipient As String = Nothing)
            MyBase.New(GameClientPacket.SendMessage)

            If (message Is Nothing) OrElse (message.Length = 0) Then
                Throw New ArgumentException()
            End If

            InsertUInt16(type)

            For i = 0 To message.Length - 1
                InsertByte(System.Convert.ToByte(message(i)))
            Next
            InsertByte(0)

            If recipient IsNot Nothing Then
                InsertCString(recipient)
            Else
                InsertInt16(0)
            End If

            Me.type = type
            Me.message = message
            Me.recipient = recipient

        End Sub

    End Class

    Public Class SendOverheadMessage
        Inherits GCPacket

        Public ReadOnly message As String

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.message = ByteConverter.GetNullString(data, 3)
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(GameClientPacket.SendOverheadMessage)

            If (message Is Nothing) OrElse (message.Length = 0) Then
                Throw New ArgumentException("Invalid Message")
            End If

            InsertInt16(0)
            InsertCString(message)
            InsertInt16(0)

            Me.message = message

        End Sub

    End Class

    Public Class SetPlayerRelation
        Inherits GCPacket

        Public ReadOnly relation As PlayerRelationType
        Public ReadOnly uid As UInt32
        Public ReadOnly value As Boolean

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.relation = data(1)
            Me.value = BitConverter.ToBoolean(data, 2)
            Me.uid = BitConverter.ToUInt32(data, 3)
        End Sub


        Public Sub New(ByVal uid As UInteger, ByVal relation As PlayerRelationType, ByVal value As Boolean)
            MyBase.New(GameClientPacket.SetPlayerRelation)

            InsertByte(relation)
            InsertBoolean(value)
            InsertUInt32(uid)

            Me.uid = uid
            Me.relation = relation
            Me.value = value

        End Sub

    End Class

    Public Class SetSkillHotkey
        Inherits GCPacket
        ' Fields
        Public ReadOnly chargedItemUID As UInteger

        Public ReadOnly skill As SkillType
        Public ReadOnly slot As UShort

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.skill = DirectCast(BitConverter.ToUInt16(data, 1), SkillType)
            Me.slot = BitConverter.ToUInt16(data, 3)
            Me.chargedItemUID = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal slot As UShort, ByVal skill As SkillType, Optional ByVal itemUID As UInteger = UInteger.MaxValue)
            MyBase.New(GameClientPacket.SetSkillHotkey)

            InsertUInt16(skill)
            InsertUInt16(slot)
            InsertUInt32(itemUID)

            Me.slot = slot
            Me.skill = skill
            Me.chargedItemUID = itemUID

        End Sub
    End Class


    Public Class StackItems
        Inherits GCPacket

        Public ReadOnly objectUID As UInt32
        Public ReadOnly subjectUID As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.subjectUID = BitConverter.ToUInt32(data, 1)
            Me.objectUID = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal subjectUID As UInteger, ByVal objectUID As UInteger)
            MyBase.New(GameClientPacket.StackItems)

            InsertUInt32(subjectUID)
            InsertUInt32(objectUID)

            Me.subjectUID = subjectUID
            Me.objectUID = objectUID

        End Sub

    End Class

    Public Class SwapBeltItem
        Inherits GCPacket

        Public ReadOnly newItemUID As UInt32
        Public ReadOnly oldItemUID As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.oldItemUID = BitConverter.ToUInt32(data, 1)
            Me.newItemUID = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal oldItemUID As UInteger, ByVal newItemUID As UInteger)
            MyBase.New(GameClientPacket.SwapBeltItem)
            InsertUInt32(oldItemUID)
            InsertUInt32(newItemUID)

            Me.oldItemUID = oldItemUID
            Me.newItemUID = newItemUID

        End Sub

    End Class

    Public Class SwapContainerItem
        Inherits GCPacket

        Public ReadOnly objectUID As UInt32
        Public ReadOnly subjectUID As UInt32
        Public ReadOnly x As Integer
        Public ReadOnly y As Integer

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.subjectUID = BitConverter.ToUInt32(data, 1)
            Me.objectUID = BitConverter.ToUInt32(data, 5)
            Me.x = BitConverter.ToInt32(data, 9)
            Me.y = BitConverter.ToInt32(data, 13)
        End Sub

        Public Sub New(ByVal subjectUID As UInteger, ByVal objectUID As UInteger, ByVal x As Integer, ByVal y As Integer)
            MyBase.New(GameClientPacket.SwapContainerItem)

            InsertUInt32(subjectUID)
            InsertUInt32(objectUID)
            InsertUInt32(x)
            InsertUInt32(y)

            Me.subjectUID = subjectUID
            Me.objectUID = objectUID
            Me.x = x
            Me.y = y

        End Sub

    End Class

    Public Class SwapEquippedItem
        Inherits GCPacket

        Public ReadOnly location As EquipmentLocation
        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
            Me.location = data(5)
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal location As EquipmentLocation)
            MyBase.New(GameClientPacket.SwapEquippedItem)

            InsertUInt32(uid)
            InsertInt32(location)

            Me.uid = uid
            Me.location = location

        End Sub

    End Class

    Public Class SwitchWeapons
        Inherits GCPacket

        Public Sub New()
            MyBase.New(GameClientPacket.SwitchWeapons)
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

    End Class


    Public Class TownFolkCancelInteraction
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(data(1), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.TownFolkCancelInteraction)

            InsertInt32(unitType)
            InsertUInt32(uid)

            Me.unitType = unitType
            Me.uid = uid

        End Sub

    End Class

    Public Class TownFolkInteract
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(data(1), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.TownFolkInteract)

            InsertInt32(unitType)
            InsertUInt32(uid)

            Me.unitType = unitType
            Me.uid = uid

        End Sub
    End Class

    Public Class TownFolkMenuSelect
        Inherits GCPacket

        Public ReadOnly selection As TownFolkMenuItem
        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.selection = BitConverter.ToUInt32(data, 1)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub


        Public Sub New(ByVal selection As TownFolkMenuItem, ByVal uid As UInteger, ByVal unknown9 As UInteger)
            MyBase.New(GameClientPacket.TownFolkMenuSelect)

            InsertInt32(selection)
            InsertUInt32(uid)
            InsertUInt32(unknown9)

            Me.selection = selection
            Me.uid = uid
        End Sub

    End Class

    ''' <summary>
    ''' To Check
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TownFolkRepair
        Inherits GCPacket

        Public ReadOnly dealerUID As UInt32
        Public ReadOnly itemUID As UInt32
        Public Shared ReadOnly NULL_UInt32 As UInt32
        Public ReadOnly repairType As RepairType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.dealerUID = BitConverter.ToUInt32(data, 1)
            Me.itemUID = BitConverter.ToUInt32(data, 5)
            Me.repairType = BitConverter.ToUInt32(data, 9)
        End Sub


        Public Sub New(ByVal dealerUID As UInteger)
            MyBase.New(GameClientPacket.TownFolkRepair)

            InsertUInt32(dealerUID) '0x04

            Dim filler(11) As Byte
            InsertByteArray(filler)

            InsertByte(&H80) '0x16

            Me.dealerUID = dealerUID
            Me.itemUID = 0
            Me.repairType = repairType.RepairAll

        End Sub

    End Class

    Public Enum TradeType As UShort
        BuyItem = 0
        GambleItem = 2
        SellItem = 4
    End Enum

    Public Class UnequipItem
        Inherits GCPacket

        Public ReadOnly location As EquipmentLocation

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.location = data(1)
        End Sub

        Public Sub New(ByVal location As EquipmentLocation)
            MyBase.New(GameClientPacket.UnequipItem)
            InsertInt16(location)
            Me.location = location
        End Sub

    End Class

    Public Class UnitInteract
        Inherits GCPacket

        Public ReadOnly uid As UInt32
        Public ReadOnly unitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.unitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.UnitInteract)

            InsertInt32(unitType)
            InsertUInt32(uid)
            Me.unitType = unitType
            Me.uid = uid

        End Sub
    End Class

    Public Class UpdatePosition
        Inherits GCPacket

        Public ReadOnly x As UInt16
        Public ReadOnly y As UInt16

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.x = BitConverter.ToUInt16(data, 1)
            Me.y = BitConverter.ToUInt16(data, 3)
        End Sub

        Public Sub New(ByVal x As UShort, ByVal y As UShort)
            MyBase.New(GameClientPacket.UpdatePosition)

            InsertUInt16(x)
            InsertUInt16(y)
            Me.x = x
            Me.y = y

        End Sub

    End Class

    Public Class UseBeltItem
        Inherits GCPacket

        Public ReadOnly toMerc As Boolean
        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
            If (BitConverter.ToUInt32(data, 5) = 1) Then
                Me.toMerc = True
            End If
        End Sub

        Public Sub New(ByVal Itemuid As UInteger, ByVal toMerc As Boolean, Optional ByVal unknown9 As UInteger = 0)
            MyBase.New(GameClientPacket.UseBeltItem)

            InsertUInt32(Itemuid)
            InsertInt32(IIf(toMerc, CByte(1), CByte(0)))
            InsertUInt32(unknown9)

            Me.uid = uid
            Me.toMerc = toMerc

        End Sub

    End Class

    Public Class UseInventoryItem
        Inherits GCPacket

        Public ReadOnly itemUID As UInt32
        Public ReadOnly meX As Integer
        Public ReadOnly meY As Integer

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.itemUID = BitConverter.ToUInt32(data, 1)
            Me.meX = BitConverter.ToInt32(data, 5)
            Me.meY = BitConverter.ToInt32(data, 9)
        End Sub

        Public Sub New(ByVal itemUID As UInteger, ByVal meX As Integer, ByVal meY As Integer)
            MyBase.New(GameClientPacket.UseInventoryItem)

            InsertUInt32(itemUID)
            InsertUInt32(meX)
            InsertUInt32(meY)

            Me.itemUID = itemUID
            Me.meX = meX
            Me.meY = meY

        End Sub

    End Class

    Public Class WalkToLocation
        Inherits GCPacket

        Public ReadOnly x As UInt16
        Public ReadOnly y As UInt16

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.x = BitConverter.ToUInt16(data, 1)
            Me.y = BitConverter.ToUInt16(data, 3)
        End Sub


        Public Sub New(ByVal x As UShort, ByVal y As UShort)
            MyBase.New(GameClientPacket.WalkToLocation)
            InsertUInt16(x)
            InsertUInt16(y)
            Me.x = x
            Me.y = y
        End Sub

    End Class

    Public Class WalkToTarget
        Inherits GCPacket

        Public ReadOnly uid As UInt16
        Public ReadOnly UnitType As UnitType

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.UnitType = DirectCast(CByte(BitConverter.ToUInt32(data, 1)), UnitType)
            Me.uid = BitConverter.ToUInt32(data, 5)
        End Sub


        Public Sub New(ByVal unittype As UnitType, ByVal uid As UInteger)
            MyBase.New(GameClientPacket.WalkToTarget)
            InsertInt32(unittype)
            InsertUInt32(uid)

            Me.UnitType = unittype
            Me.uid = uid
        End Sub

    End Class

    Public Class WardenResponse
        Inherits GCPacket

        Public ReadOnly dataLength As Integer

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.dataLength = BitConverter.ToUInt16(data, 1)
        End Sub

        Public ReadOnly Property WardenData() As Byte()
            Get
                Dim destinationArray As Byte() = New Byte(Me.dataLength - 1) {}
                Array.Copy(MyBase.GetData, 3, destinationArray, 0, Me.dataLength)
                Return destinationArray
            End Get
        End Property

    End Class

    Public Class WaypointInteract
        Inherits GCPacket

        Public ReadOnly destination As WaypointDestination
        Public ReadOnly uid As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
            Me.destination = DirectCast(data(5), WaypointDestination)
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal destination As WaypointDestination)
            MyBase.New(GameClientPacket.WaypointInteract)

            InsertUInt32(uid)
            InsertInt32(destination)

            Me.uid = uid
            Me.destination = destination

        End Sub

    End Class

End Namespace
