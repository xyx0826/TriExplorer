using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriExplorer
{
    class NodeCategories
    {
        Dictionary<string, Style> _iconMappings;
        Dictionary<string, string> _descMappings;

        #region Singleton
        static NodeCategories _singleInstance;

        public static NodeCategories Instance
        {
            get
            {
                if (_singleInstance == null)
                    _singleInstance = new NodeCategories();
                return _singleInstance;
            }
        }

        private NodeCategories()
        {
            _iconMappings = new Dictionary<string, Style>()
            {
                {"gr2", Application.Current.FindResource("ModelIcon") as Style },
                {"dds", Application.Current.FindResource("ImageIcon") as Style },
                {"png", Application.Current.FindResource("ImageIcon") as Style },
                {"black", Application.Current.FindResource("SceneIcon") as Style },
                {"sm_depth", Application.Current.FindResource("EffectIcon") as Style },
                {"sm_hi", Application.Current.FindResource("EffectIcon") as Style },
                {"sm_lo", Application.Current.FindResource("EffectIcon") as Style },
                {"bnk", Application.Current.FindResource("AudioIcon") as Style },
                {"wem", Application.Current.FindResource("AudioIcon") as Style },
                {"xml", Application.Current.FindResource("YamlIcon") as Style },
                {"yaml", Application.Current.FindResource("YamlIcon") as Style },
                {"txt", Application.Current.FindResource("YamlIcon") as Style },
                {"FOLDER", Application.Current.FindResource("FolderIcon") as Style },
                {"DEFAULT", Application.Current.FindResource("MiscIcon") as Style },
            };

            _descMappings = new Dictionary<string, string>()
            {
                {"gr2", "Granny2 model file" },
                {"dds", "DDS (DirectDraw Surface) texture file" },
                {"png", "PNG (Portal Network Graphics) texture/icon file" },
                {"black", "EVE Online scene file" },
                {"sm_depth", "EVE Online effect file" },
                {"sm_hi", "EVE Online effect file" },
                {"sm_lo", "EVE Online effect file" },
                {"bnk", "Audiokinetic Wwise sound bank file" },
                {"wem", "Audiokinetic Wwise audio file" },
                {"xml", "XML markup file" },
                {"yaml", "YAML markup file" },
                {"txt", "Text file" },
                {"DEFAULT", "(Unknown file format)" },
            };
        }
        #endregion

        public Style FindIcon(string nodeType)
        {
            var style = new Style();
            var iconExists = _iconMappings.TryGetValue(nodeType, out style);
            if (!iconExists) style = _iconMappings["DEFAULT"];
            return style;
        }

        public string FindDesc(string fileType)
        {
            var desc = "";
            var descExists = _descMappings.TryGetValue(fileType, out desc);
            if (!descExists) desc = _descMappings["DEFAULT"];
            return desc;
        }
    }
}
