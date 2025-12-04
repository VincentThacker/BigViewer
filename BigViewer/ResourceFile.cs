namespace BigViewer
{
    internal class ResourceFile
    {
        public string filePath = "";
        public int headerSize = 0;
        public int addHeaderCount = 0;
        public int resourceCount = 0;
        public int tableStart = 0;
        public int tableEnd = 0;
        public int contentSize = 0;

        public byte[] headerBytes = [];
        public List<byte[]> addHeaders = [];
        public List<Resource> resources = [];

        public ResourceFile(string _filePath, byte[] totalData)
        {
            if (totalData.Length > 0x30)
            {
                if (!totalData[0x0..0x4].SequenceEqual(new byte[] { 0x46, 0x47, 0x49, 0x42 }))
                {
                    throw new ArgumentException("Incorrect starting bytes!");
                }

                filePath = _filePath;

                headerSize = checked((int)BitConverter.ToUInt32(totalData.AsSpan()[0x8..0xC]));
                if (headerSize < 0x20)
                {
                    throw new ArgumentException("Incorrect file header!");
                }
                headerBytes = totalData[0x0..headerSize];

                addHeaderCount = checked((int)BitConverter.ToUInt32(headerBytes.AsSpan()[0xC..0x10]));
                tableStart = checked((int)BitConverter.ToUInt32(headerBytes.AsSpan()[0x10..0x14]));
                resourceCount = checked((int)BitConverter.ToUInt32(headerBytes.AsSpan()[0x14..0x18]));
                tableEnd = checked((int)BitConverter.ToUInt32(headerBytes.AsSpan()[0x18..0x1C]));
                contentSize = checked((int)BitConverter.ToUInt32(headerBytes.AsSpan()[0x1C..0x20]));

                // Check for inconsistencies
                if (tableEnd - tableStart < 0x10)
                {
                    throw new ArgumentException("TOC must have at least one entry!");
                }
                if (resourceCount * 8 != (tableEnd - tableStart - 8))
                {
                    throw new ArgumentException("TOC length and resource count mismatch!");
                }
                if (checked((int)BitConverter.ToUInt32(totalData.AsSpan()[checked(tableStart + 0x4)..checked(tableStart + 0x8)])) != tableEnd)
                {
                    throw new ArgumentException("TOC end and content start mismatch!");
                }
                if (tableStart - headerSize != addHeaderCount * 8)
                {
                    throw new ArgumentException("Additional header items mismatch!");
                }
                if ((contentSize + tableEnd) != totalData.Length)
                {
                    throw new ArgumentException("Content Size + TOC and file size mismatch!");
                }
                if (!(totalData[(tableEnd - 0x8)..(tableEnd - 0x4)].SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }) && BitConverter.ToUInt32(totalData[(tableEnd - 0x4)..tableEnd]) == totalData.Length))
                {
                    throw new ArgumentException("TOC end mismatch!");
                }

                // Ensure TOC is ordered and has no duplicate offsets
                List<int> testList = [];
                List<int> sortList = [];
                for (int i = 0; i < resourceCount; i++)
                {
                    var num = tableStart + (i * 8);
                    int offset = checked((int)BitConverter.ToUInt32(totalData.AsSpan()[(num + 0x4)..(num + 0x8)]));
                    testList.Add(offset);
                    sortList.Add(offset);
                }
                sortList.Sort();
                if (!sortList.SequenceEqual(testList))
                {
                    testList = [];
                    sortList = [];
                    throw new ArgumentException("TOC is not ordered!");
                }
                else if (testList.Distinct().Count() != testList.Count)
                {
                    testList = [];
                    sortList = [];
                    throw new ArgumentException("TOC has duplicate offsets!");
                }
                else
                {
                    testList = [];
                    sortList = [];
                }

                // Read additional headers
                for (int i = 0; i < addHeaderCount; i++)
                {
                    addHeaders.Add(totalData[(headerSize + (i * 8))..(headerSize + ((i + 1) * 8))]);
                }

                // Construct resource list
                for (int i = 0; i < resourceCount; i++)
                {
                    
                    int num = tableStart + (i * 8);
                    int offset = checked((int)BitConverter.ToUInt32(totalData.AsSpan()[(num + 0x4)..(num + 0x8)]));
                    int offsetNext = checked((int)BitConverter.ToUInt32(totalData.AsSpan()[(num + 0xC)..(num + 0x10)]));
                    resources.Add(new Resource(
                        _id: i,
                        _type: totalData[num..(num + 0x4)],
                        _offset: offset,
                        _data: totalData[offset..offsetNext]
                    ));
                }
            }
            else
            {
                throw new ArgumentException("File is too short!");
            }
        }

        public int GetTotalSize()
        {
            return checked(tableStart + contentSize + ((resourceCount + 1) * 8));
        }

        public void ReplaceResourceRaw(int id, byte[] newRawData)
        {
            int oldSize = resources[id].size;
            resources[id].data = Utils.EncodeResource(newRawData, resources[id].format);
            resources[id].rawData = newRawData;
            resources[id].size = resources[id].data.Length;
            resources[id].rawSize = newRawData.Length;
            int newSize = resources[id].size;

            // Update subsequent offsets
            for (int i = id + 1; i < resourceCount; i++)
            {
                resources[i].offset = checked(resources[i].offset + newSize - oldSize);
            }
            // Update contentSize
            contentSize = checked(contentSize + newSize - oldSize);
            byte[] contentSizeBytes = BitConverter.GetBytes(contentSize);
            headerBytes[0x1C] = contentSizeBytes[0];
            headerBytes[0x1D] = contentSizeBytes[1];
            headerBytes[0x1E] = contentSizeBytes[2];
            headerBytes[0x1F] = contentSizeBytes[3];
        }

        public byte[] ConstructFile()
        {
            List<byte> result = [];

            // Construct header
            result.AddRange(headerBytes);
            // Construct additional header
            foreach (byte[] addHeader in addHeaders)
            {
                result.AddRange(addHeader);
            }
            // Construct TOC
            foreach (Resource res in resources)
            {
                result.AddRange(res.type);
                result.AddRange(BitConverter.GetBytes(res.offset));
            }
            // Construct TOC ending
            result.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
            result.AddRange(BitConverter.GetBytes(GetTotalSize()));
            // Construct data
            foreach (Resource res in resources)
            {
                result.AddRange(res.data);
            }
            return result.ToArray();
        }
    }
}