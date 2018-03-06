using System;

namespace ResourceExplorer
{
    internal static class ResourceViewerFactory
    {
        private static MemFileViewer _MemFileViewer = null;

        private static ScriptViewer _ScriptViewer = null;

        private static TextureViewer _TextureViewer = null;

        private static XmlViewer _XmlViewer = null;

        private static RawViewer _RawViewer = null;

        private static BnkViewer _BnkViewer = null;

        private static FlashViewer _FlashViewer = null;

        public static IResourceViewer Create(string name, FileFormats.Archive.ResourceEntry resourceEntry)
        {
            if (Properties.Settings.Default.ArchiveViewer_HexMode)
            {
                return _RawViewer != null ? _RawViewer : _RawViewer = new RawViewer();
            }

            switch (name)
            {
            // case "Generic":
            // case "hkAnimation":
            // case "Flash":

            case "MemFile":
                {
                    int hash = System.BitConverter.ToInt32(resourceEntry.Data, 0); // Hash
                    int size = System.BitConverter.ToInt32(resourceEntry.Data, 4);
                    string file = System.Text.Encoding.ASCII.GetString(resourceEntry.Data, 8, size);
                    int unk = System.BitConverter.ToInt32(resourceEntry.Data, 8 + size);
                    int type = System.BitConverter.ToInt32(resourceEntry.Data, 12 + size);

                    if (type == 16)
                    {
                        return _BnkViewer != null ? _BnkViewer : _BnkViewer = new BnkViewer();
                    }

                    return _MemFileViewer != null ? _MemFileViewer : _MemFileViewer = new MemFileViewer();
                }

            case "Script":
                {
                    return _ScriptViewer != null ? _ScriptViewer : _ScriptViewer = new ScriptViewer();
                }

            case "Texture":
                {
                    return _TextureViewer != null ? _TextureViewer : _TextureViewer = new TextureViewer();
                }

            case "XML":
                {
                    return _XmlViewer != null ? _XmlViewer : _XmlViewer = new XmlViewer();
                }

            case "Flash":
                {
                    return _FlashViewer != null ? _FlashViewer : _FlashViewer = new FlashViewer();
                }
            }

            return _RawViewer != null ? _RawViewer : _RawViewer = new RawViewer();
        }

        public static void Dispose()
        {
            if (_MemFileViewer != null)
            {
                _MemFileViewer.Dispose();
            }

            if (_ScriptViewer != null)
            {
                _ScriptViewer.Dispose();
            }

            if (_TextureViewer != null)
            {
                _TextureViewer.Dispose();
            }

            if (_XmlViewer != null)
            {
                _XmlViewer.Dispose();
            }

            if (_RawViewer != null)
            {
                _RawViewer.Dispose();
            }

            if (_BnkViewer != null)
            {
                _BnkViewer.Dispose();
            }

            if (_FlashViewer != null)
            {
                _FlashViewer.Dispose();
            }
        }

    }
}
