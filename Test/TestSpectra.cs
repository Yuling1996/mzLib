﻿// Copyright 2012, 2013, 2014 Derek J. Bailey
// Modified work Copyright 2016 Stefan Solntsev
//
// This file (TestSpectra.cs) is part of MassSpectrometry.Tests.
//
// MassSpectrometry.Tests is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MassSpectrometry.Tests is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with MassSpectrometry.Tests. If not, see <http://www.gnu.org/licenses/>.

using IO.MzML;
using MzLibUtil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    [TestFixture]
    public sealed class SpectrumTestFixture
    {
        #region Private Fields

        private MzmlMzSpectrum _mzSpectrumA;

        #endregion Private Fields

        #region Public Methods

        [SetUp]
        public void Setup()
        {
            double[] mz = { 328.73795, 329.23935, 447.73849, 448.23987, 482.23792, 482.57089, 482.90393, 500.95358, 501.28732, 501.62131, 611.99377, 612.32806, 612.66187, 722.85217, 723.35345 };
            double[] intensities = { 81007096.0, 28604418.0, 78353512.0, 39291696.0, 122781408.0, 94147520.0, 44238040.0, 71198680.0, 54184096.0, 21975364.0, 44514172.0, 43061628.0, 23599424.0, 56022696.0, 41019144.0 };

            _mzSpectrumA = new MzmlMzSpectrum(mz, intensities, false);
        }

        [Test]
        public void SpectrumCount()
        {
            Assert.AreEqual(15, _mzSpectrumA.Size);
        }

        [Test]
        public void SpectrumFirstMZ()
        {
            Assert.AreEqual(328.73795, _mzSpectrumA.FirstX);
        }

        [Test]
        public void SpectrumLastMZ()
        {
            Assert.AreEqual(723.35345, _mzSpectrumA.LastX);
        }

        [Test]
        public void SpectrumBasePeakIntensity()
        {
            double basePeakIntensity = _mzSpectrumA.YofPeakWithHighestY;

            Assert.AreEqual(122781408.0, basePeakIntensity);
        }

        [Test]
        public void SpectrumTIC()
        {
            double tic = _mzSpectrumA.SumOfAllY;

            Assert.AreEqual(843998894.0, tic);
        }

        [Test]
        public void SpectrumGetIntensityFirst()
        {
            Assert.AreEqual(81007096.0, _mzSpectrumA[0].Intensity);
        }

        [Test]
        public void SpectrumGetIntensityRandom()
        {
            Assert.AreEqual(44238040.0, _mzSpectrumA[6].Intensity);
        }

        [Test]
        public void SpectrumGetMassFirst()
        {
            Assert.AreEqual(328.73795, _mzSpectrumA.FirstX);
        }

        [Test]
        public void SpectrumGetMassRandom()
        {
            Assert.AreEqual(482.90393, _mzSpectrumA[6].Mz);
        }

        [Test]
        public void SpectrumContainsPeak()
        {
            Assert.IsTrue(_mzSpectrumA.Size > 0);
        }

        [Test]
        public void SpectrumContainsPeakInRange()
        {
            Assert.AreEqual(1, _mzSpectrumA.NumPeaksWithinRange(448.23987 - 0.001, 448.23987 + 0.001));
        }

        [Test]
        public void SpectrumContainsPeakInRangeEnd()
        {
            Assert.AreEqual(0, _mzSpectrumA.NumPeaksWithinRange(448.23987 - 0.001, 448.23987));
        }

        [Test]
        public void SpectrumContainsPeakInRangeStart()
        {
            Assert.AreEqual(1, _mzSpectrumA.NumPeaksWithinRange(448.23987, 448.23987 + 0.001));
        }

        [Test]
        public void SpectrumContainsPeakInRangeStartEnd()
        {
            Assert.AreEqual(0, _mzSpectrumA.NumPeaksWithinRange(448.23987, 448.23987));
        }

        [Test]
        public void SpectrumDoesntContainPeakInRange()
        {
            Assert.AreEqual(0, _mzSpectrumA.NumPeaksWithinRange(603.4243 - 0.001, 603.4243 + 0.001));
        }

        [Test]
        public void SpectrumMassRange()
        {
            MzRange range = new MzRange(328.73795, 723.35345);

            Assert.AreEqual(0, _mzSpectrumA.Range.Minimum - range.Minimum, 1e-9);
            Assert.AreEqual(0, _mzSpectrumA.Range.Maximum - range.Maximum, 1e-9);
        }

        [Test]
        public void SpectrumFilterCount()
        {
            var filteredMzSpectrum = _mzSpectrumA.FilterByY(28604417, 28604419);

            Assert.AreEqual(1, filteredMzSpectrum.Count());
        }

        [Test]
        public void FilterByNumberOfMostIntenseTest()
        {
            Assert.AreEqual(5, _mzSpectrumA.FilterByNumberOfMostIntense(5).Count());
        }

        [Test]
        public void GetBasePeak()
        {
            Assert.AreEqual(122781408.0, _mzSpectrumA.PeakWithHighestY.Intensity);
        }

        [Test]
        public void GetClosestPeak()
        {
            Assert.AreEqual(448.23987, _mzSpectrumA.GetClosestPeak(448).Mz);
            Assert.AreEqual(447.73849, _mzSpectrumA.GetClosestPeak(447.9).Mz);
        }

        [Test]
        public void Extract()
        {
            Assert.AreEqual(3, _mzSpectrumA.Extract(500, 600).Count());
        }

        [Test]
        public void CorrectOrder()
        {
            _mzSpectrumA = new MzmlMzSpectrum(new double[] { 5, 6, 7 }, new double[] { 1, 2, 3 }, false);
            Assert.IsTrue(_mzSpectrumA.FilterByNumberOfMostIntense(2).First().Mz < _mzSpectrumA.FilterByNumberOfMostIntense(2).ToList()[1].Mz);
        }

        [Test]
        public void TestFunctionToX()
        {
            _mzSpectrumA.ReplaceXbyApplyingFunction(b => -1);
            Assert.AreEqual(-1, _mzSpectrumA[0].X);
        }

        [Test]
        public void TestGetClosestPeakXValue()
        {
            Assert.AreEqual(447.73849, _mzSpectrumA.GetClosestPeakXvalue(447.73849));
            Assert.AreEqual(447.73849, _mzSpectrumA.GetClosestPeakXvalue(447));
            Assert.Throws<IndexOutOfRangeException>(() => { new MzmlMzSpectrum(new double[0], new double[0], false).GetClosestPeakXvalue(1); }, "No peaks in spectrum!");
        }

        //[Test]
        //public void IMzSpectrumTpeakTest()
        //{
        //    //double[] mz = { 328.73795, 329.23935, 447.73849, 448.23987, 482.23792, 482.57089, 482.90393, 500.95358, 501.28732, 501.62131, 611.99377, 612.32806, 612.66187, 722.85217, 723.35345 };
        //    //double[] intensities = { 81007096.0, 28604418.0, 78353512.0, 39291696.0, 122781408.0, 94147520.0, 44238040.0, 71198680.0, 54184096.0, 21975364.0, 44514172.0, 43061628.0, 23599424.0, 56022696.0, 41019144.0 };

        //    IMzSpectrum<MzmlPeak> ok = _mzSpectrumA;

        //    Assert.Greater(100, ok.ApplyFunctionToX(b => b / 10).LastX);

        //    Assert.AreEqual(1, ok.Extract(new DoubleRange(723, 1000)).Size);

        //    Assert.AreEqual(15, ok.Extract(double.MinValue, double.MaxValue).Size);

        //    Assert.AreEqual(0, ok.FilterByNumberOfMostIntense(0).Size);
        //    Assert.AreEqual(5, ok.FilterByNumberOfMostIntense(5).Size);
        //    Assert.AreEqual(15, ok.FilterByNumberOfMostIntense(15).Size);

        //    Assert.AreEqual(15, ok.FilterByY(new DoubleRange(double.MinValue, double.MaxValue)).Size);

        //    Assert.AreEqual(1, ok.FilterByY(39291695, 39291697).Size);

        //    Assert.AreEqual(2, ok.WithRangeRemoved(new DoubleRange(329, 723)).Size);

        //    Assert.AreEqual(15, ok.WithRangeRemoved(0, 1).Size);

        //    Assert.AreEqual(2, ok.WithRangesRemoved(new List<DoubleRange> { new DoubleRange(329, 400), new DoubleRange(400, 723) }).Size);
        //}

        [Test]
        public void TestNumPeaksWithinRange()
        {
            double[] xArray = { 1, 2, 3, 4, 5, 6, 7 };
            double[] yArray = { 1, 2, 1, 5, 1, 2, 1 };

            var thisSpectrum = new MzmlMzSpectrum(xArray, yArray, false);

            Assert.AreEqual(7, thisSpectrum.NumPeaksWithinRange(double.MinValue, double.MaxValue));

            Assert.AreEqual(6, thisSpectrum.NumPeaksWithinRange(1, 7));

            Assert.AreEqual(0, thisSpectrum.NumPeaksWithinRange(1, 1));

            Assert.AreEqual(1, thisSpectrum.NumPeaksWithinRange(1, 2));

            Assert.AreEqual(2, thisSpectrum.NumPeaksWithinRange(0.001, 2.999));

            Assert.AreEqual(1, thisSpectrum.NumPeaksWithinRange(0, 1.5));

            Assert.AreEqual(1, thisSpectrum.NumPeaksWithinRange(6.5, 8));

            Assert.AreEqual(2, thisSpectrum.NumPeaksWithinRange(3, 5));

            Assert.AreEqual(2, thisSpectrum.NumPeaksWithinRange(3.5, 5.5));

            Assert.AreEqual(1, thisSpectrum.NumPeaksWithinRange(7, 8));

            Assert.AreEqual(0, thisSpectrum.NumPeaksWithinRange(8, 9));

            Assert.AreEqual(0, thisSpectrum.NumPeaksWithinRange(-2, -1));

            Assert.AreEqual("[1 to 7] m/z (Peaks 7)", thisSpectrum.ToString());

            //Assert.AreEqual(7, thisSpectrum.FilterByNumberOfMostIntense(7).Size);
            //Assert.AreEqual(1, thisSpectrum.FilterByNumberOfMostIntense(1).Size);
            //Assert.AreEqual(4, thisSpectrum.FilterByNumberOfMostIntense(1).FirstX);

            //Assert.AreEqual(2, thisSpectrum.FilterByNumberOfMostIntense(3).FirstX);

            //Assert.AreEqual(0, thisSpectrum.FilterByNumberOfMostIntense(0).Size);

            //Assert.AreEqual(2, thisSpectrum.WithRangeRemoved(2, 6).Size);
            //Assert.AreEqual(0, thisSpectrum.WithRangeRemoved(0, 100).Size);

            //Assert.AreEqual(6, thisSpectrum.WithRangeRemoved(7, 100).Size);

            //Assert.AreEqual(1, thisSpectrum.WithRangeRemoved(new DoubleRange(double.MinValue, 6)).Size);

            List<DoubleRange> xRanges = new List<DoubleRange>
            {
                new DoubleRange(2, 5),
                new DoubleRange(3, 6)
            };
            //Assert.AreEqual(2, thisSpectrum.WithRangesRemoved(xRanges).Size);

            //Assert.AreEqual(3, thisSpectrum.Extract(new DoubleRange(4.5, 10)).Size);

            //Assert.AreEqual(2, thisSpectrum.FilterByY(new DoubleRange(1.5, 2.5)).Size);

            //Assert.AreEqual(3, thisSpectrum.FilterByY(1.5, double.MaxValue).Size);

            //Assert.AreEqual(2, thisSpectrum.ApplyFunctionToX(b => b * 2).FirstX);

            Assert.AreEqual(1, thisSpectrum.GetClosestPeak(-100).X);

            Assert.AreEqual(7, thisSpectrum.GetClosestPeak(6.6).X);

            Assert.AreEqual(7, thisSpectrum.GetClosestPeak(7).X);

            Assert.AreEqual(7, thisSpectrum.GetClosestPeak(8).X);
        }

        #endregion Public Methods
    }
}