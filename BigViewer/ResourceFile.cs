namespace BigViewer
{
    internal class ResourceFile
    {
        // public bool active = false;
        public string filePath = "";
        public uint headerSize = 0;
        public uint resourceCount = 0;
        public uint tableStart = 0;
        public uint tableEnd = 0;
        public uint contentSize = 0;
        public uint firstResourceOffset = 0;

        public byte[] headerBytes;
        public List<Resource> resources = [];

        public ResourceFile(string _filePath, byte[] totalData)
        {
            if (totalData[0x0..0x4].SequenceEqual(new byte[] { 0x46, 0x47, 0x49, 0x42 }))
            {
                filePath = _filePath;

                headerSize = BitConverter.ToUInt32(totalData[0x8..0xC]);
                tableStart = BitConverter.ToUInt32(totalData[0x10..0x14]);
                resourceCount = BitConverter.ToUInt32(totalData[0x14..0x18]);
                tableEnd = BitConverter.ToUInt32(totalData[0x18..0x1C]);
                contentSize = BitConverter.ToUInt32(totalData[0x1C..0x20]);

                firstResourceOffset = BitConverter.ToUInt32(totalData[checked((int)(tableStart + 0x4))..checked((int)(tableStart + 0x8))]);
                headerBytes = totalData[0x0..checked((int)tableStart)];

                // Check for inconsistencies
                if (resourceCount * 8 != (tableEnd - tableStart - 8))
                {
                    throw new ArgumentException("TOC length and resource count mismatch!");
                }
                if (firstResourceOffset != tableEnd)
                {
                    throw new ArgumentException("TOC end and content start mismatch!");
                }
                if ((contentSize + tableEnd) != totalData.Length)
                {
                    throw new ArgumentException("Content Size + TOC and file size mismatch!");
                }
                if (!(totalData[checked((int)(tableEnd - 0x8))..checked((int)(tableEnd - 0x4))].SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }) && BitConverter.ToUInt32(totalData[checked((int)(tableEnd - 0x4))..checked((int)tableEnd)]) == totalData.Length))
                {
                    throw new ArgumentException("TOC end mismatch!");
                }

                // Check if TOC is ordered
                List<uint> testList = [];
                List<uint> sortList = [];
                for (int i = 0; i < resourceCount; i++)
                {
                    var num = tableStart + (i * 8);
                    testList.Add(BitConverter.ToUInt32(totalData[checked((int)(num + 0x4))..checked((int)(num + 0x8))]));
                    sortList.Add(BitConverter.ToUInt32(totalData[checked((int)(num + 0x4))..checked((int)(num + 0x8))]));
                }
                sortList.Sort();
                if (!sortList.SequenceEqual(testList))
                {
                    testList = [];
                    sortList = [];
                    throw new ArgumentException("TOC is not ordered! Abandoning.");
                }
                else
                {
                    testList = [];
                    sortList = [];
                }

                // Read TOC, check for compression and save processed data
                for (int i = 0; i < resourceCount; i++)
                {
                    
                    var num = tableStart + (i * 8);
                    uint offset = BitConverter.ToUInt32(totalData[checked((int)(num + 0x4))..checked((int)(num + 0x8))]);
                    uint offsetNext = BitConverter.ToUInt32(totalData[checked((int)(num + 0xC))..checked((int)(num + 0x10))]);
                    resources.Add(new Resource(
                        _id: i,
                        _type: totalData[checked((int)num)..checked((int)(num + 0x4))],
                        _offset: BitConverter.ToUInt32(totalData[checked((int)(num + 0x4))..checked((int)(num + 0x8))]),
                        _data: totalData[checked((int)offset)..checked((int)offsetNext)]
                    ));
                }
            }
            else
            {
                throw new ArgumentException("Incorrect BIG header!");
            }
        }

        public void ReplaceResource(int id, byte[] newRawData)
        {
            uint oldSize = resources[id].size;
            resources[id].data = Resource.EncodeResource(newRawData, resources[id].format);
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
                // Update contentSize
                contentSize += (newSize - oldSize);
                byte[] contentSizeBytes = BitConverter.GetBytes(contentSize);
                headerBytes[0x1C] = contentSizeBytes[0];
                headerBytes[0x1D] = contentSizeBytes[1];
                headerBytes[0x1E] = contentSizeBytes[2];
                headerBytes[0x1F] = contentSizeBytes[3];
            }
            else if (oldSize > newSize)
            {
                // Update subsequent offsets
                for (int i = id + 1; i < resourceCount; i++)
                {
                    resources[i].offset -= (oldSize - newSize);
                }
                // Update contentSize
                contentSize -= (oldSize - newSize);
                byte[] contentSizeBytes = BitConverter.GetBytes(contentSize);
                headerBytes[0x1C] = contentSizeBytes[0];
                headerBytes[0x1D] = contentSizeBytes[1];
                headerBytes[0x1E] = contentSizeBytes[2];
                headerBytes[0x1F] = contentSizeBytes[3];
            }
        }

        public byte[] ConstructFile()
        {
            List<byte> result = [];

            // Construct header
            result.AddRange(headerBytes);

            // Construct TOC
            foreach (Resource res in resources)
            {
                result.AddRange(res.type);
                result.AddRange(BitConverter.GetBytes(res.offset));
            }

            // Construct TOC ending
            result.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
            result.AddRange(BitConverter.GetBytes(headerSize + contentSize + ((resourceCount + 1) * 8)));

            // Construct data
            foreach (Resource res in resources)
            {
                result.AddRange(res.data);
            }

            return result.ToArray();
        }
    }
}
