namespace PanasiaXbar.Common
{
    public static class XbarFactor
    {
        public static decimal GetFactor(VariableName variableName, int n)
        {
            switch (variableName)
            {
                case VariableName.A2:
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
                case VariableName.D3:
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
                case VariableName.D4:
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

    public enum VariableName
    {
        A2,
        D3,
        D4
    }
}
