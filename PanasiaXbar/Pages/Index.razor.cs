using System.Text.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using PanasiaXbar.Common;

namespace PanasiaXbar.Pages
{
    public partial class Index
    {
        private int _subGroupCount = 10;

        public List<XbarRangeModel> XbarRangeModels { get; set; }
        

        [Inject]
        public IJSRuntime JS { get; set; }

        protected override void OnInitialized()
        {
            string text = System.IO.File.ReadAllText(@"Data\data.json");
            List<RawDataModel> data = JsonSerializer.Deserialize<List<RawDataModel>>(text);

            XbarRangeModels = GetXbarRangeModels(data);
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
}

