using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;
using DevExpress.Data.Filtering;
using System.Collections.Generic;

public partial class _Default : System.Web.UI.Page {
    const string ShowAllFilterId = "Select All";

    protected void Page_Load(object sender, EventArgs e) {

    }

    protected void grid_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e) {
        if (e.Column.FieldName != "CategoryName")
            return;
        if (e.Editor is ASPxComboBox) {
            if (Session[GetSessionFilterName(e.Column)] != null) {
                ((ASPxComboBox)e.Editor).Text = Session[GetSessionFilterName(e.Column)].ToString();
            }
            else {
                ((ASPxComboBox)e.Editor).Text = ShowAllFilterId;
            }
        }
    }

    string GetSessionFilterName(GridViewDataColumn column) {
        return column != null ? column.FieldName + "FilterSelIndex" : ShowAllFilterId;
    }

    protected void grid_AutoFilterCellEditorCreate(object sender, ASPxGridViewEditorCreateEventArgs e) {
        ASPxGridView grid = sender as ASPxGridView;
        if (e.Column.FieldName != "CategoryName")
            return;
        ComboBoxProperties combo = new ComboBoxProperties();
        string command = string.Format("SELECT DISTINCT [{0}] FROM [CategoriesFilter]", e.Column.FieldName);
        AccessDataSource ds = new AccessDataSource(AccessDataSource1.DataFile, command);
        DataView dv = ds.Select(DataSourceSelectArguments.Empty) as DataView;
        combo.Items.Add(ShowAllFilterId);
        for (int i = 0; i < dv.Count; i++) {
            combo.Items.Add(dv[i][0].ToString());
        }
        e.EditorProperties = combo;
    }
    protected void grid_ProcessColumnAutoFilter(object sender, ASPxGridViewAutoFilterEventArgs e) {
        if (e.Column.FieldName != "CategoryName")
            return;


        if (e.Kind != GridViewAutoFilterEventKind.CreateCriteria)
            return;
        if (e.Value == ShowAllFilterId) {
            Session[GetSessionFilterName(e.Column)] = null;
            e.Criteria = null;
        }
        else {
            Session[GetSessionFilterName(e.Column)] = e.Value; 

            string leftOperand = "CategoryName";
            string rightOperand = string.Format("%{0}%", e.Value);
            BinaryOperator op = new BinaryOperator(leftOperand, rightOperand, BinaryOperatorType.Like);

            e.Criteria = op;
        }
    }
}
