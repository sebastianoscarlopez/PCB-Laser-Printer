using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using System.Reactive.Concurrency;

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
            lblProcess.Text = "Parse processing";
            barProcess.Value = 0;
            lblProcess.Visible = barProcess.Visible = statusBar.Visible = true;
        }

        public void parseProgress(int progress, int total)
        {
            barProcess.Maximum = total;
            barProcess.Value = progress;
        }

        public void parseProgress(int progress)
        {
            barProcess.Value = progress;
        }

        public void parseComplete()
        {
            hideStatus();
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
    }
}
