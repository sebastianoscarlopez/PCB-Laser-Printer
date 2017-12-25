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
        private List<GerberDrawData> drawData;

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
                    drawData = new List<GerberDrawData>();
                    GerberDrawData lastDrawData;
                    var lastCoordenate = new Point(0, 0);
                    
                    foreach (var r in rows.OrderBy(r => r.rowIndex))
                    {
                        Thread.Sleep(200); // TODO: Eliminar linea
                        var coordenate = getCoordenate(r.rowText);
                        rowIndex++;
                        switch (r.type)
                        {
                            case EGerberRowType.Command:
                                if (r.rowText.StartsWith("G"))
                                {
                                    switch (r.rowText.Substring(0, 3))
                                    {
                                        case "G04": // Comment
                                            break;
                                        case "G01": // Linear mode
                                            lastDrawData = new GerberDrawData()
                                            {
                                                GCode = r.rowText.Substring(0, 3),
                                                AbsolutePointStart = lastCoordenate,
                                                AbsolutePointEnd = (Point)coordenate
                                            };
                                            lastCoordenate = (Point)coordenate;
                                            drawData.Add(lastDrawData);
                                            break;
                                        default:
                                            observer.OnError(new Exception(ConstantMessage.CommandUnknow));
                                            break;
                                    }
                                }
                                
                                break;
                        }
                        statusProcess.Percent = rowIndex * 100 / rows.Count();
                        observer.OnNext(statusProcess);
                    },¿;
                    observer.OnCompleted();
                }
                catch (Exception)
                {
                    observer.OnError(new Exception(ConstantMessage.UnexpectedError));
                }
                return () => { };
            });
        }
        
        private Point? getCoordenate(string text)
        {
            var re = new Regex(@"(?:G\d\d){0,1}(?:X(\d+)){0,1}(?:Y(\d+)){0,1}");
            var matches = re.Matches(text);
            if(matches.Count == 3)
            {
                return new Point(int.Parse(matches[1].ToString()), int.Parse(matches[2].ToString()));
            }
            return null;
        }
    }
}
