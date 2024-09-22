using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ReportViewer
{
    public abstract class IReportPanel : UserControl
    {
        /// <summary>
        /// Sets the data into the Report panel depending on the parameters passed to this method
        /// </summary>
        /// <param name="id">Session id (used to identify the actual session)</param>
        /// <param name="module">Module ID (used to identify the actual module)</param>
        /// <param name="host">host (dotted notation) (used to identify the host)</param>
        internal abstract void setData(int id, int module, string host);
    }
}
