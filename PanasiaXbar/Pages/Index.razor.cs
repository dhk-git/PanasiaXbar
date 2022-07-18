using System.Text.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using PanasiaXbar.Common;

namespace PanasiaXbar.Pages
{
    public partial class Index
    {
        private int _subGroupCount = 10; //서브그룹(측정한 시료의 개수)

        public List<XbarRangeModel> XbarRangeModels { get; set; }
        public decimal? Cp { get; set; } = null;
        public decimal? Cpk { get; set; } = null;
        private decimal _avg;
        private decimal _stddev;


        [Inject]
        public IJSRuntime JS { get; set; }

        protected override void OnInitialized()
        {
            string text = System.IO.File.ReadAllText(@"Data\data.json");
            List<RawDataModel> data = JsonSerializer.Deserialize<List<RawDataModel>>(text);

            //Xbar-R chart 데이터
            XbarRangeModels = GetXbarRangeModels(data);

            //cp/cpk 
            Cp = Math.Round(GetCp(data), 3);
            Cpk = Math.Round(GetCpk(), 3);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            JS.InvokeVoidAsync("AddXbarChart", "xbarChart", XbarRangeModels, "Xbar");
            JS.InvokeVoidAsync("AddRangeChart", "rangeChart", XbarRangeModels, "Range");
        }


        public List<XbarRangeModel> GetXbarRangeModels(List<RawDataModel> data)
        {
            List<SubGroupModel> subGroupModels = new List<SubGroupModel>();
            int dataCount = data.Count;
            //개별 샘플을 10개씩 subGroup으로 처리
            for (int i = 0; i < data.Count; i += _subGroupCount)
            {
                if ((dataCount - i) < _subGroupCount)   //10개 이하로 남은건 버림
                {
                    break;
                }
                var sample = data.GetRange(i, 10);
                //10개에 대한 Xbar, Range(Max - Min)
                subGroupModels.Add(new SubGroupModel { Xbar = sample.Average(x => x.VALUE), Range = sample.Max(x => x.VALUE) - sample.Min(x => x.VALUE) });
            }

            ControlModel controlModel = new ControlModel();
            //xbar mean
            controlModel.XbarMean = subGroupModels.Average(x => x.Xbar);
            //range mean
            controlModel.RangeMean = Math.Round(subGroupModels.Average(x => x.Range), 3);
            //xbar
            controlModel.XbarUcl = Math.Round(controlModel.XbarMean + controlModel.RangeMean * XbarFactor.GetFactor(VariableName.A2, _subGroupCount), 3);
            controlModel.XbarCl = controlModel.XbarMean;
            controlModel.XbarLcl = Math.Round(controlModel.XbarMean - controlModel.RangeMean * XbarFactor.GetFactor(VariableName.A2, _subGroupCount), 3);
            //Range
            controlModel.RangeUcl = Math.Round(controlModel.RangeMean * XbarFactor.GetFactor(VariableName.D4, _subGroupCount), 3);
            controlModel.RangeCl = controlModel.RangeMean;
            controlModel.RangeLcl = Math.Round(controlModel.RangeMean * XbarFactor.GetFactor(VariableName.D3, _subGroupCount), 3);

            List<XbarRangeModel> xbarRangeModels = new List<XbarRangeModel>();
            int subGroupNo = 1;
            foreach (var model in subGroupModels)
            {
                xbarRangeModels.Add(new XbarRangeModel
                {
                    SubGroupNo = subGroupNo,
                    Xbar = model.Xbar,
                    XbarCl = controlModel.XbarCl,
                    XbarUcl = controlModel.XbarUcl,
                    XbarLcl = controlModel.XbarLcl,
                    Range = model.Range,
                    RangeUcl = controlModel.RangeUcl,
                    RangeCl = controlModel.RangeCl,
                    RangeLcl = controlModel.RangeLcl
                });
                subGroupNo++;
            }
            return xbarRangeModels;
        }

        private decimal GetCp(List<RawDataModel> rawDataModels)
        {
            //표준편차
            _avg = rawDataModels.Select(x => x.VALUE).Average();
            var sdSum = rawDataModels.Select(x => (x.VALUE - _avg) * (x.VALUE - _avg)).Sum();
            _stddev = Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(sdSum / rawDataModels.Count)));
            //cp 
            var cp = (Spec.Usl - Spec.Lsl) / (6 * _stddev);
            return cp;
        }

        private decimal GetCpk()
        {
            var val1 = Math.Abs(((Spec.Usl + Spec.Lsl) / 2) - _avg);
            var val2 = (Spec.Usl - Spec.Lsl) / 2;
            var k = val1 / val2; //k(치우침)
            var cpk = (1 - k) * Convert.ToDecimal(Cp);
            return cpk;
        }

    }
    public class RawDataModel
    {
        public string LOT_NO { get; set; }
        public decimal VALUE { get; set; }
    }

    public class SubGroupModel
    {
        public decimal Xbar { get; set; }
        public decimal Range { get; set; }
    }

    public class ControlModel
    {
        public decimal XbarMean { get; set; }
        public decimal RangeMean { get; set; }

        public decimal XbarUcl { get; set; }
        public decimal XbarCl { get; set; }
        public decimal XbarLcl { get; set; }

        public decimal RangeUcl { get; set; }
        public decimal RangeCl { get; set; }
        public decimal RangeLcl { get; set; }
    }

    public class XbarRangeModel
    {
        public int SubGroupNo { get; set; }

        public decimal Xbar { get; set; }
        public decimal XbarUcl { get; set; }
        public decimal XbarCl { get; set; }
        public decimal XbarLcl { get; set; }

        public decimal Range { get; set; }
        public decimal RangeUcl { get; set; }
        public decimal RangeCl { get; set; }
        public decimal RangeLcl { get; set; }
    }

    //cp/cpk 
    public static class Spec
    {
        public static decimal Usl { get; set; } = 3.9M; //임의로 지정함
        public static decimal Lsl { get; set; } = 3.6M; //임의로 지정함
    }

}

