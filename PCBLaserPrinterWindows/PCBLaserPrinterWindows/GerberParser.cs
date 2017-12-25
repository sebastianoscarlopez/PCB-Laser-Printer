using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace PCBLaserPrinterWindows
{
    class GerberParser
    {
        private readonly string filePath;
        private List<GerberRow> rows;
        private List<GerberDrawInfo> DrawInfo;
        private GerberHeader Header = new GerberHeader();

        public GerberParser(string filePath)
        {
            this.filePath = filePath;
        }

        public IObservable<StatusProcess> ClassifyRows()
        {
            return Observable.Create((IObserver<StatusProcess> observer) =>
            {
                try
                {
                    var statusProcess = new StatusProcess() {
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

        public IObservable<StatusProcess> GenerateDataDraw()
        {
            return Observable.Create((IObserver<StatusProcess> observer) =>
            {
                try
                {
                    var statusProcess = new StatusProcess() {
                        ProcessName = ConstantMessage.DataDrawProcessing
                    };
                    DrawInfo = new List<GerberDrawInfo>();
                    GerberDrawInfo lastDrawInfo = new GerberDrawInfo
                    {
                        GCode = "G01",
                        ApertureMode = 2,
                        Aperture = 10,
                        IsLPDark = true,
                        AbsolutePointStart = new Point(0, 0),
                        AbsolutePointEnd = new Point(0, 0)
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
                                                Header.Unit = 25400;
                                                break;
                                            case "G71": // Millimeter Unit expressed in Micrometer
                                                Header.Unit = 1000;
                                                break;
                                            case "G01": // Linear mode
                                                var di = getDataDraw(r.rowText, lastDrawInfo);
                                                switch(di.ApertureMode)
                                                {
                                                    case 1:
                                                    case 3:
                                                        DrawInfo.Add(di);
                                                        lastDrawInfo = di;
                                                        break;
                                                    case 2: // Mode 02 it just change lastCoordenate
                                                        lastDrawInfo = di;
                                                        break;
                                                }
                                                break;
                                            case "G54": // Aperture change
                                                lastDrawInfo = new GerberDrawInfo{
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
                                        var aperture = new GerberAperture
                                        {
                                            Aperture = int.Parse(groupsAD[1].Value),
                                            Shape = groupsAD[2].Value[0]
                                        };
                                        foreach(Capture c in groupsAD[3].Captures)
                                        {
                                            aperture.Modifiers.Add(double.Parse(c.Value, CultureInfo.InvariantCulture));
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
        

        // It support G01 draw command, it goin to support G02 and G03. G36 and G37 will be a list of this.
        private GerberDrawInfo getDataDraw(string text, GerberDrawInfo lastDrawInfo)
        {
            var re = new Regex(@"^(G\d\d)?(?:X(\d+))?(?:Y(\d+))?(?:D(0[1-3]{1}))?\*$");
            var matches = re.Matches(text);
            if(matches.Count == 0)
            {
                return null;
            }
            var di = new GerberDrawInfo();

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

            // Start point is equal to end last point, in D03 it's ommitted
            di.AbsolutePointStart = lastDrawInfo.AbsolutePointEnd;

            // End point
            var x = gX.Success
                ? int.Parse(gX.Value.ToString())
                : lastDrawInfo.AbsolutePointEnd.X;
            var y = gY.Success
                ? int.Parse(gY.Value.ToString())
                : lastDrawInfo.AbsolutePointEnd.Y;
            di.AbsolutePointEnd = new Point(x, y);
            
            // Aperture mode
            di.ApertureMode = gD.Success
                ? int.Parse(gD.Value)
                : lastDrawInfo.ApertureMode;

            return di;
        }
    }
}
