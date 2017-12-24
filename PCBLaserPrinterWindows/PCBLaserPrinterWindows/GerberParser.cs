using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace PCBLaserPrinterWindows
{
    class GerberParser
    {
        private readonly string filePath;
        private List<GerberRow> rows;

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
                catch (Exception ex)
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
                    var commands = rows.Where(r => r.type == EGerberRowType.Command).OrderBy(r => r.rowIndex);
                    foreach (var c in commands)
                    {
                        Thread.Sleep(200); // TODO: Eliminar linea
                        rowIndex++;
                        setDataDraw(c);
                        statusProcess.Percent = rowIndex * 100 / commands.Count();
                        observer.OnNext(statusProcess);
                    };
                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    observer.OnError(new Exception(ConstantMessage.UnexpectedError));
                }
                return () => { };
            });
        }
        
        private void setDataDraw(GerberRow r)
        {
            
        }
    }
}
