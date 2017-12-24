using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCBLaserPrinterWindows
{
    class GerberParser
    {
        private readonly string filePath;
        private List<GerberRow> rows;

        public GerberParser(string filePath)
        {
            this.filePath = filePath;
            classifyRows();
        }

        public IObservable<GerberRow> GetCommands()
        {
            return Observable.Create<GerberRow>((IObserver<GerberRow> observer) =>
                {
                    try
                    {
                        foreach(var r in rows.Where(r => r.type == EGerberRowType.Command))
                        {
                            Thread.Sleep(500);
                            observer.OnNext(r);
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

        public int totalHeaders()
        {
            return rows.Count(r => r.type == EGerberRowType.Header);
        }

        public int totalCommands()
        {
            return rows.Count(r => r.type == EGerberRowType.Command);
        }

        void classifyRows()
        {
            var lines = File.ReadAllLines(filePath);
            rows = new List<GerberRow>(lines.Length);
            foreach (var line in lines)
            {
                var type = EGerberRowType.Unknow;
                if (line[0] == '%')
                {
                    type = EGerberRowType.Header;
                }
                if (new char[] { 'G', 'X', 'Y' }.Contains(line[0]))
                {
                    type = EGerberRowType.Command;
                }
                rows.Add(new GerberRow
                {
                    rowIndex = rows.Count,
                    rowText = line,
                    type = type
                });
            }
        }
    }
}
