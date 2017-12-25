using System;
using System.Collections.Generic;
using System.Drawing;
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
                        Thread.Sleep(200); // TODO: Eliminar linea
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
                    var rowIndex = 0;
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
                    
                    foreach (var r in rows.OrderBy(r => r.rowIndex))
                    {
                        Thread.Sleep(200); // TODO: Eliminar linea
                        rowIndex++;
                        switch (r.type)
                        {
                            case EGerberRowType.Command:
                                var startChar = r.rowText[0];
                                switch(startChar)
                                {
                                    // Header
                                    case '%':
                                        break;
                                    // Command
                                    case 'G':
                                    case 'X':
                                    case 'Y':
                                        switch (r.rowText.Substring(0, 3))
                                        {
                                            case "G04": // Comment
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
                                                    GCode = "G54",
                                                    Aperture = int.Parse(r.rowText.Substring(4, 2)),
                                                    ApertureMode = lastDrawInfo.ApertureMode,
                                                    IsLPDark = lastDrawInfo.IsLPDark,
                                                    AbsolutePointStart = lastDrawInfo.AbsolutePointStart,
                                                    AbsolutePointEnd = lastDrawInfo.AbsolutePointEnd
                                                };
                                                break;
                                            default:
                                                observer.OnError(new Exception(ConstantMessage.CommandUnknow));
                                                break;
                                        }
                                        break;
                                    default:
                                        observer.OnError(new Exception(ConstantMessage.CommandUnknow));
                                        break;
                                }

                                break;
                        }
                        statusProcess.Percent = rowIndex * 100 / rows.Count();
                        observer.OnNext(statusProcess);
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

            // LP change in header
            di.IsLPDark = lastDrawInfo.IsLPDark;

            // Aperture change in G54 command
            di.Aperture = lastDrawInfo.Aperture;

            // Start point is equal to end last point, in D03 it's ommitted
            di.AbsolutePointStart = lastDrawInfo.AbsolutePointStart;

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
