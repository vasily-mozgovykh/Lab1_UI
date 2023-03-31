using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SplinesDataStructures;

namespace MainUserInterface
{
    public class ViewData : INotifyPropertyChanged
    {
        #region FIELDS
        /* RawData */
        private double[] LimitsValue { get; set; }
        private int NodesCountValue { get; set; }

        private bool IsUniformValue { get; set; }
        private FRawEnum FunctionNameValue { get; set; }

        /* SplineData */
        public int SplineNodesCount { get; set; }
        public double LeftFirstDerivative { get; set; }
        public double RightFirstDerivative { get; set; }
        
        /* Refs */
        private RawData rawData { get; set; }
        private SplineData splineData { get; set; }
        public List<SplineDataItem> Items { get; set; }
        #endregion

        #region PUBLIC PROPERTIES
        public double[] Limits
        {
            get => LimitsValue;
            set
            {
                if (value != null && (value[0] != LimitsValue[0] || value[1] != LimitsValue[1]))
                {
                    LimitsValue[0] = value[0]; LimitsValue[1] = value[1];
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Limits)));
                }
            }
        }
        public int NodesCount
        {
            get => NodesCountValue;
            set
            {
                if (value != NodesCountValue)
                {
                    NodesCountValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NodesCount)));
                }
            }
        }
        public bool IsUniform
        {
            get => IsUniformValue;
            set
            {
                if (value != IsUniformValue)
                {
                    IsUniformValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUniform)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNonUniform)));
                }
            }
        }
        public bool IsNonUniform
        {
            get => !IsUniformValue;
            set
            {
                if (value != !IsUniformValue)
                {
                    IsUniformValue = !value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUniform)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNonUniform)));
                }
            }
        }
        public FRawEnum FunctionName
        {
            get => FunctionNameValue;
            set
            {
                if (value != FunctionNameValue)
                {
                    FunctionNameValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FunctionName)));
                }
            }
        }
        #endregion

        public ViewData()
        {
            LimitsValue = new double[2] { -1d, 1d };
            NodesCountValue = 3;
            IsUniformValue = true;
            FunctionNameValue = FRawEnum.Linear;
            rawData = new RawData(Limits[0], Limits[1], NodesCountValue, IsUniform, RawData.Linear);

            SplineNodesCount = 5;
            LeftFirstDerivative = 3;
            RightFirstDerivative = 3;
            splineData = new SplineData(rawData, new double[2] { LeftFirstDerivative, RightFirstDerivative }, SplineNodesCount);

            Items = splineData.Items;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        #region INPUT/OUTPUT
        public void Save(string filename)
        {
            RawDataFromControls();
            rawData.Save(filename);
        }

        public void Load(string filename)
        {
            try
            {
                rawData = RawData.Load(filename);
                Limits = new double[2] { rawData.Left, rawData.Right };
                NodesCount = rawData.NodesCount;
                IsUniform = rawData.IsUniform;
                IsNonUniform = !(rawData.IsUniform);
                if (rawData.Function.Method.Name == "Linear")
                    FunctionName = FRawEnum.Linear;
                else if (rawData.Function.Method.Name == "Cubic")
                    FunctionName = FRawEnum.Cubic;
                else
                    FunctionName = FRawEnum.PseudoRandom;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            splineData.RawData = rawData;
            splineData.SplineNodesCount = SplineNodesCount;
            splineData.BoundaryConditions[0] = LeftFirstDerivative;
            splineData.BoundaryConditions[1] = RightFirstDerivative;
        }

        public void RawDataFromControls()
        {
            FRaw function = RawData.Linear;
            switch (FunctionName)
            {
                case FRawEnum.Linear:
                    function = RawData.Linear; break;
                case FRawEnum.Cubic:
                    function = RawData.Cubic; break;
                case FRawEnum.PseudoRandom:
                    function = RawData.PseudoRandom; break;
            }
            rawData = new RawData(Limits[0], Limits[1], NodesCountValue, IsUniform, function);

            splineData.RawData = rawData;
            splineData.SplineNodesCount = SplineNodesCount;
            splineData.BoundaryConditions[0] = LeftFirstDerivative;
            splineData.BoundaryConditions[1] = RightFirstDerivative;
        }
        #endregion
    
        public void CalculateSpline()
        {
            splineData.CalculateSpline();
        }

        public double GetIntegral() => splineData.Integral;

        public (double, double) GetNodeValue(int index) => (rawData.Nodes[index], rawData.Values[index]);
    }
}
