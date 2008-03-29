<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
        Me.tsbDiablo = New System.Windows.Forms.ToolStripButton
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RealmsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ManagePluginsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AboutBlueVexToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.StartToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.StopToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator
        Me.QuitToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.ToolStrip1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbDiablo})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 24)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(553, 25)
        Me.ToolStrip1.TabIndex = 1
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'tsbDiablo
        '
        Me.tsbDiablo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbDiablo.Name = "tsbDiablo"
        Me.tsbDiablo.Size = New System.Drawing.Size(75, 22)
        Me.tsbDiablo.Text = "Load Diablo 2"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RealmsToolStripMenuItem, Me.OptionsToolStripMenuItem, Me.ManagePluginsToolStripMenuItem})
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(58, 20)
        Me.SettingsToolStripMenuItem.Text = "Settings"
        '
        'RealmsToolStripMenuItem
        '
        Me.RealmsToolStripMenuItem.Name = "RealmsToolStripMenuItem"
        Me.RealmsToolStripMenuItem.Size = New System.Drawing.Size(159, 22)
        Me.RealmsToolStripMenuItem.Text = "Realms"
        Me.RealmsToolStripMenuItem.Visible = False
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(159, 22)
        Me.OptionsToolStripMenuItem.Text = "Options"
        '
        'ManagePluginsToolStripMenuItem
        '
        Me.ManagePluginsToolStripMenuItem.Name = "ManagePluginsToolStripMenuItem"
        Me.ManagePluginsToolStripMenuItem.Size = New System.Drawing.Size(159, 22)
        Me.ManagePluginsToolStripMenuItem.Text = "Manage Plugins"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutBlueVexToolStripMenuItem})
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'AboutBlueVexToolStripMenuItem
        '
        Me.AboutBlueVexToolStripMenuItem.Name = "AboutBlueVexToolStripMenuItem"
        Me.AboutBlueVexToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.AboutBlueVexToolStripMenuItem.Text = "About BlueVex"
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.NotifyIcon1.Text = "BlueVex"
        Me.NotifyIcon1.Visible = True
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StartToolStripMenuItem1, Me.StopToolStripMenuItem1, Me.ToolStripMenuItem3, Me.QuitToolStripMenuItem1})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(110, 76)
        '
        'StartToolStripMenuItem1
        '
        Me.StartToolStripMenuItem1.Name = "StartToolStripMenuItem1"
        Me.StartToolStripMenuItem1.Size = New System.Drawing.Size(109, 22)
        Me.StartToolStripMenuItem1.Text = "Start"
        '
        'StopToolStripMenuItem1
        '
        Me.StopToolStripMenuItem1.Name = "StopToolStripMenuItem1"
        Me.StopToolStripMenuItem1.Size = New System.Drawing.Size(109, 22)
        Me.StopToolStripMenuItem1.Text = "Stop"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(106, 6)
        '
        'QuitToolStripMenuItem1
        '
        Me.QuitToolStripMenuItem1.Name = "QuitToolStripMenuItem1"
        Me.QuitToolStripMenuItem1.Size = New System.Drawing.Size(109, 22)
        Me.QuitToolStripMenuItem1.Text = "Quit"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SettingsToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.MenuStrip1.Size = New System.Drawing.Size(553, 24)
        Me.MenuStrip1.TabIndex = 3
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(553, 405)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "frmMain"
        Me.Text = "BlueVex Beta 5"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutBlueVexToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RealmsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ManagePluginsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents StartToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StopToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents QuitToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents tsbDiablo As System.Windows.Forms.ToolStripButton

End Class
