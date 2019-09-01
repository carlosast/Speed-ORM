namespace Speed
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class ItemText<T>
    {

        public string Text;
        public T Value;
        public object Tag;

        public ItemText()
        {
        }

        public ItemText(string text, T value, object tag = null)
        {
            this.Text = text;
            this.Value = value;
            this.Tag = tag;
        }

        public override string ToString()
        {
            return Text;
        }

    }

}
