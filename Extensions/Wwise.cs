using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;
using Gibbed.IO;

namespace ResourceExplorer
{
    public class WEMFile
    {
        public uint Num = 0;
        public uint Length = 0;
        public long Offset = 0;
        public uint Id = 0;

        private string _name = "";
        public string Name
        {
            get
            {
                return string.IsNullOrEmpty(_name) ? string.Format("{0}_{1:X8}.wem", Num, Id) : _name;
            }
            set
            {
                _name = value;
            }
        }
    }

    public class DIDXSection
    {
        public uint SectionSize = 0;
        public uint ObjCount = 0;
        public List<WEMFile> DIDXFiles;
    }

    public struct HIRC
    {
        public struct Object
        {
            // [Flags]
            public enum ObjectType : byte
            {
                Settings = 1,
                Sound = 2,
                EventAction = 3,
                Event = 4,
                SequenceContainer = 5,
                SwitchContainer = 6,
                ActorMixer = 7,
                AudioBus = 8,
                BlendContainer = 9,
                MusicSegment = 10,
                MusicTrack = 11,
                MusicSwitchContainer = 12,
                MusicPlaylistContainer = 13,
                Attenuation = 14,
                DialogueEvent = 15,
                MotionBus = 16,
                MotionFX = 17,
                Effect = 18,
                AuxillaryBus = 20
            }

            public ObjectType _ObjectType;
            public uint Length;
            public uint Id;
            public byte[] OtherBytes;

            public struct SoundObject
            {
                public uint SoundFileID;
                // More data left, if that matters
            }
            public struct EventActionData
            {
                public byte EventActionScope;
                public byte EventActionType;
                public uint SoundObjectID;
                // More data left, if that matters
            }
            public struct EventData
            {
                public uint ActionCount;
                public uint[] Actions;
            }

            public SoundObject _SoundObject;
            public EventActionData _EventActionData;
            public EventData _EventData;

            public override string ToString()
            {
                return string.Format("Id=\"{0:D8}\" Type=\"{1:G}\"", Id, _ObjectType);
            }
        }

        public uint length, objectCount;
        public List<Object> objects;
    }

    public class HIRCSection
    {
        public uint SectionSize = 0;
        public uint ObjCount = 0;
        public List<HIRC.Object> HIRCObject = new List<HIRC.Object>();
    }

