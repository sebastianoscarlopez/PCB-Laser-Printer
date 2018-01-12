using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBLaserPrinterCommunication
{
    public class Config
    {
        #region Printer constants configuration
        /// <summary>
        /// Width in micrometers
        /// </summary>
        public readonly int PrinterWidth = 200000;
        /// <summary>
        /// Angle in degrees
        /// </summary>
        public readonly int PrinterAngle = 45;
        #endregion
        
        #region Data sended by Controller
        /// <summary>
        /// Controller work unit picoseconds
        /// </summary>
        public int ControllerUnit { get; set; }

        /// <summary>
        /// A motor revolution in microseconds
        /// </summary>
        public int MotorRevolutionAverage { get; set; }
        #endregion

        #region Calculate
        /// <summary>
        /// Ratio of convertion between GerberUnit and ControllerUnit
        /// </summary>
        public double RatioConvert { get; set; }
        /// <summary>
        /// Width in ControllerUnit
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Height in ControllerUnit
        /// </summary>
        public int Height { get; set; }
        #endregion
    }
}
