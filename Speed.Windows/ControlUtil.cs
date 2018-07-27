using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Speed.Windows
{

    public static class ControlUtil
    {

        public static void SetVisible(bool visible, params Control[] controls)
        {
            foreach (var ctrl in controls)
                ctrl.Visible = visible;
        }

        public static List<Control> GetChildren(Control ctrl, bool includeThis = false)
        {
            List<Control> controls = new List<Control>();
            if (includeThis)
                controls.Add(ctrl);
            getAllControls(controls, ctrl);
            return controls;
        }

        /// <summary>
        /// Retorna todos os controles filhos de ctrl. Se includeThis=true, inclui ctrl no retorno tambem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctrl"></param>
        /// <param name="includeThis"></param>
        /// <returns></returns>
        public static List<T> GetChildren<T>(Control ctrl, bool includeThis = false) where T : Control
        {
            return GetChildren(ctrl, includeThis).OfType<T>().ToList();
        }

        /// <summary>
        /// Retorna todos os controles filhos de ctrl. Se includeThis=true, inclui ctrl no retorno tambem
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="ctrl"></param>
        private static void getAllControls(List<Control> controls, Control ctrl)
        {
            foreach (Control c in ctrl.Controls)
            {
                controls.Add(c);
                getAllControls(controls, c);
            }
        }

        public static void ForEachControl(Control control, bool recursive, Action<Control> action)
        {
            foreach (Control ctrl in control.Controls)
            {
                action(ctrl);
                if (recursive)
                    ForEachControl(ctrl, true, action);
            }
        }

        /// <summary>
        /// Sobe na hierarquia procurando por um Parent do tipo T
        /// Se não encontrar retorna null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T FindParent<T>(Control element) where T : Control
        {
            while (element.Parent != null)
            {
                var parent = element.Parent as T;
                if (parent != null)
                    return parent;
                else
                {
                    element = (element.Parent as Control);
                }
            }
            return (T)(null as object);
        }

        /// <summary>
        /// Sobe na hierarquia do controle (Parent) e retorna todos os controles que são ScrollableControl
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static List<ScrollableControl> GetParentScrollableControls(Control control)
        {
            List<ScrollableControl> scrolls = new List<ScrollableControl>();
            Control parent = control.Parent;
            while (parent != null)
            {
                if (parent is ScrollableControl)
                    scrolls.Add((ScrollableControl)parent);
                parent = parent.Parent;
            }
            return scrolls;
        }

        /// <summary>
        /// Dá um Clear em vários TextBoxes
        /// </summary>
        /// <param name="ctrls"></param>
        public static void Clear(params TextBoxBase[] ctrls)
        {
            foreach (var ctrl in ctrls)
                ctrl.Clear();
        }

        /// <summary>
        /// Seta DoubleBuffered=true
        /// </summary>
        /// <param name="c"></param>
        /// <param name="recursive">Se seta DoubleBuffered=true para todos controles filhos</param>
        public static void SetDoubleBuffered(System.Windows.Forms.Control c, bool recursive = false)
        {
            //Taxes: Remote Desktop Connection and painting 
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx 
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);

            if (recursive)
                foreach (var ctrl in Speed.Windows.ControlUtil.GetChildren<Control>(c, false))
                    Speed.Windows.ControlUtil.SetDoubleBuffered(ctrl);
        }

        public static void Fill<T>(this ComboBox list, IEnumerable<T> items, Func<T, string> text, Func<T, T> value)
        {
            foreach (var item in items)
                list.Items.Add(new ItemText<T>(text(item), value(item)));
        }

        public static void FillEnum<T>(this ComboBox list)
        {
            var items = Enum.GetValues(typeof(T));
            foreach (var item in items)
                list.Items.Add(new ItemText<T>(item.ToString(), (T)item));
            list.Sorted = true;
        }

        public static void Fill<T>(this ComboBox list, IEnumerable<T> items)
        {
            Fill<T>(list, items, p => p.ToString(), p => p);
        }

        public static ItemText<T> Insert<T>(this ComboBox list, int index, string text, T value, object tag = null)
        {
            ItemText<T> item = new ItemText<T>(text, value, tag);
            list.Items.Insert(index, item);
            return item;
        }

        public static ItemText<T> Insert<T>(this ComboBox list, string text, T value)
        {
            return Insert(list, 0, text, value);
        }

        public static ItemText<T> Add<T>(this ComboBox list, string text, T value, object tag = null)
        {
            ItemText<T> item = new ItemText<T>(text, value, tag);
            list.Items.Add(item);
            return item;
        }

        public static T SelectedItem<T>(this ComboBox list)
        {
            return ((ItemText<T>)list.SelectedItem).Value;
        }

        public static void SelectByValue<T>(this ComboBox list, T value)
        {
            for (int i = 0; i < list.Items.Count; i++)
            {
                if (((ItemText<T>)list.Items[i]).Value.Equals(value))
                {
                    list.SelectedIndex = i;
                    break;
                }
            }
        }

        public static void SelectByText<T>(this ComboBox list, string text)
        {
            for (int i = 0; i < list.Items.Count; i++)
            {
                if (((ItemText<T>)list.Items[i]).Text.Equals(text))
                {
                    list.SelectedIndex = i;
                    break;
                }
            }
        }

        public static void CenterInParent(this Control ctrl)
        {
            var par = ctrl.Parent;
            if (par == null)
                par = ctrl.FindForm();

            if (par != null)
            {
                par.Tag = ctrl;
                par.SizeChanged += (o, ev) =>
                    {
                        // d + w + d = W    ==> d = (W-x)/2
                        Control c = (Control)((Control)o).Tag;
                        c.Left = (par.Width - c.Width) / 2;
                        c.Invalidate();
                    };
                ctrl.Left = (par.Width - ctrl.Width) / 2;
                ctrl.Invalidate();
            }
        }

    }

}
