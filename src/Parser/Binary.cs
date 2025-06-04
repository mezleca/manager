namespace Main.Parser;

public unsafe static class Binary
{
    public static T Read<T>(byte* ptr, int* offset)
    {
        T result = *(T*)(ptr + *offset);
        *offset += sizeof(T);
        return result;
    }

    public static bool ReadBool(byte* ptr, int* offset) => Read<byte>(ptr, offset) != 0x00;

    public static int ReadULEB128(byte* ptr, int* offset)
    {
        int shift = 0;
        int result;

        while (true)
        {
            byte b = Read<byte>(ptr, offset);
            result = (b & 0x7F) << shift;
            shift += 7;
            if ((b & 0x80) == 0)
                break;
        }

        return result;
    }

    public static string ReadString(byte* ptr, int* offset)
    {
        bool present = ReadBool(ptr, offset);

        if (!present) {
            return "";
        }

        var length = ReadULEB128(ptr, offset);
        var result = System.Text.Encoding.UTF8.GetString(ptr + *offset, length);
        *offset += length;

        return result;
    }
} 