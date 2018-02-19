using Gerber;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;
using GerberDTO;
using System.Windows.Threading;
using System.Reactive.Concurrency;
using System.Collections.Generic;

namespace PCBLaserPrinterWindows
{
    public partial class Viewer : Form, IGerberViewer
    {
        readonly GerberPresenter presenter;
        bool hasPreview = false;
        Subject<ZoomDTO> zoomSubject = new Subject<ZoomDTO>();

        public Viewer()
        {
            InitializeComponent();

            presenter = new GerberPresenter(this);
            var click = Observable.FromEventPattern<EventHandler, EventArgs>(
                eh => btnProcesar.Click += eh,
                eh => btnProcesar.Click -= eh
            )
            .Subscribe(_ => presenter.processGerber(txtFile.Text));
            ViewerBox.Controls.Add(PreviewBox);
            PreviewBox_UpdateLocation();
            PreviewBox.Image = null;

            zoomSubject
                .Buffer(TimeSpan.FromMilliseconds(50))
                .Where(lz => lz.Count() > 0)
                .Select(lz => new ZoomDTO
                {
                    x = lz.Max(z => z.x),
                    y = lz.Max(z => z.y),
                    zoom = lz.Sum(z => z.zoom)
                })
                .ObserveOn(Dispatcher.CurrentDispatcher)
                .SubscribeOn(ThreadPoolScheduler.Instance)
                .Subscribe(z => presenter.Zoom(z));
        }

        /// <summary>
        /// This must be overridden in the Form because the pictureBox never receives MouseWheel messages
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {

            if (hasPreview)
            {
                Point pt_MouseAbs = Control.MousePosition;
                Control i_Ctrl = ViewerBox;
                Rectangle r_Ctrl = i_Ctrl.RectangleToScreen(i_Ctrl.ClientRectangle);
                if (!r_Ctrl.Contains(pt_MouseAbs))
                {
                    base.OnMouseWheel(e);
                    return; // mouse position is outside the picturebox
                }

                // Mouse position relative to the pictureBox
                Point pt_MouseRel = ViewerBox.PointToClient(pt_MouseAbs);
                // Presenter receive zoom request
                zoomSubject.OnNext(new ZoomDTO { x = pt_MouseRel.X, y = pt_MouseAbs.Y, zoom = e.Delta });

            }
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
            hasPreview = false;
            presenter.startDrawCanvas(new Size(PreviewBox.Size.Width, PreviewBox.Size.Height));
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
            lblStatus.Visible = lblError.Visible = lblProcess.Visible = barProcess.Visible = statusBar.Visible = false;
            
        }

        public void refreshCanvas(Bitmap bitmap, Rectangle bounds, int scale)
        {
            var box = ViewerBox;
            if (!hasPreview)
            {
                box = PreviewBox;
            }
            box.Image = bitmap;
            statusBar.Visible = true;
            lblStatus.Text = string.Format("dpi:{0} size:{1}x{2} scale:1/{3}", 
                bitmap.HorizontalResolution.ToString(CultureInfo.InvariantCulture),
                bitmap.Width, bitmap.Height,
                scale);
            lblStatus.Visible = true;
            if (!hasPreview)
            {
                presenter.startDrawCanvas(new Size());
                hasPreview = true;
            }
        }

        public void startPrinter()
        {
            BackColor = Color.Green;
        }

        public void endPrinter()
        {
            BackColor = Color.Red;
        }

        private void Viewer_Resize(object sender, EventArgs e)
        {
            PreviewBox_UpdateLocation();
        }
        private void PreviewBox_UpdateLocation()
        {
            PreviewBox.Location = new Point((int)(ViewerBox.Width * 0.66), (int)(ViewerBox.Height * 0.66));
            PreviewBox.Width = ViewerBox.Width - PreviewBox.Left;
            PreviewBox.Height = ViewerBox.Height - PreviewBox.Top;
        }

        private void btnPrinter_Click(object sender, EventArgs e)
        {
            presenter.print();
        }
    }
}
