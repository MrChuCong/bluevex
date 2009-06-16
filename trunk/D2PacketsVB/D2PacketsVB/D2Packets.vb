Imports System
Imports ETUtils
Imports D2Data
Imports System.Runtime.InteropServices

Namespace D2Packets

    ''' <summary>
    ''' Base class for Diablo II Packets
    ''' </summary>
    Public MustInherit Class D2Packet
        Inherits DataBuffer

        Public MustOverride ReadOnly Property Data() As Byte()

    End Class

    Public Class StatString

        Public Shared Sub ParseD2StatString(ByVal data As Byte(), ByVal index As Integer, ByRef clientVersion As Integer, ByRef characterType As BattleNetCharacter, ByRef characterLevel As Integer, ByRef characterFlags As CharacterFlags, _
         ByRef characterAct As Integer, ByRef characterTitle As CharacterTitle)

            Dim num2 As Integer
            clientVersion = data(index)
            Dim num As Integer = data(index + 13) - 1
            If (num < 0) OrElse (num > 6) Then
                characterType = BattleNetCharacter.Unknown
            Else
                characterType = DirectCast(num, BattleNetCharacter)
                If CharactersInfo.Genders(CInt(characterType)) Then
                    characterFlags = characterFlags Or characterFlags.Female
                End If
            End If
            characterLevel = data(index + 25)
            characterFlags = characterFlags Or data(index + 26)
            Dim num3 As Integer = (data(index + 27) And 62) >> 1
            If (characterFlags And characterFlags.Expansion) = characterFlags.Expansion Then
                num2 = num3 / 5
                num3 = num3 Mod 5
            Else
                num2 = num3 / 4
                num3 = num3 Mod 4
            End If
            If num2 = 3 Then
                characterAct = 666
            Else
                characterAct = num3 + 1
            End If
            If (characterFlags And characterFlags.Hardcore) = characterFlags.Hardcore Then
                num2 = num2 Or 4
            End If
            If (characterFlags And characterFlags.Expansion) = characterFlags.Expansion Then
                num2 = num2 Or 32
            End If
            If (characterFlags And characterFlags.Female) = characterFlags.Female Then
                num2 = num2 Or 256
            End If
            characterTitle = DirectCast(num2, CharacterTitle)
        End Sub
    End Class

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure RealmInfo

        Public ReadOnly Unknown As UInteger
        Public ReadOnly Name As String
        Public ReadOnly Description As String

        Public Sub New(ByVal data As Byte(), ByVal offset As Integer)

            Me.Unknown = BitConverter.ToUInt32(data, offset)
            Me.Name = ByteConverter.GetNullString(data, (offset + 4))
            Me.Description = ByteConverter.GetNullString(data, ((offset + 5) + Me.Name.Length))

        End Sub


        Public Overloads Overrides Function ToString() As String
            Return Me.Name
        End Function
    End Structure

    Public Class CharacterInfo
        ' Fields
        Public Act As Integer
        Public [Class] As BattleNetCharacter
        Public ClientVersion As Integer
        Public Expires As DateTime
        Public Flags As CharacterFlags
        Public Level As Integer
        Public Name As String
        Public Title As CharacterTitle

        ' Methods
        Public Overloads Overrides Function ToString() As String
            Return StringUtils.ToFormatedInfoString(Me, False, ": ", ", ")
        End Function
    End Class

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure CDKeyInfo
        Public ReadOnly Length As UInt32
        Public ReadOnly ProductValue As UInt32
        Public ReadOnly PublicValue As UInt32
        Public ReadOnly Unknown As UInt32
        Public ReadOnly Hash As Byte()
        Public Sub New(ByVal data As Byte(), ByVal offset As Integer)
            Me.Length = BitConverter.ToUInt32(data, offset)
            Me.ProductValue = BitConverter.ToUInt32(data, (offset + 4))
            Me.PublicValue = BitConverter.ToUInt32(data, (offset + 8))
            Me.Unknown = BitConverter.ToUInt32(data, (offset + 12))
            Me.Hash = New Byte(20 - 1) {}
            Array.Copy(data, (offset + &H10), Me.Hash, 0, 20)
        End Sub
    End Structure

    Public Class GameQuestInfo
        ' Fields
        Public State As GameQuestState
        Public Type As QuestType

        ' Methods
        Public Sub New(ByVal type As QuestType, ByVal state As GameQuestState)
            Me.Type = type
            Me.State = state
        End Sub

        Public Overloads Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Me.Type, Me.State)
        End Function
    End Class

    Public Class QuestLog
        ' Fields
        Public State As Integer
        Public Type As QuestType

        ' Methods
        Public Sub New(ByVal type As QuestType, ByVal state As Integer)
            Me.Type = type
            Me.State = state
        End Sub

        Public Overloads Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Me.Type, Me.State.ToString("x2"))
        End Function
    End Class

    Public Class QuestInfo
        ' Fields
        Public Standing As QuestStanding
        Public State As QuestState
        Public Type As QuestType

        ' Methods
        Public Sub New(ByVal type As QuestType, ByVal state As QuestState, ByVal standing As QuestStanding)
            Me.Type = type
            Me.State = state
            Me.Standing = standing
        End Sub

        Public Overloads Overrides Function ToString() As String
            Return String.Format("{0}: {1}. {2}", Me.Type, Me.State, Me.Standing)
        End Function
    End Class

End Namespace

