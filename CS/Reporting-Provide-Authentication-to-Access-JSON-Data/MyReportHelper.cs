using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtraReport_JsonDataSource_with_Authorization {
    public class MyReportHelper {
        public XtraReport CreateReport() {
            XtraReport report = new XtraReport();
            report.Bands.AddRange(GetReportBands(report.PageWidth - report.Margins.Left - report.Margins.Right));
            CreateStyles((DetailBand)report.Bands[BandKind.Detail]);
            return report;
        }

        Band[] GetReportBands(float pageWidth) {
            List<Band> reportBands = new List<Band>();
            float indent = 0;
            reportBands.Add(GetGroupHeader("Country", pageWidth, indent, 1));
            indent += 25;
            reportBands.Add(GetGroupHeader("City", pageWidth, indent, 0));
            reportBands.Add(GetDetailBand(pageWidth, indent));

            return reportBands.ToArray();
        }

        Band GetDetailBand(float pageWidth, float indent) {
            DetailBand detailBand = new DetailBand();
            detailBand.HeightF = 25;
            detailBand.Controls.Add(GetDetailsTable(pageWidth, indent));
            return detailBand;
        }

        XRControl GetDetailsTable(float pageWidth, float indent) {
            XRTable detailsTable = XRTable.CreateTable(new RectangleF(indent, 0, pageWidth - indent, 25), 1, 4);
            CreateBindings(detailsTable);
            return detailsTable;
        }

        void CreateStyles(DetailBand detailBand) {
            var evenStyle = new XRControlStyle(Color.LightGray, Color.White, DevExpress.XtraPrinting.BorderSide.None, 0, null, Color.Black, DevExpress.XtraPrinting.TextAlignment.MiddleLeft) { Name = "evenStyle" };
            var oddStyle = new XRControlStyle(Color.White, Color.White, DevExpress.XtraPrinting.BorderSide.None, 0, null, Color.Black, DevExpress.XtraPrinting.TextAlignment.MiddleLeft) { Name = "oddStyle" };
            detailBand.RootReport.StyleSheet.AddRange(new XRControlStyle[] { evenStyle, oddStyle });
            detailBand.OddStyleName = oddStyle.Name;
            detailBand.EvenStyleName = evenStyle.Name;
        }

        void CreateBindings(XRTable detailsTable) {
            detailsTable.Rows[0].Cells[0].ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[CompanyName]"));
            detailsTable.Rows[0].Cells[1].ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Address]"));
            detailsTable.Rows[0].Cells[2].ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Phone]"));
            detailsTable.Rows[0].Cells[3].ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "FormatString('{0}, {1}', [ContactName], [ContactTitle])"));
        }

        GroupHeaderBand GetGroupHeader(string groupFieldName, float pageWidth, float indent, int level) {
            GroupHeaderBand groupBand = new GroupHeaderBand() { HeightF = 35, Level = level };
            groupBand.GroupFields.Add(new GroupField(groupFieldName));
            groupBand.Controls.Add(GetGroupHeaderLabel(groupFieldName, indent, pageWidth));
            return groupBand;
        }

        XRControl GetGroupHeaderLabel(string groupFieldName, float indent, float pageWidth) {
            XRLabel groupLabel = new XRLabel() { SizeF = new SizeF(pageWidth - indent, 25), LocationF = new PointF(indent, 10), Font = new Font("Arial", 12, FontStyle.Bold) };
            groupLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", String.Format("[{0}]", groupFieldName)));
            return groupLabel;
        }
    }
}
