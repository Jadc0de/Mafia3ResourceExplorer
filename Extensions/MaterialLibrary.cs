using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gibbed.IO;

namespace ResourceExplorer.Extensions
{
	public class MaterialLib
	{
		public class ShaderParameter
		{
			public string fourcc;

			public double[] data;

			public ShaderParameter(Stream r)
			{
                fourcc = Encoding.ASCII.GetString(r.ReadBytes(4));
				int num = r.ReadValueS32();
				int num2 = num / 4;
                data = new double[num2];
				for (int i = 0; i < num2; i++)
				{
                    data[i] = (double)r.ReadValueF32();
				}
			}
		}

		public class ShaderParameterSampler
		{
			public string fourcc;

			public ulong textureHash;

			public string texture;

			public ShaderParameterSampler(Stream r)
			{
				fourcc = Encoding.ASCII.GetString(r.ReadBytes(4));
				textureHash = r.ReadValueU64();
                short length = r.ReadValueS16();
				byte[] bytes = r.ReadBytes(length);
				texture = Encoding.ASCII.GetString(bytes);
            }

			public override string ToString()
			{
				return string.Format("{0} \"{1}\"", fourcc, texture);
			}
		}

		public class Material
		{
			public uint HashPart1;

			public uint HashPart2;

			public ulong HashComplete;

			public string Name;

			public byte alpha1;

			public byte alpha2;

			public int unk3;

			public byte unk4;

			public int unk5;

			public int unk6;

            public int UnkHash;

            public ulong ShaderHash;

            public uint AdaptHash;

            public ShaderParameter[] Parameters;

			public ShaderParameterSampler[] Samplers;

			public Material(Stream r)
			{
                Read(r);
			}

			public void Read(Stream r)
			{
                HashPart1 = r.ReadValueU32();
                HashPart2 = r.ReadValueU32();
                HashComplete = ((ulong)HashPart2 << 32 | (ulong)HashPart1);
				int length = r.ReadValueS32();
				byte[] bytes = r.ReadBytes(length);
                Name = Encoding.ASCII.GetString(bytes);
                alpha1 = r.ReadValueU8();
                alpha2 = r.ReadValueU8();
                unk3 = r.ReadValueS32();
                unk4 = r.ReadValueU8();
				this.unk5 = r.ReadValueS32();
                unk6 = r.ReadValueS32();
                UnkHash = r.ReadValueS32(); // mafia 3
                ShaderHash = r.ReadValueU64();
                AdaptHash = r.ReadValueU32();

                int count = r.ReadValueS32();
                Parameters = new ShaderParameter[count];
				for (int i = 0; i < count; i++)
				{
                    Parameters[i] = new ShaderParameter(r);
				}

				count = r.ReadValueS32();
                Samplers = new ShaderParameterSampler[count];
				for (int j = 0; j < count; j++)
				{
                    Samplers[j] = new ShaderParameterSampler(r);
				}

                count = r.ReadValueS32();
                for (int j = 0; j < count; j++)
                {
                    var fourcc = Encoding.ASCII.GetString(r.ReadBytes(4));
                    var unk5 = r.ReadBytes(12);
                }
            }

			public override string ToString()
			{
				return string.Format("\"{0}\"", Name);
			}
		}

        public Dictionary<ulong, Material> Materials;

        public MaterialLib(Stream input)
		{
            Read(input);
		}

		public void Read(Stream input)
		{
            if (input.ReadValueS32() != 1112298573)
			{
				throw new NotSupportedException("not a material library");
			}

			if (input.ReadValueS32() != 63)
			{
				throw new NotSupportedException("unknown version");
			}

			int count = input.ReadValueS32();

			input.ReadValueS32();

            Materials = new Dictionary<ulong, Material>();
			for (int i = 0; i < count; i++)
			{
				Material material = new Material(input);
                Materials.Add(material.HashComplete, material);
			}
		}
	}
}
