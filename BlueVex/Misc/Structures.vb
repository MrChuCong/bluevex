Namespace Structures

    Public Class OrderedDictionary(Of TKey, TValue)
        Implements IDictionary(Of TKey, TValue)

        Private m_dictionary As New Dictionary(Of TKey, TValue)
        Private m_keysInOrder As New List(Of TKey)

        Default Public Overloads Property Item(ByVal index As Integer) As TValue
            Get
                Return CType(m_dictionary, IDictionary(Of TKey, TValue)).Item(m_keysInOrder(index))
            End Get
            Set(ByVal value As TValue)
                CType(m_dictionary, IDictionary(Of TKey, TValue)).Item(m_keysInOrder(index)) = value
            End Set
        End Property
        Default Public Overloads Property Item(ByVal key As TKey) As TValue Implements IDictionary(Of TKey, TValue).Item
            Get
                Return CType(m_dictionary, IDictionary(Of TKey, TValue)).Item(key)
            End Get
            Set(ByVal value As TValue)
                CType(m_dictionary, IDictionary(Of TKey, TValue)).Item(key) = value
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

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Clear
            CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).Clear()
            m_keysInOrder.Clear()
        End Sub
        Public Function Contains(ByVal item As KeyValuePair(Of TKey, TValue)) As Boolean Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Contains
            CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).Contains(item)
        End Function
        Public Sub CopyTo(ByVal array() As KeyValuePair(Of TKey, TValue), ByVal arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of TKey, TValue)).CopyTo
            CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).CopyTo(array, arrayIndex)
        End Sub
        Public ReadOnly Property Count() As Integer Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Count
            Get
                Return CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).Count
            End Get
        End Property
        Public ReadOnly Property IsReadOnly() As Boolean Implements ICollection(Of KeyValuePair(Of TKey, TValue)).IsReadOnly
            Get
                Return CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).IsReadOnly
            End Get
        End Property
        Public Function Remove(ByVal key As TKey) As Boolean Implements IDictionary(Of TKey, TValue).Remove
            Return CType(m_dictionary, IDictionary(Of TKey, TValue)).Remove(key)
            m_keysInOrder.Remove(key)
        End Function
        Public Function Remove(ByVal item As KeyValuePair(Of TKey, TValue)) As Boolean Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Remove
            CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).Remove(item)
            m_keysInOrder.Remove(item.Key)
        End Function
        Public Sub Add(ByVal key As TKey, ByVal value As TValue) Implements IDictionary(Of TKey, TValue).Add
            If m_dictionary.ContainsKey(key) = False Then
                CType(m_dictionary, IDictionary(Of TKey, TValue)).Add(key, value)
                m_keysInOrder.Add(key)
            End If

        End Sub
        Public Sub Add(ByVal item As KeyValuePair(Of TKey, TValue)) Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Add
            CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).Add(item)
            m_keysInOrder.Add(item.Key)
        End Sub
        Public Function ContainsKey(ByVal key As TKey) As Boolean Implements IDictionary(Of TKey, TValue).ContainsKey
            CType(m_dictionary, IDictionary(Of TKey, TValue)).ContainsKey(key)
        End Function
        Public ReadOnly Property Keys() As ICollection(Of TKey) Implements IDictionary(Of TKey, TValue).Keys
            Get
                Return m_keysInOrder
            End Get
        End Property
        Public Function TryGetValue(ByVal key As TKey, ByRef value As TValue) As Boolean Implements IDictionary(Of TKey, TValue).TryGetValue
            Return CType(m_dictionary, IDictionary(Of TKey, TValue)).TryGetValue(key, value)
        End Function
        Public ReadOnly Property Values() As ICollection(Of TValue) Implements IDictionary(Of TKey, TValue).Values
            Get
                Dim rtnVal As New List(Of TValue)

                For Each key As TKey In m_keysInOrder
                    rtnVal.Add(m_dictionary(key))
                Next

                Return rtnVal
            End Get
        End Property
        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of TKey, TValue)) Implements IEnumerable(Of KeyValuePair(Of TKey, TValue)).GetEnumerator
            Return New Enumerator(Me)
        End Function
        Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return New Enumerator(Me)
        End Function
        Public Structure Enumerator
            Implements IEnumerator(Of KeyValuePair(Of TKey, TValue))

            Private m_dictionary As OrderedDictionary(Of TKey, TValue)
            Private m_nextIndex As Integer
            Private m_current As KeyValuePair(Of TKey, TValue)

            Friend Sub New(ByVal orderedDictionary As OrderedDictionary(Of TKey, TValue))
                m_dictionary = orderedDictionary
                m_nextIndex = 0
                m_current = New KeyValuePair(Of TKey, TValue)
            End Sub

            Public ReadOnly Property Current() As KeyValuePair(Of TKey, TValue) Implements IEnumerator(Of KeyValuePair(Of TKey, TValue)).Current
                Get
                    Return New KeyValuePair(Of TKey, TValue)(m_current.Key, m_current.Value)
                End Get
            End Property

            Private ReadOnly Property Current1() As Object Implements IEnumerator.Current
                Get
                    Return Current
                End Get
            End Property

            Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
                Dim rtnVal As Boolean = False

                If m_nextIndex < m_dictionary.Count Then
                    rtnVal = True
                    Dim key As TKey = m_dictionary.m_keysInOrder(m_nextIndex)
                    m_current = New KeyValuePair(Of TKey, TValue)(key, m_dictionary.Item(key))
                    m_nextIndex += 1
                Else
                    m_current = New KeyValuePair(Of TKey, TValue)
                End If

                Return rtnVal
            End Function

            Public Sub Reset() Implements System.Collections.IEnumerator.Reset
                m_nextIndex = 0
            End Sub

            Public Sub Dispose() Implements System.IDisposable.Dispose
            End Sub
        End Structure
    End Class
End Namespace