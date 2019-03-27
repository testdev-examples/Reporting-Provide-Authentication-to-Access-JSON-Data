﻿Imports DevExpress.DataAccess.Json
Imports DevExpress.DataAccess.Native.Json
Imports DevExpress.DataAccess.UI.Json
Imports DevExpress.DataAccess.UI.Native.Json
Imports DevExpress.DataAccess.UI.Wizard
Imports DevExpress.DataAccess.Wizard
Imports DevExpress.DataAccess.Wizard.Model
Imports DevExpress.DataAccess.Wizard.Presenters
Imports DevExpress.DataAccess.Wizard.Views
Imports DevExpress.XtraLayout
Imports DevExpress.XtraReports.Wizards
Imports System
Imports System.ComponentModel.Design

Namespace XtraReport_JsonDataSource_with_Authorization.ReportCustomization
    Public Interface IMyChooseJsonSourcePageView
        Inherits IChooseJsonSourcePageView

        Property UserName() As String
        Property Password() As String
    End Interface
    Public Class MyChooseJsonSourcePageView
        Inherits DevExpress.DataAccess.UI.Wizard.Views.ChooseJsonSourcePageView
        Implements IMyChooseJsonSourcePageView

        Private Property IChooseJsonSourcePageView_ConnectionType() As JsonConnectionType Implements IChooseJsonSourcePageView.ConnectionType
            Get
                Return MyBase.ConnectionType
            End Get
            Set
                MyBase.ConnectionType = value
            End Set
        End Property

        Private Property IChooseJsonSourcePageView_Json() As String Implements IChooseJsonSourcePageView.Json
            Get
                Return MyBase.Json
            End Get
            Set
                MyBase.Json = value
            End Set
        End Property

        Private Property IChooseJsonSourcePageView_Uri() As String Implements IChooseJsonSourcePageView.Uri
            Get
                Return MyBase.Uri
            End Get
            Set
                MyBase.Uri = value
            End Set
        End Property

        Private Property IChooseJsonSourcePageView_FilePath() As String Implements IChooseJsonSourcePageView.FilePath
            Get
                Return MyBase.FilePath
            End Get
            Set
                MyBase.FilePath = value
            End Set
        End Property

        Public Property UserName() As String Implements IMyChooseJsonSourcePageView.UserName
            Get
                Return userNameCtrl.Text
            End Get
            Set(ByVal value As String)
                userNameCtrl.Text = value
            End Set
        End Property
        Public Property Password() As String Implements IMyChooseJsonSourcePageView.Password
            Get
                Return PasswordCtrl.Text
            End Get
            Set(ByVal value As String)
                PasswordCtrl.Text = value
            End Set
        End Property

        Protected ReadOnly userNameLayoutItem As LayoutControlItem
        Protected ReadOnly passwordLayoutItem As LayoutControlItem
        Protected ReadOnly userNameCtrl As DevExpress.XtraEditors.TextEdit
        Protected ReadOnly PasswordCtrl As DevExpress.XtraEditors.TextEdit
        Public Sub New()
            MyBase.New()
            userNameLayoutItem = layoutControlContent.AddItem()
            userNameLayoutItem.Text = "User Name"
            userNameCtrl = New DevExpress.XtraEditors.TextEdit()
            userNameLayoutItem.Control = userNameCtrl

            passwordLayoutItem = layoutControlContent.AddItem()
            passwordLayoutItem.Text = "Password"
            PasswordCtrl = New DevExpress.XtraEditors.TextEdit()
            PasswordCtrl.Properties.PasswordChar = "*"c
            PasswordCtrl.Properties.UseSystemPasswordChar = True
            passwordLayoutItem.Control = PasswordCtrl
        End Sub

        Protected Overrides Sub TypeComboOnSelectedValueChanged(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.TypeComboOnSelectedValueChanged(sender, e)
            If userNameLayoutItem Is Nothing OrElse passwordLayoutItem Is Nothing Then
                Return
            End If
            If ConnectionType <> JsonConnectionType.Uri Then
                userNameLayoutItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never
                passwordLayoutItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never
            Else
                userNameLayoutItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                passwordLayoutItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always
            End If
        End Sub
    End Class
    Public Class MyChooseJsonSourcePage(Of TModel As {Class, IJsonDataSourceModel})
        Inherits ChooseJsonSourcePage(Of TModel)

        Private Context As IWizardRunnerContext
        Public Sub New(ByVal view As IChooseJsonSourcePageView, ByVal context As IWizardRunnerContext)
            MyBase.New(view, context)
            Me.Context = context
        End Sub
        Public Overrides Sub Begin()
            MyBase.Begin()
            If Model.JsonSource IsNot Nothing Then
                Dim myJsonSource = TryCast(Model.JsonSource, MyUriJsonSource)
                If myJsonSource IsNot Nothing AndAlso TypeOf View Is IMyChooseJsonSourcePageView Then
                    TryCast(View, IMyChooseJsonSourcePageView).Password = myJsonSource.Password
                    TryCast(View, IMyChooseJsonSourcePageView).UserName = myJsonSource.UserName
                End If
            End If
        End Sub
        Public Overrides Function Validate(<System.Runtime.InteropServices.Out()> ByRef errorMessage As String) As Boolean
            If View.ConnectionType <> JsonConnectionType.Uri Then
                Return MyBase.Validate(errorMessage)
            Else
                errorMessage = Nothing
                Dim source As JsonDataSource = Nothing
                Try
                    Return AsyncHelper.DoWithWaitForm(Sub(token)
                        source = New JsonDataSource With { _
                            .JsonSource = New MyUriJsonSource() With { _
                                .Uri = New Uri(View.Uri), _
                                .UserName = (TryCast(View, IMyChooseJsonSourcePageView))?.UserName, _
                                .Password = (TryCast(View, IMyChooseJsonSourcePageView))?.Password _
                            } _
                        }
                        token.ThrowIfCancellationRequested()
                        source.Fill()
                        Model.DataSchema = DevExpress.DataAccess.Native.Data.DataView.ConvertToDataView(source)
                        Model.JsonSchema = source.Schema
                    End Sub, Context.WaitFormActivator, Context.CreateExceptionHandler(ExceptionHandlerKind.Loading))
                Catch e As Exception
                    errorMessage = e.Message
                    Return False
                Finally
                    source?.Dispose()
                End Try
            End If
        End Function
        Public Overrides Sub Commit()
            Model.JsonSource = New MyUriJsonSource() With { _
                .Uri = New Uri(View.Uri), _
                .UserName = (TryCast(View, IMyChooseJsonSourcePageView))?.UserName, _
                .Password = (TryCast(View, IMyChooseJsonSourcePageView))?.Password _
            }
            RemoveHandler View.Changed, AddressOf ViewOnChanged
        End Sub
    End Class
    Public Class MyWizardCustomizationService
        Implements IWizardCustomizationService, IJsonEditorsCustomizationService

        Public Sub CustomizeDataSourceWizard(ByVal tool As IWizardCustomization(Of XtraReportModel)) Implements IWizardCustomizationService.CustomizeDataSourceWizard
            tool.RegisterPage(Of ChooseJsonSourcePage(Of XtraReportModel), MyChooseJsonSourcePage(Of XtraReportModel))()
            tool.RegisterPageView(Of IChooseJsonSourcePageView, MyChooseJsonSourcePageView)()
        End Sub

        Public Sub CustomizeReportWizard(ByVal tool As IWizardCustomization(Of XtraReportModel)) Implements IWizardCustomizationService.CustomizeReportWizard
            tool.RegisterPage(Of ChooseJsonSourcePage(Of XtraReportModel), MyChooseJsonSourcePage(Of XtraReportModel))()
            tool.RegisterPageView(Of IChooseJsonSourcePageView, MyChooseJsonSourcePageView)()
        End Sub

        Public Sub CustomizeWizard(ByVal editor As JsonEditorId, ByVal tool As IWizardCustomization(Of JsonDataSourceModel)) Implements IJsonEditorsCustomizationService.CustomizeWizard
            tool.RegisterPage(Of ChooseJsonSourcePage(Of JsonDataSourceModel), MyChooseJsonSourcePage(Of JsonDataSourceModel))()
            tool.RegisterPageView(Of IChooseJsonSourcePageView, MyChooseJsonSourcePageView)()
        End Sub

        Public Function TryCreateDataSource(ByVal model As IDataSourceModel, <System.Runtime.InteropServices.Out()> ByRef dataSource As Object, <System.Runtime.InteropServices.Out()> ByRef dataMember As String) As Boolean Implements IWizardCustomizationService.TryCreateDataSource
            dataSource = Nothing
            dataMember = model?.DataMember
            Return False
        End Function

        Public Function TryCreateReport(ByVal designerHost As IDesignerHost, ByVal model As XtraReportModel, ByVal dataSource As Object, ByVal dataMember As String) As Boolean Implements IWizardCustomizationService.TryCreateReport
            Return False
        End Function
    End Class
End Namespace
