Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports DevExpress.Web
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Private Const ShowAllFilterId As String = "Select All"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Protected Sub grid_AutoFilterCellEditorInitialize(ByVal sender As Object, ByVal e As ASPxGridViewEditorEventArgs)
        If e.Column.FieldName <> "CategoryName" Then
            Return
        End If
        If TypeOf e.Editor Is ASPxComboBox Then
            If Session(GetSessionFilterName(e.Column)) IsNot Nothing Then
                CType(e.Editor, ASPxComboBox).Text = Session(GetSessionFilterName(e.Column)).ToString()
            Else
                CType(e.Editor, ASPxComboBox).Text = ShowAllFilterId
            End If
        End If
    End Sub

    Private Function GetSessionFilterName(ByVal column As GridViewDataColumn) As String
        Return If(column IsNot Nothing, column.FieldName & "FilterSelIndex", ShowAllFilterId)
    End Function

    Protected Sub grid_AutoFilterCellEditorCreate(ByVal sender As Object, ByVal e As ASPxGridViewEditorCreateEventArgs)
        Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)
        If e.Column.FieldName <> "CategoryName" Then
            Return
        End If
        Dim combo As New ComboBoxProperties()
        Dim command As String = String.Format("SELECT DISTINCT [{0}] FROM [CategoriesFilter]", e.Column.FieldName)
        Dim ds As New AccessDataSource(AccessDataSource1.DataFile, command)
        Dim dv As DataView = TryCast(ds.Select(DataSourceSelectArguments.Empty), DataView)
        combo.Items.Add(ShowAllFilterId)
        For i As Integer = 0 To dv.Count - 1
            combo.Items.Add(dv(i)(0).ToString())
        Next i
        e.EditorProperties = combo
    End Sub
    Protected Sub grid_ProcessColumnAutoFilter(ByVal sender As Object, ByVal e As ASPxGridViewAutoFilterEventArgs)
        If e.Column.FieldName <> "CategoryName" Then
            Return
        End If


        If e.Kind <> GridViewAutoFilterEventKind.CreateCriteria Then
            Return
        End If
        If e.Value = ShowAllFilterId Then
            Session(GetSessionFilterName(e.Column)) = Nothing
            e.Criteria = Nothing
        Else
            Session(GetSessionFilterName(e.Column)) = e.Value

            Dim leftOperand As String = "CategoryName"
            Dim rightOperand As String = String.Format("%{0}%", e.Value)
            Dim op As New BinaryOperator(leftOperand, rightOperand, BinaryOperatorType.Like)

            e.Criteria = op
        End If
    End Sub
End Class
