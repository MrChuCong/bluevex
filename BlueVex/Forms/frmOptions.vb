Imports System.Configuration

Public Class frmOptions

    Private Sub butOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butOK.Click
        My.Settings.LoadDiablo = Me.cbLoadDiablo.Checked
        My.Settings.DiabloPath = Me.txtDiabloPath.Text
        My.Settings.DiabloArgs = Me.txtArgs.Text
        My.Settings.Save()
        Me.Close()
    End Sub

    Private Sub butCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butCancel.Click
        Me.Close()
    End Sub

    Private Sub frmOptions_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.cbLoadDiablo.Checked = My.Settings.LoadDiablo
        Me.txtDiabloPath.Text = My.Settings.DiabloPath
        Me.txtArgs.Text = My.Settings.DiabloArgs
    End Sub

    Private Sub butBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butBrowse.Click
        Me.OpenFileDialog1.ShowDialog()
        If Me.OpenFileDialog1.FileName <> "" Then
            Me.txtDiabloPath.Text = Me.OpenFileDialog1.FileName
        End If
    End Sub

End Class