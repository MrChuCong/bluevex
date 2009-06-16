Imports System
Imports System.Text
Imports D2Data
Imports D2PacketsVB.D2Packets
Imports ETUtils
Imports System.Collections.Generic

Namespace GameServer

    Public Class GSPacket

        Inherits D2Packet

        Public ReadOnly PacketID As GameServerPacket

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


    Public Class AboutPlayer
        Inherits GSPacket
        ' Fields
        Protected m_isInMyParty As Boolean
        Protected m_level As Integer
        Protected m_partyID As Short
        Protected m_relationship As PlayerRelationshipType
        Protected m_uid As UInteger
        Protected m_unknown12 As Byte

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_partyID = BitConverter.ToInt16(data, 5)
            Me.m_level = BitConverter.ToUInt16(data, 7)

            Me.m_relationship = BitConverter.ToUInt16(data, 9)

            Me.m_isInMyParty = BitConverter.ToBoolean(data, 11)
            Me.m_unknown12 = data(12)
        End Sub

        ' Properties
        Public ReadOnly Property IsInMyParty() As Boolean
            Get
                Return Me.m_isInMyParty
            End Get
        End Property

        Public ReadOnly Property Level() As Integer
            Get
                Return Me.m_level
            End Get
        End Property

        Public ReadOnly Property PartyID() As Short
            Get
                Return Me.m_partyID
            End Get
        End Property

        Public ReadOnly Property Relationship() As PlayerRelationshipType
            Get
                Return Me.m_relationship
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property Unknown12() As Byte
            Get
                Return Me.m_unknown12
            End Get
        End Property
    End Class

    Public Class AcceptTrade
        Inherits GSPacket
        ' Fields
        Protected m_playerName As String
        Protected m_playerUID As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_playerName = ByteConverter.GetNullString(data, 1, 16)
            Me.m_playerUID = BitConverter.ToUInt32(data, 17)
        End Sub

        Public Sub New(ByVal name As String, ByVal uid As UInteger)
            MyBase.New(Build(name, uid))
            Me.m_playerName = name
            Me.m_playerUID = uid
        End Sub

        Public Shared Function Build(ByVal name As String, ByVal uid As UInteger) As Byte()
            If ((name Is Nothing) OrElse (name.Length = 0)) OrElse (name.Length > 16) Then
                Throw New ArgumentException("name")
            End If
            Dim buffer As Byte() = New Byte(20) {}
            buffer(0) = 120
            buffer(17) = CByte(uid)
            buffer(18) = CByte((uid >> 8))
            buffer(19) = CByte((uid >> 16))
            buffer(20) = CByte((uid >> 24))
            For i As Integer = 0 To name.Length - 1
                buffer(1 + i) = AscW(name(i))
            Next
            Return buffer
        End Function

        ' Properties
        Public ReadOnly Property PlayerName() As String
            Get
                Return Me.m_playerName
            End Get
        End Property

        Public ReadOnly Property PlayerUID() As UInteger
            Get
                Return Me.m_playerUID
            End Get
        End Property
    End Class

    Public Class AddUnit
        Inherits GSPacket
        ' Fields
        Protected offset As Long
        Protected m_states As List(Of NPCState)
        Protected m_uid As UInteger
        Protected m_unitType As UnitType
        Protected m_unknownEnd As Byte()

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Dim num As Integer
            Dim num2 As Integer
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_states = New List(Of NPCState)()
            Dim reader As New BitReader(data, 7)
Label_0030:
            num = reader.ReadInt32(8)
            If num = 255 Then
                Me.offset = reader.Position
                Me.m_unknownEnd = reader.ReadByteArray()
                Return
            End If
            If Not reader.ReadBoolean(1) Then
                Me.m_states.Add(New NPCState(BaseState.[Get](num)))
                GoTo Label_0030
            End If
            Dim stats As New List(Of StatBase)()
