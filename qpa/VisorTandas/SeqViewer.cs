using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace VisorTandas
{
    public class SeqViewer<TElem>
    {
        public delegate Color SeqViewerFilterCur(TElem current);

        public delegate Color SeqViewerFilterPrevCur(TElem previous, TElem current);

        public class Task
        {
            public int Height { get; set; } = 1;
        }

        public class TaskSeparator : Task
        {
            //
        }

        public class TaskFilterCur : Task
        {
            public SeqViewerFilterCur Filter { get; set; }

            public Color Color { get; set; }
        }

        public class TaskFilterPrevCur : Task
        {
            public SeqViewerFilterPrevCur Filter { get; set; }
        }

        private List<Task> Tasks { get; set; } = new List<Task>();

        public Color Fondo { get; private set; } = Color.Gray;

        public Predicate<TElem> IsNoValue { get; private set; } = new Predicate<TElem>(elem => elem == null);

        public SeqViewer<TElem> SetIsNoValuePredicate(Predicate<TElem> isNoValuePredicate)
        {
            IsNoValue = isNoValuePredicate;
            return this;
        }

        public SeqViewer<TElem> SetColorFondo(Color fondo)
        {
            Fondo = fondo;
            return this;
        }

        public SeqViewer<TElem> SetFilter(SeqViewerFilterCur filter)
        {
            Tasks.Add(new TaskFilterCur { Height = 12, Filter = filter });
            return this;
        }

        public SeqViewer<TElem> SetFilter(SeqViewerFilterPrevCur filter)
        {
            Tasks.Add(new TaskFilterPrevCur { Height = 12, Filter = filter });
            return this;
        }

        public SeqViewer<TElem> SetSeparator(int height = 1)
        {
            Tasks.Add(new TaskSeparator { Height = height });
            return this;
        }

        public Bitmap Render(IEnumerable<TElem> data)
        {
            var list = data.ToList();
            var len = list.Count;
            var alturas = Tasks
                .Select(task => task.Height)
                .Sum()
            ;
            var alturaActual = 0;
            var bitmap = new Bitmap(len, alturas);

            foreach (var tx in Tasks)
            {
                if (tx is TaskSeparator)
                {
                    int x = 0;
                    foreach (var _ in data)
                    {
                        using Graphics g = Graphics.FromImage(bitmap);
                        var pen = new Pen(Fondo);
                        g.DrawLine(pen, x, alturaActual, x, alturaActual + tx.Height);
                        x += 1;
                    }
                    alturaActual += tx.Height;
                }
                else if (tx is TaskFilterCur)
                {
                    var filter = (TaskFilterCur)tx;
                    var resultColors = data
                        .Select(elem => filter.Filter(elem))
                        .ToList()
                    ;
                    int x = 0;
                    foreach (var color in resultColors)
                    {
                        using Graphics g = Graphics.FromImage(bitmap);
                        var pen = new Pen(color);
                        g.DrawLine(pen, x, alturaActual, x, alturaActual + tx.Height);
                        x += 1;
                    }
                    alturaActual += tx.Height;
                }
                else if (tx is TaskFilterPrevCur)
                {
                    var filter = (TaskFilterPrevCur)tx;
                    var resultColors = new List<Color>();
                    TElem prev = default;
                    foreach (var elem in data)
                    {
                        if (IsNoValue(elem))
                        {
                            resultColors.Add(Fondo);
                        }
                        else
                        {
                            var color = filter.Filter(prev, elem);
                            resultColors.Add(color);
                            prev = elem;
                        }
                    }

                    int x = 0;
                    foreach (var color in resultColors)
                    {
                        using Graphics g = Graphics.FromImage(bitmap);
                        var pen = new Pen(color);
                        g.DrawLine(pen, x, alturaActual, x, alturaActual + tx.Height);
                        x += 1;
                    }

                    alturaActual += tx.Height;
                }
            }

            return bitmap;
        }
    }
}
