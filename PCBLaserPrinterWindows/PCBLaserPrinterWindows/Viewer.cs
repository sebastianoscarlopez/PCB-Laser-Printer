using DrawerHelper;
using Gerber;
using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace PCBLaserPrinterWindows
{
    public partial class Viewer : Form, IGerberViewer
    {
        readonly GerberPresenter presenter;

        public Viewer()
        {
            InitializeComponent();

            presenter = new GerberPresenter(this);
            var click = Observable.FromEventPattern<EventHandler, EventArgs>(
                eh => btnProcesar.Click += eh,
                eh => btnProcesar.Click -= eh
            )
            .Subscribe(_ => presenter.processGerber(txtFile.Text));
        }

        private void Viewer_Load(object sender, EventArgs e)
        {
        }

        public void startParse()
        {
            hideStatus();
            lblProcess.Text = ConstantMessage.Processing;
            barProcess.Value = 0;
            lblProcess.Visible = barProcess.Visible = statusBar.Visible = true;
        }

        public void parseProgress(StatusProcessDTO status)
        {
            lblProcess.Text = status.ProcessName;
            barProcess.Value = status.Percent;
        }

        public void parseComplete()
        {
            hideStatus();
            presenter.startDrawCanvas();
        }

        public void parseError(Exception exception)
        {
            error(exception.Message);
        }

        public void error(string errorDescription)
        {
            hideStatus();
            lblError.Text = errorDescription;
            lblError.Visible = statusBar.Visible = true;
        }

        void hideStatus()
        {
            lblError.Visible = false;
            lblProcess.Visible = false;
            barProcess.Visible = false;
            statusBar.Visible = false;
        }

        public void refreshCanvas(Bitmap bitmap)
        {
            ViewerBox.Image = bitmap;
        }
    }
}
