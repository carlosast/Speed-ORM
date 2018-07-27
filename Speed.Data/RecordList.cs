/*
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Speed.Data
{

    [Serializable]
    [CollectionDataContract]
    public class RecordList<T> : List<T>
    {

        private int recorIndex = 0;
        private int bufferSize = 20;

        public RecordList()
        {
        }

        public int RecorIndex
        {
            get
            {
                return recorIndex;
            }
            set
            {
                recorIndex = value;
            }
        }

        public int BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }

        public T[] DataSource
        {
            get
            {
                if (this.Count == 0)
                {
                    return null;
                }
                else
                {
                    int count = bufferSize;
                    if (recorIndex + count > this.Count)
                        count = this.Count - 1 - recorIndex;
                    if (count > 0)
                    {
                        T[] dataSource = new T[count];
                        this.CopyTo(recorIndex, dataSource, 0, count);
                        return dataSource;
                    }
                    else
                        return null;
                }
            }
        }

        public void NextPage()
        {
            recorIndex += bufferSize;
            if (recorIndex > this.Count - 1)
                recorIndex = this.Count - 1;
        }

        public void PreviousPage()
        {
            recorIndex -= bufferSize;
            if (recorIndex < 0)
                recorIndex = 0;
        }

    }
}
*/
