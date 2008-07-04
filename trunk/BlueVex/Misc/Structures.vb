Namespace Structures

    Public Class Dicto(Of TKey, TValue)

        Private m_dictionary As New Dictionary(Of TKey, TValue)
        Private m_keysInOrder As New List(Of TKey)

        Default Public Overloads Property Item(ByVal key As TKey) As TValue
            Get
                Return m_dictionary(key)
            End Get
            Set(ByVal value As TValue)
                m_dictionary(key) = value
            End Set
        End Property

        Default Public Overloads Property Item(ByVal Index As Integer) As TValue
            Get
                Return m_dictionary(m_keysInOrder(Index))
            End Get
            Set(ByVal value As TValue)
                m_dictionary(m_keysInOrder(Index)) = value
            End Set
        End Property

        Public Property ItemBykey(ByVal key As TKey) As TValue
            Get
                Return m_dictionary(key)
            End Get
            Set(ByVal Value As TValue)
                m_dictionary(key) = Value
            End Set
        End Property

        Public Property ItemByIndex(ByVal Index As Integer) As TValue
            Get
                Return m_dictionary(m_keysInOrder(Index))
            End Get
            Set(ByVal Value As TValue)
                m_dictionary(m_keysInOrder(Index)) = Value
            End Set
        End Property

        Public Sub Clear()
            m_dictionary.Clear()
            m_keysInOrder.Clear()
        End Sub

        Public Function Remove(ByVal key As TKey) As Boolean
            Dim Success As Boolean = m_dictionary.Remove(key)
            m_keysInOrder.Remove(key)
            Return Success
        End Function

        Public Function Contains(ByVal item As TValue) As Boolean
            For i As Integer = 0 To m_keysInOrder.Count - 1
                If m_dictionary(m_keysInOrder(i)).Equals(item) Then
                    Return True
                End If
            Next
        End Function

        Public ReadOnly Property Count() As Integer
            Get
                Return m_keysInOrder.Count
            End Get
        End Property

        Public Function RemoveKey(ByVal key As TKey) As Boolean
            For i As Integer = 0 To m_keysInOrder.Count - 1
                If m_keysInOrder(i).Equals(key) Then
                    m_dictionary.Remove(m_keysInOrder(i))
                    Exit For
                End If
            Next
            m_keysInOrder.Remove(key)
        End Function
        Public Function RemoveIndex(ByVal Index As Integer) As Boolean

            Dim Succes As Boolean = m_dictionary.Remove(m_keysInOrder(Index))

            If Succes Then
                m_keysInOrder.RemoveAt(Index)
            End If

            Return Succes

        End Function

        Public Sub Add(ByVal key As TKey, ByVal value As TValue)
            If m_dictionary.ContainsKey(key) = False Then
                m_dictionary.Add(key, value)
                m_keysInOrder.Add(key)
            End If

        End Sub

        Public Function ContainsKey(ByVal key As TKey) As Boolean
            Return m_keysInOrder.Contains(key)
        End Function

        Public Function FindKeyIndex(ByVal key As TKey) As Integer
            For i As Integer = 0 To m_keysInOrder.Count - 1
                If m_keysInOrder(i).Equals(key) Then Return i
            Next
        End Function

        Public ReadOnly Property Keys() As List(Of TKey)
            Get
                Return m_keysInOrder
            End Get
        End Property

        Public ReadOnly Property Values() As List(Of TValue)
            Get
                Dim M_ValuesInOrder As New Collections.Generic.List(Of TValue)

                For i As Integer = 0 To m_keysInOrder.Count - 1
                    M_ValuesInOrder.Add(m_dictionary(m_keysInOrder(i)))
                Next
                Return M_ValuesInOrder
            End Get
        End Property

    End Class

End Namespace