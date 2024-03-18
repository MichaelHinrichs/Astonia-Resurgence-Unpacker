//Written for Astonia games.
//Astonia Resurgence https://store.steampowered.com/app/1584040/
//Astonia Remastered https://store.steampowered.com/app/1220900/
using System.IO.Compression;

BinaryReader br = new(File.OpenRead(args[0]));
br.ReadSingle();
int fileCount = br.ReadInt32();

List<SUBFILE> subfiles = [];
for (int i = 0; i < fileCount; i++)
{
    subfiles.Add(new()
    {
        start = br.ReadInt32() + 8,
        size = br.ReadInt32(),
        number = br.ReadInt32(),
        unknown = br.ReadInt32()
    });
}

Directory.CreateDirectory(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]));
foreach (var subfile in subfiles)
{
    br.BaseStream.Position = subfile.start;

    using FileStream FS = File.Create(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + subfile.number);
    BinaryWriter bw = new(FS);

    int size = subfile.size;
    MemoryStream ms = new();
    br.ReadInt16();
    using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes(subfile.size - 2)), CompressionMode.Decompress))
        ds.CopyTo(ms);
    br = new(ms);
    br.BaseStream.Position = 0;
    size = (int)ms.Length;

    bw.Write(br.ReadBytes(size));
    bw.Close();
    br = new(File.OpenRead(args[0]));
}

class SUBFILE
{
    public int start;
    public int size;
    public int number;
    public int unknown;
}