    public class ControlWriter : TextWriter
    {
        private TextBox textbox;
        public ControlWriter(TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(string value)
        {
            if (textbox.IsDisposed) return;

            if (textbox.InvokeRequired)
            {
                textbox.Invoke((MethodInvoker)(() => textbox.AppendText(value)));
            }
            else
            {
                textbox.AppendText(value);
            }
        }

        public override void WriteLine(string value)
        {
            if (textbox.IsDisposed) return;

            if (textbox.InvokeRequired)
            {
                textbox.Invoke((MethodInvoker)(() => textbox.AppendText(value + Environment.NewLine)));
            }
            else
            {
                textbox.AppendText(value + Environment.NewLine);
            }
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }

    public class WaveFile
    {
        public static void Create(byte[] data, string dst, WaveFileHeader h)
        {
            if (File.Exists(dst))
            {
                File.Delete(dst);
            }
            WaveFileHeader header = CreateNewWaveFileHeader(h.SamplesPerSecond, h.BitsPerSample, h.Channels, (uint)(data.Length), 44 + data.Length);
            WriteHeader(dst, header);
            WriteData(dst, header.DATAPos, data);
        }

        public static void Create(string sourcePath, string outputFile, uint samplesPerSecond = 0, short bitsPerSample = 0, short channels = 0)
        {
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            byte[] data = File.ReadAllBytes(sourcePath);
            WaveFileHeader file_header = ReadHeader(sourcePath);
            if (samplesPerSecond == 0)
            {
                samplesPerSecond = file_header.SamplesPerSecond;
            }
            if (bitsPerSample == 0)
            {
                bitsPerSample = file_header.BitsPerSample;
            }
            if (channels == 0)
            {
                channels = file_header.Channels;
            }

            WaveFileHeader header = CreateNewWaveFileHeader(samplesPerSecond, bitsPerSample, channels, (uint)(data.Length), 44 + data.Length);
            WriteHeader(outputFile, header);
            WriteData(outputFile, header.DATAPos, data);

            data = null;
        }

        public static void AppendData(string fileName, Byte[] data)
        {
            WaveFileHeader header = ReadHeader(fileName);

            if (header.DATASize > 0)
            {
                WriteData(fileName, (int)(header.DATAPos + header.DATASize), data);

                header.DATASize += (uint)data.Length;
                header.RiffSize += (uint)data.Length;

                WriteHeader(fileName, header);
            }
        }

        private static WaveFileHeader CreateNewWaveFileHeader(uint SamplesPerSecond, short BitsPerSample, short Channels, uint dataSize, long fileSize)
        {
            WaveFileHeader Header = new WaveFileHeader();

            Array.Copy(new char[] { 'R', 'I', 'F', 'F' }, Header.RIFF, 4);
            Header.RiffSize = (uint)(fileSize - 8);
            Array.Copy(new char[] { 'W', 'A', 'V', 'E' }, Header.RiffFormat, 4);
            Array.Copy(new char[] { 'f', 'm', 't', ' ' }, Header.FMT, 4);
            Header.FMTSize = 16;
            Header.AudioFormat = WavFormatId.PCM;
            Header.Channels = (short)Channels;
            Header.SamplesPerSecond = (uint)SamplesPerSecond;
            Header.BitsPerSample = (short)BitsPerSample;
            Header.BlockAlign = (short)((BitsPerSample * Channels) >> 3);
            Header.BytesPerSecond = (uint)(Header.BlockAlign * Header.SamplesPerSecond);
            Array.Copy(new char[] { 'd', 'a', 't', 'a' }, Header.DATA, 4);
            Header.DATASize = dataSize;

            return Header;
        }

        private static WaveFileHeader ReadHeader(string fileName)
        {
            WaveFileHeader header = new WaveFileHeader();

            if (File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                if (fs.CanRead)
                {
                    header = ReadHeader(fs);
                }

                fs.Close();
            }

            return header;
        }

        public static WaveFileHeader ReadHeader(Stream input, bool dataRead = true)
        {
            WaveFileHeader header = new WaveFileHeader();

            if (input.CanRead)
            {
                header.RIFF = input.ReadString(4).ToCharArray();
                header.RiffSize = input.ReadValueU32();
                header.RiffFormat = input.ReadString(4).ToCharArray();

                header.FMT = input.ReadString(4).ToCharArray();
                header.FMTSize = input.ReadValueU32();
                header.FMTPos = input.Position;
                //header.AudioFormat = (short)rd.ReadInt16();
                header.AudioFormat = (WavFormatId)input.ReadValueS16();
                header.Channels = input.ReadValueS16();
                header.SamplesPerSecond = input.ReadValueU32();
                header.BytesPerSecond = input.ReadValueU32();
                header.BlockAlign = input.ReadValueS16();
                header.BitsPerSample = input.ReadValueS16();

                if (!dataRead) return header;

                input.Seek(header.FMTPos + header.FMTSize, SeekOrigin.Begin);

                header.DATA = input.ReadString(4).ToCharArray();
                header.DATASize = input.ReadValueU32();
                header.DATAPos = (int)input.Position;

                if (new string(header.DATA).ToUpper() != "DATA")
                {
                    uint DataChunkSize = header.DATASize + 8;
                    input.Seek(DataChunkSize, SeekOrigin.Current);
                    header.DATASize = (uint)(input.Length - header.DATAPos - DataChunkSize);
                }

                header.Payload = input.ReadBytes((int)header.DATASize);
            }
            return header;
        }

        private static void WriteHeader(string fileName, WaveFileHeader header)
        {
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryWriter wr = new BinaryWriter(fs, Encoding.UTF8);

            wr.Write(header.RIFF);
            wr.Write(Int32ToBytes((int)header.RiffSize));
            wr.Write(header.RiffFormat);

            wr.Write(header.FMT);
            wr.Write(Int32ToBytes((int)header.FMTSize));
            //wr.Write(Int16ToBytes(header.AudioFormat));
            wr.Write(Int16ToBytes((short)header.AudioFormat));
            wr.Write(Int16ToBytes(header.Channels));
            wr.Write(Int32ToBytes((int)header.SamplesPerSecond));
            wr.Write(Int32ToBytes((int)header.BytesPerSecond));
            wr.Write(Int16ToBytes((short)header.BlockAlign));
            wr.Write(Int16ToBytes((short)header.BitsPerSample));

            wr.Write(header.DATA);
            wr.Write(Int32ToBytes((int)header.DATASize));

            wr.Close();
            fs.Close();
        }

        private static void WriteData(string fileName, int pos, byte[] data)
        {
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryWriter wr = new BinaryWriter(fs, Encoding.UTF8);

            wr.Seek(pos, System.IO.SeekOrigin.Begin);
            wr.Write(data);
            wr.Close();
            fs.Close();
        }

        private static int BytesToInt32(ref Byte[] bytes)
        {
            int Int32 = 0;
            Int32 = (Int32 << 8) + bytes[3];
            Int32 = (Int32 << 8) + bytes[2];
            Int32 = (Int32 << 8) + bytes[1];
            Int32 = (Int32 << 8) + bytes[0];
            return Int32;
        }

        private static short BytesToInt16(ref Byte[] bytes)
        {
            short Int16 = 0;
            Int16 = (short)((Int16 << 8) + bytes[1]);
            Int16 = (short)((Int16 << 8) + bytes[0]);
            return Int16;
        }

        private static Byte[] Int32ToBytes(int value)
        {
            Byte[] bytes = new Byte[4];
            bytes[0] = (Byte)(value & 0xFF);
            bytes[1] = (Byte)(value >> 8 & 0xFF);
            bytes[2] = (Byte)(value >> 16 & 0xFF);
            bytes[3] = (Byte)(value >> 24 & 0xFF);
            return bytes;
        }

        private static Byte[] Int16ToBytes(short value)
        {
            Byte[] bytes = new Byte[2];
            bytes[0] = (Byte)(value & 0xFF);
            bytes[1] = (Byte)(value >> 8 & 0xFF);
            return bytes;
        }
    }

    // yusik
    public enum WavFormatId : ushort
    {
        UNKNOWN = 0x0000, /* Microsoft Corporation */
        PCM = 0x0001, /* Microsoft Corporation */
        //ADPCM = 0x0002, /* Microsoft Corporation */
        WWISE_ADPCM = 0x0002, /* Microsoft Corporation */
        IEEE_FLOAT = 0x0003, /* Microsoft Corporation */
        VSELP = 0x0004, /* Compaq Computer Corp. */
        IBM_CVSD = 0x0005, /* IBM Corporation */
        ALAW = 0x0006, /* Microsoft Corporation */
        MULAW = 0x0007, /* Microsoft Corporation */
        DTS = 0x0008, /* Microsoft Corporation */
        DRM = 0x0009, /* Microsoft Corporation */
        OKI_ADPCM = 0x0010, /* OKI */
        DVI_ADPCM = 0x0011, /* Intel Corporation */
        IMA_ADPCM = (DVI_ADPCM), /* Intel Corporation */
        MEDIASPACE_ADPCM = 0x0012, /* Videologic */
        SIERRA_ADPCM = 0x0013, /* Sierra Semiconductor Corp */
        G723_ADPCM = 0x0014, /* Antex Electronics Corporation */
        DIGISTD = 0x0015, /* DSP Solutions, Inc. */
        DIGIFIX = 0x0016, /* DSP Solutions, Inc. */
        DIALOGIC_OKI_ADPCM = 0x0017, /* Dialogic Corporation */
        MEDIAVISION_ADPCM = 0x0018, /* Media Vision, Inc. */
        CU_CODEC = 0x0019, /* Hewlett-Packard Company */
        YAMAHA_ADPCM = 0x0020, /* Yamaha Corporation of America */
        SONARC = 0x0021, /* Speech Compression */
        DSPGROUP_TRUESPEECH = 0x0022, /* DSP Group, Inc */
        ECHOSC1 = 0x0023, /* Echo Speech Corporation */
        AUDIOFILE_AF36 = 0x0024, /* Virtual Music, Inc. */
        APTX = 0x0025, /* Audio Processing Technology */
        AUDIOFILE_AF10 = 0x0026, /* Virtual Music, Inc. */
        PROSODY_1612 = 0x0027, /* Aculab plc */
        LRC = 0x0028, /* Merging Technologies S.A. */
        DOLBY_AC2 = 0x0030, /* Dolby Laboratories */
        GSM610 = 0x0031, /* Microsoft Corporation */
        MSNAUDIO = 0x0032, /* Microsoft Corporation */
        ANTEX_ADPCME = 0x0033, /* Antex Electronics Corporation */
        CONTROL_RES_VQLPC = 0x0034, /* Control Resources Limited */
        DIGIREAL = 0x0035, /* DSP Solutions, Inc. */
        DIGIADPCM = 0x0036, /* DSP Solutions, Inc. */
        CONTROL_RES_CR10 = 0x0037, /* Control Resources Limited */
        NMS_VBXADPCM = 0x0038, /* Natural MicroSystems */
        CS_IMAADPCM = 0x0039, /* Crystal Semiconductor IMA ADPCM */
        ECHOSC3 = 0x003A, /* Echo Speech Corporation */
        ROCKWELL_ADPCM = 0x003B, /* Rockwell International */
        ROCKWELL_DIGITALK = 0x003C, /* Rockwell International */
        XEBEC = 0x003D, /* Xebec Multimedia Solutions Limited */
        G721_ADPCM = 0x0040, /* Antex Electronics Corporation */
        G728_CELP = 0x0041, /* Antex Electronics Corporation */
        MSG723 = 0x0042, /* Microsoft Corporation */
        MPEG = 0x0050, /* Microsoft Corporation */
        RT24 = 0x0052, /* InSoft, Inc. */
        PAC = 0x0053, /* InSoft, Inc. */
        MPEGLAYER3 = 0x0055, /* ISO/MPEG Layer3 Format Tag */
        LUCENT_G723 = 0x0059, /* Lucent Technologies */
        CIRRUS = 0x0060, /* Cirrus Logic */
        ESPCM = 0x0061, /* ESS Technology */
        VOXWARE = 0x0062, /* Voxware Inc */
        CANOPUS_ATRAC = 0x0063, /* Canopus, co., Ltd. */
        G726_ADPCM = 0x0064, /* APICOM */
        G722_ADPCM = 0x0065, /* APICOM */
        DSAT_DISPLAY = 0x0067, /* Microsoft Corporation */
        VOXWARE_BYTE_ALIGNED = 0x0069, /* Voxware Inc */
        VOXWARE_AC8 = 0x0070, /* Voxware Inc */
        VOXWARE_AC10 = 0x0071, /* Voxware Inc */
        VOXWARE_AC16 = 0x0072, /* Voxware Inc */
        VOXWARE_AC20 = 0x0073, /* Voxware Inc */
        VOXWARE_RT24 = 0x0074, /* Voxware Inc */
        VOXWARE_RT29 = 0x0075, /* Voxware Inc */
        VOXWARE_RT29HW = 0x0076, /* Voxware Inc */
        VOXWARE_VR12 = 0x0077, /* Voxware Inc */
        VOXWARE_VR18 = 0x0078, /* Voxware Inc */
        VOXWARE_TQ40 = 0x0079, /* Voxware Inc */
        SOFTSOUND = 0x0080, /* Softsound, Ltd. */
        VOXWARE_TQ60 = 0x0081, /* Voxware Inc */
        MSRT24 = 0x0082, /* Microsoft Corporation */
        G729A = 0x0083, /* AT&amp;T Labs, Inc. */
        MVI_MVI2 = 0x0084, /* Motion Pixels */
        DF_G726 = 0x0085, /* DataFusion Systems (Pty) (Ltd) */
        DF_GSM610 = 0x0086, /* DataFusion Systems (Pty) (Ltd) */
        ISIAUDIO = 0x0088, /* Iterated Systems, Inc. */
        ONLIVE = 0x0089, /* OnLive! Technologies, Inc. */
        SBC24 = 0x0091, /* Siemens Business Communications Sys */
        DOLBY_AC3_SPDIF = 0x0092, /* Sonic Foundry */
        MEDIASONIC_G723 = 0x0093, /* MediaSonic */
        PROSODY_8KBPS = 0x0094, /* Aculab plc */
        ZYXEL_ADPCM = 0x0097, /* ZyXEL Communications, Inc. */
        PHILIPS_LPCBB = 0x0098, /* Philips Speech Processing */
        PACKED = 0x0099, /* Studer Professional Audio AG */
        MALDEN_PHONYTALK = 0x00A0, /* Malden Electronics Ltd. */
        RHETOREX_ADPCM = 0x0100, /* Rhetorex Inc. */
        IRAT = 0x0101, /* BeCubed Software Inc. */
        VIVO_G723 = 0x0111, /* Vivo Software */
        VIVO_SIREN = 0x0112, /* Vivo Software */
        DIGITAL_G723 = 0x0123, /* Digital Equipment Corporation */
        SANYO_LD_ADPCM = 0x0125, /* Sanyo Electric Co., Ltd. */
        SIPROLAB_ACEPLNET = 0x0130, /* Sipro Lab Telecom Inc. */
        SIPROLAB_ACELP4800 = 0x0131, /* Sipro Lab Telecom Inc. */
        SIPROLAB_ACELP8V3 = 0x0132, /* Sipro Lab Telecom Inc. */
        SIPROLAB_G729 = 0x0133, /* Sipro Lab Telecom Inc. */
        SIPROLAB_G729A = 0x0134, /* Sipro Lab Telecom Inc. */
        SIPROLAB_KELVIN = 0x0135, /* Sipro Lab Telecom Inc. */
        G726ADPCM = 0x0140, /* Dictaphone Corporation */
        QUALCOMM_PUREVOICE = 0x0150, /* Qualcomm, Inc. */
        QUALCOMM_HALFRATE = 0x0151, /* Qualcomm, Inc. */
        TUBGSM = 0x0155, /* Ring Zero Systems, Inc. */
        MSAUDIO1 = 0x0160, /* Microsoft Corporation */
        UNISYS_NAP_ADPCM = 0x0170, /* Unisys Corp. */
        UNISYS_NAP_ULAW = 0x0171, /* Unisys Corp. */
        UNISYS_NAP_ALAW = 0x0172, /* Unisys Corp. */
        UNISYS_NAP_16K = 0x0173, /* Unisys Corp. */
        CREATIVE_ADPCM = 0x0200, /* Creative Labs, Inc */
        CREATIVE_FASTSPEECH8 = 0x0202, /* Creative Labs, Inc */
        CREATIVE_FASTSPEECH10 = 0x0203, /* Creative Labs, Inc */
        UHER_ADPCM = 0x0210, /* UHER informatic GmbH */
        QUARTERDECK = 0x0220, /* Quarterdeck Corporation */
        ILINK_VC = 0x0230, /* I-link Worldwide */
        RAW_SPORT = 0x0240, /* Aureal Semiconductor */
        ESST_AC3 = 0x0241, /* ESS Technology, Inc. */
        IPI_HSX = 0x0250, /* Interactive Products, Inc. */
        IPI_RPELP = 0x0251, /* Interactive Products, Inc. */
        CS2 = 0x0260, /* Consistent Software */
        SONY_SCX = 0x0270, /* Sony Corp. */
        FM_TOWNS_SND = 0x0300, /* Fujitsu Corp. */
        BTV_DIGITAL = 0x0400, /* Brooktree Corporation */
        QDESIGN_MUSIC = 0x0450, /* QDesign Corporation */
        VME_VMPCM = 0x0680, /* AT&amp;T Labs, Inc. */
        TPC = 0x0681, /* AT&amp;T Labs, Inc. */
        OLIGSM = 0x1000, /* Ing C. Olivetti &amp; C., S.p.A. */
        OLIADPCM = 0x1001, /* Ing C. Olivetti &amp; C., S.p.A. */
        OLICELP = 0x1002, /* Ing C. Olivetti &amp; C., S.p.A. */
        OLISBC = 0x1003, /* Ing C. Olivetti &amp; C., S.p.A. */
        OLIOPR = 0x1004, /* Ing C. Olivetti &amp; C., S.p.A. */
        LH_CODEC = 0x1100, /* Lernout &amp; Hauspie */
        NORRIS = 0x1400, /* Norris Communications, Inc. */
        SOUNDSPACE_MUSICOMPRESS = 0x1500, /* AT&amp;T Labs, Inc. */
        DVM = 0x2000, /* FAST Multimedia AG */
        WWISE_PCM = 0xFFFE, /* // Microsoft (Wwise PCM) */
        WWISE_VORBIS = 0xFFFF
        //EXTENSIBLE = 0xFFFE, /* // Microsoft (Wwise PCM) */
        //DEVELOPMENT = 0xFFFF
    };

    public class WaveFileHeader
    {
        public Char[] RIFF = new Char[4];
        public uint RiffSize = 8;
        public Char[] RiffFormat = new Char[4];
        public Char[] FMT = new Char[4];
        public uint FMTSize = 16;
        //public short AudioFormat;
        public WavFormatId AudioFormat;
        public short Channels;
        public uint SamplesPerSecond;
        public uint BytesPerSecond;
        public short BlockAlign;
        public short BitsPerSample;
        public Char[] DATA = new Char[4];
        public uint DATASize;
        public Byte[] Payload = new Byte[0];
        public int DATAPos = 44;
        public long FMTPos = 20;

        public override string ToString()
        {
            int sampleRate = (BitsPerSample == 0) ? 32 : BitsPerSample;
            return $"{sampleRate}bit {SamplesPerSecond / 1000}kHz channels={Channels} \"{AudioFormat.ToString("g")}\"";

            //return string.Format("{0:G}", AudioFormat);
        }

        public TimeSpan Duration
        {
            get
            {
                int sampleRate = (BitsPerSample == 0) ? 32 : BitsPerSample;
                int blockAlign = ((sampleRate * Channels) >> 3);
                int bytesPerSec = (int)(blockAlign * SamplesPerSecond);
                double value = (double)Payload.Length / (double)bytesPerSec;

                return new TimeSpan(0, 0, (int)value);
            }
        }
    }
}