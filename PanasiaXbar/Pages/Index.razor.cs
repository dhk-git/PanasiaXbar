//using MongoDB.Bson;
//using MongoDB.Bson.Serialization.Attributes;
//using MongoDB.Driver;
using System.Text.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace PanasiaXbar.Pages
{
    public partial class Index
    {
        //private MongoClient _mdCli = new MongoClient("mongodb://localhost:27017");
        //private IMongoDatabase _mdbTest;
        //private IMongoCollection<TestModel> _mdbCollection;
        private int _subGroupCount = 10;

        // public List<TestModel> TestModels { get; set; }
        public List<XbarRangeModel> XbarRangeModels { get; set; }
        public List<RangeModel> RangeModels { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        protected override void OnInitialized()
        {
            //_mdbTest = _mdCli.GetDatabase("test");
            //_mdbCollection = _mdbTest.GetCollection<TestModel>("testCollection");
            //TestModels = await _mdbCollection.Find(a => true).Limit(50).ToListAsync();
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
            controlModel.XbarMean = subGroupModels.Average(x => x.Xbar);
            controlModel.RangeMean = Math.Round(subGroupModels.Average(x => x.Range), 3);
            controlModel.XbarUcl = Math.Round(controlModel.XbarMean + controlModel.RangeMean * XbarFactor.GetFactor(ColumName.A2, _subGroupCount), 3);
            controlModel.XbarCl = controlModel.XbarMean;
            controlModel.XbarLcl = Math.Round(controlModel.XbarMean - controlModel.RangeMean * XbarFactor.GetFactor(ColumName.A2, _subGroupCount), 3);
            controlModel.RangeUcl = Math.Round(controlModel.RangeMean * XbarFactor.GetFactor(ColumName.D4, _subGroupCount), 3);
            controlModel.RangeCl = controlModel.RangeMean;
            controlModel.RangeLcl = Math.Round(controlModel.RangeMean * XbarFactor.GetFactor(ColumName.D3, _subGroupCount), 3);

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

    public class RangeModel
    {
        public decimal RangeUcl { get; set; }
        public decimal RangeCl { get; set; }
        public decimal RangeLcl { get; set; }
    }

    public static class XbarFactor
    {
        public static decimal GetFactor(ColumName columName, int n)
        {
            switch (columName)
            {
                case ColumName.A2:
                    switch (n)
                    {
                        case 2:
                            return 1.880M;
                        case 3:
                            return 1.023M;
                        case 4:
                            return 0.729M;
                        case 5:
                            return 0.577M;
                        case 6:
                            return 0.483M;
                        case 7:
                            return 0.419M;
                        case 8:
                            return 0.373M;
                        case 9:
                            return 0.337M;
                        case 10:
                            return 0.308M;
                        default:
                            return 0M;
                    }
                case ColumName.D3:
                    switch (n)
                    {
                        case 2:
                            return 0M;
                        case 3:
                            return 0M;
                        case 4:
                            return 0M;
                        case 5:
                            return 0M;
                        case 6:
                            return 0M;
                        case 7:
                            return 0.076M;
                        case 8:
                            return 0.136M;
                        case 9:
                            return 0.184M;
                        case 10:
                            return 0.223M;
                        default:
                            return 0M;
                    }
                case ColumName.D4:
                    switch (n)
                    {
                        case 2:
                            return 3.267M;
                        case 3:
                            return 2.575M;
                        case 4:
                            return 2.282M;
                        case 5:
                            return 2.115M;
                        case 6:
                            return 2.004M;
                        case 7:
                            return 1.924M;
                        case 8:
                            return 1.864M;
                        case 9:
                            return 1.816M;
                        case 10:
                            return 1.777M;
                        default:
                            return 0M;
                    }
                default:
                    return 0M;
            }
        }
    }

    public enum ColumName
    {
        A2,
        D3,
        D4
    }


    //public class TestModel
    //{
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string Id { get; set; }

    //    public string test1 { get; set; }
    //    public string test2 { get; set; }
    //    public string test3 { get; set; }
    //    public string test4 { get; set; }
    //    public string test5 { get; set; }
    //    public string test6 { get; set; }
    //    public string test7 { get; set; }
    //    public string CreateDttm { get; set; }
    //}
}

