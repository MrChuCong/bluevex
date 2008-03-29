Public Class frmPluginManager

    Private Sub frmPluginManager_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Me.Hide()
        e.Cancel = True
    End Sub

    Private Sub frmPluginManager_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.lvModules.Items.Clear()

        Dim GameModule As IGameModule
        For i As Integer = 1 To AvailableGameModules.Count
            GameModule = DirectCast(PluginServices.CreateInstance(AvailableGameModules(i)), IGameModule)
            Dim lvi As New ListViewItem
            lvi.Text = "Game"
            lvi.Tag = "Game"
            lvi.SubItems.Add(GameModule.Name)
            lvi.SubItems.Add(GameModule.Author)
            lvi.SubItems.Add(GameModule.Version)
            lvi.SubItems.Add(GameModule.ReleaseDate)
            lvi.SubItems.Add(GameModule.AboutInfo)
            Me.lvModules.Items.Add(lvi)
        Next

        Dim RealmModule As IRealmModule
        'Initialize Each Module
        For i As Integer = 1 To AvailableRealmModules.Count
            RealmModule = DirectCast(PluginServices.CreateInstance(AvailableRealmModules(i)), IRealmModule)
            Dim lvi As New ListViewItem
            lvi.Text = "Realm"
            lvi.Tag = "Realm"
            lvi.SubItems.Add(RealmModule.Name)
            lvi.SubItems.Add(RealmModule.Author)
            lvi.SubItems.Add(RealmModule.Version)
            lvi.SubItems.Add(RealmModule.ReleaseDate)
            lvi.SubItems.Add(RealmModule.AboutInfo)
            Me.lvModules.Items.Add(lvi)
        Next

        Dim ChatModule As IChatModule
        'Initialize Each Module
        For i As Integer = 1 To AvailableChatModules.Count
            ChatModule = DirectCast(PluginServices.CreateInstance(AvailableChatModules(i)), IChatModule)
            Dim lvi As New ListViewItem
            lvi.Text = "Chat"
            lvi.Tag = "Chat"
            lvi.SubItems.Add(ChatModule.Name)
            lvi.SubItems.Add(ChatModule.Author)
            lvi.SubItems.Add(ChatModule.Version)
            lvi.SubItems.Add(ChatModule.ReleaseDate)
            lvi.SubItems.Add(ChatModule.AboutInfo)
            Me.lvModules.Items.Add(lvi)
        Next

        Dim GUIModule As IGUIModule
        'Initialize Each Module
        For i As Integer = 1 To AvailableGUIModules.Count
            GUIModule = DirectCast(PluginServices.CreateInstance(AvailableGUIModules(i)), IGUIModule)
            Dim lvi As New ListViewItem
            lvi.Text = "GUI"
            lvi.Tag = "GUI"
            lvi.SubItems.Add(GUIModule.Name)
            lvi.SubItems.Add(GUIModule.Author)
            lvi.SubItems.Add(GUIModule.Version)
            lvi.SubItems.Add(GUIModule.ReleaseDate)
            lvi.SubItems.Add(GUIModule.AboutInfo)
            Me.lvModules.Items.Add(lvi)
        Next

        For Each item As ListViewItem In Me.lvModules.Items
            If My.Settings.DisabledModules Is Nothing Then My.Settings.DisabledModules = New Collections.Specialized.StringCollection
            If My.Settings.DisabledModules.Contains(item.SubItems(1).Text) Then
                item.Text = "Disabled"
                item.ForeColor = Drawing.Color.Gray
            End If
        Next

    End Sub

    Private Sub lvModules_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvModules.SelectedIndexChanged
        If Me.lvModules.SelectedItems.Count = 1 Then
            If Me.lvModules.SelectedItems(0).Text = "Disabled" Then
                Me.tsbEnable.Enabled = True
                Me.tsbDisable.Enabled = False
            Else
                Me.tsbEnable.Enabled = False
                Me.tsbDisable.Enabled = True
            End If
        ElseIf Me.lvModules.SelectedItems.Count = 0 Then
            Me.tsbDisable.Enabled = False
            Me.tsbEnable.Enabled = False
        Else
            Me.tsbDisable.Enabled = True
            Me.tsbEnable.Enabled = True
        End If
    End Sub

    Private Sub tsbEnable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbEnable.Click
        For Each item As ListViewItem In Me.lvModules.SelectedItems
            item.Text = item.Tag
            item.ForeColor = Drawing.Color.Black
            If My.Settings.DisabledModules.Contains(item.SubItems(1).Text) Then
                My.Settings.DisabledModules.Remove(item.SubItems(1).Text)
            End If
        Next
        lvModules_SelectedIndexChanged(Nothing, Nothing)
        My.Settings.Save()
    End Sub

    Private Sub tsbDisable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbDisable.Click
        For Each item As ListViewItem In Me.lvModules.SelectedItems
            If item.Text <> "Disabled" Then
                item.Text = "Disabled"
                item.ForeColor = Drawing.Color.Gray
                My.Settings.DisabledModules.Add(item.SubItems(1).Text)
            End If
        Next
        lvModules_SelectedIndexChanged(Nothing, Nothing)
        My.Settings.Save()
    End Sub

End Class