<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPluginManager
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
        Me.lvModules = New System.Windows.Forms.ListView
        Me.ColumnHeader6 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader3 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader5 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader4 = New System.Windows.Forms.ColumnHeader
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
        Me.tsbEnable = New System.Windows.Forms.ToolStripButton
        Me.tsbDisable = New System.Windows.Forms.ToolStripButton
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lvModules
        '
        Me.lvModules.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader6, Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader5, Me.ColumnHeader4})
        Me.lvModules.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvModules.FullRowSelect = True
        Me.lvModules.Location = New System.Drawing.Point(0, 25)
        Me.lvModules.Name = "lvModules"
        Me.lvModules.Size = New System.Drawing.Size(607, 396)
        Me.lvModules.TabIndex = 0
        Me.lvModules.UseCompatibleStateImageBehavior = False
        Me.lvModules.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Type"
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "ModuleName"
        Me.ColumnHeader1.Width = 200
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Author"
        Me.ColumnHeader2.Width = 120
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Version"
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Release Date"
        Me.ColumnHeader5.Width = 120
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Info"
        Me.ColumnHeader4.Width = 419
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbEnable, Me.tsbDisable})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(607, 25)
        Me.ToolStrip1.TabIndex = 1
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'tsbEnable
        '
        Me.tsbEnable.Enabled = False
        Me.tsbEnable.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbEnable.Name = "tsbEnable"
        Me.tsbEnable.Size = New System.Drawing.Size(46, 22)
        Me.tsbEnable.Text = "Enable"
        '
        'tsbDisable
        '
        Me.tsbDisable.Enabled = False
        Me.tsbDisable.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbDisable.Name = "tsbDisable"
        Me.tsbDisable.Size = New System.Drawing.Size(49, 22)
        Me.tsbDisable.Text = "Disable"
        '
        'frmPluginManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(607, 421)
        Me.Controls.Add(Me.lvModules)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Name = "frmPluginManager"
        Me.ShowIcon = False
        Me.Text = "Plugin Manager"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lvModules As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents ColumnHeader5 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents tsbEnable As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsbDisable As System.Windows.Forms.ToolStripButton
End Class
