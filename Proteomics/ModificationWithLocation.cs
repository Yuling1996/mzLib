﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proteomics
{
    public class ModificationWithLocation : Modification
    {

        #region Public Fields

        public static readonly Dictionary<string, ModificationSites> terminusLocalizationTypeCodes;
        public readonly Tuple<string, string> accession;
        public readonly IDictionary<string, IList<string>> linksToOtherDbs;
        public readonly ModificationSites terminusLocalization;
        public readonly ModificationMotif motif;

        #endregion Public Fields

        #region Public Constructors

        static ModificationWithLocation()
        {
            terminusLocalizationTypeCodes = new Dictionary<string, ModificationSites>
            {
                { "N-terminal.", ModificationSites.NProt }, // Implies protein only, not peptide
                { "C-terminal.", ModificationSites.ProtC },
                { "Peptide N-terminal.", ModificationSites.NPep },
                { "Peptide C-terminal.", ModificationSites.PepC },
                { "Anywhere.", ModificationSites.Any },
                { "Protein core.", ModificationSites.Any }
            };
        }

        public ModificationWithLocation(string id, Tuple<string, string> accession, ModificationMotif motif, ModificationSites terminusLocalization, IDictionary<string, IList<string>> linksToOtherDbs, string modificationType) : base(id, modificationType)
        {
            this.accession = accession;
            this.motif = motif;
            this.terminusLocalization = terminusLocalization;
            this.linksToOtherDbs = linksToOtherDbs;
        }

        #endregion Public Constructors

        #region Public Methods

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine("PP   " + terminusLocalizationTypeCodes.First(b => b.Value.Equals(terminusLocalization)).Key);
            sb.AppendLine("TG   " + motif.Motif);
            if (linksToOtherDbs != null)
                foreach (var nice in linksToOtherDbs)
                    foreach (var db in nice.Value)
                        sb.AppendLine("DR   " + nice.Key + "; " + db);
            return sb.ToString();
        }

        public override bool Equals(object o)
        {
            ModificationWithLocation m = o as ModificationWithLocation;
            return m == null ? false :

               base.Equals(m)
               && (this.accession == null && m.accession == null
               || this.accession != null && m.accession != null
               && this.accession.Item1 == m.accession.Item1
               && this.accession.Item2 == m.accession.Item2)

               && (this.motif == null && m.motif == null
               || this.motif != null && m.motif != null
               && this.motif.Motif == m.motif.Motif)

               && (this.linksToOtherDbs == null && m.linksToOtherDbs == null
               || this.linksToOtherDbs != null && m.linksToOtherDbs != null
               && this.linksToOtherDbs.Keys.OrderBy(x => x).SequenceEqual(m.linksToOtherDbs.Keys.OrderBy(x => x))
               && this.linksToOtherDbs.Values.SelectMany(x => x).OrderBy(x => x).SequenceEqual(m.linksToOtherDbs.Values.SelectMany(x => x).OrderBy(x => x)))

               && this.modificationType == m.modificationType
               && this.terminusLocalization == m.terminusLocalization;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode() ^ terminusLocalization.GetHashCode();
            hash = hash ^ (modificationType == null ? 0 : modificationType.GetHashCode());
            hash = hash ^ (accession == null ? 0 : accession.GetHashCode());
            hash = hash ^ (motif == null ? 0 : motif.Motif.GetHashCode());
            if (linksToOtherDbs != null)
            {
                foreach (string a in linksToOtherDbs.Keys) hash = hash ^ (a == null ? 0 : a.GetHashCode());
                foreach (string b in linksToOtherDbs.Values.SelectMany(c => c)) hash = hash ^ (b == null ? 0 : b.GetHashCode());
            }
            return hash;
        }

        #endregion Public Methods

    }
}