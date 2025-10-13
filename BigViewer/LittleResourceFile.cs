namespace BigViewer
{
    internal class LittleResourceFile
    {
        public string filePath = "";
        // public static int resourceCountStart = 0x2;
        public int resourceCount = 0;
        public static ushort offsetTableStart = 0x6;
        public int offsetTableEnd = 0;
        public int typeTableStart = 0;
        public int typeTableEnd = 0;
        public int totalSize = 0;

        public byte[] headerBytes1;
        public byte[] headerBytes2;
        public List<Resource> resources = [];

        public LittleResourceFile(string _filePath, byte[] totalData)
        {
            if (totalData[0x0] == 0x00)
            {
                filePath = _filePath;
                totalSize = totalData.Length;
                resourceCount = BitConverter.ToUInt16(totalData[0x2..0x4]);
                offsetTableEnd = offsetTableStart + (resourceCount * 2);
                typeTableStart = offsetTableEnd + 0x4;
                typeTableEnd = typeTableStart + (resourceCount * 4);

                headerBytes1 = totalData[0x0..0x2];
                headerBytes2 = totalData[0x4..offsetTableStart];

                // Check for inconsistencies
                if (totalSize > 0xFFFF)
                {
                    throw new ArgumentException("File size too large!");
                }
                if (BitConverter.ToUInt16(totalData[offsetTableStart..(offsetTableStart + 2)]) != typeTableEnd)
                {
                    throw new ArgumentException("TOC end and content start mismatch!");
                }
                if (!(totalData[(offsetTableEnd + 2)..(offsetTableEnd + 4)].SequenceEqual(new byte[] { 0x00, 0x00 }) && BitConverter.ToUInt16(totalData[offsetTableEnd..(offsetTableEnd + 2)]) == totalSize))
                {
                    throw new ArgumentException("TOC end mismatch!");
                }

                // Check if TOC is ordered
                List<ushort> testList = [];
                List<ushort> sortList = [];
                for (int i = 0; i < resourceCount; i++)
                {
                    var num = offsetTableStart + (i * 2);
                    testList.Add(BitConverter.ToUInt16(totalData[num..(num + 0x2)]));
                    sortList.Add(BitConverter.ToUInt16(totalData[num..(num + 0x2)]));
                }
                sortList.Sort();
                if (!sortList.SequenceEqual(testList))
                {
                    testList = [];
                    sortList = [];
                    throw new ArgumentException("TOC is not ordered!");
                }
                else
                {
                    testList = [];
                    sortList = [];
                }

                // Read TOC, check for compression and save processed data
                for (int i = 0; i < resourceCount; i++)
                {
                    
                    var numOffset = offsetTableStart + (i * 2);
                    var numType = typeTableStart + (i * 4);
                    ushort offset = BitConverter.ToUInt16(totalData[numOffset..(numOffset + 0x2)]);
                    ushort offsetNext = BitConverter.ToUInt16(totalData[(numOffset + 0x2)..(numOffset + 0x4)]);
                    resources.Add(new Resource(
                        _id: i,
                        _type: totalData[numType..(numType + 0x4)],
                        _offset: offset,
                        _data: totalData[offset..offsetNext]
                    ));
                }
            }
            else
            {
                throw new ArgumentException("Incorrect little header!");
            }
        }

        public void ReplaceResource(int id, byte[] newRawData)
        {
            uint oldSize = resources[id].size;
            resources[id].data = Utils.EncodeResource(newRawData, resources[id].format);
            resources[id].rawData = newRawData;
            resources[id].size = checked((uint)resources[id].data.Length);
            resources[id].rawSize = checked((uint)newRawData.Length);
            uint newSize = resources[id].size;

            if (newSize > oldSize)
            {
                // Update subsequent offsets
                for (int i = id + 1; i < resourceCount; i++)
                {
                    resources[i].offset += (newSize - oldSize);
                }
            }
            else if (oldSize > newSize)
            {
                // Update subsequent offsets
                for (int i = id + 1; i < resourceCount; i++)
                {
                    resources[i].offset -= (oldSize - newSize);
                }
                // Update contentSize
            }
        }

        public byte[] ConstructFile()
        {
            List<byte> result = [];

            // Construct header
            result.AddRange(headerBytes1);
            result.AddRange(BitConverter.GetBytes(checked((ushort)resourceCount)));
            result.AddRange(headerBytes2);

            // Construct offset table
            foreach (Resource res in resources)
            {
                result.AddRange(BitConverter.GetBytes(checked((ushort)res.offset)));
            }

            // Construct offset table ending
            result.AddRange(BitConverter.GetBytes(checked((ushort)totalSize)));
            result.AddRange(new byte[] { 0x00, 0x00 });

            // Construct type table
            foreach (Resource res in resources)
            {
                result.AddRange(res.type);
            }

            // Construct data
            foreach (Resource res in resources)
            {
                result.AddRange(res.data);
            }

            return result.ToArray();
        }
    }
}
