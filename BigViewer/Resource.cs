namespace BigViewer
{
    internal class Resource
    {
        public int id;
        public byte[] type;
        public string typeName;
        public uint offset;
        public uint format;
        public string formatName;
        public uint size;
        public uint rawSize;
        public byte[] data;
        public byte[] rawData;
        public byte[] otherData;

        public Resource(int _id, byte[] _type, uint _offset, byte[] _data)
        {
            id = _id;
            type = _type;
            typeName = Utils.GetTypeName(_type);
            offset = _offset;
            data = _data;
            size = checked((uint)_data.Length);
            (format, rawData) = Utils.DecodeResource(data);
            otherData = [];
            formatName = Utils.GetFormatName(format);
            rawSize = checked((uint)rawData.Length);
        }

        public Resource(int _id, byte[] _type, uint _offset, byte[] _data, byte[] _otherData)
        {
            id = _id;
            type = _type;
            typeName = Utils.GetTypeName(_type);
            offset = _offset;
            data = _data;
            size = checked((uint)_data.Length);
            (format, rawData) = Utils.DecodeResource(data);
            otherData = _otherData;
            formatName = Utils.GetFormatName(format);
            rawSize = checked((uint)rawData.Length);
        }
    }
}