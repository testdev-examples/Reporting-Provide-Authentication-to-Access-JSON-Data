﻿Imports DevExpress.DataAccess.Json
Imports DevExpress.DataAccess.UI.Native.Json
Imports DevExpress.DataAccess.UI.Json
Imports System
Imports System.Windows.Forms
Imports XtraReport_JsonDataSource_with_Authorization.ReportCustomization
Imports DevExpress.XtraReports.Wizards

Namespace XtraReport_JsonDataSource_with_Authorization
	Partial Public Class Form1
		Inherits Form

		Public Sub New()
			InitializeComponent()
		End Sub
		Private Sub DesignTimeAuthenticationButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button1.Click
			Dim reportDesignTool = New DevExpress.XtraReports.UI.ReportDesignTool(New DevExpress.XtraReports.UI.XtraReport())
			reportDesignTool.DesignForm.DesignMdiController.AddService(GetType(IWizardCustomizationService), New MyWizardCustomizationService())
			reportDesignTool.DesignForm.DesignMdiController.AddService(GetType(IJsonEditorsCustomizationService), New MyWizardCustomizationService())
			reportDesignTool.ShowDesigner()
		End Sub
		Private Sub RuntimeAuthenticationButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button2.Click
			Dim report = (New MyReportHelper()).CreateReport()
			report.DataSource = JsonDataSourceAuthorization_Example.CreateCustomJsonDataSource("http://northwind.servicestack.net/customers.json", "userName1", "userPassword1")
			report.DataMember = "Customers"
			Call (New DevExpress.XtraReports.UI.ReportDesignTool(report)).ShowDesigner()
		End Sub
	End Class
	Public Module JsonDataSourceAuthorization_Example

		Public Function CreateCustomJsonDataSource(ByVal uri As String, ByVal userName As String, ByVal password As String) As JsonDataSource
			Dim jsonDatasource = New JsonDataSource() With {
				.Name = "jsonDataSource",
				.JsonSource = New MyUriJsonSource() With {
					.Uri = New Uri(uri),
					.Username = userName,
					.Password = password
				}
			}
			Return jsonDatasource
		End Function
	End Module
End Namespace