Label_0056:
            num2 = reader.ReadInt32(9)
            If num2 <> 511 Then
                Dim stat As BaseStat = BaseStat.[Get](num2)
                Dim val As Integer = reader.ReadInt32(stat.SendBits)
                If stat.SendParamBits > 0 Then
                    Dim param As Integer = reader.ReadInt32(stat.SendParamBits)
                    If stat.Signed Then
                        stats.Add(New SignedStatParam(stat, val, param))
                    Else
                        stats.Add(New UnsignedStatParam(stat, CInt(val), CInt(param)))
                    End If
                ElseIf stat.Signed Then
                    stats.Add(New SignedStat(stat, val))
                Else
                    stats.Add(New UnsignedStat(stat, CInt(val)))
                End If
                GoTo Label_0056
            End If
            Me.m_states.Add(New NPCState(BaseState.[Get](num), stats))
            GoTo Label_0030
        End Sub

        ' Properties
        Public ReadOnly Property States() As List(Of NPCState)
            Get
                Return Me.m_states
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property UnknownEnd() As Byte()
            Get
                Return Me.m_unknownEnd
            End Get
        End Property
    End Class

    Public Class AssignGameObject
        Inherits GSPacket
        ' Fields
        Protected m_destination As AreaLevel
        Protected m_interactType As GameObjectInteractType
        Protected m_objectID As GameObjectID
        Protected m_objectMode As GameObjectMode
        Protected m_uid As UInteger
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_objectID = BitConverter.ToUInt16(data, 6)
            Me.m_x = BitConverter.ToUInt16(data, 8)
            Me.m_y = BitConverter.ToUInt16(data, 10)
            Me.m_objectMode = data(12)
            If Me.m_objectID = GameObjectID.TownPortal Then
                Me.m_interactType = GameObjectInteractType.TownPortal
                Me.m_destination = data(13)
            Else
                Me.m_interactType = data(13)
                Me.m_destination = AreaLevel.None
            End If
        End Sub

        ' Properties
        Public ReadOnly Property Destination() As AreaLevel
            Get
                Return Me.m_destination
            End Get
        End Property

        Public ReadOnly Property InteractType() As GameObjectInteractType
            Get
                Return Me.m_interactType
            End Get
        End Property

        Public ReadOnly Property ObjectID() As GameObjectID
            Get
                Return Me.m_objectID
            End Get
        End Property

        Public ReadOnly Property ObjectMode() As GameObjectMode
            Get
                Return Me.m_objectMode
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Class AssignMerc
        Inherits GSPacket
        ' Fields
        Protected m_id As NPCCode
        Protected m_ownerUID As UInteger
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_id = BitConverter.ToUInt16(data, 2)
            Me.m_ownerUID = BitConverter.ToUInt32(data, 4)
            Me.m_uid = BitConverter.ToUInt32(data, 8)
        End Sub

        ' Properties
        Public ReadOnly Property ID() As NPCCode
            Get
                Return Me.m_id
            End Get
        End Property

        Public ReadOnly Property OwnerUID() As UInteger
            Get
                Return Me.m_ownerUID
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property Unknown1() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 1, 1)
            End Get
        End Property

        Public ReadOnly Property Unknown5() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 12, 8)
            End Get
        End Property
    End Class

    Public Class AssignNPC
        Inherits GSPacket
        ' Fields
        Public ReadOnly id As NPCCode
        Public ReadOnly life As Byte
        Public ReadOnly uid As UInteger
        Public ReadOnly x As Integer
        Public ReadOnly y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.uid = BitConverter.ToUInt32(data, 1)
            Me.id = BitConverter.ToUInt16(data, 5)
            Me.x = BitConverter.ToUInt16(data, 7)
            Me.y = BitConverter.ToUInt16(data, 9)
            Me.life = data(11)
        End Sub

        Public ReadOnly Property Unknown13() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 13)
            End Get
        End Property

    End Class

    Public Class AssignPlayer
        Inherits GSPacket
        ' Fields
        Protected charClass As CharacterClass
        Protected m_name As String
        Protected m_uid As UInteger
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.charClass = data(5)
            Me.m_name = ByteConverter.GetNullString(data, 6, 16)
            Me.m_x = BitConverter.ToUInt16(data, 22)
            Me.m_y = BitConverter.ToUInt16(data, 24)
        End Sub

        ' Properties
        Public ReadOnly Property [Class]() As CharacterClass
            Get
                Return Me.charClass
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return Me.m_name
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Class AssignPlayerCorpse
        Inherits GSPacket
        ' Fields
        Protected m_assign As Boolean
        Protected m_corpseUID As UInteger
        Protected m_playerUID As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_assign = Convert.ToBoolean(data(1))
            Me.m_playerUID = BitConverter.ToUInt32(data, 2)
            Me.m_corpseUID = BitConverter.ToUInt32(data, 6)
        End Sub

        Public Sub New(ByVal assign As Boolean, ByVal playerUID As UInteger, ByVal corpseUID As UInteger)
            MyBase.New(Build(assign, playerUID, corpseUID))
            Me.m_assign = assign
            Me.m_playerUID = playerUID
            Me.m_corpseUID = corpseUID
        End Sub

        Public Shared Function Build(ByVal assign As Boolean, ByVal playerUID As UInteger, ByVal corpseUID As UInteger) As Byte()
            Return New Byte() {142, (IIf(assign, CByte(1), CByte(0))), CByte(playerUID), CByte((playerUID >> 8)), CByte((playerUID >> 16)), CByte((playerUID >> 24)), _
             CByte(corpseUID), CByte((corpseUID >> 8)), CByte((corpseUID >> 16)), CByte((corpseUID >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property Assign() As Boolean
            Get
                Return Me.m_assign
            End Get
        End Property

        Public ReadOnly Property CorpseUID() As UInteger
            Get
                Return Me.m_corpseUID
            End Get
        End Property

        Public ReadOnly Property PlayerUID() As UInteger
            Get
                Return Me.m_playerUID
            End Get
        End Property
    End Class

    Public Class AssignPlayerToParty
        Inherits GSPacket
        ' Fields
        Protected m_partyNumber As Short
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_partyNumber = BitConverter.ToInt16(data, 5)
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal partyNumber As Short)
            MyBase.New(Build(uid, partyNumber))
            Me.m_uid = uid
            Me.m_partyNumber = partyNumber
        End Sub

        Public Shared Function Build(ByVal uid As UInteger, ByVal partyNumber As Short) As Byte()
            Return New Byte() {141, CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24)), CByte(partyNumber), _
             CByte((partyNumber >> 8))}
        End Function

        ' Properties
        Public ReadOnly Property PartyNumber() As Short
            Get
                Return Me.m_partyNumber
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class

    Public Class AssignSkill
        Inherits GSPacket
        ' Fields
        Protected m_chargedItemUID As UInteger
        Protected m_hand As SkillHand
        Public Shared ReadOnly NULL_UInt32 As UInteger = UInteger.MaxValue
        Protected m_skill As SkillType
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_hand = data(6)
            Me.m_skill = DirectCast(BitConverter.ToUInt16(data, 7), SkillType)
            Me.m_chargedItemUID = BitConverter.ToUInt32(data, 9)
        End Sub

        ' Properties
        Public ReadOnly Property ChargedItemUID() As UInteger
            Get
                Return Me.m_chargedItemUID
            End Get
        End Property

        Public ReadOnly Property Hand() As SkillHand
            Get
                Return Me.m_hand
            End Get
        End Property

        Public ReadOnly Property Skill() As SkillType
            Get
                Return Me.m_skill
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class

    Public Class AssignSkillHotkey
        Inherits GSPacket
        ' Fields
        Protected m_chargedItemUID As UInteger
        Public Shared ReadOnly NULL_UInt32 As UInteger = UInteger.MaxValue
        Protected m_skill As SkillType
        Protected m_slot As Byte

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_slot = data(1)
            Me.m_skill = DirectCast(BitConverter.ToUInt16(data, 2), SkillType)
            Me.m_chargedItemUID = BitConverter.ToUInt32(data, 4)
        End Sub

        Public Sub New(ByVal slot As Byte, ByVal skill As SkillType)
            Me.New(slot, skill, UInteger.MaxValue)
        End Sub

        Public Sub New(ByVal slot As Byte, ByVal skill As SkillType, ByVal itemUID As UInteger)
            MyBase.New(Build(slot, skill, itemUID))
            Me.m_slot = slot
            Me.m_skill = skill
            Me.m_chargedItemUID = itemUID
        End Sub

        Public Shared Function Build(ByVal slot As Byte, ByVal skill As SkillType) As Byte()
            Return Build(slot, skill, UInteger.MaxValue)
        End Function

        Public Shared Function Build(ByVal slot As Byte, ByVal skill As SkillType, ByVal itemUID As UInteger) As Byte()
            Return New Byte() {123, slot, CByte(skill), CByte((CUShort(skill) >> 8)), CByte(itemUID), CByte((itemUID >> 8)), _
             CByte((itemUID >> 16)), CByte((itemUID >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property ChargedItemUID() As UInteger
            Get
                Return Me.m_chargedItemUID
            End Get
        End Property

        Public ReadOnly Property Skill() As SkillType
            Get
                Return Me.m_skill
            End Get
        End Property

        Public ReadOnly Property Slot() As Byte
            Get
                Return Me.m_slot
            End Get
        End Property
    End Class

    Public Class AssignWarp
        Inherits GSPacket
        ' Fields
        Protected m_id As WarpType
        Protected m_uid As UInteger
        Protected m_unitType As UnitType
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_id = data(6)
            Me.m_x = BitConverter.ToUInt16(data, 7)
            Me.m_y = BitConverter.ToUInt16(data, 9)
        End Sub

        ' Properties
        Public ReadOnly Property ID() As WarpType
            Get
                Return Me.m_id
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Class AttributeByte
        Inherits AttributeNotification
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Dim stat As BaseStat = BaseStat.[Get](data(1))
            If stat.Signed Then
                MyBase.m_stat = New SignedStat(stat, data(2))
            Else
                MyBase.m_stat = New UnsignedStat(stat, data(2))
            End If
        End Sub
    End Class

    Public Class AttributeDWord
        Inherits AttributeNotification
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Dim stat As BaseStat = BaseStat.[Get](data(1))
            Dim val As Integer = BitConverter.ToInt32(data, 2)
            If stat.ValShift > 0 Then
                val = val >> stat.ValShift
            End If
            If stat.Signed Then
                MyBase.m_stat = New SignedStat(stat, val)
            Else
                MyBase.m_stat = New UnsignedStat(stat, CInt(val))
            End If
        End Sub
    End Class

    Public Class AttributeNotification
        Inherits GSPacket
        ' Fields
        Protected m_stat As StatBase

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        ' Properties
        Public ReadOnly Property Stat() As StatBase
            Get
                Return Me.m_stat
            End Get
        End Property
    End Class

    Public Class AttributeWord
        Inherits AttributeNotification
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Dim stat As BaseStat = BaseStat.[Get](data(1))
            Dim val As Integer = IIf((data(2) = 0), data(3), BitConverter.ToUInt16(data, 2))
            If stat.Signed Then
                MyBase.m_stat = New SignedStat(stat, val)
            Else
                MyBase.m_stat = New UnsignedStat(stat, CInt(val))
            End If
        End Sub
    End Class

    Public Class ByteToExperience
        Inherits GainExperience
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            MyBase.m_experience = data(1)
        End Sub
    End Class

    Public Class DelayedState
        Inherits GSPacket
        ' Fields
        Protected m_state As BaseState
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_state = BaseState.[Get](data(6))
        End Sub

        ' Properties
        Public ReadOnly Property State() As BaseState
            Get
                Return Me.m_state
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class

    Public Class DWordToExperience
        Inherits GainExperience
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            MyBase.m_experience = BitConverter.ToUInt32(data, 1)
        End Sub
    End Class

    Public Class EndState
        Inherits GSPacket
        ' Fields
        Protected m_state As BaseState
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_state = BaseState.[Get](data(6))
        End Sub

        ' Properties
        Public ReadOnly Property State() As BaseState
            Get
                Return Me.m_state
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class

    Public Class GainExperience
        Inherits GSPacket
        ' Fields
        Protected m_experience As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        ' Properties
        Public ReadOnly Property Experience() As UInteger
            Get
                Return Me.m_experience
            End Get
        End Property
    End Class

    Public Class GameHandshake
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
        End Sub

        Public Sub New(ByVal type As UnitType, ByVal uid As UInteger)
            MyBase.New(Build(type, uid))
            Me.m_unitType = type
            Me.m_uid = uid
        End Sub

        Public Shared Function Build(ByVal type As UnitType, ByVal uid As UInteger) As Byte()
            Return New Byte() {11, CByte(type), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class

    Public Class GameLoading
        Inherits GSPacket
        ' Methods
        Public Sub New()
            MyBase.New(Build())
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Shared Function Build() As Byte()
            Return New Byte(0) {}
        End Function
    End Class

    Public Class GameLogonReceipt
        Inherits GSPacket
        ' Fields
        Protected m_difficulty As GameDifficulty
        Protected m_expansion As Boolean
        Protected m_hardcore As Boolean
        Protected m_ladder As Boolean
        Protected m_unknown2 As Byte

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_difficulty = data(1)
            Me.m_unknown2 = data(2)
            Me.m_hardcore = (data(3) And 8) = 8
            Me.m_expansion = data(6) = 1
            Me.m_ladder = data(7) = 1
        End Sub

        ' Properties
        Public ReadOnly Property Difficulty() As GameDifficulty
            Get
                Return Me.m_difficulty
            End Get
        End Property

        Public ReadOnly Property Expansion() As Boolean
            Get
                Return Me.m_expansion
            End Get
        End Property

        Public ReadOnly Property Hardcore() As Boolean
            Get
                Return Me.m_hardcore
            End Get
        End Property

        Public ReadOnly Property Ladder() As Boolean
            Get
                Return Me.m_ladder
            End Get
        End Property

        Public ReadOnly Property Unknown2() As Byte
            Get
                Return Me.m_unknown2
            End Get
        End Property
    End Class

    Public Class GameLogonSuccess
        Inherits GSPacket
        ' Methods
        Public Sub New()
            MyBase.New(Build())
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Shared Function Build() As Byte()
            Return New Byte() {2}
        End Function
    End Class

    Public Class GameLogoutSuccess
        Inherits GSPacket
        ' Methods
        Public Sub New()
            MyBase.New(Build())
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Shared Function Build() As Byte()
            Return New Byte() {6}
        End Function
    End Class

    Public Class GameMessage
        Inherits GSPacket
        ' Fields
        Protected m_message As String
        Protected m_messageType As GameMessageType
        Public Shared ReadOnly NULL_Int32 As Integer = -1
        Public Shared ReadOnly NULL_UInt32 As Integer = 0
        Protected m_playerName As String
        Protected m_random As Integer
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = UnitType.NotApplicable
            Me.m_random = -1
            Me.m_messageType = DirectCast(CUShort(BitConverter.ToInt16(data, 1)), GameMessageType)

            If Me.m_messageType = GameMessageType.OverheadMessage Then
                Me.m_unitType = DirectCast(data(3), UnitType)
                Me.m_uid = BitConverter.ToUInt32(data, 4)
                Me.m_random = BitConverter.ToUInt16(data, 8)
                Me.m_message = ByteConverter.GetNullString(data, 11)
            Else
                Me.m_playerName = ByteConverter.GetNullString(data, 10)
                Me.m_message = ByteConverter.GetNullString(data, 11 + Me.m_playerName.Length)
            End If

        End Sub

        Public Sub New(ByVal type As GameMessageType, ByVal charFlags As Byte, ByVal charName As String, ByVal message As String)
            MyBase.New(Build(type, charFlags, charName, message))
            Me.m_unitType = UnitType.NotApplicable
            Me.m_random = -1
            Me.m_messageType = type
            Me.m_playerName = charName
            Me.m_message = message
        End Sub

        Public Sub New(ByVal type As GameMessageType, ByVal charName As String, ByVal message As String)
            MyBase.New(Build(type, charName, message))
            Me.m_unitType = UnitType.NotApplicable
            Me.m_random = -1
            Me.m_messageType = type
            Me.m_playerName = charName
            Me.m_message = message
        End Sub

        Public Sub New(ByVal type As GameMessageType, ByVal message As String)
            MyBase.New(Build(type, message))
            Me.m_unitType = UnitType.NotApplicable
            Me.m_random = -1
            Me.m_messageType = type
            Me.m_playerName = Nothing
            Me.m_message = message
        End Sub

        Public Sub New(ByVal type As UnitType, ByVal uid As UInteger, ByVal random As UShort, ByVal message As String)
            MyBase.New(Build(type, uid, random, message))
            Me.m_unitType = UnitType.NotApplicable
            Me.m_random = -1
            Me.m_messageType = GameMessageType.OverheadMessage
            Me.m_uid = uid
            Me.m_random = random
            Me.m_message = message
        End Sub


        Public Shared Function Build(ByVal type As GameMessageType, ByVal charFlags As Byte, ByVal charName As String, ByVal message As String) As Byte()
            If (charName Is Nothing) OrElse (charName.Length = 0) Then
                Throw New ArgumentException("charName")
            End If
            If (message Is Nothing) OrElse (message.Length = 0) Then
                Throw New ArgumentException("message")
            End If
            Dim buffer As Byte() = New Byte((12 + charName.Length) + message.Length - 1) {}
            buffer(0) = 38
            buffer(1) = CByte(type)
            buffer(3) = 2
            buffer(9) = charFlags
            For i As Integer = 0 To charName.Length - 1
                buffer(10 + i) = AscW(charName(i))
            Next
            Dim num2 As Integer = 0
            Dim num3 As Integer = 11 + charName.Length
            While num2 < message.Length
                buffer(num3 + num2) = AscW(message(num2))
                num2 += 1
            End While
            Return buffer
        End Function
        Public Shared Function Build(ByVal type As GameMessageType, ByVal charName As String, ByVal message As String) As Byte()
            If (charName Is Nothing) OrElse (charName.Length = 0) Then
                Throw New ArgumentException("charName")
            End If
            If (message Is Nothing) OrElse (message.Length = 0) Then
                Throw New ArgumentException("message")
            End If
            Dim buffer As Byte() = New Byte((12 + charName.Length) + message.Length - 1) {}
            buffer(0) = 38
            buffer(1) = CByte(type)
            buffer(3) = 2
            buffer(9) = 5
            For i As Integer = 0 To charName.Length - 1
                buffer(10 + i) = AscW(charName(i))
            Next
            Dim num2 As Integer = 0
            Dim num3 As Integer = 11 + charName.Length
            While num2 < message.Length
                buffer(num3 + num2) = AscW(message(num2))
                num2 += 1
            End While
            Return buffer
        End Function
        Public Shared Function Build(ByVal type As GameMessageType, ByVal message As String) As Byte()

            If (message Is Nothing) OrElse (message.Length = 0) Then
                Throw New ArgumentException("message")
            End If

            Dim Buffer As Byte() = New Byte(((12 + 1) + message.Length) - 1) {}
            Buffer(0) = 38
            Buffer(1) = CByte(type)
            Buffer(3) = 2
            Buffer(9) = 5
            Dim num2 As Integer = 0
            Dim num3 As Integer = 11

            While (num2 < message.Length)
                Buffer(num3 + num2) = AscW(message(num2))
                num2 += 1
            End While

            Return Buffer
        End Function

        Public Shared Function Build(ByVal type As UnitType, ByVal uid As UInteger, ByVal random As UShort, ByVal message As String) As Byte()
            If (message Is Nothing) OrElse (message.Length = 0) Then
                Throw New ArgumentException("message")
            End If
            Dim buffer As Byte() = New Byte(12 + message.Length - 1) {}
            buffer(0) = 38
            buffer(1) = 5
            buffer(3) = CByte(type)
            buffer(4) = CByte(uid)
            buffer(5) = CByte((uid >> 8))
            buffer(6) = CByte((uid >> 16))
            buffer(7) = CByte((uid >> 24))
            buffer(8) = CByte(random)
            buffer(9) = CByte((random >> 8))
            For i As Integer = 0 To message.Length - 1
                buffer(11 + i) = AscW(message(i))
            Next
            Return buffer
        End Function

        ' Properties
        Public ReadOnly Property Message() As String
            Get
                Return Me.m_message
            End Get
        End Property

        Public ReadOnly Property MessageType() As GameMessageType
            Get
                Return Me.m_messageType
            End Get
        End Property

        Public ReadOnly Property PlayerName() As String
            Get
                Return Me.m_playerName
            End Get
        End Property

        Public ReadOnly Property Random() As Integer
            Get
                Return Me.m_random
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property Unknown3() As String
            Get
                If Me.m_messageType <> GameMessageType.OverheadMessage Then
                    Return ByteConverter.ToHexString(MyBase.GetData, 3, 7)
                End If
                Return Nothing
            End Get
        End Property
    End Class

    Public Class GameOver
        Inherits GSPacket
        ' Methods
        Public Sub New()
            MyBase.New(Build())
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Shared Function Build() As Byte()
            Return New Byte() {176}
        End Function
    End Class

    Public Class GoldTrade
        Inherits GSPacket
        ' Fields
        Protected m_amount As UInteger
        Protected m_myGold As Boolean

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_myGold = BitConverter.ToBoolean(data, 1)
            Me.m_amount = BitConverter.ToUInt32(data, 2)
        End Sub

        ' Properties
        Public ReadOnly Property Amount() As UInteger
            Get
                Return Me.m_amount
            End Get
        End Property

        Public ReadOnly Property MyGold() As Boolean
            Get
                Return Me.m_myGold
            End Get
        End Property
    End Class

    Public Class InformationMessage
        Inherits GSPacket
        ' Fields
        Protected m_actionType As Byte
        Protected m_amount As Integer
        Protected charClass As CharacterClass
        Protected m_informationType As PlayerInformationActionType
        Public Shared ReadOnly NULL_Int32 As Integer = -1
        Public Shared ReadOnly NULL_UInt32 As Integer = 0
        Protected m_objectName As String
        Protected m_objectUID As UInteger
        Protected m_relationType As PlayerRelationActionType
        Protected m_slayerMonster As NPCCode
        Protected m_slayerObject As GameObjectID
        Protected m_slayerType As UnitType
        Protected m_subjectName As String
        Protected m_type As InformationMessageType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_slayerType = UnitType.NotApplicable
            Me.charClass = CharacterClass.NotApplicable
            Me.m_slayerObject = GameObjectID.NotApplicable
            Me.m_slayerMonster = NPCCode.NotApplicable
            Me.m_informationType = PlayerInformationActionType.None
            Me.m_relationType = PlayerRelationActionType.NotApplicable
            Me.m_amount = -1
            Me.m_type = data(1)
            Me.m_actionType = data(2)
            Select Case Me.m_type
                Case InformationMessageType.DroppedFromGame, InformationMessageType.JoinedGame, InformationMessageType.LeftGame
                    Me.m_subjectName = ByteConverter.GetNullString(data, 8)
                    Me.m_objectName = ByteConverter.GetNullString(data, 24)
                    Return
                Case DirectCast(1, InformationMessageType), DirectCast(5, InformationMessageType)

                    Exit Select
                Case InformationMessageType.NotInGame

                    Me.m_subjectName = ByteConverter.GetNullString(data, 8)
                    Return
                Case InformationMessageType.PlayerSlain

                    Me.m_slayerType = DirectCast(data(7), UnitType)
                    Me.m_subjectName = ByteConverter.GetNullString(data, 8)
                    If Me.m_slayerType <> UnitType.Player Then
                        If Me.m_slayerType = UnitType.NPC Then
                            Me.m_slayerMonster = BitConverter.ToInt32(data, 3)

                            Return
                        End If
                        If Me.m_slayerType = UnitType.GameObject Then
                            Me.m_slayerObject = BitConverter.ToInt32(data, 3)
                            Return
                        End If
                        Exit Select
                    End If
                    Me.charClass = BitConverter.ToInt32(data, 3)
                    Me.m_objectName = ByteConverter.GetNullString(data, 24)
                    Return
                Case InformationMessageType.PlayerRelation

                    Me.m_informationType = Me.m_actionType
                    Me.m_objectUID = BitConverter.ToUInt32(data, 3)
                    Me.m_relationType = data(7)
                    Return
                Case InformationMessageType.SoJsSoldToMerchants

                    Me.m_amount = BitConverter.ToInt32(data, 3)
                    Exit Select
                Case Else

                    Return
            End Select
        End Sub

        ' Properties
        Public ReadOnly Property ActionType() As Byte
            Get
                Return Me.m_actionType
            End Get
        End Property

        Public ReadOnly Property Amount() As Integer
            Get
                Return Me.m_amount
            End Get
        End Property

        Public ReadOnly Property [Class]() As CharacterClass
            Get
                Return Me.charClass
            End Get
        End Property

        Public ReadOnly Property InformationType() As PlayerInformationActionType
            Get
                Return Me.m_informationType
            End Get
        End Property

        Public ReadOnly Property ObjectName() As String
            Get
                Return Me.m_objectName
            End Get
        End Property

        Public ReadOnly Property ObjectUID() As UInteger
            Get
                Return Me.m_objectUID
            End Get
        End Property

        Public ReadOnly Property RelationType() As PlayerRelationActionType
            Get
                Return Me.m_relationType
            End Get
        End Property

        Public ReadOnly Property SlayerMonster() As NPCCode
            Get
                Return Me.m_slayerMonster
            End Get
        End Property

        Public ReadOnly Property SlayerObject() As GameObjectID
            Get
                Return Me.m_slayerObject
            End Get
        End Property

        Public ReadOnly Property SlayerType() As UnitType
            Get
                Return Me.m_slayerType
            End Get
        End Property

        Public ReadOnly Property SubjectName() As String
            Get
                Return Me.m_subjectName
            End Get
        End Property

        Public ReadOnly Property Type() As InformationMessageType
            Get
                Return Me.m_type
            End Get
        End Property
    End Class

    Public Enum InformationMessageType
        DiabloWalksTheEarth = 18
        DroppedFromGame = 0
        JoinedGame = 2
        LeftGame = 3
        NotInGame = 4
        PlayerRelation = 7
        PlayerSlain = 6
        SoJsSoldToMerchants = 17
    End Enum

    Public Class ItemAction
        Inherits GSPacket
        ' Fields
        Protected m_action As ItemActionType
        Protected m_baseItem As BaseItem
        Protected m_category As ItemCategory
        Protected charClass As CharacterClass
        Protected m_color As Integer
        Protected m_container As ItemContainer
        Protected m_destination As ItemDestination
        Protected m_flags As ItemFlags
        Protected m_graphic As Integer
        Protected m_level As Integer
        Protected m_location As EquipmentLocation
        Protected m_magicPrefixes As List(Of MagicPrefixType)
        Protected m_magicSuffixes As List(Of MagicSuffixType)
        Protected m_mods As List(Of StatBase)
        Protected m_name As String
        Protected m_prefix As ItemAffix
        Protected m_quality As ItemQuality
        Protected m_runeword As BaseRuneword
        Protected m_runewordID As Integer
        Protected m_runewordParam As Integer
        Protected m_setBonuses As List(Of StatBase)()
        Protected m_setItem As BaseSetItem
        Protected m_stats As List(Of StatBase)
        Protected m_suffix As ItemAffix
        Protected m_superiorType As SuperiorItemType
        Protected m_uid As UInteger
        Protected m_uniqueItem As BaseUniqueItem
        Protected m_unknown1 As Integer
        Protected m_use As Integer
        Protected m_usedSockets As Integer
        Protected m_version As ItemVersion
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_superiorType = SuperiorItemType.NotApplicable
            Me.charClass = CharacterClass.NotApplicable
            Me.m_level = -1
            Me.m_usedSockets = -1
            Me.m_use = -1
            Me.m_graphic = -1
            Me.m_color = -1
            Me.m_stats = New List(Of StatBase)()
            Me.m_unknown1 = -1
            Me.m_runewordID = -1
            Me.m_runewordParam = -1
            Dim br As New BitReader(data, 1)
            Me.m_action = br.ReadByte()
            br.SkipBytes(1)
            Me.m_category = br.ReadByte()
            Me.m_uid = br.ReadUInt32()
            If data(0) = 157 Then
                br.SkipBytes(5)
            End If
            Me.m_flags = br.ReadUInt32()
            Me.m_version = br.ReadByte()
            Me.m_unknown1 = br.ReadByte(2)
            Me.m_destination = br.ReadByte(3)
            If Me.m_destination = ItemDestination.Ground Then
                Me.m_x = br.ReadUInt16()
                Me.m_y = br.ReadUInt16()
            Else
                Me.m_location = br.ReadByte(4)
                Me.m_x = br.ReadByte(4)
                Me.m_y = br.ReadByte(3)
                Me.m_container = br.ReadByte(4)
            End If
            If (Me.m_action = ItemActionType.AddToShop) OrElse (Me.m_action = ItemActionType.RemoveFromShop) Then
                Dim num As Integer = CInt(Me.m_container) Or 128
                If (num And 1) = 1 Then
                    num -= 1
                    Me.m_y += 8
                End If
                Me.m_container = DirectCast(num, ItemContainer)
            ElseIf Me.m_container = ItemContainer.Unspecified Then
                If Me.m_location = EquipmentLocation.NotApplicable Then
                    If (Me.Flags And ItemFlags.InSocket) = ItemFlags.InSocket Then
                        Me.m_container = ItemContainer.Item
                        Me.m_y = -1
                    ElseIf (Me.m_action = ItemActionType.PutInBelt) OrElse (Me.m_action = ItemActionType.RemoveFromBelt) Then
                        Me.m_container = ItemContainer.Belt
                        Me.m_y = Me.m_x / 4
                        Me.m_x = Me.m_x Mod 4
                    End If
                Else
                    Me.m_x = -1
                    Me.m_y = -1
                End If
            End If
            If (Me.m_flags And ItemFlags.Ear) = ItemFlags.Ear Then
                Me.charClass = br.ReadByte(3)
                Me.m_level = br.ReadByte(7)
                Me.m_name = br.ReadString(7, Chr(0), 16)
                Me.m_baseItem = BaseItem.[Get](ItemType.Ear)
            Else
                Me.m_baseItem = BaseItem.GetByID(Me.m_category, br.ReadUInt32())
                If Me.m_baseItem.Type = ItemType.Gold Then
                    Me.m_stats.Add(New SignedStat(BaseStat.[Get](StatType.Quantity), br.ReadInt32(IIf(br.ReadBoolean(1), 32, 12))))
                Else
                    Me.m_usedSockets = br.ReadByte(3)
                    If (Me.m_flags And (ItemFlags.Compact Or ItemFlags.Gamble)) = ItemFlags.None Then
                        Dim stat As BaseStat
                        Dim num2 As Integer
                        Me.m_level = br.ReadByte(7)
                        Me.m_quality = br.ReadByte(4)
                        If br.ReadBoolean(1) Then
                            Me.m_graphic = br.ReadByte(3)
                        End If
                        If br.ReadBoolean(1) Then
                            Me.m_color = br.ReadInt32(11)
                        End If
                        If (Me.m_flags And ItemFlags.Identified) = ItemFlags.Identified Then
                            Select Case Me.m_quality
                                Case ItemQuality.Inferior
                                    Me.m_prefix = New ItemAffix(ItemAffixType.InferiorPrefix, br.ReadByte(3))
                                    Exit Select
                                Case ItemQuality.Superior

                                    Me.m_prefix = New ItemAffix(ItemAffixType.SuperiorPrefix, 0)
                                    Me.m_superiorType = br.ReadByte(3)
                                    Exit Select
                                Case ItemQuality.Magic

                                    Me.m_prefix = New ItemAffix(ItemAffixType.MagicPrefix, br.ReadUInt16(11))
                                    Me.m_suffix = New ItemAffix(ItemAffixType.MagicSuffix, br.ReadUInt16(11))
                                    Exit Select
                                Case ItemQuality.[Set]

                                    Me.m_setItem = BaseSetItem.[Get](br.ReadUInt16(12))
                                    Exit Select
                                Case ItemQuality.Rare, ItemQuality.Crafted

                                    Me.m_prefix = New ItemAffix(ItemAffixType.RarePrefix, br.ReadByte(8))
                                    Me.m_suffix = New ItemAffix(ItemAffixType.RareSuffix, br.ReadByte(8))
                                    Exit Select
                                Case ItemQuality.Unique

                                    If Me.m_baseItem.Code <> "std" Then
                                        Try
                                            Me.m_uniqueItem = BaseUniqueItem.[Get](br.ReadUInt16(12))
                                        Catch
                                        End Try
                                    End If
                                    Exit Select
                            End Select
                        End If
                        If (Me.m_quality = ItemQuality.Rare) OrElse (Me.m_quality = ItemQuality.Crafted) Then
                            Me.m_magicPrefixes = New List(Of MagicPrefixType)()
                            Me.m_magicSuffixes = New List(Of MagicSuffixType)()
                            For i As Integer = 0 To 2
                                If br.ReadBoolean(1) Then
                                    Me.m_magicPrefixes.Add(br.ReadUInt16(11))
                                End If
                                If br.ReadBoolean(1) Then
                                    Me.m_magicSuffixes.Add(br.ReadUInt16(11))
                                End If
                            Next
                        End If
                        If (Me.Flags And ItemFlags.Runeword) = ItemFlags.Runeword Then
                            Me.m_runewordID = br.ReadUInt16(12)
                            Me.m_runewordParam = br.ReadUInt16(4)
                            num2 = -1
                            If Me.m_runewordParam = 5 Then
                                num2 = Me.m_runewordID - (Me.m_runewordParam * 5)
                                If num2 < 100 Then
                                    num2 -= 1
                                End If
                            ElseIf Me.m_runewordParam = 2 Then
                                num2 = ((Me.m_runewordID And 1023) >> 5) + 2
                            End If
                            br.ByteOffset -= 2
                            Me.m_runewordParam = br.ReadUInt16()
                            Me.m_runewordID = num2
                            If num2 = -1 Then
                                Throw New Exception("Unknown Runeword: " + Me.m_runewordParam)
                            End If
                            Me.m_runeword = BaseRuneword.[Get](num2)
                        End If
                        If (Me.Flags And ItemFlags.Personalized) = ItemFlags.Personalized Then
                            Me.m_name = br.ReadString(7, Chr(0), 16)
                        End If
                        If TypeOf Me.m_baseItem Is BaseArmor Then
                            stat = BaseStat.[Get](StatType.ArmorClass)
                            Me.m_stats.Add(New SignedStat(stat, br.ReadInt32(stat.SaveBits) - stat.SaveAdd))
                        End If
                        If (TypeOf Me.m_baseItem Is BaseArmor) OrElse (TypeOf Me.m_baseItem Is BaseWeapon) Then
                            stat = BaseStat.[Get](StatType.MaxDurability)
                            num2 = br.ReadInt32(stat.SaveBits)
                            Me.m_stats.Add(New SignedStat(stat, num2))
                            If num2 > 0 Then
                                stat = BaseStat.[Get](StatType.Durability)
                                Me.m_stats.Add(New SignedStat(stat, br.ReadInt32(stat.SaveBits)))
                            End If
                        End If
                        If (Me.Flags And (ItemFlags.None Or ItemFlags.Socketed)) = (ItemFlags.None Or ItemFlags.Socketed) Then
                            stat = BaseStat.[Get](StatType.Sockets)
                            Me.m_stats.Add(New SignedStat(stat, br.ReadInt32(stat.SaveBits)))
                        End If
                        If Me.m_baseItem.Stackable Then
                            If Me.m_baseItem.Useable Then
                                Me.m_use = br.ReadByte(5)
                            End If
                            Me.m_stats.Add(New SignedStat(BaseStat.[Get](StatType.Quantity), br.ReadInt32(9)))
                        End If
                        If (Me.Flags And ItemFlags.Identified) = ItemFlags.Identified Then
                            Dim base2 As StatBase
                            Dim num4 As Integer = IIf((Me.Quality = ItemQuality.[Set]), br.ReadByte(5), -1)
                            Me.m_mods = New List(Of StatBase)()

                            While True
                                Try
                                    base2 = ReadStat(br)
                                Catch ex As Exception
                                End Try

                                If base2 IsNot Nothing Then
                                    Me.m_mods.Add(base2)
                                Else
                                    Exit While
                                End If
                            End While

                            If (Me.m_flags And ItemFlags.Runeword) = ItemFlags.Runeword Then
                                While True
                                    base2 = ReadStat(br)
                                    If base2 IsNot Nothing Then
                                        Me.m_mods.Add(base2)
                                    Else
                                        Exit While
                                    End If
                                End While
                            End If
                            If num4 > 0 Then
                                Me.m_setBonuses = New List(Of StatBase)(4) {}
                                For j As Integer = 0 To 4
                                    If (num4 And (CInt(1) << j)) <> 0 Then
                                        Me.m_setBonuses(j) = New List(Of StatBase)()

                                        While True
                                            base2 = ReadStat(br)
                                            If base2 IsNot Nothing Then
                                                Me.m_setBonuses(j).Add(base2)
                                            Else
                                                Exit While
                                            End If
                                        End While

                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Private Shared Function ReadStat(ByVal br As BitReader) As StatBase
            Dim index As Integer = br.ReadInt32(9)
            If index = 511 Then
                Return Nothing
            End If
            Dim stat As BaseStat = BaseStat.[Get](index)
            If stat.SaveParamBits = -1 Then
                If stat.OpBase = StatType.Level Then
                    Return New PerLevelStat(stat, br.ReadInt32(stat.SaveBits))
                End If
                Select Case stat.Type
                    Case StatType.MaxDamagePercent, StatType.MinDamagePercent
                        Return New DamageRangeStat(stat, br.ReadInt32(stat.SaveBits), br.ReadInt32(stat.SaveBits))
                    Case StatType.FireMinDamage, StatType.LightMinDamage, StatType.MagicMinDamage

                        Return New DamageRangeStat(stat, br.ReadInt32(stat.SaveBits), br.ReadInt32(BaseStat.[Get](CInt((stat.Index + 1))).SaveBits))
                    Case StatType.FireMaxDamage, StatType.LightMaxDamage, StatType.MagicMaxDamage

                        GoTo Label_0350
                    Case StatType.ColdMinDamage

                        Return New ColdDamageStat(stat, br.ReadInt32(stat.SaveBits), br.ReadInt32(BaseStat.[Get](StatType.ColdMaxDamage).SaveBits), br.ReadInt32(BaseStat.[Get](StatType.ColdLength).SaveBits))
                    Case StatType.ReplenishDurability, StatType.ReplenishQuantity

                        Return New ReplenishStat(stat, br.ReadInt32(stat.SaveBits))
                    Case StatType.PoisonMinDamage

                        Return New PoisonDamageStat(stat, br.ReadInt32(stat.SaveBits), br.ReadInt32(BaseStat.[Get](StatType.PoisonMaxDamage).SaveBits), br.ReadInt32(BaseStat.[Get](StatType.PoisonLength).SaveBits))
                End Select
            Else
                Select Case stat.Type
                    Case StatType.SingleSkill, StatType.NonClassSkill
                        Return New SkillBonusStat(stat, br.ReadInt32(stat.SaveParamBits), br.ReadInt32(stat.SaveBits))
                    Case StatType.ElementalSkillBonus

                        Return New ElementalSkillsBonusStat(stat, br.ReadInt32(stat.SaveParamBits), br.ReadInt32(stat.SaveBits))
                    Case StatType.ClassSkillsBonus

                        Return New ClassSkillsBonusStat(stat, br.ReadInt32(stat.SaveParamBits), br.ReadInt32(stat.SaveBits))
                    Case StatType.Aura

                        Return New AuraStat(stat, br.ReadInt32(stat.SaveParamBits), br.ReadInt32(stat.SaveBits))
                    Case StatType.Reanimate

                        Return New ReanimateStat(stat, br.ReadUInt32(stat.SaveParamBits), br.ReadUInt32(stat.SaveBits))
                    Case StatType.SkillOnAttack, StatType.SkillOnKill, StatType.SkillOnDeath, StatType.SkillOnStriking, StatType.SkillOnLevelUp, StatType.SkillOnGetHit

                        Return New SkillOnEventStat(stat, br.ReadInt32(6), br.ReadInt32(10), br.ReadInt32(stat.SaveBits))
                    Case StatType.ChargedSkill

                        Return New ChargedSkillStat(stat, br.ReadInt32(6), br.ReadInt32(10), br.ReadInt32(8), br.ReadInt32(8))
                    Case StatType.SkillTabBonus

                        Return New SkillTabBonusStat(stat, br.ReadInt32(3), br.ReadInt32(3), br.ReadInt32(10), br.ReadInt32(stat.SaveBits))
                End Select
                Throw New Exception("Invalid stat: " + stat.Index.ToString())
            End If
Label_0350:
            If stat.Signed Then
                Dim num2 As Integer = br.ReadInt32(stat.SaveBits)
                If stat.SaveAdd > 0 Then
                    num2 -= stat.SaveAdd
                End If
                Return New SignedStat(stat, num2)
            End If

            Dim val As UInteger = br.ReadUInt32(stat.SaveBits)

            If stat.SaveAdd > 0 Then
                val -= CInt(stat.SaveAdd)
            End If

            Return New UnsignedStat(stat, Val)
        End Function

        ' Properties
        Public ReadOnly Property Action() As ItemActionType
            Get
                Return Me.m_action
            End Get
        End Property

        Public ReadOnly Property BaseItem() As BaseItem
            Get
                Return Me.m_baseItem
            End Get
        End Property

        Public ReadOnly Property Category() As ItemCategory
            Get
                Return Me.m_category
            End Get
        End Property

        Public ReadOnly Property [Class]() As CharacterClass
            Get
                Return Me.charClass
            End Get
        End Property

        Public ReadOnly Property Color() As Integer
            Get
                Return Me.m_color
            End Get
        End Property

        Public ReadOnly Property Container() As ItemContainer
            Get
                Return Me.m_container
            End Get
        End Property

        Public ReadOnly Property Destination() As ItemDestination
            Get
                Return Me.m_destination
            End Get
        End Property

        Public ReadOnly Property Flags() As ItemFlags
            Get
                Return Me.m_flags
            End Get
        End Property

        Public ReadOnly Property Graphic() As Integer
            Get
                Return Me.m_graphic
            End Get
        End Property

        Public ReadOnly Property Level() As Integer
            Get
                Return Me.m_level
            End Get
        End Property

        Public ReadOnly Property Location() As EquipmentLocation
            Get
                Return Me.m_location
            End Get
        End Property

        Public ReadOnly Property MagicPrefixes() As List(Of MagicPrefixType)
            Get
                Return Me.m_magicPrefixes
            End Get
        End Property

        Public ReadOnly Property MagicSuffixes() As List(Of MagicSuffixType)
            Get
                Return Me.m_magicSuffixes
            End Get
        End Property

        Public ReadOnly Property Mods() As List(Of StatBase)
            Get
                Return Me.m_mods
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return Me.m_name
            End Get
        End Property

        Public ReadOnly Property Prefix() As ItemAffix
            Get
                Return Me.m_prefix
            End Get
        End Property

        Public ReadOnly Property Quality() As ItemQuality
            Get
                Return Me.m_quality
            End Get
        End Property

        Public ReadOnly Property Runeword() As BaseRuneword
            Get
                Return Me.m_runeword
            End Get
        End Property

        Public ReadOnly Property RunewordID() As Integer
            Get
                Return Me.m_runewordID
            End Get
        End Property

        Public ReadOnly Property RunewordParam() As Integer
            Get
                Return Me.m_runewordParam
            End Get
        End Property

        Public ReadOnly Property SetBonuses() As List(Of StatBase)()
            Get
                Return Me.m_setBonuses
            End Get
        End Property

        Public ReadOnly Property SetItem() As BaseSetItem
            Get
                Return Me.m_setItem
            End Get
        End Property

        Public ReadOnly Property Stats() As List(Of StatBase)
            Get
                Return Me.m_stats
            End Get
        End Property

        Public ReadOnly Property Suffix() As ItemAffix
            Get
                Return Me.m_suffix
            End Get
        End Property

        Public ReadOnly Property SuperiorType() As SuperiorItemType
            Get
                Return Me.m_superiorType
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UniqueItem() As BaseUniqueItem
            Get
                Return Me.m_uniqueItem
            End Get
        End Property

        Public ReadOnly Property Unknown1() As Integer
            Get
                If Me.m_unknown1 <> 0 Then
                    Return Me.m_unknown1
                End If
                Return -1
            End Get
        End Property

        Public ReadOnly Property Use() As Integer
            Get
                Return Me.m_use
            End Get
        End Property

        Public ReadOnly Property UsedSockets() As Integer
            Get
                If (Me.m_usedSockets = 0) AndAlso ((Me.m_flags And (ItemFlags.None Or ItemFlags.Socketed)) <> (ItemFlags.None Or ItemFlags.Socketed)) Then
                    Return -1
                End If
                Return Me.m_usedSockets
            End Get
        End Property

        Public ReadOnly Property Version() As ItemVersion
            Get
                Return Me.m_version
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Enum ItemEventCause
        Target
        Owner
    End Enum

    Public Enum ItemStateType
        Broken = 1
        Full = 2
    End Enum

    Public Class ItemTriggerSkill
        Inherits GSPacket
        ' Fields
        Protected m_cause As ItemEventCause
        Protected m_level As Byte
        Protected m_ownerType As UnitType
        Protected m_ownerUID As UInteger
        Protected m_skill As SkillType
        Protected m_targetType As UnitType
        Protected m_targetUID As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_ownerType = DirectCast(data(1), UnitType)
            Me.m_ownerUID = BitConverter.ToUInt32(data, 2)
            Me.m_skill = DirectCast(BitConverter.ToUInt16(data, 6), SkillType)
            Me.m_level = data(8)
            Me.m_targetType = DirectCast(data(9), UnitType)
            Me.m_targetUID = BitConverter.ToUInt32(data, 10)
            Me.m_cause = BitConverter.ToUInt16(data, 14)
        End Sub

        ' Properties
        Public ReadOnly Property Cause() As ItemEventCause
            Get
                Return Me.m_cause
            End Get
        End Property

        Public ReadOnly Property Level() As Byte
            Get
                Return Me.m_level
            End Get
        End Property

        Public ReadOnly Property OwnerType() As UnitType
            Get
                Return Me.m_ownerType
            End Get
        End Property

        Public ReadOnly Property OwnerUID() As UInteger
            Get
                Return Me.m_ownerUID
            End Get
        End Property

        Public ReadOnly Property Skill() As SkillType
            Get
                Return Me.m_skill
            End Get
        End Property

        Public ReadOnly Property TargetType() As UnitType
            Get
                Return Me.m_targetType
            End Get
        End Property

        Public ReadOnly Property TargetUID() As UInteger
            Get
                Return Me.m_targetUID
            End Get
        End Property
    End Class

    Public Class LoadAct
        Inherits GSPacket
        ' Fields
        Protected m_act As ActLocation
        Protected m_mapId As UInteger
        Protected m_townArea As AreaLevel

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_act = data(1)
            Me.m_mapId = BitConverter.ToUInt32(data, 2)
            Me.m_townArea = BitConverter.ToUInt16(data, 6)
        End Sub

        ' Properties
        Public ReadOnly Property Act() As ActLocation
            Get
                Return Me.m_act
            End Get
        End Property

        Public ReadOnly Property MapId() As UInteger
            Get
                Return Me.m_mapId
            End Get
        End Property

        Public ReadOnly Property TownArea() As AreaLevel
            Get
                Return Me.m_townArea
            End Get
        End Property

        Public ReadOnly Property Unknown8() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 8, 4)
            End Get

        End Property
    End Class

    Public Class LoadDone
        Inherits GSPacket
        ' Methods
        Public Sub New()
            MyBase.New(Build())
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Shared Function Build() As Byte()
            Return New Byte() {4}
        End Function
    End Class

    Public Class MapAdd
        Inherits GSPacket
        ' Fields
        Protected m_area As AreaLevel
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_x = BitConverter.ToUInt16(data, 1)
            Me.m_y = BitConverter.ToUInt16(data, 3)
            Me.m_area = data(5)
        End Sub

        Public Sub New(ByVal area As AreaLevel, ByVal x As Integer, ByVal y As Integer)
            MyBase.New(Build(area, x, y))
            Me.m_x = x
            Me.m_y = y
            Me.m_area = area
        End Sub

        Public Shared Function Build(ByVal area As AreaLevel, ByVal x As Integer, ByVal y As Integer) As Byte()
            Return New Byte() {7, CByte(x), CByte((x >> 8)), CByte(y), CByte((y >> 8)), CByte(area)}
        End Function

        ' Properties
        Public ReadOnly Property Area() As AreaLevel
            Get
                Return Me.m_area
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Class MapRemove
        Inherits GSPacket
        ' Fields
        Protected m_area As AreaLevel
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_x = BitConverter.ToUInt16(data, 1)
            Me.m_y = BitConverter.ToUInt16(data, 3)
            Me.m_area = data(5)
        End Sub

        Public Sub New(ByVal area As AreaLevel, ByVal x As Integer, ByVal y As Integer)
            MyBase.New(Build(area, x, y))
            Me.m_x = x
            Me.m_y = y
            Me.m_area = area
        End Sub

        Public Shared Function Build(ByVal area As AreaLevel, ByVal x As Integer, ByVal y As Integer) As Byte()
            Return New Byte() {8, CByte(x), CByte((x >> 8)), CByte(y), CByte((y >> 8)), CByte(area)}
        End Function

        ' Properties
        Public ReadOnly Property Area() As AreaLevel
            Get
                Return Me.m_area
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Class MercAttributeByte
        Inherits MercAttributeNotification
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Dim stat As BaseStat = BaseStat.[Get](data(1))
            If stat.Signed Then
                MyBase.m_stat = New SignedStat(stat, data(6))
            Else
                MyBase.m_stat = New UnsignedStat(stat, data(6))
            End If
        End Sub
    End Class

    Public Class MercAttributeDWord
        Inherits MercAttributeNotification
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Dim stat As BaseStat = BaseStat.[Get](data(1))
            Dim val As Integer = BitConverter.ToInt32(data, 6)
            If stat.ValShift > 0 Then
                val = val >> stat.ValShift
            End If
            If stat.Signed Then
                MyBase.m_stat = New SignedStat(stat, val)
            Else
                MyBase.m_stat = New UnsignedStat(stat, CInt(val))
            End If
        End Sub
    End Class

    Public Class MercAttributeNotification
        Inherits GSPacket
        ' Fields
        Protected m_stat As StatBase
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
        End Sub

        ' Properties
        Public ReadOnly Property Stat() As StatBase
            Get
                Return Me.m_stat
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class

    Public Class MercAttributeWord
        Inherits MercAttributeNotification
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Dim stat As BaseStat = BaseStat.[Get](data(1))
            Dim val As Integer = IIf((data(6) = 0), data(7), BitConverter.ToUInt16(data, 6))
            If stat.Signed Then
                MyBase.m_stat = New SignedStat(stat, val)
            Else
                MyBase.m_stat = New UnsignedStat(stat, CInt(val))
            End If
        End Sub
    End Class

    Public Class MercByteToExperience
        Inherits MercGainExperience
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            MyBase.m_experience = data(6)
        End Sub
    End Class

    Public Class MercForHire
        Inherits GSPacket
        ' Fields
        Protected m_mercID As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_mercID = BitConverter.ToUInt16(data, 1)
        End Sub

        ' Properties
        Public ReadOnly Property MercID() As Integer
            Get
                Return Me.m_mercID
            End Get
        End Property

        Public ReadOnly Property Unknown3() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 3, 4)
            End Get
        End Property
    End Class

    Public Class MercForHireListStart
        Inherits GSPacket
        ' Methods
        Public Sub New()
            MyBase.New(Build())
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Shared Function Build() As Byte()
            Return New Byte() {79}
        End Function
    End Class

    Public Class MercGainExperience
        Inherits GSPacket
        ' Fields
        Protected m_experience As UInteger
        Protected m_id As Byte
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_id = data(1)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
        End Sub

        ' Properties
        Public ReadOnly Property Experience() As UInteger
            Get
                Return Me.m_experience
            End Get
        End Property

        Protected ReadOnly Property ID() As Byte
            Get
                Return Me.m_id
            End Get
        End Property

        Protected ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class

    Public Class MercWordToExperience
        Inherits MercGainExperience
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            MyBase.m_experience = BitConverter.ToUInt16(data, 6)
        End Sub
    End Class

    Public Class MonsterAttack
        Inherits GSPacket
        ' Fields
        Protected m_attackType As UShort
        Protected m_targetType As UnitType
        Protected m_targetUID As UInteger
        Protected m_uid As UInteger
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_attackType = BitConverter.ToUInt16(data, 5)
            Me.m_targetUID = BitConverter.ToUInt32(data, 7)
            Me.m_targetType = DirectCast(data(11), UnitType)
            Me.m_x = BitConverter.ToUInt16(data, 12)
            Me.m_y = BitConverter.ToUInt16(data, 14)
        End Sub

        ' Properties
        Public ReadOnly Property AttackType() As UShort
            Get
                Return Me.m_attackType
            End Get
        End Property

        Public ReadOnly Property TargetType() As UnitType
            Get
                Return Me.m_targetType
            End Get
        End Property

        Public ReadOnly Property TargetUID() As UInteger
            Get
                Return Me.m_targetUID
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Class NPCAction
        Inherits GSPacket
        ' Fields
        Protected m_actionType As UShort
        Protected m_uid As UInteger
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_actionType = data(5)
            Me.m_x = BitConverter.ToUInt16(data, 12)
            Me.m_y = BitConverter.ToUInt16(data, 14)
        End Sub

        ' Properties
        Public ReadOnly Property ActionType() As UShort
            Get
                Return Me.m_actionType
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Class NPCGetHit
        Inherits GSPacket
        ' Fields
        Protected m_animation As Integer
        Protected m_life As Byte
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_animation = BitConverter.ToUInt16(data, 6)
            Me.m_life = data(8)
        End Sub

        Public Sub New(ByVal type As UnitType, ByVal uid As UInteger, ByVal life As Byte, ByVal anim As Integer)
            MyBase.New(Build(type, uid, life, anim))
            Me.m_unitType = type
            Me.m_uid = uid
            Me.m_animation = anim
            Me.m_life = life
        End Sub

        Public Shared Function Build(ByVal type As UnitType, ByVal uid As UInteger, ByVal life As Byte, ByVal anim As Integer) As Byte()
            If life > 128 Then
                Throw New ArgumentOutOfRangeException("life")
            End If
            Return New Byte() {12, CByte(type), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24)), _
             CByte(anim), CByte((anim >> 8)), life}
        End Function

        ' Properties
        Public ReadOnly Property Animation() As Integer
            Get
                Return Me.m_animation
            End Get
        End Property

        Public ReadOnly Property Life() As Byte
            Get
                Return Me.m_life
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class

    Public Class NPCHeal
        Inherits GSPacket
        ' Fields
        Protected m_life As Byte
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_life = data(6)
        End Sub

        ' Properties
        Public ReadOnly Property Life() As Byte
            Get
                Return Me.m_life
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class




    Public Class NPCInfo
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
        End Sub

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property Unknown6() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 6, 34)
            End Get
        End Property
    End Class




    Public Enum NPCMode
        Alive = 6
        Dead = 9
        Dying = 8
    End Enum




    Public Class NPCMove
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger
        Protected m_unknown5 As Byte
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_unknown5 = data(5)
            Me.m_x = BitConverter.ToUInt16(data, 6)
            Me.m_y = BitConverter.ToUInt16(data, 8)
        End Sub

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property Unknown10() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 10, 2)
            End Get
        End Property

        Public ReadOnly Property Unknown12() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 12, 4)
            End Get
        End Property

        Public ReadOnly Property Unknown5() As Byte
            Get
                Return Me.m_unknown5
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class



    Public Class NPCMoveToTarget
        Inherits GSPacket
        ' Fields
        Protected m_currentX As Integer
        Protected m_currentY As Integer
        Protected m_movementType As Byte
        Protected m_targetType As UnitType
        Protected m_targetUID As UInteger
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_movementType = data(5)
            Me.m_currentX = BitConverter.ToUInt16(data, 6)
            Me.m_currentY = BitConverter.ToUInt16(data, 8)
            Me.m_targetType = DirectCast(data(10), UnitType)
            Me.m_targetUID = BitConverter.ToUInt32(data, 11)
        End Sub

        ' Properties
        Public ReadOnly Property CurrentX() As Integer
            Get
                Return Me.m_currentX
            End Get
        End Property

        Public ReadOnly Property CurrentY() As Integer
            Get
                Return Me.m_currentY
            End Get
        End Property

        Public ReadOnly Property MovementType() As Byte
            Get
                Return Me.m_movementType
            End Get
        End Property

        Public ReadOnly Property TargetType() As UnitType
            Get
                Return Me.m_targetType
            End Get
        End Property

        Public ReadOnly Property TargetUID() As UInteger
            Get
                Return Me.m_targetUID
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property Unknown15() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 15, 2)
            End Get
        End Property

        Public ReadOnly Property Unknown17() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 17, 4)
            End Get
        End Property
    End Class




    Public Class NPCState
        ' Fields
        Public ReadOnly State As BaseState
        Public ReadOnly Stats As List(Of StatBase)

        ' Methods
        Public Sub New(ByVal state As BaseState)
            Me.State = state
            Me.Stats = Nothing
        End Sub

        Public Sub New(ByVal state As BaseState, ByVal stats As List(Of StatBase))
            Me.State = state
            Me.Stats = stats
        End Sub

        Public Overloads Overrides Function ToString() As String
            If (Me.Stats Is Nothing) OrElse (Me.Stats.Count = 0) Then
                Return Me.State.ToString()
            End If
            Dim builder As New StringBuilder()
            builder.Append(Me.State)
            builder.Append(" (")
            Dim num As Integer = 0
            While True
                builder.Append(Me.Stats(num).ToString())
                If System.Threading.Interlocked.Increment(num) >= Me.Stats.Count Then
                    Exit While
                End If
                builder.Append(", ")
            End While
            builder.Append(")")
            Return builder.ToString()
        End Function
    End Class




    Public Class NPCStop
        Inherits GSPacket
        ' Fields
        Protected m_life As Byte
        Protected m_uid As UInteger
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_x = BitConverter.ToUInt16(data, 5)
            Me.m_y = BitConverter.ToUInt16(data, 7)
            Me.m_life = data(9)
        End Sub

        ' Properties
        Public ReadOnly Property Life() As Byte
            Get
                Return Me.m_life
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class




    Public Class NPCWantsInteract
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
        End Sub

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class




    Public Class OpenWaypoint
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger
        Protected m_waypoints As WaypointsAvailiable

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_waypoints = DirectCast(BitConverter.ToUInt64(data, 7), WaypointsAvailiable)
        End Sub

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property Waypoints() As WaypointsAvailiable
            Get
                Return Me.m_waypoints
            End Get
        End Property
    End Class




    Public Class OwnedItemAction
        Inherits ItemAction
        ' Fields
        Public Shared ReadOnly NULL_Int32 As Integer = -1
        Protected m_ownerType As UnitType
        Protected m_ownerUID As UInteger
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_ownerType = DirectCast(data(8), UnitType)
            Me.m_ownerUID = BitConverter.ToUInt32(data, 9)
        End Sub

        ' Properties
        Public ReadOnly Property OwnerType() As UnitType
            Get
                Return Me.m_ownerType
            End Get
        End Property

        Public ReadOnly Property OwnerUID() As UInteger
            Get
                Return Me.m_ownerUID
            End Get
        End Property
    End Class




    Public Class PartyMemberPulse
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_x = BitConverter.ToInt32(data, 5)
            Me.m_y = BitConverter.ToInt32(data, 9)
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal x As Integer, ByVal y As Integer)
            MyBase.New(Build(uid, x, y))
            Me.m_uid = uid
            Me.m_x = x
            Me.m_y = y
        End Sub

        Public Shared Function Build(ByVal uid As UInteger, ByVal x As Integer, ByVal y As Integer) As Byte()
            Return New Byte() {144, CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24)), CByte(x), _
             CByte((x >> 8)), CByte((x >> 16)), CByte((x >> 24)), CByte(y), CByte((y >> 8)), CByte((y >> 16)), _
             CByte((y >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class




    Public Class PartyMemberUpdate
        Inherits GSPacket
        ' Fields
        Protected m_area As AreaLevel
        Protected m_isPlayer As Boolean
        Protected m_lifePercent As Integer
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_isPlayer = Convert.ToBoolean(data(1))
            Me.m_lifePercent = BitConverter.ToUInt16(data, 2)
            Me.m_uid = BitConverter.ToUInt32(data, 4)
            Me.m_area = BitConverter.ToUInt16(data, 8)
        End Sub

        ' Properties
        Public ReadOnly Property Area() As AreaLevel
            Get
                Return Me.m_area
            End Get
        End Property

        Public ReadOnly Property IsPlayer() As Boolean
            Get
                Return Me.m_isPlayer
            End Get
        End Property

        Public ReadOnly Property LifePercent() As Integer
            Get
                Return Me.m_lifePercent
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class




    Public Class PartyRefresh
        Inherits GSPacket
        ' Fields
        Protected m_alternator As Byte
        Protected m_count As UInteger
        Protected m_slotNumber As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_slotNumber = BitConverter.ToUInt32(data, 1)
            Me.m_alternator = data(5)
            Me.m_count = BitConverter.ToUInt32(data, 4)
        End Sub

        ' Properties
        Public ReadOnly Property Alternator() As Byte
            Get
                Return Me.m_alternator
            End Get
        End Property

        Public ReadOnly Property SlotCount() As UInteger
            Get
                Return Me.m_count
            End Get
        End Property

        Public ReadOnly Property SlotNumber() As UInteger
            Get
                Return Me.m_slotNumber
            End Get
        End Property
    End Class




    Public Class PlayerAttributeNotification
        Inherits GSPacket
        ' Fields
        Protected m_stat As StatBase
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Dim stat As BaseStat = BaseStat.[Get](data(5))
            Dim val As Integer = BitConverter.ToInt32(data, 6)
            If stat.ValShift > 0 Then
                val = val >> stat.ValShift
            End If
            If stat.Signed Then
                Me.m_stat = New SignedStat(stat, val)
            Else
                Me.m_stat = New UnsignedStat(stat, CInt(val))
            End If
        End Sub

        ' Properties
        Public ReadOnly Property Stat() As StatBase
            Get
                Return Me.m_stat
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class




    Public Class PlayerClearCursor
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(Build(unitType, uid))
            Me.m_unitType = unitType
            Me.m_uid = uid
        End Sub

        Public Shared Function Build(ByVal unitType As UnitType, ByVal uid As UInteger) As Byte()
            Return New Byte() {66, CByte(unitType), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class




    Public Class PlayerCorpseVisible
        Inherits GSPacket
        ' Fields
        Protected m_assign As Boolean
        Protected m_corpseUID As UInteger
        Protected m_playerUID As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_assign = Convert.ToBoolean(data(1))
            Me.m_playerUID = BitConverter.ToUInt32(data, 2)
            Me.m_corpseUID = BitConverter.ToUInt32(data, 6)
        End Sub

        Public Sub New(ByVal assign As Boolean, ByVal playerUID As UInteger, ByVal corpseUID As UInteger)
            MyBase.New(Build(assign, playerUID, corpseUID))
            Me.m_assign = assign
            Me.m_playerUID = playerUID
            Me.m_corpseUID = corpseUID
        End Sub

        Public Shared Function Build(ByVal assign As Boolean, ByVal playerUID As UInteger, ByVal corpseUID As UInteger) As Byte()
            Return New Byte() {116, (IIf(assign, CByte(1), CByte(0))), CByte(playerUID), CByte((playerUID >> 8)), CByte((playerUID >> 16)), CByte((playerUID >> 24)), _
             CByte(corpseUID), CByte((corpseUID >> 8)), CByte((corpseUID >> 16)), CByte((corpseUID >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property Assign() As Boolean
            Get
                Return Me.m_assign
            End Get
        End Property

        Public ReadOnly Property CorpseUID() As UInteger
            Get
                Return Me.m_corpseUID
            End Get
        End Property

        Public ReadOnly Property PlayerUID() As UInteger
            Get
                Return Me.m_playerUID
            End Get
        End Property
    End Class




    Public Class PlayerInGame
        Inherits GSPacket
        ' Fields
        Protected charClass As CharacterClass
        Protected m_level As Short
        Protected m_name As String
        Protected m_partyID As Short
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 3)
            Me.charClass = data(7)
            Me.m_name = ByteConverter.GetNullString(data, 8)
            Me.m_level = BitConverter.ToInt16(data, 24)
            Me.m_partyID = BitConverter.ToInt16(data, 26)
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal charClass As CharacterClass, ByVal name As String, ByVal level As Short, ByVal partyID As Short)
            MyBase.New(Build(uid, charClass, name, level, partyID))
            Me.m_uid = uid
            Me.charClass = charClass
            Me.m_name = name
            Me.m_level = level
            Me.m_partyID = partyID
        End Sub

        Public Shared Function Build(ByVal uid As UInteger, ByVal charClass As CharacterClass, ByVal name As String, ByVal level As Short, ByVal partyID As Short) As Byte()
            If ((name Is Nothing) OrElse (name.Length = 0)) OrElse (name.Length > 16) Then
                Throw New ArgumentException("name")
            End If
            Dim buffer As Byte() = New Byte(33) {}
            buffer(0) = 91
            buffer(1) = 34
            buffer(3) = CByte(uid)
            buffer(4) = CByte((uid >> 8))
            buffer(5) = CByte((uid >> 16))
            buffer(6) = CByte((uid >> 24))
            buffer(24) = CByte(level)
            buffer(25) = CByte((level >> 8))
            buffer(26) = CByte(partyID)
            buffer(27) = CByte((partyID >> 8))
            For i As Integer = 0 To name.Length - 1
                buffer(8 + i) = AscW(name(i))
            Next
            Return buffer
        End Function

        ' Properties
        Public ReadOnly Property [Class]() As CharacterClass
            Get
                Return Me.charClass
            End Get
        End Property

        Public ReadOnly Property Level() As Short
            Get
                Return Me.m_level
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return Me.m_name
            End Get
        End Property

        Public ReadOnly Property PartyID() As Short
            Get
                Return Me.m_partyID
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property Unknown2() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 28)
            End Get
        End Property
    End Class




    Public Class PlayerInSight
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger)
            MyBase.New(Build(unitType, uid))
            Me.m_unitType = unitType
            Me.m_uid = uid
        End Sub

        Public Shared Function Build(ByVal unitType As UnitType, ByVal uid As UInteger) As Byte()
            Return New Byte() {118, CByte(unitType), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class


    Public Class PlayerKillCount
        Inherits GSPacket
        ' Fields
        Protected m_count As Short
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_count = BitConverter.ToInt16(data, 5)
        End Sub

        ' Properties
        Public ReadOnly Property KillCount() As Short
            Get
                Return Me.m_count
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class


    Public Class PlayerLeaveGame
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
        End Sub

        Public Sub New(ByVal uid As UInteger)
            MyBase.New(Build(uid))
            Me.m_uid = uid
        End Sub

        Public Shared Function Build(ByVal uid As UInteger) As Byte()
            Return New Byte() {92, CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class


    Public Class PlayerLifeManaChange
        Inherits GSPacket
        ' Fields
        Protected m_life As Integer
        Protected m_mana As Integer
        Protected m_stamina As Integer
        Protected m_unknown85b As Byte()
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Dim reader As New BitReader(data, 1)
            Me.m_life = reader.ReadInt32(15)
            Me.m_mana = reader.ReadInt32(15)
            Me.m_stamina = reader.ReadInt32(15)
            Me.m_x = reader.ReadInt32(16)
            Me.m_y = reader.ReadInt32(16)
            Me.m_unknown85b = reader.ReadByteArray()
        End Sub

        ' Properties
        Public ReadOnly Property Life() As Integer
            Get
                Return Me.m_life
            End Get
        End Property

        Public ReadOnly Property Mana() As Integer
            Get
                Return Me.m_mana
            End Get
        End Property

        Public ReadOnly Property Stamina() As Integer
            Get
                Return Me.m_stamina
            End Get
        End Property

        Public ReadOnly Property Unknown85b() As Byte()
            Get
                Return Me.m_unknown85b
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class


    Public Class PlayerMove
        Inherits GSPacket
        ' Fields
        Protected m_currentX As Integer
        Protected m_currentY As Integer
        Protected m_movementType As Byte
        Protected m_targetX As Integer
        Protected m_targetY As Integer
        Protected m_uid As UInteger
        Protected m_unitType As UnitType
        Protected m_unknown12 As Byte

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_movementType = data(6)
            Me.m_targetX = BitConverter.ToUInt16(data, 7)
            Me.m_targetY = BitConverter.ToUInt16(data, 9)
            Me.m_unknown12 = data(12)
            Me.m_currentX = BitConverter.ToUInt16(data, 12)
            Me.m_currentY = BitConverter.ToUInt16(data, 14)
        End Sub

        Public Sub New(ByVal type As D2Data.UnitType, ByVal uid As UInteger, ByVal movementtype As Byte, ByVal targetX As Integer, ByVal targetY As Integer, ByVal unknown12 As Byte, _
         ByVal currentX As Integer, ByVal currentY As Integer)
            MyBase.New(Build(type, uid, movementtype, targetX, targetY, unknown12, _
             currentX, currentY))
            Me.m_currentX = currentX
            Me.m_currentY = currentY
            Me.m_movementType = movementtype
            Me.m_targetX = targetX
            Me.m_targetY = targetY
            Me.m_uid = uid
            Me.m_unitType = type
            Me.m_unknown12 = unknown12
        End Sub

        Public Shared Function Build(ByVal type As D2Data.UnitType, ByVal uid As UInteger, ByVal movementtype As Byte, ByVal targetX As Integer, ByVal targetY As Integer, ByVal unknown12 As Byte, _
         ByVal currentX As Integer, ByVal currentY As Integer) As Byte()
            Dim buffer As Byte() = {15, CByte(type), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24)), _
             CByte(movementtype), CByte(targetX), CByte((targetX >> 8)), CByte(targetY), CByte((targetY >> 8)), unknown12, _
             CByte(currentX), CByte((currentX >> 8)), CByte(currentY), CByte((currentY >> 8))}
            Return buffer
        End Function

        ' Properties
        Public ReadOnly Property CurrentX() As Integer
            Get
                Return Me.m_currentX
            End Get
        End Property

        Public ReadOnly Property CurrentY() As Integer
            Get
                Return Me.m_currentY
            End Get
        End Property

        Public ReadOnly Property MovementType() As Byte
            Get
                Return Me.m_movementType
            End Get
        End Property

        Public ReadOnly Property TargetX() As Integer
            Get
                Return Me.m_targetX
            End Get
        End Property

        Public ReadOnly Property TargetY() As Integer
            Get
                Return Me.m_targetY
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property Unknown12() As Byte
            Get
                Return Me.m_unknown12
            End Get
        End Property
    End Class

    Public Class PlayerMoveToTarget
        Inherits GSPacket
        ' Fields
        Protected m_currentX As Integer
        Protected m_currentY As Integer
        Protected m_movementType As Byte
        Protected m_targetType As UnitType
        Protected m_targetUID As UInteger
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_movementType = data(6)
            Me.m_targetType = DirectCast(data(7), UnitType)
            Me.m_targetUID = BitConverter.ToUInt32(data, 8)
            Me.m_currentX = BitConverter.ToUInt16(data, 12)
            Me.m_currentY = BitConverter.ToUInt16(data, 14)
        End Sub

        ' Properties
        Public ReadOnly Property CurrentX() As Integer
            Get
                Return Me.m_currentX
            End Get
        End Property

        Public ReadOnly Property CurrentY() As Integer
            Get
                Return Me.m_currentY
            End Get
        End Property

        Public ReadOnly Property MovementType() As Byte
            Get
                Return Me.m_movementType
            End Get
        End Property

        Public ReadOnly Property TargetType() As UnitType
            Get
                Return Me.m_targetType
            End Get
        End Property

        Public ReadOnly Property TargetUID() As UInteger
            Get
                Return Me.m_targetUID
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class

    Public Class PlayerPartyRelationship
        Inherits GSPacket
        ' Fields
        Protected m_relationship As PartyRelationshipType
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_relationship = data(5)
        End Sub

        ' Properties
        Public ReadOnly Property Relationship() As PartyRelationshipType
            Get
                Return Me.m_relationship
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class

    Public Class PlayerReassign
        Inherits GSPacket
        ' Fields
        Protected m_reassign As Boolean
        Protected m_uid As UInteger
        Protected m_unitType As UnitType
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_x = BitConverter.ToUInt16(data, 6)
            Me.m_y = BitConverter.ToUInt16(data, 8)
            Me.m_reassign = data(10) <> 0
        End Sub

        Public Sub New(ByVal type As UnitType, ByVal uid As UInteger, ByVal x As Integer, ByVal y As Integer, ByVal reassign As Boolean)
            MyBase.New(Build(type, uid, x, y, reassign))
            Me.m_unitType = type
            Me.m_uid = uid
        End Sub

        Public Shared Function Build(ByVal type As UnitType, ByVal uid As UInteger, ByVal x As Integer, ByVal y As Integer, ByVal reassign As Boolean) As Byte()
            Return New Byte() {21, CByte(type), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24)), _
             CByte(x), CByte((y >> 8)), CByte(y), CByte((y >> 8)), (IIf(reassign, CByte(1), CByte(0)))}
        End Function

        ' Properties
        Public ReadOnly Property Reassign() As Boolean
            Get
                Return Me.m_reassign
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Class PlayerRelationship
        Inherits GSPacket
        ' Fields
        Protected m_objectUID As UInteger
        Protected m_relations As PlayerRelationshipType
        Protected m_subjectUID As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_subjectUID = BitConverter.ToUInt32(data, 1)
            Me.m_objectUID = BitConverter.ToUInt32(data, 5)
            Me.m_relations = BitConverter.ToUInt16(data, 9)
        End Sub

        ' Properties
        Public ReadOnly Property ObjectUID() As UInteger
            Get
                Return Me.m_objectUID
            End Get
        End Property

        Public ReadOnly Property Relations() As PlayerRelationshipType
            Get
                Return Me.m_relations
            End Get
        End Property

        Public ReadOnly Property SubjectUID() As UInteger
            Get
                Return Me.m_subjectUID
            End Get
        End Property
    End Class

    Public Class PlayerStop
        Inherits GSPacket
        ' Fields
        Protected m_life As Byte
        Protected m_uid As UInteger
        Protected m_unitType As UnitType
        Protected m_unknown1 As Byte
        Protected m_unknown2 As Byte
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_unknown1 = data(6)
            Me.m_x = BitConverter.ToUInt16(data, 7)
            Me.m_y = BitConverter.ToUInt16(data, 9)
            Me.m_unknown2 = data(11)
            Me.m_life = data(12)
        End Sub

        ' Properties
        Public ReadOnly Property Life() As Byte
            Get
                Return Me.m_life
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property Unknown1() As Byte
            Get
                Return Me.m_unknown1
            End Get
        End Property

        Public ReadOnly Property Unknown2() As Byte
            Get
                Return Me.m_unknown2
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Class PlaySound
        Inherits GSPacket
        ' Fields
        Protected m_sound As GameSound
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_sound = BitConverter.ToUInt16(data, 6)
        End Sub

        Public Sub New(ByVal unitType As UnitType, ByVal uid As UInteger, ByVal sound As GameSound)
            MyBase.New(Build(unitType, uid, sound))
            Me.m_unitType = unitType
            Me.m_uid = uid
            Me.m_sound = sound
        End Sub

        Public Shared Function Build(ByVal unitType As UnitType, ByVal uid As UInteger, ByVal sound As GameSound) As Byte()
            Return New Byte() {44, CByte(unitType), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24)), _
             CByte(sound), CByte((CUShort(sound) >> 8))}
        End Function

        ' Properties
        Public ReadOnly Property Sound() As GameSound
            Get
                Return Me.m_sound
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class

    Public Class Pong
        Inherits GSPacket
        ' Methods
        Public Sub New()
            MyBase.New(Build())
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Shared Function Build() As Byte()
            Return New Byte() {143}
        End Function
    End Class

    Public Class PortalInfo
        Inherits GSPacket
        ' Fields
        Protected m_destination As AreaLevel
        Protected m_state As TownPortalState
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_state = data(1)
            Me.m_destination = data(2)
            Me.m_uid = BitConverter.ToUInt32(data, 3)
        End Sub

        ' Properties
        Public ReadOnly Property Destination() As AreaLevel
            Get
                Return Me.m_destination
            End Get
        End Property

        Public ReadOnly Property State() As TownPortalState
            Get
                Return Me.m_state
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class

    Public Class PortalOwnership
        Inherits GSPacket
        ' Fields
        Protected m_ownerName As String
        Protected m_ownerUID As UInteger
        Protected m_portalLocalUID As UInteger
        Protected m_portalRemoteUID As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_ownerUID = BitConverter.ToUInt32(data, 1)
            Me.m_ownerName = ByteConverter.GetNullString(data, 5, 16)
            Me.m_portalLocalUID = BitConverter.ToUInt32(data, 21)
            Me.m_portalRemoteUID = BitConverter.ToUInt32(data, 25)
        End Sub

        ' Properties
        Public ReadOnly Property OwnerName() As String
            Get
                Return Me.m_ownerName
            End Get
        End Property

        Public ReadOnly Property OwnerUID() As UInteger
            Get
                Return Me.m_ownerUID
            End Get
        End Property

        Public ReadOnly Property PortalLocalUID() As UInteger
            Get
                Return Me.m_portalLocalUID
            End Get
        End Property

        Public ReadOnly Property PortalRemoteUID() As UInteger
            Get
                Return Me.m_portalRemoteUID
            End Get
        End Property
    End Class

    Public Enum QuestInfoUpdateType
        NPCInteract = 1
        QuestLog = 6
    End Enum




    Public Class QuestItemState
        Inherits GSPacket
        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        ' Properties
        Public ReadOnly Property Unknown1() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 1)
            End Get
        End Property
    End Class




    Public Class Relator1
        Inherits GSPacket
        ' Fields
        Protected m_param1 As UShort
        Protected m_param2 As UInteger
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_param1 = BitConverter.ToUInt16(data, 1)
            Me.m_uid = BitConverter.ToUInt32(data, 3)
            Me.m_param2 = BitConverter.ToUInt32(data, 7)
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal param1 As UShort, ByVal param2 As UInteger)
            MyBase.New(Build(uid, param1, param2))
            Me.m_param1 = param1
            Me.m_uid = uid
            Me.m_param2 = param2
        End Sub

        Public Shared Function Build(ByVal uid As UInteger, ByVal param1 As UShort, ByVal param2 As UInteger) As Byte()
            Return New Byte() {71, CByte(param1), CByte((param1 >> 8)), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), _
             CByte((uid >> 24)), CByte(param2), CByte((param2 >> 8)), CByte((param2 >> 16)), CByte((param2 >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property Param1() As UShort
            Get
                Return Me.m_param1
            End Get
        End Property

        Public ReadOnly Property Param2() As UInteger
            Get
                Return Me.m_param2
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class




    Public Class Relator2
        Inherits GSPacket
        ' Fields
        Protected m_param1 As UShort
        Protected m_param2 As UInteger
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_param1 = BitConverter.ToUInt16(data, 1)
            Me.m_uid = BitConverter.ToUInt32(data, 3)
            Me.m_param2 = BitConverter.ToUInt32(data, 7)
        End Sub

        Public Sub New(ByVal uid As UInteger, ByVal param1 As UShort, ByVal param2 As UInteger)
            MyBase.New(Build(uid, param1, param2))
            Me.m_param1 = param1
            Me.m_uid = uid
            Me.m_param2 = param2
        End Sub

        Public Shared Function Build(ByVal uid As UInteger, ByVal param1 As UShort, ByVal param2 As UInteger) As Byte()
            Return New Byte() {72, CByte(param1), CByte((param1 >> 8)), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), _
             CByte((uid >> 24)), CByte(param2), CByte((param2 >> 8)), CByte((param2 >> 16)), CByte((param2 >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property Param1() As UShort
            Get
                Return Me.m_param1
            End Get
        End Property

        Public ReadOnly Property Param2() As UInteger
            Get
                Return Me.m_param2
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class




    Public Class RemoveGroundUnit
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
        End Sub

        Public Sub New(ByVal type As UnitType, ByVal uid As UInteger)
            MyBase.New(Build(type, uid))
            Me.m_unitType = type
            Me.m_uid = uid
        End Sub

        Public Shared Function Build(ByVal type As UnitType, ByVal uid As UInteger) As Byte()
            Return New Byte() {10, CByte(type), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class




    Public Class ReportKill
        Inherits GSPacket
        ' Fields
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
        End Sub

        Public Sub New(ByVal type As UnitType, ByVal uid As UInteger)
            MyBase.New(Build(type, uid))
            Me.m_unitType = type
            Me.m_uid = uid
        End Sub

        Public Shared Function Build(ByVal type As UnitType, ByVal uid As UInteger) As Byte()
            Return New Byte() {17, CByte(type), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property
    End Class




    Public Class RequestLogonInfo
        Inherits GSPacket
        ' Fields
        Protected m_protocolVersion As Byte

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_protocolVersion = data(1)
        End Sub

        ' Properties
        Public ReadOnly Property ProtocolVersion() As Byte
            Get
                Return Me.m_protocolVersion
            End Get
        End Property
    End Class


    Public Class SetGameObjectMode
        Inherits GSPacket
        ' Fields
        Protected m_canChangeBack As Boolean
        Protected m_mode As GameObjectMode
        Protected m_uid As UInteger
        Protected m_unitType As UnitType
        Protected m_unknown6 As Byte

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_unknown6 = data(6)
            Me.m_canChangeBack = BitConverter.ToBoolean(data, 7)
            Me.m_mode = BitConverter.ToUInt32(data, 8)
        End Sub

        ' Properties
        Public ReadOnly Property CanChangeBack() As Boolean
            Get
                Return Me.m_canChangeBack
            End Get
        End Property

        Public ReadOnly Property Mode() As GameObjectMode
            Get
                Return Me.m_mode
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property Unknown6() As Byte
            Get
                Return Me.m_unknown6
            End Get
        End Property
    End Class


    Public Class SetItemState
        Inherits GSPacket
        ' Fields
        Protected m_itemUID As UInteger
        Protected m_ownerType As UnitType
        Protected m_ownerUID As UInteger
        Protected m_state As ItemStateType
        Protected m_state2 As ItemStateType
        Protected m_unknown10 As Byte
        Protected m_unknown17 As Byte

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_ownerType = DirectCast(data(1), UnitType)
            Me.m_ownerUID = BitConverter.ToUInt32(data, 2)
            Me.m_itemUID = BitConverter.ToUInt32(data, 6)
            Me.m_unknown10 = data(10)
            Me.m_state = BitConverter.ToUInt32(data, 11)
            Me.m_state2 = BitConverter.ToUInt16(data, 15)
            Me.m_unknown17 = data(17)
        End Sub

        Public Sub New(ByVal ownerType As UnitType, ByVal ownerUID As UInteger, ByVal itemUID As UInteger, ByVal state As ItemStateType)
            MyBase.New(Build(ownerType, ownerUID, itemUID, state))
            Me.m_ownerType = ownerType
            Me.m_ownerUID = ownerUID
            Me.m_itemUID = itemUID
            Me.m_state = state
            Me.m_state2 = state
        End Sub

        Public Shared Function Build(ByVal ownerType As UnitType, ByVal ownerUID As UInteger, ByVal itemUID As UInteger, ByVal state As ItemStateType) As Byte()
            Dim buffer As Byte() = New Byte(17) {}
            buffer(0) = 125
            buffer(1) = CByte(ownerType)
            buffer(2) = CByte(ownerUID)
            buffer(3) = CByte((ownerUID >> 8))
            buffer(4) = CByte((ownerUID >> 16))
            buffer(5) = CByte((ownerUID >> 24))
            buffer(6) = CByte(itemUID)
            buffer(7) = CByte((itemUID >> 8))
            buffer(8) = CByte((itemUID >> 16))
            buffer(9) = CByte((itemUID >> 24))
            buffer(11) = CByte(state)
            buffer(12) = CByte((CInt(state) >> 8))
            buffer(13) = CByte((CInt(state) >> 16))
            buffer(14) = CByte((CInt(state) >> 24))
            buffer(15) = CByte(state)
            buffer(16) = CByte((CUShort(state) >> 8))
            Return buffer
        End Function

        ' Properties
        Public ReadOnly Property ItemUID() As UInteger
            Get
                Return Me.m_itemUID
            End Get
        End Property

        Public ReadOnly Property OwnerType() As UnitType
            Get
                Return Me.m_ownerType
            End Get
        End Property

        Public ReadOnly Property OwnerUID() As UInteger
            Get
                Return Me.m_ownerUID
            End Get
        End Property

        Public ReadOnly Property State() As ItemStateType
            Get
                Return Me.m_state
            End Get
        End Property

        Public ReadOnly Property State2() As ItemStateType
            Get
                Return Me.m_state2
            End Get
        End Property

        Public ReadOnly Property Unknown10() As Byte
            Get
                Return Me.m_unknown10
            End Get
        End Property

        Public ReadOnly Property Unknown17() As Byte
            Get
                Return Me.m_unknown17
            End Get
        End Property
    End Class


    Public Class SetNPCMode
        Inherits GSPacket
        ' Fields
        Protected m_life As Byte
        Protected m_mode As NPCMode
        Protected m_uid As UInteger
        Protected m_unknown11 As Byte
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_uid = BitConverter.ToUInt32(data, 1)
            Me.m_mode = data(5)
            Me.m_x = BitConverter.ToUInt16(data, 6)
            Me.m_y = BitConverter.ToUInt16(data, 8)
            Me.m_life = data(10)
            Me.m_unknown11 = data(11)
        End Sub

        ' Properties
        Public ReadOnly Property Life() As Byte
            Get
                Return Me.m_life
            End Get
        End Property

        Public ReadOnly Property Mode() As NPCMode
            Get
                Return Me.m_mode
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property Unknown11() As Byte
            Get
                Return Me.m_unknown11
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class


    Public Class SetState
        Inherits GSPacket
        ' Fields
        Protected m_state As BaseState
        Protected m_stats As List(Of StatBase)
        Protected m_uid As UInteger
        Protected m_unitType As UnitType
        Protected m_unknownEnd As Byte()

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Dim num As Integer
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_state = BaseState.[Get](data(7))
            Me.m_stats = New List(Of StatBase)()
            Dim reader As New BitReader(data, 8)
Label_003E:
            num = reader.ReadInt32(9)
            If num <> 511 Then
                Dim stat As BaseStat = BaseStat.[Get](num)
                Dim val As Integer = reader.ReadInt32(stat.SendBits)
                If stat.SendParamBits > 0 Then
                    Dim param As Integer = reader.ReadInt32(stat.SendParamBits)
                    If stat.Signed Then
                        Me.m_stats.Add(New SignedStatParam(stat, val, param))
                    Else
                        Me.m_stats.Add(New UnsignedStatParam(stat, CInt(val), CInt(param)))
                    End If
                ElseIf stat.Signed Then
                    Me.m_stats.Add(New SignedStat(stat, val))
                Else
                    Me.m_stats.Add(New UnsignedStat(stat, CInt(val)))
                End If
                GoTo Label_003E
            End If
            Me.m_unknownEnd = reader.ReadByteArray()
        End Sub

        ' Properties
        Public ReadOnly Property State() As BaseState
            Get
                Return Me.m_state
            End Get
        End Property

        Public ReadOnly Property Stats() As List(Of StatBase)
            Get
                Return Me.m_stats
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property UnknownEnd() As Byte()
            Get
                Return Me.m_unknownEnd
            End Get
        End Property
    End Class


    Public Class SkillsLog
        Inherits GSPacket
        ' Fields
        Protected m_skills As BaseSkillLevel()
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_skills = New BaseSkillLevel(data(1) - 1) {}
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            For i As Integer = 0 To data(1) - 1
                Me.m_skills(i) = New BaseSkillLevel(BitConverter.ToUInt16(data, 6 + (i * 3)), data(8 + (i * 3)))
            Next
        End Sub

        ' Properties
        Public ReadOnly Property Skills() As BaseSkillLevel()
            Get
                Return Me.m_skills
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class


    Public Class SmallGoldAdd
        Inherits GSPacket
        ' Fields
        Protected m_amount As Byte

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_amount = data(1)
        End Sub

        Public Sub New(ByVal amount As Byte)
            MyBase.New(Build(amount))
            Me.m_amount = amount
        End Sub

        Public Shared Function Build(ByVal amount As Byte) As Byte()
            Return New Byte() {25, amount}
        End Function

        ' Properties
        Public ReadOnly Property Amount() As Byte
            Get
                Return Me.m_amount
            End Get
        End Property
    End Class


    Public Enum SpecialItemType
        TomeOrScroll = 4
    End Enum


    Public Enum StackableItemClickType1 As SByte
        ActionClick = -1
        NormalClick = 0
    End Enum


    Public Enum StackableItemClickType2 As Short
        ActionClick = -1
        NormalClick = 218
    End Enum


    Public Class SummonAction
        Inherits GSPacket
        ' Fields
        Protected m_actionType As SummonActionType
        Protected m_petType As Integer
        Protected m_petUID As UInteger
        Protected m_playerUID As UInteger
        Protected m_skillTree As Byte

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_actionType = data(1)
            Me.m_skillTree = data(2)
            Me.m_petType = BitConverter.ToUInt16(data, 3)
            Me.m_playerUID = BitConverter.ToUInt32(data, 5)
            Me.m_petUID = BitConverter.ToUInt32(data, 9)
        End Sub

        ' Properties
        Public ReadOnly Property ActionType() As SummonActionType
            Get
                Return Me.m_actionType
            End Get
        End Property

        Public ReadOnly Property PetType() As Integer
            Get
                Return Me.m_petType
            End Get
        End Property

        Public ReadOnly Property PetUID() As UInteger
            Get
                Return Me.m_petUID
            End Get
        End Property

        Public ReadOnly Property PlayerUID() As UInteger
            Get
                Return Me.m_playerUID
            End Get
        End Property

        Public ReadOnly Property SkillTree() As Byte
            Get
                Return Me.m_skillTree
            End Get
        End Property
    End Class


    Public Enum SummonActionType
        UnsummonedOrLostSight
        SummonedOrReassigned
    End Enum


    Public Class SwitchWeaponSet
        Inherits GSPacket
        ' Methods
        Public Sub New()
            MyBase.New(Build())
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Shared Function Build() As Byte()
            Return New Byte() {151}
        End Function
    End Class


    <Flags()> _
    Public Enum TownPortalState
        IsOtherSide = 2
        None = 0
        Unknown1 = 1
        Used = 4
    End Enum


    Public Class TransactionComplete
        Inherits GSPacket
        ' Fields
        Protected m_goldLeft As UInteger
        Protected m_type As TransactionType
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_type = data(1)
            Me.m_uid = BitConverter.ToUInt32(data, 7)
            Me.m_goldLeft = BitConverter.ToUInt32(data, 11)
        End Sub

        ' Properties
        Public ReadOnly Property GoldLeft() As UInteger
            Get
                Return Me.m_goldLeft
            End Get
        End Property

        Public ReadOnly Property Type() As TransactionType
            Get
                Return Me.m_type
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property Unknown2() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 2, 5)
            End Get
        End Property
    End Class


    Public Enum TransactionType
        Buy = 4
        Hire = 0
        Repair = 1
        Sell = 3
        ToStack = 5
    End Enum


    Public Class UnitUseSkill
        Inherits GSPacket
        ' Fields
        Protected m_skill As SkillType
        Protected m_uid As UInteger
        Protected m_unitType As UnitType
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            If Me.m_unitType <> UnitType.GameObject Then
                Me.m_skill = DirectCast(CUShort(BitConverter.ToUInt32(data, 6)), SkillType)
                Me.m_x = BitConverter.ToUInt16(data, 11)
                Me.m_y = BitConverter.ToUInt16(data, 13)
            End If
        End Sub

        ' Properties
        Public ReadOnly Property Skill() As SkillType
            Get
                Return Me.m_skill
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property Unknown10() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 10, 1)
            End Get
        End Property

        Public ReadOnly Property Unknown15() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 15, 2)
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class


    Public Class UnitUseSkillOnTarget
        Inherits GSPacket
        ' Fields
        Protected m_skill As SkillType
        Protected m_targetUID As UInteger
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(data(1), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_skill = DirectCast(BitConverter.ToUInt16(data, 6), SkillType)
            Me.m_targetUID = BitConverter.ToUInt32(data, 10)
        End Sub

        ' Properties
        Public ReadOnly Property Skill() As SkillType
            Get
                Return Me.m_skill
            End Get
        End Property

        Public ReadOnly Property TargetUID() As UInteger
            Get
                Return Me.m_targetUID
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property Unknown14() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 14, 2)
            End Get
        End Property

        Public ReadOnly Property Unknown8() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 8, 2)
            End Get
        End Property
    End Class


    Public Class UnloadDone
        Inherits GSPacket
        ' Methods
        Public Sub New()
            MyBase.New(Build())
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        Public Shared Function Build() As Byte()
            Return New Byte() {5}
        End Function
    End Class

    Public Class UpdateGameQuestLog
        Inherits GSPacket
        ' Fields
        Protected m_quests As GameQuestInfo()

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_quests = New GameQuestInfo(40) {}
            For i As Integer = 0 To 40
                Me.m_quests(i) = New GameQuestInfo(DirectCast(i, QuestType), BitConverter.ToUInt16(data, 1 + (i * 2)))
            Next
        End Sub

        ' Properties
        Public ReadOnly Property Quests() As GameQuestInfo()
            Get
                Return Me.m_quests
            End Get
        End Property

        Public ReadOnly Property Unknown82() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 83)
            End Get
        End Property
    End Class

    Public Class UpdateItemStats
        Inherits GSPacket
        ' Fields
        Public Shared ReadOnly NULL_Int32 As Integer = -1
        Protected offset As Long
        Protected m_stats As List(Of StatBase)
        Protected m_uid As UInteger
        Protected m_unknown60b As Integer
        Protected m_unknown61b As Integer
        Protected m_unknown78b As Integer
        Protected m_unknown8b As Integer
        Protected m_unknownEnd As Byte()

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_stats = New List(Of StatBase)()
            Me.m_unknown61b = -1
            Me.m_unknown78b = -1
            Dim reader As New BitReader(data, 1)
            Me.m_unknown8b = reader.ReadInt32(10)
            Me.m_uid = reader.ReadUInt32()
            While reader.ReadBoolean(1)
                Dim stat As BaseStat = BaseStat.[Get](reader.ReadInt32(9))
                Me.m_unknown60b = reader.ReadInt32(1)
                If stat.Type = StatType.ChargedSkill Then
                    Me.m_unknown61b = reader.ReadInt32(1)
                    Dim charges As Integer = reader.ReadInt32(8)
                    Dim maxCharges As Integer = reader.ReadInt32(8)
                    Me.m_unknown78b = reader.ReadInt32(1)
                    Dim level As Integer = reader.ReadInt32(6)
                    Dim skill As Integer = reader.ReadInt32(10)
                    Me.m_stats.Add(New ChargedSkillStat(stat, level, skill, charges, maxCharges))
                Else
                    If stat.Signed Then
                        Me.m_stats.Add(New SignedStat(stat, reader.ReadInt32(stat.SendBits)))
                        Continue While
                    End If
                    Me.m_stats.Add(New UnsignedStat(stat, reader.ReadUInt32(stat.SendBits)))
                End If
            End While
            Me.offset = reader.Position
            Me.m_unknownEnd = reader.ReadByteArray()
        End Sub

        ' Properties
        Public ReadOnly Property Stats() As List(Of StatBase)
            Get
                Return Me.m_stats
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property Unknown60b() As Integer
            Get
                Return Me.m_unknown60b
            End Get
        End Property

        Public ReadOnly Property Unknown61b() As Integer
            Get
                Return Me.m_unknown61b
            End Get
        End Property

        Public ReadOnly Property Unknown78b() As Integer
            Get
                Return Me.m_unknown78b
            End Get
        End Property

        Public ReadOnly Property Unknown8b() As Integer
            Get
                Return Me.m_unknown8b
            End Get
        End Property

        Public ReadOnly Property UnknownEnd() As Byte()
            Get
                Return Me.m_unknownEnd
            End Get
        End Property
    End Class


    Public Class UpdateItemUI
        Inherits GSPacket
        ' Fields
        Protected m_action As ItemUIAction

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_action = DirectCast(data(1), ItemUIAction)
        End Sub

        Public Sub New(ByVal action As ItemUIAction)
            MyBase.New(Build(action))
            Me.m_action = action
        End Sub

        Public Shared Function Build(ByVal action As ItemUIAction) As Byte()
            Return New Byte() {119, CByte(action)}
        End Function

        ' Properties
        Public ReadOnly Property Action() As ItemUIAction
            Get
                Return Me.m_action
            End Get
        End Property
    End Class


    Public Class UpdatePlayerItemSkill
        Inherits GSPacket
        ' Fields
        Protected m_playerUID As UInteger
        Protected m_quantity As Integer
        Protected m_skill As SkillType
        Protected m_unknown1 As UShort
        Protected m_unknown10 As UShort

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unknown1 = BitConverter.ToUInt16(data, 1)
            Me.m_playerUID = BitConverter.ToUInt32(data, 3)
            Me.m_skill = DirectCast(BitConverter.ToUInt16(data, 7), SkillType)
            Me.m_quantity = data(9)
            Me.m_unknown10 = BitConverter.ToUInt16(data, 10)
        End Sub

        ' Properties
        Public ReadOnly Property PlayerUID() As UInteger
            Get
                Return Me.m_playerUID
            End Get
        End Property

        Public ReadOnly Property Quantity() As Integer
            Get
                Return Me.m_quantity
            End Get
        End Property

        Public ReadOnly Property Skill() As SkillType
            Get
                Return Me.m_skill
            End Get
        End Property

        Public ReadOnly Property Unknown1() As UShort
            Get
                Return Me.m_unknown1
            End Get
        End Property

        Public ReadOnly Property Unknown10() As UShort
            Get
                Return Me.m_unknown10
            End Get
        End Property
    End Class


    Public Class UpdateQuestInfo
        Inherits GSPacket
        ' Fields
        Public Shared ReadOnly NULL_UInt32 As UInteger
        Protected m_quests As QuestInfo()
        Protected m_type As QuestInfoUpdateType
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_type = data(1)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_quests = New QuestInfo(40) {}
            For i As Integer = 0 To 40
                Me.m_quests(i) = New QuestInfo(DirectCast(i, QuestType), data(6 + (i * 2)), data(7 + (i * 2)))
            Next
        End Sub

        ' Properties
        Public ReadOnly Property Quests() As QuestInfo()
            Get
                Return Me.m_quests
            End Get
        End Property

        Public ReadOnly Property Type() As QuestInfoUpdateType
            Get
                Return Me.m_type
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property Unknown88() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 88)
            End Get
        End Property
    End Class

    Public Class UpdateQuestLog
        Inherits GSPacket
        ' Fields
        Protected m_quests As QuestLog()

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_quests = New QuestLog(40) {}
            For i As Integer = 0 To 40
                Me.m_quests(i) = New QuestLog(DirectCast(i, QuestType), data(i + 1))
            Next
        End Sub

        ' Properties
        Public ReadOnly Property Quests() As QuestLog()
            Get
                Return Me.m_quests
            End Get
        End Property
    End Class

    Public Class UpdateSkill
        Inherits GSPacket
        ' Fields
        Protected m_baseLevel As Integer
        Protected m_bonus As Integer
        Protected m_skill As SkillType
        Protected m_uid As UInteger
        Protected m_unitType As UnitType

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_unitType = DirectCast(CByte(BitConverter.ToUInt16(data, 1)), UnitType)
            Me.m_uid = BitConverter.ToUInt32(data, 3)
            Me.m_skill = DirectCast(BitConverter.ToUInt16(data, 7), SkillType)
            Me.m_baseLevel = data(9)
            Me.m_bonus = data(10)
        End Sub

        ' Properties
        Public ReadOnly Property BaseLevel() As Integer
            Get
                Return Me.m_baseLevel
            End Get
        End Property

        Public ReadOnly Property Bonus() As Integer
            Get
                Return Me.m_bonus
            End Get
        End Property

        Public ReadOnly Property Skill() As SkillType
            Get
                Return Me.m_skill
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property

        Public ReadOnly Property UnitType() As UnitType
            Get
                Return Me.m_unitType
            End Get
        End Property

        Public ReadOnly Property Unknown11() As String
            Get
                Return ByteConverter.ToHexString(MyBase.GetData, 11, 1)
            End Get
        End Property
    End Class

    Public Class UseSpecialItem
        Inherits GSPacket
        ' Fields
        Protected m_action As SpecialItemType
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_action = data(1)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
        End Sub

        Public Sub New(ByVal action As SpecialItemType, ByVal uid As UInteger)
            MyBase.New(Build(action, uid))
            Me.m_action = action
            Me.m_uid = uid
        End Sub

        Public Shared Function Build(ByVal action As SpecialItemType, ByVal uid As UInteger) As Byte()
            Return New Byte() {124, CByte(action), CByte(uid), CByte((uid >> 8)), CByte((uid >> 16)), CByte((uid >> 24))}
        End Function

        ' Properties
        Public ReadOnly Property Action() As SpecialItemType
            Get
                Return Me.m_action
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class

    Public Class UseStackableItem
        Inherits GSPacket
        ' Fields
        Protected m_type1 As StackableItemClickType1
        Protected m_type2 As StackableItemClickType2
        Protected m_uid As UInteger

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_type1 = DirectCast(CSByte(data(1)), StackableItemClickType1)
            Me.m_uid = BitConverter.ToUInt32(data, 2)
            Me.m_type2 = DirectCast(BitConverter.ToInt16(data, 6), StackableItemClickType2)
        End Sub

        ' Properties
        Public ReadOnly Property Type1() As StackableItemClickType1
            Get
                Return Me.m_type1
            End Get
        End Property

        Public ReadOnly Property Type2() As StackableItemClickType2
            Get
                Return Me.m_type2
            End Get
        End Property

        Public ReadOnly Property UID() As UInteger
            Get
                Return Me.m_uid
            End Get
        End Property
    End Class

    Public Class WalkVerify
        Inherits GSPacket
        ' Fields
        Protected m_stamina As Integer
        Protected m_state As Integer
        Protected m_x As Integer
        Protected m_y As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Dim reader As New BitReader(data, 1)
            Me.m_stamina = reader.ReadInt32(15)
            Me.m_x = reader.ReadInt32(16)
            Me.m_y = reader.ReadInt32(16)
            Me.m_state = reader.ReadInt32(17)
        End Sub

        ' Properties
        Public ReadOnly Property Stamina() As Integer
            Get
                Return Me.m_stamina
            End Get
        End Property

        Public ReadOnly Property State() As Integer
            Get
                Return Me.m_state
            End Get
        End Property

        Public ReadOnly Property X() As Integer
            Get
                Return Me.m_x
            End Get
        End Property

        Public ReadOnly Property Y() As Integer
            Get
                Return Me.m_y
            End Get
        End Property
    End Class

    Public Class WardenCheck
        Inherits GSPacket
        ' Fields
        Protected m_dataLength As Integer

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_dataLength = BitConverter.ToUInt16(data, 1)
        End Sub

        ' Properties
        Public ReadOnly Property DataLength() As Integer
            Get
                Return Me.m_dataLength
            End Get
        End Property

        Public ReadOnly Property WardenData() As Byte()
            Get
                Dim destinationArray As Byte() = New Byte(Me.m_dataLength - 1) {}
                Array.Copy(MyBase.GetData, 3, destinationArray, 0, Me.m_dataLength)
                Return destinationArray
            End Get
        End Property
    End Class

    Public Class WordToExperience
        Inherits GainExperience
        ' Fields
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            MyBase.m_experience = BitConverter.ToUInt16(data, 1)
        End Sub
    End Class

    Public Class WorldItemAction
        Inherits ItemAction
        ' Fields
        Public Shared ReadOnly NULL_Int32 As Integer = -1
        Public Shared ReadOnly WRAPPED As Boolean = True

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub
    End Class

End Namespace
