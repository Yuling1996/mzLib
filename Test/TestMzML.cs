﻿using Chemistry;
using IO.MzML;
using MassSpectrometry;
using MzIdentML;
using NUnit.Framework;
using Proteomics;
using Spectra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Test
{
    [TestFixture]
    public sealed class TestMzML
    {

        #region Public Methods

        [OneTimeSetUp]
        public void setup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            UsefulProteomicsDatabases.Loaders.LoadElements(@"elements.dat");
        }

        [Test]
        public void LoadMzmlTest()
        {
            Mzml a = new Mzml(@"tiny.pwiz.1.1.mzML");
            a.Open();
            Assert.AreEqual(true, a.IsIndexedMzML);

            var ya = a.GetOneBasedScan(1).MassSpectrum;

            a.Close();
        }

        [Test]
        public void WriteMzmlTest()
        {
            var peptide = new Peptide("GPEAPPPALPAGAPPPCTAVTSDHLNSLLGNILR");
            ChemicalFormulaModification carbamidomethylationOfCMod = new ChemicalFormulaModification("H3C2NO", "carbamidomethylation of C", ModificationSites.C);
            peptide.AddModification(carbamidomethylationOfCMod);

            DefaultMzSpectrum MS1 = createSpectrum(peptide.GetChemicalFormula(), 300, 2000, 1);

            DefaultMzSpectrum MS2 = createMS2spectrum(peptide.Fragment(FragmentTypes.b | FragmentTypes.y, true), 100, 1500);

            MsDataScan<IMzSpectrum<MzPeak>>[] Scans = new MsDataScan<IMzSpectrum<MzPeak>>[2];
            Scans[0] = new MsDataScan<IMzSpectrum<MzPeak>>(1, MS1.NewSpectrumApplyFunctionToX(b => b + 0.000001 * b + 0.000001), "spectrum 1", 1, false, Polarity.Positive, 1.0, new MzRange(300, 2000), "FTMS first spectrum", MZAnalyzerType.Unknown, 1, MS1.SumOfAllY);

            Scans[1] = new MsDataScan<IMzSpectrum<MzPeak>>(2, MS2.NewSpectrumApplyFunctionToX(b => b + 0.00001 * b + 0.00001), "spectrum 2", 2, false, Polarity.Positive, 2.0, new MzRange(100, 1500), "FTMS second spectrum", MZAnalyzerType.Unknown, 1, MS2.SumOfAllY, "spectrum 1", 1134.26091302033, 3, 0.141146966879759, 1134.3, 1, DissociationType.Unknown, 1, 0.141146966879759, 1134.26091302033);

            var myMsDataFile = new FakeMsDataFile(@"myFakeFile.mzML", Scans);

			var oldFirstValue = myMsDataFile.GetOneBasedScan(1).MassSpectrum.XArray[0];

            MzmlMethods.CreateAndWriteMyIndexedMZmlwithCalibratedSpectra(myMsDataFile, Path.Combine(Path.GetDirectoryName(myMsDataFile.FilePath), Path.GetFileNameWithoutExtension(myMsDataFile.FilePath)) + ".mzML");

            Mzml okay = new Mzml(@"myFakeFile.mzML");
            okay.Open();
            okay.GetOneBasedScan(2);

            Assert.AreEqual(1, okay.GetClosestOneBasedSpectrumNumber(1));
            Assert.AreEqual(2, okay.GetClosestOneBasedSpectrumNumber(2));


			var newFirstValue = okay.GetOneBasedScan(1).MassSpectrum.XArray[0];

			Assert.AreEqual(oldFirstValue, newFirstValue, 1e-9);
        }

        [Test]
        public void mzidTest()
        {
            XmlSerializer _indexedSerializer = new XmlSerializer(typeof(mzIdentML.Generated.MzIdentMLType));
            var _mzid = new mzIdentML.Generated.MzIdentMLType();
            _mzid.DataCollection = new mzIdentML.Generated.DataCollectionType();
            _mzid.DataCollection.AnalysisData = new mzIdentML.Generated.AnalysisDataType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList = new mzIdentML.Generated.SpectrumIdentificationListType[1];
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0] = new mzIdentML.Generated.SpectrumIdentificationListType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult = new mzIdentML.Generated.SpectrumIdentificationResultType[1];
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0] = new mzIdentML.Generated.SpectrumIdentificationResultType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].spectrumID = "spectrum 2";
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem = new mzIdentML.Generated.SpectrumIdentificationItemType[1];
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0] = new mzIdentML.Generated.SpectrumIdentificationItemType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].experimentalMassToCharge = 1134.2609130203 + 0.000001 * 1134.2609130203 + 0.000001;
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].calculatedMassToCharge = 1134.26091302033;
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].calculatedMassToChargeSpecified = true;
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].chargeState = 3;
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].cvParam = new mzIdentML.Generated.CVParamType[1];
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].cvParam[0] = new mzIdentML.Generated.CVParamType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].cvParam[0].value = 100.ToString();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].PeptideEvidenceRef = new mzIdentML.Generated.PeptideEvidenceRefType[1];
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].PeptideEvidenceRef[0] = new mzIdentML.Generated.PeptideEvidenceRefType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].PeptideEvidenceRef[0].peptideEvidence_ref = "PE_1";

            _mzid.DataCollection.Inputs = new mzIdentML.Generated.InputsType();
            _mzid.DataCollection.Inputs.SpectraData = new mzIdentML.Generated.SpectraDataType[1];
            _mzid.DataCollection.Inputs.SpectraData[0] = new mzIdentML.Generated.SpectraDataType();
            _mzid.DataCollection.Inputs.SpectraData[0].FileFormat = new mzIdentML.Generated.FileFormatType();
            _mzid.DataCollection.Inputs.SpectraData[0].FileFormat.cvParam = new mzIdentML.Generated.CVParamType();
            _mzid.DataCollection.Inputs.SpectraData[0].FileFormat.cvParam.name = "mzML format";

            _mzid.SequenceCollection = new mzIdentML.Generated.SequenceCollectionType();
            _mzid.SequenceCollection.PeptideEvidence = new mzIdentML.Generated.PeptideEvidenceType[1];
            _mzid.SequenceCollection.PeptideEvidence[0] = new mzIdentML.Generated.PeptideEvidenceType();
            _mzid.SequenceCollection.PeptideEvidence[0].isDecoy = false;
            _mzid.SequenceCollection.PeptideEvidence[0].peptide_ref = "P_1";
            _mzid.SequenceCollection.Peptide = new mzIdentML.Generated.PeptideType[1];
            _mzid.SequenceCollection.Peptide[0] = new mzIdentML.Generated.PeptideType();
            _mzid.SequenceCollection.Peptide[0].id = "P_1";
            _mzid.SequenceCollection.Peptide[0].PeptideSequence = "GPEAPPPALPAGAPPPCTAVTSDHLNSLLGNILR";
            _mzid.SequenceCollection.Peptide[0].Modification = new mzIdentML.Generated.ModificationType[1];
            _mzid.SequenceCollection.Peptide[0].Modification[0] = new mzIdentML.Generated.ModificationType();
            _mzid.SequenceCollection.Peptide[0].Modification[0].locationSpecified = true;
            _mzid.SequenceCollection.Peptide[0].Modification[0].location = 17;
            _mzid.SequenceCollection.Peptide[0].Modification[0].monoisotopicMassDeltaSpecified = true;
            _mzid.SequenceCollection.Peptide[0].Modification[0].monoisotopicMassDelta = 57.02146373;
            _mzid.SequenceCollection.Peptide[0].Modification[0].cvParam = new mzIdentML.Generated.CVParamType[1];
            _mzid.SequenceCollection.Peptide[0].Modification[0].cvParam[0] = new mzIdentML.Generated.CVParamType();
            _mzid.SequenceCollection.Peptide[0].Modification[0].cvParam[0].accession = "UNIMOD:4";
            _mzid.SequenceCollection.Peptide[0].Modification[0].cvParam[0].name = "Carbamidomethyl";
            _mzid.SequenceCollection.Peptide[0].Modification[0].cvParam[0].cvRef = "UNIMOD";

            _mzid.AnalysisProtocolCollection = new mzIdentML.Generated.AnalysisProtocolCollectionType();
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol = new mzIdentML.Generated.SpectrumIdentificationProtocolType[1];
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0] = new mzIdentML.Generated.SpectrumIdentificationProtocolType();
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].ParentTolerance = new mzIdentML.Generated.CVParamType[1];
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].ParentTolerance[0] = new mzIdentML.Generated.CVParamType();
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].ParentTolerance[0].unitName = "dalton";
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].ParentTolerance[0].value = "0.1";

            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].FragmentTolerance = new mzIdentML.Generated.CVParamType[1];
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].FragmentTolerance[0] = new mzIdentML.Generated.CVParamType();
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].FragmentTolerance[0].unitName = "dalton";
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].FragmentTolerance[0].value = "0.01";

            TextWriter writer = new StreamWriter("myIdentifications.mzid");
            _indexedSerializer.Serialize(writer, _mzid);
            writer.Close();

            var identifications = new MzidIdentifications("myIdentifications.mzid");

            Assert.AreEqual(1134.26091302033, identifications.CalculatedMassToCharge(0));
            Assert.AreEqual(3, identifications.ChargeState(0));
            Assert.AreEqual(1, identifications.Count);
            Assert.AreEqual(1134.26091302033 + 0.000001 * 1134.2609130203 + 0.000001, identifications.ExperimentalMassToCharge(0), 1e-10);
            Assert.IsFalse(identifications.IsDecoy(0));
            Assert.AreEqual("UNIMOD:4", identifications.ModificationAcession(0, 0));
            Assert.AreEqual("UNIMOD", identifications.ModificationDictionary(0, 0));
            Assert.AreEqual(17, identifications.ModificationLocation(0, 0));
            Assert.AreEqual("spectrum 2", identifications.Ms2SpectrumID(0));
            Assert.AreEqual(1, identifications.NumModifications(0));
            Assert.AreEqual("GPEAPPPALPAGAPPPCTAVTSDHLNSLLGNILR", identifications.PeptideSequenceWithoutModifications(0));
            Assert.AreEqual(0.1, identifications.ParentTolerance.Value);
            Assert.AreEqual(0.01, identifications.FragmentTolerance.Value);
        }

        [Test]
        public void mzid110Test()
        {
            XmlSerializer _indexedSerializer = new XmlSerializer(typeof(mzIdentML110.Generated.MzIdentMLType));
            var _mzid = new mzIdentML110.Generated.MzIdentMLType();
            _mzid.DataCollection = new mzIdentML110.Generated.DataCollectionType();
            _mzid.DataCollection.AnalysisData = new mzIdentML110.Generated.AnalysisDataType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList = new mzIdentML110.Generated.SpectrumIdentificationListType[1];
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0] = new mzIdentML110.Generated.SpectrumIdentificationListType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult = new mzIdentML110.Generated.SpectrumIdentificationResultType[1];
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0] = new mzIdentML110.Generated.SpectrumIdentificationResultType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].spectrumID = "spectrum 2";
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem = new mzIdentML110.Generated.SpectrumIdentificationItemType[1];
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0] = new mzIdentML110.Generated.SpectrumIdentificationItemType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].experimentalMassToCharge = 1134.2609130203 + 0.000001 * 1134.2609130203 + 0.000001;
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].calculatedMassToCharge = 1134.26091302033;
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].calculatedMassToChargeSpecified = true;
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].chargeState = 3;
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].cvParam = new mzIdentML110.Generated.CVParamType[1];
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].cvParam[0] = new mzIdentML110.Generated.CVParamType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].cvParam[0].value = 100.ToString();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].PeptideEvidenceRef = new mzIdentML110.Generated.PeptideEvidenceRefType[1];
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].PeptideEvidenceRef[0] = new mzIdentML110.Generated.PeptideEvidenceRefType();
            _mzid.DataCollection.AnalysisData.SpectrumIdentificationList[0].SpectrumIdentificationResult[0].SpectrumIdentificationItem[0].PeptideEvidenceRef[0].peptideEvidence_ref = "PE_1";

            _mzid.DataCollection.Inputs = new mzIdentML110.Generated.InputsType();
            _mzid.DataCollection.Inputs.SpectraData = new mzIdentML110.Generated.SpectraDataType[1];
            _mzid.DataCollection.Inputs.SpectraData[0] = new mzIdentML110.Generated.SpectraDataType();
            _mzid.DataCollection.Inputs.SpectraData[0].FileFormat = new mzIdentML110.Generated.FileFormatType();
            _mzid.DataCollection.Inputs.SpectraData[0].FileFormat.cvParam = new mzIdentML110.Generated.CVParamType();
            _mzid.DataCollection.Inputs.SpectraData[0].FileFormat.cvParam.name = "mzML format";

            _mzid.SequenceCollection = new mzIdentML110.Generated.SequenceCollectionType();
            _mzid.SequenceCollection.PeptideEvidence = new mzIdentML110.Generated.PeptideEvidenceType[1];
            _mzid.SequenceCollection.PeptideEvidence[0] = new mzIdentML110.Generated.PeptideEvidenceType();
            _mzid.SequenceCollection.PeptideEvidence[0].isDecoy = false;
            _mzid.SequenceCollection.PeptideEvidence[0].peptide_ref = "P_1";
            _mzid.SequenceCollection.Peptide = new mzIdentML110.Generated.PeptideType[1];
            _mzid.SequenceCollection.Peptide[0] = new mzIdentML110.Generated.PeptideType();
            _mzid.SequenceCollection.Peptide[0].id = "P_1";
            _mzid.SequenceCollection.Peptide[0].PeptideSequence = "GPEAPPPALPAGAPPPCTAVTSDHLNSLLGNILR";
            _mzid.SequenceCollection.Peptide[0].Modification = new mzIdentML110.Generated.ModificationType[1];
            _mzid.SequenceCollection.Peptide[0].Modification[0] = new mzIdentML110.Generated.ModificationType();
            _mzid.SequenceCollection.Peptide[0].Modification[0].locationSpecified = true;
            _mzid.SequenceCollection.Peptide[0].Modification[0].location = 17;
            _mzid.SequenceCollection.Peptide[0].Modification[0].monoisotopicMassDeltaSpecified = true;
            _mzid.SequenceCollection.Peptide[0].Modification[0].monoisotopicMassDelta = 57.02146373;
            _mzid.SequenceCollection.Peptide[0].Modification[0].cvParam = new mzIdentML110.Generated.CVParamType[1];
            _mzid.SequenceCollection.Peptide[0].Modification[0].cvParam[0] = new mzIdentML110.Generated.CVParamType();
            _mzid.SequenceCollection.Peptide[0].Modification[0].cvParam[0].accession = "UNIMOD:4";
            _mzid.SequenceCollection.Peptide[0].Modification[0].cvParam[0].name = "Carbamidomethyl";
            _mzid.SequenceCollection.Peptide[0].Modification[0].cvParam[0].cvRef = "UNIMOD";

            _mzid.AnalysisProtocolCollection = new mzIdentML110.Generated.AnalysisProtocolCollectionType();
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol = new mzIdentML110.Generated.SpectrumIdentificationProtocolType[1];
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0] = new mzIdentML110.Generated.SpectrumIdentificationProtocolType();
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].ParentTolerance = new mzIdentML110.Generated.CVParamType[1];
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].ParentTolerance[0] = new mzIdentML110.Generated.CVParamType();
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].ParentTolerance[0].unitName = "dalton";
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].ParentTolerance[0].value = "0.1";

            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].FragmentTolerance = new mzIdentML110.Generated.CVParamType[1];
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].FragmentTolerance[0] = new mzIdentML110.Generated.CVParamType();
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].FragmentTolerance[0].unitName = "dalton";
            _mzid.AnalysisProtocolCollection.SpectrumIdentificationProtocol[0].FragmentTolerance[0].value = "0.01";

            TextWriter writer = new StreamWriter("myIdentifications.mzid");
            _indexedSerializer.Serialize(writer, _mzid);
            writer.Close();

            var identifications = new MzidIdentifications("myIdentifications.mzid");

            Assert.AreEqual(1134.26091302033, identifications.CalculatedMassToCharge(0));
            Assert.AreEqual(3, identifications.ChargeState(0));
            Assert.AreEqual(1, identifications.Count);
            Assert.AreEqual(1134.26091302033 + 0.000001 * 1134.2609130203 + 0.000001, identifications.ExperimentalMassToCharge(0), 1e-10);
            Assert.IsFalse(identifications.IsDecoy(0));
            Assert.AreEqual("UNIMOD:4", identifications.ModificationAcession(0, 0));
            Assert.AreEqual("UNIMOD", identifications.ModificationDictionary(0, 0));
            Assert.AreEqual(17, identifications.ModificationLocation(0, 0));
            Assert.AreEqual("spectrum 2", identifications.Ms2SpectrumID(0));
            Assert.AreEqual(1, identifications.NumModifications(0));
            Assert.AreEqual("GPEAPPPALPAGAPPPCTAVTSDHLNSLLGNILR", identifications.PeptideSequenceWithoutModifications(0));
            Assert.AreEqual(0.1, identifications.ParentTolerance.Value);
            Assert.AreEqual(0.01, identifications.FragmentTolerance.Value);
        }

        #endregion Public Methods

        #region Private Methods

        private DefaultMzSpectrum createMS2spectrum(IEnumerable<Fragment> fragments, int v1, int v2)
        {
            List<double> allMasses = new List<double>();
            List<double> allIntensities = new List<double>();
            foreach (ChemicalFormulaFragment f in fragments)
            {
                foreach (var p in createSpectrum(f.ThisChemicalFormula, v1, v2, 2))
                {
                    allMasses.Add(p.Mz);
                    allIntensities.Add(p.Intensity);
                }
            }
            var allMassesArray = allMasses.ToArray();
            var allIntensitiessArray = allIntensities.ToArray();

            Array.Sort(allMassesArray, allIntensitiessArray);
            return new DefaultMzSpectrum(allMassesArray, allIntensitiessArray, false);
        }

        private DefaultMzSpectrum createSpectrum(ChemicalFormula f, double lowerBound, double upperBound, int minCharge)
        {
            IsotopicDistribution isodist = new IsotopicDistribution(f, 0.1);

            IMzSpectrum<MzPeak> massSpectrum1 = new DefaultMzSpectrum(isodist.Masses.ToArray(), isodist.Intensities.ToArray(), false);
            massSpectrum1 = massSpectrum1.NewSpectrumFilterByNumberOfMostIntense(5);

            var chargeToLookAt = minCharge;
            var correctedSpectrum = massSpectrum1.NewSpectrumApplyFunctionToX(s => s.ToMassToChargeRatio(chargeToLookAt));

            List<double> allMasses = new List<double>();
            List<double> allIntensitiess = new List<double>();

            while (correctedSpectrum.FirstX > lowerBound)
            {
                foreach (var thisPeak in correctedSpectrum)
                {
                    if (thisPeak.Mz > lowerBound && thisPeak.Mz < upperBound)
                    {
                        allMasses.Add(thisPeak.Mz);
                        allIntensitiess.Add(thisPeak.Intensity);
                    }
                }
                chargeToLookAt += 1;
                correctedSpectrum = massSpectrum1.NewSpectrumApplyFunctionToX(s => s.ToMassToChargeRatio(chargeToLookAt));
            }

            var allMassesArray = allMasses.ToArray();
            var allIntensitiessArray = allIntensitiess.ToArray();

            Array.Sort(allMassesArray, allIntensitiessArray);

            return new DefaultMzSpectrum(allMassesArray, allIntensitiessArray, false);
        }

        #endregion Private Methods

    }
}