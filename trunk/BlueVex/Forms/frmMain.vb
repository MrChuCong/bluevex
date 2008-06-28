Imports System.Net
Imports System.Net.Sockets

Public Class frmMain
    Implements IGUI

    Dim newAbout As frmAbout

#Region " IGUI Implementations "

    Public Sub AddMenuItem(ByRef item As System.Windows.Forms.ToolStripMenuItem) Implements IGUI.AddMenuItem
        Me.MenuStrip1.Items.Insert(Me.MenuStrip1.Items.Count - 1, item)
    End Sub

    Public Sub InitForm(ByRef form As System.Windows.Forms.Form) Implements IGUI.InitForm
        form.MdiParent = Me
        form.WindowState = FormWindowState.Maximized
        form.Visible = True
    End Sub

#End Region

#Region " Hot Keys "

    Private m_hotKeys As HotKeyCollection = New HotKeyCollection(Me)
    Public Event HotKeyPressed As HotKeyPressedEventHandler

    Public ReadOnly Property HotKeys() As HotKeyCollection
        Get
            HotKeys = m_hotKeys
        End Get
    End Property

    Dim Fake As Boolean = False
    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        MyBase.WndProc(m)
        If (m.Msg = UnmanagedMethods.WM_HOTKEY) Then
            Dim hotKeyId As Integer = m.WParam.ToInt32()
            Dim htk As HotKey
            For Each htk In m_hotKeys
                If (htk.AtomId.Equals(m.WParam)) Then
                    Dim e As HotKeyPressedEventArgs = New HotKeyPressedEventArgs(htk)

                    HotKeys.RemoveHotKey(e.HotKey)

                    UnmanagedMethods.keybd_event(e.HotKey.KeyCode, UnmanagedMethods.MapVirtualKey(e.HotKey.KeyCode, 0), 0, 0)
                    UnmanagedMethods.keybd_event(e.HotKey.KeyCode, UnmanagedMethods.MapVirtualKey(e.HotKey.KeyCode, 0), 2, 0)

                    HotKeys.Add(e.HotKey)

                    RaiseEvent HotKeyPressed(Me, e)
                    Exit For
                End If
            Next
        End If
    End Sub

    Protected Overrides Sub OnClosed(ByVal e As System.EventArgs)
        HotKeys.Clear()
        MyBase.OnClosed(e)
    End Sub

#End Region

    Sub LoadModules()
        Dim GUIModule As IGUIModule
        'Initialize Each Module
        For i As Integer = 1 To AvailableGUIModules.Count
            GUIModule = DirectCast(PluginServices.CreateInstance(AvailableGUIModules(i)), IGUIModule)
            If Not My.Settings.DisabledModules.Contains(GUIModule.Name) Then
                GUIModule.Initialize(Me)
            End If
        Next
    End Sub

    Private Loaded As Integer = 0

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
    End Sub

    Public Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Text = IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath)
        Main = Me
        If Not IO.Directory.Exists(Application.StartupPath & "\ManagedPlugins") Then IO.Directory.CreateDirectory(Application.StartupPath & "\ManagedPlugins")

        LoadGameModules()
        LoadChatModules()
        LoadRealmModules()
        LoadGUIModules()
        LoadModules()

        If My.Settings.LoadDiablo Then
            If My.Settings.DiabloPath <> "" Then
                If IO.File.Exists(My.Settings.DiabloPath) Then
                    Dim p As New System.Diagnostics.ProcessStartInfo()
                    p.FileName = My.Settings.DiabloPath
                    p.Arguments = My.Settings.DiabloArgs
                    p.WorkingDirectory = IO.Path.GetDirectoryName(My.Settings.DiabloPath)
                    System.Diagnostics.Process.Start(p)
                End If
            End If
        End If

    End Sub

    Private Delegate Sub GUIModuleInitializeDelegate(ByVal Host As IGUI, ByVal form As System.Windows.Forms.Form)
    Sub LoadGUIModules()
        Dim Plugins() As PluginServices.AvailablePlugin
        Plugins = PluginServices.FindPlugins(Application.StartupPath & "\ManagedPlugins", "BlueVex.IGUIModule")
        If Plugins Is Nothing Then
            'Log.WriteLine("No GUI Modules Found", False)
            Exit Sub
        End If
        For i As Integer = 0 To Plugins.Length - 1
            AvailableGUIModules.Add(Plugins(i))
        Next
        'Log.WriteLine(AvailableGUIModules.Count & " Available Game Modules", False)
    End Sub

    Private Delegate Sub GameModuleInitializeDelegate(ByVal Host As IGame, ByVal form As System.Windows.Forms.Form)

    Sub LoadGameModules()

        Dim Plugins() As PluginServices.AvailablePlugin
        Plugins = PluginServices.FindPlugins(Application.StartupPath & "\ManagedPlugins", "BlueVex.IGameModule")

        If Plugins Is Nothing Then

            Log.WriteLine("Loaded game modules (0)")
            Exit Sub
        End If
        Log.WriteLine("Loaded game modules (" & Plugins.Length & " total)")
        For i As Integer = 0 To Plugins.Length - 1
            AvailableGameModules.Add(Plugins(i))
            Dim GameModule As IGameModule = DirectCast(PluginServices.CreateInstance(Plugins(i)), IGameModule)
            Log.WriteLine("	" & (i + 1) & ".	Title: " & GameModule.Name)
            GameModule = Nothing
        Next

    End Sub

    Private Delegate Sub ChatModuleInitializeDelegate(ByVal Host As IGame, ByVal form As System.Windows.Forms.Form)
    Sub LoadChatModules()
        Dim Plugins() As PluginServices.AvailablePlugin
        Plugins = PluginServices.FindPlugins(Application.StartupPath & "\ManagedPlugins", "BlueVex.IChatModule")
        If Plugins Is Nothing Then
            Log.WriteLine("Loaded chat modules (0 total)")
            Exit Sub
        End If
        Log.WriteLine("Loaded chat modules (" & Plugins.Length & " total)")
        For i As Integer = 0 To Plugins.Length - 1
            AvailableChatModules.Add(Plugins(i))
            Dim ChatModule As IChatModule = DirectCast(PluginServices.CreateInstance(Plugins(i)), IChatModule)
            Log.WriteLine("	" & (i + 1) & ".	Title: " & ChatModule.Name)
            ChatModule = Nothing
        Next
    End Sub

    Private Delegate Sub RealmModuleInitializeDelegate(ByVal Host As IGame, ByVal form As System.Windows.Forms.Form)
    Sub LoadRealmModules()
        Dim Plugins() As PluginServices.AvailablePlugin
        Plugins = PluginServices.FindPlugins(Application.StartupPath & "\ManagedPlugins", "BlueVex.IRealmModule")
        If Plugins Is Nothing Then
            Log.WriteLine("Loaded realm modules (0 total)")
            Log.WriteLine("")
            Exit Sub
        End If
        Log.WriteLine("Loaded realm modules (" & Plugins.Length & " total)")

        For i As Integer = 0 To Plugins.Length - 1
            AvailableRealmModules.Add(Plugins(i))
            Dim RealmModule As IRealmModule = DirectCast(PluginServices.CreateInstance(Plugins(i)), IRealmModule)
            Log.WriteLine("	" & (i + 1) & ".	Title: " & RealmModule.Name)
            RealmModule = Nothing
        Next
    End Sub

