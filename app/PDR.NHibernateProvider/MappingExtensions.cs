using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using NHibernate.Cfg.MappingSchema;

namespace PDR.NHibernateProvider
{
    public static class NHibernateMappingsExtensions
    {
        /// <summary>
        /// Writes all XML mapping.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <param name="name">The name of mappings file.</param>
        public static void WriteAllXmlMapping(this IEnumerable<HbmMapping> mappings, string name = null)
        {
            if (mappings == null)
            {
                throw new ArgumentNullException("mappings");
            }

            string mappingsFolderPath = ArrangeMappingsFolderPath();
            foreach (HbmMapping hbmMapping in mappings)
            {
                string fileName = name ?? GetFileName(hbmMapping);
                string document = Serialize(hbmMapping);
                File.WriteAllText(Path.Combine(mappingsFolderPath, fileName), document);
            }
        }

        /// <summary>
        /// Writes all XML mapping.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="name">The name of mappings file.</param>
        public static void WriteAllXmlMapping(this HbmMapping mapping, string name = null)
        {
            var mappings = new List<HbmMapping> { mapping };
            mappings.WriteAllXmlMapping(name);
        }

        /// <summary>
        /// Ases the string.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>The mappings as string.</returns>
        public static string AsString(this HbmMapping mappings)
        {
            if (mappings == null)
            {
                throw new ArgumentNullException("mappings");
            }

            return Serialize(mappings);
        }

        #region Auxillary

        /// <summary>
        /// Arranges the mappings folder path.
        /// </summary>
        /// <returns>The folder of mappings.</returns>
        private static string ArrangeMappingsFolderPath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
            string binPath = relativeSearchPath != null ? Path.Combine(baseDir, relativeSearchPath) : baseDir;
            var dir = Directory.CreateDirectory(binPath);
            while (dir != null && dir.Name != "bin")
            {
                dir = dir.Parent;
            }

            if (dir == null)
            {
                throw new DirectoryNotFoundException("Can't found 'bin' directory");
            }

            string parentBinDirPath = dir.Parent.FullName;
            string mappingsFolderPath = Path.Combine(parentBinDirPath, "Mappings");

            if (!Directory.Exists(mappingsFolderPath))
            {
                Directory.CreateDirectory(mappingsFolderPath);
            }
            else
            {
                Array.ForEach(Directory.GetFiles(mappingsFolderPath), File.Delete);
            }

            return mappingsFolderPath;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="hbmMapping">The HBM mapping.</param>
        /// <returns>The file name.</returns>
        private static string GetFileName(HbmMapping hbmMapping)
        {
            string name = "MyMapping";
            HbmClass rc = hbmMapping.RootClasses.FirstOrDefault();
            if (rc != null)
            {
                name = rc.Name;
            }

            HbmSubclass sc = hbmMapping.SubClasses.FirstOrDefault();
            if (sc != null)
            {
                name = sc.Name;
            }

            HbmJoinedSubclass jc = hbmMapping.JoinedSubclasses.FirstOrDefault();
            if (jc != null)
            {
                name = jc.Name;
            }

            HbmUnionSubclass uc = hbmMapping.UnionSubclasses.FirstOrDefault();
            if (uc != null)
            {
                name = uc.Name;
            }

            return name + ".hbm.xml";
        }

        /// <summary>
        /// Serializes the specified HBM element.
        /// </summary>
        /// <param name="hbmElement">The HBM element.</param>
        /// <returns>The serialized mappings.</returns>
        private static string Serialize(HbmMapping hbmElement)
        {
            string result;
            var setting = new XmlWriterSettings { Indent = true };
            var serializer = new XmlSerializer(typeof(HbmMapping));
            using (var memStream = new MemoryStream(2048))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(memStream, setting))
                {
                    serializer.Serialize(xmlWriter, hbmElement);
                }

                memStream.Position = 0;
                using (var sr = new StreamReader(memStream))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }

        #endregion
    }
}