using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace Gerber
{
    /// <summary>
    /// Gerber Parser
    /// Every method return an observable with status for a simple progress report implementation
    /// Follow the next steps
    /// Call the constructor
    /// Call ClassifyRows method
    /// Call GenerateDataDraw method
    /// 
    /// Next pass the Draws and and Header to GerberMetaData
    /// </summary>
    public class GerberFileParser
    {
        private readonly string filePath;
        private List<GerberRow> rows;

        /// <summary>
        /// Header usefull for GerberMetaData, previous to use must call GenerateDataTrace
        /// </summary>
        public GerberHeaderDTO Header = new GerberHeaderDTO();

        /// <summary>
        /// Draws usefull for GerberMetaData, previous to use must call GenerateDataDraw
        /// </summary>
        public List<GerberTraceDTO> Draws = new List<GerberTraceDTO>();
        
        /// <summary>
        /// Construct the parser
        /// </summary>
        /// <param name="filePath">Complete path file</param>
        public GerberFileParser(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Read the complete file and classify his rows into Command and Header group
        /// </summary>
        /// <returns>Observable of status process</returns>
        public IObservable<StatusProcessDTO> ClassifyRows()
        {
            return Observable.Create((IObserver<StatusProcessDTO> observer) =>
            {
                try
                {
                    var statusProcess = new StatusProcessDTO() {
                        ProcessName = ConstantMessage.ParseProcessing
                    };
                    var lines = File.ReadAllLines(filePath);
                    rows = new List<GerberRow>(lines.Length);
                    
                    for(var rowIndex = 0; rowIndex < lines.Length; rowIndex++)
                    {
                        var line = lines[rowIndex];
                        var type = line[0] == '%'
                            ? EGerberRowType.Header
                            : (
                                new char[] { 'G', 'X', 'Y', 'M' }.Contains(line[0])
                                ? (EGerberRowType?)EGerberRowType.Command
                                : null
                            );
                        if (type == null)
                        {
                            observer.OnError(new Exception(ConstantMessage.CommandUnknow));
                        }
                        else
                        {
                            rows.Add(new GerberRow
                            {
                                rowIndex = rowIndex,
                                rowText = line,
                                type = (EGerberRowType)type
                            });
                            statusProcess.Percent = rowIndex * 100 / rows.Count();
                            observer.OnNext(statusProcess);
                        }
                    }
                    observer.OnCompleted();
                }
                catch (Exception)
                {
                    observer.OnError(new Exception(ConstantMessage.UnexpectedError));
                }
                return () => { };
            });
        }

        /// <summary>
        /// Compress data into GerberHeaderDTO and list of GerberDrawDTO
        /// It's two objects contain all necesary information for draw in screen or printer
        /// 
        /// It support G01 draw command, it goin to support G02 and G03, G36 and G37.
        /// </summary>
        /// <returns>Observable of status process</returns>
        public IObservable<StatusProcessDTO> GenerateDataDraw()
        {
            return Observable.Create((IObserver<StatusProcessDTO> observer) =>
            {
                try
                {
                    var statusProcess = new StatusProcessDTO() {
                        ProcessName = ConstantMessage.DataDrawProcessing
                    };
                    GerberTraceDTO lastDrawInfo = new GerberTraceDTO
                    {
                        GCode = "G01",
                        ApertureMode = 2,
                        Aperture = 10,
                        IsLPDark = true,
                        AbsolutePointStart = new CoordinateDTO(0, 0),
                        AbsolutePointEnd = new CoordinateDTO(0, 0)
                    };

                    var rowIndex = 0;
                    var parseOk = true;
                    while (rowIndex < rows.Count && parseOk)
                    {
                        var r = rows[rowIndex];
                        rowIndex++;
                        switch (r.type)
                        {
                            case EGerberRowType.Command:
                                var startChar = r.rowText[0];
                                switch(startChar)
                                {
                                    // Command
                                    case 'G':
                                    case 'X':
                                    case 'Y':
                                        switch (startChar == 'G' 
                                            ? r.rowText.Substring(0, 3)
                                            : lastDrawInfo.GCode)
                                        {
                                            case "G04": // Comments are ignored
                                            case "G90": // Absolute coordinate
                                                Header.isAbsolute = true;
                                                break;
                                            case "G91": // Relative coordinate
                                                Header.isAbsolute = false;
                                                break;
                                            case "G70": // Inch unit expressed in Micrometer
                                                Header.UnitInMicroMeters = 25400;
                                                break;
                                            case "G71": // Millimeter Unit expressed in Micrometer
                                                Header.UnitInMicroMeters = 1000;
                                                break;
                                            case "G01": // Linear mode
                                                var di = getDataDraw(r.rowText, lastDrawInfo);
                                                switch(di.ApertureMode)
                                                {
                                                    case 1:
                                                    case 3:
                                                        Draws.Add(di);
                                                        lastDrawInfo = di;
                                                        break;
                                                    case 2: // Mode 02 it just change lastCoordenate
                                                        lastDrawInfo = di;
                                                        break;
                                                }
                                                break;
                                            case "G54": // Aperture change
                                                lastDrawInfo = new GerberTraceDTO{
                                                    GCode = lastDrawInfo.GCode,
                                                    Aperture = int.Parse(r.rowText.Substring(4, 2)),
                                                    ApertureMode = lastDrawInfo.ApertureMode,
                                                    IsLPDark = lastDrawInfo.IsLPDark,
                                                    AbsolutePointStart = lastDrawInfo.AbsolutePointStart,
                                                    AbsolutePointEnd = lastDrawInfo.AbsolutePointEnd
                                                };
                                                break;
                                            default:
                                                observer.OnError(new Exception(ConstantMessage.CommandUnknow));
                                                parseOk = false;
                                                break;
                                        }
                                        break;
                                    // End
                                    case 'M':
                                        // M00 and M01 are deprecated
                                        if (r.rowText.Equals("M02*")) {
                                            observer.OnCompleted();
                                        }
                                        else
                                        {
                                            observer.OnError(new Exception(ConstantMessage.CommandUnknow));
                                            parseOk = false;
                                        }
                                        break;
                                    default:
                                        observer.OnError(new Exception(ConstantMessage.CommandUnknow));
                                        parseOk = false;
                                        break;
                                }
                                break;
                            case EGerberRowType.Header:
                                switch (r.rowText.Substring(1, 2))
                                {
                                    // Format Specification
                                    case "FS":
                                        var reFS = new Regex(@"^%FS([L|T])([A|I])X(\d)(\d)Y\3\4\*%$");
                                        var groupsFS = reFS.Matches(r.rowText)[0].Groups;
                                        Header.isLeadingZeroOmission = groupsFS[1].Value.Equals("L");
                                        Header.isAbsolute = groupsFS[2].Value.Equals("A");
                                        Header.LeadingDigits = int.Parse(groupsFS[3].Value);
                                        Header.TrailingDigits = int.Parse(groupsFS[4].Value);
                                        break;
                                    // Aperture Definition
                                    case "AD":
                                        var reAD = new Regex(@"^%ADD([1-9]\d+)([C|R|O|P]{1}),(?:([\d]*[\.][\d]*)X?)+\*%$");
                                        var groupsAD = reAD.Matches(r.rowText)[0].Groups;
                                        var aperture = new GerberApertureDTO
                                        {
                                            Aperture = int.Parse(groupsAD[1].Value),
                                            Shape = groupsAD[2].Value[0]
                                        };
                                        foreach(Capture c in groupsAD[3].Captures)
                                        {
                                            var valueUnit = double.Parse(c.Value, CultureInfo.InvariantCulture);
                                            var value = (int)(valueUnit * Math.Pow(10, Header.TrailingDigits));
                                            aperture.Modifiers.Add(value);
                                        }
                                        Header.Apertures.Add(aperture);
                                        break;
                                    default:
                                        observer.OnError(new Exception(ConstantMessage.CommandUnknow));
                                        parseOk = false;
                                        break;
                                }
                                break;
                        }
                        statusProcess.Percent = rowIndex * 100 / rows.Count();
                        observer.OnNext(statusProcess);
                    }
                }
                catch (Exception)
                {
                    observer.OnError(new Exception(ConstantMessage.UnexpectedError));
                }
                return () => { };
            });
        }

        /// <summary>
        /// It support G01 draw command, it goin to support G02 and G03. G36 and G37 will maybe be a list of this.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lastDrawInfo"></param>
        /// <returns></returns>
        private GerberTraceDTO getDataDraw(string text, GerberTraceDTO lastDrawInfo)
        {
            var re = new Regex(@"^(G\d\d)?(?:X([-]?\d+))?(?:Y([-]?\d+))?(?:D(0[1-3]{1}))?\*$");
            var matches = re.Matches(text);
            if(matches.Count == 0)
            {
                return null;
            }
            var di = new GerberTraceDTO();

            var gGC = matches[0].Groups[1];
            var gX = matches[0].Groups[2];
            var gY = matches[0].Groups[3];
            var gD = matches[0].Groups[4];

            // GCode
            di.GCode = gGC.Success
                ? gGC.Value
                : lastDrawInfo.GCode;

            // LP change in header
            di.IsLPDark = lastDrawInfo.IsLPDark;

            // Aperture change in G54 command
            di.Aperture = lastDrawInfo.Aperture;

            // End point
            var x = gX.Success
                ? int.Parse(gX.Value.ToString())
                : lastDrawInfo.AbsolutePointEnd.X;
            var y = gY.Success
                ? int.Parse(gY.Value.ToString())
                : lastDrawInfo.AbsolutePointEnd.Y;
            di.AbsolutePointEnd = new CoordinateDTO(x, y);

            // Aperture mode
            di.ApertureMode = gD.Success
                ? int.Parse(gD.Value)
                : lastDrawInfo.ApertureMode;

            // Start point is equal to end last point, in D03 is just a point
            di.AbsolutePointStart = di.ApertureMode != 3
                ? lastDrawInfo.AbsolutePointEnd
                : di.AbsolutePointEnd;

            return di;
        }
    }
}