#Region " Menu Items and Buttons "

    Private Sub AboutBlueVexToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutBlueVexToolStripMenuItem.Click
        newAbout = New frmAbout
        'newAbout.ShowDialog()
    End Sub

    Private Sub OptionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptionsToolStripMenuItem.Click
        Dim newOptions As New frmOptions
        newOptions.ShowDialog()
    End Sub

    Private Sub NotifyIcon1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.DoubleClick
        'Me.Show()
        'Me.BringToFront()
        'Me.Focus()
    End Sub

    Private Sub QuitToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuitToolStripMenuItem1.Click
        'Me.NotifyIcon1.Visible = False
        'End
    End Sub

    Private Sub ManagePluginsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ManagePluginsToolStripMenuItem.Click
        PluginManagerForm.MdiParent = Me
        Main.Height = 300
        PluginManagerForm.WindowState = FormWindowState.Maximized
        PluginManagerForm.Show()
    End Sub
    Private Sub PluginManagerButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PluginManagerButton.Click

        PluginManagerForm.MdiParent = Me
        Main.Height = 300
        PluginManagerForm.WindowState = FormWindowState.Maximized
        PluginManagerForm.Show()
    End Sub
    Private Sub SettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingsToolStripMenuItem.Click
        SettingsToolStripMenuItem.ShowDropDown()
    End Sub
    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        AboutToolStripMenuItem.ShowDropDown()
    End Sub
#End Region

#Region " Hide MDI Child Icons in the MenuStrip "

    Private Sub MenuStrip1_ItemAdded(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemEventArgs) Handles MenuStrip1.ItemAdded
        If e.Item.GetType.Name.ToString = "SystemMenuItem" Then
            Me.MenuStrip1.Items.Remove(e.Item)
        End If
    End Sub

#End Region

    Private Sub tsbDiablo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbDiablo.Click
        If My.Settings.DiabloPath <> "" Then
            If IO.File.Exists(My.Settings.DiabloPath) Then
                Dim p As New System.Diagnostics.ProcessStartInfo()
                p.FileName = My.Settings.DiabloPath
                p.Arguments = My.Settings.DiabloArgs
                p.WorkingDirectory = IO.Path.GetDirectoryName(My.Settings.DiabloPath)
                System.Diagnostics.Process.Start(p)
            End If
        Else
            MsgBox("Please set your diablo 2 path in the options")
        End If
    End Sub

End Class

